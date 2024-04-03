using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateB : MonoBehaviour
{
    public PuzzleController puzzleController;
    private Renderer plateRenderer;
    private bool isActivated = false;

    void Start()
    {
        plateRenderer = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated && (other.CompareTag("Player1") || other.CompareTag("Player2")))
        {
            isActivated = true;
            plateRenderer.material.color = Color.green;
            puzzleController.PressurePlateBActivated();
        }
    }

    public bool IsActivated()
    {
        return isActivated;
    }
}