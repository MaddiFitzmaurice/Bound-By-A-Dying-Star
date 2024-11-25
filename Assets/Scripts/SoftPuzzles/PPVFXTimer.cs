using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class PPVFXTimer : MonoBehaviour
{
    #region EXTERNAL DATA
    [SerializeField] private Color _waitingColour;
    [SerializeField] private Color _pressedColour;
    [SerializeField] private Color _finishedColour;
    #endregion
    
    #region INTERNAL DATA
    private float _drainTime;
    private ParticleSystem _system;
    ParticleSystem.ShapeModule _shape;
    ParticleSystem.MainModule _main;
    ParticleSystem.EmissionModule _emission;
    private float _timer = 0f;
    #endregion

    private void Awake()
    {
        _system = GetComponentInChildren<ParticleSystem>();
        _shape = _system.shape;
        _main = _system.main;
        _emission = _system.emission;
        _main.startColor = _waitingColour;
        _system.gameObject.SetActive(false);
    }

    public void FinaliseActivation()
    {
        ResetAnim();
        SetColour(_finishedColour);
        _system.gameObject.SetActive(true);
    }

    public void PlatePressed()
    {
        SetColour(_pressedColour);
    }

    public void SetColour(Color colour)
    {
        _main.startColor = colour;
    }

    public void SetDrainTime(float drainTime)
    {
        _drainTime = drainTime;
    }

    public void StartVFXDrainer()
    {
        StartCoroutine(VFXDrainer());
    }

    public void StopVFXDrainer()
    {
        StopAllCoroutines();
        _system.Stop();
        ResetAnim();
        _system.gameObject.SetActive(false);
    }

    private IEnumerator VFXDrainer()
    {
        _system.gameObject.SetActive(true);
        _timer = 0f;

        while (_timer < _drainTime)
        {
            float num = Mathf.Lerp(360f, 1f, _timer / _drainTime);
            _shape.arc = num;
            _emission.rateOverTimeMultiplier = (num * 10f);
            _timer += Time.deltaTime;
            yield return null;
        }

        _system.gameObject.SetActive(false);
        ResetAnim();
    }

    private void ResetAnim()
    {
        _shape.arc = 360f;
        _emission.rateOverTimeMultiplier = 3600f;
        SetColour(_waitingColour);
    }
}
