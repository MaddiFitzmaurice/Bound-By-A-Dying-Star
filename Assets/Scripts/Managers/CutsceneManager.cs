using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

[RequireComponent(typeof(PlayableDirector))]
public class CutsceneManager : MonoBehaviour
{
    #region INTERNAL DATA
    private PlayableDirector _director;
    private VideoPlayer _videoPlayer;
    #endregion

    #region FRAMEWORK FUNCTIONS
    private void Awake()
    {
        // Init Components
        _director = GetComponent<PlayableDirector>();
        _videoPlayer = GetComponent<VideoPlayer>(); 

        // Init Events
        EventManager.EventInitialise(EventType.CUTSCENE_PLAY);
        EventManager.EventInitialise(EventType.PRERENDERED_CUTSCENE_PLAY);
        EventManager.EventInitialise(EventType.PRERENDERED_CUTSCENE_FINISHED);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.CUTSCENE_PLAY, CutscenePlayHandler);
        EventManager.EventSubscribe(EventType.PRERENDERED_CUTSCENE_PLAY, PreRenderedCutscenePlay);
        _director.stopped += CutsceneFinishedHandler;
        _videoPlayer.loopPointReached += PreRenderedCutsceneFinishedHandler;
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.CUTSCENE_PLAY, CutscenePlayHandler);
        EventManager.EventUnsubscribe(EventType.PRERENDERED_CUTSCENE_PLAY, PreRenderedCutscenePlay);
        _director.stopped -= CutsceneFinishedHandler;
        _videoPlayer.loopPointReached -= PreRenderedCutsceneFinishedHandler;
    }
    #endregion

    #region EVENT FUNCTIONS
    public void PreRenderedCutscenePlay(object data)
    {
        if (data is VideoClip clip)
        {
            _videoPlayer.clip = clip;
            EventManager.EventTrigger(EventType.RENDERTEX_TOGGLE, true);
            _videoPlayer.Play();
        }
    }

    public void PreRenderedCutsceneFinishedHandler(VideoPlayer source)
    {
        EventManager.EventTrigger(EventType.RENDERTEX_TOGGLE, false);
        EventManager.EventTrigger(EventType.PRERENDERED_CUTSCENE_FINISHED, _videoPlayer.clip);
    }

    public void CutsceneFinishedHandler(PlayableDirector director)
    {
        EventManager.EventTrigger(EventType.ENABLE_GAMEPLAY_INPUTS, null);
        EventManager.EventTrigger(EventType.CUTSCENE_FINISHED, null);
    }

    public void CutscenePlayHandler(object data)
    {
        if (data is not PlayableAsset)
        {
            Debug.LogError("CutsceneManager has not received a PlayableAsset!");
        }

        PlayableAsset cinematic = (PlayableAsset)data;
        EventManager.EventTrigger(EventType.DISABLE_GAMEPLAY_INPUTS, null);
        _director.Play(cinematic); 
    }
    #endregion
}
