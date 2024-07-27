using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    private FMODEventManager _fmodEventManager;
    private FMOD.Studio.EventInstance _musicInstance;
    private FMOD.Studio.EventInstance _sfxInstance;

    private void Awake()
    {
        // Find the FMODEventManager in the scene
        _fmodEventManager = GetComponent<FMODEventManager>();

        if (_fmodEventManager == null)
        {
            Debug.LogError("FMODEventManager not found in the scene.");
        }

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

        if (_fmodEventManager != null && data is string marker)
        {
            _fmodEventManager.HandleBackgroundMusic(marker);
        }
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
