using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateSingle : MonoBehaviour, IPressurePlateBase
{
    #region External Data
    public bool Activated { get; set; }
    #endregion

    #region InternalData
    private PressurePlateSystem _ppSystem;
    private Renderer _renderer;
    #endregion

    private void Awake()
    {
        // get components
        _renderer = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            Activated = true;
            _ppSystem.PlateActivated(this, Activated);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            Activated = false;
            _ppSystem.PlateActivated(this, Activated);
        }
    }

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
