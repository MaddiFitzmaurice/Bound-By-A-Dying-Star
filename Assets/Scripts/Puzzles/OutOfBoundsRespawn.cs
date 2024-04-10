using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsRespawn : MonoBehaviour
{
    public PuzzleController puzzleController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            puzzleController.RespawnPlayer(other.gameObject);
        }
    }
}


