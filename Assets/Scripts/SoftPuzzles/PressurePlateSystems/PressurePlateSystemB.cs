using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PressurePlateSystemB : PressurePlateSystem
{
    // External Data
    [SerializeField] private GameObject _affectedObj; // Object to lock/unlock
    [SerializeField] private bool _onIsUnlock = true; // Does Object unlock or lock when system is completed?

    private void Start()
    {
        if (!_onIsUnlock)
        {
            _affectedObj.SetActive(false);
        }    
    }

    protected override void InitAllPressurePlates()
    {
        var pairs = GetComponentsInChildren<PressurePlatePair>().ToList(); // Initialise all pressure plate children
        PressurePlates = new List<IPressurePlateBase>();

        foreach (PressurePlatePair pair in pairs)
        {
            PressurePlates.Add(pair);
            pair.InitPlateSystem(this);
        }
    }

    // Each pair that is activated
    public override void PlateActivated(IPressurePlateBase plate, bool activated)
    {
        foreach (IPressurePlateBase pair in PressurePlates)
        {
            if (!pair.Activated)
            {
                return; // If any pair is not successful, immediately exit the method
            }
        }

        // Decide what the object should do when the system has been completed
        bool active = _onIsUnlock ? !activated : activated;

        _affectedObj.SetActive(active);
    }
}
