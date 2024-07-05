using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Level1Mirror : MonoBehaviour, IInteractable, IPickupable, ISoftPuzzleReward
{
    #region EXTERNAL DATA
    [SerializeField] private float _maxIntensity = 5f;
    [SerializeField] private float _maxDistance = 10f;
    public bool InteractLocked { get; set; } = false;

    #endregion

    #region INTERNAL DATA
    // Components
    private Light _light;

    // Player
    private PlayerBase _player;

    // Pedestal
    private bool _isOnPedestal = false;

    // Soft Puzzle
    private SoftPuzzle _softPuzzle = null;
    public bool HeldInSoftPuzzle { get; set; } = false;
    private Transform _softPuzzleRewardGrouper; // Makes sure that mirror stay in level scene

    // Item Floating
    private Transform _followTarget;
    private bool _isFollowing = false;
    private float _followSpeed = 5f;  // Adjust this value as needed
    [SerializeField] private ParticleSystem _itemPassivePS;
    private ParticleSystem.EmissionModule _emissionPS;

    private bool isIntensityChanging = false;

    // Item Bobbing
    private bool isBobbingAllowed = true;
    private Vector3 finalRestingPosition;
    private bool hasSettled = false;
    #endregion

    private void Awake()
    {
        // Get components
        _light = GetComponentInChildren<Light>();
        _light.intensity = 0;
        _emissionPS = _itemPassivePS.emission;

    }

    private void Start()
    {
        // If is not a part of soft puzzle, should be stored in LevelManager's reward grouper
        if (_softPuzzle == null)
        {
            _softPuzzleRewardGrouper = gameObject.transform.parent;
        }
    }


    void Update()
    {
        if (_isFollowing && _followTarget != null)
        {
            // Perform the interpolation in world space
            transform.position = Vector3.Lerp(transform.position, _followTarget.position, _followSpeed * Time.deltaTime);
        }

        if (isBobbingAllowed && _isOnPedestal == false)
        {
            BobbingEffect(finalRestingPosition);
        }
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
        Transform parent = newParent != null ? newParent : _softPuzzleRewardGrouper;
        SetParent(parent);

        _emissionPS.enabled = false;

        _player.DropItem();

        // Start coroutine to smoothly move the item to the ground
        if (parent == _softPuzzleRewardGrouper)
        {
            StartCoroutine(FloatItemToGround());
        }
        _player = null;
    }

    public void BePickedUp(PlayerBase player)
    {
        // If currently associated with a soft puzzle
        if (_softPuzzle)
        {
            HeldInSoftPuzzle = true;
            _softPuzzle.CheckAllRewardsHeld();
        }

        _player = player;
        _followTarget = _player.PickupPoint;   

        Collider[] childColliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in childColliders)
        {
            collider.enabled = false;
        }

        StartCoroutine(ItemFloatUp(_followTarget));
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

            // Moves item to the ground
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, dropSpeed * Time.deltaTime);
                yield return null; 
            }

            // Gets the latest position to be sent to bobbing function. This is because of how coroutines behave with the Update() function.
            finalRestingPosition = transform.position; 
            hasSettled = true; // Mark that the item has settled

            //Enables all the coliders attached to the object
            Collider[] childColliders = GetComponentsInChildren<Collider>();
            foreach (Collider collider in childColliders)
            {
                collider.enabled = true;
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
        _player.PickupItem(gameObject);
        SetParent(_player.transform);

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
        if (!isIntensityChanging)
        {
            isIntensityChanging = true;
            //Debug.Log("Starting to change light intensity towards maximum.");

            LeanTween.value(gameObject, _light.intensity, _maxIntensity, 1f).setOnUpdate((float val) => {
                _light.intensity = val;
                //Debug.Log("Current light intensity: " + val);
            }).setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() => {
                //Debug.Log("Light intensity change complete. Current intensity: " + _light.intensity);
                isIntensityChanging = false;
            });
        }
    }

    public void PlayerNotInRange(PlayerBase player)
    {
        if (isIntensityChanging)
        {

            isIntensityChanging = false;
            // Debug.Log("Starting to decrease light intensity towards minimal.");
            LeanTween.value(gameObject, _light.intensity, 0f, 1f).setOnUpdate((float val) => {
                _light.intensity = val;
                // Debug.Log("Current light intensity: " + val);
            }).setEase(LeanTweenType.easeOutSine)
            .setOnComplete(() => {
                // Debug.Log("Light intensity decrease complete. Current intensity: " + _light.intensity);
                isIntensityChanging = true;
            });
        }
    }

    public void PlayerStartInteract(PlayerBase player)
    {

        // Play the FMOD event
        EventManager.EventTrigger(EventType.ITEM_PICKUP, null);

        // If a player is holding the mirror
        if (_player != null)
        {
            // If player initiating the interaction is holding this item
            if (player.CarriedPickupable == gameObject)
            {
                BeDropped(null);
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
        throw new System.NotImplementedException();
    }

    public void PlayerReleaseHoldInteract(PlayerBase player)
    {
        throw new System.NotImplementedException();
        // Play the FMOD event
        EventManager.EventTrigger(EventType.ITEM_PICKUP, null);
    }
    #endregion

    #region ISOFTPUZZLEREWARD FUNCTIONS
    public void SetSoftPuzzle(SoftPuzzle softPuzzle)
    {
        _softPuzzle = softPuzzle;
    }

    public void SetRewardGrouper(Transform transform)
    {
        _softPuzzleRewardGrouper = transform;
    }
    #endregion
}
