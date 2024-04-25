using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rift : MonoBehaviour, IInteractable
{
    #region INTERNAL DATA
    // Target Rift
    private Transform _targetRift; // Link to other Rift if it exists
    
    // Material Data
    private Material _defaultMat;

    // Components
    private Renderer _renderer;
    private ParticleSystem _riftSendEffect;
    #endregion

    private void Awake()
    {
        // Get components
        _renderer = GetComponentInChildren<Renderer>();
        _riftSendEffect = GetComponentInChildren<ParticleSystem>();

        // Set default material
        _defaultMat = _renderer.material;
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.RIFT_SEND_EFFECT, PortalEffectHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.RIFT_SEND_EFFECT, PortalEffectHandler);
    }

    public void UpdateTargetRift(Transform newTarget)
    {
        _targetRift = newTarget;
    }

    public void UnhighlightItem()
    {
        _renderer.material = _defaultMat;
    }

    public void HighlightItem(Material mat)
    {
        _renderer.material = mat;
    }

    private void PortalEffectHandler(object data)
    {
        //Debug.Log("effect play");
        _riftSendEffect.Emit(50);
    }

    #region IINTERACTABLE FUNCTIONS
    // If interact button is pressed near the Rift
    public void PlayerStartInteract(PlayerBase player)
    {   
        // If Rifts are linked and a player is in the trigger
        if (_targetRift != null)
        {
            GameObject itemToSend = player.GetComponent<PlayerBase>().CarriedPickupable;
            Item itemType = itemToSend.GetComponent<Item>();

            // If the player is currently holding an item
            if (itemToSend != null && itemType != null)
            {
                itemType.RiftEffect(_targetRift.transform);
                itemType.BeDropped(null);
            }
        }
    }

    public void PlayerInRange(PlayerBase player)
    {
        HighlightItem(player.HighlightMat);
    }

    public void PlayerNotInRange(PlayerBase player)
    {
        UnhighlightItem();
    }

    public void PlayerHoldInteract(PlayerBase player)
    {
    }

    public void PlayerStopInteract(PlayerBase player)
    {
    }
    #endregion
}
