using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUIManager : MonoBehaviour
{
    #region EXTERNAL DATA
    [SerializeField] private OffscreenIndicator _indicatorP1;
    [SerializeField] private OffscreenIndicator _indicatorP2;
    [SerializeField] private GameObject _tapPrompt;
    [SerializeField] private GameObject _holdPrompt;
    #endregion
    #region INTERNAL DATA
    // Player Data
    private Player1 _player1;
    private Player2 _player2;
    private bool _isP1Offscreen = false;
    private bool _isP2Offscreen = false;

    // Screen Data
    private Camera _cam;
    private RectTransform _screen;
    private bool _showOffscreenUI = true;
    private bool _p1IndicatorShown = false;
    private bool _p2IndicatorShown = false;
    #endregion

    #region FRAMEWORK FUNCTIONS
    private void Awake()
    {
        // Get Components
        _cam = Camera.main;
        _screen = GetComponentInChildren<RectTransform>();
        _tapPrompt.SetActive(true);
        _holdPrompt.SetActive(true);
        _tapPrompt.SetActive(false);
        _holdPrompt.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYERMANAGER_SEND_PLAYER1, ReceivePlayer1Handler);
        EventManager.EventSubscribe(EventType.PLAYERMANAGER_SEND_PLAYER2, ReceivePlayer2Handler);
        EventManager.EventSubscribe(EventType.CUTSCENE_PLAY, HideUI);
        EventManager.EventSubscribe(EventType.CUTSCENE_FINISHED, ShowUI);
        EventManager.EventSubscribe(EventType.SHOW_PROMPT_HOLD_INTERACT, ShowHoldPrompt);
        EventManager.EventSubscribe(EventType.HIDE_PROMPT_HOLD_INTERACT, HideHoldPrompt);
        EventManager.EventSubscribe(EventType.SHOW_PROMPT_INTERACT, ShowTapPrompt);
        EventManager.EventSubscribe(EventType.HIDE_PROMPT_INTERACT, HideTapPrompt);
        EventManager.EventSubscribe(EventType.PLAYER1_ISOFFSCREEN, IsP1OffscreenHandler);
        EventManager.EventSubscribe(EventType.PLAYER2_ISOFFSCREEN, IsP2OffscreenHandler);
    }

    public void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYERMANAGER_SEND_PLAYER1, ReceivePlayer1Handler);
        EventManager.EventUnsubscribe(EventType.PLAYERMANAGER_SEND_PLAYER2, ReceivePlayer2Handler);
        EventManager.EventUnsubscribe(EventType.CUTSCENE_PLAY, HideUI);
        EventManager.EventUnsubscribe(EventType.CUTSCENE_FINISHED, ShowUI);
        EventManager.EventUnsubscribe(EventType.SHOW_PROMPT_HOLD_INTERACT, ShowHoldPrompt);
        EventManager.EventUnsubscribe(EventType.HIDE_PROMPT_HOLD_INTERACT, HideHoldPrompt);
        EventManager.EventUnsubscribe(EventType.SHOW_PROMPT_INTERACT, ShowTapPrompt);
        EventManager.EventUnsubscribe(EventType.HIDE_PROMPT_INTERACT, HideTapPrompt);
        EventManager.EventUnsubscribe(EventType.PLAYER1_ISOFFSCREEN, IsP1OffscreenHandler);
        EventManager.EventUnsubscribe(EventType.PLAYER2_ISOFFSCREEN, IsP2OffscreenHandler);
    }

    public void Update()
    {
        UpdateOffscreenIndicators();    
    }
    #endregion

    private void UpdateOffscreenIndicators()
    {
        // If we're not in a cutscene
        if (_showOffscreenUI)
        {
            // If P1 is offscreen
            if (_isP1Offscreen)
            {
                // If P1 indicator is not already active
                if (!_p1IndicatorShown)
                {
                    _indicatorP1.gameObject.SetActive(true);
                    _p1IndicatorShown = true;
                }

                // Update indicator pos
                _indicatorP1.UpdatePos(_player1, _cam, _screen);
            }
            // If P1 is onscreen
            else
            {
                // If P1 indicator is active
                if (_p1IndicatorShown)
                {
                    _indicatorP1.gameObject.SetActive(false);
                    _p1IndicatorShown = false;
                }
            }

            // If P2 is offscreen
            if (_isP2Offscreen)
            {
                // If P2 indicator is not already active
                if (!_p2IndicatorShown)
                {
                    _indicatorP2.gameObject.SetActive(true);
                    _p2IndicatorShown = true;
                }

                // Update indicator pos
                _indicatorP2.UpdatePos(_player2, _cam, _screen);
            }
            // If P2 is onscreen
            else
            {
                // If P1 indicator is active
                if (_p2IndicatorShown)
                {
                    _indicatorP2.gameObject.SetActive(false);
                    _p2IndicatorShown = false;
                }
            }
        }
    }

    #region EVENT FUNCTIONS
    public void IsP1OffscreenHandler(object data)
    {
        // If receiving just a bool, means player is back on screen
        if (data is bool isOffscreen)
        {
            _isP1Offscreen = isOffscreen;
        }
        // Else, if tuple, player is offscreen
        else if (data is Tuple<bool, Plane> tuple)
        {
            _isP1Offscreen = tuple.Item1;
        }
        else
        {
            Debug.LogError("Did not receive a bool or tuple<bool, Plane>!");
        }
    }

    public void IsP2OffscreenHandler(object data)
    {
        // If receiving just a bool, means player is back on screen
        if (data is bool isOffscreen)
        {
            _isP2Offscreen = isOffscreen;
        }
        // Else, if tuple, player is offscreen
        else if (data is Tuple<bool, Plane> tuple)
        {
            _isP2Offscreen = tuple.Item1;
        }
        else
        {
            Debug.LogError("Did not receive a bool or tuple<bool, Plane>!");
        }
    }

    public void ReceivePlayer1Handler(object data)
    {
        if (data is Player1 player1)
        {
            _player1 = player1;
        }
        else
        {
            Debug.LogError("GameplayUIManager has not received Player1");
        }
    } 

    public void ReceivePlayer2Handler(object data)
    {
        if (data is Player2 player2)
        {
            _player2 = player2;
        }
        else
        {
            Debug.LogError("GameplayUIManager has not received Player2");
        }
    }

    public void ShowUI(object data)
    {
        _showOffscreenUI = true;
    }

    public void HideUI(object data)
    {
        _showOffscreenUI = false;
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
