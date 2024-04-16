using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate_MovingPlatforms : MonoBehaviour
{
    public PuzzleController puzzleController; 

    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            
            puzzleController.StopPlatform();
        }
    }

    private void OnTriggerExit(Collider other)
    {
       
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            
            puzzleController.ResumePlatform();
        }
    }
}
