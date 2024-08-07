using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateTriggerAnimation : MonoBehaviour
{
    public GameObject plate; // Reference to the plate object
    public ParticleSystem glowEffect; // Reference to the glow particle system
    public float sinkDistance = 0.2f;
    public float sinkSpeed = 2.0f;

    private const string playerTag1 = "Player1";
    private const string playerTag2 = "Player2";

    private Color player1Color = Color.blue;
    private Color player2Color = new Color(1f, 0.5f, 0f); // Orange

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isPressed = false;
    private int objectsOnPlate = 0;

    private AudioSource audioSource;
    private AudioClip tileDraggingClip;


    void Start()
    {
        if (plate == null)
        {
            Debug.LogError("missing plate reference");
            return;
        }
        if (glowEffect == null)
        {
            Debug.LogError("missing particle systen");
            return;
        }
       
        originalPosition = plate.transform.localPosition;
        targetPosition = originalPosition - new Vector3(0, sinkDistance, 0);

        // Initialize and configure the AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
       
       tileDraggingClip = Resources.Load<AudioClip>("Tile_Dragging_Demo");

        if (tileDraggingClip == null)
        {
            Debug.LogError("Tile_Dragging_Demo audio clip not in Resources Folder (IT WONT WORK UNLESS IN THIS FOLDER FOR SOME REASON AHHHHHHHHHHHHHHHHHHHHHHHH)");
        }
        else
        {
            audioSource.clip = tileDraggingClip;
            audioSource.volume = 1.0f;
            audioSource.playOnAwake = false; 
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag1) || other.CompareTag(playerTag2))
        {
            objectsOnPlate++;
            if (!isPressed)
            {
                isPressed = true;
                StopAllCoroutines();
                StartCoroutine(SinkPlate(targetPosition));
      //          ChangeGlowEffectColor(other.tag);
                glowEffect.Play();
                audioSource.Play();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag1) || other.CompareTag(playerTag2))
        {
            objectsOnPlate--;
            if (objectsOnPlate == 0)
            {
                isPressed = false;
                StopAllCoroutines();
                StartCoroutine(SinkPlate(originalPosition));
                glowEffect.Stop();
                audioSource.Play();
            }
        }
    }

    System.Collections.IEnumerator SinkPlate(Vector3 target)
    {
        while (Vector3.Distance(plate.transform.localPosition, target) > 0.01f)
        {
            plate.transform.localPosition = Vector3.MoveTowards(plate.transform.localPosition, target, sinkSpeed * Time.deltaTime);
            yield return null;
        }
        plate.transform.localPosition = target; 
    }
    /*
    void ChangeGlowEffectColor(string playerTag)
    {
        var mainModule = glowEffect.main;
        if (playerTag == playerTag1)
        {
            mainModule.startColor = player1Color;
        }
        else if (playerTag == playerTag2)
        {
            mainModule.startColor = player2Color;
        }
    }
    */
}
