using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer1 : MonoBehaviour
{
    // Components
    private Rigidbody _rb;

    //movement
    [SerializeField] private float _moveSpeed = 5f;
    private Vector2 _moveDirection;

    void Awake()
    {
        // Set components
        _rb = GetComponent<Rigidbody>();
    }

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

    void Start()
    {

    }

    private void FixedUpdate()
    {
        // Set Velocity to zero
        _rb.velocity = Vector3.zero;
        
        // then add forces based on movement inputs
        _rb.AddForce(new Vector3(_moveSpeed * _moveDirection.x, 0f, _moveSpeed * _moveDirection.y), ForceMode.Impulse);
    }

    private void Player1Vect2DHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("Player1Vect2DHandler is null");
        }

        // Set move direction
        _moveDirection = (Vector2)data;
    }
}
