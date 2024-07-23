using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    #region EXTERNAL DATA
    [SerializeField] private float _rotationSpeed = 10f;
    #endregion
    #region INTERNAL DATA
    private Transform _playerGrouper;
    #endregion

    private void Start()
    {
        RequestFollowGroup();
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYERMANAGER_SEND_FOLLOWGROUP, ReceiveFollowGroup);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYERMANAGER_SEND_FOLLOWGROUP, ReceiveFollowGroup);
    }

    private void Update()
    {
        if (_playerGrouper != null)
        {
            Vector3 lookDir = _playerGrouper.transform.position - transform.position;
            Quaternion fullRot = Quaternion.LookRotation(lookDir, Vector3.up);
            Quaternion lookRot = Quaternion.identity;
            lookRot.eulerAngles = new Vector3(0, fullRot.eulerAngles.y, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, _rotationSpeed * Time.deltaTime);
        }
    }

    // Receive the player grouper transform so whatever is attached to the script can look at the players at all times
    private void RequestFollowGroup()
    {
        EventManager.EventTrigger(EventType.PLAYERMANAGER_REQUEST_FOLLOWGROUP, null);
    }

    private void ReceiveFollowGroup(object data)
    {
        if (data is not CinemachineTargetGroup)
        {
            Debug.LogError($"LookAtPlayer on {this} did not receive a CinemachineTargetGroup");
        }
        else
        {
            CinemachineTargetGroup cinemachineTargetGroup = (CinemachineTargetGroup)data;
            _playerGrouper = cinemachineTargetGroup.gameObject.transform;
        }
    }
}
