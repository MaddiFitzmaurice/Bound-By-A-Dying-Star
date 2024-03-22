using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstTrigger : MonoBehaviour
{
    public List<GameObject> validObjects; // List of constellations that will change color
    private Renderer diskRenderer;

    void Start()
    {
        // Get the Renderer component from the disk
        diskRenderer = GetComponent<Renderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the Mirror is in the list of valid objects
        if (validObjects.Contains(other.gameObject))
        {
            // Change the disk's color to green
            diskRenderer.material.color = Color.green;
        }
    }

    void OnTriggerExit(Collider other)
    {
        //Reset the disk's color when the object leaves
        if (validObjects.Contains(other.gameObject))
        {
            // Change the disk' color back. THESE ARE ALL TEST COLORS FOR FUNCTIONALITY.
            diskRenderer.material.color = Color.red; 
        }
    }
}
