using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    // External Data
    [SerializeField] private GameObject _itemVersion1;
    [SerializeField] private GameObject _itemVersion2;

    // Internal Data
    private bool _version1;
    private GameObject _currentItemVersion;
    private PlayerBase _playerInTrigger; // TODO: Potential for both players to be in trigger, change to list instead
    private PlayerBase _playerHoldingItem;
    private bool _isCarried = false;
    private Material _defaultMat;

    // Components
    private Renderer _renderer;

    private void Awake()
    {
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
        _isCarried = false;
        _playerHoldingItem.DropItem();
        _playerHoldingItem = null;
        // Removes the parent-child relationship, making the object independent in the scene
        SetItemParent(null); 
    }

    public void BePickedUp()
    {
        _playerHoldingItem = _playerInTrigger;
        SetItemParent(_playerInTrigger.PickupPoint);
        transform.localPosition = Vector3.zero;
        _playerHoldingItem.PickupItem(this.gameObject);
        _isCarried = true;
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

    public void PlayerInRange(Material mat)
    {
        HighlightItem(mat);
    }

    public void PlayerNotInRange()
    {
        UnhighlightItem();
    }

    public void PlayerStartInteract()
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
        else if (_playerInTrigger != null)
        {
            // If player is not already holding another item
            if (_playerInTrigger.CarriedItem == null && !_isCarried)
            {
                Debug.Log("Item picked up");
                BePickedUp();
            }
        }
    }
}
