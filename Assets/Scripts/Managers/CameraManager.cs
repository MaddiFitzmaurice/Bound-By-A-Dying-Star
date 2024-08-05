using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region INTERNAL DATA
    // Components
    private CinemachineBrain _cmBrain;
    private List<CinemachineVirtualCamera> _registeredCameras;
    private CinemachineTargetGroup _playerFollowGroup;
    #endregion

    #region FRAMEWORK FUNCTIONS
    private void Awake()
    {
        // Get Components
        _cmBrain = GetComponentInChildren<CinemachineBrain>();

        // Init Events
        EventManager.EventInitialise(EventType.CAMERA_NEW_FWD_DIR);
        EventManager.EventInitialise(EventType.CAMERA_REGISTER);
        EventManager.EventInitialise(EventType.CAMERA_DEREGISTER);
        EventManager.EventInitialise(EventType.CAMERA_ACTIVATE);

        // Init Data
        _registeredCameras = new List<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYERMANAGER_SEND_FOLLOWGROUP, ReceiveFollowGroupHandler);
        EventManager.EventSubscribe(EventType.CAMERA_REGISTER, RegisterCameraHandler);
        EventManager.EventSubscribe(EventType.CAMERA_DEREGISTER, DeregisterCameraHandler);
        EventManager.EventSubscribe(EventType.CAMERA_ACTIVATE, ActivateCameraHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYERMANAGER_SEND_FOLLOWGROUP, ReceiveFollowGroupHandler);
        EventManager.EventUnsubscribe(EventType.CAMERA_REGISTER, RegisterCameraHandler);
        EventManager.EventUnsubscribe(EventType.CAMERA_DEREGISTER, DeregisterCameraHandler);
        EventManager.EventUnsubscribe(EventType.CAMERA_ACTIVATE, ActivateCameraHandler);
    }
    #endregion

    // Called when Cinemachine Brain detects a camera activating
    public void LevelCameraActivateEvent()
    {
        EventManager.EventTrigger(EventType.CAMERA_NEW_FWD_DIR, _cmBrain.ActiveVirtualCamera.VirtualCameraGameObject);
    }

    private void ActivateSelectedCamera(CinemachineVirtualCamera cam)
    {
        if (!_registeredCameras.Contains(cam))
        {
            Debug.LogWarning("Cam was not registered... registering now. Is this going to be an issue?");
            _registeredCameras.Add(cam);
        }
        DeactivateAllCameras();
        cam.Priority = 5;
    }

    private void DeactivateAllCameras()
    {
        foreach (var cam in _registeredCameras)
        {
            cam.Priority = 0;
        }
    }

    #region EVENT HANDLERS
    // Receive follow group that cinemachine will use to follow or look at the players
    public void ReceiveFollowGroupHandler(object data)
    {
        if (data is not CinemachineTargetGroup)
        {
            Debug.LogError("LevelManager has not received a CinemachineTargetGroup!");
        }

        CinemachineTargetGroup target = (CinemachineTargetGroup)data;
        _playerFollowGroup = target;
    }

    public void RegisterCameraHandler(object data)
    {
        if (data is CameraData camData)
        {
            _registeredCameras.Add(camData.VirtualCamera);

            // Check if camera needs to follow player
            if (camData.CameraType == CameraType.DOLLY || camData.CameraType == CameraType.DOLLY_LOOK)
            {
                // Set follow to the players
                camData.VirtualCamera.Follow = _playerFollowGroup.transform;
            }

            // Check if camera needs to look at player
            if (camData.CameraType == CameraType.DOLLY_LOOK)
            {
                camData.VirtualCamera.LookAt = _playerFollowGroup.transform;
            }
        }
        else
        {
            Debug.LogError("Did not receive CameraData");
        }
    }

    public void DeregisterCameraHandler(object data)
    {
        if (data is CinemachineVirtualCamera cam)
        {
            _registeredCameras.Remove(cam);
        }
        else
        {
            Debug.LogError("Did not receive a VirtualCamera");
        }
    }

    public void ActivateCameraHandler(object data)
    {
        if (data is CinemachineVirtualCamera cam)
        {
            ActivateSelectedCamera(cam);
        }
        else
        {
            Debug.LogError("Did not receive a VirtualCamera");
        }
    }
    #endregion
}

// FIXED - Stationary, no looking at players
// DOLLY - Moving, no looking at players
// DOLLY_LOOK - Moving, looking at players
public enum CameraType { FIXED, DOLLY, DOLLY_LOOK }

// Data Class to distinguish what type of camera is being registered
public class CameraData
{
    public CinemachineVirtualCamera VirtualCamera { get; }
    public CameraType CameraType { get; }

    public CameraData(CinemachineVirtualCamera virtualCamera, CameraType cameraType)
    {
        VirtualCamera = virtualCamera;
        CameraType = cameraType;
    }
}