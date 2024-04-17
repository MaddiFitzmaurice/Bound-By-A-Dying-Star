using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable, IPickupable
{
    #region INTERNAL DATA
    // Item Grouper
    private Transform _itemGrouper; // Makes sure that items stay in level scene


    // Item Version Data
    private List<GameObject> _itemVersions = new List<GameObject>();
    private int _currentVersionIndex = 0;

    // Player Interacting Data
    private PlayerBase _playerHoldingItem; // Player who is currently holding the item
    private bool _itemLocked = false;

    // Material Data
    private Material _defaultMat;

    // Components
    private Renderer _renderer;
    #endregion

    private void Awake()
    {
        // Get item skins, assign current item version and item grouper
        Collider[] list = GetComponentsInChildren<Collider>();
        if (list.Length == 0)
        {
            Debug.LogError("Item does not have version skins associated with it");
        }
        else
        {
            foreach (var item in list)
            {
                GameObject itemVersion = item.gameObject;
                // Start with all items inactive
                itemVersion.SetActive(false);
                _itemVersions.Add(itemVersion);
            }
            // Activate the first version
            _itemVersions[_currentVersionIndex].SetActive(true);
        }

        _itemGrouper = transform.parent;
        _renderer = GetComponentInChildren<Renderer>();
        _defaultMat = _renderer.material;
    }

    public void UnhighlightItem()
    {
        _renderer.material = _defaultMat;
    }

    public void HighlightItem(Material mat)
    {
        _renderer.material = mat;
    }

    public void RiftEffect(Transform pos)
    {
        // Update the current item version index. This will loop back to the 1st index once it has reached it's max index
        _currentVersionIndex = (_currentVersionIndex + 1) % _itemVersions.Count;

        // Deactivate all versions and activate the current one
        foreach (var item in _itemVersions)
        {
            item.SetActive(false);
        }
        _itemVersions[_currentVersionIndex].SetActive(true);
        transform.position = pos.position;
    }

    #region IPICKUPABLE FUNCTIONS
    public void PickupLocked(bool flag)
    {
        _itemLocked = flag;
    }

    public void BeDropped(Transform newParent)
    {
        // Removes the parent-child relationship, making the object independent in the scene
        // If an incoming parent is specified, use that. Else, use the default parent assigned in the scene
        Transform parent = newParent != null ? newParent : _itemGrouper;
        SetParent(parent);
  
        _playerHoldingItem.DropItem();
        _playerHoldingItem = null;
    }

    public void BePickedUp(PlayerBase player)
    {
        _playerHoldingItem = player;
        SetParent(_playerHoldingItem.PickupPoint);
        transform.localPosition = Vector3.zero;
        _playerHoldingItem.PickupItem(gameObject);
        UnhighlightItem();
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent); 
    }
    #endregion

    #region IINTERACTABLE FUNCTIONS
    public void PlayerInRange(PlayerBase player)
    {
        HighlightItem(player.HighlightMat);
    }

    public void PlayerNotInRange(PlayerBase player)
    {
        UnhighlightItem();
    }

    public void PlayerStartInteract(PlayerBase player)
    {
        // If a player is holding the item
        if (_playerHoldingItem != null)
        {
            // If player is holding this item
            if (_playerHoldingItem.CarriedPickupable == gameObject)
            {
                BeDropped(_itemGrouper);
            }
        }
        // If a player is near the item
        else
        {
            BePickedUp(player);
        }
    }
    #endregion
}
