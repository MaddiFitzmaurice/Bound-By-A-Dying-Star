using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConstTrigger : MonoBehaviour
{
    // List of constellations that will change color
    public List<GameObject> validObjects; 
    private Renderer _diskRenderer;
    [SerializeField] private Vector3 _mirrorRotationAngle;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _portalSizeScale;

    // The pedestal that forms a pair with this one
    [SerializeField] private GameObject[] _pairedPedestals;

    

    //Effects
    [SerializeField] private ParticleSystem _lightEffect;
    [SerializeField] private LineRenderer _lightBeam;

    public bool isPortalPlaced = false;
    public GameObject currentPortal = null;

    void Start()
    {
        // Get the Renderer component from the disk
        _diskRenderer = GetComponentInChildren<Renderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        // If player enters, check it's pickup point to check for valid object
        foreach (Transform child in other.transform)
        {
            foreach (Transform grandChild in child)
            {
                if (validObjects.Contains(grandChild.gameObject))
                {
                    // Change the disk's color to green
                    _diskRenderer.material.color = Color.green;

                    // Start rotating and locking the mirror if it's the correct object
                    PickupableObject pickupableObject = grandChild.GetComponent<PickupableObject>();

                    if (pickupableObject != null)
                    {
                        // Drops the object
                        pickupableObject.BeDropped();
                        // Calls the routine to rotate the mirror and lock it
                        StartCoroutine(RotateMirror(grandChild.transform, pickupableObject));
                    }

                    break;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        //Reset the disk's color when the object leaves
        if (validObjects.Contains(other.gameObject))
        {
            // Change the disk' color back. THESE ARE ALL TEST COLORS FOR FUNCTIONALITY.
            _diskRenderer.material.color = Color.red;
        }
    }

    private IEnumerator RotateMirror(Transform mirror, PickupableObject pickupableObject)
    {
        // Set the mirror's position and rotation to match the pedestal before starting the rotation
        mirror.position = transform.position;
        mirror.rotation = transform.rotation;

        Quaternion targetRotation = Quaternion.Euler(_mirrorRotationAngle + transform.eulerAngles);

        while (Quaternion.Angle(mirror.rotation, targetRotation) > 0.01f)
        {
            mirror.rotation = Quaternion.Lerp(mirror.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
            yield return null;
        }

        // Ensure the mirror snaps to the exact rotation
        mirror.rotation = targetRotation; 

        // Make the mirror a child of the pedestal to maintain relative positioning
        mirror.SetParent(transform);

        // Lock the mirror if needed
        pickupableObject.LockObject();

        // Set laser beam to go in the direction of the mirror (+ 25) then turn beam on
        Vector3 beamDirection = mirror.position + mirror.forward * 10;
        _lightBeam.SetPosition(1, beamDirection);
        _lightBeam.enabled = true;
    }

    public void HandlePortalOverlap(GameObject portal, GameObject mirror)
    {
        if (mirror != null)
        {
            // Get the mirror's transform scale
            Vector3 mirrorScale = mirror.GetComponent<Renderer>().bounds.size;

            // Set the portal's position and rotation to match the mirror
            //MANUAL MOVING OF PORTAL, REMOVE ONCE CENTRE OF OBJECT HAS BEEN FIXED
            portal.transform.position = new Vector3(mirror.transform.position.x, mirror.transform.position.y * 1.5f, mirror.transform.position.z);
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
                PortalManager.Instance.CheckForMatchingPortals(this, pairedPedestal.GetComponent<ConstTrigger>());
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

    public void ActivateEffect()
    {
        if (_lightEffect != null)
        {
            _lightEffect.Play();
        }
    }
}
