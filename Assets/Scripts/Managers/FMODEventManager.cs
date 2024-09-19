using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODEventManager : MonoBehaviour
{
    #region Audio Events
    [Header("Item SFX")]
    [SerializeField] private EventReference _itemPickup;
    [SerializeField] private EventReference _itemDrop;
    [SerializeField] private EventReference _mirrorPlacement;
    [SerializeField] private EventReference _mirrorCarryingPlayer1;
    [SerializeField] private EventReference _mirrorCarryingPlayer2;

    [Header("Pedestal SFX")]
    [SerializeField] private EventReference _pedestalRotation;
    [SerializeField] private EventReference _beamConnection;

    [Header("Pressure Plate SFX")]
    [SerializeField] private EventReference _pressurePlatePlayer1On;
    [SerializeField] private EventReference _pressurePlatePlayer1Off;
    [SerializeField] private EventReference _pressurePlatePlayer2On;
    [SerializeField] private EventReference _pressurePlatePlayer2Off;

    [Header("Puzzle Completion SFX")]
    [SerializeField] private EventReference _constellationComplete;

    [Header("Music Events")]
    [SerializeField] private EventReference _backgroundMusic;
    #endregion

    #region Music Section Parameters
    [Header("Music Sections")]
    [SerializeField] private float _mainAreaLoopStart = 3.424f;
    [SerializeField] private float _mainAreaLoopEnd = 54.859f;
    [SerializeField] private float _softPuzzleLoopStart = 58.286f;
    [SerializeField] private float _softPuzzleLoopEnd = 109.901f;
    [SerializeField] private float _mainAreaTransitionDuration = 3.429f;
    [SerializeField] private float _softPuzzleTransitionDuration = 6.875f;
    #endregion

    #region Private Fields
    private EventInstance _itemPickupInstance;
    private EventInstance _itemDropInstance;
    private EventInstance _mirrorPlacementInstance;
    private EventInstance _mirrorCarryingPlayer1Instance;
    private EventInstance _mirrorCarryingPlayer2Instance;
    private EventInstance _pedestalRotationInstance;
    private EventInstance _beamConnectionInstance;
    private EventInstance _constellationCompleteInstance;
    private EventInstance _pressurePlatePlayer1OnInstance;
    private EventInstance _pressurePlatePlayer1OffInstance;
    private EventInstance _pressurePlatePlayer2OnInstance;
    private EventInstance _pressurePlatePlayer2OffInstance;
    private EventInstance _mainAreaMusicInstance;
    private EventInstance _softPuzzleMusicInstance;
    private bool _isInMainArea = true;

    private const string LOOP_REGION_PARAMETER = "LoopRegion";
    private const string VOLUME_PARAMETER = "Volume";
    #endregion

    private void Awake()
    {
        InitializeEvents();
        CreateEventInstances();
        SetupMusicInstances();
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
        ReleaseMusicInstances();
    }

    private void InitializeEvents()
    {
        EventManager.EventInitialise(EventType.ITEM_PICKUP);
        EventManager.EventInitialise(EventType.ITEM_DROP);
        EventManager.EventInitialise(EventType.MIRROR_PLACEMENT);
        EventManager.EventInitialise(EventType.MIRROR_CARRYING_PLAYER1);
        EventManager.EventInitialise(EventType.MIRROR_CARRYING_PLAYER2);
        EventManager.EventInitialise(EventType.PEDESTAL_ROTATION);
        EventManager.EventInitialise(EventType.BEAM_CONNECTION);
        EventManager.EventInitialise(EventType.CONSTELLATION_COMPLETE);
        EventManager.EventInitialise(EventType.PRESSURE_PLATE_PLAYER1_ON);
        EventManager.EventInitialise(EventType.PRESSURE_PLATE_PLAYER1_OFF);
        EventManager.EventInitialise(EventType.PRESSURE_PLATE_PLAYER2_ON);
        EventManager.EventInitialise(EventType.PRESSURE_PLATE_PLAYER2_OFF);
        EventManager.EventInitialise(EventType.BACKGROUND_MUSIC);
    }

    private void CreateEventInstances()
    {
        _itemPickupInstance = RuntimeManager.CreateInstance(_itemPickup);
        _itemDropInstance = RuntimeManager.CreateInstance(_itemDrop);
        _mirrorPlacementInstance = RuntimeManager.CreateInstance(_mirrorPlacement);
        _mirrorCarryingPlayer1Instance = RuntimeManager.CreateInstance(_mirrorCarryingPlayer1);
        _mirrorCarryingPlayer2Instance = RuntimeManager.CreateInstance(_mirrorCarryingPlayer2);
        _pedestalRotationInstance = RuntimeManager.CreateInstance(_pedestalRotation);
        _beamConnectionInstance = RuntimeManager.CreateInstance(_beamConnection);
        _constellationCompleteInstance = RuntimeManager.CreateInstance(_constellationComplete);
        _pressurePlatePlayer1OnInstance = RuntimeManager.CreateInstance(_pressurePlatePlayer1On);
        _pressurePlatePlayer1OffInstance = RuntimeManager.CreateInstance(_pressurePlatePlayer1Off);
        _pressurePlatePlayer2OnInstance = RuntimeManager.CreateInstance(_pressurePlatePlayer2On);
        _pressurePlatePlayer2OffInstance = RuntimeManager.CreateInstance(_pressurePlatePlayer2Off);
    }

    private void SetupMusicInstances()
    {
        _mainAreaMusicInstance = RuntimeManager.CreateInstance(_backgroundMusic);
        _softPuzzleMusicInstance = RuntimeManager.CreateInstance(_backgroundMusic);

        _mainAreaMusicInstance.setParameterByName(LOOP_REGION_PARAMETER, 0);
        _mainAreaMusicInstance.setTimelinePosition((int)(_mainAreaLoopStart * 1000));
        _mainAreaMusicInstance.setParameterByName(VOLUME_PARAMETER, 1f);
        _mainAreaMusicInstance.start();

        _softPuzzleMusicInstance.setParameterByName(LOOP_REGION_PARAMETER, 1);
        _softPuzzleMusicInstance.setTimelinePosition((int)(_softPuzzleLoopStart * 1000));
        _softPuzzleMusicInstance.setParameterByName(VOLUME_PARAMETER, 0f);
        _softPuzzleMusicInstance.start();
    }

    private void SubscribeToEvents()
    {
        EventManager.EventSubscribe(EventType.ITEM_PICKUP, HandleItemPickup);
        EventManager.EventSubscribe(EventType.ITEM_DROP, HandleItemDrop);
        EventManager.EventSubscribe(EventType.MIRROR_PLACEMENT, HandleMirrorPlacement);
        EventManager.EventSubscribe(EventType.MIRROR_CARRYING_PLAYER1, HandleMirrorCarryingPlayer1);
        EventManager.EventSubscribe(EventType.MIRROR_CARRYING_PLAYER2, HandleMirrorCarryingPlayer2);
        EventManager.EventSubscribe(EventType.PEDESTAL_ROTATION, HandlePedestalRotation);
        EventManager.EventSubscribe(EventType.BEAM_CONNECTION, HandleBeamConnection);
        EventManager.EventSubscribe(EventType.CONSTELLATION_COMPLETE, HandleConstellationComplete);
        EventManager.EventSubscribe(EventType.PRESSURE_PLATE_PLAYER1_ON, HandlePressurePlatePlayer1On);
        EventManager.EventSubscribe(EventType.PRESSURE_PLATE_PLAYER1_OFF, HandlePressurePlatePlayer1Off);
        EventManager.EventSubscribe(EventType.PRESSURE_PLATE_PLAYER2_ON, HandlePressurePlatePlayer2On);
        EventManager.EventSubscribe(EventType.PRESSURE_PLATE_PLAYER2_OFF, HandlePressurePlatePlayer2Off);
        EventManager.EventSubscribe(EventType.BACKGROUND_MUSIC, HandleBackgroundMusic);
    }

    private void UnsubscribeFromEvents()
    {
        EventManager.EventUnsubscribe(EventType.ITEM_PICKUP, HandleItemPickup);
        EventManager.EventUnsubscribe(EventType.ITEM_DROP, HandleItemDrop);
        EventManager.EventUnsubscribe(EventType.MIRROR_PLACEMENT, HandleMirrorPlacement);
        EventManager.EventUnsubscribe(EventType.MIRROR_CARRYING_PLAYER1, HandleMirrorCarryingPlayer1);
        EventManager.EventUnsubscribe(EventType.MIRROR_CARRYING_PLAYER2, HandleMirrorCarryingPlayer2);
        EventManager.EventUnsubscribe(EventType.PEDESTAL_ROTATION, HandlePedestalRotation);
        EventManager.EventUnsubscribe(EventType.BEAM_CONNECTION, HandleBeamConnection);
        EventManager.EventUnsubscribe(EventType.CONSTELLATION_COMPLETE, HandleConstellationComplete);
        EventManager.EventUnsubscribe(EventType.PRESSURE_PLATE_PLAYER1_ON, HandlePressurePlatePlayer1On);
        EventManager.EventUnsubscribe(EventType.PRESSURE_PLATE_PLAYER1_OFF, HandlePressurePlatePlayer1Off);
        EventManager.EventUnsubscribe(EventType.PRESSURE_PLATE_PLAYER2_ON, HandlePressurePlatePlayer2On);
        EventManager.EventUnsubscribe(EventType.PRESSURE_PLATE_PLAYER2_OFF, HandlePressurePlatePlayer2Off);
        EventManager.EventUnsubscribe(EventType.BACKGROUND_MUSIC, HandleBackgroundMusic);
    }

    private void ReleaseMusicInstances()
    {
        if (_mainAreaMusicInstance.isValid())
        {
            _mainAreaMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _mainAreaMusicInstance.release();
        }

        if (_softPuzzleMusicInstance.isValid())
        {
            _softPuzzleMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _softPuzzleMusicInstance.release();
        }
    }

    private void HandleItemPickup(object data) => PlayEvent(_itemPickupInstance);
    private void HandleItemDrop(object data) => PlayEvent(_itemDropInstance);
    private void HandleMirrorPlacement(object data) => PlayEvent(_mirrorPlacementInstance);
    private void HandleConstellationComplete(object data) => PlayEvent(_constellationCompleteInstance);
    private void HandlePressurePlatePlayer1On(object data) => PlayEvent(_pressurePlatePlayer1OnInstance);
    private void HandlePressurePlatePlayer1Off(object data) => PlayEvent(_pressurePlatePlayer1OffInstance);
    private void HandlePressurePlatePlayer2On(object data) => PlayEvent(_pressurePlatePlayer2OnInstance);
    private void HandlePressurePlatePlayer2Off(object data) => PlayEvent(_pressurePlatePlayer2OffInstance);

    private void HandleMirrorCarryingPlayer1(object data)
    {
        if (data is bool isCarrying)
        {
            if (isCarrying)
                _mirrorCarryingPlayer1Instance.start();
            else
                _mirrorCarryingPlayer1Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void HandleMirrorCarryingPlayer2(object data)
    {
        if (data is bool isCarrying)
        {
            if (isCarrying)
                _mirrorCarryingPlayer2Instance.start();
            else
                _mirrorCarryingPlayer2Instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void HandlePedestalRotation(object data)
    {
        if (data is PedestalRotationData rotationData)
        {
            _pedestalRotationInstance.setParameterByName("Rotation_Speed", rotationData.RotationSpeed);
            _pedestalRotationInstance.set3DAttributes(RuntimeUtils.To3DAttributes(rotationData.Position));
            _pedestalRotationInstance.setPaused(!rotationData.IsRotating);
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

    public void HandleBackgroundMusic(object data)
    {
        if (data is string section)
        {
            switch (section)
            {
                case "MainArea":
                    if (!_isInMainArea)
                        StartCoroutine(CrossfadeToMainArea());
                    break;
                case "SoftPuzzle":
                    if (_isInMainArea)
                        StartCoroutine(CrossfadeToSoftPuzzle());
                    break;
            }
        }
    }

    private IEnumerator CrossfadeToMainArea()
    {
        yield return CrossfadeBetweenAreas(_mainAreaMusicInstance, _softPuzzleMusicInstance, _mainAreaTransitionDuration);
        _isInMainArea = true;
    }

    private IEnumerator CrossfadeToSoftPuzzle()
    {
        yield return CrossfadeBetweenAreas(_softPuzzleMusicInstance, _mainAreaMusicInstance, _softPuzzleTransitionDuration);
        _isInMainArea = false;
    }

    private IEnumerator CrossfadeBetweenAreas(EventInstance fadeInInstance, EventInstance fadeOutInstance, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float fadeInVolume = Mathf.Lerp(0f, 1f, t);
            float fadeOutVolume = Mathf.Lerp(1f, 0f, t);

            fadeInInstance.setParameterByName(VOLUME_PARAMETER, fadeInVolume);
            fadeOutInstance.setParameterByName(VOLUME_PARAMETER, fadeOutVolume);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeInInstance.setParameterByName(VOLUME_PARAMETER, 1f);
        fadeOutInstance.setParameterByName(VOLUME_PARAMETER, 0f);
    }

    private void PlayEvent(EventInstance eventInstance)
    {
        eventInstance.setPaused(false);
        eventInstance.start();
    }
}

public struct PedestalRotationData
{
    public float RotationSpeed { get; }
    public Vector3 Position { get; }
    public bool IsRotating { get; }

    public PedestalRotationData(float rotationSpeed, Vector3 position, bool isRotating)
    {
        RotationSpeed = rotationSpeed;
        Position = position;
        IsRotating = isRotating;
    }
}