using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateSingle : MonoBehaviour
{
    #region External Data
    public bool Activated { get; set; }
    [SerializeField] private Color _player1Colour;
    [SerializeField] private Color _player2Colour;
    //[SerializeField] public bool _persistVisualEffect = false;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private PressurePlateSystem _ppSystem;
    #endregion

    #region Internal Data
    private bool _isPlayer1OnPlate = false;
    private bool _isPlayer2OnPlate = false;
    private GameObject _lastPlayer;
    ParticleSystem.MainModule _mainPS;
    #endregion

    private void Awake()
    {
        // Set Components
        _mainPS = _particleSystem.main;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1"))
        {
            _isPlayer1OnPlate = true;
            _lastPlayer = other.gameObject;
            UpdateActivationStatus();
        }
        else if (other.CompareTag("Player2"))
        {
            _isPlayer2OnPlate = true;
            _lastPlayer = other.gameObject;
            UpdateActivationStatus();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player1"))
        {
            _isPlayer1OnPlate = false;
            UpdateActivationStatus();
        }
        else if (other.CompareTag("Player2"))
        {
            _isPlayer2OnPlate = false;
            UpdateActivationStatus();
        }
    }

    private void UpdateActivationStatus()
    {
        // Activate if either player is on the plate, deactivate only if both are off
        if (_isPlayer1OnPlate || _isPlayer2OnPlate)
        {
            Activated = true;
        }
        else
        {
            Activated = false;
        }

        // Notify the system of the updated activation status
        _ppSystem.PlateActivated(this, Activated);
    }

    public void InitPlateSystem(PressurePlateSystem system)
    {
        _ppSystem = system;
    }

    // NOT part of the overall system's VFX
    public void ActivateIndividualVFX()
    {
        DeactivateIndividualVFX();
        _particleSystem.Play();
    }

    public void DeactivateIndividualVFX()
    {
        _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void SetVFXPlayerColour()
    {
        if (_lastPlayer.CompareTag("Player1"))
        {
            _mainPS.startColor = _player1Colour;
        }
        else if (_lastPlayer.CompareTag("Player2"))
        {
            _mainPS.startColor = _player2Colour;
        }
    }

    //public void ActivateColour(int colorMode)
    //{
    //    ParticleSystem.MainModule PSMain = _pressurePlatePS.main;
    //    if (colorMode == 0)
    //    {
    //        PSMain.startColor = Color.white;
    //        // Lock the color to white permanently
    //        _permaActivated = true;
    //    }
    //    else
    //    {
    //        PSMain.startColor = colorMode == 1 ? _player1Colour : _player2Colour;
    //    }

    //    // Restart particle system
    //    //_pressurePlatePS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    //    //_pressurePlatePS.Play();
    //}

    //public void ResetVisuals()
    //{
    //    // Easy way to reset visuals (Pressure plate system D)
    //    _pressurePlatePS.Stop();
    //    _permaActivated = false; 
    //}

    //public int ReturnPlayerColour()
    //{
    //    // This will return the player's colour that steped on the pressure plate
    //    if (_lastPlayer != null)
    //    {
    //        return _lastPlayer.tag == "Player1" ? 1 : _lastPlayer.tag == "Player2" ? 2 : 0;
    //    }
    //    return 0;
    //}


}
