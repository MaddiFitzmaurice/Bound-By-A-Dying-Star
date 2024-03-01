using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [field: Header("Assignment Code Stuff")]
    [field: SerializeField] private int _sentCode = 0;
    [field: SerializeField] private int _recievedCode = 0;
    [field: SerializeField] private bool _disabled = false;

    private bool _resetOnRecieved = true;


    [field: Header("References")]
    [field: SerializeField] private SpriteRenderer _fillSprite;
    [field: SerializeField] private ParticleSystem _particleSystem;
    
    private SpriteRenderer _borderSprite;
    private float _originalHue;

    private List<ParticleSystem.Particle> _particles = new List<ParticleSystem.Particle>();
    [field: SerializeField] private int _numEnter = 0;

    // Start is called before the first frame update
    void Awake()
    {
        _borderSprite = GetComponent<SpriteRenderer>();
        Color.RGBToHSV(_fillSprite.color, out var H, out var S, out var V);
        _originalHue = H * 360;
        if (_disabled)
        {
            DisablePlate();
        }
    }

    private void OnEnable()
    {
        if (_recievedCode != 0)
        {
            EventManager.EventSubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
        }
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.ASSIGNMENT_CODE_TRIGGER, AssignmentCodeHandler);
        StopAllCoroutines();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_sentCode != 0)
        {
            if (collision.tag == "Player")
            {
                ActivatePlate();
            }
        }
    }

    private void ActivatePlate()
    {
        if (!_disabled)
        {
            DisablePlate();
            EventManager.EventTrigger(EventType.ASSIGNMENT_CODE_TRIGGER, _sentCode);
            StartCoroutine(DisableForASec());
        }
    }

    public void OnParticleTrigger()
    {
        _numEnter = _particleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, _particles);
        _numEnter = _particles.Count;
    }

    private void DisablePlate()
    {
        _disabled = true;
        _borderSprite.color = Color.HSVToRGB(0f, 0f, 0.5f);
        _fillSprite.color = Color.HSVToRGB(0f, 0f, 0.5f);
    }

    private void EnablePlate()
    {
        _disabled = false;
        _borderSprite.color = Color.HSVToRGB(0f, 0f, 1f);
        _fillSprite.color = Color.HSVToRGB(_originalHue/360, 1f, 0.6f);
    }

    private void AssignmentCodeHandler(object data)
    {
        if (data == null)
        {
            Debug.Log("PressurePlate AssignmentCodeHandler is null");
        }
        if (_recievedCode == (int)data)
        {
            
            StartCoroutine(DelayAssignmentCheck());
        }
    }

    private IEnumerator DisableForASec()
    {
        _resetOnRecieved = false;
        yield return new WaitForSeconds(0.2f);
        _resetOnRecieved = true;
    }

    private IEnumerator DelayAssignmentCheck()
    {
        yield return new WaitForSeconds(0.1f);
        if (_resetOnRecieved)
            {
                if (_disabled)
                {
                    EnablePlate();
                }
                else
                {
                    DisablePlate();
                }
            }
    }
}
