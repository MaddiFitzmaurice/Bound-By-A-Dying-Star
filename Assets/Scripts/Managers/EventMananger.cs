using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventManager
{
    private static Dictionary<EventType, Action<object>> _gameEventDict = new Dictionary<EventType, Action<object>>();

    // Create an event and add to the dictionary
    public static void EventInitialise(EventType gameEventName)
    {
        if (!_gameEventDict.ContainsKey(gameEventName))
        {
            Action<object> newGameEvent = null;
            _gameEventDict.Add(gameEventName, newGameEvent);
        }
    }

    // Subscribe function handler to event
    public static void EventSubscribe(EventType gameEventName, Action<object> funcToSub)
    {
        // Check if event exists, then sub handler function to it
        if (!_gameEventDict.ContainsKey(gameEventName))
        {
            EventInitialise(gameEventName);
        }

        _gameEventDict[gameEventName] += funcToSub;
    }

    // Unsubscribe function handler from event
    public static void EventUnsubscribe(EventType gameEventName, Action<object> funcToUnsub)
    {
        // Check if event exists, then unsub handler function from it
        if (_gameEventDict.ContainsKey(gameEventName))
        {
            _gameEventDict[gameEventName] -= funcToUnsub;
        }
    }

    // Trigger event
    public static void EventTrigger(EventType gameEventName, object data)
    {
        // Check if event exists, then invoke and execute all handlers subscribed to it
        if (_gameEventDict.ContainsKey(gameEventName))
        {
            _gameEventDict[gameEventName]?.Invoke(data);
        }
    }
}
