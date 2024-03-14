using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    // Components
    private Rigidbody _rb1;
    private Rigidbody _rb2;
    private BoxCollider _boxCollider1;
    private BoxCollider _boxCollider2;

    //movement
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _velocityPower = 1f;
    [SerializeField] private float _moveAccel = 1f;
    [SerializeField] private float _moveDecel = 1f;
    private Vector2 _moveDirection1;
    private Vector2 _moveDirection2;

    // Object references
    [SerializeField] private GameObject _player1;
    [SerializeField] private GameObject _player2;

    //PickableObjects
    // Assign this in the Inspector, this is the pickup point of the object for player 1
    [SerializeField] private Transform pickupPoint1;
    // Assign this in the Inspector, this is the pickup point of the object for player 2
    [SerializeField] private Transform pickupPoint2;

    void Awake()
    {
        // Set values and components
        _rb1 = _player1.GetComponent<Rigidbody>();
        _rb2 = _player2.GetComponent<Rigidbody>();
        _boxCollider1 = _player1.GetComponent<BoxCollider>();
        _boxCollider2 = _player2.GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYER_1_MOVE_VECT2D, Player1Vect2DHandler);
        EventManager.EventSubscribe(EventType.PLAYER_2_MOVE_VECT2D, Player2Vect2DHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYER_1_MOVE_VECT2D, Player1Vect2DHandler);
        EventManager.EventUnsubscribe(EventType.PLAYER_2_MOVE_VECT2D, Player2Vect2DHandler);
    }

    void Start()
    {
     
    }

    private void FixedUpdate()
    {
        // Set Velocity to zero
        _rb1.velocity = Vector3.zero;
        _rb2.velocity = Vector3.zero;
        
        // then add forces based on movement inputs
        _rb1.AddForce(new Vector3(_moveSpeed * _moveDirection1.x, 0f, _moveSpeed * _moveDirection1.y), ForceMode.Impulse);
        _rb2.AddForce(new Vector3(_moveSpeed * _moveDirection2.x, 0f, _moveSpeed * _moveDirection2.y), ForceMode.Impulse);
    }
    
    private void Player1Vect2DHandler(object data)
    {
        if (data == null)
        {
            Debug.Log("Player1Vect2DHandler is null");
        }

        // Set move direction
        _moveDirection1 = (Vector2)data;
    }

    private void Player2Vect2DHandler(object data)
    {
        if (data == null)
        {
            Debug.Log("Player2Vect2DHandler is null");
        }

        // Set move direction
        _moveDirection2 = (Vector2)data;
    }
}
