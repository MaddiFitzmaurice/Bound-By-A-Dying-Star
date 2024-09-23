using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneDoor : MonoBehaviour
{
    // Lists of slots in the door
    [SerializeField] private List<RuneSlot> _slots; 

    // Method to be called by slots to check if all are occupied
    public void CheckSlots()
    {
        foreach (RuneSlot slot in _slots)
        {
            // If any slot is not occupied
            if (!slot.isOccupied) 
            {
                // Exit the method early
                return; 
            }
        }

        // If all slots are occupied
        Debug.Log("All slots are occupied. Door will disappear.");

        // Disable door 
        gameObject.SetActive(false);
    }
}
