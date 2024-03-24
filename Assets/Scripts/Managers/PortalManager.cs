using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

// Data class for player portals
public class PortalData
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public GameObject PortalPrefab { get; private set; }
    public string PlayerTag { get; private set; }
    public PortalInteraction PortalInteractionScript { get; private set; } // NOTE: Can combine player tag and this into one later

    public PortalData(Vector3 position, Quaternion rotation, GameObject portalPrefab, string playerTag, PortalInteraction portalInteractionScript)
    {
        Position = position;
        Rotation = rotation;
        PortalPrefab = portalPrefab;
        PlayerTag = playerTag;
        PortalInteractionScript = portalInteractionScript;
    }
}

public class PortalManager : MonoBehaviour
{
    private GameObject _lastPortalPlayer1 = null;
    private GameObject _lastPortalPlayer2 = null;

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PORTALMANAGER_CREATEPORTAL, CreatePortal);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PORTALMANAGER_CREATEPORTAL, CreatePortal);
    }

    public void CreatePortal(object data)
    {
        if (data is not PortalData)
        {
            Debug.LogError("PortalManager has not received a PortalData object!");
        }

        PortalData portalData = (PortalData)data;

        // Destroy the existing portal if it exists before creating a new one.
        GameObject currentPortal = portalData.PlayerTag == "Player 1" ? _lastPortalPlayer1 : _lastPortalPlayer2;

        if (currentPortal != null)
        {
            Destroy(currentPortal);
        }

        GameObject newPortal = Instantiate(portalData.PortalPrefab, portalData.Position, portalData.Rotation);
        PortalInfo portalInfo = newPortal.GetComponent<PortalInfo>();

        // Update the last portal reference for this player.
        if (portalData.PlayerTag == "Player 1")
        {
            _lastPortalPlayer1 = newPortal;
        }
        else // Player 2
        {
            _lastPortalPlayer2 = newPortal;
        }

        // Attempt to link portals between players if both exist.
        if (_lastPortalPlayer1 != null && _lastPortalPlayer2 != null)
        {
            _lastPortalPlayer1.GetComponent<PortalInfo>().SetDestinationPortal(_lastPortalPlayer2.transform);
            _lastPortalPlayer2.GetComponent<PortalInfo>().SetDestinationPortal(_lastPortalPlayer1.transform);
        }

        // Associate the creating player's PortalInteraction.
        portalInfo.portalInteractionScript = portalData.PortalInteractionScript; 
    }
}



