using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateSystemD : PressurePlateSystem
{
    #region EXTERNAL DATA
    [SerializeField] private GameObject _objectToUnlock; 
    [SerializeField] private float _activationTime = 10.0f;
    [SerializeField] private List<GameObject> _pressurePlateObjects;
    #endregion

    #region INTERNAL DATA
    private bool _timerActive = false;
    private float _timer;
    private List<PPVFXTimer> _ppVFXs;
    #endregion

    protected override void InitAllPressurePlates()
    {
        // Initialize the list of pressure plates from the manually assigned list in inspector
        PressurePlates = new List<PressurePlateSingle>();
        // Init list of VFX timers
        _ppVFXs = new List<PPVFXTimer>();

        foreach (GameObject plateObject in _pressurePlateObjects)
        {
            var plate = plateObject.GetComponent<PressurePlateSingle>();
            if (plate != null)
            {
                PressurePlates.Add(plate);
                plate.InitPlateSystem(this);
            }

            PPVFXTimer vfx = plateObject.GetComponentInChildren<PPVFXTimer>();
            if (vfx != null)
            {
                _ppVFXs.Add(vfx);
                vfx.SetDrainTime(_activationTime);
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

    // When all pressure plates have successfully been activated
    private void FinalizeActivation()
    {
        // Stop timer VFX and set it to complete state
        foreach (var vfx in _ppVFXs)
        {
            vfx.StopVFXDrainer();
            vfx.FinaliseActivation();
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
        }

        foreach (var vfx in _ppVFXs)
        {
            vfx.StopVFXDrainer();
        }
    }

    // If a plate from the system is activated
    public override void PlateActivated(PressurePlateSingle plate, bool activated)
    {
        if (!_timerActive)
        {
            _timer = _activationTime;
            _timerActive = true;
            ActivateTimerVFXs();
        }

        plate.Activated = true;
        plate.SetVFXPlayerColour();

        // Immediately check if this activation fulfilled all conditions
        CheckAllPlatesActivated();
    }

    private void ActivateTimerVFXs()
    {
        foreach (PPVFXTimer timer in _ppVFXs)
        {
            timer.StartVFXDrainer();
        }
    }
}
