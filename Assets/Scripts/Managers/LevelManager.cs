using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region EXTERNAL DATA
    [SerializeField] GameObject _levelCams;
    #endregion

    #region INTERNAL DATA
    private CinemachineClearShot _vCamClearShot;
    #endregion

    private void Awake()
    {
        // Get Components
        _vCamClearShot = _levelCams.GetComponent<CinemachineClearShot>();

        // Event Inits
        EventManager.EventInitialise(EventType.LEVEL_CAMS_REQUEST_FOLLOWGROUP);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LEVEL_CAMS_SEND_FOLLOWGROUP, ReceiveFollowGroup);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LEVEL_CAMS_SEND_FOLLOWGROUP, ReceiveFollowGroup);
    }

    private void Start()
    {
        if (_levelCams != null)
        {
            EventManager.EventTrigger(EventType.LEVEL_CAMS_REQUEST_FOLLOWGROUP, null);
        }
        else
        {
            Debug.LogError("No Level Cameras have been set up!");
        }
    }

    #region EVENT HANDLERS
    public void ReceiveFollowGroup(object data)
    {
        if (data is not CinemachineTargetGroup)
        {
            Debug.LogError("LevelManager has not received a CinemachineTargetGroup!");
        }

        CinemachineTargetGroup target = (CinemachineTargetGroup)data;
        _vCamClearShot.LookAt = target.transform;
    }
    #endregion
}
