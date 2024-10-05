using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftPuzzleFixedPlatform : MonoBehaviour
{
    #region EXTERNAL DATA
    [field: SerializeField] public Transform MoveToPos { get; private set; }
    #endregion

    #region INTERNAL DATA
    private PlayerBase _lastPlayerOn;
    private bool _hasPlayer1 = false;
    private bool _hasPlayer2 = false;
    #endregion

    public void CheckIfPlayerOn()
    {
        // If no player is on the platform, teleport last player to it
        if (!_hasPlayer1 && !_hasPlayer2)
        {
            if (_lastPlayerOn != null)
            {
                _lastPlayerOn.PuzzleTeleport(transform);
            }
            else
            {
                Debug.LogError("Somehow no player stepped on this platform!");
            }
        }
    }

    #region FRAMEWORK FUNCTIONS
    private void OnTriggerEnter(Collider other)
    {
        var p1 = other.GetComponent<Player1>();

        if (p1 != null)
        {
            _hasPlayer1 = true;
        }

        var p2 = other.GetComponent<Player2>();

        if (p2 != null)
        {
            _hasPlayer2 = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var p1 = other.GetComponent<Player1>();

        if (p1 != null)
        {
            _hasPlayer1 = false;

            // If player 2 is not on, assign player 1 as last on
            if (!_hasPlayer2)
            {
                _lastPlayerOn = p1;
            }
        }

        var p2 = other.GetComponent<Player2>();

        if (p2 != null)
        {
            _hasPlayer2 = false;

            // If player 1 is not on, assign player 2 as last on
            if (!_hasPlayer1)
            {
                _lastPlayerOn = p2;
            }
        }
    }
    #endregion
}
