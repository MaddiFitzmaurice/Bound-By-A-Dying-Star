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
    protected Vector3 MoveDirection;

    // Rift Data
    [Header("Rift Data")]
    protected RiftData RiftData;
    [SerializeField] protected float DistanceInFront = 2f;

    // Item Data
    [field:Header("Item Data")]
    [SerializeField] private Material _highlightMat; // Temp material to highlight object to be picked up
    [field:SerializeField] public Transform PickupPoint { get; private set; } // Assign the pick up location of the object in the Inspector
    [field:SerializeField] public GameObject CarriedItem { get; private set; } = null;
    #endregion

    #region INTERNAL DATA
    // Components
    private Rigidbody _rb;

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
    }

    private void FixedUpdate()
    {
        PlayerMovement();
        PlayerRotation();
    }

    // Movement
    public void PlayerMovement()
    {
        // Calculate desired velocity
        float targetVelocity = MoveDirection.magnitude * MoveSpeed;

        // Find diff between desired velocity and current velocity
        float velocityDif = targetVelocity - _rb.velocity.magnitude;

        // Check whether to accel or deccel
        float accelRate = (Mathf.Abs(targetVelocity) > 0.01f) ? MoveAccel :
            MoveDecel;

        // Calc force by multiplying accel and velocity diff, and applying velocity power
        float movementForce = Mathf.Pow(Mathf.Abs(velocityDif) * accelRate, VelocityPower)
            * Mathf.Sign(velocityDif);

        _rb.AddForce(movementForce * MoveDirection);
    }

    public void PlayerRotation()
    {
        if (MoveDirection.magnitude != 0)
        {
            _rb.MoveRotation(Quaternion.LookRotation(MoveDirection, Vector3.up));
        }
    }

    // Interaction
    public void Interact(object data)
    {

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
                if (_closestInteractable == null)
                {
                    _closestInteractable = collider;
                }
                else 
                {
                    if (Vector3.Distance(collider.transform.position, transform.position) < 
                        Vector3.Distance(_closestInteractable.transform.position, transform.position))
                    {
                        // Dehighlight previous closest interactable then assign new closest one
                        _closestInteractable.GetComponentInParent<IInteractable>().PlayerNotInRange();
                        _closestInteractable = collider;
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
                        item.GetComponentInParent<IInteractable>().PlayerNotInRange();
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
            _closestInteractable.GetComponentInParent<IInteractable>().PlayerInRange(_highlightMat);
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