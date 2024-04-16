using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    #region EXTERNAL DATA
    public bool Active { get; set; } = false;
    #endregion

    #region INTERNAL DATA
    // Players
    private bool _player1In = false;
    private bool _player2In = false;

    // Teleport
    private Teleport _teleportParent;
    #endregion

    public void SetTeleportParent(Teleport teleport)
    {
        _teleportParent = teleport;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Active)
        {
            if (other.GetComponent<Player1>())
            {
                _player1In = true;
            }
            else if (other.GetComponent<Player2>())
            {
                _player2In = true;
            }

            if (_player1In && _player2In)
            {
                if (_teleportParent == null)
                {
                    Debug.LogError("Teleport Parent has not been set!");
                }
                else
                {
                    _teleportParent.TeleportPlayer(this);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player1>())
        {
            _player1In = false;
        }
        else if (other.GetComponent<Player2>())
        {
            _player2In = false;
        }

        if (!_player1In && !_player2In)
        {
            _teleportParent.ResetTeleport(this);
        }
    }
}
