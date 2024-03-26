using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer2 : Player
{
    // On enabling of the attached GameObject
    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYER_2_MOVE_VECT, Player2VectHandler);
    }

    // On disabling of the attached GameObject
    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYER_2_MOVE_VECT, Player2VectHandler);
    }

    private void Player2VectHandler(object data)
    {
        if (data is not Vector2)
        {
            Debug.LogError("Player2 has not received a Vector2!");
        }

        // Set move direction
        Vector2 input = (Vector2)data;
        MoveDirection.Set(input.x, 0, input.y);
    }
}
