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
    #endregion
    private void Awake()
    {
        // Get Components
        _cmBrain = GetComponentInChildren<CinemachineBrain>();

        // Init Events
        EventManager.EventInitialise(EventType.LEVEL_CAMS_YROT);    
    }

    // Called when Cinemachine Brain detects a camera change
    public void LevelCameraChangeEvent()
    {
        // Make sure active camera is one contained within a Level
        if (_cmBrain.ActiveVirtualCamera.VirtualCameraGameObject.transform.parent.GetComponentInParent<LevelManager>())
        {
            CinemachineClearShot clearShot = _cmBrain.ActiveVirtualCamera as CinemachineClearShot;
            EventManager.EventTrigger(EventType.LEVEL_CAMS_YROT, clearShot.LiveChild.VirtualCameraGameObject.transform.localEulerAngles.y);
        }
    }
}