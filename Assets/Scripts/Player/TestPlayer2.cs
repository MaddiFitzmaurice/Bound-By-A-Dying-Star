using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer2 : Player
{
    
    

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYER_2_MOVE_VECT, Player2VectHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYER_2_MOVE_VECT, Player2VectHandler);
    }

    private void Player2VectHandler(object data)
    {
        if (data is Vector2 input)
        {
            MoveDirection = input; 
        }
        else
        {
            Debug.LogError("Player2 has not received a Vector2!");
        }
    }
}

