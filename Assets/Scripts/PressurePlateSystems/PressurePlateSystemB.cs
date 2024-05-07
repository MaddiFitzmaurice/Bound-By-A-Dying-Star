using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateSystemB : PressurePlateSystem
{
    // External Data
    [SerializeField] private GameObject _affectedObj; // Object to lock/unlock
    [SerializeField] private bool _onIsUnlock = true; // Does Object unlock or lock when system is completed?

    public override void PlateActivated(IPressurePlateBase plate, bool activated)
    {
        // Only check if pair has been completed
        if (activated)
        {
            foreach (IPressurePlateBase pair in PressurePlates)
            {
                if (!pair.Activated)
                {
                    return; // If any pair is not successful, immediately exit the method (Josh)
                }
            }

            // Decide what the object should do when the system has been completed
            bool active = _onIsUnlock ? !activated : activated;

            _affectedObj.SetActive(active);
        }
    }
}
