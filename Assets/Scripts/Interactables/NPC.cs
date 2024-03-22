using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPC : MonoBehaviour, Interactable
{
    [SerializeField] TextAsset _dialogue;

    // Internal Data
    private bool _player1InTrigger = false; // Whether Player 1 is within the interacting trigger
    private bool _player2InTrigger = false; // Whether Player 2 is within the interacting trigger
    private bool _canInteract = false;
    private bool _currentlyInteracting = false; // Whether interaction has started or not

    private void Awake()
    {
        EventManager.EventInitialise(EventType.NPC_SEND_DIALOGUE);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYER_1_NPC, PlayerStartInteract);
        EventManager.EventSubscribe(EventType.PLAYER_2_NPC, PlayerStartInteract);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYER_1_NPC, PlayerStartInteract);
        EventManager.EventUnsubscribe(EventType.PLAYER_2_NPC, PlayerStartInteract);
    }

    public void PlayerAboutToInteract(Collider player, bool isInTrigger)
    {
        // Check if players are in trigger or not
        if (player.CompareTag("Player1"))
        {
            _player1InTrigger = isInTrigger;
        }
        else if (player.CompareTag("Player2"))
        {
            _player2InTrigger = isInTrigger;
        }

        // If both in trigger, NPC can be interacted with
        if (_player1InTrigger && _player2InTrigger)
        {
            _canInteract = true;
            Debug.Log($"NPC can now be interacted with");
        }
        else
        {
            _canInteract = false;
            _currentlyInteracting = false;
            Debug.Log($"NPC cannot be interacted with");
        }
    }

    #region EVENT HANDLERS
    // If one player has started an interaction
    public void PlayerStartInteract(object data)
    {
        if (_canInteract && !_currentlyInteracting)
        {
            _currentlyInteracting = true;
            EventManager.EventTrigger(EventType.NPC_SEND_DIALOGUE, _dialogue);
        }
    }
    #endregion
}
