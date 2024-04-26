using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

public class InputManager : MonoBehaviour, Player1InputActions.IGameplayActions, Player2InputActions.IGameplayActions
{
    // Input Action Assets
    private Player1InputActions _player1Inputs;
    private Player2InputActions _player2Inputs;
    
    // Input Users
    private InputUser _player1;
    private InputUser _player2;

    void Awake()
    {
        //_inputs = new InputActions();
        //_inputs.Gameplay.SetCallbacks(this);

        _player1Inputs = new Player1InputActions();
        _player2Inputs = new Player2InputActions();
        _player1Inputs.Gameplay.SetCallbacks(this);
        _player2Inputs.Gameplay.SetCallbacks(this);

        EventManager.EventInitialise(EventType.PLAYER_1_MOVE_VECT);
        EventManager.EventInitialise(EventType.PLAYER_2_MOVE_VECT);
        EventManager.EventInitialise(EventType.PLAYER_1_INTERACT);
        EventManager.EventInitialise(EventType.PLAYER_2_INTERACT);
        EventManager.EventInitialise(EventType.PLAYER_1_RIFT);
        EventManager.EventInitialise(EventType.PLAYER_2_RIFT);

        DeviceSetup();
    }

    void OnEnable()
    {
        //_inputs.Gameplay.Enable();

        // Subscription to listen for device changes
        InputSystem.onDeviceChange += DeviceChangeHandler;
    }

    void OnDisable()
    {
        //_inputs.Gameplay.Disable();

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

//#if UNITY_EDITOR
        InputUser.PerformPairingWithDevice(Keyboard.current, _player1);
        InputUser.PerformPairingWithDevice(Keyboard.current, _player2);
        _player1Inputs.Enable();
        _player2Inputs.Enable();
//#endif

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
    // Player 1 movement
    public void OnPlayer1Move(InputAction.CallbackContext context)
    {
        EventManager.EventTrigger(EventType.PLAYER_1_MOVE_VECT, context.ReadValue<Vector2>());
    }

    // Player 2 movement
    public void OnPlayer2Move(InputAction.CallbackContext context)
    {
        EventManager.EventTrigger(EventType.PLAYER_2_MOVE_VECT, context.ReadValue<Vector2>());
    }

    // For testing, if Player 1 uses the interact E key
    public void OnPlayer1Interact(InputAction.CallbackContext context)
    {
        // if (context.performed && context.interaction is HoldInteraction)
        // {
        //     // Debug.Log("Hold!");
        //     EventManager.EventTrigger(EventType.PLAYER_1_INTERACT, context);
        // }
        // else if (context.canceled && context.interaction is HoldInteraction)
        // {
        //     // Debug.Log("Release!");
        //     EventManager.EventTrigger(EventType.PLAYER_1_INTERACT, context);
        // }
        // else if (context.started && context.interaction is PressInteraction)
        // {
        //     // Debug.Log("Press!");
        //     EventManager.EventTrigger(EventType.PLAYER_1_INTERACT, context);
        // }
        if (context.started && context.interaction is PressInteraction)
        {
            // Debug.Log("Press!");
            EventManager.EventTrigger(EventType.PLAYER_1_INTERACT, context);
        }
    }

    // For testing, if Player 2 uses the interract numpad 0 key
    public void OnPlayer2Interact(InputAction.CallbackContext context)
    {
        if (context.performed && context.interaction is HoldInteraction)
        {
            Debug.Log("Hold!");
            EventManager.EventTrigger(EventType.PLAYER_2_HOLDINTERACT, "Player 2");
        }
        else if (context.started && context.interaction is PressInteraction)
        {
            Debug.Log("Press!");
            EventManager.EventTrigger(EventType.PLAYER_2_INTERACT, "Player 2");
        }
    }

    public void OnPlayer1GenerateRift(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            EventManager.EventTrigger(EventType.PLAYER_1_RIFT, "Player 1");
        }
    }

    public void OnPlayer2GenerateRift(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            EventManager.EventTrigger(EventType.PLAYER_2_RIFT, "Player 2");
        }
    }
}
#endregion