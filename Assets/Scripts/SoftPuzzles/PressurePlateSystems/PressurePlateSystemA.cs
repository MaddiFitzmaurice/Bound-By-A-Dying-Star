using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Pressure Plate System A: Lock/Show and Unlock/Hide an Object
public class PressurePlateSystemA : PressurePlateSystem
{
    #region EXTERNAL DATA
    [SerializeField] private GameObject _affectedObj; // Object to lock/unlock
    [SerializeField] private Material _ghostEffectMat;
    [SerializeField] private bool _onIsUnlock = true; // Does Object unlock or lock when pressure plate is stepped on?
    #endregion

    #region INTERNAL DATA
    private List<Renderer> _affectedObjRenderers;
    private List<Collider> _affectedObjColliders;
    private List<Material> _normalMats;
    #endregion
    private void Awake()
    {
        // Init pressure plates
        InitAllPressurePlates();

        // Init components
        _affectedObjColliders = _affectedObj.GetComponents<Collider>().ToList();
        _affectedObjRenderers = _affectedObj.GetComponentsInChildren<Renderer>().ToList();

        // Record default materials of each affected renderer
        _normalMats = new List<Material>();
        for (int i = 0;  i < _affectedObjRenderers.Count; i++)
        {
            _normalMats.Add(_affectedObjRenderers[i].material);
        }

        if (_normalMats.Count != _affectedObjRenderers.Count)
        {
            Debug.LogError("Num of materials does not match number of renderers!");
        }
    }

    public override void PlateActivated(PressurePlateSingle plate, bool activated)
    {
        // If plate is activated, play VFX
        if (activated)
        {
            //plate.SetVFXPlayerColour();
            plate.ActivateIndividualVFX();
        }

        // If plate is deactivated
        if (!activated) 
        {
            plate.DeactivateIndividualVFX();

            // If one plate is deactivated, check if other plates are still activated
            foreach (PressurePlateSingle pressurePlate in PressurePlates)
            {
                if (pressurePlate != plate && pressurePlate.Activated)
                {
                    return; // Do not do anything to the affected object if one plate is still active
                }
            }
        }

        // Decide what the object should do when a plate is triggered
        bool active = _onIsUnlock ? !activated : activated;

        if (active)
        {
            for (int i = 0; i < _affectedObjRenderers.Count; i++)
            {
                _affectedObjRenderers[i].material = _normalMats[i];
            }
        }
        else
        {
            for (int i = 0; i < _affectedObjRenderers.Count; i++)
            {
                _affectedObjRenderers[i].material = _ghostEffectMat;
            }
        }

        foreach (Collider collider in _affectedObjColliders)
        {
            collider.enabled = active;
        }
    }
}
