using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private EventReference musicEvent;
    [SerializeField] private EventReference sfxEvent;
    private FMOD.Studio.EventInstance musicInstance;
    private FMOD.Studio.EventInstance sfxInstance;

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
        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicInstance.release();
        }

        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        musicInstance.start();
    }

    private void SFXEventHandler(object data)
    {
        if (data is string sfxName)
        {
            sfxInstance = RuntimeManager.CreateInstance(sfxName);
            sfxInstance.start();
            sfxInstance.release();
        }
        else
        {
            Debug.LogError("SFX Event received invalid data.");
        }
    }
}
