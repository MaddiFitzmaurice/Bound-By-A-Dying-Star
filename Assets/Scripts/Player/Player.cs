using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Components
    private Rigidbody _rb;

    // Movement
    [SerializeField] protected float MoveSpeed = 5f;
    protected Vector2 MoveDirection;

    void Awake()
    {
        // Set components
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Set Velocity to zero
        _rb.velocity = Vector3.zero;
        
        // then add forces based on movement inputs
        _rb.AddForce(new Vector3(MoveSpeed * MoveDirection.x, 0f, MoveSpeed * MoveDirection.y), ForceMode.Impulse);
    }

    
}
