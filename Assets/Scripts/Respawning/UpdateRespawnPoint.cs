using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateRespawnPoint : MonoBehaviour
{
    #region EXTERNAL DATA
    [Header("Path Respawns To Update")]
    [SerializeField] private bool _forwardPath1ToUpdate;
    [SerializeField] private bool _forwardPath2ToUpdate;
    [SerializeField] private bool _backPath1ToUpdate;
    [SerializeField] private bool _backPath2ToUpdate;
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

        player = other.GetComponent<PlayerBase>();

        if (player != null)
        {
            if (_forwardPath1ToUpdate)
            {
                //_forwardPath1ToUpdate = !_respawnSystem.UpdateForwardPath1RespawnPoint(player);
                bool success = _respawnSystem.UpdateForwardPath1RespawnPoint(player);

                if (success)
                {
                    _forwardPath1ToUpdate = false;
                }
            }
            else if (_forwardPath2ToUpdate)
            {
                bool success = _respawnSystem.UpdateForwardPath2RespawnPoint(player);

                if (success)
                {
                    _forwardPath2ToUpdate = false;
                }
            }
            else if (_backPath1ToUpdate)
            {
                bool success = _respawnSystem.UpdateBackPath1RespawnPoint(player);

                if (success)
                {
                    _backPath1ToUpdate = false;
                }
            }
            else if (_backPath2ToUpdate)
            {
                bool success = _respawnSystem.UpdateBackPath2RespawnPoint(player);

                if (success)
                {
                    _backPath2ToUpdate = false;
                }
            }
        }        
    }
}
