using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEventManager : MonoBehaviour
{
    [field: Header("Item SFX")]
    [field: SerializeField] public EventReference ItemPickup { get; private set; }
    [field: SerializeField] public EventReference ItemDrop { get; private set; }

    [field: Header("Music Events")]
    [field: SerializeField] public EventReference BackgroundMusic { get; private set; }

    private void Awake()
    {
        // Initialize events
        EventManager.EventInitialise(EventType.ITEM_PICKUP);
        EventManager.EventInitialise(EventType.ITEM_DROP);
        EventManager.EventInitialise(EventType.BACKGROUND_MUSIC);
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
        RuntimeManager.PlayOneShot(ItemPickup, transform.position);
    }

    private void HandleItemDrop(object data)
    {
        RuntimeManager.PlayOneShot(ItemDrop, transform.position);
    }

    private void HandleBackgroundMusic(object data)
    {
        RuntimeManager.PlayOneShot(BackgroundMusic, transform.position);
    }
}