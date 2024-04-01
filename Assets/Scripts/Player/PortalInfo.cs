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

    [SerializeField] private ParticleSystem _portalSendEffect;

    public GameObject Pedestal { get; set; }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PORTALSENDEFFECT, PortalEffectHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PORTALSENDEFFECT, PortalEffectHandler);
    }


    public void SetDestinationPortal(Transform destination)
    {
        destinationPortal = destination;
    }

    // When the portal aligns with a mirror
    public void AlignWithMirror()
    {
        isAlignedWithMirror = true;
    }

    private void PortalEffectHandler(object data)
    {
        //Debug.Log("effect play");
        _portalSendEffect.Emit(50);
    }
}
