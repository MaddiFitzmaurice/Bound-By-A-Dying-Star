using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class MainMenuUIManager : MonoBehaviour
{
    private bool _buttonPressed = false; // Stops multiple clicking of same button
    
    public void Start()
    {
        _buttonPressed = false;
    }

    // Starts event sequence which exits the game
    public void QuitButton()
    {
        EventManager.EventTrigger(EventType.QUIT_GAME, null);
    }

    // Starts the event sequence which opens the game scene
    public void PlayButton(int levelNum)
    {
        if (!_buttonPressed)
        {
            EventManager.EventTrigger(EventType.PLAY_GAME, levelNum);
            _buttonPressed = true;
        }
    }
}