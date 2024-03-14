using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer1 : Player
{
    // On enabling of the attached GameObject
    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYER_1_MOVE_VECT2D, Player1Vect2DHandler);
    }

    // On disabling of the attached GameObject
    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYER_1_MOVE_VECT2D, Player1Vect2DHandler);
    }

    private void Player1Vect2DHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("Player1Vect2DHandler is null");
        }

        // Set move direction
        MoveDirection = (Vector2)data;
    }
}
