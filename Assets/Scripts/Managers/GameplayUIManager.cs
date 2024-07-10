using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUIManager : MonoBehaviour
{
    #region EXTERNAL DATA
    [SerializeField] private OffscreenIndicator _indicatorP1;
    [SerializeField] private OffscreenIndicator _indicatorP2;
    #endregion
    #region INTERNAL DATA
    // Player Data
    private Player1 _player1;
    private Player2 _player2;

    // Screen Data
    private Camera _cam;
    private RectTransform _screen;
    private bool _showOffscreenUI = true;
    #endregion

    #region FRAMEWORK FUNCTIONS
    private void Awake()
    {
        // Get Components
        _cam = Camera.main;
        _screen = GetComponentInChildren<RectTransform>();
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PLAYERMANAGER_SEND_PLAYER1, ReceivePlayer1Handler);
        EventManager.EventSubscribe(EventType.PLAYERMANAGER_SEND_PLAYER2, ReceivePlayer2Handler);
        EventManager.EventSubscribe(EventType.CUTSCENE_PLAY, HideUI);
        EventManager.EventSubscribe(EventType.CUTSCENE_FINISHED, ShowUI);
    }

    public void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PLAYERMANAGER_SEND_PLAYER1, ReceivePlayer1Handler);
        EventManager.EventUnsubscribe(EventType.PLAYERMANAGER_SEND_PLAYER2, ReceivePlayer2Handler);
        EventManager.EventUnsubscribe(EventType.CUTSCENE_PLAY, HideUI);
        EventManager.EventUnsubscribe(EventType.CUTSCENE_FINISHED, ShowUI);
    }

    public void Update()
    {
        if (_showOffscreenUI)
        {
            _indicatorP1.gameObject.SetActive(_indicatorP1.UpdatePos(_player1, _cam, _screen));
            _indicatorP2.gameObject.SetActive(_indicatorP2.UpdatePos(_player2, _cam, _screen));
        }
        else
        {
            _indicatorP1.gameObject.SetActive(false);
            _indicatorP2.gameObject.SetActive(false);
        }
    }
    #endregion

    #region EVENT FUNCTIONS
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
    #endregion
}
