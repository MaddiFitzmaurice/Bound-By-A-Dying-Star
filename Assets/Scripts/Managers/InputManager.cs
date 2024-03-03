using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputManager : MonoBehaviour, InputActions.IGameplayActions, Player1InputActions.IGameplayActions, Player2InputActions.IGameplayActions
{
    // Internal data
    private InputActions _inputs;
    private bool _canPause;
    private bool _debugging = false;

    private Player1InputActions _player1Inputs;
    private Player2InputActions _player2Inputs;
    private InputUser _player1;
    private InputUser _player2;

    void Awake()
    {
        _inputs = new InputActions();
        _inputs.Gameplay.SetCallbacks(this);

        _player1Inputs = new Player1InputActions();
        _player2Inputs = new Player2InputActions();
        _player1Inputs.Gameplay.SetCallbacks(this);
        _player2Inputs.Gameplay.SetCallbacks(this);

        EventManager.EventInitialise(EventType.PLAYER_MOVE_VECT2D);

        DeviceSetup();
    }

    void OnEnable()
    {
        _inputs.Gameplay.Enable();

        // Subscription to listen for device changes
        InputSystem.onDeviceChange += DeviceChangeHandler;

        EventManager.EventSubscribe(EventType.FADING, PauseAllowedHandler);
    }

    void OnDisable()
    {
        _inputs.Gameplay.Disable();
   
        // Unsubscribing from listening to device changes
        InputSystem.onDeviceChange -= DeviceChangeHandler;

        EventManager.EventUnsubscribe(EventType.FADING, PauseAllowedHandler);
    }

    public void DeviceSetup()
    {
        if (Gamepad.all[0] != null)
        {
            _player1Inputs.devices = new [] { Gamepad.all[0] };
        }

        if (Gamepad.all[1] != null)
        {
            _player2Inputs.devices = new [] { Gamepad.all[1] };
        }
    }

    // Handles when devices change states
    public void DeviceChangeHandler(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                // New Device.
                break;
            case InputDeviceChange.Disconnected:
                // Device got unplugged.
                break;
            case InputDeviceChange.Reconnected:
                // Plugged back in.
                break;
            case InputDeviceChange.Removed:
                // Remove from Input System entirely; by default, Devices stay in the system once discovered.
                break;
            default:
                // See InputDeviceChange reference for other event types.
                break;
        }
    }

    public void PauseAllowedHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("PauseAllowedHandler has not received a bool!!!");
        }

        _canPause = (bool)data;
    }

    #region ACTIONMAP INTERFACES
    // If WSAD or Arrows are pressed
    public void OnMove(InputAction.CallbackContext context)
    {
        EventManager.EventTrigger(EventType.PLAYER_MOVE_VECT2D, _inputs.Gameplay.Move.ReadValue<Vector2>());
    }

    public void OnPlayer1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("PLAYER 1");
        }
    }

    public void OnPlayer2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("PLAYER 2");
        }    
    }
    #endregion
}
