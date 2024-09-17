using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftPuzzleManager : MonoBehaviour
{
    #region External Data
    [SerializeField] private GameObject _softPuzzleDoor1;
    [SerializeField] private List<GameObject> _softPuzzleRewards;
    #endregion

    #region Internal Data
    private GameObject _player1;
    private bool _player1InPuzzle;
    private GameObject _player2;
    private bool _player2InPuzzle;
    #endregion

    private void CheckMusicTransition()
    {
        if (_player1InPuzzle && _player2InPuzzle)
        {
            EventManager.EventTrigger(EventType.MUSIC, "SoftPuzzle");
        }
        else if (!_player1InPuzzle && !_player2InPuzzle)
        {
            EventManager.EventTrigger(EventType.MUSIC, "MainArea");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1"))
        {
            _player1 = other.gameObject;
            _player1InPuzzle = true;
        }
        else if (other.CompareTag("Player2"))
        {
            _player2 = other.gameObject;
            _player2InPuzzle = true;
        }
        CheckMusicTransition();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player1"))
        {
            _player1InPuzzle = false;
        }
        else if (other.CompareTag("Player2"))
        {
            _player2InPuzzle = false;
        }
        CheckMusicTransition();
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
