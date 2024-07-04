using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;

public class LevelManager : MonoBehaviour
{
    #region EXTERNAL DATA 
    [Header("Spawn Data")]
    [SerializeField] private GameObject _startSpawnPoint; // Where players first spawn in
    [SerializeField] private GameObject _spawnPoint; // Where players get TP'd to after solving a soft puzzle or if they fall
    [Header("Soft Puzzle Data")]
    [SerializeField] private GameObject _rewardGrouper;
    [SerializeField] private List<GameObject> _softPuzzles;
    [SerializeField] private PlayableAsset _introCutscene;
    #endregion

    #region INTERNAL DATA
    private Queue<GameObject> _softPuzzlesQueue;
    #endregion

    private void Awake()
    {
        // Event Inits
        EventManager.EventInitialise(EventType.LEVEL_SPAWN);
        EventManager.EventInitialise(EventType.SOFTPUZZLE_PLAYER_TELEPORT);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.SOFTPUZZLE_COMPLETE, OnSoftPuzzleComplete);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.SOFTPUZZLE_COMPLETE, OnSoftPuzzleComplete);
    }

    private void Start()
    {
        // Spawn players in
        if (_startSpawnPoint != null)
        {
            EventManager.EventTrigger(EventType.LEVEL_SPAWN, _startSpawnPoint.transform);
        }
        else
        {
            Debug.LogError("Please use SpawnPoint object to assign players' initial spawn in level.");
        }
        
        // Play cutscene if assigned
        if (_introCutscene != null)
        {
            EventManager.EventTrigger(EventType.PLAY_CINEMATIC, _introCutscene);
        }

        EventManager.EventTrigger(EventType.MUSIC, "Calm"); // Ensure transition to Calm section

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
        //EventManager.EventTrigger(EventType.ADD_GAMEPLAY_CAM, softPuzzleToActivate.GetComponent<SoftPuzzle>().SendReceiveCams());
        softPuzzleToActivate.SetActive(true);
    }

    private void DeactivateSoftPuzzle(GameObject softPuzzleToDeactivate)
    {
        //EventManager.EventTrigger(EventType.DELETE_GAMEPLAY_CAM, softPuzzleToDeactivate.GetComponent<SoftPuzzle>().SendReceiveCams());
        softPuzzleToDeactivate.SetActive(false);
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
