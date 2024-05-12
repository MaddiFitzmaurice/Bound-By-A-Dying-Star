using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region EXTERNAL DATA
    [SerializeField] private GameObject _levelCams;
    [SerializeField] private GameObject _rewardGrouper;
    [SerializeField] private GameObject _spawnPoint; // Where players initially start, and where they get TP'd to after solving a soft puzzle
    [SerializeField] private List<GameObject> _softPuzzles;
    #endregion

    #region INTERNAL DATA
    private CinemachineClearShot _vCamClearShot;
    private Queue<GameObject> _softPuzzlesQueue;
    #endregion

    private void Awake()
    {
        // Get Components
        _vCamClearShot = _levelCams.GetComponent<CinemachineClearShot>();

        // Event Inits
        EventManager.EventInitialise(EventType.LEVEL_CAMS_REQUEST_FOLLOWGROUP);
        EventManager.EventInitialise(EventType.LEVEL_SPAWN);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LEVEL_CAMS_SEND_FOLLOWGROUP, ReceiveFollowGroup);
        EventManager.EventSubscribe(EventType.SOFTPUZZLE_COMPLETE, OnSoftPuzzleComplete);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LEVEL_CAMS_SEND_FOLLOWGROUP, ReceiveFollowGroup);
        EventManager.EventUnsubscribe(EventType.SOFTPUZZLE_COMPLETE, OnSoftPuzzleComplete);
    }

    private void Start()
    {
        if (_levelCams != null)
        {
            EventManager.EventTrigger(EventType.LEVEL_CAMS_REQUEST_FOLLOWGROUP, null);
        }
        else
        {
            Debug.LogError("No Level Cameras have been set up!");
        }

        if (_spawnPoint != null)
        {
            Spawn();
        }
        else
        {
            Debug.LogError("Please use SpawnPoint object to assign players' initial spawn in level.");
        }

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

        // Convert soft puzzle list to a queue and reenable first soft puzzle
        _softPuzzlesQueue = new Queue<GameObject>(_softPuzzles);
        _softPuzzlesQueue.Peek().SetActive(true);
    }

    private void Spawn()
    {
        EventManager.EventTrigger(EventType.LEVEL_SPAWN, _spawnPoint.transform);
    }

    // Coruoutine function to delay the teleporting of players to make space 
    private IEnumerator PuzzleTransition()
    {
        // Insert teleport effect trigger here
        yield return new WaitForSeconds(1);
        Spawn();
        _softPuzzlesQueue.Peek().SetActive(false);
        _softPuzzlesQueue.Dequeue();
        _softPuzzlesQueue.Peek().SetActive(true);
        // Insert teleport effect trigger here
        yield return new WaitForSeconds(1);
        // Debug.Log("Teleport Done");
    }

    #region EVENT HANDLERS
    // Teleport player and load in next Soft Puzzle
    public void OnSoftPuzzleComplete(object data)
    {
        StopAllCoroutines();
        StartCoroutine(PuzzleTransition());
    }

    public void ReceiveFollowGroup(object data)
    {
        if (data is not CinemachineTargetGroup)
        {
            Debug.LogError("LevelManager has not received a CinemachineTargetGroup!");
        }

        CinemachineTargetGroup target = (CinemachineTargetGroup)data;
        _vCamClearShot.LookAt = target.transform;
    }
    #endregion
}
