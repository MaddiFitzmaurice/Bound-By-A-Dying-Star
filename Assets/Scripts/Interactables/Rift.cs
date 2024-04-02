using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rift : MonoBehaviour, IInteractable
{
    // Internal Data
    private PlayerBase _playerInTrigger;
    private Transform _target; // Link to other Rift if it exists

    public void UpdateTarget(Transform newTarget)
    {
        _target = newTarget;
    }

    public void PlayerAboutToInteract()
    {
        // Assign which player is in the trigger or null if neither
    }

    // If interact button is pressed near the Rift
    public void PlayerStartInteract()
    {   
        // If Rifts are linked and a player is in the trigger
        if (_target != null && _playerInTrigger != null)
        {
            GameObject itemToSend = _playerInTrigger.GetComponent<PlayerBase>().CarriedItem;

            // If the player is currently holding an item
            if (itemToSend != null)
            {
                Item item = itemToSend.GetComponent<Item>();
                Debug.Log("Item to teleport original position: " + itemToSend.transform.position);
                Debug.Log("Item to teleport new position: " + itemToSend.transform.position);
                item.SetTransform(_target);
                item.BeDropped();
                item.SetItemParent(null); 
                Debug.Log("Item teleported to target portal");
            }
        }
    }

    public void PlayerInRange(Material mat)
    {
        throw new System.NotImplementedException();
    }

    public void PlayerNotInRange()
    {
        throw new System.NotImplementedException();
    }
}
