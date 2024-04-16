using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    #region EXTERNAL DATA
    
    #endregion

    #region INTERNAL DATA
    private bool _player1In;
    private bool _player2In;
    #endregion

    private void OnTriggerEnter(Collider other)
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
            //EventManager.EventTrigger(EventType.LEVEL_CAMS_CHANGE, _changeCamOnEnter);
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
            //EventManager.EventTrigger(EventType.LEVEL_CAMS_CHANGE, _changeCamOnExit);
        }
    }
}
