using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODEventManager : MonoBehaviour
{
    [field: Header("Item SFX")]
    [field: SerializeField] public EventReference ItemPickup { get; private set; }
    [field: SerializeField] public EventReference ItemDrop { get; private set; }

    [field: Header("Music Events")]
    [field: SerializeField] public EventReference BackgroundMusic { get; private set; }

    private EventInstance _itemPickupInstance;
    private EventInstance _itemDropInstance;
    private EventInstance _backgroundMusicInstance;

    private void Awake()
    {
        // Initialize events
        EventManager.EventInitialise(EventType.ITEM_PICKUP);
        EventManager.EventInitialise(EventType.ITEM_DROP);
        EventManager.EventInitialise(EventType.BACKGROUND_MUSIC);

        // Preload FMOD event instances
        _itemPickupInstance = RuntimeManager.CreateInstance(ItemPickup);
        _itemDropInstance = RuntimeManager.CreateInstance(ItemDrop);
        _backgroundMusicInstance = RuntimeManager.CreateInstance(BackgroundMusic);

        // Pre-start instances to load resources (but do not release them)
        _itemPickupInstance.start();
        _itemPickupInstance.setPaused(true); // Pause immediately after starting

        _itemDropInstance.start();
        _itemDropInstance.setPaused(true); // Pause immediately after starting

        _backgroundMusicInstance.start();
        _backgroundMusicInstance.setPaused(true); // Pause immediately after starting
    }

    private void OnEnable()
    {
        // Subscribe to events
        EventManager.EventSubscribe(EventType.ITEM_PICKUP, HandleItemPickup);
        EventManager.EventSubscribe(EventType.ITEM_DROP, HandleItemDrop);
        EventManager.EventSubscribe(EventType.BACKGROUND_MUSIC, HandleBackgroundMusic);
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        EventManager.EventUnsubscribe(EventType.ITEM_PICKUP, HandleItemPickup);
        EventManager.EventUnsubscribe(EventType.ITEM_DROP, HandleItemDrop);
        EventManager.EventUnsubscribe(EventType.BACKGROUND_MUSIC, HandleBackgroundMusic);
    }

    private void HandleItemPickup(object data)
    {
        PlayEvent(ItemPickup);
    }

    private void HandleItemDrop(object data)
    {
        PlayEvent(ItemDrop);
    }

    private void HandleBackgroundMusic(object data)
    {
        PlayEvent(BackgroundMusic);
    }

    private void PlayEvent(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstance.start();
        eventInstance.release();
    }
}
