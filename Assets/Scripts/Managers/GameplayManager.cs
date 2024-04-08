using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] GameObject _gameplayCams;

    void Awake()
    {
        EventManager.EventInitialise(EventType.ASSIGNMENT_CODE_TRIGGER);
        EventManager.EventInitialise(EventType.GAMEPLAY_CAMS_INIT);
    }

    private void Start()
    {
        if (_gameplayCams != null)
        {
            EventManager.EventTrigger(EventType.GAMEPLAY_CAMS_INIT, _gameplayCams);
        }
        else
        {
            Debug.LogError("No Gameplay Cameras assigned in GameplayManager!");
        }
    }
}
