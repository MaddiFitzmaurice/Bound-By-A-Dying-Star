using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerManager : MonoBehaviour
{
    #region EXTERNAL DATA
    [Header("Camera Data")]
    [SerializeField] CinemachineTargetGroup _targetGroup;    
    #endregion

    private void Awake()
    {
        // Event Inits
        EventManager.EventInitialise(EventType.LEVEL_CAMS_SEND_FOLLOWGROUP);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LEVEL_CAMS_REQUEST_FOLLOWGROUP, SendFollowGroup);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LEVEL_CAMS_REQUEST_FOLLOWGROUP, SendFollowGroup);
    }

    #region EVENT HANDLERS
    public void SendFollowGroup(object data)
    {
        if (_targetGroup != null)
        {
            EventManager.EventTrigger(EventType.LEVEL_CAMS_SEND_FOLLOWGROUP, _targetGroup);
        }
        else 
        {
            Debug.LogError("No Cinemachine Target Group assigned!");
        }
    }
    #endregion
}
