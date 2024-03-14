using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    // Assign the pick up location of the object in the Inspector
    [SerializeField] private Transform pickupPoint;
    private GameObject carriedObject = null;
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
        get { return carriedObject; }
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
                    Debug.Log(this.name + "Picked Up An Object");
                    carriedObject = collider.gameObject;
                    carriedObject.transform.SetParent(pickupPoint);
                    carriedObject.transform.localPosition = Vector3.zero;
                    isHoldingItem = true;
                    break;
                }
            }
        }      
    }

    public void DropObject()
    {
        if (isHoldingItem)
        {
            if (carriedObject != null)
            {
                // Detach the object
                carriedObject.transform.SetParent(null);
                // Can Apply force to throw or drop it gently here if need
                carriedObject = null;
                isHoldingItem = false;
                Debug.Log(this.name + "Dropped An Object");
            }
        }
    }
}
