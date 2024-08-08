using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateTriggerAnimation : MonoBehaviour
{
    [SerializeField] private GameObject _plate; // Reference to the plate object
    [SerializeField] private ParticleSystem _glowEffect; // Reference to the glow particle system
    [SerializeField] private float _sinkDistance = 0.2f;
    [SerializeField] private float _sinkSpeed = 2.0f;

    private const string _playerTag1 = "Player1";
    private const string _playerTag2 = "Player2";

    private Color _player1Color = Color.blue;
    private Color _player2Color = new Color(1f, 0.5f, 0f); // Orange

    private Vector3 _originalPosition;
    private Vector3 _targetPosition;
    private bool _isPressed = false;
    private int _objectsOnPlate = 0;

    private AudioSource _audioSource;
    private AudioClip _tileDraggingClip;

    void Start()
    {
        if (_plate == null)
        {
            Debug.LogError("Missing plate reference");
            return;
        }
        if (_glowEffect == null)
        {
            Debug.LogError("Missing particle system");
            return;
        }

        _originalPosition = _plate.transform.localPosition;
        _targetPosition = _originalPosition - new Vector3(0, _sinkDistance, 0);

        // Initialize and configure the AudioSource
        _audioSource = gameObject.AddComponent<AudioSource>();

        _tileDraggingClip = Resources.Load<AudioClip>("Tile_Dragging_Demo");

        if (_tileDraggingClip == null)
        {
            Debug.LogError("Tile_Dragging_Demo audio clip not in Resources Folder");
        }
        else
        {
            _audioSource.clip = _tileDraggingClip;
            _audioSource.volume = 1.0f;
            _audioSource.playOnAwake = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag1) || other.CompareTag(_playerTag2))
        {
            _objectsOnPlate++;
            if (!_isPressed)
            {
                _isPressed = true;
                StopAllCoroutines();
                StartCoroutine(SinkPlate(_targetPosition));
                _glowEffect.Play();
                _audioSource.Play();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag1) || other.CompareTag(_playerTag2))
        {
            _objectsOnPlate--;
            if (_objectsOnPlate == 0)
            {
                _isPressed = false;
                StopAllCoroutines();
                StartCoroutine(SinkPlate(_originalPosition));
                _glowEffect.Stop();
                _audioSource.Play();
            }
        }
    }

    IEnumerator SinkPlate(Vector3 target)
    {
        while (Vector3.Distance(_plate.transform.localPosition, target) > 0.01f)
        {
            _plate.transform.localPosition = Vector3.MoveTowards(_plate.transform.localPosition, target, _sinkSpeed * Time.deltaTime);
            yield return null;
        }
        _plate.transform.localPosition = target;
    }

    /*
    void ChangeGlowEffectColor(string playerTag)
    {
        var mainModule = _glowEffect.main;
        if (playerTag == _playerTag1)
        {
            mainModule.startColor = _player1Color;
        }
        else if (playerTag == _playerTag2)
        {
            mainModule.startColor = _player2Color;
        }
    }
    */
}
