using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    public GameObject boulder; 
    private bool plate1Active = false;
    private bool plate2Active = false;

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
}