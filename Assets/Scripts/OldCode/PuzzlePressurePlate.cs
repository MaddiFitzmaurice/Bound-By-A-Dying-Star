using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePressurePlate : MonoBehaviour
{
    public PuzzleController puzzleController;
    public string doorID;  // The ID of the door this plate controls

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            puzzleController.UnlockDoor(doorID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            puzzleController.LockDoor(doorID);
        }
    }
}