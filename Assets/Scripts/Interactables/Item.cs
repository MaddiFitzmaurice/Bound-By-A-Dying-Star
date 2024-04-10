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
    private bool _isVersion1;
    private GameObject _itemVersion1;
    private GameObject _itemVersion2;

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
        _itemVersion1 = list[0].gameObject;
        _itemVersion2 = list[1].gameObject;
        _itemGrouper = transform.parent;

        if (list.Length == 0)
        {
            Debug.LogError("Item does not have version skins associated with it");
        }
        else 
        {
            _isVersion1 = true;
            _itemVersion1.SetActive(true);
            _itemVersion2.SetActive(false);
        }

        // Get components
        _renderer = GetComponentInChildren<Renderer>();

        // Set default material
        _defaultMat = _renderer.material;
    }

    public void SetTransform(Transform newTransform)
    {
        transform.position = newTransform.position;
    }   

    public void UnhighlightItem()
    {
        _renderer.material = _defaultMat;
    }

    public void HighlightItem(Material mat)
    {
        _renderer.material = mat;
    }

    public void ChangeItemVersion()
    {
        _isVersion1 = !_isVersion1;
        _itemVersion1.SetActive(_isVersion1);
        _itemVersion2.SetActive(!_isVersion1);
    }

    public void Transform()
    {
        Debug.Log("Transform!");
        ChangeItemVersion();
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
        SetParent(newParent);
  
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
