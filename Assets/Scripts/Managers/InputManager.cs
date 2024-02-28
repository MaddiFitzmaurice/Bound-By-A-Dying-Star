using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, InputActions.IGameplayActions
{
    // Internal data
    private InputActions _inputs;
    private bool _canPause;
    private bool _debugging = false;

    void Awake()
    {
        _inputs = new InputActions();
        _inputs.Gameplay.SetCallbacks(this);

        EventManager.EventInitialise(EventType.PLAYER_MOVE_VECT2D);
    }

    void OnEnable()
    {
        _inputs.Gameplay.Enable();
        EventManager.EventSubscribe(EventType.FADING, PauseAllowedHandler);
    }

    void OnDisable()
    {
        _inputs.Gameplay.Disable();
        EventManager.EventUnsubscribe(EventType.FADING, PauseAllowedHandler);
    }

    public void PauseAllowedHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("PauseAllowedHandler has not received a bool!!!");
        }

        _canPause = (bool)data;
    }

    public void DebuggingHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("DebuggingHandler has not received a bool!!!");
        }

        _debugging = (bool)data;
    }

    // If WSAD or Arrows are pressed
    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log(context);
        EventManager.EventTrigger(EventType.PLAYER_MOVE_VECT2D, _inputs.Gameplay.Move.ReadValue<Vector2>());
    }
}
