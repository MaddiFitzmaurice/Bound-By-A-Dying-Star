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
    //private GameObject _gameplayCams; // Transform to parent child vCams to it
    //private Dictionary<GameObject, GameObject> _activeGameplayCams; // Key: VCam gameObj, Value: Level/Softpuzzle gameObj parent
    private List<CinemachineVirtualCamera> _registeredCameras;
    private CinemachineTargetGroup _playerFollowGroup;
    #endregion
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
        //_activeGameplayCams = new Dictionary<GameObject, GameObject>();
        _registeredCameras = new List<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYERMANAGER_SEND_FOLLOWGROUP, ReceiveFollowGroupHandler);
        EventManager.EventSubscribe(EventType.CAMERA_REGISTER, RegisterCameraHandler);
        EventManager.EventSubscribe(EventType.CAMERA_DEREGISTER, DeregisterCameraHandler);
        EventManager.EventSubscribe(EventType.CAMERA_ACTIVATE, ActivateCameraHandler);
        //EventManager.EventSubscribe(EventType.RECEIVE_GAMEPLAY_CAM_PARENT, ReceiveGameplayCamParentHandler);
        //EventManager.EventSubscribe(EventType.ADD_GAMEPLAY_CAM, AddGameplayCamHandler);
        //EventManager.EventSubscribe(EventType.DELETE_GAMEPLAY_CAM, DeleteGameplayCamHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYERMANAGER_SEND_FOLLOWGROUP, ReceiveFollowGroupHandler);
        EventManager.EventUnsubscribe(EventType.CAMERA_REGISTER, RegisterCameraHandler);
        EventManager.EventUnsubscribe(EventType.CAMERA_DEREGISTER, DeregisterCameraHandler);
        EventManager.EventUnsubscribe(EventType.CAMERA_ACTIVATE, ActivateCameraHandler);
        //EventManager.EventUnsubscribe(EventType.RECEIVE_GAMEPLAY_CAM_PARENT, ReceiveGameplayCamParentHandler);
        //EventManager.EventUnsubscribe(EventType.ADD_GAMEPLAY_CAM, AddGameplayCamHandler);
        //EventManager.EventUnsubscribe(EventType.DELETE_GAMEPLAY_CAM, DeleteGameplayCamHandler);
    }

    // Called when Cinemachine Brain detects a camera activating
    public void LevelCameraActivateEvent()
    {
        EventManager.EventTrigger(EventType.CAMERA_NEW_FWD_DIR, _cmBrain.ActiveVirtualCamera.VirtualCameraGameObject.transform.localEulerAngles.y);
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
    // Receive follow group that ClearShot will look at
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
        if (data is CinemachineVirtualCamera cam)
        {
            _registeredCameras.Add(cam);

            // Check if it is a dolly cam
            if (cam.GetCinemachineComponent<CinemachineTrackedDolly>() != null)
            {
                // Set follow to the players
                cam.Follow = _playerFollowGroup.transform;
            }
        }
        else
        {
            Debug.LogError("Did not receive a VirtualCamera");
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

    //// Receive gameplay cam parent to swap out active cameras as they are received
    //public void ReceiveGameplayCamParentHandler(object data)
    //{
    //    if (data is not GameObject)
    //    {
    //        Debug.LogError("CameraManager did not receive a GameObject!");
    //    }

    //    _gameplayCams = data as GameObject;
    //}

    //public void AddGameplayCamHandler(object data)
    //{
    //    if (data is not GameObject)
    //    {
    //        Debug.LogError("CameraManager did not receive a GameObject Cam parent!");
    //    }

    //    GameObject sceneParent = data as GameObject;

    //    // Get list of vCams that are childed to the received sceneParent as gameobjects
    //    List<Transform> vCamTransforms = sceneParent.GetComponentsInChildren<Transform>().ToList();
    //    List<GameObject> vCams = new List<GameObject>();

    //    foreach (Transform child in  vCamTransforms)
    //    {
    //        vCams.Add(child.gameObject);
    //    }

    //    // Then add to Active Gameplay Cams Dictionary
    //    foreach (GameObject vCam in vCams)
    //    {
    //        // Make sure a virtual camera is attached to it
    //        CinemachineVirtualCamera vCamSettings = vCam.GetComponent<CinemachineVirtualCamera>();
    //        if (vCamSettings != null)
    //        {
    //            // Add to dict and change parent to ClearShot Cam in Gameplay Scene
    //            _activeGameplayCams.Add(vCam, sceneParent);
    //            vCam.transform.SetParent(_gameplayCams.transform);
    //            vCamSettings.LookAt = _playerFollowGroup.transform;

    //            // Check if it is a dolly cam
    //            if (vCamSettings.GetCinemachineComponent<CinemachineTrackedDolly>() != null)
    //            {
    //                // Set follow to the players, and lookat to null
    //                vCamSettings.Follow = _playerFollowGroup.transform;
    //            }
    //        }
    //    }
    //}

    //public void DeleteGameplayCamHandler(object data)
    //{
    //    if (data is not GameObject)
    //    {
    //        Debug.LogError("CameraManager did not receive a GameObject Cam parent!");
    //    }

    //    GameObject sceneParent = data as GameObject;
    //    List<GameObject> vCams = _activeGameplayCams.Keys.ToList(); // Get all active vcams

    //    // Remove vcams associated with received Scene parent
    //    foreach (GameObject vCam in vCams)
    //    {
    //        if (_activeGameplayCams[vCam] ==  sceneParent)
    //        {
    //            vCam.transform.SetParent(sceneParent.transform);
    //            _activeGameplayCams.Remove(vCam);
    //        }
    //    }
    //}
    #endregion
}