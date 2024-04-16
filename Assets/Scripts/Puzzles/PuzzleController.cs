using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    public GameObject boulder;
    public GameObject movingPlatform;
    public bool isPlatformMoving = true;  // Track the movement state of the platform
    public Transform respawnPoint; 

    public float platformSpeed = 2f;
    public Vector3 platformStartPos;
    public Vector3 platformEndPos;
    private bool movingToEnd = true;

    private bool plate1Active = false;
    private bool plate2Active = false;

    public List<GameObject> pressurePlatesBObjects = new List<GameObject>(); // Assign in Inspector
    private List<PressurePlateB> pressurePlatesB = new List<PressurePlateB>();
    private int activatedPlatesCount = 0;
    private Dictionary<string, bool> plateStatus = new Dictionary<string, bool>();
    private Dictionary<string, Renderer> plateRenderers = new Dictionary<string, Renderer>();
    private Dictionary<string, bool> pairSuccess = new Dictionary<string, bool>(); // Tracks successful pairings


    public GameObject[] platformsToAppear; // Assign in Inspector

    public List<Mirror> mirrors = new List<Mirror>(); // Assign all mirrors in the Inspector
    public GameObject puzzleDoor; // Assign the puzzle door in the Inspector
    private int emittingMirrorsCount = 0; // Track the number of mirrors currently emitting light


   

    public List<Transform> checkpoints = new List<Transform>(); // Assign in Inspector

    private Transform currentRespawnTransform;

    private Vector3 currentRespawnPoint;





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
                plateScript.puzzleController = this; 
            }
        }
        currentRespawnPoint = checkpoints.Count > 0 ? checkpoints[0].position : transform.position;
        InitializePlates();
      
    }

    void Update()
    {
        if (isPlatformMoving)
        {
            MovePlatform();  
        }
    }

    public void StopPlatform()
    {
        isPlatformMoving = false;
    }

    public void ResumePlatform()
    {
        isPlatformMoving = true;
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
            RespawnPlayer(other.gameObject); // Direct call without 'puzzleController'
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

    private Dictionary<string, Coroutine> plateTimers = new Dictionary<string, Coroutine>();

    public void PlateActivated(string id, Renderer renderer)
    {
        plateStatus[id] = true;
        if (!plateRenderers.ContainsKey(id))
        {
            plateRenderers[id] = renderer;
        }
        StartCoroutine(CheckPairedPlate(id));
    }

    // Coroutine to check paired plates
    IEnumerator CheckPairedPlate(string id)
    {
        yield return new WaitForSeconds(2);
        string pairId = GetPairedId(id);
        if (plateStatus.ContainsKey(pairId) && plateStatus[pairId])
        {
            Color randomColor = new Color(Random.value, Random.value, Random.value);
            plateRenderers[id].material.color = randomColor;
            plateRenderers[pairId].material.color = randomColor;
            pairSuccess[id] = true;
            pairSuccess[pairId] = true;
            CheckAllPairs();  // Check if all pairs are successful
        }
        else
        {
            plateRenderers[id].material.color = Color.white;
            if (plateRenderers.ContainsKey(pairId))
            {
                plateRenderers[pairId].material.color = Color.white;
            }
            plateStatus[id] = false;
            plateStatus[pairId] = false;
        }
    }

    void CheckAllPairs()
    {
        // Check if all pairs are successfully completed
        foreach (var pair in pairSuccess)
        {
            if (!pair.Value) return;  // If any pair is not successful, immediately exit the method
        }

        // If all pairs are successful, loop through each platform in the array and set it active
        foreach (GameObject platform in platformsToAppear)
        {
            platform.SetActive(true);  // Correctly apply SetActive to each GameObject in the array
        }
    }

    string GetPairedId(string id)
    {
        var pairs = new Dictionary<string, string> {
            {"1", "A"}, {"2", "B"}, {"3", "C"}, {"4", "D"}, {"5", "E"},
            {"A", "1"}, {"B", "2"}, {"C", "3"}, {"D", "4"}, {"E", "5"}
        };
        return pairs.ContainsKey(id) ? pairs[id] : null;
    }

    void InitializePlates()
    {
        string[] ids = new string[] { "1", "2", "3", "4", "5", "A", "B", "C", "D", "E" };
        foreach (var id in ids)
        {
            plateStatus[id] = false;
            pairSuccess[id] = false;  // Initialize pairing success
        }
    }

    public void RegisterPlate(string id, Renderer renderer)
    {
        plateRenderers[id] = renderer;
        plateStatus[id] = false;
    }


    public void UpdatePlateStatus(string id, bool isActive)
    {
        if (plateStatus.ContainsKey(id))
        {
            plateStatus[id] = isActive;
            CheckPairedPlate(id);  // Passing the id to the method
        }
    }

    public void PuzzleDoneHandler(object data)
    {
        if (data is not int)
        {
            Debug.LogError("PuzzleDone handler has not received an int!");
        }

        int code = (int)data;

        // Remove door
        if (code == 1)
        {
            puzzleDoor.SetActive(false);
        }
        
    }

    public void MirrorStartedEmittingLight()
    {
        emittingMirrorsCount++;
        CheckAllMirrorsEmittingLight();
    }

    public void MirrorStoppedEmittingLight()
    {
        emittingMirrorsCount--;
    }

    void CheckAllMirrorsEmittingLight()
    {
        // If all mirrors are emitting light, remove the puzzle door
        if (emittingMirrorsCount == mirrors.Count)
        {
            puzzleDoor.SetActive(false);
        }
    }

    // Call this method to update the current respawn point when players pass a checkpoint
    public void UpdateRespawnPoint(Transform newRespawnPoint)
    {
        currentRespawnPoint = newRespawnPoint.position;
    }

    public void RespawnPlayer(GameObject player)
    {
        player.transform.position = currentRespawnPoint;
    }
}