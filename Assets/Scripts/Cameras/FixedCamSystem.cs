using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class FixedCamSystem : MonoBehaviour
{
    #region EXTERNAL DATA
    [Tooltip("FIXED: Stationary, not looking at players.\n" +
        "DOLLY: Movement, not looking at players.\n" +
        "DOLLY_LOOK: Movement, looking at players.")]
    [SerializeField] private CameraType _camType = CameraType.FIXED;
    #endregion

    #region INTERNAL DATA
    private CinemachineVirtualCamera _cam;
    private List<CamTrigger> _triggers;
    private CameraData _camData;
    #endregion

    #region FRAMEWORK FUNCTIONS
    private void Awake()
    {
        // Set up components
        _cam = GetComponentInChildren<CinemachineVirtualCamera>();
        _triggers = GetComponentsInChildren<CamTrigger>().ToList();

        // Set up camera data
        // Check if dolly track is present if camera is set to follow players
        if (_camType == CameraType.DOLLY || _camType == CameraType.DOLLY_LOOK)
        {
            if (_cam.GetCinemachineComponent<CinemachineTrackedDolly>() == null)
            {
                Debug.LogError("Camera is set to follow players but has no dolly track!");
            }
        }

        _camData = new CameraData(_cam, _camType);
    }

    public void OnEnable()
    {
        // Register camera to CameraManager
        if (_cam != null)
        {
            EventManager.EventTrigger(EventType.CAMERA_REGISTER, _camData);
        }
        else
        {
            Debug.LogError("No camera assigned to this trigger!");
        }
#if UNITY_EDITOR
        Selection.selectionChanged += OnSelectionChange;
#endif
    }

    public void OnDisable()
    {
        // Deregister camera from CameraManager
        if (_cam != null)
        {
            EventManager.EventTrigger(EventType.CAMERA_DEREGISTER, _cam);
        }
        else
        {
            Debug.LogError("No camera assigned to this trigger!");
        }
#if UNITY_EDITOR
        Selection.selectionChanged -= OnSelectionChange;
#endif
    }
    #endregion

    #region EXTERNAL FUNCTIONS
    // If both players are in trigger box, activate camera
    public void Triggered()
    {
        EventManager.EventTrigger(EventType.CAMERA_ACTIVATE, _cam);   
    }
    #endregion

    #region EVENT FUNCTIONS
    // Check if this, triggers, or camera has been selected in hierarchy
    // If yes, make the camera and triggers active/highlighted in editor
#if UNITY_EDITOR
    public void OnSelectionChange()
    {
        GameObject selectedObj = Selection.activeObject as GameObject;
        bool isSelected = false;

        if (selectedObj == null) { return; }

        // If selected obj is the parent or camera
        if (selectedObj == _cam.gameObject || selectedObj == this.gameObject)
        {
            isSelected = true;
            _cam.Priority = 1;
        }
        // Else if selected obj is one of the triggers
        else if (selectedObj.GetComponent<CamTrigger>() != null)
        {
            foreach (CamTrigger trigger in _triggers)
            {
                if (selectedObj == trigger.gameObject)
                {
                    isSelected = true;
                    break;
                }
            }

            _cam.Priority = isSelected ? 1 : 0;
        }
        else
        {
            _cam.Priority = 0;
        }

        // Change color of trigger box depending on if selected or not
        foreach (CamTrigger trigger in _triggers)
        {
            trigger.IsSelected(isSelected);
        }
    }
#endif
    #endregion
}
