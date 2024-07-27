using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class CutsceneManager : MonoBehaviour
{
    #region INTERNAL DATA
    private PlayableDirector _director;
    #endregion

    #region FRAMEWORK FUNCTIONS
    private void Awake()
    {
        // Init Components
        _director = GetComponent<PlayableDirector>();

        // Init Events
        EventManager.EventInitialise(EventType.CUTSCENE_PLAY);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.CUTSCENE_PLAY, CutscenePlayHandler);
        _director.stopped += CutsceneFinishedHandler;
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.CUTSCENE_PLAY, CutscenePlayHandler);
        _director.stopped -= CutsceneFinishedHandler;
    }
    #endregion

    #region EVENT FUNCTIONS
    public void CutsceneFinishedHandler(PlayableDirector director)
    {
        EventManager.EventTrigger(EventType.ENABLE_GAMEPLAY_INPUTS, null);
        EventManager.EventTrigger(EventType.CUTSCENE_FINISHED, null);
    }

    public void CutscenePlayHandler(object data)
    {
        if (data is not PlayableAsset)
        {
            Debug.LogError("CinematicsManager has not received a PlayableAsset!");
        }

        PlayableAsset cinematic = (PlayableAsset)data;
        EventManager.EventTrigger(EventType.DISABLE_GAMEPLAY_INPUTS, null);
        _director.Play(cinematic); 
    }
    #endregion
}
