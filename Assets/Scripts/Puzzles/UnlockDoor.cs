using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoor : MonoBehaviour, IInteractable
{
    //Key object to open locked door
    [SerializeField] private GameObject _KeyObject;

    public bool InteractLocked { get; set; } = false;

    private void OnTriggerEnter(Collider other)
    {
        // If player has entered the trigger
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            CheckForKey(other);
        }
    }

    private void CheckForKey(Collider playerCollider)
    {
        PlayerBase player = playerCollider.GetComponent<PlayerBase>();

        // Ensure player has entered the trigger and is holding something
        if (player != null && player.CarriedPickupable != null)
        {
            // IPickupable and IInteractable manipulation
            GameObject carriedPickupable = player.CarriedPickupable;
            IPickupable pickupableType = carriedPickupable.GetComponent<IPickupable>();

            // Check to see if picked up item contains the key object as an active child
            Transform keyObjectChild = carriedPickupable.transform.Find(_KeyObject.name);

            // If picked up item contains key object and it's active
            if (keyObjectChild != null && keyObjectChild.gameObject == _KeyObject && keyObjectChild.gameObject.activeSelf)
            {
                OpenDoor();
               pickupableType.BeDropped(transform);  
            }
        }
    }

    private void OpenDoor()
    {
        // Deactivates the door and the key
        this.gameObject.SetActive(false);
        _KeyObject.SetActive(false);
    }

    #region IINTERACTABLE FUNCTIONS
    public void PlayerInRange(PlayerBase player)
    {
    }

    public void PlayerNotInRange(PlayerBase player)
    {
    }

    public void PlayerStartInteract(PlayerBase player)
    {

    }

    public void PlayerHoldInteract(PlayerBase player)
    {

    }

    public void PlayerReleaseHoldInteract(PlayerBase player)
    {

    }
    #endregion
}
