using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalInfo : MonoBehaviour
{
    // The other portal this one links to
    public Transform destinationPortal;
    // Store the reference to the player's PortalInteraction script
    public PortalInteraction portalInteractionScript;

    public void SetDestinationPortal(Transform destination)
    {
        destinationPortal = destination;
    }
}
