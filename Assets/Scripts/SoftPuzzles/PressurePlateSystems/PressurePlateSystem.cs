using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PressurePlateSystem : MonoBehaviour
{
    // Internal Data
    protected List<PressurePlateSingle> PressurePlates; // Pressure plates that are part of the system

    private void Awake()
    {
        InitAllPressurePlates();
    }

    protected virtual void InitAllPressurePlates()
    {
        PressurePlates = GetComponentsInChildren<PressurePlateSingle>().ToList(); // Initialise all pressure plate children

        foreach (PressurePlateSingle plate in PressurePlates)
        {
            plate.InitPlateSystem(this);
        }
    }

    public abstract void PlateActivated(PressurePlateSingle plate, bool activated);
}
