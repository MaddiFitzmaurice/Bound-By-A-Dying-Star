using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateSingle : MonoBehaviour, IPressurePlateBase
{
    #region External Data
    public bool Activated { get; set; }
    [SerializeField] private ParticleSystem _pressurePlatePS;
    [SerializeField] private Color _player1Colour;
    [SerializeField] private Color _player2Colour;
    [SerializeField] public bool persistVisualEffect = false;
    [SerializeField] private PressurePlateSystem _ppSystem;
    #endregion

    #region InternalData
    private bool _permaActivated = false; 
    private GameObject _lastPlayer;
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (!_permaActivated)
        {
            if (other.CompareTag("Player1"))
            {
                _lastPlayer = other.gameObject;
                Activated = true;
                ActivateColour(1);
                _ppSystem.PlateActivated(this, Activated);
            }
            else if (other.CompareTag("Player2"))
            {
                _lastPlayer = other.gameObject;
                Activated = true;
                ActivateColour(2);
                _ppSystem.PlateActivated(this, Activated);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_permaActivated && !persistVisualEffect)
        {
            if (other.CompareTag("Player1") || other.CompareTag("Player2"))
            {
                // Check if the exiting player is the one who activated the plate
                if (other.gameObject == _lastPlayer)
                {
                    Activated = false;
                    _ppSystem.PlateActivated(this, Activated);
                    _pressurePlatePS.Stop();
                }
            }
        }
    }

    public void ActivateColour(int colorMode)
    {
        ParticleSystem.MainModule PSMain = _pressurePlatePS.main;
        if (colorMode == 0)
        {
            PSMain.startColor = Color.white;
            // Lock the color to white permanently
            _permaActivated = true;
        }
        else
        {
            PSMain.startColor = colorMode == 1 ? _player1Colour : _player2Colour;
        }

        // Restart particle system
        _pressurePlatePS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _pressurePlatePS.Play();
    }

    public void ResetVisuals()
    {
        // Easy way to reset visuals (Pressure plate system D)
        _pressurePlatePS.Stop();
        _permaActivated = false; 
    }

    public int ReturnPlayerColour()
    {
        // This will return the player's colour that steped on the pressure plate
        if (_lastPlayer != null)
        {
            return _lastPlayer.tag == "Player1" ? 1 : _lastPlayer.tag == "Player2" ? 2 : 0;
        }
        return 0;
    }

    public void InitPlateSystem(PressurePlateSystem system)
    {
        _ppSystem = system;
    }
}
