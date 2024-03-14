using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
    // Internal Data
    private Interactable _interactable; // Set up object to be interactable by player

    void Awake()
    {
        _interactable = GetComponentInParent<Interactable>();
    }

    // When player starts an interaction
    void OnTriggerEnter(Collider other)
    {
        if (CheckIfPlayer(other))
        {
            _interactable.PlayerAboutToInteract(other, true);
        } 
    }

    // When player finishes an interaction
    void OnTriggerExit(Collider other)
    {
        if (CheckIfPlayer(other))
        {
            _interactable.PlayerAboutToInteract(other, false);
        } 
    }

    bool CheckIfPlayer(Collider other)
    {
        var colliderType = other.GetComponent<Player>();

        return colliderType is Player ? true : false;
    }
}
