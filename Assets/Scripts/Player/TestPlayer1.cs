using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer1 : Player
{
    
    

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYER_1_MOVE_VECT, Player1VectHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYER_1_MOVE_VECT, Player1VectHandler);
    }

    private void Player1VectHandler(object data)
    {
        if (data is Vector2 input)
        {
            MoveDirection = input; 
        }
        else
        {
            Debug.LogError("Player1 has not received a Vector2!");
        }
    }
}


