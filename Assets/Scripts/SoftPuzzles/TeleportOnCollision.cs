using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportOnCollision : MonoBehaviour
{
    
    [SerializeField] private Transform _teleportLocation;

   
    private bool _player1Teleported = false;
    private bool _player2Teleported = false;

    // Check for entering the collider
    private void OnTriggerEnter(Collider other)
    {
       
        var playerTransform = other.transform;

        
        if (other.CompareTag("Player1") && !_player1Teleported)
        {
            playerTransform.position = _teleportLocation.position;
            _player1Teleported = true; // Mark Player 1 as teleported
        }

        
        if (other.CompareTag("Player2") && !_player2Teleported)
        {
            playerTransform.position = _teleportLocation.position;
            _player2Teleported = true; // Mark Player 2 as teleported
        }
    }

   //Reset teleport status when players exit collider
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player1"))
        {
            _player1Teleported = false;
        }
        if (other.CompareTag("Player2"))
        {
            _player2Teleported = false;
        }
    }
}
