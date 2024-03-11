using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.iOS;

public class InputManager : MonoBehaviour, InputActions.IGameplayActions, Player1InputActions.IGameplayActions, Player2InputActions.IGameplayActions
{
    // Internal data
    private InputActions _inputs;

    // Input Action Assets
    private Player1InputActions _player1Inputs;
    private Player2InputActions _player2Inputs;
    
    // Input Users
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

        EventManager.EventInitialise(EventType.PLAYER_1_MOVE_VECT2D);
        EventManager.EventInitialise(EventType.PLAYER_2_MOVE_VECT2D);
        EventManager.EventInitialise(EventType.PLAYER_1_INTERACT);
        EventManager.EventInitialise(EventType.PLAYER_2_INTERACT);

        DeviceSetup();
    }

    void OnEnable()
    {
        _inputs.Gameplay.Enable();

        // Subscription to listen for device changes
        InputSystem.onDeviceChange += DeviceChangeHandler;
    }

    void OnDisable()
    {
        _inputs.Gameplay.Disable();

        _player1Inputs.Disable();
        _player2Inputs.Disable();
   
        // Unsubscribing from listening to device changes
        InputSystem.onDeviceChange -= DeviceChangeHandler;
    }

    #region DEVICE FUNCTIONS
    public void DeviceSetup()
    {
        // Create player 1 and 2 InputUsers and associate them with respective Action Assets
        _player1 = InputUser.CreateUserWithoutPairedDevices();
        _player2 = InputUser.CreateUserWithoutPairedDevices();
        _player1.AssociateActionsWithUser(_player1Inputs);
        _player2.AssociateActionsWithUser(_player2Inputs);

        // At least find one gamepad device to pair with player 1
        if (Gamepad.current != null)
        {
            InputUser.PerformPairingWithDevice(Gamepad.current, _player1);
            _player1Inputs.Enable();
        }
        else 
        {
            Debug.Log("Please connect a gamepad to the game!");
        }
    }

    // Handles when devices change states
    public void DeviceChangeHandler(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                // If player 1 has no currently paired devices
                if (_player1.pairedDevices.Count == 0)
                {
                    if (device.description.deviceClass != "Keyboard" || device.description.deviceClass != "Mouse")
                    {
                        InputUser.PerformPairingWithDevice(device, _player1);
                        _player1Inputs.Enable();
                    }
                }
                // If player 2 has no currently paired devices
                else if (_player2.pairedDevices.Count == 0)
                {
                    if (device.description.deviceClass != "Keyboard" || device.description.deviceClass != "Mouse")
                    {
                        InputUser.PerformPairingWithDevice(device, _player2);
                        _player2Inputs.Enable();
                    }
                }
                break;
            case InputDeviceChange.Disconnected:
                // If device has been disconnected from player 1
                if (_player1.pairedDevices.ToList().Contains(device))
                {
                    _player1Inputs.Disable();
                }
                 // Else if device has been disconnected from player 2
                if (_player1.pairedDevices.ToList().Contains(device))
                {
                    _player2Inputs.Disable();
                }
                break;
            case InputDeviceChange.Reconnected:
                // Plugged back in.
                Debug.Log($"Reconnected {device}");
                break;
            default:
                //Debug.Log($"Unknown activity occuring with {device}.");
                break;
        }
    }
    #endregion

    #region ACTIONMAP INTERFACES
    // For testing, if Player 1's WSAD keys are pressed
    public void OnMovePlayer1(InputAction.CallbackContext context)
    {
        EventManager.EventTrigger(EventType.PLAYER_1_MOVE_VECT2D, _inputs.Gameplay.MovePlayer1.ReadValue<Vector2>());
    }

    // For testing, if Player 2's arrow keys are pressed
    public void OnMovePlayer2(InputAction.CallbackContext context)
    {
        EventManager.EventTrigger(EventType.PLAYER_2_MOVE_VECT2D, _inputs.Gameplay.MovePlayer2.ReadValue<Vector2>());
    }

    // For testing, if Player 1's WSAD keys are pressed
    public void OnPlayer1Move(InputAction.CallbackContext context)
    {
        EventManager.EventTrigger(EventType.PLAYER_1_MOVE_VECT2D, _player1Inputs.Gameplay.Player1Move.ReadValue<Vector2>());
    }

    // For testing, if Player 2's arrow keys are pressed
    public void OnPlayer2Move(InputAction.CallbackContext context)
    {
        EventManager.EventTrigger(EventType.PLAYER_2_MOVE_VECT2D, _player2Inputs.Gameplay.Player2Move.ReadValue<Vector2>());
    }

    // For testing, if Player 1 uses the interract E key
    public void OnPlayer1Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            EventManager.EventTrigger(EventType.PLAYER_1_INTERACT, null);
            Debug.Log("PLAYER_1_INTERACT");
        }
    }

    // For testing, if Player 2 uses the interract numpad 0 key
    public void OnPlayer2Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            EventManager.EventTrigger(EventType.PLAYER_2_INTERACT, null);
            Debug.Log("PLAYER_2_INTERACT");
        }
    }

    // TEST CALLBACKS FOR 2 PLAYER TEST
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
