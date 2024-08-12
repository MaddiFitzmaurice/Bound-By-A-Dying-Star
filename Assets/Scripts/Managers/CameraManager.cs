using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region INTERNAL DATA
    // Components
    private CinemachineBrain _cmBrain;
    private Camera _cam;

    // Cam frustum
    private List<Plane> _frustumPlanes;
    private bool _genPlanes = false;

    // Cinemachine
    private List<CinemachineVirtualCamera> _registeredCameras;
    private CinemachineTargetGroup _playerFollowGroup;

    // Players
    private Player1 _p1;
    private Player2 _p2;
    private Collider _p1Collider;
    private Collider _p2Collider;
    #endregion

    #region FRAMEWORK FUNCTIONS
    private void Awake()
    {
        // Get Components
        _cmBrain = GetComponentInChildren<CinemachineBrain>();
        _cam = GetComponentInChildren<Camera>();

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
        EventManager.EventSubscribe(EventType.ENABLE_GAMEPLAY_INPUTS, StartGeneratingPlanes);
        EventManager.EventSubscribe(EventType.DISABLE_GAMEPLAY_INPUTS, StopGeneratingPlanes);
        EventManager.EventSubscribe(EventType.PLAYERMANAGER_SEND_PLAYER1, ReceivePlayers);
        EventManager.EventSubscribe(EventType.PLAYERMANAGER_SEND_PLAYER2, ReceivePlayers);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYERMANAGER_SEND_FOLLOWGROUP, ReceiveFollowGroupHandler);
        EventManager.EventUnsubscribe(EventType.CAMERA_REGISTER, RegisterCameraHandler);
        EventManager.EventUnsubscribe(EventType.CAMERA_DEREGISTER, DeregisterCameraHandler);
        EventManager.EventUnsubscribe(EventType.CAMERA_ACTIVATE, ActivateCameraHandler);
        EventManager.EventUnsubscribe(EventType.ENABLE_GAMEPLAY_INPUTS, StartGeneratingPlanes);
        EventManager.EventUnsubscribe(EventType.DISABLE_GAMEPLAY_INPUTS, StopGeneratingPlanes);
        EventManager.EventUnsubscribe(EventType.PLAYERMANAGER_SEND_PLAYER1, ReceivePlayers);
        EventManager.EventUnsubscribe(EventType.PLAYERMANAGER_SEND_PLAYER2, ReceivePlayers);
    }

    private void Update()
    {
        // Get camera's frustum planes
        if (_genPlanes)
        {
            _frustumPlanes = GeometryUtility.CalculateFrustumPlanes(_cam).ToList();

            if (_p1 != null)
            {
                if (!GeometryUtility.TestPlanesAABB(_frustumPlanes.ToArray(), _p1Collider.bounds))
                {
                    Debug.Log("PLAYER 1 OFFSCREEN");
                }
            }

            if (_p2 != null)
            {
                if (!GeometryUtility.TestPlanesAABB(_frustumPlanes.ToArray(), _p2Collider.bounds))
                {
                    Debug.Log("PLAYER 2 OFFSCREEN");
                }
            }
        }
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
    // When to start generating frustum planes
    public void StartGeneratingPlanes(object data)
    {
        _genPlanes = true;
    }

    // When to stop generating frustum planes
    public void StopGeneratingPlanes(object data)
    {
        _genPlanes = false;
    }

    // Receive Players
    public void ReceivePlayers(object data)
    {
        if (data is Player1 p1)
        {
            _p1 = p1;
            _p1Collider = _p1.GetComponent<Collider>();
        }
        else if (data is Player2 p2)
        {
            _p2 = p2;
            _p2Collider = _p2.GetComponent<Collider>();
        }
    }

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