using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupableObject : MonoBehaviour
{
    [SerializeField] private GameObject itemVersion1;
    [SerializeField] private GameObject itemVersion2;
    private GameObject currentItemVersion;
    private Transform playerTransform;
    private bool isCarried = false;

    public void BePickedUp(Transform player)
    {
        isCarried = true;
        playerTransform = player;
        // Makes the object a child of the player, so it moves with the player
        transform.SetParent(player);
        // Adjust this to where you want the object relative to the player
        transform.localPosition = new Vector3(0, 1, 0); 
    }

    public void BeDropped()
    {
        isCarried = false;
        playerTransform = null;
        // Removes the parent-child relationship, making the object independent in the scene
        transform.SetParent(null); 
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
        currentItemVersion = itemVersion1;
    }

    //Returns and sets the CurrentItemVersion of the object
    public GameObject CurrentItemVersion
    {
        get => currentItemVersion;
        set => currentItemVersion = value;
    }

    public void SetItemVersions(GameObject version1, GameObject version2)
    {
        itemVersion1 = version1;
        itemVersion2 = version2;
    }


    //Creates "public" properties of the item versions to avoid accidently changing the default values assigned in inspector. 
    public GameObject ItemVersion1 => itemVersion1;
    public GameObject ItemVersion2 => itemVersion2;
    public bool IsCarried => isCarried;
}
