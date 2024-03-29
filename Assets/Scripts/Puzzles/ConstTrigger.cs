using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstTrigger : MonoBehaviour
{
    public List<GameObject> validObjects; // List of constellations that will change color
    private Renderer _diskRenderer;
    [SerializeField] private Vector3 _mirrorRotationAngle;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _portalSizeScale;

    [SerializeField] private GameObject pairedPedestal; // The pedestal that forms a pair with this one

    public bool isPortalPlaced = false;
    private GameObject currentPortal = null;

    void Start()
    {
        // Get the Renderer component from the disk
        _diskRenderer = GetComponent<Renderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the Mirror is in the list of valid objects
        if (validObjects.Contains(other.gameObject))
        {
            // Change the disk's color to green
            _diskRenderer.material.color = Color.green;

            // Start rotating and locking the mirror if it's the correct object
            PickupableObject pickupableObject = other.GetComponent<PickupableObject>();
                     
            if (pickupableObject != null)
            {
                // Drops the object
                pickupableObject.BeDropped();
                // Calls the routine to rotate the mirror and lock it
                StartCoroutine(RotateMirror(other.transform, pickupableObject));
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

        mirror.rotation = targetRotation; // Ensure the mirror snaps to the exact rotation

        // Make the mirror a child of the pedestal to maintain relative positioning
        mirror.SetParent(transform);

        // Lock the mirror if needed
        pickupableObject.LockObject();
    }

    public void HandlePortalOverlap(GameObject portal, GameObject mirror)
    {
        if (mirror != null)
        {
            // Get the mirror's transform scale
            Vector3 mirrorScale = mirror.GetComponent<Renderer>().bounds.size;

            // Set the portal's position and rotation to match the mirror
            //MANUAL MOVING OF PORTAL, REMOVE OCNE CENTRE OF OBJECT HAS BEEN FIXED
            portal.transform.position = new Vector3(mirror.transform.position.x, mirror.transform.position.y * 1.5f, mirror.transform.position.z);
            portal.transform.rotation = mirror.transform.rotation;

            float targetWidth = mirror.transform.localScale.x;
            float targetHeight = mirror.transform.localScale.y;
            float flattenFactor = 0.1f; // Adjust this value to control the flatness

            // Scale the portal relative to the mirror's scale
            portal.transform.localScale = new Vector3(targetWidth, targetHeight, flattenFactor);

            isPortalPlaced = true;
            currentPortal = portal;
           
            PortalManager.Instance.CheckForMatchingPortals(this, pairedPedestal.GetComponent<ConstTrigger>());

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

    public void LockPortal()
    {
        if (currentPortal != null)
        {
            PortalInfo portalInfo = currentPortal.GetComponent<PortalInfo>();
            if (portalInfo != null && !portalInfo.isLocked)
            {
                portalInfo.LockPortal();
                isPortalPlaced = true;
            }
        }
    }

    public bool IsPortalPlaced()
    {
        return isPortalPlaced;
    }
}
