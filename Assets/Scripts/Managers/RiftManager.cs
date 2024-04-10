using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Data class for player rifts
public class RiftData
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public string PlayerTag { get; private set; }

    public RiftData(Vector3 position, Quaternion rotation, string playerTag)
    {
        Position = position;
        Rotation = rotation;
        PlayerTag = playerTag;
    }
}

// Manager class for player rifts
public class RiftManager : MonoBehaviour
{
    // External Data
    [SerializeField] private GameObject _player1RiftPrefab;
    [SerializeField] private GameObject _player2RiftPrefab;

    // Internal Data
    private GameObject _lastRiftPlayer1 = null;
    private GameObject _lastRiftPlayer2 = null;

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.CREATE_RIFT, CreateRift);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.CREATE_RIFT, CreateRift);
    }

   public void CreateRift(object data)
    {
        if (data is not RiftData)
        {
            Debug.LogError("RiftManager has not received a RiftData object!");
        }

        RiftData riftData = (RiftData)data;

        // Destroy the existing rift if it exists before creating a new one.
        GameObject currentRift = riftData.PlayerTag == "Player1" ? _lastRiftPlayer1 : _lastRiftPlayer2;

        if (currentRift != null)
        {
            Destroy(currentRift);
        }

        // Create Rift for specified player and update the last Rift reference for this player.
        if (riftData.PlayerTag == "Player1")
        {
            _lastRiftPlayer1 = Instantiate(_player1RiftPrefab, riftData.Position, riftData.Rotation);
        }
        else // Player 2
        {
            _lastRiftPlayer2 = Instantiate(_player2RiftPrefab, riftData.Position, riftData.Rotation);
        }

        // Attempt to link Rifts between players if both exist.
        if (_lastRiftPlayer1 != null && _lastRiftPlayer2 != null)
        {
            _lastRiftPlayer1.GetComponent<Rift>().UpdateTargetRift(_lastRiftPlayer2.transform);
            _lastRiftPlayer2.GetComponent<Rift>().UpdateTargetRift(_lastRiftPlayer1.transform);
        }
    }
}
