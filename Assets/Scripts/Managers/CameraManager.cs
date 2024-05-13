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
    private GameObject _gameplayCams; // Transform to parent child vCams to it
    private Dictionary<GameObject, GameObject> _activeGameplayCams; // Key: VCam gameObj, Value: Level/Softpuzzle gameObj parent
    #endregion
    private void Awake()
    {
        // Get Components
        _cmBrain = GetComponentInChildren<CinemachineBrain>();

        // Init Events
        EventManager.EventInitialise(EventType.CLEARSHOT_CAMS_YROT);

        // Init Data
        _activeGameplayCams = new Dictionary<GameObject, GameObject>();
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.RECEIVE_GAMEPLAY_CAM_PARENT, ReceiveGameplayCamParentHandler);
        EventManager.EventSubscribe(EventType.ADD_GAMEPLAY_CAM, AddGameplayCamHandler);
        EventManager.EventSubscribe(EventType.DELETE_GAMEPLAY_CAM, DeleteGameplayCamHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.RECEIVE_GAMEPLAY_CAM_PARENT, ReceiveGameplayCamParentHandler);
        EventManager.EventUnsubscribe(EventType.ADD_GAMEPLAY_CAM, AddGameplayCamHandler);
        EventManager.EventUnsubscribe(EventType.DELETE_GAMEPLAY_CAM, DeleteGameplayCamHandler);
    }

    // Called when Cinemachine Brain detects a camera change
    public void LevelCameraChangeEvent()
    {
        // Make sure active camera is one contained within a Level or SoftPuzzle
        if (_cmBrain.ActiveVirtualCamera.VirtualCameraGameObject.transform.parent.GetComponentInParent<GameplayCamerasManager>())
        {
            CinemachineClearShot clearShot = _cmBrain.ActiveVirtualCamera as CinemachineClearShot;
            EventManager.EventTrigger(EventType.CLEARSHOT_CAMS_YROT, clearShot.LiveChild.VirtualCameraGameObject.transform.localEulerAngles.y);
        }
    }

    #region EVENT FUNCTIONS
    public void ReceiveGameplayCamParentHandler(object data)
    {
        if (data is not GameObject)
        {
            Debug.LogError("CameraManager did not receive a GameObject!");
        }

        _gameplayCams = data as GameObject;
    }

    public void AddGameplayCamHandler(object data)
    {
        if (data is not GameObject)
        {
            Debug.LogError("CameraManager did not receive a GameObject Cam parent!");
        }

        GameObject sceneParent = data as GameObject;
        Debug.Log(sceneParent);

        // Get list of vCams that are childed to the received sceneParent as gameobjects
        List<Transform> vCamTransforms = sceneParent.GetComponentsInChildren<Transform>().ToList();
        List<GameObject> vCams = new List<GameObject>();

        foreach (Transform child in  vCamTransforms)
        {
            vCams.Add(child.gameObject);
        }

        // Then add to Active Gameplay Cams Dictionary
        foreach (GameObject vCam in vCams)
        {
            // Make sure a virtual camera is attached to it
            if (vCam.GetComponent<CinemachineVirtualCamera>() != null)
            {
                // Add to dict and change parent to ClearShot Cam in Gameplay Scene
                _activeGameplayCams.Add(vCam, sceneParent);
                Debug.Log(_gameplayCams);
                vCam.transform.SetParent(_gameplayCams.transform);
            }
        }

        //LevelCameraChangeEvent();
    }

    public void DeleteGameplayCamHandler(object data)
    {
        if (data is not GameObject)
        {
            Debug.LogError("CameraManager did not receive a GameObject Cam parent!");
        }

        GameObject sceneParent = data as GameObject;
        List<GameObject> vCams = _activeGameplayCams.Keys.ToList(); // Get all active vcams

        // Remove vcams associated with received Scene parent
        foreach (GameObject vCam in vCams)
        {
            if (_activeGameplayCams[vCam] ==  sceneParent)
            {
                vCam.transform.SetParent(sceneParent.transform);
                _activeGameplayCams.Remove(vCam);
            }
        }
    }
    #endregion
}