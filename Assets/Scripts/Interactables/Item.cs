using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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

    #endregion

    private void Awake()
    {
        // Init Event
        EventManager.EventInitialise(EventType.GRAVITY_INVERT);

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
        if (parent.name.Contains("ItemGrouper"))
        {
            StartCoroutine(FloatItemToGround());
        }

        _playerHoldingItem = null;
    }

    public void BePickedUp(PlayerBase player)
    {
        if (_itemLocked == false)
        {
            _playerHoldingItem = player;
            _followTarget = _playerHoldingItem.PickupPoint;
            UnhighlightItem();

            if (isGravityFlipItem)
            {
                _isFollowing = true;
                _playerHoldingItem.PickupItem(gameObject);

                // Toggle the gravity state and apply the new state
                _currentGravityState = !_currentGravityState;
                // Flip gravity for all players
                FlipGravityForAllPlayers(_currentGravityState);
            }
            else
            {
                _emissionPS.enabled = true;
                StartCoroutine(ItemFloatUp());
            }
        }
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent); 
    }

    private IEnumerator FloatItemToGround()
    {
        // Speed at which the item will move to the ground
        float dropSpeed = 1.5f;
        // Maximum distance to check for the ground
        float maxDropHeight = 100.0f;
        // Get the layer mask for the ground, so the raycast doesn't hit the players
        int groundLayer = LayerMask.GetMask("Default");

        // Use the direction of gravity to determine the ray direction
        Vector3 gravityDirection = Physics.gravity.normalized;
        // Start the ray from the item's position adjusted by half its height in the direction of gravity
        Vector3 rayStart = transform.position + gravityDirection * (transform.localScale.y / 2);

        RaycastHit hit;
        if (Physics.Raycast(rayStart, gravityDirection, out hit, maxDropHeight, groundLayer))
        {
            // Calculate target position ensuring the item sits on the ground properly
            // Refactor adjustment based on the current direction of gravity
            Vector3 adjustment = -gravityDirection * (transform.localScale.y / 2);  
            Vector3 targetPosition = hit.point + adjustment;

            // Move the item to the target position
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, dropSpeed * Time.deltaTime);
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("Ground not found below item, not moving.");
        }
    }


    // Make the item float upwards, then set it to follow the player
    private IEnumerator ItemFloatUp()
    {
        // Calculate the world space position towards which to move
        // Should be the mirror's current position, but at the follow target's elevation
        Vector3 targetPosition = new Vector3(transform.position.x, _followTarget.position.y, transform.position.z);

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // Perform the interpolation in world space
            transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);
            yield return null;  // Wait for the next frame
        }

        _isFollowing = true;
        _playerHoldingItem.PickupItem(gameObject);
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

    private void FlipGravityForAllPlayers(bool isFlipped)
    {
        // Inverts the gravity
        Physics.gravity = new Vector3(0, isFlipped ? -flippedGravityScale : flippedGravityScale, 0);

        // Calls the invert gravity event for the players that rotates the game object
        EventManager.EventTrigger(EventType.GRAVITY_INVERT, null);
    }
    #endregion
}
