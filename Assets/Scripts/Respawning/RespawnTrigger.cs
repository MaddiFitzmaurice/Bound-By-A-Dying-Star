using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    #region INTERNAL DATA
    private RespawnSystem _respawnSystem;
    #endregion

    private void Awake()
    {
        // Get components
        _respawnSystem = GetComponentInParent<RespawnSystem>();
        
        if (_respawnSystem == null)
        {
            Debug.LogError("RespawnSystem script not attached to Soft Puzzle Parent Grouper!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerBase player = other.GetComponent<PlayerBase>();

        // Assign Player 1 to a spawn point grouping
        if (player is Player1 || player is Player2)
        {
            _respawnSystem.RespawnPlayer(player);
        }
    }
}
