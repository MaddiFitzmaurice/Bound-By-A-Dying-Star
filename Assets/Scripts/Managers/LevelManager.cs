using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelManager : MonoBehaviour
{
    #region EXTERNAL DATA 
    [Header("Level Cam Parent")]
    [SerializeField] private GameObject _levelCamParent;
    [Header("Spawn Data")]
    [SerializeField] private GameObject _spawnPoint; // Where players initially start, and where they get TP'd to after solving a soft puzzle
    [Header("Soft Puzzle Data")]
    [SerializeField] private GameObject _rewardGrouper;
    [SerializeField] private List<GameObject> _softPuzzles;
    #endregion

    #region INTERNAL DATA
    private Queue<GameObject> _softPuzzlesQueue;
    #endregion

    private void Awake()
    {
        // Event Inits
        EventManager.EventInitialise(EventType.LEVEL_SPAWN);
        EventManager.EventInitialise(EventType.SOFTPUZZLE_PLAYER_TELEPORT);

        // Data Checks
        if (_levelCamParent == null)
        {
            Debug.LogError("Please assign a level cam parent!");
        }
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.SOFTPUZZLE_COMPLETE, OnSoftPuzzleComplete);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.SOFTPUZZLE_COMPLETE, OnSoftPuzzleComplete);
        EventManager.EventTrigger(EventType.DELETE_GAMEPLAY_CAM, _levelCamParent); 
    }

    private void Start()
    {
        if (_spawnPoint != null)
        {
            Spawn();
        }
        else
        {
            Debug.LogError("Please use SpawnPoint object to assign players' initial spawn in level.");
        }

        // Send Level Cams
        EventManager.EventTrigger(EventType.ADD_GAMEPLAY_CAM, _levelCamParent);

        // Set reward grouper parent and disable all soft puzzles
        foreach (GameObject softPuzzle in _softPuzzles)
        {
            softPuzzle.SetActive(true);
            if (softPuzzle.GetComponent<SoftPuzzle>() != null)
            {
                softPuzzle.GetComponent<SoftPuzzle>().SetRewardGrouper(_rewardGrouper.transform);
            }
            softPuzzle.SetActive(false);
        }

        // Convert soft puzzle list to a queue
        _softPuzzlesQueue = new Queue<GameObject>(_softPuzzles);

        // Enable first soft puzzle and send its cams
        ActivateSoftPuzzle(_softPuzzlesQueue.Peek());
    }

    private void ActivateSoftPuzzle(GameObject softPuzzleToActivate)
    {
        EventManager.EventTrigger(EventType.ADD_GAMEPLAY_CAM, softPuzzleToActivate.GetComponent<SoftPuzzle>().SendReceiveCams());
        softPuzzleToActivate.SetActive(true);
    }

    private void DeactivateSoftPuzzle(GameObject softPuzzleToDeactivate)
    {
        EventManager.EventTrigger(EventType.DELETE_GAMEPLAY_CAM, softPuzzleToDeactivate.GetComponent<SoftPuzzle>().SendReceiveCams());
        softPuzzleToDeactivate.SetActive(false);
    }

    private void Spawn()
    {
        EventManager.EventTrigger(EventType.LEVEL_SPAWN, _spawnPoint.transform);
    }

    // Coruoutine function to delay the teleporting of players to make space 
    private IEnumerator PuzzleTransition()
    {
        EventManager.EventTrigger(EventType.SOFTPUZZLE_PLAYER_TELEPORT, _spawnPoint.transform);
        yield return new WaitForSeconds(3.5f);
        DeactivateSoftPuzzle(_softPuzzlesQueue.Peek());
        _softPuzzlesQueue.Dequeue();
        ActivateSoftPuzzle(_softPuzzlesQueue.Peek());
    }

    #region EVENT HANDLERS
    // Teleport player and load in next Soft Puzzle
    public void OnSoftPuzzleComplete(object data)
    {
        StopAllCoroutines();
        StartCoroutine(PuzzleTransition());
    }
    #endregion
}
