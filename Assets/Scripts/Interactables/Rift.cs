using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rift : MonoBehaviour, IInteractable
{
    // Internal Data
    private PlayerBase _playerNearby;
    private Transform _target; // Link to other Rift if it exists
    
    // Material Data
    private Material _defaultMat;

    // Components
    private Renderer _renderer;

    private void Awake()
    {
        // Get components
        _renderer = GetComponentInChildren<Renderer>();

        // Set default material
        _defaultMat = _renderer.material;
    }

    public void UpdateTarget(Transform newTarget)
    {
        _target = newTarget;
    }

    public void UnhighlightItem()
    {
        _renderer.material = _defaultMat;
    }

    public void HighlightItem(Material mat)
    {
        _renderer.material = mat;
    }

    // If interact button is pressed near the Rift
    public void PlayerStartInteract(PlayerBase player)
    {   
        // If Rifts are linked and a player is in the trigger
        if (_target != null)
        {
            GameObject itemToSend = player.GetComponent<PlayerBase>().CarriedItem;

            // If the player is currently holding an item
            if (itemToSend != null)
            {
                Item item = itemToSend.GetComponent<Item>();
                Debug.Log("Item to teleport original position: " + itemToSend.transform.position);
                Debug.Log("Item to teleport new position: " + itemToSend.transform.position);
                item.SetTransform(_target);
                item.ChangeItemVersion();
                item.BeDropped();
                Debug.Log("Item teleported to target portal");
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
}
