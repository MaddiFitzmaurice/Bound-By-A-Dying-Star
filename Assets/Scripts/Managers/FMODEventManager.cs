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
    [field: SerializeField] public EventReference MirrorPlacement { get; private set; }
    
    [field: Header("Pedestal SFX")]
    [field: SerializeField] public EventReference PedestalRotation { get; private set; }
    [field: SerializeField] public EventReference BeamConnection { get; private set; }

    [field: Header("Pressure Plate SFX")]
    [field: SerializeField] public EventReference PressurePlatePlayer1On { get; private set; }
    [field: SerializeField] public EventReference PressurePlatePlayer1Off { get; private set; }
    [field: SerializeField] public EventReference PressurePlatePlayer2On { get; private set; }
    [field: SerializeField] public EventReference PressurePlatePlayer2Off { get; private set; }
        
    [field: Header("Music Events")]
    [field: SerializeField] public EventReference BackgroundMusic { get; private set; }

    private EventInstance _itemPickupInstance;
    private EventInstance _itemDropInstance;
    private EventInstance _mirrorPlacementInstance;
    private EventInstance _pedestalRotationInstance;
    private EventInstance _beamConnectionInstance;
    private EventInstance _pressurePlatePlayer1OnInstance;
    private EventInstance _pressurePlatePlayer1OffInstance;
    private EventInstance _pressurePlatePlayer2OnInstance;
    private EventInstance _pressurePlatePlayer2OffInstance;
    private EventInstance _backgroundMusicInstance;

    private const int AMBIENT_POSITION = 0; 
    private const int CALM_POSITION = 3428; 
    private const int DEEP_POSITION = 58287; 

    private void Awake()
    {
        // Initialize events
        EventManager.EventInitialise(EventType.ITEM_PICKUP);
        EventManager.EventInitialise(EventType.ITEM_DROP);
        EventManager.EventInitialise(EventType.MIRROR_PLACEMENT);
        EventManager.EventInitialise(EventType.PEDESTAL_ROTATION);
        EventManager.EventInitialise(EventType.BEAM_CONNECTION);
        EventManager.EventInitialise(EventType.PRESSURE_PLATE_PLAYER1_ON);
        EventManager.EventInitialise(EventType.PRESSURE_PLATE_PLAYER1_OFF);
        EventManager.EventInitialise(EventType.PRESSURE_PLATE_PLAYER2_ON);
        EventManager.EventInitialise(EventType.PRESSURE_PLATE_PLAYER2_OFF);
        EventManager.EventInitialise(EventType.BACKGROUND_MUSIC);

        // Preload FMOD event instances
        _itemPickupInstance = RuntimeManager.CreateInstance(ItemPickup);
        _itemDropInstance = RuntimeManager.CreateInstance(ItemDrop);
        _mirrorPlacementInstance = RuntimeManager.CreateInstance(MirrorPlacement);
        _pedestalRotationInstance = RuntimeManager.CreateInstance(PedestalRotation);
        _beamConnectionInstance = RuntimeManager.CreateInstance(BeamConnection);
        _pressurePlatePlayer1OnInstance = RuntimeManager.CreateInstance(PressurePlatePlayer1On);
        _pressurePlatePlayer1OffInstance = RuntimeManager.CreateInstance(PressurePlatePlayer1Off);
        _pressurePlatePlayer2OnInstance = RuntimeManager.CreateInstance(PressurePlatePlayer2On);
        _pressurePlatePlayer2OffInstance = RuntimeManager.CreateInstance(PressurePlatePlayer2Off);
        _backgroundMusicInstance = RuntimeManager.CreateInstance(BackgroundMusic);

        // Pre-start instances to load resources
        _itemPickupInstance.start();
        _itemPickupInstance.setPaused(true);

        _itemDropInstance.start();
        _itemDropInstance.setPaused(true);

        _mirrorPlacementInstance.start();
        _mirrorPlacementInstance.setPaused(true);

        _pedestalRotationInstance.start();
        _pedestalRotationInstance.setPaused(true);

        _beamConnectionInstance.start();
        _beamConnectionInstance.setPaused(true);

        _pressurePlatePlayer1OnInstance.start();
        _pressurePlatePlayer1OnInstance.setPaused(true);

        _pressurePlatePlayer1OffInstance.start();
        _pressurePlatePlayer1OffInstance.setPaused(true);

        _pressurePlatePlayer2OnInstance.start();
        _pressurePlatePlayer2OnInstance.setPaused(true);

        _pressurePlatePlayer2OffInstance.start();
        _pressurePlatePlayer2OffInstance.setPaused(true);

        _backgroundMusicInstance.start();
        _backgroundMusicInstance.setPaused(true);
    }

    private void OnEnable()
    {
        // Subscribe to events
        EventManager.EventSubscribe(EventType.ITEM_PICKUP, HandleItemPickup);
        EventManager.EventSubscribe(EventType.ITEM_DROP, HandleItemDrop);
        EventManager.EventSubscribe(EventType.MIRROR_PLACEMENT, HandleMirrorPlacement);
        EventManager.EventSubscribe(EventType.PEDESTAL_ROTATION, HandlePedestalRotation);
        EventManager.EventSubscribe(EventType.BEAM_CONNECTION, HandleBeamConnection);
        EventManager.EventSubscribe(EventType.PRESSURE_PLATE_PLAYER1_ON, HandlePressurePlatePlayer1On);
        EventManager.EventSubscribe(EventType.PRESSURE_PLATE_PLAYER1_OFF, HandlePressurePlatePlayer1Off);
        EventManager.EventSubscribe(EventType.PRESSURE_PLATE_PLAYER2_ON, HandlePressurePlatePlayer2On);
        EventManager.EventSubscribe(EventType.PRESSURE_PLATE_PLAYER2_OFF, HandlePressurePlatePlayer2Off);
        EventManager.EventSubscribe(EventType.BACKGROUND_MUSIC, HandleBackgroundMusic);
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        EventManager.EventUnsubscribe(EventType.ITEM_PICKUP, HandleItemPickup);
        EventManager.EventUnsubscribe(EventType.ITEM_DROP, HandleItemDrop);
        EventManager.EventUnsubscribe(EventType.MIRROR_PLACEMENT, HandleMirrorPlacement);
        EventManager.EventUnsubscribe(EventType.PEDESTAL_ROTATION, HandlePedestalRotation);
        EventManager.EventUnsubscribe(EventType.BEAM_CONNECTION, HandleBeamConnection);
        EventManager.EventUnsubscribe(EventType.PRESSURE_PLATE_PLAYER1_ON, HandlePressurePlatePlayer1On);
        EventManager.EventUnsubscribe(EventType.PRESSURE_PLATE_PLAYER1_OFF, HandlePressurePlatePlayer1Off);
        EventManager.EventUnsubscribe(EventType.PRESSURE_PLATE_PLAYER2_ON, HandlePressurePlatePlayer2On);
        EventManager.EventUnsubscribe(EventType.PRESSURE_PLATE_PLAYER2_OFF, HandlePressurePlatePlayer2Off);
        EventManager.EventUnsubscribe(EventType.BACKGROUND_MUSIC, HandleBackgroundMusic);
    }

    private void HandleItemPickup(object data)
    {
        PlayEvent(_itemPickupInstance);
    }

    private void HandleItemDrop(object data)
    {
        PlayEvent(_itemDropInstance);
    }

    private void HandleMirrorPlacement(object data)
    {
        PlayEvent(_mirrorPlacementInstance);
    }

    private void HandlePedestalRotation(object data)
    {
        if (data is PedestalRotationData rotationData)
        {
            _pedestalRotationInstance.setParameterByName("Rotation_Speed", rotationData.rotationSpeed);
            _pedestalRotationInstance.set3DAttributes(RuntimeUtils.To3DAttributes(rotationData.position));

            if (rotationData.isRotating)
            {
                _pedestalRotationInstance.setPaused(false);
            }
            else
            {
                _pedestalRotationInstance.setPaused(true);
            }
        }
    }

    private void HandleBeamConnection(object data)
    {
        if (data is Vector3 position)
        {
            _beamConnectionInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
            PlayEvent(_beamConnectionInstance);
        }
    }

    private void HandlePressurePlatePlayer1On(object data)
    {
        PlayEvent(_pressurePlatePlayer1OnInstance);
    }
    private void HandlePressurePlatePlayer1Off(object data)
    {
        PlayEvent(_pressurePlatePlayer1OffInstance);
    }

    private void HandlePressurePlatePlayer2On(object data)
    {
        PlayEvent(_pressurePlatePlayer2OnInstance);
    }
    private void HandlePressurePlatePlayer2Off(object data)
    {
        PlayEvent(_pressurePlatePlayer2OffInstance);
    }

    public void HandleBackgroundMusic(object data)
    {
        if (data is string marker)
        {
            _backgroundMusicInstance.setPaused(false);
            int position = GetMarkerPosition(marker);
            if (position >= 0)
            {
                _backgroundMusicInstance.setTimelinePosition(position);
                _backgroundMusicInstance.start();
                SetLoopRegion(marker);
            }
        }
    }

    private void PlayEvent(EventInstance eventInstance)
    {
        eventInstance.setPaused(false);
        eventInstance.start();
    }

    private int GetMarkerPosition(string marker)
    {
        switch (marker)
        {
            case "Ambient":
                return AMBIENT_POSITION;
            case "Calm":
                return CALM_POSITION;
            case "Deep":
                return DEEP_POSITION;
            default:
                return -1;
        }
    }

    private void SetLoopRegion(string marker)
    {
        
        switch (marker)
        {
            case "Ambient":
                // Enable Ambient loop
                _backgroundMusicInstance.setParameterByName("AmbientLoop", 1);
                _backgroundMusicInstance.setParameterByName("CalmLoop", 0);
                _backgroundMusicInstance.setParameterByName("DeepLoop", 0);
                break;
            case "Calm":
                // Enable Calm loop
                _backgroundMusicInstance.setParameterByName("AmbientLoop", 0);
                _backgroundMusicInstance.setParameterByName("CalmLoop", 1);
                _backgroundMusicInstance.setParameterByName("DeepLoop", 0);
                break;
            case "Deep":
                // Enable Deep loop
                _backgroundMusicInstance.setParameterByName("AmbientLoop", 0);
                _backgroundMusicInstance.setParameterByName("CalmLoop", 0);
                _backgroundMusicInstance.setParameterByName("DeepLoop", 1);
                break;
        }
    }
}

public struct PedestalRotationData
{
    public float rotationSpeed;
    public Vector3 position;
    public bool isRotating;

    public PedestalRotationData(float speed, Vector3 pos, bool rotating)
    {
        rotationSpeed = speed;
        position = pos;
        isRotating = rotating;
    }
}


