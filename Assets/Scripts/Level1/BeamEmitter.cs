using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamEmitter : MonoBehaviour
{
    [SerializeField] private LineRenderer _beamRenderer;
    [SerializeField] private Transform _beamSource;
    [SerializeField] private Transform _beamDestination;
    [SerializeField] private bool _startEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set start and end point of the beam in local space
        _beamRenderer.SetPosition(0, transform.InverseTransformPoint(_beamSource.position));
        _beamRenderer.SetPosition(1, transform.InverseTransformPoint(_beamDestination.position));
        if (_startEnabled)
        {
            _beamRenderer.enabled = true;
        }
        else
        {
            _beamRenderer.enabled = false;
        }
    }

    public void SetBeamStatus(bool status)
    {
        if (status)
        {
            _beamRenderer.enabled = true;
        }
        else
        {
            _beamRenderer.enabled = false;
        }
    }
}