using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region INTERNAL DATA
    private CinemachineVirtualCamera _forwardCam;
    private CinemachineVirtualCamera _leftCam;
    private CinemachineVirtualCamera _rightCam;

    #endregion
    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.GAMEPLAY_CAMS_INIT, GameplayCamsInit);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.GAMEPLAY_CAMS_INIT, GameplayCamsInit);
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

        // Don't know how else to distinguish them so doing this for now
        foreach (CinemachineVirtualCamera cam in listOfCams)
        {
            if (cam.transform.rotation.y == 0)
            {
                _forwardCam = cam;
            }
            else if (cam.transform.rotation.y == 90)
            {
                _rightCam = cam;
            }
            else if (cam.transform.rotation.y == -90)
            {
                _leftCam = cam;
            }
        }
    }

    // Change the camera according to the direction the players are heading in
    public void ChangeGameplayCam(object data)
    {

    }
    #endregion
}