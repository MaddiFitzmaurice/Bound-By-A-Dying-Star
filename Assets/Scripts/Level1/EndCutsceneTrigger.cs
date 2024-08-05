using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCutsceneTrigger : MonoBehaviour
{
    // Are both Players inside the trigger?
    private bool _player1In = false;
    private bool _player2In = false;

    public void OnTriggerEnter(Collider other)
    {
        // Check if Player 1 entered trigger
        var p1 = other.GetComponent<Player1>();

        if (p1 != null)
        {
            _player1In = true;

            // If both players are in trigger, activate camera
            if (_player2In)
            {
                EndLevelCutscene();
            }

            return;
        }

        // If no Player 1, check Player 2
        var p2 = other.GetComponent<Player2>();

        if (p2 != null)
        {
            _player2In = true;

            // If both players are in trigger, activate camera
            if (_player1In)
            {
                EndLevelCutscene();
            }

            return;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        // Check if Player 1 exited trigger
        var p1 = other.GetComponent<Player1>();

        if (p1 != null)
        {
            _player1In = false;
            return;
        }

        // If no Player 1, check Player 2
        var p2 = other.GetComponent<Player2>();

        if (p2 != null)
        {
            _player2In = false;
            return;
        }
    }

    private void EndLevelCutscene()
    {
        EventManager.EventTrigger(EventType.DISABLE_GAMEPLAY_INPUTS, null);
        EventManager.EventTrigger(EventType.ENABLE_MAINMENU_INPUTS, null);
        EventManager.EventTrigger(EventType.MAIN_MENU, null);
    }
}
