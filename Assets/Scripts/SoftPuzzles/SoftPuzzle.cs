using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftPuzzle : MonoBehaviour
{
    #region External Data
    [SerializeField, Header("Puzzle Groups")]
    private GameObject _forwardPuzzle;
    [SerializeField] private GameObject _backwardPuzzle;

    [SerializeField, Space(10), Header("Moving Platform")]
    private List<GameObject> _puzzlePlatforms;
    [SerializeField] private List<GameObject> _puzzleMovePoints;
    [SerializeField] private float _platformMoveSpeed = 2.0f;

    [SerializeField, Space(10)] private List<GameObject> _rewardObjs;
    [SerializeField] private Respawn _respawnScript;
    [SerializeField] private GameObject _puzzleDoor;
    [SerializeField] private GameObject _softPuzzleCamParent;
    #endregion

    #region Internal Data
    private List<ISoftPuzzleReward> _rewards = new List<ISoftPuzzleReward>();
    private bool _player1InPuzzle;
    private bool _player2InPuzzle;
    private bool _puzzleCompleted;
    #endregion

    private void Start()
    {
    }

    private void Awake()
    {
        // Make sure only 1 or 2 reward objects are assigned
        if (_rewardObjs.Count < 0 || _rewardObjs.Count > 2)
        {
            Debug.LogError("Please assign either 1 or 2 reward objects to SoftPuzzle!");
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

        // Enables flag to check if players are outside the puzzle
        if (!_player1InPuzzle && !_player2InPuzzle && _puzzleCompleted)
        {
            _puzzleDoor.SetActive(true);
        }
    }

    public GameObject SendReceiveCams()
    {
        return _softPuzzleCamParent;
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
        int arrayIndex = 0;
        List<Coroutine> coroutines = new List<Coroutine>();

        // Loops through each platform and starts to move it to the required position
        foreach (GameObject platform in _puzzlePlatforms)
        {
            coroutines.Add(StartCoroutine(MovePlatforms(platform, arrayIndex)));
            arrayIndex++;
        }

        // Waits till all platforms have finished moving
        foreach (Coroutine coroutine in coroutines)
        {
            yield return coroutine;
        }

        // Activates the backward puzzle and resets the spawn point
        _backwardPuzzle.SetActive(true);
        _respawnScript.CurrentSpawnPoint = _respawnScript.BackwardPuzzleRespawnPoint;
        _puzzleCompleted = true;

    }

    private IEnumerator MovePlatforms(GameObject platform, int index)
    {
        float elapsedTime = 0f;

        Vector3 initialPositionPlatform = platform.transform.position;
        Vector3 targetPosition = _puzzleMovePoints[index].transform.position;

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
