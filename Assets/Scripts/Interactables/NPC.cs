using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPC : MonoBehaviour, Interactable
{
    [SerializeField] TextAsset _dialogue;

    // Internal Data
    private bool _aboutToInteract = false; // Whether a player is within the interacting trigger
    private bool _currentlyInteracting = false; // Whether a player is currently interacting with NPC

    private void Awake()
    {
        EventManager.EventInitialise(EventType.NPC_SEND_DIALOGUE);
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    public void PlayerAboutToInteract(Collider player, bool aboutToInteract)
    {
        _aboutToInteract = aboutToInteract;

        Debug.Log($"NPC about to interact: {_aboutToInteract}");
    }
}
