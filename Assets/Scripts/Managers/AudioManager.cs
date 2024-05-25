using System;
using System.Collections;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter musicEmitter;
    [SerializeField] private StudioEventEmitter sfxEmitter;
    private bool isMusicMuted = false;

    private void Awake()
    {
        EventManager.EventInitialise(EventType.BACKGROUND);
        EventManager.EventInitialise(EventType.SFX);
        EventManager.EventInitialise(EventType.MUTEMUSIC_TOGGLE);
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.BACKGROUND, PlayBackgroundMusic);
        EventManager.EventSubscribe(EventType.SFX, PlaySFX);
        EventManager.EventSubscribe(EventType.MUTEMUSIC_TOGGLE, ToggleMusicMute);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.BACKGROUND, PlayBackgroundMusic);
        EventManager.EventUnsubscribe(EventType.SFX, PlaySFX);
        EventManager.EventUnsubscribe(EventType.MUTEMUSIC_TOGGLE, ToggleMusicMute);
    }

    private void PlayBackgroundMusic(object data)
    {
        if (isMusicMuted)
            return;

        if (data is string path)
        {
            musicEmitter.Event = path;
            musicEmitter.Play();
        }
        else
        {
            Debug.LogError("Invalid data for PlayBackgroundMusic");
        }
    }

    private void PlaySFX(object data)
    {
        if (data is string path)
        {
            sfxEmitter.Event = path;
            sfxEmitter.Play();
        }
        else
        {
            Debug.LogError("Invalid data for PlaySFX");
        }
    }

    private void ToggleMusicMute(object data)
    {
        isMusicMuted = !isMusicMuted;
        if (isMusicMuted)
        {
            musicEmitter.Stop();
        }
        else
        {
            musicEmitter.Play();
        }
    }
}
