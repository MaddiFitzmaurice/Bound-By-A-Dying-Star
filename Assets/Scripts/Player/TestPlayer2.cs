using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer2 : Player
{
    // On enabling of the attached GameObject
    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYER_2_MOVE_VECT2D, Player2Vect2DHandler);
    }

    // On disabling of the attached GameObject
    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYER_2_MOVE_VECT2D, Player2Vect2DHandler);
    }

    private void Player2Vect2DHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("Player2Vect2DHandler is null");
        }

        // Set move direction
        MoveDirection = (Vector2)data;
    }
}
