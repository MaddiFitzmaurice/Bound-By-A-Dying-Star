using System.Collections;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    // Components
    private Rigidbody2D _rb;
    private BoxCollider2D _boxCollider;

    //movement
    [SerializeField] private float _moveSpeed = 3f;
    private Vector2 _moveDirection;

    void Awake()
    {
        // Set values and components
        _rb = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYER_MOVE_VECT2D, MoveVect2DHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYER_MOVE_VECT2D, MoveVect2DHandler);
    }

    void Start()
    {

    }

    private void FixedUpdate()
    {

        _rb.AddForce(_moveSpeed * _moveDirection, ForceMode2D.Impulse);
    }

    private void MoveVect2DHandler(object data)
    {
        if (data == null)
        {
            Debug.Log("MoveVect2DHandler is null");
        }


        _rb.velocity = Vector2.zero;
        _moveDirection = (Vector2)data;
        if (_moveDirection == Vector2.zero)
        {
            _rb.velocity = new Vector2(0, 0);
        }
    }
}
