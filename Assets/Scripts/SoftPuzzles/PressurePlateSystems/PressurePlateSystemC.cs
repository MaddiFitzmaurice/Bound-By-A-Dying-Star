using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateSystemC : PressurePlateSystem
{
    #region External Data
    [SerializeField] private GameObject _platform; // Platform to move/stop
    [SerializeField] private bool _onIsMove; // Does Object move or stop when pressure plate is stepped on?
    [SerializeField] private Transform _platformStartPos;
    [SerializeField] private Transform _platformEndPos;
    [SerializeField] private float _platformSpeed;
    [SerializeField] private float _slowdownDistance; // Distance to start slowing down
    [SerializeField] private float _slowdownFactor; // How much to slow down (e.g., 0.5f = half speed)
    #endregion

    #region Internal Data
    private bool _isMoving;
    private bool _movingToEnd;
    private Coroutine _currentCoroutine;
    private HashSet<PressurePlateSingle> _activePlates = new HashSet<PressurePlateSingle>();
    #endregion

    private void OnEnable()
    {
        // Decide what the object should do when the system has been completed
        if (!_onIsMove)
        {
            _isMoving = true;
            StartCoroutine(UpdateLoop());
        }
    }

    public override void PlateActivated(PressurePlateSingle plate, bool activated)
    {
        if (activated)
        {
            //plate.SetVFXPlayerColour();
            _activePlates.Add(plate);
            plate.ActivateIndividualVFX();
        }
        else
        {
            _activePlates.Remove(plate);
            plate.DeactivateIndividualVFX();
        }

        // Decide what the object should do when the system has been completed
        //_isMoving = _onIsMove ? activated : !activated;

        // Update moving state based on active plates
        _isMoving = _activePlates.Count > 0;

        // Checks to see if Coroutine is already happening to ensure no duplicate coroutines
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);  
        }

        if (_isMoving)
        {
            _currentCoroutine = StartCoroutine(UpdateLoop());
        }
    }

    IEnumerator UpdateLoop() 
    {
        while (_isMoving)
        {
            MovePlatform();
            yield return null;
        }
    }

    void MovePlatform()
    {
        Vector3 targetPosition = _movingToEnd ? _platformEndPos.position : _platformStartPos.position;

        // Calculate the distance to both the start and end positions
        float distanceToStart = Vector3.Distance(_platform.transform.position, _platformStartPos.position);
        float distanceToEnd = Vector3.Distance(_platform.transform.position, _platformEndPos.position);

        // Find the smaller distance between the platform and its closest start or end point
        float closestDistance = Mathf.Min(distanceToStart, distanceToEnd);

        // Adjust speed based on proximity to either the start or end position
        float currentSpeed = _platformSpeed;

        // If within the slowdown distance, adjust speed accordingly
        if (closestDistance <= _slowdownDistance)
        {
            // Gradually slow down as it approaches either the start or end position
            currentSpeed *= Mathf.Lerp(_slowdownFactor, 1f, closestDistance / _slowdownDistance);
        }

        // Move the platform toward the target position at the adjusted speed
        _platform.transform.position = Vector3.MoveTowards(_platform.transform.position, targetPosition, currentSpeed * Time.deltaTime);

        // If the platform has reached the target, reverse the direction
        if (_platform.transform.position == targetPosition)
        {
            _movingToEnd = !_movingToEnd;
        }
    }
}
