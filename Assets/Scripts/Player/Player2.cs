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
        EventManager.EventSubscribe(EventType.GRAVITY_INVERT, ModifyGravityAndFallingSpeed);
    }

    protected override void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYER_2_MOVE_VECT, Player2VectHandler);
        EventManager.EventUnsubscribe(EventType.PLAYER_2_RIFT, CreatePortalInFrontOfPlayer);
        EventManager.EventUnsubscribe(EventType.PLAYER_2_INTERACT, Interact);
        EventManager.EventUnsubscribe(EventType.TEST_CONTROLS, TestControlHandler);
        EventManager.EventUnsubscribe(EventType.GRAVITY_INVERT, ModifyGravityAndFallingSpeed);
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

    public void ModifyGravityAndFallingSpeed(object data)
    {
        // How long it takes for the player to rotate when falling
        float rotationDuration = 2f;
        StartCoroutine(RotateOverTime(Vector3.forward, 180f, rotationDuration));
    }

    private IEnumerator RotateOverTime(Vector3 axis, float angle, float duration)
    {
        // Stop input from occuring to avoid the object going flying
        EventManager.EventUnsubscribe(EventType.PLAYER_2_MOVE_VECT, Player2VectHandler);

        // In case the player is moving while picking up the object. Will bring the player to a stop
        MoveInput = Vector3.zero;

        // Current rigid body of the player
        Rigidbody rb = GetComponent<Rigidbody>();

        // Sets the players velocity to 0 if they are moving while interacting with the item
        rb.velocity = Vector3.zero;

        Quaternion originalRotation = transform.rotation;
        Quaternion finalRotation = transform.rotation * Quaternion.Euler(axis * angle);
        float elapsedTime = 0f;

        // Keep rotating player until elapsed time
        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Slerp(originalRotation, finalRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final rotation is exact
        transform.rotation = finalRotation;

        // Updates the Input manager's player's angle
        PlayerZAngle = transform.rotation.eulerAngles.z;

        // Wait until the player has "landed" or is sufficiently stable by checking if the velocity is low enough to consider stopped
        yield return new WaitUntil(() => rb.velocity.magnitude < 0.05f);

        // Re-enable movement input
        EventManager.EventSubscribe(EventType.PLAYER_2_MOVE_VECT, Player2VectHandler);
    }

}
