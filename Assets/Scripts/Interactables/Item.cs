using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    #region EXTERNAL DATA
    // Item Versions
    [SerializeField] private GameObject _itemVersion1;
    [SerializeField] private GameObject _itemVersion2;
    #endregion

    #region INTERNAL DATA
    // Item Version Data
    private bool _version1;
    private GameObject _currentItemVersion;

    // Player Interacting Data
    private PlayerBase _playerHoldingItem; // Player who is currently holding the item

    // Material Data
    private Material _defaultMat;

    // Components
    private Renderer _renderer;
    #endregion

    private void Awake()
    {
        // Assign current item version
        _currentItemVersion = GetComponentInChildren<Transform>().gameObject;

        if (_currentItemVersion == null)
        {
            Debug.LogError("Item does not have version skin associated with it");
        }
        else 
        {
            _version1 = _currentItemVersion == _version1 ? true : false;
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

    public void BeDropped()
    {
        _playerHoldingItem.DropItem();
        _playerHoldingItem = null;
        // Removes the parent-child relationship, making the object independent in the scene
        SetItemParent(null); 
    }

    public void BePickedUp(PlayerBase player)
    {
        _playerHoldingItem = player;
        SetItemParent(_playerHoldingItem.PickupPoint);
        transform.localPosition = Vector3.zero;
        _playerHoldingItem.PickupItem(gameObject);
        UnhighlightItem();
    }

    public void SetItemParent(Transform parent)
    {
        transform.SetParent(parent); 
    }

    public void ChangeItemVersion()
    {
        Destroy(_currentItemVersion);

        GameObject newVersion = _version1 ? _itemVersion1 : _itemVersion2;
        _currentItemVersion = Instantiate(newVersion, transform);
    }

    #region INTERFACE FUNCTIONS
    public void PlayerInRange(Material mat)
    {
        HighlightItem(mat);
    }

    public void PlayerNotInRange()
    {
        UnhighlightItem();
    }

    public void PlayerStartInteract(PlayerBase player)
    {
        // If a player is holding the item
        if (_playerHoldingItem != null)
        {
            // If player is holding this item
            if (_playerHoldingItem.CarriedItem == gameObject)
            {
                Debug.Log("Item dropped");
                BeDropped();
            }
        }
        // If a player is near the item
        else
        {
            Debug.Log("Item picked up");
            BePickedUp(player);
        }
    }
    #endregion
}
