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
    #endregion

    private void Start()
    {
        // Decide what the object should do when the system has been completed
        if (!_onIsMove)
        {
            _isMoving = true;
            StartCoroutine(UpdateLoop());
        }
    }

    public override void PlateActivated(IPressurePlateBase plate, bool activated)
    {
        // Decide what the object should do when the system has been completed
        _isMoving = _onIsMove ? activated : !activated;

        if (_isMoving)
        {
            StartCoroutine(UpdateLoop());
        }
        else
        {
            StopAllCoroutines();
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
        if (_movingToEnd)
        {
            _platform.transform.position = Vector3.MoveTowards(_platform.transform.position, _platformEndPos.position, _platformSpeed * Time.deltaTime);
            if (_platform.transform.position == _platformEndPos.position)
                _movingToEnd = false;
        }
        else
        {
            _platform.transform.position = Vector3.MoveTowards(_platform.transform.position, _platformStartPos.position, _platformSpeed * Time.deltaTime);
            if (_platform.transform.position == _platformStartPos.position)
                _movingToEnd = true;
        }
    }
}
