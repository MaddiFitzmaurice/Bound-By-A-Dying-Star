using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateRespawnPoint : MonoBehaviour
{
    #region EXTERNAL DATA
    [Header("Path Respawns To Update")]
    [SerializeField] private bool _path1ToUpdate;
    [SerializeField] private bool _path2ToUpdate;
    #endregion

    #region INTERNAL DATA
    // Respawn system
    private RespawnSystem _respawnSystem;
    #endregion

    private void Awake()
    {
        // Component init
        _respawnSystem = GetComponentInParent<RespawnSystem>();

        if (_respawnSystem == null)
        {
            Debug.LogError("RespawnSystem script not attached to Soft Puzzle Parent Grouper!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerBase player = null;

        // If either path still needs to update
        if (_path1ToUpdate || _path2ToUpdate)
        {
            player = GetComponent<PlayerBase>();

            if (player == null)
            {
                if (_path1ToUpdate)
                {
                    _respawnSystem.UpdatePath1RespawnPoint(player);
                    _path1ToUpdate = false;
                }
                else if (_path2ToUpdate)
                {
                    _respawnSystem.UpdatePath2RespawnPoint(player);
                    _path2ToUpdate = false;
                }
            }
        }
        else
        {
            return;
        }
    }
}
