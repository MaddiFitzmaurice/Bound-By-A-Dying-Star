using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupableObject : MonoBehaviour
{
    private Transform playerTransform;
    private bool isCarried = false;

    public void BePickedUp(Transform player)
    {
        isCarried = true;
        playerTransform = player;
        transform.SetParent(player); // Makes the object a child of the player, so it moves with the player
        transform.localPosition = new Vector3(0, 1, 0); // Adjust this to where you want the object relative to the player
    }

    public void BeDropped()
    {
        isCarried = false;
        playerTransform = null;
        transform.SetParent(null); // Removes the parent-child relationship, making the object independent in the scene
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isCarried)
        {
            // Here, you can add any logic that needs to happen while the object is being carried
            // For example, you could adjust the position relative to the player if needed
        }
    }
}
