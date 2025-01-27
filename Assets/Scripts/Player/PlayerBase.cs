using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.VFX;

public abstract class PlayerBase : MonoBehaviour
{
    #region EXTERNAL DATA
    // Movement
    [Header("Movement")]
    [SerializeField] protected float MoveSpeed = 5f;
    [SerializeField] protected float MoveAccel = 3f;
    [SerializeField] protected float MoveDecel = 2f;
    [SerializeField] protected float VelocityPower = 2f;

    [Header("Stairs")]
    [SerializeField] protected float StepHeight;
    [SerializeField] protected float StepSmoothing;
    [SerializeField] protected GameObject UpperBoundRay;
    [SerializeField] protected GameObject LowerBoundRay;

    // Rotation
    [Header("Rotation")]
    [SerializeField] protected float RotationSpeed = 100f;

    // Rift Data
    [Header("Rift Data")]
    [SerializeField] protected float DistanceInFront = 2f;

    // Cloth Data
    [Header("Cloth")]
    [SerializeField] protected float ClothMovementAccel = 1f;

    // Item Data
    [field:Header("Interactable Data")]
    [field:SerializeField] public Material HighlightMat { get; private set; } // Temp material to highlight object to be interacted with
    [field:SerializeField] public Transform PickupPoint { get; private set; } // Assign the pick up location of the object in the Inspector
    [field:SerializeField] public GameObject CarriedPickupable { get; private set; } = null;

    // Effects
    [field:Header("Effects Data")]
    [field:SerializeField] protected VisualEffect _teleportInEffect;
    [field:SerializeField] protected VisualEffect _teleportOutEffect;
    [field:SerializeField] protected VisualEffect _flashEffect;
    #endregion

    #region INTERNAL DATA
    // Components
    private Rigidbody _rb;
    private List<Renderer> _renderers;
    private Cloth _clothPhysics;
    private Animator _animator;

    // Default Parent Info
    private Transform _playerGrouper;

    // Movement Data
    protected Vector3 MoveInput;
    protected Vector3 PrevMoveInput;
    protected InteractTypes MoveType;
    protected float PlayerZAngle;
    protected bool FacingMoveDir = false;
    protected bool _canMove = true;
    private Vector3 _orientation = Vector3.up;
    private LayerMask _playerLayer;

    // Camera Frustum Data
    protected bool IsOffscreen = false;
    protected Plane ClosestPlane;

    // Clothing Data
    private Vector3 _clothExternalAccel;
    private Vector3 _clothAccelGravityMod = new Vector3(0f, 19.62f, 0f); // When player is upside down, cloth accel reverses gravity
    private Vector3 _clothMoveDir = new Vector3(1f, 0f, 1f); // Wind movement

    // Interactables
    List<Collider> _interactablesInRange;
    List<Collider> _interactablesNotInRange;
    Collider _closestInteractable;

    // Camera Data
    private GameObject _currentCam;
    private GameObject _prevCam;
    //private float _camYAngle;
    //private float _prevCamYAngle;
    #endregion

    #region FRAMEWORK FUNCTIONS
    void Awake()
    {
        // Set components
        _rb = GetComponent<Rigidbody>();
        _renderers = new List<Renderer>();
        _renderers.AddRange(GetComponentsInChildren<MeshRenderer>().ToList<Renderer>());
        _renderers.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>().ToList<Renderer>());
        _clothPhysics = GetComponentInChildren<Cloth>();
        _animator = GetComponentInChildren<Animator>();

        // Store parent so it can return to this parent after being temporarily parented
        _playerGrouper = this.transform.parent;

        // Set data
        _animator.SetBool("IsRunning", false);
        _interactablesInRange = new List<Collider>();
        _interactablesNotInRange = new List<Collider>();
        _playerLayer = LayerMask.GetMask("Player");

        PlayerZAngle = transform.rotation.eulerAngles.z;

        // Set step height data
        UpperBoundRay.transform.localPosition = new Vector3(0, StepHeight, 0);
        LowerBoundRay.transform.localPosition = new Vector3(0, 0.03f, 0);
    }

    protected virtual void OnEnable()
    {
        EventManager.EventSubscribe(EventType.CAMERA_NEW_FWD_DIR, ReceiveNewCamAngle);
        EventManager.EventSubscribe(EventType.GRAVITY_INVERT, GravityChangeHandler);
        EventManager.EventSubscribe(EventType.CAN_MOVE, CanMoveHandler);
        EventManager.EventSubscribe(EventType.RESET_CLOTH_PHYS, ResetClothPhys);
    }

    protected virtual void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.CAMERA_NEW_FWD_DIR, ReceiveNewCamAngle);
        EventManager.EventUnsubscribe(EventType.GRAVITY_INVERT, GravityChangeHandler);
        EventManager.EventUnsubscribe(EventType.CAN_MOVE, CanMoveHandler);
        EventManager.EventUnsubscribe(EventType.RESET_CLOTH_PHYS, ResetClothPhys);
    }

    private void Update()
    {
        CheckInteract();
        PlayerRotation();
    }

    private void FixedUpdate()
    {
        PlayerMovement();
        ClothMovement();
        StepClimb();
    }
    #endregion

    #region MOVEMENT FUNCTIONS
    // Movement
    public void PlayerMovement()
    {
        if (_canMove)
        {
            // Calculate desired velocity
            float targetVelocity = MoveInput.magnitude * MoveSpeed;

            // Find diff between desired velocity and current velocity
            float velocityDif = targetVelocity - _rb.velocity.magnitude;

            // Check whether to accel or deccel
            float accelRate = (Mathf.Abs(targetVelocity) > 0.01f) ? MoveAccel :
                MoveDecel;

            // Calc force by multiplying accel and velocity diff, and applying velocity power
            float movementForce = Mathf.Pow(Mathf.Abs(velocityDif) * accelRate, VelocityPower)
                * Mathf.Sign(velocityDif);

            if (FacingMoveDir)
            {
                // If is onscreen, move normally
                if (!IsOffscreen)
                {
                    _rb.AddForce(movementForce * transform.forward * MoveInput.magnitude);
                }
                // Else if offscreen, restrict movement
                else
                {
                    // Update plane normal and 0 out y component (players don't walk in Y dir)
                    Vector3 closestPlaneNormal = ClosestPlane.normal;
                    closestPlaneNormal.y = 0f;

                    // Make sure player only moves either less than perpendicular to plane's normal or walks back to be onscreen
                    if (Vector3.Dot(transform.forward, closestPlaneNormal.normalized) >= 0.2f)
                    {
                        _rb.AddForce(movementForce * transform.forward * MoveInput.magnitude);
                    }
                }
            }

            // Update Animator
            PlayerAnimation();
        }
    }

    public void PlayerRotation()
    {
        if (_canMove)
        {
            // If enough of a cam change has occurred to change the current cam
            if (Vector3.Dot(PrevMoveInput, MoveInput) < 0.85f)
            {
                _prevCam = _currentCam;
            }

            if (MoveInput.magnitude != 0)
            {
                var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, _prevCam.transform.eulerAngles.y, 0));
                var skewedInput = matrix.MultiplyPoint3x4(MoveInput);

                if (Vector3.Dot(transform.forward, skewedInput) > 0.99f)
                {
                    FacingMoveDir = true;
                }
                else
                {
                    FacingMoveDir = false;
                    Vector3 turnDir = Vector3.RotateTowards(transform.forward, skewedInput, RotationSpeed * Time.deltaTime, 1f);
                    transform.rotation = Quaternion.LookRotation(turnDir, _orientation);
                }
            }
        }
    }

    public void StepClimb()
    {
        if (Physics.Raycast(LowerBoundRay.transform.position, transform.forward, 0.3f))
        {
            if (!Physics.Raycast(UpperBoundRay.transform.position, transform.TransformDirection(Vector3.forward), 0.4f, _playerLayer))
            {
                _rb.position -= new Vector3(0f, -StepSmoothing * Time.deltaTime, 0f);
            }
        }
    }
    
    #endregion

    // Player character animation
    private void PlayerAnimation()
    {
        if (MoveInput.magnitude != 0)
        {
            _animator.SetBool("IsRunning", true);
        }
        else
        {
            _animator.SetBool("IsRunning", false);
        }
    }

   

    private void ClothMovement()
    {
        _clothExternalAccel = ClothMovementAccel * Mathf.Sin(Time.fixedTime) * _clothMoveDir;

        // If movement is inverted
        if (_orientation == Vector3.down)
        {
            _clothPhysics.externalAcceleration = _clothExternalAccel + _clothAccelGravityMod;
        }
        else
        {
            _clothPhysics.externalAcceleration = _clothExternalAccel;
        }
    }

    public void PlayTeleportOutEffect()
    {
        _teleportOutEffect.Play();
    }

    public void PlayTeleportInEffect()
    {
        _teleportInEffect.Play();
    }

    public void PlayTeleportOutEffect(bool mode)
    {
        if (mode)
        {
            _teleportInEffect.Play();
        }
        else
        {
            _teleportOutEffect.Play();
        }
    }

    public void PlayFlashEffect()
    {
        _flashEffect.Play();
    }

    public void ToggleVisibility(bool mode)
    {
        if (mode)
        {
            foreach (Renderer renderer in _renderers)
            {
                renderer.enabled = true;
            }
        }
        else
        {
            foreach (Renderer renderer in _renderers)
            {
                renderer.enabled = false;
            }
        }
    }

    public void ToggleClothPhysics(bool toggle)
    {
        _clothPhysics.enabled = toggle;
    }

    #region POSITION MANIPULATION
    public void Respawn(Transform respawnPoint)
    {
        StopAllCoroutines();
        StartCoroutine(RespawnSequence(respawnPoint));
    }

    private IEnumerator RespawnSequence(Transform spawnPoint)
    {
        EventManager.EventTrigger(EventType.DISABLE_GAMEPLAY_INPUTS, this);
        PlayTeleportOutEffect();
        yield return new WaitForSeconds(_teleportOutEffect.GetFloat("EffectLifetime"));
        PlayFlashEffect();
        ToggleVisibility(false);
        ToggleClothPhysics(false);
        yield return new WaitForSeconds(1f);
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        PlayTeleportInEffect();
        yield return new WaitForSeconds(_teleportInEffect.GetFloat("EffectLifetime"));
        PlayFlashEffect();
        ToggleClothPhysics(true);
        ToggleVisibility(true);
        EventManager.EventTrigger(EventType.ENABLE_GAMEPLAY_INPUTS, this);
    }

    public void PuzzleTeleport(Transform teleportPoint)
    {
        EventManager.EventTrigger(EventType.DISABLE_GAMEPLAY_INPUTS, this);
        StopAllCoroutines();
        StartCoroutine(PuzzleTeleportSequence(teleportPoint));
    }

    private IEnumerator PuzzleTeleportSequence(Transform spawnPoint)
    {
        PlayTeleportOutEffect();
        yield return new WaitForSeconds(_teleportOutEffect.GetFloat("EffectLifetime"));
        PlayFlashEffect();
        ToggleVisibility(false);
        ToggleClothPhysics(false);
        yield return new WaitForSeconds(0.1f);
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
        PlayTeleportInEffect();
        yield return new WaitForSeconds(_teleportInEffect.GetFloat("EffectLifetime"));
        PlayFlashEffect();
        ToggleClothPhysics(true);
        ToggleVisibility(true);
    }

    public void DefaultParent()
    {
        transform.parent = _playerGrouper;
    }
    #endregion

    #region INTERACTION FUNCTIONS
    // Interaction
    public void Interact(object data)
    {
        if (data == null)
        {
            Debug.LogError("PlayerBase has not received an InteractType!");
        }

        InteractTypes iType = (InteractTypes)data;

        // If carrying an item and no objects are nearby
        if (CarriedPickupable != null && _closestInteractable == null)
        {
            CarriedPickupable.GetComponent<IInteractable>().PlayerStartInteract(this);
        }
        // If interactable is nearby
        else if (_closestInteractable != null)
        {
            IInteractable interactable = _closestInteractable.GetComponentInParent<IInteractable>();

            // If not currently carrying item
            if (CarriedPickupable == null)
            {
                _closestInteractable = null;
            }

            interactable.PlayerNotInRange(this);

            if (iType == InteractTypes.HOLD)
            {
                interactable.PlayerHoldInteract(this);
            }
            else if (iType == InteractTypes.PRESS)
            {
                interactable.PlayerStartInteract(this);
            }
            else if (iType == InteractTypes.RELEASE_HOLD)
            {
                interactable.PlayerReleaseHoldInteract(this);
            }
        }
    }

    public void CheckInteract()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, 1.5f, LayerMask.GetMask("Interactables", "HighlightInteract"));
        List<Collider> colliders = colliderArray.ToList();

        foreach (var collider in colliders)
        {
            IInteractable interactable = collider.GetComponentInParent<IInteractable>();
            // Only check colliders that have the IInteractable script
            if (interactable != null)
            {
                // Make sure locked interactable is not included as closest interactable
                if (!interactable.InteractLocked)
                {
                    // If closest interactable hasn't been assigned yet, assign first one in found collider list
                    // Make sure picked up interactable is not included again as a closest interactable
                    if (_closestInteractable == null && collider.transform.parent.gameObject != CarriedPickupable)
                    {
                        _closestInteractable = collider;
                    }
                    else
                    {
                        if (collider.transform.parent.gameObject != CarriedPickupable)
                        {
                            if (Vector3.Distance(collider.transform.position, transform.position) <
                                Vector3.Distance(_closestInteractable.transform.position, transform.position))
                            {
                                // Unhighlight previous closest interactable then assign new closest one
                                _closestInteractable.GetComponentInParent<IInteractable>().PlayerNotInRange(this);
                                _closestInteractable = collider;
                            }
                        }
                    }
                }
            }
        }

        // If interactable is no longer in range, add to another list to be deleted safely
        if (_interactablesInRange.Count != 0)
        {
            foreach (var interactableCollider in _interactablesInRange)
            {
                if (!colliders.Contains(interactableCollider))
                {
                    _interactablesNotInRange.Add(interactableCollider);

                    if (interactableCollider == _closestInteractable)
                    {
                        interactableCollider.GetComponentInParent<IInteractable>().PlayerNotInRange(this);
                        _closestInteractable = null;
                    }
                }
            }
        }

        // Remove interactables that are no longer in range
        if (_interactablesNotInRange.Count != 0)
        {
            foreach (var interactable in _interactablesNotInRange)
            {
                _interactablesInRange.Remove(interactable);
            }
            _interactablesNotInRange.Clear();
        }

        // Add recently detected colliders to list
        _interactablesInRange.AddRange(colliders);

        // Highlight closest interactable to player
        if (_closestInteractable != null)
        {
            _closestInteractable.GetComponentInParent<IInteractable>().PlayerInRange(this);
        }
    }

    public void DropItem()
    {
        CarriedPickupable = null;
    }    

    public void PickupItem(GameObject item)
    {
        CarriedPickupable = item;
    }
    #endregion

    #region EVENT HANDLERS
    public void CanMoveHandler(object data)
    {
        if (data is bool canMove)
        {
            _canMove = canMove;
        }
        else
        {
            Debug.LogError("Did not receive a bool!");
        }
    }

    public void OffScreenHandler(object data)
    {
        // If receiving just a bool, means player is back on screen
        if (data is bool isOffscreen)
        {
            IsOffscreen = isOffscreen;
        }
        // If receiving a tuple, player has gone offscreen and will have a closest frustum plane to reference
        else if (data is Tuple<bool, Plane> tuple)
        {
            IsOffscreen = tuple.Item1;
            ClosestPlane = tuple.Item2;
        }
        else
        {
            Debug.LogError("Did not receive a bool or tuple<bool, Plane>!");
        }
    }

    public void GravityChangeHandler(object data) 
    {
        if (data is bool isInverted)
        {
            // If movement is inverted, change orientation for movement and gravity for cloth
            if (isInverted)
            {
                _orientation = Vector3.down;
                //_clothPhysics.externalAcceleration = _clothExternalAccel + _clothAccelGravityMod;
            }
            else
            {
                _orientation = Vector3.up;
                //_clothPhysics.externalAcceleration = _clothExternalAccel;
            }
        }
        else
        {
            Debug.LogError("GravityChangeHandler did not receive a bool");
        }
    }

    public void ReceiveNewCamAngle(object data)
    {
        if (data is not GameObject)
        {
            Debug.LogError("PlayerBase has not received a camera GameObject!");
        }

        PrevMoveInput = MoveInput; // Record previous Move Input so it can continue until player input changes
        _prevCam = _currentCam; // Record previous CamY so it can continue until player input changes
        _currentCam = (GameObject)data;
    }

    public void ResetClothPhys(object data)
    {
        ToggleVisibility(false);
        ToggleClothPhysics(false);
        ToggleVisibility(true);
        ToggleClothPhysics(true);
    }
    #endregion
}
