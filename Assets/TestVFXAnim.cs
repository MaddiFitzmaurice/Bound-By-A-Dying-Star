using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVFXAnim : MonoBehaviour
{
    [SerializeField] float _drainTime;
    private ParticleSystem _system;
    ParticleSystem.ShapeModule _shape;

    private void Awake()
    {
        _system = GetComponentInChildren<ParticleSystem>();
        _shape = _system.shape;
    }

    private void Update()
    {
        if (_shape.arc > 1)
        {
            _shape.arc -= _drainTime * Time.deltaTime;
        }
        else
        {
            _system.gameObject.SetActive(false);
        }
    }
}
