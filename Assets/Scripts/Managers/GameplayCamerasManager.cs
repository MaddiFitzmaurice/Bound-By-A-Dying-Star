using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayCamerasManager : MonoBehaviour
{
    #region EXTERNAL DATA
    [SerializeField] private GameObject _gameplayCamParent;
    #endregion

    private void Awake()
    {
        // Event Init
        EventManager.EventInitialise(EventType.RECEIVE_GAMEPLAY_CAM_PARENT);

        // Data Check
        if (_gameplayCamParent == null)
        {
            Debug.LogError("Please assign _gameplayCamParent!!!");
        }
    }

    private void OnEnable()
    {
        // Send Parent to CameraManager
        EventManager.EventTrigger(EventType.RECEIVE_GAMEPLAY_CAM_PARENT, _gameplayCamParent); // Temp. Put back in Start when gameplay scene loads before Level
    }
}
