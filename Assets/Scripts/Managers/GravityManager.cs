using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    private float flippedGravityScale = -9.81f; // The gravity scale when flipped
    private bool isFlipped = false;  // Default gravity state

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYER_1_INTERACT, data => FlipGravityUp());
        EventManager.EventSubscribe(EventType.PLAYER_2_INTERACT, data => FlipGravityDown());
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYER_1_INTERACT, data => FlipGravityUp());
        EventManager.EventUnsubscribe(EventType.PLAYER_2_INTERACT, data => FlipGravityDown());
    }

    private void FlipGravityUp()
    {
        if (!isFlipped)
        {
            // Set gravity to an upward force
            //Physics.gravity = new Vector3(0, Mathf.Abs(Physics.gravity.y), 0);
            Physics.gravity = new Vector3(0, Mathf.Abs(flippedGravityScale), 0);
            isFlipped = true;

            // Optionally trigger an event if needed
            EventManager.EventTrigger(EventType.GRAVITY_INVERT, isFlipped);
        }
    }

    private void FlipGravityDown()
    {
        if (isFlipped)
        {
            // Set gravity to a downward force
            //Physics.gravity = new Vector3(0, -Mathf.Abs(Physics.gravity.y), 0);
            Physics.gravity = new Vector3(0, -Mathf.Abs(flippedGravityScale), 0);
            isFlipped = false;

            // Optionally trigger an event if needed
            EventManager.EventTrigger(EventType.GRAVITY_INVERT, isFlipped);
        }
    }


    //private void FlipGravityForAllPlayers(bool isFlipped)
    //{
    //    // Inverts the gravity
    //    Physics.gravity = new Vector3(0, isFlipped ? -flippedGravityScale : flippedGravityScale, 0);

    //    // Calls the invert gravity event for the players that rotates the game object
    //    EventManager.EventTrigger(EventType.GRAVITY_INVERT, null);
    //}

}
