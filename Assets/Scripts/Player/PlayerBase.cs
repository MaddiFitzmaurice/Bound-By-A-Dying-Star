using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
    [field:SerializeField] public GameObject CarriedItem { get; private set; } = null;
    #endregion

    #region INTERNAL DATA
    // Components
    private Rigidbody _rb;

    // Data
    protected RiftData RiftData;
    protected Vector3 MoveInput;

    // Interactables
    List<Collider> _interactablesInRange;
    List<Collider> _interactablesNotInRange;
    Collider _closestInteractable;
    #endregion

    void Awake()
    {
        // Set components
        _rb = GetComponent<Rigidbody>();

        // Set data
        RiftData = new RiftData(transform.position, transform.rotation, tag);
        _interactablesInRange = new List<Collider>();
        _interactablesNotInRange = new List<Collider>();
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

        _rb.AddForce(movementForce * transform.forward * MoveInput.z);
    }

    public void PlayerRotation()
    {
        Vector3 rotVector = new Vector3(0, MoveInput.x, 0);

        Quaternion playerRotChange = Quaternion.Euler(rotVector * Time.deltaTime * RotationSpeed);

        _rb.MoveRotation(_rb.rotation * playerRotChange);
    }

    // Interaction
    public void Interact(object data)
    {
        // If carrying an item and no objects are nearby
        if (CarriedItem != null && _closestInteractable == null)
        {
            CarriedItem.GetComponent<IInteractable>().PlayerStartInteract(this);
        }
        // If interactable is nearby
        else if (_closestInteractable != null)
        {
            IInteractable interactable = _closestInteractable.GetComponentInParent<IInteractable>();

            // If not currently carrying item
            if (CarriedItem == null)
            {
                // Closest interactable is item or NPC
                if (interactable is Item || interactable is NPC)
                {
                    _closestInteractable = null;
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
        List<Collider> colliders = colliderArray.ToList<Collider>();

        foreach (var collider in colliders)
        {
            IInteractable interactable = collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                // If closest interactable hasn't been assigned yet, assign first one in found collider list
                // Make sure carried item is not included again as a closest interactable
                if (_closestInteractable == null && collider.transform.parent.gameObject != CarriedItem)
                {
                    _closestInteractable = collider;
                }
                else 
                {
                    if (collider.transform.parent.gameObject != CarriedItem)
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
            foreach (var item in _interactablesInRange)
            {
                if (!colliders.Contains(item))
                {
                    _interactablesNotInRange.Add(item);

                    if (item == _closestInteractable)
                    {
                        item.GetComponentInParent<IInteractable>().PlayerNotInRange(this);
                        _closestInteractable = null;
                    }
                }
            }
        }

        // Remove interactables that are no longer in range
        if (_interactablesNotInRange.Count != 0)
        {
            foreach (var item in _interactablesNotInRange)
            {
                _interactablesInRange.Remove(item);
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
        CarriedItem = null;
    }    

    public void PickupItem(GameObject item)
    {
        CarriedItem = item;
    }
}
