using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.PackageManager;
using UnityEngine;

public class PedestalConstellation : MonoBehaviour
{
    #region EXTERNAL DATA
    // List? of valid interactables that can be placed on the pedestal
    [SerializeField] private List<GameObject> _validInteractables;

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
    #endregion

    public bool isPortalPlaced = false;
    public GameObject currentPortal = null;

    void Awake()
    {
        // Get the Renderer component from the disk
        _diskRenderer = GetComponentInChildren<Renderer>();
        _conController = GetComponentInParent<ConstellationController>();  
    }

    void OnTriggerEnter(Collider other)
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
                    StartCoroutine(RotateMirror(carriedPickupable.transform));
                }
            }
        }

        // If player enters, check it's pickup point to check for valid object
        //foreach (Transform child in other.transform)
        //{
        //    foreach (Transform grandChild in child)
        //    {
        //        if (validObjects.Contains(grandChild.gameObject))
        //        {
        //            // Change the disk's color to green
        //            _diskRenderer.material.color = Color.green;

        //            // Start rotating and locking the mirror if it's the correct object
        //            PickupableObject pickupableObject = grandChild.GetComponent<PickupableObject>();

        //            if (pickupableObject != null)
        //            {
        //                // Drops the object
        //                pickupableObject.BeDropped();
        //                // Calls the routine to rotate the mirror and lock it
        //                StartCoroutine(RotateMirror(grandChild.transform, pickupableObject));
        //            }

        //            break;
        //        }
        //    }
        //}
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


        //// Make the mirror a child of the pedestal to maintain relative positioning
        //mirror.SetParent(transform);

        //// Lock the mirror if needed
        //pickupableObject.LockObject();

        //tell contreoler that mirror is on pedestal
        //_conController.PedestalHasMirror(this);

    }

    public void HandlePortalOverlap(GameObject portal, GameObject mirror)
    {
        if (mirror != null)
        {
            // Set the portal's position and rotation to match the mirror
            // Adjust portal position to be in centre of mirror object
            portal.transform.position = new Vector3(mirror.transform.position.x, mirror.transform.position.y + _raisePortalHeight, mirror.transform.position.z);
            portal.transform.rotation = mirror.transform.rotation;

            float targetWidth = mirror.transform.localScale.x;
            float targetHeight = mirror.transform.localScale.y;
            // Adjust this value to control the flatness of portal
            float flattenFactor = 0.1f;

            // Scale the portal relative to the mirror's scale
            portal.transform.localScale = new Vector3(targetWidth, targetHeight, flattenFactor);

            isPortalPlaced = true;
            currentPortal = portal;
            portal.GetComponent<PortalInfo>().AlignWithMirror();

            //Old Logic
            //PortalManager.Instance.CheckForMatchingPortals(this, _pairedPedestal.GetComponent<ConstTrigger>());
            foreach (GameObject pairedPedestal in _pairedPedestals)
            {
                PortalManager.Instance.CheckForMatchingPortals(this, pairedPedestal.GetComponent<PedestalConstellation>());
            }
        }
    }

    public GameObject FindMirror()
    {
        //Loops through each child object to ensure the mirror has been found with the tag "Mirror"
        foreach (Transform child in transform)
        {
            foreach (Transform grandChild in child)
            {
                if (grandChild.CompareTag("Mirror"))
                {
                    return child.gameObject;
                }
            }
        }
        return null;
    }

    public bool IsPortalPlaced()
    {
        return isPortalPlaced;
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
