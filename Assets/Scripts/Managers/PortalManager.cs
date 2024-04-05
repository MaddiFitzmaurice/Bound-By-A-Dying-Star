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
    public PortalInteraction PortalInteractionScript { get; private set; }
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
    // Adjust radius for the portal to detect pedestal
    private float _checkRadius = 2.0f;  

    public static PortalManager Instance { get; private set; }

    private void Awake()
    {
        EventManager.EventInitialise(EventType.PORTALMANAGER_CREATEPORTAL);
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

        //Logic to keep portals remaining on Pedestal
        //// Check if the existing portal is locked
        //GameObject existingPortal = portalData.PlayerTag == "Player 1" ? _lastPortalPlayer1 : _lastPortalPlayer2;
        //if (existingPortal != null)
        //{
        //    PortalInfo PortalInfo = existingPortal.GetComponent<PortalInfo>();
        //    if (PortalInfo != null && PortalInfo.isLocked)
        //    {
        //        // Do not replace the locked portal
        //        return;
        //    }
        //    else
        //    {
        //        // Destroy the existing portal if it's not locked
        //        Destroy(existingPortal);
        //    }
        //}

        //Old logic
        // Destroy the existing portal if it exists before creating a new one.
        GameObject currentPortal = portalData.PlayerTag == "Player 1" ? _lastPortalPlayer1 : _lastPortalPlayer2;

        if (currentPortal != null)
        {
            Destroy(currentPortal);
        }

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

    // After instantiating the portal, check to see if it overlaps with the pedestal
    void CheckForPlatformOverlap(GameObject portal)
    {
        Collider[] hitColliders = Physics.OverlapSphere(portal.transform.position, _checkRadius);
        foreach (var hitCollider in hitColliders)
        {
            PedestalConstellation constTrigger = hitCollider.GetComponent<PedestalConstellation>();
            if (constTrigger != null)
            {
                // Find the mirror on the platform
                GameObject mirror = constTrigger.FindMirror();
                if (mirror != null)
                {
                    constTrigger.HandlePortalOverlap(portal, mirror);
                }
            }
        }
    }

    public void CheckForMatchingPortals(PedestalConstellation pedestal1, PedestalConstellation pedestal2)
    {
        //Check to see if both portals have been placed on the pedestal
        if (pedestal1.IsPortalPlaced() && pedestal2.IsPortalPlaced())
        {
            // Activate the particle effects
            //pedestal1.ActivateEffect();
            //pedestal2.ActivateEffect();

            //Destroy portals
            Destroy(pedestal1.currentPortal);
            Destroy(pedestal2.currentPortal);

            // Reset the isPortalPlaced flag to allow new portals to be placed and checked
            pedestal1.isPortalPlaced = false;
            pedestal2.isPortalPlaced = false;
        }
    }

}



