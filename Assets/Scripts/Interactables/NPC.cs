using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] TextAsset _dialogue;

    [SerializeField] Material _highlightMat;

    // Components
    private Renderer _renderer;

    // Internal Data
    private Material _defaultMat;

    private bool _player1InTrigger = false; // Whether Player 1 is within the interacting trigger
    private bool _player2InTrigger = false; // Whether Player 2 is within the interacting trigger
    private bool _canInteract = false;
    private bool _currentlyInteracting = false; // Whether interaction has started or not

    private void Awake()
    {
        // Get components
        _renderer = GetComponentInChildren<Renderer>();
        _defaultMat = _renderer.material;
    }

    // private void OnDisable()
    // {
    //     EventManager.EventUnsubscribe(EventType.PLAYER_1_NPC, PlayerStartInteract);
    //     EventManager.EventUnsubscribe(EventType.PLAYER_2_NPC, PlayerStartInteract);
    // }

    public void PlayerInRange(Material mat)
    {
        HighlightItem(mat);
    }

    public void PlayerNotInRange()
    {
        UnhighlightItem();
    }

    public void PlayerStartInteract(PlayerBase player)
    {
        throw new System.NotImplementedException();
    }

    public void UnhighlightItem()
    {
        _renderer.material = _defaultMat;
    }

    public void HighlightItem(Material mat)
    {
        _renderer.material = mat;
    }

    // public void PlayerAboutToInteract(Collider player, bool isInTrigger)
    // {
    //     // Check if players are in trigger or not
    //     if (player.CompareTag("Player1"))
    //     {
    //         _player1InTrigger = isInTrigger;
    //     }
    //     else if (player.CompareTag("Player2"))
    //     {
    //         _player2InTrigger = isInTrigger;
    //     }

    //     // If both in trigger, NPC can be interacted with
    //     if (_player1InTrigger && _player2InTrigger)
    //     {
    //         // Start listening for input event
    //         EventManager.EventSubscribe(EventType.PLAYER_1_NPC, PlayerStartInteract);
    //         EventManager.EventSubscribe(EventType.PLAYER_2_NPC, PlayerStartInteract);
    //         _canInteract = true;
    //         Debug.Log($"NPC can now be interacted with");
    //     }
    //     else
    //     {
    //         // Stop listening for input event
    //         EventManager.EventUnsubscribe(EventType.PLAYER_1_NPC, PlayerStartInteract);
    //         EventManager.EventUnsubscribe(EventType.PLAYER_2_NPC, PlayerStartInteract);
    //         _canInteract = false;
    //         _currentlyInteracting = false;
    //         Debug.Log($"NPC cannot be interacted with");
    //     }
    // }

    // #region EVENT HANDLERS
    // // If one player has started an interaction
    // public void PlayerStartInteract(object data)
    // {
    //     if (_canInteract && !_currentlyInteracting)
    //     {
    //         _currentlyInteracting = true;
    //         EventManager.EventTrigger(EventType.NPC_SEND_DIALOGUE, _dialogue);
    //     }
    // }
    // #endregion
}
