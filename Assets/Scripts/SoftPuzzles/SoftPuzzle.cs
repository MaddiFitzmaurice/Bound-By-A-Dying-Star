using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftPuzzle : MonoBehaviour
{
    #region External Data
    [SerializeField] private List<GameObject> _rewardObjs;
    [SerializeField] private GameObject _forwardPuzzle;
    [SerializeField] private GameObject _backwardPuzzle;
    [SerializeField] private Respawn _respawnScript;
    [SerializeField] private GameObject _puzzlePlatform;
    [SerializeField] private GameObject _puzzleMovePoint;
    [SerializeField] private GameObject _puzzleDoor;
    [SerializeField] private GameObject _softPuzzleCamParent;
    #endregion

    #region Internal Data
    private List<ISoftPuzzleReward> _rewards = new List<ISoftPuzzleReward>();
    private GameObject _player1;
    private bool _player1InPuzzle;
    private GameObject _player2;
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
        if (other.CompareTag("Player1"))
        {
            _player1 = other.gameObject;
            _player1InPuzzle = true;
        }
        else if (other.CompareTag("Player2"))
        {
            _player2 = other.gameObject;
            _player2InPuzzle = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player1"))
        {
            _player1InPuzzle = false;
        }
        else if (other.CompareTag("Player2"))
        {
            _player2InPuzzle = false;
        }

        if (!_player1InPuzzle && !_player2InPuzzle && _puzzleCompleted)
        {
            _puzzleDoor.SetActive(true);
        }
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
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

        //CompleteSoftPuzzle();
        StartCoroutine(SwapPuzzle());
    }

    private IEnumerator SwapPuzzle()
    {
        // Swap the puzzles
        _forwardPuzzle.SetActive(false);

        float duration = 2.0f; // Duration of the movement
        float elapsedTime = 0f;

        Vector3 initialPositionPlatform = _puzzlePlatform.transform.position;

        Vector3 targetPosition = _puzzleMovePoint.transform.position;

        while (elapsedTime < duration)
        {
            _puzzlePlatform.transform.position = Vector3.Lerp(initialPositionPlatform, targetPosition, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _backwardPuzzle.SetActive(true);
        _respawnScript.CurrentSpawnPoint = _respawnScript.BackwardPuzzleRespawnPoint;
        _puzzleCompleted = true;
    }

    // Remove all references to the soft puzzle so players don't get teleported every time they touch reward obj
    //private void CompleteSoftPuzzle()
    //{
    //    foreach (ISoftPuzzleReward reward in _rewards)
    //    {
    //        reward.HeldInSoftPuzzle = false;
    //        reward.SetSoftPuzzle(null);
    //    }

    //    EventManager.EventTrigger(EventType.SOFTPUZZLE_COMPLETE, null);
    //}

    public void SetRewardGrouper(Transform transform)
    {
        foreach (ISoftPuzzleReward reward in _rewards)
        {
            reward.SetRewardGrouper(transform);
        }
    }
}
