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

    // Item Floating
    private Transform _followTarget;
    private bool _isFollowing = false;
    private float _followSpeed = 5f;  // Adjust this value as needed
    #endregion

    private void Awake()
    {
        // Get components
        _light = GetComponentInChildren<Light>();
        _mirrorGrouper = transform.parent;

        _light.intensity = 0;
    }

    void Update()
    {
        if (_isFollowing && _followTarget != null)
        {
            // Calculate the world space position towards which to move
            Vector3 targetPosition = _followTarget.position;

            // Perform the interpolation in world space
            transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);
        }
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
        _isFollowing = false;

        // Removes the parent-child relationship, making the object independent in the scene
        // If an incoming parent is specified, use that. Else, use the default parent assigned in the scene
        Transform parent = newParent != null ? newParent : _mirrorGrouper;
        SetParent(parent);

        _player.DropItem();

        // Start coroutine to smoothly move the item to the ground
        if (parent.name.Contains("MirrorGrouper"))
        {
            StartCoroutine(FloatItemToGround(_player));
        }

        _player = null;
    }

    public void BePickedUp(PlayerBase player)
    {
        _player = player;
        _followTarget = _player.PickupPoint;
        _isFollowing = true;
        transform.localPosition = Vector3.zero;
        transform.position = _followTarget.position;
        _player.PickupItem(gameObject);
        
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    private IEnumerator FloatItemToGround(PlayerBase player)
    {
        _player = player;
        // Speed at which the item will move to the ground
        float dropSpeed = 1.5f;
        // Maximum distance to check for the ground
        float maxDropHeight = 100.0f;
        // Get the layer mask for the ground, so the raycast doesn't hit the players
        int groundLayer = LayerMask.GetMask("Default");

        // Calculate the offset position behind the player by using the player's forward direction
        Vector3 offsetPosition = transform.position - _player.transform.forward * 2; // Offset by 2 units behind the player, for bigger game objects

        // Start the raycast from above the offset position to avoid collisions with the player
        Vector3 rayStart = offsetPosition + Vector3.up * 5;  // Start 5 units above the offset position
        RaycastHit hit;
        if (Physics.Raycast(rayStart, Vector3.down, out hit, maxDropHeight + 5, groundLayer))  // Increase ray distance to account for starting offset
        {
            // Adjust position, so the game object sits on the ground
            Vector3 targetPosition = hit.point + Vector3.up * (transform.localScale.y / 2);

            // Move item directly to offset position to avoid hitting player
            transform.position = offsetPosition;

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, dropSpeed * Time.deltaTime);
                yield return null;  // Wait for the next frame
            }
        }
        else
        {
            Debug.LogWarning("Ground not found below item, not moving.");
        }

    }
    #endregion

    #region IINTERACTABLE FUNCTIONS
    public void PlayerInRange(PlayerBase player)
    {
        // set _maxIntensity when the player is close to the mirror
        LeanTween.value(gameObject, _light.intensity, _maxIntensity, 1f).setOnUpdate((float val) => {
            _light.intensity = val;
        }).setEase(LeanTweenType.easeInOutSine);
    }

    public void PlayerNotInRange(PlayerBase player)
    {
        // Fade the light to minimal intensity when the player moves out of range
        LeanTween.value(gameObject, _light.intensity, 0f, 1f).setOnUpdate((float val) => {
            _light.intensity = val;
        }).setEase(LeanTweenType.easeInOutSine);
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

    public void PlayerHoldInteract(PlayerBase player)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
