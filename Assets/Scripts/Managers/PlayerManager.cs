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
        EventManager.EventInitialise(EventType.PLAYERMANAGER_SEND_FOLLOWGROUP);
        EventManager.EventInitialise(EventType.SOFTPUZZLE_PLAYER_TELEPORT);
        EventManager.EventInitialise(EventType.PLAYERMANAGER_SEND_PLAYER1);
        EventManager.EventInitialise(EventType.PLAYERMANAGER_SEND_PLAYER2);
        EventManager.EventInitialise(EventType.PLAYERMANAGER_REQUEST_FOLLOWGROUP);

    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LEVEL_SPAWN, SpawnInLevel);
        EventManager.EventSubscribe(EventType.SOFTPUZZLE_PLAYER_TELEPORT, PlayerTeleport);
        EventManager.EventSubscribe(EventType.PLAYERMANAGER_REQUEST_FOLLOWGROUP, SendFollowGroup);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LEVEL_SPAWN, SpawnInLevel);
        EventManager.EventUnsubscribe(EventType.SOFTPUZZLE_PLAYER_TELEPORT, PlayerTeleport);
        EventManager.EventUnsubscribe(EventType.PLAYERMANAGER_REQUEST_FOLLOWGROUP, SendFollowGroup);
    }

    private void Start()
    {
        SendFollowGroup(null);
        SendPlayers();
    }

    private void SendPlayers()
    {
        EventManager.EventTrigger(EventType.PLAYERMANAGER_SEND_PLAYER1, _player1);
        EventManager.EventTrigger(EventType.PLAYERMANAGER_SEND_PLAYER2, _player2);
    }

    private void SendFollowGroup(object data)
    {
        if (_targetGroup != null)
        {
            EventManager.EventTrigger(EventType.PLAYERMANAGER_SEND_FOLLOWGROUP, _targetGroup);
        }
        else
        {
            Debug.LogError("No Cinemachine Target Group assigned!");
        }
    }

    private IEnumerator PlayerTeleport(Transform spawnPoint)
    {
        EventManager.EventTrigger(EventType.DISABLE_GAMEPLAY_INPUTS, null);
        _player1.PlayTeleportEffect(true);
        _player2.PlayTeleportEffect(true);
        yield return new WaitForSeconds(2f);
        _player1.PlayFlashEffect();
        _player2.PlayFlashEffect();
        _player1.ToggleVisibility(false);
        _player2.ToggleVisibility(false);
        _player1.ToggleClothPhysics(false);
        _player2.ToggleClothPhysics(false);
        yield return new WaitForSeconds(1.5f);
        _playerGrouper.transform.position = spawnPoint.position;
        _playerGrouper.transform.rotation = spawnPoint.rotation;
        _player1.transform.localPosition = new Vector3(-2, 2, 0);
        _player2.transform.localPosition = new Vector3(2, 2, 0);
        _player1.PlayTeleportEffect(false);
        _player2.PlayTeleportEffect(false);
        yield return new WaitForSeconds(1.5f);
        _player1.PlayFlashEffect();
        _player2.PlayFlashEffect();
        _player1.ToggleClothPhysics(true);
        _player2.ToggleClothPhysics(true);
        _player1.ToggleVisibility(true);
        _player2.ToggleVisibility(true);
        EventManager.EventTrigger(EventType.ENABLE_GAMEPLAY_INPUTS, null);
    }

    #region EVENT HANDLERS
    // Have players spawn at a desired location in a specific level
    public void SpawnInLevel(object data)
    {
        if (data is not Transform)
        {
            Debug.LogError("PlayerManager has not received a transform!");
        }

        Transform spawnPoint = (Transform)data;
        _player1.ToggleClothPhysics(false);
        _player2.ToggleClothPhysics(false);
        _playerGrouper.transform.position = spawnPoint.position;
        _playerGrouper.transform.rotation = spawnPoint.rotation;
        _player1.transform.localPosition = new Vector3(-2, 0, 0);
        _player2.transform.localPosition = new Vector3(2, 0, 0);
        _player1.ToggleClothPhysics(true);
        _player2.ToggleClothPhysics(true);
    }

    // Have players make VFX for teleportation then
    // move them to desired location in a specific level
    public void PlayerTeleport(object data)
    {
        if (data is not Transform)
        {
            Debug.LogError("PlayerManager has not received a transform!");
        }

        Transform spawnPoint = (Transform)data;
        StopAllCoroutines();
        StartCoroutine(PlayerTeleport(spawnPoint));
    }
    #endregion
}
