using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PortalInteraction : MonoBehaviour
{
    // This is dynamically updated
    [SerializeField] private Transform _portalTarget;
    // Assign players portal prefab in the Inspector
    [SerializeField] private GameObject _portalPrefab;
    // How far in front the portal should spawn 
    [SerializeField] private float _distanceInFront = 2f;
    private bool _isNearPortal = false;
    private ItemPickup _itemPickup;
    // Portal Data to send to the PortalManager
    private PortalData _portalData;

    [SerializeField] private float detectionRadius = 5.0f;
    private GameObject nearestPedestal = null;

    private void Awake()
    {
        EventManager.EventInitialise(EventType.PORTALMANAGER_CREATEPORTAL);
    }

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
        _itemPickup = GetComponent<ItemPickup>();

        // Create default data to be changed later
        _portalData = new PortalData(Vector3.zero, Quaternion.Euler(0, 0, 0), _portalPrefab, this.name, this, nearestPedestal);
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
                _isNearPortal = true;

                // Update portalTarget to the collided portal's destination
                _portalTarget = portalInfo.destinationPortal;
                Debug.Log($"{gameObject.name} entered portal, setting target to {_portalTarget.name}");
            }
            else if (other.CompareTag("Player2Portal"))
            {
                _isNearPortal = true;

                // Update portalTarget to the collided portal's destination
                _portalTarget = portalInfo.destinationPortal;
                Debug.Log($"{gameObject.name} entered portal, setting target to {_portalTarget.name}");
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
                _portalTarget = null;
                Debug.Log($"{gameObject.name} exited portal, clearing portal target");
            }
            _isNearPortal = false;
        }
        else if (other.CompareTag("Player2Portal"))
        {
            // Reset portalTarget when exiting the portal area
            if (other.GetComponent<PortalInfo>() != null)
            {
                _portalTarget = null;
                Debug.Log($"{gameObject.name} exited portal, clearing portal target");
            }
            _isNearPortal = false;
        }
    }

    public void InteractWithPortal(object data)
    {
        if (_isNearPortal && _portalTarget != null)
        {
            if (_itemPickup.isHoldingItem)
            {
                // If the player is holding an item and near a portal, teleport the item.
                GameObject itemToTeleport = _itemPickup.CurrentlyHeldObject;
                Debug.Log("Item to teleport original position: " + itemToTeleport.transform.position);
                itemToTeleport = ChangeItemVersion(itemToTeleport);
                Debug.Log("Item to teleport new position: " + itemToTeleport.transform.position);
                itemToTeleport.transform.position = _portalTarget.position;
                _itemPickup.DropObject();
                Debug.Log("Item teleported to target portal");
                EventManager.EventTrigger(EventType.PORTALSENDEFFECT, _portalData);
            }
            else
            {
                // If the player is not holding an item, and is at the portal, grabs the item instead of teleporting it
                _itemPickup.TryPickupObject();
            }
        }
    }

    private void CreatePortalInFrontOfPlayer(object data)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            // Assuming pedestals are tagged as "Mirror Pedestal"
            if (hitCollider.CompareTag("Mirror Pedestal"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestPedestal = hitCollider.gameObject;
                }
            }
        }

        if (_portalData.PlayerTag == "Player 1" && (string)data == "Player 1")
        {
            _portalData.Position = transform.position + transform.forward * _distanceInFront;
            _portalData.Rotation = Quaternion.LookRotation(transform.forward);

            EventManager.EventTrigger(EventType.PORTALMANAGER_CREATEPORTAL, _portalData);
            Debug.Log("Portal created in front of the player");
        }
        else if (this.name.Contains("Player 2") && (string)data == "Player 2")
        {
            _portalData.Position = transform.position + transform.forward * _distanceInFront;
            _portalData.Rotation = Quaternion.LookRotation(transform.forward);

            EventManager.EventTrigger(EventType.PORTALMANAGER_CREATEPORTAL, _portalData);
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

