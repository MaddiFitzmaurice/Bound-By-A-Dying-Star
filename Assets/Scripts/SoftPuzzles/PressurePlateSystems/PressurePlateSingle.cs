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

    #endregion

    #region InternalData
    private PressurePlateSystem _ppSystem;
    private Renderer _renderer;
    private bool _permaActivated = false;
    #endregion

    private void Awake()
    {
        // get components
        _renderer = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_permaActivated)
        {
            if (other.CompareTag("Player1"))
            {
                Activated = true;
                _ppSystem.PlateActivated(this, Activated);
                ActivateColour(1);
                _pressurePlatePS.Play();
            }
            else if (other.CompareTag("Player2"))
            {
                Activated = true;
                _ppSystem.PlateActivated(this, Activated);
                ActivateColour(2);
                _pressurePlatePS.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_permaActivated)
        {
            if (other.CompareTag("Player1") || other.CompareTag("Player2"))
            {
                Activated = false;
                _ppSystem.PlateActivated(this, Activated);
                _pressurePlatePS.Stop(); //Warning! May cause issue if two players are on pressure plate and one walks off
            }
        }
    }

    #region IPressurePlate Interface
    public void InitPlateSystem(PressurePlateSystem system)
    {
        _ppSystem = system;
    }

    public void ActivateColour(int colorMode)
    {
        ParticleSystem.MainModule PSMain = _pressurePlatePS.main;
        switch (colorMode)
        {
            case 0:
                PSMain.startColor = Color.white;
                _permaActivated = true;
            break;
            case 1:
                PSMain.startColor = _player1Colour;
            break;
            case 2:
                PSMain.startColor = _player2Colour;
            break;
        }
    }
    #endregion
}
