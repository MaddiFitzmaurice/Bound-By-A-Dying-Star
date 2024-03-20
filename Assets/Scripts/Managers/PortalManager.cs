using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance { get; private set; }

    private GameObject lastPortalPlayer1 = null;
    private GameObject lastPortalPlayer2 = null;

    private void Awake()
    {
        //if (Instance == null)
        //{
        //    Instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    public void CreatePortal(Vector3 position, Quaternion rotation, GameObject portalPrefab, string playerTag, PortalInteraction portalInteractionScript)
    {
        // Destroy the existing portal if it exists before creating a new one.
        GameObject currentPortal = playerTag == "Player 1" ? lastPortalPlayer1 : lastPortalPlayer2;
        if (currentPortal != null)
        {
            Destroy(currentPortal);
        }

        GameObject newPortal = Instantiate(portalPrefab, position, rotation);
        PortalInfo portalInfo = newPortal.GetComponent<PortalInfo>();

        // Update the last portal reference for this player.
        if (playerTag == "Player 1")
        {
            lastPortalPlayer1 = newPortal;
        }
        else // Player 2
        {
            lastPortalPlayer2 = newPortal;
        }

        // Attempt to link portals between players if both exist.
        if (lastPortalPlayer1 != null && lastPortalPlayer2 != null)
        {
            lastPortalPlayer1.GetComponent<PortalInfo>().SetDestinationPortal(lastPortalPlayer2.transform);
            lastPortalPlayer2.GetComponent<PortalInfo>().SetDestinationPortal(lastPortalPlayer1.transform);
        }

        // Associate the creating player's PortalInteraction.
        portalInfo.portalInteractionScript = portalInteractionScript; 
    }
}



