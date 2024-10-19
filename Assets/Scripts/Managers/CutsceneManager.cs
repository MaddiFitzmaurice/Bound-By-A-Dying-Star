using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

[RequireComponent(typeof(PlayableDirector))]
public class CutsceneManager : MonoBehaviour
{
    #region EXTERNAL DATA
    [Header("Unskippable Cutscenes")]
    [SerializeField] private List<VideoClip> _preRenderedUnskippables;
    [SerializeField] private List<PlayableAsset> _inGameUnskippables;
    #endregion

    #region INTERNAL DATA
    // Components
    private PlayableDirector _director;
    private VideoPlayer _videoPlayer;

    // Player Skip?
    private bool _player1Skip = false;
    private bool _player2Skip = false;
    #endregion

    #region FRAMEWORK FUNCTIONS
    private void Awake()
    {
        // Init Components
        _director = GetComponent<PlayableDirector>();
        _videoPlayer = GetComponent<VideoPlayer>(); 

        // Init Events
        EventManager.EventInitialise(EventType.INGAME_CUTSCENE_PLAY);
        EventManager.EventInitialise(EventType.INGAME_CUTSCENE_FINISHED);
        EventManager.EventInitialise(EventType.PRERENDERED_CUTSCENE_PLAY);
        EventManager.EventInitialise(EventType.PRERENDERED_CUTSCENE_FINISHED);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.INGAME_CUTSCENE_PLAY, InGameCutscenePlayHandler);
        EventManager.EventSubscribe(EventType.PRERENDERED_CUTSCENE_PLAY, PreRenderedCutscenePlayHandler);
        _director.stopped += InGameCutsceneFinHandler;
        _videoPlayer.loopPointReached += PreRenderedCutsceneFinHandler;
        EventManager.EventSubscribe(EventType.CUTSCENE_SKIP, CutsceneSkipHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.INGAME_CUTSCENE_PLAY, InGameCutscenePlayHandler);
        EventManager.EventUnsubscribe(EventType.PRERENDERED_CUTSCENE_PLAY, PreRenderedCutscenePlayHandler);
        _director.stopped -= InGameCutsceneFinHandler;
        _videoPlayer.loopPointReached -= PreRenderedCutsceneFinHandler;
        EventManager.EventUnsubscribe(EventType.CUTSCENE_SKIP, CutsceneSkipHandler);
    }
    #endregion

    #region CUTSCENE FUNCTIONS
    public void StopInGameCutscene()
    {
        EventManager.EventTrigger(EventType.DISABLE_CUTSCENE_INPUTS, null);
        EventManager.EventTrigger(EventType.ENABLE_GAMEPLAY_INPUTS, null);
        EventManager.EventTrigger(EventType.INGAME_CUTSCENE_FINISHED, _director.playableAsset);
        ResetSkipBools();
    }

    public void StopPreRenderedCutscene()
    {
        _videoPlayer.Stop();
        _videoPlayer.targetTexture.Release();
        EventManager.EventTrigger(EventType.DISABLE_CUTSCENE_INPUTS, null);
        EventManager.EventTrigger(EventType.ENABLE_GAMEPLAY_INPUTS, null);
        ResetSkipBools();
        EventManager.EventTrigger(EventType.RENDERTEX_TOGGLE, false);
        EventManager.EventTrigger(EventType.PRERENDERED_CUTSCENE_FINISHED, _videoPlayer.clip);
    }

    public void SkipCutscene()
    {
        // If in-game cutscene is playing
        if (_director.state == PlayState.Playing)
        {
            _director.Pause();
            _director.time = _director.duration;
            _director.Resume();
            StopInGameCutscene();

        }
        // Else if pre-rendered cutscene is playing
        else if (_videoPlayer.isPlaying)
        {
            StopPreRenderedCutscene();
        }
    }

    public void ResetSkipBools()
    {
        _player1Skip = false;
        _player2Skip = false;
    }

    public bool IsInGameUnskippable(PlayableAsset inGameCutscene)
    {
        foreach (PlayableAsset playable in _inGameUnskippables)
        {
            if (playable == inGameCutscene)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsPreRenderedUnskippable(VideoClip preRenderedCutscene)
    {
        foreach (VideoClip clip in _preRenderedUnskippables)
        {
            if (clip == preRenderedCutscene)
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region EVENT FUNCTIONS
    public void CutsceneSkipHandler(object data)
    {
        if (data is int player)
        {
            if (player == 1)
            {
                if (!_player1Skip)
                {
                    _player1Skip = true;
                }
            }
            else if (player == 2)
            {
                if (!_player2Skip)
                {
                    _player2Skip = true;
                }
            }

            if (_player1Skip && _player2Skip)
            {
                SkipCutscene();
            }
        }
        else
        {
            Debug.LogError("Did not receive int!");
        }
    }

    public void PreRenderedCutscenePlayHandler(object data)
    {
        if (data is VideoClip clip)
        {
            _videoPlayer.clip = clip;

            if (!IsPreRenderedUnskippable(clip))
            {
                EventManager.EventTrigger(EventType.SKIPUI_SHOW, null);
                EventManager.EventTrigger(EventType.ENABLE_CUTSCENE_INPUTS, null);
            }

            EventManager.EventTrigger(EventType.RENDERTEX_TOGGLE, true);
            EventManager.EventTrigger(EventType.DISABLE_GAMEPLAY_INPUTS, null);
            _videoPlayer.targetTexture.Create();
            _videoPlayer.Play();
        }
        else
        {
            Debug.LogError("CutsceneManager has not received a VideoClip!");
        }
    }

    public void PreRenderedCutsceneFinHandler(VideoPlayer source)
    {
        StopPreRenderedCutscene();
    }

    public void InGameCutscenePlayHandler(object data)
    {
        if (data is PlayableAsset playable)
        {

            if (!IsInGameUnskippable(playable))
            {
                EventManager.EventTrigger(EventType.SKIPUI_SHOW, null);
                EventManager.EventTrigger(EventType.ENABLE_CUTSCENE_INPUTS, null);
            }

            EventManager.EventTrigger(EventType.DISABLE_GAMEPLAY_INPUTS, null);
            _director.Play(playable);
        }
        else
        {
            Debug.LogError("CutsceneManager has not received a PlayableAsset!");
        }

    }

    public void InGameCutsceneFinHandler(PlayableDirector director)
    {
        StopInGameCutscene();
    }
    #endregion
}
