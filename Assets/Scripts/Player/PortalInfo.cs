using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalInfo : MonoBehaviour
{
    // The other portal this one links to
    public Transform destinationPortal;
    // Store the reference to the player's PortalInteraction script
    public PortalInteraction portalInteractionScript;
    public bool isAlignedWithMirror = false;

    public GameObject Pedestal { get; set; }
    public void SetDestinationPortal(Transform destination)
    {
        destinationPortal = destination;
    }

    // When the portal aligns with a mirror
    public void AlignWithMirror()
    {
        isAlignedWithMirror = true;
    }
}
