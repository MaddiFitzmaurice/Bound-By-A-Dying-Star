using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Pressure Plate System A: Lock/Show and Unlock/Hide an Object
public class PressurePlateSystemA : PressurePlateSystem
{
    // External Data
    [SerializeField] private GameObject _affectedObj; // Object to lock/unlock
    [SerializeField] private bool _onIsUnlock = true; // Does Object unlock or lock when pressure plate is stepped on?
   
    private void Awake()
    {
        InitAllPressurePlates();
    }

    public override void PlateActivated(IPressurePlateBase plate, bool activated)
    {
        // If one plate is deactivated, check if other plates are still activated
        if (!activated) 
        {
            foreach (IPressurePlateBase pressurePlate in PressurePlates)
            {
                if (pressurePlate != plate && pressurePlate.Activated)
                {
                    return; // Do not do anything to the affected object if one plate is still active
                }
            }
        }

        // Decide what the object should do when a plate is triggered
        bool active = _onIsUnlock ? !activated : activated;

        _affectedObj.SetActive(active);
    }
}
