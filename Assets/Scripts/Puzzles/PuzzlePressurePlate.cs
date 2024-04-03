using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePressurePlate : MonoBehaviour
{
    public PuzzleController puzzleController;
    private Renderer plateRenderer;

    void Start()
    {
        plateRenderer = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            plateRenderer.material.color = Color.green;
            puzzleController.PlayerSteppedOnPlate(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            plateRenderer.material.color = Color.white; 
            puzzleController.PlayerLeftPlate(this);
        }
    }
}