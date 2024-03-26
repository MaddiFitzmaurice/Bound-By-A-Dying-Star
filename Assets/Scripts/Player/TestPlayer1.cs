using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer1 : Player
{
    // On enabling of the attached GameObject
    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYER_1_MOVE_VECT, Player1VectHandler);
    }

    // On disabling of the attached GameObject
    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYER_1_MOVE_VECT, Player1VectHandler);
    }

    private void Player1VectHandler(object data)
    {
        if (data is not Vector2)
        {
            Debug.LogError("Player1 has not received a Vector2!");
        }

        // Set move direction
        Vector2 input = (Vector2)data;
        MoveDirection.Set(input.x, 0, input.y);
    }
}
