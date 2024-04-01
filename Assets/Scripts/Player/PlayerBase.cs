using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    // Components
    private Rigidbody _rb;

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
    [SerializeField] private Material _highlightMat;
    [field:SerializeField] public Transform PickupPoint { get; private set; } // Assign the pick up location of the object in the Inspector
    public bool isHoldingItem = false;
    [field:SerializeField] public GameObject CarriedItem { get; private set; } = null;
    List<Collider> _itemsInRange;
    List<Collider> _itemsNotInRange;

    void Awake()
    {
        // Set components
        _rb = GetComponent<Rigidbody>();

        // Set data
        RiftData = new RiftData(transform.position, transform.rotation, tag);
        _itemsInRange = new List<Collider>();
        _itemsNotInRange = new List<Collider>();
    }

    private void Update()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, 1f, LayerMask.GetMask("Interactables"));
        List<Collider> colliders = colliderArray.ToList<Collider>();

        foreach (var collider in colliders)
        {
            IInteractable interactable = collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                interactable.PlayerInRange(_highlightMat);
            }
        }

        if (_itemsInRange.Count != 0)
        {
            foreach (var item in _itemsInRange)
            {
                // If item is no longer in range
                if (!colliders.Contains(item))
                {
                    item.GetComponentInParent<IInteractable>().PlayerNotInRange();
                    _itemsNotInRange.Add(item);
                }
            }

            foreach (var item in _itemsNotInRange)
            {
                _itemsInRange.Remove(item);
            }
            _itemsNotInRange.Clear();
        }

        // Add items to list
        _itemsInRange.AddRange(colliders);
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

    public void DropItem()
    {
        CarriedItem = null;
    }    

    public void PickupItem(GameObject item)
    {
        CarriedItem = item;
    }
}
