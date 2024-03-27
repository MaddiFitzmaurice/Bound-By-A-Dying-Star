using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Components
    private CharacterController _controller;

    // Movement
    [SerializeField] protected float MoveSpeed = 5f;
    protected Vector2 MoveDirection;

    void Awake()
    {
        // Set components
        _controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        // Converting for character controller
        Vector3 move = new Vector3(MoveDirection.x, 0, MoveDirection.y) * MoveSpeed;

        // Apply gravity manually
        if (!_controller.isGrounded)
        {
            move += Physics.gravity * Time.deltaTime;
        }

        // Move the player
        _controller.Move(move * Time.deltaTime);
    }
}

    

