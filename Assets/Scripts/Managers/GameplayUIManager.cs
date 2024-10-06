using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviour
{
    #region EXTERNAL DATA
    //[SerializeField] private OffscreenIndicator _indicatorP1;
    //[SerializeField] private OffscreenIndicator _indicatorP2;
    [SerializeField] private Image _artwork;
    [SerializeField] private GameObject _tapPrompt;
    [SerializeField] private GameObject _holdPrompt;
    [SerializeField] private GameObject _preRenderedCutscene;
    [SerializeField] private GameObject _skipCutscenePrompt;
    #endregion

    #region FRAMEWORK FUNCTIONS
    private void Awake()
    {
        // Preload UI elements
        _tapPrompt.SetActive(true);
        _holdPrompt.SetActive(true);
        _tapPrompt.SetActive(false);
        _holdPrompt.SetActive(false);
        _artwork.gameObject.SetActive(false);
        _preRenderedCutscene.SetActive(false);
        _skipCutscenePrompt.SetActive(true);
        _skipCutscenePrompt.SetActive(false);

        // Event Init
        EventManager.EventInitialise(EventType.ARTWORK_SHOW);
        EventManager.EventInitialise(EventType.ARTWORK_HIDE);
        EventManager.EventInitialise(EventType.CAN_MOVE);
        EventManager.EventInitialise(EventType.RENDERTEX_TOGGLE);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.CUTSCENE_PLAY, ShowUI);
        EventManager.EventSubscribe(EventType.PRERENDERED_CUTSCENE_PLAY, ShowUI);
        EventManager.EventSubscribe(EventType.CUTSCENE_FINISHED, HideUI);
        EventManager.EventSubscribe(EventType.PRERENDERED_CUTSCENE_FINISHED, HideUI);
        EventManager.EventSubscribe(EventType.SHOW_PROMPT_HOLD_INTERACT, ShowHoldPrompt);
        EventManager.EventSubscribe(EventType.HIDE_PROMPT_HOLD_INTERACT, HideHoldPrompt);
        EventManager.EventSubscribe(EventType.SHOW_PROMPT_INTERACT, ShowTapPrompt);
        EventManager.EventSubscribe(EventType.HIDE_PROMPT_INTERACT, HideTapPrompt);
        EventManager.EventSubscribe(EventType.ARTWORK_SHOW, ShowArtwork);
        EventManager.EventSubscribe(EventType.ARTWORK_HIDE, HideArtwork);
        EventManager.EventSubscribe(EventType.RENDERTEX_TOGGLE, RenderTexToggle);
    }

    public void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.CUTSCENE_PLAY, ShowUI);
        EventManager.EventUnsubscribe(EventType.PRERENDERED_CUTSCENE_PLAY, ShowUI);
        EventManager.EventUnsubscribe(EventType.CUTSCENE_FINISHED, HideUI);
        EventManager.EventUnsubscribe(EventType.PRERENDERED_CUTSCENE_FINISHED, HideUI);
        EventManager.EventUnsubscribe(EventType.SHOW_PROMPT_HOLD_INTERACT, ShowHoldPrompt);
        EventManager.EventUnsubscribe(EventType.HIDE_PROMPT_HOLD_INTERACT, HideHoldPrompt);
        EventManager.EventUnsubscribe(EventType.SHOW_PROMPT_INTERACT, ShowTapPrompt);
        EventManager.EventUnsubscribe(EventType.HIDE_PROMPT_INTERACT, HideTapPrompt);
        EventManager.EventUnsubscribe(EventType.ARTWORK_SHOW, ShowArtwork);
        EventManager.EventUnsubscribe(EventType.ARTWORK_HIDE, HideArtwork);
        EventManager.EventUnsubscribe(EventType.RENDERTEX_TOGGLE, RenderTexToggle);
    }
    #endregion

    #region EVENT FUNCTIONS
    public void ShowArtwork(object data)
    {
        if (data is Sprite artToShow)
        {
            _artwork.sprite = artToShow;
            _artwork.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("GameplayUI did not receive a Sprite for ShowArtwork!");
        }
    }

    public void HideArtwork(object data)
    {
        _artwork.gameObject.SetActive(false);
    }

    public void RenderTexToggle(object data)
    {
        if (data is bool toggle)
        {
            _preRenderedCutscene.gameObject.SetActive(toggle);
        }
    }

    public void ShowUI(object data)
    {
        _skipCutscenePrompt.SetActive(true);
    }

    public void HideUI(object data)
    {
        _skipCutscenePrompt.SetActive(false);
    }

    public void ShowTapPrompt(object data)
    {
        if (_tapPrompt.activeInHierarchy == false)
        {
            _tapPrompt.SetActive(true);
        }
    }

    public void HideTapPrompt(object data)
    {
        _tapPrompt.SetActive(false);
    }

    public void ShowHoldPrompt(object data)
    {
        if (_holdPrompt.activeInHierarchy == false)
        {
            _holdPrompt.SetActive(true);
        }
    }    

    public void HideHoldPrompt(object data)
    {
        _holdPrompt.SetActive(false);
    }
    #endregion
}
