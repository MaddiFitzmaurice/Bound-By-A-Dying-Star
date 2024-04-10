using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rift : MonoBehaviour, IInteractable
{
    #region EXTERNAL DATA
    [SerializeField] private ParticleSystem _portalSendEffect;
    #endregion
    #region INTERNAL DATA
    // Target Rift
    private Transform _targetRift; // Link to other Rift if it exists
    
    // Material Data
    private Material _defaultMat;

    // Components
    private Renderer _renderer;
    #endregion

    private void Awake()
    {
        // Get components
        _renderer = GetComponentInChildren<Renderer>();

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
        _portalSendEffect.Emit(50);
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
                itemType.ChangeItemVersion();
                itemType.BeDropped(_targetRift);
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
    #endregion
}
