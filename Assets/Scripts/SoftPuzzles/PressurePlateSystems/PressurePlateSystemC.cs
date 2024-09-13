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
    #endregion

    #region Internal Data
    private bool _isMoving;
    private bool _movingToEnd;
    private Coroutine _currentCoroutine;
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
            plate.SetVFXPlayerColour();
            plate.ActivateIndividualVFX();
        }
        else
        {
            plate.DeactivateIndividualVFX();
        }

        // Decide what the object should do when the system has been completed
        _isMoving = _onIsMove ? activated : !activated;

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
        _platform.transform.position = Vector3.MoveTowards(_platform.transform.position, targetPosition, _platformSpeed * Time.deltaTime);

        if (_platform.transform.position == targetPosition)
        {
            _movingToEnd = !_movingToEnd;
        }

    }
}
