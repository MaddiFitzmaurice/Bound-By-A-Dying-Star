using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerManager : MonoBehaviour
{
    #region EXTERNAL DATA
    [Header("Player Data")]
    [SerializeField] private GameObject _playerGrouper;
    #endregion

    #region INTERNAL DATA
    // Players
    private Player1 _player1;
    private Player2 _player2;

    // Camera Data
    private CinemachineTargetGroup _targetGroup;
    #endregion

    private void Awake()
    {
        // Get Components
        _targetGroup = _playerGrouper.GetComponentInChildren<CinemachineTargetGroup>();

        // Get Players
        _player1 = _playerGrouper.GetComponentInChildren<Player1>();
        _player2 = _playerGrouper.GetComponentInChildren<Player2>();

        // Event Inits
        EventManager.EventInitialise(EventType.LEVEL_CAMS_SEND_FOLLOWGROUP);
        EventManager.EventInitialise(EventType.TELEPORT_PLAYERS);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LEVEL_CAMS_REQUEST_FOLLOWGROUP, SendFollowGroup);
        EventManager.EventSubscribe(EventType.TELEPORT_PLAYERS, TeleportPlayers);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LEVEL_CAMS_REQUEST_FOLLOWGROUP, SendFollowGroup);
        EventManager.EventUnsubscribe(EventType.TELEPORT_PLAYERS, TeleportPlayers);
    }

    #region EVENT HANDLERS
    // Send LevelManager the VCamFollowGroup
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

    // Listen to Teleport and assign new position to player grouper
    public void TeleportPlayers(object data)
    {
        if (data is not Transform)
        {
            Debug.LogError("PlayerManager has not received a valid Transform to teleport players!");
        }

        Transform transform = (Transform)data;

        if (_playerGrouper == null)
        {
            Debug.LogError("Cannot teleport players, _playerGrouper not assigned!");
        }
        else
        {
            // TODO: Adjust height when 3D models are put in
            _playerGrouper.transform.position = transform.position;
            _player1.transform.localPosition = new Vector3(2, 1, 0);
            _player2.transform.localPosition = new Vector3(-2, 1, 0);
        }
    }
    #endregion
}