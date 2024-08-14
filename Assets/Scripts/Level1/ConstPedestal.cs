using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstPedestal : MonoBehaviour, IInteractable
{
    private int _id = 0;
    #region EXTERNAL DATA
    public bool InteractLocked { get; set; } = false;
    [SerializeField] private List<GameObject> _validInteractables;
    [SerializeField] private GameObject _presetPlacedObject = null;    
    [SerializeField] private float _raiseMirrorHeight = 2.5f;

    // Moving the Beam once it is activated
    [SerializeField] private float _rotationSpeed = 40f;
    [SerializeField] private float _initialAngleOffset;

    // The pedestals that forms a pair with this one
    [SerializeField] private List<GameObject> _beamDestinations;

    //Effects
    [SerializeField] private GameObject _lightBeam;
    [SerializeField] private GameObject _mirrorBeamFX;
    [SerializeField] private float _raiseLightBeam = 1f;
    [SerializeField] private Transform _beamSource;
    [SerializeField] private ParticleSystem _beamTurningPS;

    #endregion

    #region INTERNAL DATA
    private List<ConstPedestal> _pedestalDestinations;
    private List<float> _beamMaxLength = new List<float>();
    
    // Components
    private Renderer _diskRenderer;
    private PedestalConstController _conController;

    private List<LineRenderer> _beamRenderer = new List<LineRenderer>();
    
    // Mirror
    private Level1Mirror _mirror = null;
    private Vector3 _targetDir;
    private bool _isRotating = false;
    private bool _correctAngle = false;
    Quaternion _startRot;
    private bool _canRotateMirror = false;

    // Tutorial Prompt
    private static bool _showPrompt = true;
    #endregion

    void Awake()
    {
        // Get the Renderer component from the disk
        _diskRenderer = GetComponentInChildren<Renderer>();
        _conController = GetComponentInParent<PedestalConstController>();  
        _pedestalDestinations = new List<ConstPedestal>();
        foreach (GameObject dest in _beamDestinations)
        {
            _pedestalDestinations.Add(dest.GetComponent<ConstPedestal>());
            _beamMaxLength.Add(0f);
        }

        // Set beam source to be the mirror's position
        _beamSource.position = new Vector3(transform.position.x, transform.position.y + _raiseLightBeam + _raiseMirrorHeight, transform.position.z);
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
            offsetTarget = new Vector3(offsetTarget.x, _beamSource.position.y, offsetTarget.z);
            
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
                pos = new Vector3(pos.x, _beamSource.position.y, pos.z);
                meanVector += pos;
                
                // Set beam length to be distance between source and destination
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

        if (!TryGetComponent<ConstPedestal>(out ConstPedestal hinge))
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

    public void SetID(int id)
    {
        _id = id;
    }

    public PedestalLinkData GetPedestalLinkData()
    {
        PedestalLinkData pedestalLinkData = new PedestalLinkData(_pedestalDestinations, this);
        return pedestalLinkData;
    }

    private void LockPlacedMirror(GameObject mirror)
    {
        IInteractable interactable = mirror.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.InteractLocked = true;
        }
    }

    private void PlacePresetObject()
    {
        if (_validInteractables.Contains(_presetPlacedObject))
        {
            // Change the disk's color to green
            //_diskRenderer.material.color = Color.green;

            // Lock Interaction
            LockPlacedMirror(_presetPlacedObject);

            _presetPlacedObject.transform.SetParent(_beamSource, false);

            // IPickupable manipulation
            IPickupable pickupableType = _presetPlacedObject.GetComponent<IPickupable>();
            if (pickupableType != null)
            {
                pickupableType.PickupLocked(true);

                // If a mirror is to be placed on a pedestal
                if (pickupableType is Level1Mirror)
                {
                    _mirror = (Level1Mirror)pickupableType;
                    InitialRotateMirror(_mirror.transform);
                }
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

    // Set start and end point of the beam in local space
    private void SetBeamPositions()
    {
        int beamCount = _beamRenderer.Count;
        
        if (beamCount == _beamDestinations.Count)
        {
            Vector3 localSource = transform.InverseTransformPoint(_beamSource.position);
            
            // If there are two beams
            if (beamCount == 2)
            {
                // Iterate over both the two beams
                for (int i = 0; i < beamCount; i++)
                {
                    // Calculate destination in local space
                    Vector3 offsetTarget = transform.InverseTransformPoint(_beamDestinations[i].transform.position);
                    offsetTarget = new Vector3(offsetTarget.x, localSource.y, offsetTarget.z);
                    
                    // Set Beam start and endpoints
                    _beamRenderer[i].gameObject.transform.parent.gameObject.SetActive(true);
                    _beamRenderer[i].SetPosition(0, localSource);
                    _beamRenderer[i].SetPosition(1, offsetTarget);
                }
            }
            else // If there is just the one beam
            {
                // Calculate destination in local space
                Vector3 offsetTarget = transform.InverseTransformPoint(_beamDestinations[0].transform.position);

                offsetTarget = new Vector3(offsetTarget.x, localSource.y, offsetTarget.z);

                // Set Beam start and endpoints
                _beamRenderer[0].gameObject.transform.parent.gameObject.SetActive(true);
                _beamRenderer[0].SetPosition(0, localSource);
                _beamRenderer[0].SetPosition(1, offsetTarget);
            }
        }
        else
        {
            Debug.LogError("WARNING NOT _beamRenderer.Count != _beamDestinations.Count");
        }
    }

    // Rotate mirror to initial angle when placing on pedestal
    private void InitialRotateMirror(Transform mirror)
    {
        // Set the mirror's position and rotation to match the pedestal before starting the rotation
        mirror.position = new Vector3(transform.position.x, transform.position.y + _raiseMirrorHeight, transform.position.z);
        mirror.rotation = transform.rotation;

        // Get position of beam destination (eventually should get average pos when multiple)
        Vector3 targetPosition = _beamDestinations[0].transform.position;

        // Remove the y component from the target position and the mirror's position to avoid tilting
        Vector3 targetDirection = (new Vector3(targetPosition.x, mirror.position.y, targetPosition.z) - mirror.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // Combine target rotation with the initial offset
        targetRotation = targetRotation * (mirror.rotation * Quaternion.Euler(0f, _initialAngleOffset, 0f));

        // Ensure the mirror snaps to the exact rotation
        mirror.rotation = targetRotation; 
        _conController.PedestalHasMirror(this);
    }

    // Activate beam effects for each destination in list
    public void ActivateEffect()
    {
        foreach (var item in _beamDestinations)
        {
            GameObject newLightbeam = Instantiate(_lightBeam, transform);
            _beamRenderer.Add(newLightbeam.GetComponentInChildren<LineRenderer>());
            newLightbeam.SetActive(false);
        }
    }

    // Activate mirror orb effects
    public void ActivateOrb()
    {
        _mirrorBeamFX.SetActive(true);
        _canRotateMirror = true;
    }

    // Activate sky beam
    public void ActivateSkyBeam()
    {
        EventManager.EventTrigger(EventType.LVL1_STARBEAM_ACTIVATE, _id);
    }

    // Rotate beam to target direction anticlockwise
    private IEnumerator RotateMirror(Vector3 targetDir)
    {
        _isRotating = true;
        _beamTurningPS.Play();
        while (_isRotating)
        {
            Quaternion endRot = Quaternion.LookRotation(targetDir, Vector3.up);

            // if angle is 180 or over set angle to 90, otherwise leave it
            if (Quaternion.Angle(endRot, _beamSource.rotation) >= 180)
            {
                // Calculate angle 90 degrees anticlockwise
                endRot = _beamSource.rotation * Quaternion.Euler(0f, -90f, 0f);
                Vector3 endRotEuler = endRot.eulerAngles;
                endRot = Quaternion.Euler(0f, endRotEuler.y, 0f);
            }

            // Keep rotating until enemy has reached new angle or _isRotating is false
            while (Mathf.Abs(Quaternion.Angle(endRot, _beamSource.rotation)) > 0.05f && _isRotating)
            {
                _beamSource.rotation = Quaternion.RotateTowards(_beamSource.rotation, endRot, _rotationSpeed * Time.deltaTime);
                yield return null;
            }

            _beamSource.rotation = endRot;
            _isRotating = CheckAngles();
        }
        _beamTurningPS.Stop();
        yield return null;
    }

    // Check if beam if racing the right angle
    private bool CheckAngles()
    {
        float dot = Vector3.Dot(Vector3.Normalize(_beamSource.forward), Vector3.Normalize(_targetDir));
        if(dot > 0.985f) 
        { 
            _correctAngle = true;
            SetBeamPositions();

            _conController.BeamRightDirection(this);
            _conController.PedestalHasBeam(_pedestalDestinations);
            return false;
        }
        else
        {
            return true;
        }
    }

    public List<ConstPedestal> ReturnDestinations()
    {
        return _pedestalDestinations;
    }

    #region IINTERACTABLE FUNCTIONS
    public void PlayerInRange(PlayerBase player)
    {
        if (_showPrompt && _canRotateMirror && _id == 0)
        {
            EventManager.EventTrigger(EventType.SHOW_PROMPT_HOLD_INTERACT, null);
        }
    }

    public void PlayerNotInRange(PlayerBase player)
    {
        if (_showPrompt && _canRotateMirror && _id == 0)
        {
            EventManager.EventTrigger(EventType.HIDE_PROMPT_HOLD_INTERACT, null);
        }
    }

    public void PlayerStartInteract(PlayerBase player)
    {
        // If mirror has not been placed on pedestal
        // Ensure player is holding something and it's the correct interactable to be placed on pedestal
        if (player.CarriedPickupable != null && _validInteractables.Contains(player.CarriedPickupable) && _mirror == null)
        {
            // IPickupable and IInteractable manipulation
            GameObject carriedPickupable = player.CarriedPickupable;
            LockPlacedMirror(carriedPickupable);
            IPickupable pickupableType = carriedPickupable.GetComponent<IPickupable>();
            pickupableType.PickupLocked(true);
            pickupableType.BeDropped(_beamSource);

            // If a mirror is to be placed on a pedestal
            if (pickupableType is Level1Mirror)
            {
                _mirror = (Level1Mirror)pickupableType;

                // Added due to removal of dropping functionality of mirror
                _mirror.isFollowing = false;
                _mirror.emissionPS.enabled = false;
                _mirror.SetParent(null);
                player.DropItem();

                InitialRotateMirror(_mirror.transform);
            }
        }
    }

    public void PlayerHoldInteract(PlayerBase player)
    {
        if (_showPrompt && _canRotateMirror && _id == 0)
        {
            _showPrompt = false;
            EventManager.EventTrigger(EventType.HIDE_PROMPT_HOLD_INTERACT, null);
        }

        if (player.CarriedPickupable == null)
        {
            if (_isRotating == false && _beamRenderer.Count != 0 && !_correctAngle)
            {
                StartCoroutine(RotateMirror(_targetDir));
            }
        }
    }

    public void PlayerReleaseHoldInteract(PlayerBase player)
    {
        if (_isRotating)
        {
            StopAllCoroutines();
            CheckAngles();
            _isRotating = false;
            _beamTurningPS.Stop();
        }
    }
    #endregion
}
