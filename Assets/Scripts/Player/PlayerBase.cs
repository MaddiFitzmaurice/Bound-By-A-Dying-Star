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
    [SerializeField] protected float RotationSpeed = 2f;

    // Rift Data
    [Header("Rift Data")]
    [SerializeField] protected float DistanceInFront = 2f;

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
    [field:SerializeField] protected ParticleSystem _flameHeadPS;


    #endregion

    #region INTERNAL DATA
    // Components
    private Rigidbody _rb;
    private MeshRenderer _meshRenderer;

    // Data
    protected RiftData RiftData;
    protected Vector3 MoveInput;
    protected Vector3 PrevMoveInput;
    protected float PlayerZAngle;

    // Interactables
    List<Collider> _interactablesInRange;
    List<Collider> _interactablesNotInRange;
    Collider _closestInteractable;

    // TEST
    private ControlType _controlType = ControlType.FIXEDCAM2;
    private float _camYAngle;
    private float _prevCamYAngle;
    #endregion

    void Awake()
    {
        // Set components
        _rb = GetComponent<Rigidbody>();
        _meshRenderer = GetComponent<MeshRenderer>();

        // Set data
        RiftData = new RiftData(transform.position, transform.rotation, tag);
        _interactablesInRange = new List<Collider>();
        _interactablesNotInRange = new List<Collider>();

        PlayerZAngle = transform.rotation.eulerAngles.z;

        // Set step height data
        // Note: Height calc will change depending on height/transform centre of models artists settle on
        // Current transform centre: 1 unit
        UpperBoundRay.transform.localPosition = new Vector3(0, -1 + StepHeight, 0);
        LowerBoundRay.transform.localPosition = new Vector3(0, -1, 0);

        _flameHeadPS.Play();
    }

    protected virtual void OnEnable()
    {
        EventManager.EventSubscribe(EventType.TEST_CONTROLS, TestControlHandler);
        EventManager.EventSubscribe(EventType.CLEARSHOT_CAMS_YROT, ReceiveNewCamAngle);
    }

    protected virtual void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.TEST_CONTROLS, TestControlHandler);
        EventManager.EventUnsubscribe(EventType.CLEARSHOT_CAMS_YROT, ReceiveNewCamAngle);
    }

    private void Update()
    {
        CheckInteract();
        PlayerRotation();
    }

    private void FixedUpdate()
    {
        PlayerMovement();
        StepClimb();
    }

    #region MOVEMENT FUNCTIONS
    // Movement
    public void PlayerMovement()
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

        if (_controlType == ControlType.TANK)
        {
            _rb.AddForce(movementForce * transform.forward * MoveInput.z);
        }
        else if (_controlType == ControlType.WORLDSTRAFE)
        {
            _rb.AddForce(movementForce * MoveInput);
        }
        else if (_controlType == ControlType.FIXEDCAM || _controlType == ControlType.FIXEDCAM2)
        {
            _rb.AddForce(movementForce * transform.forward * MoveInput.magnitude);
        }
    }

    public void PlayerRotation()
    {
        if (_controlType == ControlType.TANK)
        {
            Vector3 rotVector = new Vector3(0, MoveInput.x, PlayerZAngle);

            Quaternion playerRotChange = Quaternion.Euler(rotVector * Time.deltaTime * RotationSpeed);

            _rb.MoveRotation(_rb.rotation * playerRotChange);
        }
        else if (_controlType == ControlType.WORLDSTRAFE)
        {
            if (MoveInput.magnitude != 0)
            {
                if (PlayerZAngle == 0)
                {
                    _rb.MoveRotation(Quaternion.LookRotation(MoveInput, Vector3.up));
                }
                else if (PlayerZAngle == -180)
                {
                    _rb.MoveRotation(Quaternion.LookRotation(MoveInput, Vector3.down));
                }
            }
        }
        else if (_controlType == ControlType.FIXEDCAM)
        {
            if (MoveInput.magnitude != 0)
            {
                var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, _camYAngle, 0));
                var skewedInput = matrix.MultiplyPoint3x4(MoveInput);

                var relative = (transform.position + skewedInput) - transform.position;
                var rot = Quaternion.LookRotation(relative, Vector3.up);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, RotationSpeed * Time.deltaTime);
            }
        }
        else if (_controlType == ControlType.FIXEDCAM2)
        {
            if (PrevMoveInput != MoveInput)
            {
                _prevCamYAngle = _camYAngle;
            }

            if (MoveInput.magnitude != 0)
            {
                var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, _prevCamYAngle, 0));
                var skewedInput = matrix.MultiplyPoint3x4(MoveInput);

                _rb.MoveRotation(Quaternion.LookRotation(skewedInput, Vector3.up));
            }
        }
    }
    
    public void StepClimb()
    {
        RaycastHit lowerBoundHit;
        if (Physics.Raycast(LowerBoundRay.transform.position, transform.forward, out lowerBoundHit, 0.8f))
        {
            RaycastHit upperBoundHit;
            if (!Physics.Raycast(UpperBoundRay.transform.position, transform.forward, out upperBoundHit, 0.9f))
            {
                _rb.position -= new Vector3(0f, -StepSmoothing * Time.deltaTime, 0f);
            }
        }
    }
    
    #endregion

    public void PlayTeleportEffect(bool mode)
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
            _meshRenderer.enabled = true;
            _flameHeadPS.Play();
        }
        else
        {
            _meshRenderer.enabled = false;
            _flameHeadPS.Stop();
        }
    }

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
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, 1f, LayerMask.GetMask("Interactables"));
        List<Collider> colliders = colliderArray.ToList();

        foreach (var collider in colliders)
        {
            IInteractable interactable = collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                // If closest interactable hasn't been assigned yet, assign first one in found collider list
                // Make sure locked interactable is not included as closest interactable
                if (!interactable.InteractLocked)
                {
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
            foreach (var interactable in _interactablesInRange)
            {
                if (!colliders.Contains(interactable))
                {
                    _interactablesNotInRange.Add(interactable);

                    if (interactable == _closestInteractable)
                    {
                        interactable.GetComponentInParent<IInteractable>().PlayerNotInRange(this);
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

    #region RIFT FUNCTIONS
    protected void CreatePortalInFrontOfPlayer(object data)
    {
        RiftData.Position = transform.position + transform.forward * DistanceInFront;
        RiftData.Rotation = Quaternion.LookRotation(transform.forward);

        EventManager.EventTrigger(EventType.CREATE_RIFT, RiftData);
    }
    #endregion

    #region EVENT HANDLERS
    public void TestControlHandler(object data)
    {
        if (data is not ControlType)
        {
            Debug.LogError("TestControlHandler has not received a ControlType");
        }

        _controlType = (ControlType)data;
    }

    public void ReceiveNewCamAngle(object data)
    {
        if (data is not float)
        {
            Debug.LogError("PlayerBase has not received a float!");
        }

        PrevMoveInput = MoveInput; // Record previous Move Input so it can continue until player input changes
        _prevCamYAngle = _camYAngle; // Record previous CamY so it can continue until player input changes
        _camYAngle = (float)data;
    }
    #endregion
}
