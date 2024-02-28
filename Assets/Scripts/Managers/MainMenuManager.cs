using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class MainMenuUIManager : MonoBehaviour
{
    private bool _buttonPressed = false; // Stops multiple clicking of same button

    #region Init

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    private void Start()
    {
        _buttonPressed = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    #endregion
    // Button to signal exiting the game
    public void QuitButton()
    {
        EventManager.EventTrigger(EventType.QUIT_GAME, null);
    }

    public void LevelSelectButton(int levelNum)
    {
        if (!_buttonPressed)
        {
            EventManager.EventTrigger(EventType.LEVEL_SELECTED, levelNum);
            _buttonPressed = true;
        }
    }

    public void MainMenuEventHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("MainMenuEventHandler hasn't received a string");
        }

        int code = (int)data;

        switch (code)
        {
            case 1:
                // Quit
                if (!_buttonPressed)
                {
                    EventManager.EventTrigger(EventType.QUIT_GAME, null);
                    _buttonPressed = true;
                }
                break;
            case 2:
                // Play
                if (!_buttonPressed)
                {
                    EventManager.EventTrigger(EventType.LEVEL_SELECTED, 0);
                    _buttonPressed = true;
                }
                break;
            //case 3:
            //    // Options
            //    if (!_buttonPressed)
            //    {

            //        ShowHideSettings();
            //    }
            //    break;
            default:
                
                break;
        }
    }
}