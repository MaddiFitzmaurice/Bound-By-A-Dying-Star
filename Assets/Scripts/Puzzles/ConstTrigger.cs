using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstTrigger : MonoBehaviour
{
    public List<GameObject> validObjects; // List of constellations that will change color
    private Renderer _diskRenderer;
    [SerializeField] private Vector3 _mirrorRotationAngle;
    [SerializeField] private float _rotationSpeed;

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
        Quaternion targetRotation = Quaternion.Euler(_mirrorRotationAngle);
        while (Quaternion.Angle(mirror.rotation, targetRotation) > 0.01f)
        {
            mirror.rotation = Quaternion.Lerp(mirror.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
            yield return null;
        }
        mirror.rotation = targetRotation; // Ensure the mirror snaps to the exact rotation

        // Lock the mirror if needed
        pickupableObject.LockObject();
    }
}
