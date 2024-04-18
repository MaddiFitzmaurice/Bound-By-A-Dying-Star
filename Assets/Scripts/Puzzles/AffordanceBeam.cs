using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AffordanceBeam : MonoBehaviour
{
    [SerializeField] private LineRenderer _beamRenderer;
    [SerializeField] private Transform _beamSource;
    [SerializeField] private Transform _beamDestination;

    // Start is called before the first frame update
    void Start()
    {
        // Set start and end point of the beam in local space
        _beamRenderer.SetPosition(0, transform.InverseTransformPoint(_beamSource.position));
        _beamRenderer.SetPosition(1, transform.InverseTransformPoint(_beamDestination.position));
        _beamRenderer.enabled = true;
    }
}