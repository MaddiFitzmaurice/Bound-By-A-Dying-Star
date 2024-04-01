using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1 : PlayerBase
{
    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYER_1_MOVE_VECT, Player1VectHandler);
        EventManager.EventSubscribe(EventType.PLAYER_1_CREATEPORTAL, CreatePortalInFrontOfPlayer);
        //EventManager.EventSubscribe(EventType.PLAYER_1_SENDITEM, InteractWithPortal);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYER_1_MOVE_VECT, Player1VectHandler);
        EventManager.EventUnsubscribe(EventType.PLAYER_1_CREATEPORTAL, CreatePortalInFrontOfPlayer);
    }

    private void Player1VectHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("Player1Vect2DHandler is null");
        }

        // Set move direction
        Vector2 input = (Vector2)data;
        MoveDirection.Set(input.x, 0, input.y);
    }

    private void CreatePortalInFrontOfPlayer(object data)
    {
        RiftData.Position = transform.position + transform.forward * DistanceInFront;
        RiftData.Rotation = Quaternion.LookRotation(transform.forward);

        EventManager.EventTrigger(EventType.PORTALMANAGER_CREATEPORTAL, RiftData);
        Debug.Log("Portal created in front of player 1");
    }
}
