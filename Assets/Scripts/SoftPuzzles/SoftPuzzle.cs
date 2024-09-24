using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoftPuzzle : MonoBehaviour
{
    #region External Data
    [SerializeField, Header("Puzzle Groups")]
    private GameObject _forwardPuzzle;
    [SerializeField] private GameObject _backwardPuzzle;

    [Header("Moving Platform Data")]
    [SerializeField] private List<SoftPuzzleFixedPlatform> _fixedPlatforms;
    [SerializeField] private float _platformMoveSpeed = 2.0f;

    [Header("Cameras")]
    [SerializeField] private GameObject _forwardCams;
    [SerializeField] private GameObject _backwardCams;

    [Header("Puzzle Components")]
    [SerializeField, Space(10)] private List<GameObject> _rewardObjs;
    [SerializeField] private GameObject _puzzleDoor;
    #endregion

    #region Internal Data
    private List<ISoftPuzzleReward> _rewards = new List<ISoftPuzzleReward>();
    private RespawnSystem _respawnSystem;
    private bool _player1InPuzzle;
    private bool _player2InPuzzle;
    private bool _puzzleCompleted;
    #endregion

    private void Awake()
    {
        // Component Init
        _respawnSystem = GetComponentInChildren<RespawnSystem>();

        // Make sure only 1 or 2 reward objects are assigned
        if (_rewardObjs.Count < 0 || _rewardObjs.Count > 2)
        {
            Debug.LogError("Please assign either 1 or 2 reward objects to SoftPuzzle!");
        }

        if (_forwardCams == null || _backwardCams == null)
        {
            Debug.LogError("Please assign cameras!");
        }

        // Check if objects added subscribe to IReward
        foreach (GameObject obj in _rewardObjs)
        {
            var pickupObj = obj.GetComponent<ISoftPuzzleReward>();

            if (pickupObj == null)
            {
                Debug.LogError($"{obj} cannot be a reward in the SoftPuzzle!");
            }

            pickupObj.SetSoftPuzzle(this);
            _rewards.Add(pickupObj);
        }

        // Set Forward respawn
        _respawnSystem.ChangeToForwardRespawn();
        //_forwardCams.SetActive(true);
        //_backwardCams.SetActive(false);
    }

    private void CheckMusicTransition()
    {
        if (_player1InPuzzle && _player2InPuzzle)
        {
            EventManager.EventTrigger(EventType.MUSIC, "SoftPuzzle");
        }
        else if (!_player1InPuzzle && !_player2InPuzzle)
        {
            EventManager.EventTrigger(EventType.MUSIC, "MainArea");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Enables flag to check if players are within the puzzle
        if (other.CompareTag("Player1"))
        {
            _player1InPuzzle = true;
        }
        else if (other.CompareTag("Player2"))
        {
            _player2InPuzzle = true;
        }
        CheckMusicTransition();
    }

    private void OnTriggerExit(Collider other)
    {
        // Enables flag to check if players are outside the puzzle
        if (other.CompareTag("Player1"))
        {
            _player1InPuzzle = false;
        }
        else if (other.CompareTag("Player2"))
        {
            _player2InPuzzle = false;
        }
        CheckMusicTransition();

        // Enables flag to check if players are outside the puzzle
        if (!_player1InPuzzle && !_player2InPuzzle && _puzzleCompleted)
        {
            _puzzleDoor.SetActive(true);
            // Let level manager know this puzzle is complete
            EventManager.EventTrigger(EventType.SOFTPUZZLE_COMPLETE, this.gameObject);
        }
    }

    public void CheckAllRewardsHeld()
    {
        foreach (ISoftPuzzleReward reward in _rewards)
        {
            // If all rewards are not held, exit out of function
            if (!reward.HeldInSoftPuzzle)
            {
                return;
            }
        }

        // Swap the puzzles
        _forwardPuzzle.SetActive(false);

        // INSERT HERE FOR CUTSCENE CAMERA 

        // Moves platforms and swaps puzzle
        StartCoroutine(MovePlatformsAndActivateBackwardPuzzle());
    }

    private IEnumerator MovePlatformsAndActivateBackwardPuzzle()
    {
        EventManager.EventTrigger(EventType.DISABLE_GAMEPLAY_INPUTS, null);

        List<Coroutine> coroutines = new List<Coroutine>();

        // Loops through each platform and starts to move it to the required position
        foreach (SoftPuzzleFixedPlatform platform in _fixedPlatforms)
        {
            coroutines.Add(StartCoroutine(MovePlatforms(platform)));
        }

        // Waits till all platforms have finished moving
        foreach (Coroutine coroutine in coroutines)
        {
            yield return coroutine;
        }

        // Activates the backward puzzle + cams and resets the spawn point
        _backwardPuzzle.SetActive(true);
        _forwardCams.SetActive(false);
        _backwardCams.SetActive(true);
        _respawnSystem.ChangeToBackRespawn();
        _puzzleCompleted = true;

        EventManager.EventTrigger(EventType.ENABLE_GAMEPLAY_INPUTS, null);
    }

    private IEnumerator MovePlatforms(SoftPuzzleFixedPlatform platform)
    {
        float elapsedTime = 0f;

        Vector3 initialPositionPlatform = platform.transform.position;
        Vector3 targetPosition = platform.MoveToPos.position;

        // Moves platform to the target position, players are automatically moved with the platform containing the "Moving Platform" script
        while (elapsedTime < _platformMoveSpeed)
        {
            platform.transform.position = Vector3.Lerp(initialPositionPlatform, targetPosition, elapsedTime / _platformMoveSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the platform reaches the target position after the loop
        platform.transform.position = targetPosition;
    }

    public void SetRewardGrouper(Transform transform)
    {
        foreach (ISoftPuzzleReward reward in _rewards)
        {
            reward.SetRewardGrouper(transform);
        }
    }
}