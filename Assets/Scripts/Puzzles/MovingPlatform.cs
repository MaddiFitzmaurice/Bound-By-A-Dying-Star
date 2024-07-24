using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the GameObject's tag is "Player1" or "Player2"
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the GameObject's tag is "Player1" or "Player2"
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            other.transform.SetParent(null);
        }
    }
}
