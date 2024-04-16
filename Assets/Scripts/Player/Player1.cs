using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1 : PlayerBase
{
    protected override void OnEnable()
    {
        base.OnEnable();
        EventManager.EventSubscribe(EventType.PLAYER_1_MOVE_VECT, Player1VectHandler);
        EventManager.EventSubscribe(EventType.PLAYER_1_RIFT, CreatePortalInFrontOfPlayer);
        EventManager.EventSubscribe(EventType.PLAYER_1_INTERACT, Interact);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventManager.EventUnsubscribe(EventType.PLAYER_1_MOVE_VECT, Player1VectHandler);
        EventManager.EventUnsubscribe(EventType.PLAYER_1_RIFT, CreatePortalInFrontOfPlayer);
        EventManager.EventUnsubscribe(EventType.PLAYER_1_INTERACT, Interact);
    }

    private void Player1VectHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("Player1VectHandler is null");
        }

        // Set move direction
        Vector2 input = (Vector2)data;
        MoveInput.Set(input.x, 0, input.y);
    }

    private void CreatePortalInFrontOfPlayer(object data)
    {
        RiftData.Position = transform.position + transform.forward * DistanceInFront;
        RiftData.Rotation = Quaternion.LookRotation(transform.forward);

        EventManager.EventTrigger(EventType.CREATE_RIFT, RiftData);
    }
}
