using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // The other portal this one links to
    public Transform destinationPortal;
    // Store the reference to the player's PortalInteraction script
    public PortalInteraction portalInteractionScript;

    public void SetDestinationPortal(Transform destination)
    {
        destinationPortal = destination;
    }
}
