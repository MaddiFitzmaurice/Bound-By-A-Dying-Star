using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PressurePlateSystem : MonoBehaviour
{
    // Internal Data
    protected List<IPressurePlateBase> PressurePlates; // Pressure plates that are part of the system

    private void Awake()
    {
        InitAllPressurePlates();
    }

    protected void InitAllPressurePlates()
    {
        PressurePlates = GetComponentsInChildren<IPressurePlateBase>().ToList(); // Initialise all pressure plate children

        foreach (IPressurePlateBase plate in PressurePlates)
        {
            plate.InitPlateSystem(this);
        }
    }

    public abstract void PlateActivated(IPressurePlateBase plate, bool activated);
}
