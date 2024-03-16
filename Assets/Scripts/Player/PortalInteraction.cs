using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PortalInteraction : MonoBehaviour
{
    // This is dynamically updated
    [SerializeField] private Transform portalTarget;
    // Assign players portal prefab in the Inspector
    [SerializeField] private GameObject portalPrefab; 
    // How far in front the portal should spawn 
    [SerializeField] private float distanceInFront = 2f;
    private bool isNearPortal = false;
    private ItemPickup itemPickup;

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYER_1_CREATEPORTAL, CreatePortalInFrontOfPlayer);
        EventManager.EventSubscribe(EventType.PLAYER_1_SENDITEM, InteractWithPortal);
        EventManager.EventSubscribe(EventType.PLAYER_2_CREATEPORTAL, CreatePortalInFrontOfPlayer);
        EventManager.EventSubscribe(EventType.PLAYER_2_SENDITEM, InteractWithPortal);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYER_1_CREATEPORTAL, CreatePortalInFrontOfPlayer);
        EventManager.EventUnsubscribe(EventType.PLAYER_1_SENDITEM, InteractWithPortal);
        EventManager.EventUnsubscribe(EventType.PLAYER_2_CREATEPORTAL, CreatePortalInFrontOfPlayer);
        EventManager.EventUnsubscribe(EventType.PLAYER_2_SENDITEM, InteractWithPortal);
    }

    // Start is called before the first frame update
    void Start()
    {
        itemPickup = GetComponent<ItemPickup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {

    }
        private void OnTriggerEnter(Collider other)
    {
        // Check if the collider is a portal
        PortalInfo portalInfo = other.GetComponent<PortalInfo>();
        if (portalInfo != null && portalInfo.destinationPortal != null)
        {
            // Check for portal tag and set isNearPortal true if near own portal
            if (other.CompareTag("Player1Portal")) 
            {
                isNearPortal = true;

                // Update portalTarget to the collided portal's destination
                portalTarget = portalInfo.destinationPortal;
                Debug.Log($"{gameObject.name} entered portal, setting target to {portalTarget.name}");
            }
            else if (other.CompareTag("Player2Portal"))
            {
                isNearPortal = true;

                // Update portalTarget to the collided portal's destination
                portalTarget = portalInfo.destinationPortal;
                Debug.Log($"{gameObject.name} entered portal, setting target to {portalTarget.name}");
            }
        }
         
    }

    private void OnTriggerExit(Collider other)
    {
        // Set isNearPortal false when not near the portal
        if (other.CompareTag("Player1Portal"))
        {
            // Reset portalTarget when exiting the portal area
            if (other.GetComponent<PortalInfo>() != null)
            {
                portalTarget = null;
                Debug.Log($"{gameObject.name} exited portal, clearing portal target");
            }
            isNearPortal = false;
        }
        else if (other.CompareTag("Player2Portal"))
        { 
            // Reset portalTarget when exiting the portal area
            if (other.GetComponent<PortalInfo>() != null)
            {
                portalTarget = null;
                Debug.Log($"{gameObject.name} exited portal, clearing portal target");
            }
            isNearPortal = false;
        }
    }

    public void InteractWithPortal(object data)
    {
        if (isNearPortal && portalTarget != null)
        {
            if (itemPickup.isHoldingItem)
            {
                // If the player is holding an item and near a portal, teleport the item.
                GameObject itemToTeleport = itemPickup.CurrentlyHeldObject;
                Debug.Log("Item to teleport original position: " + itemToTeleport.transform.position);
                itemToTeleport = ChangeItemVersion(itemToTeleport);
                Debug.Log("Item to teleport new position: " + itemToTeleport.transform.position);
                itemToTeleport.transform.position = portalTarget.position;
                itemPickup.DropObject();
                Debug.Log("Item teleported to target portal");
            }
            else
            {
                // If the player is not holding an item, and is at the portal, grabs the item instead of teleporting it
                itemPickup.TryPickupObject();
            }
        }
    }

    private void CreatePortalInFrontOfPlayer(object data)
    {
        if (this.name.Contains("Player 1") && (string)data == "Player 1")
        {
            Vector3 spawnPosition = transform.position + transform.forward * distanceInFront;
            Quaternion spawnRotation = Quaternion.LookRotation(transform.forward);

            PortalManager.Instance.CreatePortal(spawnPosition, spawnRotation, portalPrefab, "Player 1", this);
            Debug.Log("Portal created in front of the player");
        }
        else if (this.name.Contains("Player 2") && (string)data == "Player 2")
        {
            Vector3 spawnPosition = transform.position + transform.forward * distanceInFront;
            Quaternion spawnRotation = Quaternion.LookRotation(transform.forward);

            PortalManager.Instance.CreatePortal(spawnPosition, spawnRotation, portalPrefab, "Player 2", this);
            Debug.Log("Portal created in front of the player");
        }
    }

    private GameObject ChangeItemVersion(GameObject item)
    {
        PickupableObject pickupableObject = item.GetComponent<PickupableObject>();
        GameObject newItemVersionPrefab = null;
        if (pickupableObject != null)
        {
            //Debug.Log("Current Item Version: " + pickupableObject.CurrentItemVersion);

            if (pickupableObject.CurrentItemVersion.name == pickupableObject.ItemVersion1.name)
            {
                //Debug.Log("New Item Version (Should be 2): " + pickupableObject.ItemVersion2);
                newItemVersionPrefab = pickupableObject.ItemVersion2;
            }
            else if (pickupableObject.CurrentItemVersion.name == pickupableObject.ItemVersion2.name)
            {
                //Debug.Log("New Item Version (Should be 1): " + pickupableObject.ItemVersion1);
                newItemVersionPrefab = pickupableObject.ItemVersion1;
            }

            if (newItemVersionPrefab != null)
            {
                //Debug.Log("New Item Name: " + newItemVersionPrefab.name);
                GameObject newItem = Instantiate(newItemVersionPrefab, item.transform.position, item.transform.rotation);
                // Remove "(Clone)" from the name
                newItem.name = newItemVersionPrefab.name; 


                // Grabs PickupableObject from the newly instantiated item for version updating
                PickupableObject newPickupableObject = newItem.GetComponent<PickupableObject>();

                if (newPickupableObject != null)
                {
                    // Copy the item version references to the new object
                    newPickupableObject.SetItemVersions(pickupableObject.ItemVersion1, pickupableObject.ItemVersion2);

                    // Update the currentItemVersion to reflect the current active version
                    newPickupableObject.CurrentItemVersion = newItem;
                }

                // Destroy the old item
                Destroy(item);

                // Returns the new item
                return newItem;

            }
        }
        // Returns the original item if no transformation occured
        return item;
    }



}
