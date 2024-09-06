using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class PPVFXTimer : MonoBehaviour
{
    #region INTERNAL DATA
    private float _drainTime;
    private ParticleSystem _system;
    ParticleSystem.ShapeModule _shape;
    private float _timer = 0f;
    #endregion

    private void Awake()
    {
        _system = GetComponentInChildren<ParticleSystem>();
        _shape = _system.shape;
        _system.gameObject.SetActive(false);
    }

    public void SetDrainTime(float drainTime)
    {
        _drainTime = drainTime;
    }

    public void StartVFXDrainer()
    {
        StartCoroutine(VFXDrainer());
    }

    private IEnumerator VFXDrainer()
    {
        _system.gameObject.SetActive(true);
        _timer = 0f;

        while (_timer < _drainTime)
        {
            _shape.arc = Mathf.Lerp(360f, 1f, _timer / _drainTime);
            _timer += Time.deltaTime;
            yield return null;
        }

        _system.gameObject.SetActive(false);
        _shape.arc = 360f;
    }

    //private void Update()
    //{
    //    if (_shape.arc > 1)
    //    {
    //        _shape.arc -= (360f / _drainTime) * Time.deltaTime;
    //    }
    //    else
    //    {
    //        _system.gameObject.SetActive(false);
    //    }
    //}
}
