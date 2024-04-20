using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneSlot : MonoBehaviour
{
    #region INTERNAL DATA
    private GameObject _validRune;
    public bool isOccupied;
    [SerializeField] private int _scaleSizeFactor;
    private RuneDoor _runeDoor;
    #endregion

    void Start()
    {
        // Attempt to get the RuneDoor Script from the parent GameObject
        _runeDoor = GetComponentInParent<RuneDoor>();
        if (_runeDoor == null)
        {
            Debug.LogError("Rune door script not found on the parent object!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player and they are carrying the correct object
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            // Check to see if the player is carrying an object
            PlayerBase player = other.GetComponent<PlayerBase>();
            if (player != null && player.CarriedPickupable != null)
            {
                GameObject carriedObject = player.CarriedPickupable;
                IPickupable pickupable = carriedObject.GetComponent<IPickupable>();

                // Check to see if the picked up item is the valid rune for the door
                if (pickupable != null && carriedObject == _validRune && !isOccupied)
                {
                    // Lock the object and drop it into the slot
                    pickupable.PickupLocked(true);
                    pickupable.BeDropped(transform);

                    // Adjust the position of the rune to match the slot
                    carriedObject.transform.position = transform.position;
                    carriedObject.transform.rotation = transform.rotation;
                    carriedObject.transform.localScale = Vector3.one * _scaleSizeFactor;
                    isOccupied = true;

                    Debug.Log($"Object {carriedObject.name} placed in slot successfully.");

                    // Disable the Mesh Renderer to make the slot invisible
                    MeshRenderer renderer = GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        renderer.enabled = false;
                    }

                    // Check to see if all runes has been added to the door
                    if (_runeDoor != null)
                    {
                        _runeDoor.CheckSlots(); // Check if all slots are now occupied
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
  
    }
}
