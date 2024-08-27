using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

public enum InteractTypes
{
    PRESS,
    HOLD,
    RELEASE_HOLD
}

public class InputManager : MonoBehaviour, Player1InputActions.IGameplayActions, Player2InputActions.IGameplayActions, Player1InputActions.IMainMenuActions
{
    // Input Action Assets
    private Player1InputActions _player1Inputs;
    private Player2InputActions _player2Inputs;

    // Input Users
    private InputUser _player1;
    private InputUser _player2;

    // Hold flags
    private bool _holdInteractP1 = false;
    private bool _holdInteractP2 = false;

    // Button Press Flags
    private bool _pressedStart = false;
    private bool _pressedQuit = false;
    private bool _pressedRestart = false;

    #region FRAMEWORK FUNCTIONS
    void Awake()
    {
        //_inputs = new InputActions();
        //_inputs.Gameplay.SetCallbacks(this);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _player1Inputs = new Player1InputActions();
        _player2Inputs = new Player2InputActions();
        _player1Inputs.Gameplay.SetCallbacks(this);
        _player1Inputs.MainMenu.SetCallbacks(this);
        _player2Inputs.Gameplay.SetCallbacks(this);

        EventManager.EventInitialise(EventType.ENABLE_GAMEPLAY_INPUTS);
        EventManager.EventInitialise(EventType.DISABLE_GAMEPLAY_INPUTS);
        EventManager.EventInitialise(EventType.PLAYER_1_MOVE);
        EventManager.EventInitialise(EventType.PLAYER_2_MOVE);
        EventManager.EventInitialise(EventType.PLAYER_1_INTERACT);
        EventManager.EventInitialise(EventType.PLAYER_2_INTERACT);

        DeviceSetup();
    }

    void OnEnable()
    {
        //_inputs.Gameplay.Enable();
        EnableGameplayInput(null);
        EnableMainMenuInput(null);
        // Subscription to listen for device changes
        InputSystem.onDeviceChange += DeviceChangeHandler;

        // Event Subscriptions
        EventManager.EventSubscribe(EventType.DISABLE_GAMEPLAY_INPUTS, DisableGameplayInput);
        EventManager.EventSubscribe(EventType.ENABLE_GAMEPLAY_INPUTS, EnableGameplayInput);
        EventManager.EventSubscribe(EventType.ENABLE_MAINMENU_INPUTS, EnableMainMenuInput);
        EventManager.EventSubscribe(EventType.DISABLE_MAINMENU_INPUTS, DisableMainMenuInput);
    }

    void OnDisable()
    {
        DisableGameplayInput(null);
        DisableMainMenuInput(null);

        // Unsubscribing from listening to device changes
        InputSystem.onDeviceChange -= DeviceChangeHandler;

        // Event Unsubscriptions
        EventManager.EventUnsubscribe(EventType.DISABLE_GAMEPLAY_INPUTS, DisableGameplayInput);
        EventManager.EventUnsubscribe(EventType.ENABLE_GAMEPLAY_INPUTS, EnableGameplayInput);
        EventManager.EventUnsubscribe(EventType.ENABLE_MAINMENU_INPUTS, EnableMainMenuInput);
        EventManager.EventUnsubscribe(EventType.DISABLE_MAINMENU_INPUTS, DisableMainMenuInput);
    }
    #endregion

    #region DEVICE FUNCTIONS
    public void DeviceSetup()
    {
        // Create player 1 and 2 InputUsers and associate them with respective Action Assets
        _player1 = InputUser.CreateUserWithoutPairedDevices();
        _player2 = InputUser.CreateUserWithoutPairedDevices();
        _player1.AssociateActionsWithUser(_player1Inputs);
        _player2.AssociateActionsWithUser(_player2Inputs);

        // Enable keyboard input if using Unity Editor instead of build
        //#if UNITY_EDITOR
        InputUser.PerformPairingWithDevice(Keyboard.current, _player1);
        InputUser.PerformPairingWithDevice(Keyboard.current, _player2);
        //EnableGameplayInput(null);
        //#endif

        // At least find one gamepad device to pair with player 1
        if (Gamepad.current != null)
        {
            InputUser.PerformPairingWithDevice(Gamepad.all[0], _player1);
            _player1Inputs.Enable();
            
            // If another gamepad, pair device with player 2
            if (Gamepad.all.Count > 1)
            {
                InputUser.PerformPairingWithDevice(Gamepad.all[1], _player2);
                _player2Inputs.Enable();
            }
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
                // Make sure incoming device is not a keyboard or mouse
                if (device.description.deviceClass != "Keyboard" || device.description.deviceClass != "Mouse")
                {
                    bool p1HasDevice = false;

                    // Check devices connected to player 1
                    foreach (InputDevice p1ConDevice in _player1.pairedDevices)
                    {
                        // If same device class, break out of for loop
                        if (p1ConDevice.description.deviceClass == device.description.deviceClass)
                        {
                            p1HasDevice = true;
                            break;
                        }
                    }

                    // Assign device to player 1 and break out of switch
                    if (!p1HasDevice)
                    {
                        Debug.Log("Player 1: " + device.description);
                        InputUser.PerformPairingWithDevice(device, _player1);
                        _player1Inputs.Enable();
                        break;
                    }

                    bool p2HasDevice = false;

                    // Check devices connected to player 2
                    foreach (InputDevice p1ConDevice in _player2.pairedDevices)
                    {
                        // If same device class, break out of for loop
                        if (p1ConDevice.description.deviceClass == device.description.deviceClass)
                        {
                            p2HasDevice = true;
                            break;
                        }
                    }

                    // Assign device to player 2 and break out of switch
                    if (!p2HasDevice)
                    {
                        Debug.Log("Player 2: " + device.description);
                        InputUser.PerformPairingWithDevice(device, _player2);
                        _player2Inputs.Enable();
                        break;
                    }
                }
                break;
            case InputDeviceChange.Disconnected:
                // If device has been disconnected from player 1
                if (_player1.pairedDevices.ToList().Contains(device))
                {
                    //_player1Inputs.Disable();
                }
                // Else if device has been disconnected from player 2
                if (_player2.pairedDevices.ToList().Contains(device))
                {
                    //_player2Inputs.Disable();
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

    #region ACTIONMAP MAIN MENU INTERFACE
    public void OnStartGame(InputAction.CallbackContext context)
    {
        // Start level 1
        if (context.performed && !_pressedStart)
        {
            _pressedStart = true;
            _pressedRestart = false;
            EventManager.EventTrigger(EventType.PLAY_GAME, 1);
        }
    }

    public void OnEndGame(InputAction.CallbackContext context)
    {
        if (context.performed && !_pressedQuit)
        {
            _pressedQuit = true;
            EventManager.EventTrigger(EventType.QUIT_GAME, null);
        }
    }
    #endregion

    #region ACTIONMAP GAMEPLAY INTERFACE
    // Player 1 movement
    public void OnPlayer1Move(InputAction.CallbackContext context)
    {
        EventManager.EventTrigger(EventType.PLAYER_1_MOVE, context.ReadValue<Vector2>());
    }

    // Player 2 movement
    public void OnPlayer2Move(InputAction.CallbackContext context)
    {
        EventManager.EventTrigger(EventType.PLAYER_2_MOVE, context.ReadValue<Vector2>());
    }

    // Player 1 interact
    public void OnPlayer1Interact(InputAction.CallbackContext context)
    {
        // Hold performed
        if (context.performed && context.interaction is HoldInteraction)
        {
            _holdInteractP1 = true;
            EventManager.EventTrigger(EventType.PLAYER_1_INTERACT, InteractTypes.HOLD);
        }
        // Press performed
        else if (context.started && context.interaction is PressInteraction)
        {
            EventManager.EventTrigger(EventType.PLAYER_1_INTERACT, InteractTypes.PRESS);
        }
        // Hold released
        else if (context.canceled && _holdInteractP1)
        {
            _holdInteractP1 = false;
            EventManager.EventTrigger(EventType.PLAYER_1_INTERACT, InteractTypes.RELEASE_HOLD);
        }
    }

    // Player 2 interact
    public void OnPlayer2Interact(InputAction.CallbackContext context)
    {
        // Hold performed
        if (context.performed && context.interaction is HoldInteraction)
        {
            _holdInteractP2 = true;
            EventManager.EventTrigger(EventType.PLAYER_2_INTERACT, InteractTypes.HOLD);
        }
        // Press performed
        else if (context.started && context.interaction is PressInteraction)
        {
            EventManager.EventTrigger(EventType.PLAYER_2_INTERACT, InteractTypes.PRESS);
        }
        // Hold released
        else if (context.canceled && _holdInteractP1)
        {
            _holdInteractP2 = false;
            EventManager.EventTrigger(EventType.PLAYER_2_INTERACT, InteractTypes.RELEASE_HOLD);
        }
    }
    
    // FOR OPEN DAY TO RESTART LEVEL 1
    public void OnRestartLevel1(InputAction.CallbackContext context)
    {
        if (context.performed && !_pressedRestart)
        {
            _pressedRestart = true;
            _pressedStart = false;
            EventManager.EventTrigger(EventType.MAIN_MENU, null);
        }
    }
    #endregion

    #region EVENT HANDLERS
    public void EnableGameplayInput(object data)
    {
        _player1Inputs.Gameplay.Enable();
        _player2Inputs.Gameplay.Enable();
    }

    public void DisableGameplayInput(object data)
    {
        _player1Inputs.Gameplay.Disable();
        _player2Inputs.Gameplay.Disable();
    }

    private void EnableMainMenuInput(object data)
    {
        _player1Inputs.MainMenu.Enable();
    }

    private void DisableMainMenuInput(object data)
    {
        _player1Inputs.MainMenu.Disable();
    }
    #endregion
}