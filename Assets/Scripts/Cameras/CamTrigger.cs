using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTrigger : MonoBehaviour
{
    #region EXTERNAL DATA
    [Tooltip("Camera to trigger on/off")]
    [SerializeField] private CinemachineVirtualCamera _cam;
    #endregion

    #region INTERNAL DATA
    // Are both Players inside the trigger?
    private bool _player1In = false;
    private bool _player2In = false;
    #endregion

    public void OnEnable()
    {
        // Register camera to CameraManager
        if (_cam != null)
        {
            EventManager.EventTrigger(EventType.CAMERA_REGISTER, _cam);
        }
        else
        {
            Debug.LogError("No camera assigned to this trigger!");
        }
    }

    public void OnDisable()
    {
        // Deregister camera from CameraManager
        if (_cam != null)
        {
            EventManager.EventTrigger(EventType.CAMERA_DEREGISTER, _cam);
        }
        else
        {
            Debug.LogError("No camera assigned to this trigger!");
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        // Check if Player 1 entered trigger
        var p1 = other.GetComponent<Player1>();

        if (p1 != null)
        {
            _player1In = true;
            
            // If both players are in trigger, activate camera
            if (_player2In)
            {
                EventManager.EventTrigger(EventType.CAMERA_ACTIVATE, _cam);
            }

            return;
        }

        // If no Player 1, check Player 2
        var p2 = other.GetComponent<Player2>();

        if (p2 != null)
        {
            _player2In = true;

            // If both players are in trigger, activate camera
            if (_player1In)
            {
                EventManager.EventTrigger(EventType.CAMERA_ACTIVATE, _cam);
            }

            return;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        // Check if Player 1 exited trigger
        var p1 = other.GetComponent<Player1>();

        if (p1 != null)
        {
            _player1In = false;
            return;
        }

        // If no Player 1, check Player 2
        var p2 = other.GetComponent<Player2>();

        if (p2 != null)
        {
            _player2In = false;
            return;
        }
    }
}
