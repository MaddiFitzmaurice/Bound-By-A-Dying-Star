using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RespawnPathNum { PATH1, PATH2, NOPATH };

public class RespawnPathChooser : MonoBehaviour
{
    #region EXTERNAL DATA
    [SerializeField] private RespawnPathNum _pathNum;
    #endregion
    #region INTERNAL DATA
    // Respawn system
    private RespawnSystem _respawnSystem;
    private bool _p1Assigned = false;
    private bool _p2Assigned = false;
    #endregion

    private void Awake()
    {
        // Init system
        _respawnSystem = GetComponentInParent<RespawnSystem>();

        if (_respawnSystem == null )
        {
            Debug.LogError("RespawnSystem script not attached to Soft Puzzle Parent Grouper!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerBase player = null;

        // Check if either player needs to have their respawn point updated
        if (!_p1Assigned || !_p2Assigned)
        {
            player = other.GetComponent<PlayerBase>();
        }
        else
        {
            return;
        }

        if (player != null)
        {
            if (!_p1Assigned)
            {
                // Assign Player 1 to a spawn point grouping
                if (player is Player1)
                {
                    _respawnSystem.AssignPlayer1Path(_pathNum);
                    _p1Assigned = true;
                }
            }
            
            if (!_p2Assigned)
            {
                // Assign Player 2 to a spawn point grouping
                if (player is Player2)
                {
                    _respawnSystem.AssignPlayer2Path(_pathNum);
                    _p2Assigned = true;
                }
            }
        }
    }
}
