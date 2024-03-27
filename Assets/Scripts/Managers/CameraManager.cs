using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    [SerializeField] private List<Transform> _playerObjects;
    private Camera _camera;

    // Camera Settings
    private Vector3 _offset;
    private Vector3 _velocity;
    [SerializeField] private float _smoothTime = 0.25f;
    [SerializeField] private float _zoomOutMax = 50f;
    [SerializeField] private float _zoomInMax = 20f;
    [SerializeField] private float _zoomDefaultDistance = 20f;


    void Start()
    {
        _offset = transform.position;
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var bounds = new Bounds(_playerObjects[0].position, Vector3.zero);
        bounds.Encapsulate(_playerObjects[1].position);

        MoveCamera(bounds.center);
        ZoomCamera(bounds.size.x);
    }

    void MoveCamera(Vector3 playersCentre)
    {
        Vector3 newPosition = playersCentre + _offset;

        newPosition = new Vector3(newPosition.x, newPosition.y, _offset.z);

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref _velocity, _smoothTime);
    }

    void ZoomCamera(float playerDistance)
    {
        // Note: Change bounds.size.x to distance between the two objects
        float newZoom = Mathf.Lerp(_zoomInMax, _zoomOutMax, playerDistance / _zoomDefaultDistance);
        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, newZoom, Time.deltaTime);
    }
}