using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Level1Mirror : MonoBehaviour, IInteractable, IPickupable
{
    #region EXTERNAL DATA
    [SerializeField] private float _maxIntensity = 5f;
    [SerializeField] private float _maxDistance = 10f;
    #endregion

    #region INTERNAL DATA
    // Components
    private Light _light;

    // Player
    private PlayerBase _player;

    // Mirror Grouper
    private Transform _mirrorGrouper; // Makes sure that mirror stay in level scene

    // Pedestal
    private bool _isOnPedestal = false;
    #endregion

    private void Awake()
    {
        // Get components
        _light = GetComponentInChildren<Light>();
        _mirrorGrouper = transform.parent;

        _light.intensity = 0;
    }

    // TODO: MAKE THIS INTO A COROUTINE THAT ADJUSTS INTENSITY WHEN NEARBY INSTEAD OF RELYING ON DISTANCE
    private void AdjustLightIntensity()
    {
        // Calculate the distance between the player and the mirror
        float distance = Vector3.Distance(_player.transform.position, transform.position);

        // Normalize the distance based on maxDistance via the clamp method
        float normalizedDistance = Mathf.Clamp01(distance / _maxDistance);

        // Adjust the light intensity
        _light.intensity = _maxIntensity * (1 - normalizedDistance);
    }

    #region IPICKUPABLE FUNCTIONS
    public void PickupLocked(bool flag)
    {
        _isOnPedestal = flag;
    }

    public void BeDropped(Transform newParent)
    {
        SetParent(newParent);
        _player.DropItem();
        _player = null;
    }

    public void BePickedUp(PlayerBase player)
    {
        _player = player;
        SetParent(_player.PickupPoint);
        transform.localPosition = Vector3.zero;
        _player.PickupItem(gameObject);
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }
    #endregion

    #region IINTERACTABLE FUNCTIONS
    public void PlayerInRange(PlayerBase player)
    {
    }

    public void PlayerNotInRange(PlayerBase player)
    {
    }

    public void PlayerStartInteract(PlayerBase player)
    {
        // If a player is holding the mirror
        if (_player != null)
        {
            // If player is holding this item
            if (_player.CarriedPickupable == gameObject)
            {
                BeDropped(null);
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
