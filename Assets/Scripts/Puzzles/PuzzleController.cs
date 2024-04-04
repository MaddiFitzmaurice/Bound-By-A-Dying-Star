using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    public GameObject boulder;
    public GameObject movingPlatform; 
    public Transform respawnPoint; 

    public float platformSpeed = 2f;
    public Vector3 platformStartPos;
    public Vector3 platformEndPos;
    private bool movingToEnd = true;

    private bool plate1Active = false;
    private bool plate2Active = false;

    public List<GameObject> pressurePlatesBObjects = new List<GameObject>(); // Assign in Inspector
    private List<PressurePlateB> pressurePlatesB = new List<PressurePlateB>();
    public GameObject[] platformsToAppear; // Assign in Inspector

    private int activatedPlatesCount = 0;

    private void Awake()
    {
        EventManager.EventInitialise(EventType.PUZZLE_DONE);
    }

        private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.PUZZLE_DONE, PuzzleDoneHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.PUZZLE_DONE, PuzzleDoneHandler);
    }

    void Start()
    {
        // Initialize platforms as inactive
        foreach (GameObject platform in platformsToAppear)
        {
            platform.SetActive(false);
        }

        // Initialize pressure plates list from GameObjects
        foreach (GameObject plateObj in pressurePlatesBObjects)
        {
            PressurePlateB plateScript = plateObj.GetComponent<PressurePlateB>();
            if (plateScript != null)
            {
                pressurePlatesB.Add(plateScript);
                plateScript.puzzleController = this; // Ensure the plate script references back to this controller
            }
        }
    }

    void Update()
    {
        MovePlatform();
    }

    void MovePlatform()
    {
        if (movingToEnd)
        {
            movingPlatform.transform.position = Vector3.MoveTowards(movingPlatform.transform.position, platformEndPos, platformSpeed * Time.deltaTime);
            if (movingPlatform.transform.position == platformEndPos)
                movingToEnd = false;
        }
        else
        {
            movingPlatform.transform.position = Vector3.MoveTowards(movingPlatform.transform.position, platformStartPos, platformSpeed * Time.deltaTime);
            if (movingPlatform.transform.position == platformStartPos)
                movingToEnd = true;
        }
    }

    public void PlayerSteppedOnPlate(PuzzlePressurePlate plate)
    {
        // Check which plate the player stepped on and then trigger
        if (plate.gameObject.name == "Pressure_plate1")
        {
            plate1Active = true;
        }
        else if (plate.gameObject.name == "Pressure_plate2")
        {
            plate2Active = true;
        }

        CheckPuzzleSolved();
    }

    public void PlayerLeftPlate(PuzzlePressurePlate plate)
    {
        // Check which plate the player left
        if (plate.gameObject.name == "Pressure_plate1")
        {
            plate1Active = false;
        }
        else if (plate.gameObject.name == "Pressure_plate2")
        {
            plate2Active = false;
        }
    }

    void CheckPuzzleSolved()
    {
        // If both plates are active, solve the puzzle
        if (plate1Active && plate2Active)
        {
            SolvePuzzle();
        }
    }

    void SolvePuzzle()
    {
        // Make the boulder disappear permanently
        Destroy(boulder);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            other.transform.position = respawnPoint.position; // Teleport player to respawn point
        }
    }

    public void PressurePlateBActivated()
    {
        activatedPlatesCount++;
        CheckAllPlatesBActivated();
    }

    void CheckAllPlatesBActivated()
    {
        if (activatedPlatesCount >= pressurePlatesB.Count)
        {
            foreach (GameObject platform in platformsToAppear)
            {
                platform.SetActive(true);
            }
        }
    }

    public void PuzzleDoneHandler(object data)
    {
        if (data == null)
        {
            Debug.LogError("PuzzleDoneHandler is null!");
        }

        // do stuff here 
        Debug.Log("PUZLE DONE WOOOO");
    }
}