using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupableObject : MonoBehaviour
{
    [SerializeField] private GameObject _itemVersion1;
    [SerializeField] private GameObject _itemVersion2;
    private GameObject _currentItemVersion;
    private bool _isCarried = false;
    public bool isLocked = false;

    private ItemPickup itemPickupScript; // Reference to the ItemPickup script

    public void BePickedUp(ItemPickup itemPickup)
    {
        _isCarried = true;
        // Store the reference to the ItemPickup script
        itemPickupScript = itemPickup; 
        // Set this object as the parent to the pickup point
        transform.SetParent(itemPickup.pickupPoint);
        transform.localPosition = Vector3.zero;
    }

    public void BeDropped()
    {
        _isCarried = false;
        // Removes the parent-child relationship, making the object independent in the scene
        transform.SetParent(null);

        // Use the stored reference to drop the object
        if (itemPickupScript != null)
        {
            itemPickupScript.DropObject();
        }
        // Clear the reference
        itemPickupScript = null; 
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Use this to implement functionality while holding objects
        //if (isCarried)
        //{
         
        //}
    }

    private void Awake()
    {
        _currentItemVersion = _itemVersion1;
    }

    //Returns and sets the CurrentItemVersion of the object
    public GameObject CurrentItemVersion
    {
        get => _currentItemVersion;
        set => _currentItemVersion = value;
    }

    public void SetItemVersions(GameObject version1, GameObject version2)
    {
        _itemVersion1 = version1;
        _itemVersion2 = version2;
    }

    public void LockObject() 
    {
        isLocked = true;
    }

    //Creates "public" properties of the item versions to avoid accidently changing the default values assigned in inspector. 
    public GameObject ItemVersion1 => _itemVersion1;
    public GameObject ItemVersion2 => _itemVersion2;
    public bool IsCarried => _isCarried;
}
