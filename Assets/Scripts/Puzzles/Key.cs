using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public GameObject doorToOpen; 

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("KeyDoor"))
        {
            // Deactivate the door
            other.gameObject.SetActive(false);

            // Deactivate the key itself
            gameObject.SetActive(false);
        }
    }
}
