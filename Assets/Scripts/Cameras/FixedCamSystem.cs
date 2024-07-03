using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class FixedCamSystem : MonoBehaviour
{
    #region INTERNAL DATA
    private CinemachineVirtualCamera _cam;
    private CamTrigger _trigger;
    #endregion

    #region FRAMEWORK FUNCTIONS
    private void Awake()
    {
        // Set up components
        _cam = GetComponentInChildren<CinemachineVirtualCamera>();
        _trigger = GetComponentInChildren<CamTrigger>();
    }

    public void OnEnable()
    {
        // Register camera to CameraManager
        if (_cam != null)
        {
            EventManager.EventTrigger(EventType.CAMERA_REGISTER, _cam);
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
    // Check if this or camera has been selected in hierarchy
#if UNITY_EDITOR
    public void OnSelectionChange()
    {
        var selectedObj = Selection.activeObject;
        bool isSelected = false;

        if (selectedObj == _cam.gameObject || selectedObj == this.gameObject 
            || selectedObj == _trigger.gameObject)
        {
            isSelected = true;
        }

        // Change color of trigger box depending on if selected or not
        _trigger.IsSelected(isSelected);
    }
#endif
    #endregion
}
