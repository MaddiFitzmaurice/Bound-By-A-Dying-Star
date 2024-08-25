using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftPuzzleFixedPlatform : MonoBehaviour
{
    public bool HasPlayer1 { get; private set; } = false;
    public bool HasPlayer2 { get; private set; } = false;

    private void OnTriggerEnter(Collider other)
    {
        var p1 = other.GetComponent<Player1>();

        if (p1 != null)
        {
            HasPlayer1 = true;
        }

        var p2 = other.GetComponent<Player2>();

        if (p2 != null)
        {
            HasPlayer2 = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var p1 = other.GetComponent<Player1>();

        if (p1 != null)
        {
            HasPlayer1 = false;
        }

        var p2 = other.GetComponent<Player2>();

        if (p2 != null)
        {
            HasPlayer2 = false;
        }
    }
}
