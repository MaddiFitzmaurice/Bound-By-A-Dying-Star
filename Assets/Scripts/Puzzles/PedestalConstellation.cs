using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class PedestalConstellation : MonoBehaviour, IInteractable
{
    #region EXTERNAL DATA
    // List? of valid interactables that can be placed on the pedestal
    [SerializeField] private List<GameObject> _validInteractables;
    [SerializeField] private GameObject _presetPlacedObject = null;

    // Moving the mirror once it locks into the pedestal
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _raiseMirrorHeight;
    //[SerializeField] private Vector3 _targetAngle;
    [SerializeField] private float _initialAngleOffset;

    // move the portal to be in the centre of mirror
    [SerializeField] private float _raisePortalHeight;

    // The pedestal that forms a pair with this one
    [SerializeField] private List<GameObject> _beamDestinations;

    //Effects
    [SerializeField] private GameObject _lightBeam;
    [SerializeField] private float _raiseLightBeam;
    [SerializeField] private Transform _beamSource;
    [SerializeField] private ParticleSystem _beamTurningPS;

    #endregion

    #region INTERNAL DATA
    private List<PedestalConstellation> _pedestalDestinations;
    private List<float> _beamMaxLength = new List<float>();
    // Components
    private Renderer _diskRenderer;
    private ConstellationController _conController;

    private List<LineRenderer> _beamRenderer = new List<LineRenderer>();
    
    // Mirror
    private Level1Mirror _mirror = null;
    private Vector3 _targetDir;
    private bool _isRotating = false;
    private bool _correctAngle = false;
    Quaternion _startRot;
    #endregion

    void Awake()
    {
        // Get the Renderer component from the disk
        _diskRenderer = GetComponentInChildren<Renderer>();
        _conController = GetComponentInParent<ConstellationController>();  
        _pedestalDestinations = new List<PedestalConstellation>();
        foreach (GameObject dest in _beamDestinations)
        {
            _pedestalDestinations.Add(dest.GetComponent<PedestalConstellation>());
            _beamMaxLength.Add(0f);
        }

        // Set beam source to be the mirror's position
        _beamSource.position = new Vector3(transform.position.x, transform.position.y + _raiseLightBeam +_raiseMirrorHeight, transform.position.z);
        _beamTurningPS.Stop();
    }

        // On disabling of the attached GameObject
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    // Check if Pedestal has a preset object placed on it
    // if so, place it on the pedestal
    void Start()
    {
        // If the bedestal shoots 1 beam or more than 1 beam
        if(_beamDestinations.Count == 1)
        {
            // Set beam length to be distance between ource and destination
            _beamMaxLength[0] = Vector3.Distance(transform.InverseTransformPoint(_beamDestinations[0].transform.position), Vector3.zero);
            
            Vector3 offsetTarget = _beamDestinations[0].transform.position;
            offsetTarget = new Vector3(offsetTarget.x, offsetTarget.y + _raiseMirrorHeight, offsetTarget.z);
            
            // Set target direction to be the 1 target
            _targetDir = offsetTarget - _beamSource.position;
            _beamSource.rotation = Quaternion.LookRotation(_targetDir);
            
            // Then add initial offset to initial direction
            _startRot = _beamSource.rotation * Quaternion.Euler(0f, _initialAngleOffset, 0f);
            Vector3 startRotEuler = _startRot.eulerAngles;
            _startRot = Quaternion.Euler(0f, startRotEuler.y, 0f);
            _beamSource.rotation = _startRot;
        }
        else if(_beamDestinations.Count > 1)
        {
            // Iterate over each beam destination and calculate the average
            Vector3 meanVector = Vector3.zero;
            for (int i = 0; i < _beamDestinations.Count; i++)
            {
                Vector3 pos = _beamDestinations[i].transform.position;
                pos = new Vector3(pos.x, pos.y + _raiseMirrorHeight, pos.z);
                meanVector += pos;
                
                // Set beam length to be distance between ource and destination
                _beamMaxLength[i] = Vector3.Distance(transform.InverseTransformPoint(_beamDestinations[i].transform.position), Vector3.zero);
            }

            meanVector = meanVector / _beamDestinations.Count;

            // Then turn the average into a direction and set that to be the target
            _targetDir = meanVector -_beamSource.position;
            _beamSource.rotation = Quaternion.LookRotation(_targetDir);

            // Then add initial offset to initial direction
            _startRot = _beamSource.rotation * Quaternion.Euler(0f, _initialAngleOffset, 0f);
            Vector3 startRotEuler = _startRot.eulerAngles;
            _startRot = Quaternion.Euler(0f, startRotEuler.y, 0f);
            _beamSource.rotation = _startRot;
        }
        else 
        {
            Debug.LogError("_pairedPedestals must not be NULL!");
        }

        if (!TryGetComponent<PedestalConstellation>(out PedestalConstellation hinge))
        {
            Debug.LogError(_beamDestinations + " was not pedestal!!");
        }

        if (_initialAngleOffset == 0f)
        {
            _correctAngle = true;
            _conController.BeamRightDirection(this);
        }

        // Place mirror that is already set to be on the mirror
        if (_presetPlacedObject != null)
        {
            PlacePresetObject();
        }
    }

    void FixedUpdate()
    {
        if (_isRotating)
        {
            SetBeamPositions();
        }
    }

    private void PlacePresetObject()
    {
        if (_validInteractables.Contains(_presetPlacedObject))
        {
            // Change the disk's color to green
            _diskRenderer.material.color = Color.green;

            // IPickupable manipulation
            IPickupable pickupableType = _presetPlacedObject.GetComponent<IPickupable>();
            if (pickupableType != null)
            {
                pickupableType.PickupLocked(true);

                // If a mirror is to be placed on a pedestal
                if (pickupableType is Level1Mirror)
                {
                    _mirror = (Level1Mirror)pickupableType;
                    StartCoroutine(RotateMirror(_mirror.transform));
                    PedestalLinkData pedestalLinkData = new PedestalLinkData(_pedestalDestinations, this);
                    _conController.PedestalPreset(pedestalLinkData);
                }
                SetBeamPositions();
            }
            else
            {
                Debug.LogError("WARNING _presetPlacedObject did not have IPickupable");
            }
        }
        else
        {
            Debug.LogError("WARNING _presetPlacedObject was not set as valid interactable");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // If player has entered the trigger
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            PlayerBase player = other.GetComponent<PlayerBase>();

            // Ensure player has entered the trigger
            if (player != null)
            {
                // Ensure player is holding something and it's the correct interactable to be placed on pedestal
                if (player.CarriedPickupable != null && _validInteractables.Contains(player.CarriedPickupable))
                {
                    // Change the disk's color to green
                    _diskRenderer.material.color = Color.green;

                    // IPickupable manipulation
                    GameObject carriedPickupable = player.CarriedPickupable;
                    IPickupable pickupableType = carriedPickupable.GetComponent<IPickupable>();
                    pickupableType.PickupLocked(true);
                    pickupableType.BeDropped(transform);

                    // If a mirror is to be placed on a pedestal
                    if (pickupableType is Level1Mirror)
                    {
                        _mirror = (Level1Mirror)pickupableType;
                        StartCoroutine(RotateMirror(_mirror.transform));
                    }
                }
            }
        }
    }

    // Set start and end point of the beam in local space
    private void SetBeamPositions()
    {
        int beamCount = _beamRenderer.Count;
        
        if (beamCount == _beamDestinations.Count)
        {
            Vector3 localSource = transform.InverseTransformPoint(_beamSource.position);
            
            if (beamCount == 2)
            {
                for (int i = 0; i < beamCount; i++)
                {
                    // Get angle diff
                    Vector3 targetDir = _beamDestinations[i].transform.position - _beamSource.position;
                    float targetDiff = Vector3.Angle(_targetDir, targetDir);
                    if (i == 0)
                    {
                        targetDiff = 360 - targetDiff;
                    }

                    // Then add diff offset to direction
                    Quaternion startRot = Quaternion.Euler(0f, targetDiff, 0f);
                    Vector3 startRotEuler = startRot.eulerAngles;
                    startRot = Quaternion.Euler(0f, startRotEuler.y, 0f);

                    _beamRenderer[i].SetPosition(0, localSource);
                    _beamRenderer[i].SetPosition(1, localSource + ((startRot * _beamSource.forward) * _beamMaxLength[i]));
                }
            }
            else
            {
                _beamRenderer[0].SetPosition(0, localSource);
                _beamRenderer[0].SetPosition(1, localSource + (_beamSource.forward * _beamMaxLength[0]));
            }
        }
        else
        {
            Debug.LogError("WARNING NOT _beamRenderer.Count == _beamDestinations.Count");
        }
    }

    private IEnumerator RotateMirror(Transform mirror)
    {
        // Set the mirror's position and rotation to match the pedestal before starting the rotation
        mirror.position = new Vector3(transform.position.x, transform.position.y + _raiseMirrorHeight, transform.position.z);
        mirror.rotation = transform.rotation;

        Vector3 targetPosition = _beamDestinations[0].transform.position;

        // Remove the y component from the target position and the mirror's position to avoid tilting
        Vector3 targetDirection = (new Vector3(targetPosition.x, mirror.position.y, targetPosition.z) - mirror.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        while (Quaternion.Angle(mirror.rotation, targetRotation) > 0.01f)
        {
            mirror.rotation = Quaternion.Lerp(mirror.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
            yield return null;
        }

        // Ensure the mirror snaps to the exact rotation
        mirror.rotation = targetRotation; 
        _conController.PedestalHasMirror(this);
    }

    public void ActivateEffect()
    {
        foreach (var item in _beamDestinations)
        {
            GameObject newLightbeam = Instantiate(_lightBeam, transform);
            _beamRenderer.Add(newLightbeam.GetComponentInChildren<LineRenderer>());
        }
        SetBeamPositions();
    }

    // Patrolling rotate
    private IEnumerator RotateBeam()
    {
        _isRotating = true;
        _beamTurningPS.Play();
        while (_isRotating)
        {
            // Calculate new angle to rotate to
            Quaternion endRot = _beamSource.rotation * Quaternion.Euler(0f, 5f * -1, 0f);
            Vector3 endRotEuler = endRot.eulerAngles;
            endRot = Quaternion.Euler(0f, endRotEuler.y, 0f);

            // Keep rotating until enemy has reached new angle
            while (Mathf.Abs(Quaternion.Angle(endRot, _beamSource.rotation)) > 0.05f || !_isRotating)
            {
                _beamSource.rotation = Quaternion.RotateTowards(_beamSource.rotation, endRot, _rotationSpeed * Time.deltaTime);
                yield return null;
            }

            _beamSource.rotation = endRot;
            _isRotating = !CheckAngles();
        }
        _beamTurningPS.Stop();
    }

    private bool CheckAngles()
    {
        float dot = Vector3.Dot(Vector3.Normalize(_beamSource.forward), Vector3.Normalize(_targetDir));
        // Debug.Log("" + dot);
        if(dot > 0.985f) 
        { 
            _correctAngle = true;
            _conController.BeamRightDirection(this);
            _conController.PedestalHasBeam(_pedestalDestinations);
            return true;
        }
        else
        {
            return false;
        }
    }

    public List<PedestalConstellation> ReturnDestinations()
    {
        return _pedestalDestinations;
    }

    #region IINTERACTABLE FUNCTIONS
    public void PlayerInRange(PlayerBase player)
    {
    }

    public void PlayerNotInRange(PlayerBase player)
    {
    }

    public void PlayerStartInteract(PlayerBase player)
    {
        
    }

    public void PlayerHoldInteract(PlayerBase player)
    {
        //throw new System.NotImplementedException();
        // Debug.Log("hold");
        if (player.CarriedPickupable == null)
        {
            if (_isRotating == false && _beamRenderer.Count != 0 && !_correctAngle)
            {
                StartCoroutine(RotateBeam());
            }
        }
    }

    public void PlayerStopInteract(PlayerBase player)
    {
        if (_isRotating)
        {
            Debug.Log("styop");

            StopCoroutine(RotateBeam());
            _isRotating = false;
            CheckAngles();
            _beamTurningPS.Stop();
        }
    }
    #endregion
}
