using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private EventReference _musicEvent;
    [SerializeField] private EventReference _sfxEvent;
    private FMOD.Studio.EventInstance _musicInstance;
    private FMOD.Studio.EventInstance _sfxInstance;

    private void Awake()
    {
        EventManager.EventInitialise(EventType.MUSIC);
        EventManager.EventInitialise(EventType.SFX);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.MUSIC, MusicEventHandler);
        EventManager.EventSubscribe(EventType.SFX, SFXEventHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.MUSIC, MusicEventHandler);
        EventManager.EventUnsubscribe(EventType.SFX, SFXEventHandler);
    }

    private void MusicEventHandler(object data)
    {
        if (_musicInstance.isValid())
        {
            _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _musicInstance.release();
        }

        _musicInstance = RuntimeManager.CreateInstance(_musicEvent);
        _musicInstance.start();
    }

    private void SFXEventHandler(object data)
    {
        if (data is string sfxName)
        {
            _sfxInstance = RuntimeManager.CreateInstance(sfxName);
            _sfxInstance.start();
            _sfxInstance.release();
        }
        else
        {
            Debug.LogError("SFX Event received invalid data.");
        }
    }
}
