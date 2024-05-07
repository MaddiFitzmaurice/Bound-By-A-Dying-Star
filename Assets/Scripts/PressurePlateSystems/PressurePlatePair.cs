using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlatePair : PressurePlateSystem, IPressurePlateBase
{
    #region ExternalData
    [SerializeField] private float _timeToPressPair; // Time leeway for both pressure plates to be pressed at same time
    #endregion

    #region Internal Data
    private PressurePlateSystem _ppSystem;
    public bool Activated { get; set; }
    #endregion

    private void Awake()
    {
        InitAllPressurePlates();

        if (PressurePlates.Count != 2)
        {
            Debug.LogError("2 pressure plates can only be childed to this pressure plate system");
        }
    }

    IEnumerator CheckPairedPlate()
    {
        yield return new WaitForSeconds(_timeToPressPair);

        // If both plates are activated
        if (PressurePlates[0].Activated && PressurePlates[1].Activated)
        {
            Activated = true;
        }
        else
        {
            PressurePlates[0].Activated = false;
            PressurePlates[1].Activated = false;
        }
    }

    #region PressurePlateSystem Base
    public override void PlateActivated(IPressurePlateBase plate, bool activated)
    {
        if (!Activated)
        {
            if (activated)
            {
                StartCoroutine(CheckPairedPlate());
            }
            else
            {
                StopAllCoroutines();
            }
        }
    }
    #endregion

    #region IPressurePlate Interface
    public void InitPlateSystem(PressurePlateSystem system)
    {
        _ppSystem = system;
    }

    public void ActivateEffect()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
