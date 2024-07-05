using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.Progress;

public class Item : MonoBehaviour, IInteractable, IPickupable
{
    #region EXTERNAL DATA
    public bool InteractLocked { get; set; } = false;
    #endregion
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

    // Gravity Flip Data
    [SerializeField] private bool isGravityFlipItem = false; // Flag to check if this item flips gravity
    private float flippedGravityScale = -9.81f; // The gravity scale when flipped
    private bool _currentGravityState = false;  // Default gravity state

    // Item Float
    private Transform _followTarget;
    private bool _isFollowing = false;
    private float _followSpeed = 5f; 
    [SerializeField] private ParticleSystem _itemPassivePS;
    private ParticleSystem.EmissionModule _emissionPS;

    // Item Bobbing
    private bool isBobbingAllowed = true;
    private Vector3 finalRestingPosition;
    private bool hasSettled = false;
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
        _emissionPS = _itemPassivePS.emission;
    }

    void Update()
    {
        if (_isFollowing && _followTarget != null)
        {
            // Perform the interpolation in world space
            transform.position = Vector3.Lerp(transform.position, _followTarget.position, _followSpeed * Time.deltaTime);
        }
        if (isBobbingAllowed)
        {
            BobbingEffect(finalRestingPosition);
        }
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
        _isFollowing = false;

        // Removes the parent-child relationship, making the object independent in the scene
        // If an incoming parent is specified, use that. Else, use the default parent assigned in the scene
        Transform parent = newParent != null ? newParent : _itemGrouper;
        SetParent(parent);

        _emissionPS.enabled = false;

        _playerHoldingItem.DropItem();

        // Start coroutine to smoothly move the item to the ground
        StartCoroutine(FloatItemToGround());

        _playerHoldingItem = null;
    }

    public void BePickedUp(PlayerBase player)
    {
        if (_itemLocked == false)
        {
            _playerHoldingItem = player;
            _followTarget = _playerHoldingItem.PickupPoint;
            UnhighlightItem();

            // Disable colliders for all item versions
            foreach (GameObject itemVersion in _itemVersions)
            {
                Collider collider = itemVersion.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = false;
                }
            }

            StartCoroutine(ItemFloatUp(_followTarget));
        }
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent); 
    }

    private IEnumerator FloatItemToGround()
    {
        // Disables bobbing function
        isBobbingAllowed = false;

        // Speed at which the item will move to the ground
        float dropSpeed = 1.5f;
        // Maximum distance to check for the ground
        float maxDropHeight = 100.0f;
        // Get the layer mask for the ground, so the raycast doesn't hit the players
        int groundLayer = LayerMask.GetMask("Default");

        // Calculate the offset position behind the player by using the player's forward direction
        Vector3 offsetPosition = transform.position - _playerHoldingItem.transform.forward * 2; // Offset by 2 units behind the player, for bigger game objects

        // Start the raycast from above the offset position to avoid collisions with the player
        Vector3 rayStart = offsetPosition + Vector3.up * 5;  // Start 5 units above the offset position
        RaycastHit hit;
        if (Physics.Raycast(rayStart, Vector3.down, out hit, maxDropHeight + 5, groundLayer))  // Increase ray distance to account for starting offset
        {
            // Adjust position, so the game object sits on the ground
            Vector3 targetPosition = hit.point + Vector3.up * (transform.localScale.y / 2);

            // Move item directly to offset position to avoid hitting player
            transform.position = offsetPosition;

            // Moves item to the ground
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, dropSpeed * Time.deltaTime);
                yield return null;
            }

            // Gets the latest position to be sent to bobbing function. This is because of how coroutines behave with the Update() function.
            finalRestingPosition = transform.position;
            hasSettled = true; // Mark that the item has settled

            // Disable colliders for all item versions
            foreach (GameObject itemVersion in _itemVersions)
            {
                Collider collider = itemVersion.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = true;
                }
            }
        }
        else
        {
            Debug.LogWarning("Ground not found below item, not moving.");
        }

        // Re-enable bobbing.
        isBobbingAllowed = true;
    }


    // Make the item float upwards, then set it to follow the player
    private IEnumerator ItemFloatUp(Transform pickUpPoint)
    {
        // Stop bobbing function
        isBobbingAllowed = false;

        // Variables to adjust how high and how often the item bounces
        float riseTime = 1.0f;
        float elapsedTime = 0;

        // Ensure startPosition is the current position at the very start of the coroutine
        Vector3 startPosition = transform.position;
        Vector3 verticalOffset = Vector3.up * (transform.localScale.y / 2);
        float anticipatoryOffset = 0.9f;

        // Moves the object downwards
        while (elapsedTime < riseTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / riseTime;
            Vector3 endPosition = pickUpPoint.position + verticalOffset - new Vector3(0, anticipatoryOffset, 0);

            Vector3 nextPosition = Vector3.Lerp(startPosition, endPosition, progress);
            transform.position = nextPosition;
            yield return null;
        }

        // Set item to follow player and adjust the parent
        _isFollowing = true;
        _playerHoldingItem.PickupItem(gameObject);
        SetParent(_playerHoldingItem.transform);

        // Gets the latest position to be sent to bobbing function. This is because of how coroutines behave with the Update() function.
        finalRestingPosition = transform.position;

        // Re-enable bobbing.
        isBobbingAllowed = true;
    }

    private void BobbingEffect(Vector3 finalRestingPosition)
    {
        // Determine the correct base height for bobbing
        float baseHeight;
        if (hasSettled)
        {
            baseHeight = finalRestingPosition.y;  // Use the final resting position if the item has settled
        }
        else if (_followTarget != null)
        {
            baseHeight = _followTarget.position.y; // Use the follow target's position if following
        }
        else
        {
            baseHeight = transform.position.y; // Default to current position if not settled or following
        }

        // Adjust the bobbing parameters based on whether there is a follow target
        float bobbingAmplitude = _followTarget != null ? 0.2f : 0.0050f;
        float bobbingFrequency = 2.0f;

        // Calculate the bobbing offset using a sine wave
        float bobbingOffset = Mathf.Sin(Time.time * bobbingFrequency) * bobbingAmplitude;

        // Apply the bobbing offset to the y-axis of the base position
        transform.position = new Vector3(transform.position.x, baseHeight + bobbingOffset, transform.position.z);
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
        // If a player is holding this item
        if (_playerHoldingItem != null)
        {
            // If the player doing the interacting is holding this item
            if (player.CarriedPickupable == gameObject)
            {
                BeDropped(_itemGrouper);
            }
        }
        // If a player is near the item
        else
        {
            // If that player is not carrying anything
            if (player.CarriedPickupable == null)
            {
                BePickedUp(player);
            }
        }
    }

    public void PlayerHoldInteract(PlayerBase player)
    {

    }

    public void PlayerReleaseHoldInteract(PlayerBase player)
    {

    }
    #endregion
}
