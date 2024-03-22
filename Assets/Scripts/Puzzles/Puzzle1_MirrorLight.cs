using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle1_MirrorLight : MonoBehaviour
{
   public Transform player; 
    public Light mirrorLight; 
    public float maxIntensity = 5f; 
    public float maxDistance = 10f; 
	//Puzzle light variables
    private void Update()
    {
        AdjustLightIntensity();
    }

    void AdjustLightIntensity()
    {
        // Calculate the distance between the player and the mirror
        float distance = Vector3.Distance(player.position, transform.position);

        // Normalize the distance based on maxDistance via the clamp method
        float normalizedDistance = Mathf.Clamp01(distance / maxDistance);

        // Adjust the light intensity 
        mirrorLight.intensity = maxIntensity * (1 - normalizedDistance);
    }
}
