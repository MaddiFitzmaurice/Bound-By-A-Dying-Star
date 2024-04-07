using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    // Assign the pick up location of the object in the Inspector
    [SerializeField] public Transform pickupPoint;
    private GameObject _carriedObject = null;
    // Track whether the player is holding an item
    public bool isHoldingItem = false;


    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYER_1_INTERACT, PickupOrDrop);
        EventManager.EventSubscribe(EventType.PLAYER_2_INTERACT, PickupOrDrop);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYER_1_INTERACT, PickupOrDrop);
        EventManager.EventUnsubscribe(EventType.PLAYER_2_INTERACT, PickupOrDrop);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Returns the currently held object of the player if need for other scripts
    public GameObject CurrentlyHeldObject
    {
        get { return _carriedObject; }
    }

    // You can call this method from an input event or another script
    public void PickupOrDrop(object data)
    {
        // Simplify the method since TryPickupObject already checks isHoldingItem.
        if (data.Equals("Player 1") && this.name.Contains("Player 1"))
        {
            if (!isHoldingItem)
            {
                TryPickupObject();
            }
            else
            {
                DropObject();
            }
        }
        else if (data.Equals("Player 2") && this.name.Contains("Player 2"))
        {
            if (!isHoldingItem)
            {
                TryPickupObject();
            }
            else
            {
                DropObject();
            }
        }
    }

    public void TryPickupObject()
    { 
        if (!isHoldingItem)
        {
            // 1f player is in the pickup radius
            Collider[] colliders = Physics.OverlapSphere(this.transform.position, 1f);
            foreach (var collider in colliders)
            {
                // This is the collider tag for objects, ensure each object has the "Pickupable" tag
                if (collider.CompareTag("Pickupable"))
                {
                    PickupableObject pickupableObject = collider.GetComponent<PickupableObject>();
                    if (pickupableObject != null && !pickupableObject.isLocked)
                    {
                        Debug.Log(this.name + "Picked Up An Object");
                        _carriedObject = collider.gameObject;
                        pickupableObject.BePickedUp(this); // Set the reference to this ItemPickup script
                        isHoldingItem = true;
                        break;
                    }
                }
            }
        }      
    }

    public void DropObject()
    {
        if (isHoldingItem)
        {
            if (_carriedObject != null)
            {
                // Detach the object
                _carriedObject.transform.SetParent(null);
                // Can Apply force to throw or drop it gently here if need
                _carriedObject = null;
                isHoldingItem = false;
                Debug.Log(this.name + "Dropped An Object");
            }
        }
    }

    public void SetCarriedObject(GameObject newObject)
    {
        if (isHoldingItem && _carriedObject != null)
        {
            _carriedObject = newObject;
            // Set the parent and position
            _carriedObject.transform.SetParent(pickupPoint);
            _carriedObject.transform.localPosition = Vector3.zero;
        }
    }

}
