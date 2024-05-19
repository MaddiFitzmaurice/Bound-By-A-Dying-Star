using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class CinematicsManager : MonoBehaviour
{
    private PlayableDirector _director;

    private void Awake()
    {
        // Init Components
        _director = GetComponent<PlayableDirector>();

        // Init Events
        EventManager.EventInitialise(EventType.PLAY_CINEMATIC);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAY_CINEMATIC, PlayCinematicHandler);
        _director.stopped += CinematicFinished;
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAY_CINEMATIC, PlayCinematicHandler);
        _director.stopped -= CinematicFinished;
    }

    public void CinematicFinished(PlayableDirector director)
    {
        EventManager.EventTrigger(EventType.ENABLE_INPUTS, null);
    }

    #region EVENT HANDLERS
    public void PlayCinematicHandler(object data)
    {
        if (data is not PlayableAsset)
        {
            Debug.LogError("CinematicsManager has not received a PlayableAsset!");
        }

        PlayableAsset cinematic = (PlayableAsset)data;
        EventManager.EventTrigger(EventType.DISABLE_INPUTS, null);
        _director.Play(cinematic); 
    }
    #endregion
}
