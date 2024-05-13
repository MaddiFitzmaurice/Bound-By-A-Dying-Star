using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1FinalPuzzleDoor : MonoBehaviour
{
    public void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LVL1_DOOR_FINALPUZZLE, UnlockDoor);
    }

    public void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LVL1_DOOR_FINALPUZZLE, UnlockDoor);
    }

    public void UnlockDoor(object data)
    {
        gameObject.SetActive(false);
    }
}
