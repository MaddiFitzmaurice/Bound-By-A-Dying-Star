using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class PedestalConstellation : MonoBehaviour
{
    #region EXTERNAL DATA
    // List? of valid interactables that can be placed on the pedestal
    [SerializeField] private List<GameObject> _validInteractables;
    [SerializeField] private GameObject _presetPlacedObject = null;

    // Moving the mirror once it locks into the pedestal
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _raiseMirrorHeight;

    // move the portal to be in the centre of mirror
    [SerializeField] private float _raisePortalHeight;

    // The pedestal that forms a pair with this one
    [SerializeField] private GameObject[] _pairedPedestals;

    //Effects
    [SerializeField] private GameObject _lightBeam;
    [SerializeField] private float _raiseLightBeam;
    [SerializeField] private float _lightBeamLength;
    #endregion

    #region INTERNAL DATA
    // Components
    private Renderer _diskRenderer;
    private ConstellationController _conController;

    // Mirror
    private Level1Mirror _mirror = null;
    #endregion

    void Awake()
    {
        // Get the Renderer component from the disk
        _diskRenderer = GetComponentInChildren<Renderer>();
        _conController = GetComponentInParent<ConstellationController>();  
    }

    // Check if Pedestal has a preset object placed on it
    // if so, place it on the pedestal
    void Start()
    {
        if (_presetPlacedObject != null)
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
                    //pickupableType.BeDropped(transform);

                    // If a mirror is to be placed on a pedestal
                    if (pickupableType is Level1Mirror)
                    {
                        _mirror = (Level1Mirror)pickupableType;
                        StartCoroutine(RotateMirror(_mirror.transform));
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
    }

    void OnTriggerEnter(Collider other)
    {
        // If it's a rift
        if (other.GetComponentInParent<Rift>() != null)
        {
            Rift rift = other.GetComponentInParent<Rift>();
            HandleRift(rift);
        }
        // If player has entered the trigger
        else if (other.CompareTag("Player1") || other.CompareTag("Player2"))
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

    //void OnTriggerExit(Collider other)
    //{
    //    //Reset the disk's color when the object leaves
    //    if (validObjects.Contains(other.gameObject))
    //    {
    //        // Change the disk' color back. THESE ARE ALL TEST COLORS FOR FUNCTIONALITY.
    //        _diskRenderer.material.color = Color.red;
    //    }
    //}

    private IEnumerator RotateMirror(Transform mirror)
    {
        // Set the mirror's position and rotation to match the pedestal before starting the rotation
        mirror.position = new Vector3(transform.position.x, transform.position.y + _raiseMirrorHeight, transform.position.z);
        mirror.rotation = transform.rotation;

        Vector3 targetPosition = _pairedPedestals[0].transform.position;

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
    }

    public void HandleRift(Rift rift)
    {
        if (_mirror != null)
        {
            // Set the portal's position and rotation to match the mirror
            // Adjust portal position to be in centre of mirror object
            rift.transform.position = new Vector3(_mirror.transform.position.x, _mirror.transform.position.y + _raisePortalHeight, _mirror.transform.position.z);
            rift.transform.rotation = _mirror.transform.rotation;

            float targetWidth = _mirror.transform.localScale.x;
            float targetHeight = _mirror.transform.localScale.y;
            // Adjust this value to control the flatness of portal
            float flattenFactor = 0.1f;

            // Scale the portal relative to the mirror's scale
            rift.transform.localScale = new Vector3(targetWidth, targetHeight, flattenFactor);

            // Send to ConstellationController to manage
            _conController.PedestalHasMirror(this);
        }
    }

    public void ActivateEffect(PedestalConstellation otherPedestal)
    {
        GameObject newLightbeam = Instantiate(_lightBeam, transform);
        LineRenderer lineRenderer = newLightbeam.GetComponentInChildren<LineRenderer>();

        // Adjust lightbeam to be in the centre of the mirror
        lineRenderer.transform.position = new Vector3(transform.position.x, transform.position.y + _raiseLightBeam +_raiseMirrorHeight, transform.position.z);

        // End point of the beam in local space
        lineRenderer.SetPosition(1, transform.InverseTransformPoint(otherPedestal.transform.position));
        lineRenderer.enabled = true;
    }
}
