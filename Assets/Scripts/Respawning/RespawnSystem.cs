using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class RespawnSystem : MonoBehaviour
{
    #region External Data
    [Header("Forward Paths Respawn Ordered Lists")]
    [SerializeField] private List<Transform> _path1ForwardRespawns;
    [SerializeField] private List<Transform> _path2ForwardRespawns;
    [Header("Backward Paths Respawn Ordered Lists")]
    [SerializeField] private List<Transform> _path1BackRespawns;
    [SerializeField] private List<Transform> _path2BackRespawns;
    #endregion

    #region Internal Data
    // Current player queues of respawns
    private Queue<Transform> _player1CurrentQ;
    private Queue<Transform> _player2CurrentQ;

    // Future player queues of respawns
    private Queue<Transform> _player1FutureBackQ;
    private Queue<Transform> _player2FutureBackQ;

    // Queues for forward and backward respawns
    private Queue<Transform> _path1ForwardQ;
    private Queue<Transform> _path2ForwardQ;
    private Queue<Transform> _path1BackQ;
    private Queue<Transform> _path2BackQ;

    // List of UpdateRespawnPoints
    private List<UpdateRespawnPoint> _updaters;
    #endregion

    private void Awake()
    {
        // Init updaters
        _updaters = GetComponentsInChildren<UpdateRespawnPoint>().ToList<UpdateRespawnPoint>();

        if (_updaters.Count < 1)
        {
            Debug.LogError("UpdateRespawnPoint scripts have not been assigned within soft puzzle!");
        }

        // Create queues
        _path1ForwardQ = new Queue<Transform>();
        _path2ForwardQ = new Queue<Transform>();
        _path1BackQ = new Queue<Transform>();
        _path2BackQ = new Queue<Transform>();

        // Assign queues
        CreateRespawnQueues(_path1ForwardQ, _path1ForwardRespawns);
        CreateRespawnQueues(_path2ForwardQ, _path2ForwardRespawns);
        CreateRespawnQueues(_path1BackQ, _path1BackRespawns);
        CreateRespawnQueues(_path2BackQ, _path2BackRespawns);

        // Set current queue for players
        ChangeToForwardRespawn();
    }

    private void CreateRespawnQueues(Queue<Transform> queue, List<Transform> list)
    {
        foreach (Transform respawn in list)
        {
            queue.Enqueue(respawn);
        }
    }

    public void RespawnPlayer(PlayerBase player)
    {
        // Assign Player 1 to a spawn point grouping
        if (player is Player1)
        {
            player.Respawn(_player1CurrentQ.Peek());
        }
        // Assign Player 2 to a spawn point grouping
        else if (player is Player2)
        {
            player.Respawn(_player2CurrentQ.Peek());
        }
    }

    public void AssignPlayer1Path(RespawnPathNum pathNum)
    {
        // If no separate paths to take, assume player1 = path1
        if (pathNum == RespawnPathNum.NOPATH)
        {
            _player1CurrentQ = _path1BackQ;
        }
        else if (pathNum == RespawnPathNum.PATH1)
        {
            _player1FutureBackQ = _path1BackQ;
        }
        else
        {
            _player1FutureBackQ = _path2BackQ;
        }

        if (_player1FutureBackQ == _player2FutureBackQ)
        {
            Debug.LogError("You have assigned the RespawnPathNums incorrectly on the RespawnPathChooser!");
        }
    }

    public void AssignPlayer2Path(RespawnPathNum pathNum)
    {
        // If no separate paths to take, assume player2 = path2
        if (pathNum == RespawnPathNum.NOPATH)
        {
            _player2FutureBackQ = _path2BackQ;
        }
        else if (pathNum == RespawnPathNum.PATH1)
        {
            _player2FutureBackQ = _path1BackQ;
        }
        else
        {
            _player2FutureBackQ = _path2BackQ;
        }

        if (_player1FutureBackQ == _player2FutureBackQ)
        {
            Debug.LogError("You have assigned the RespawnPathNums incorrectly on the RespawnPathChooser!");
        }
    }

    // Update respawn point of a path
    public void UpdatePath1RespawnPoint(PlayerBase player)
    {
        if (player is Player1 && _player1CurrentQ == _path1BackQ)
        {
            _player1CurrentQ.Dequeue(); 
        }
        else if (player is Player2 && _player2CurrentQ == _path1BackQ)
        {
            _player2CurrentQ.Dequeue();
        }
    }

    public void UpdatePath2RespawnPoint(PlayerBase player)
    {
        if (player is Player1 && _player1CurrentQ == _path2BackQ)
        {
            _player1CurrentQ.Dequeue();
        }
        else if (player is Player2 && _player2CurrentQ == _path2BackQ)
        {
            _player2CurrentQ.Dequeue();
        }
    }

    // Change current queues to be forward respawn queues
    // Since there are no separate paths (due to design), can assume player1 = path1 etc.
    public void ChangeToForwardRespawn()
    {
        _player1CurrentQ = _path1ForwardQ;
        _player2CurrentQ = _path2ForwardQ;
    }

    public void ChangeToBackRespawn()
    {
        _player1CurrentQ = _player1FutureBackQ;
        _player2CurrentQ = _player2FutureBackQ;
    }
}
