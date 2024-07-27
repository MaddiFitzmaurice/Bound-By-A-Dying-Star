using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateSystemD : PressurePlateSystem
{
    [SerializeField] private GameObject _objectToUnlock; 
    [SerializeField] private float _activationTime = 10.0f;
    [SerializeField] private List<GameObject> _pressurePlateObjects;

    private bool _timerActive = false;
    private float _timer;

    protected override void InitAllPressurePlates()
    {
        // Initialize the list of pressure plates from the manually assigned list in inspector
        PressurePlates = new List<IPressurePlateBase>();
        foreach (GameObject plateObject in _pressurePlateObjects)
        {
            var plate = plateObject.GetComponent<PressurePlateSingle>();
            if (plate != null)
            {
                PressurePlates.Add(plate);
                plate.InitPlateSystem(this);
            }
        }
    }

    private void Update()
    {
        if (_timerActive)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                // Reset the timer flag when time runs out
                _timerActive = false;
                CheckAllPlatesActivated();
            }
        }
    }

    private void CheckAllPlatesActivated()
    {
        // Check to see if each pressure plate is activated
        bool allActivated = true;
        foreach (var plate in PressurePlates)
        {
            if (!plate.Activated)
            {
                allActivated = false;
                break;
            }
        }
        // If all activated, will set all pressure plates to be white permanently
        if (allActivated)
        {
            FinalizeActivation();
        }
        // If not all activated and timer has finished, it will reset all pressure plates
        else if (!_timerActive)
        {
            ResetAllPlates();
        }
    }

    private void FinalizeActivation()
    {
        // Sets each pressure plate to white
        foreach (var plate in PressurePlates)
        {
            plate.ActivateColour(0);
        }
        // Unlocks the final object, can be configured depending on scenario
        _objectToUnlock.SetActive(true);
    }

    private void ResetAllPlates()
    {
        // Resets all pressure plates in system
        foreach (PressurePlateSingle plate in PressurePlates)
        {
            plate.Activated = false;
            plate.ResetVisuals();
        }
    }

    public override void PlateActivated(IPressurePlateBase plate, bool activated)
    {
        if (!_timerActive)
        {
            _timer = _activationTime;
            _timerActive = true;
        }
        plate.Activated = true;

        // Immediately check if this activation fulfilled all conditions
        CheckAllPlatesActivated();
    }
}