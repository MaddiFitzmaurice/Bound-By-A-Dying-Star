using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public PuzzleController puzzleController;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            // Update the game's current respawn point to this checkpoint's position
            puzzleController.UpdateRespawnPoint(transform);
        }
    }
}
