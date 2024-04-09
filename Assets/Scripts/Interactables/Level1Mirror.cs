using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Level1Mirror : MonoBehaviour, IInteractable
{
    #region EXTERNAL DATA
    [SerializeField] private float _maxIntensity = 5f;
    [SerializeField] private float _maxDistance = 10f;
    #endregion

    #region INTERNAL DATA
    // Components
    private Light _light;

    // Player
    private PlayerBase _playerNearby;
    private PlayerBase _playerHoldingMirror;

    // Mirror Grouper
    private Transform _mirrorGrouper; // Makes sure that mirror stay in level scene
    #endregion

    private void Awake()
    {
        // Get components
        _light = GetComponentInChildren<Light>();
        _mirrorGrouper = transform.parent;

        _light.intensity = 0;
    }

    private void AdjustLightIntensity()
    {
        // Calculate the distance between the player and the mirror
        float distance = Vector3.Distance(_playerNearby.transform.position, transform.position);

        // Normalize the distance based on maxDistance via the clamp method
        float normalizedDistance = Mathf.Clamp01(distance / _maxDistance);

        // Adjust the light intensity
        _light.intensity = _maxIntensity * (1 - normalizedDistance);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            _playerNearby = other.GetComponent<PlayerBase>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_playerNearby != null)
        {
            AdjustLightIntensity();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _playerNearby = null;
    }

    public void BeDropped()
    {
        // Removes the parent-child relationship, making the object independent in the scene
        SetItemParent(_mirrorGrouper);
        _playerHoldingMirror.DropItem();
        _playerHoldingMirror = null;
    }

    public void BePickedUp(PlayerBase player)
    {
        _playerHoldingMirror = player;
        SetItemParent(_playerHoldingMirror.PickupPoint);
        transform.localPosition = Vector3.zero;
        _playerHoldingMirror.PickupItem(gameObject);
    }

    public void SetItemParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    #region INTERFACE FUNCTIONS
    public void PlayerInRange(PlayerBase player)
    {
        //_playerNearby = player;
    }

    public void PlayerNotInRange(PlayerBase player)
    {
        //_playerNearby = null;
    }

    public void PlayerStartInteract(PlayerBase player)
    {
        // If a player is holding the mirror
        if (_playerHoldingMirror != null)
        {
            // If player is holding this item
            if (_playerHoldingMirror.CarriedItem == gameObject)
            {
                BeDropped();
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
