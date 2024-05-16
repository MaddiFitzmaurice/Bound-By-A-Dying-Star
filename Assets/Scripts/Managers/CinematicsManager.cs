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
        EventManager.EventInitialise(EventType.CINEMATIC_FINISH);
        EventManager.EventInitialise(EventType.CINEMATIC_START);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.CINEMATIC_START, PlayCinematicHandler);
        _director.stopped += CinematicFinished;
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.CINEMATIC_START, PlayCinematicHandler);
        _director.stopped -= CinematicFinished;
    }

    public void CinematicFinished(PlayableDirector director)
    {
        EventManager.EventTrigger(EventType.CINEMATIC_FINISH, null);
    }

    #region EVENT HANDLERS
    public void PlayCinematicHandler(object data)
    {
        if (data is not PlayableAsset)
        {
            Debug.LogError("CinematicsManager has not received a PlayableAsset!");
        }

        PlayableAsset cinematic = (PlayableAsset)data;
        _director.Play(cinematic); 
    }
    #endregion
}
