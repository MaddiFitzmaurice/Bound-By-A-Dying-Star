using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : PlayerBase
{
    protected override void OnEnable()
    {
        base.OnEnable();
        EventManager.EventSubscribe(EventType.PLAYER_2_MOVE_VECT, Player2VectHandler);
        EventManager.EventSubscribe(EventType.PLAYER_2_RIFT, CreatePortalInFrontOfPlayer);
        EventManager.EventSubscribe(EventType.PLAYER_2_INTERACT, Interact);
    }

    protected override void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYER_2_MOVE_VECT, Player2VectHandler);
        EventManager.EventUnsubscribe(EventType.PLAYER_2_RIFT, CreatePortalInFrontOfPlayer);
        EventManager.EventUnsubscribe(EventType.PLAYER_2_INTERACT, Interact);
        EventManager.EventUnsubscribe(EventType.TEST_CONTROLS, TestControlHandler);
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
        
        EventManager.EventTrigger(EventType.CREATE_RIFT, RiftData);
    }

    public void ModifyGravityAndFallingSpeed()
    {
        float rotationDuration = 2f;
        // Disable movement input during rotating and falling
        EventManager.EventUnsubscribe(EventType.PLAYER_2_MOVE_VECT, Player2VectHandler);
        StartCoroutine(RotateOverTime(Vector3.forward, 180f, rotationDuration));
    }

    private IEnumerator RotateOverTime(Vector3 axis, float angle, float duration)
    {
        Quaternion originalRotation = transform.rotation;
        Quaternion finalRotation = transform.rotation * Quaternion.Euler(axis * angle);
        float elapsedTime = 0f;

        // Get the player's rigid body
        Rigidbody rb = GetComponent<Rigidbody>();

        // Keep rotating player until elapsed time
        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Slerp(originalRotation, finalRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final rotation is exact
        transform.rotation = finalRotation;
        PlayerZAngle = transform.rotation.eulerAngles.z;

        // Wait until the player has "landed" or is sufficiently stable by checking if the velocity is low enough to consider stopped
        yield return new WaitUntil(() => rb.velocity.magnitude < 0.05f);

        // Re-enable movement input
        EventManager.EventSubscribe(EventType.PLAYER_2_MOVE_VECT, Player2VectHandler);
    }

}
