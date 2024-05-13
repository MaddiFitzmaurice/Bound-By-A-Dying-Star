using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    #region EXTERNAL DATA
    public bool InteractLocked { get; set; } = false;
    [SerializeField] TextAsset _dialogue;
    [SerializeField] Material _highlightMat;
    #endregion

    #region INTERNAL DATA
    #endregion
    // Components
    private Renderer _renderer;

    // Internal Data
    private Material _defaultMat;

    private bool _player1Nearby = false; // Whether Player 1 is within the interacting trigger
    private bool _player2Nearby = false; // Whether Player 2 is within the interacting trigger
    private bool _canInteract = false;
    private bool _currentlyInteracting = false; // Whether interaction has started or not

    private void Awake()
    {
        // Get components
        _renderer = GetComponentInChildren<Renderer>();
        _defaultMat = _renderer.material;
    }

    public void PlayerInRange(PlayerBase player)
    {
        if (!_currentlyInteracting && !_canInteract)
        {
            // Check if players are nearby or not
            if (player is Player1)
            {
                _player1Nearby = true;
            }
            else if (player is Player2)
            {
                _player2Nearby = true;
            }

            // If both players are nearby
            if (_player1Nearby && _player2Nearby)
            {
                _canInteract = true;
                Debug.Log($"NPC can now be interacted with");
            }

            HighlightItem(player.HighlightMat);
        }
    }

    public void PlayerNotInRange(PlayerBase player)
    {
        // Check if players are nearby or not
        if (player is Player1)
        {
            _player1Nearby = false;
        }
        else if (player is Player2)
        {
            _player2Nearby = false;
        }

        _canInteract = false;
        UnhighlightItem();
    }

    // If one player has started an interaction
    public void PlayerStartInteract(PlayerBase player)
    {
        // If both players are nearby, the NPC is not currently in an interaction,
        // and the player who initiated the interaction is not carrying anything
        if (_canInteract && !_currentlyInteracting && player.CarriedPickupable == null)
        {
            _currentlyInteracting = true;
            UnhighlightItem();
            EventManager.EventTrigger(EventType.NPC_SEND_DIALOGUE, _dialogue);
        }
    }

    public void UnhighlightItem()
    {
        _renderer.material = _defaultMat;
    }

    public void HighlightItem(Material mat)
    {
        _renderer.material = mat;
    }

    public void PlayerHoldInteract(PlayerBase player)
    {
    }

    public void PlayerReleaseHoldInteract(PlayerBase player)
    {
    }
}
