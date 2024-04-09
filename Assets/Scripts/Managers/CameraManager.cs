using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CamDirections
{
    FORWARD,
    LEFT,
    RIGHT
}

public class CameraManager : MonoBehaviour
{
    #region INTERNAL DATA
    private CinemachineVirtualCamera _forwardCam;
    private CinemachineVirtualCamera _leftCam;
    private CinemachineVirtualCamera _rightCam;

    private void Awake()
    {
        EventManager.EventInitialise(EventType.GAMEPLAY_CAMS_INIT);
        EventManager.EventInitialise(EventType.GAMEPLAY_CAMS_CHANGE);
    }

    #endregion
    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.GAMEPLAY_CAMS_INIT, GameplayCamsInit);
        EventManager.EventSubscribe(EventType.GAMEPLAY_CAMS_CHANGE, GameplayCamsChange);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.GAMEPLAY_CAMS_INIT, GameplayCamsInit);
        EventManager.EventUnsubscribe(EventType.GAMEPLAY_CAMS_CHANGE, GameplayCamsChange);
    }

    #region EVENT HANDLERS
    // Receive the Gameplay Cameras and set them up
    public void GameplayCamsInit(object data)
    {
        if (data is not GameObject)
        {
            Debug.LogError("CameraManager did not receive a GameObject!");
        }

        GameObject cams = data as GameObject;
        var listOfCams = cams.GetComponentsInChildren<CinemachineVirtualCamera>().ToList();

        // TEMPORARY CODE BELOW
        if (listOfCams.Count != 3) 
        {
            Debug.LogError("3 Cameras not present!");
        }

        Vector3 camAngle = listOfCams[0].transform.eulerAngles;
        listOfCams[0].transform.eulerAngles = new Vector3(camAngle.x, 0, camAngle.z);
        camAngle = listOfCams[1].transform.eulerAngles;
        listOfCams[1].transform.eulerAngles = new Vector3(camAngle.x, 90, camAngle.z);
        camAngle = listOfCams[2].transform.eulerAngles;
        listOfCams[2].transform.eulerAngles = new Vector3(camAngle.x, 270, camAngle.z);

        // Don't know how else to distinguish them so doing this for now
        foreach (CinemachineVirtualCamera cam in listOfCams)
        {
            if (cam.transform.rotation.eulerAngles.y == 0)
            {
                _forwardCam = cam;
            }
            else if (cam.transform.rotation.eulerAngles.y == 90)
            {
                _rightCam = cam;
            }
            else if (cam.transform.rotation.eulerAngles.y == 270)
            {
                _leftCam = cam;
            }
        }
    }

    // Change the camera according to the direction the players are heading in
    public void GameplayCamsChange(object data)
    {
        if (data is not CamDirections)
        {
            Debug.LogError("CameraManager has not received a CamDirections!");
        }

        ChangeCamPriorities((CamDirections)data);
    }
    #endregion

    public void ChangeCamPriorities(CamDirections dir)
    {
        if (dir == CamDirections.FORWARD)
        {
            _forwardCam.Priority = 1;
            _leftCam.Priority = _rightCam.Priority = 0;
        }
        else if (dir == CamDirections.LEFT)
        {
            _leftCam.Priority = 1;
            _forwardCam.Priority = _rightCam.Priority = 0;
        }
        else if (dir == CamDirections.RIGHT)
        {
            _rightCam.Priority = 1;
            _leftCam.Priority = _forwardCam.Priority = 0;
        }
    }
}