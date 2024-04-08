using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : PlayerBase
{
    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYER_2_MOVE_VECT, Player2VectHandler);
        EventManager.EventSubscribe(EventType.PLAYER_2_CREATEPORTAL, CreatePortalInFrontOfPlayer);
        EventManager.EventSubscribe(EventType.PLAYER_2_INTERACT, Interact);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYER_2_MOVE_VECT, Player2VectHandler);
        EventManager.EventUnsubscribe(EventType.PLAYER_2_CREATEPORTAL, CreatePortalInFrontOfPlayer);
        EventManager.EventUnsubscribe(EventType.PLAYER_2_INTERACT, Interact);
    }

    private void Player2VectHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("Player2VectHandler is null");
        }

        // Set move direction
        Vector2 input = (Vector2)data;
        MoveInput.Set(input.x, 0, input.y);
    }

    private void CreatePortalInFrontOfPlayer(object data)
    {
        RiftData.Position = transform.position + transform.forward * DistanceInFront;
        RiftData.Rotation = Quaternion.LookRotation(transform.forward);
        
        EventManager.EventTrigger(EventType.PORTALMANAGER_CREATEPORTAL, RiftData);
    }
}
