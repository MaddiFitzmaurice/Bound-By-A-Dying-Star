using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class PlayerBase : MonoBehaviour
{
    #region EXTERNAL DATA
    // Movement
    [Header("Movement")]
    [SerializeField] protected float MoveSpeed = 5f;
    [SerializeField] protected float MoveAccel = 3f;
    [SerializeField] protected float MoveDecel = 2f;
    [SerializeField] protected float VelocityPower = 2f;

    // Rotation
    [SerializeField] protected float RotationSpeed = 2f;

    // Rift Data
    [Header("Rift Data")]
    [SerializeField] protected float DistanceInFront = 2f;

    // Item Data
    [field:Header("Interactable Data")]
    [field:SerializeField] public Material HighlightMat { get; private set; } // Temp material to highlight object to be interacted with
    [field:SerializeField] public Transform PickupPoint { get; private set; } // Assign the pick up location of the object in the Inspector
    [field:SerializeField] public GameObject CarriedPickupable { get; private set; } = null;
    #endregion

    #region INTERNAL DATA
    // Components
    private Rigidbody _rb;

    // Data
    protected RiftData RiftData;
    protected Vector3 MoveInput;
    protected float PlayerZAngle;

    // Interactables
    List<Collider> _interactablesInRange;
    List<Collider> _interactablesNotInRange;
    Collider _closestInteractable;

    // TEST
    private ControlType _controlType = ControlType.WORLDSTRAFE;
    private float _camYAngle;
    #endregion

    void Awake()
    {
        // Set components
        _rb = GetComponent<Rigidbody>();

        // Set data
        RiftData = new RiftData(transform.position, transform.rotation, tag);
        _interactablesInRange = new List<Collider>();
        _interactablesNotInRange = new List<Collider>();

        PlayerZAngle = transform.rotation.eulerAngles.z;
    }

    protected virtual void OnEnable()
    {
        EventManager.EventSubscribe(EventType.TEST_CONTROLS, TestControlHandler);
        EventManager.EventSubscribe(EventType.LEVEL_CAMS_YROT, ReceiveNewCamAngle);
    }

    protected virtual void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.TEST_CONTROLS, TestControlHandler);
        EventManager.EventUnsubscribe(EventType.LEVEL_CAMS_YROT, ReceiveNewCamAngle);
    }

    private void Update()
    {
        CheckInteract();
        PlayerRotation();
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

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
            if (MoveInput.magnitude != 0)
            {
                var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, _camYAngle, 0));
                var skewedInput = matrix.MultiplyPoint3x4(MoveInput);

                _rb.MoveRotation(Quaternion.LookRotation(skewedInput, Vector3.up));
            }
        }
    }

    // Interaction
    public void Interact(object data)
    {
        // TODO: Maybe make a statemachine for the player so this can be scalable

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
                // Closest interactable is item or NPC
                if (interactable is Item || interactable is NPC || interactable is Level1Mirror || interactable is PedestalConstellation)
                {
                    _closestInteractable = null;
                    Debug.Log(interactable);
                    interactable.PlayerStartInteract(this);
                }
            }
            // If carrying an item
            else
            {
                // Closest interactable is Rift
                if (interactable is Rift)
                {
                    interactable.PlayerStartInteract(this);
                }
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
                // Make sure carried item is not included again as a closest interactable
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
                            // Dehighlight previous closest interactable then assign new closest one
                            _closestInteractable.GetComponentInParent<IInteractable>().PlayerNotInRange(this);
                            _closestInteractable = collider;
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

        _camYAngle = (float)data;
    }
    #endregion
}
