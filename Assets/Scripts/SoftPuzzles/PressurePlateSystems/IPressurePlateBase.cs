using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPressurePlateBase
{
    public bool Activated { get; set; }

    public void InitPlateSystem(PressurePlateSystem system);
    public void ActivateColour(int colorMode);
}
