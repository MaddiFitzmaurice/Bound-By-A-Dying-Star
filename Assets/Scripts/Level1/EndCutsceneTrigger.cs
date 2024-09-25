using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EndCutsceneTrigger : MonoBehaviour
{
    #region EXTERNAL DATA
    [SerializeField] private VideoClip _endPreRenderedCutscene;
    #endregion
    #region INTERNAL DATA
    // Are both Players inside the trigger?
    private bool _player1In = false;
    private bool _player2In = false;
    #endregion

    #region FRAMEWORK FUNCTIONS
    public void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PRERENDERED_CUTSCENE_FINISHED, EndLevelCutsceneEnd);
    }

    public void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PRERENDERED_CUTSCENE_FINISHED, EndLevelCutsceneEnd);
    }

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
                EndLevelCutsceneStart();
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
                EndLevelCutsceneStart();
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
    #endregion

    private void EndLevelCutsceneStart()
    {
        EventManager.EventTrigger(EventType.PRERENDERED_CUTSCENE_PLAY, _endPreRenderedCutscene);
    }

    private void EndLevelCutsceneEnd(object data)
    {
        if (data is VideoClip clip)
        {
            // Ensure this is the correct cutscene that finished
            if (clip == _endPreRenderedCutscene)
            {
                EventManager.EventTrigger(EventType.DISABLE_GAMEPLAY_INPUTS, null);
                EventManager.EventTrigger(EventType.ENABLE_MAINMENU_INPUTS, null);
                EventManager.EventTrigger(EventType.MAIN_MENU, null);
            }
        }
    }
}
