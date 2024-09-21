using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    private FMODEventManager _fmodEventManager;

    private void Awake()
    {
        InitializeFMODEventManager();
        InitializeEvents();
    }

    private void InitializeFMODEventManager()
    {
        _fmodEventManager = GetComponent<FMODEventManager>();

        if (_fmodEventManager == null)
        {
            Debug.LogError("FMODEventManager not found in the scene.");
        }
    }

    private void InitializeEvents()
    {
        EventManager.EventInitialise(EventType.MUSIC);
        EventManager.EventInitialise(EventType.SFX);
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        EventManager.EventSubscribe(EventType.MUSIC, MusicEventHandler);
        EventManager.EventSubscribe(EventType.SFX, SFXEventHandler);
    }

    private void UnsubscribeFromEvents()
    {
        EventManager.EventUnsubscribe(EventType.MUSIC, MusicEventHandler);
        EventManager.EventUnsubscribe(EventType.SFX, SFXEventHandler);
    }

    private void MusicEventHandler(object data)
    {
        if (_fmodEventManager != null && data is string section)
        {
            _fmodEventManager.HandleBackgroundMusic(section);
        }
    }

    private void SFXEventHandler(object data)
    {
        if (data is string sfxName)
        {
            FMOD.Studio.EventInstance sfxInstance = RuntimeManager.CreateInstance(sfxName);
            sfxInstance.start();
            sfxInstance.release();
        }
        else
        {
            Debug.LogError("SFX Event received invalid data.");
        }
    }
}