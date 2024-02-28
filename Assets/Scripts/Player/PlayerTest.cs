using System.Collections;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    // Components
    private Rigidbody2D _rb;
    private BoxCollider2D _boxCollider;

    //movement
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _velocityPower = 1f;
    [SerializeField] private float _moveAccel = 1f;
    [SerializeField] private float _moveDecel = 1f;
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

        // Calculate desired velocity
        float targetVelocity = _moveDirection.magnitude * _moveSpeed;

        // Find diff between desired velocity and current velocity
        float velocityDif = targetVelocity - _rb.velocity.magnitude;

        // Check whether to accel or deccel
        float accelRate = (Mathf.Abs(targetVelocity) > 0.01f) ? _moveAccel :
            _moveDecel;

        // Calc force by multiplying accel and velocity diff, and applying velocity power
        float movementForce = Mathf.Pow(Mathf.Abs(velocityDif) * accelRate, _velocityPower)
            * Mathf.Sign(velocityDif);

        _rb.AddForce(movementForce * _moveDirection, ForceMode2D.Impulse);
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
