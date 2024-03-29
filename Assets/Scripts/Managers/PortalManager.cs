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
    public GameObject Pedestal { get; set; }

    public PortalData(Vector3 position, Quaternion rotation, GameObject portalPrefab, string playerTag, PortalInteraction portalInteractionScript, GameObject pedestal)
    {
        Position = position;
        Rotation = rotation;
        PortalPrefab = portalPrefab;
        PlayerTag = playerTag;
        PortalInteractionScript = portalInteractionScript;
        Pedestal = pedestal;
    }
}

public class PortalManager : MonoBehaviour
{
    private GameObject _lastPortalPlayer1 = null;
    private GameObject _lastPortalPlayer2 = null;
    private float checkRadius = 1.0f;  // Example radius, adjust based on your portal and mirror sizes

    public static PortalManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
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

        // Check if the existing portal is locked
        GameObject existingPortal = portalData.PlayerTag == "Player 1" ? _lastPortalPlayer1 : _lastPortalPlayer2;
        if (existingPortal != null)
        {
            PortalInfo PortalInfo = existingPortal.GetComponent<PortalInfo>();
            if (PortalInfo != null && PortalInfo.isLocked)
            {
                // Do not replace the locked portal
                //return;
            }
            else
            {
                // Destroy the existing portal if it's not locked
                Destroy(existingPortal);
            }
        }

        //Old logic
        //// Destroy the existing portal if it exists before creating a new one.
        //GameObject currentPortal = portalData.PlayerTag == "Player 1" ? _lastPortalPlayer1 : _lastPortalPlayer2;

        //if (currentPortal != null)
        //{
        //    Destroy(currentPortal);
        //}

        GameObject newPortal = Instantiate(portalData.PortalPrefab, portalData.Position, portalData.Rotation);
        PortalInfo portalInfo = newPortal.GetComponent<PortalInfo>();

        CheckForPlatformOverlap(newPortal);

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

    public void LockPortal(PortalInfo portalInfo)
    {
        if (portalInfo.isAlignedWithMirror)
        {
            // Check if both portals from Player 1 and Player 2 are aligned with their mirrors
            if (_lastPortalPlayer1.GetComponent<PortalInfo>().isAlignedWithMirror && _lastPortalPlayer2.GetComponent<PortalInfo>().isAlignedWithMirror)
            {
                // Lock both portals in place
                _lastPortalPlayer1.GetComponent<PortalInfo>().portalInteractionScript.LockPortal();
                _lastPortalPlayer2.GetComponent<PortalInfo>().portalInteractionScript.LockPortal();

                // Remove the portals from the manager, so they are no longer tracked for replacement
                _lastPortalPlayer1 = null;
                _lastPortalPlayer2 = null;
            }
        }
    }

    // After instantiating the portal, check for overlaps and manually invoke the necessary logic
    void CheckForPlatformOverlap(GameObject portal)
    {
        Collider[] hitColliders = Physics.OverlapSphere(portal.transform.position, checkRadius);
        foreach (var hitCollider in hitColliders)
        {
            ConstTrigger constTrigger = hitCollider.GetComponent<ConstTrigger>();
            if (constTrigger != null)
            {
                // Find the mirror on the platform
                GameObject mirror = constTrigger.FindMirror(); // Assuming you have a method to find the mirror
                if (mirror != null)
                {
                    constTrigger.HandlePortalOverlap(portal, mirror);
                }
            }
        }
    }

    public void CheckForMatchingPortals(ConstTrigger pedestal1, ConstTrigger pedestal2)
    {
        if (pedestal1.IsPortalPlaced() && pedestal2.IsPortalPlaced())
        {
            // Lock the portals on both pedestals
            pedestal1.LockPortal();
            pedestal2.LockPortal();
        }
    }

    private void LockPortalsOnPedestals(ConstTrigger pedestal1, ConstTrigger pedestal2)
    {
        // Lock the portals on these pedestals
        // Assuming we have a way to access the PortalInfo or similar component on the portal
        // For example:
        PortalInfo portal1 = pedestal1.GetComponentInChildren<PortalInfo>();
        PortalInfo portal2 = pedestal2.GetComponentInChildren<PortalInfo>();

        if (portal1 != null && portal2 != null)
        {
            portal1.LockPortal();
            portal2.LockPortal();

            // Remove the portals from the manager if necessary
        }
    }
}



