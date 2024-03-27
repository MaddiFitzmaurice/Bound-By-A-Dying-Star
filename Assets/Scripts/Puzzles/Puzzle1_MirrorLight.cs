using UnityEngine;

public class Puzzle1_MirrorLight : MonoBehaviour
{
    public Light mirrorLight;
    public float maxIntensity = 5f;
    public float maxDistance = 10f;
    private GameObject _playerPosition;

    void Start()
    {
        Invoke("FindPlayer", 0.5f); // Delay added to ensure all objects are loaded
    }

    void FindPlayer()
    {
        // Finding the player GameObject by the "Player1" tag
        _playerPosition = GameObject.FindGameObjectWithTag("Player1");

        if (_playerPosition == null)
        {
            Debug.LogError("Player 1 not found. Ensure the player is loaded and tagged as 'Player1'.");
        }
    }

    void Update()
    {
        if (_playerPosition != null)
        {
            AdjustLightIntensity();
        }
    }

    private void AdjustLightIntensity()
    {
        // Calculate the distance between the player and the mirror
        float distance = Vector3.Distance(_playerPosition.transform.position, transform.position);

        // Normalize the distance based on maxDistance via the clamp method
        float normalizedDistance = Mathf.Clamp01(distance / maxDistance);

        // Adjust the light intensity
        mirrorLight.intensity = maxIntensity * (1 - normalizedDistance);
    }
}
