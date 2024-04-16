using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    GameObject _levelCams;

    void Awake()
    {
        EventManager.EventInitialise(EventType.ASSIGNMENT_CODE_TRIGGER);
    }
}
