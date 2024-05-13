using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayCamerasManager : MonoBehaviour
{
    #region EXTERNAL DATA
    [SerializeField] private GameObject _gameplayCamParent;
    #endregion

    #region INTERNAL DATA
    private CinemachineClearShot _clearShot;
    #endregion

    private void Awake()
    {
        // Event Init
        EventManager.EventInitialise(EventType.RECEIVE_GAMEPLAY_CAM_PARENT);

        // Component Init
        _clearShot = _gameplayCamParent.GetComponent<CinemachineClearShot>();

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

        EventManager.EventSubscribe(EventType.CLEARSHOT_CAMS_SEND_FOLLOWGROUP, ReceiveFollowGroupHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.CLEARSHOT_CAMS_SEND_FOLLOWGROUP, ReceiveFollowGroupHandler);
    }

    private void Start()
    {
        
    }

    #region EVENT HANDLERS
    // Receive follow group that ClearShot will look at
    public void ReceiveFollowGroupHandler (object data)
    {
        if (data is not CinemachineTargetGroup)
        {
            Debug.LogError("LevelManager has not received a CinemachineTargetGroup!");
        }

        CinemachineTargetGroup target = (CinemachineTargetGroup)data;
        _clearShot.LookAt = target.transform;
    }
    #endregion
}
