using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateB : MonoBehaviour
{
    public string plateID;
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
            plateRenderer.material.color = Color.green;  // Turn green immediately on step
            puzzleController.PlateActivated(plateID, plateRenderer);
        }
    }
}