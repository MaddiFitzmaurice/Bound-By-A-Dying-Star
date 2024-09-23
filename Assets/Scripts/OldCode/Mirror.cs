using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    public PuzzleController puzzleController; 
    public bool isEmittingLight = false; // Track whether this mirror is emitting light

    // Call this method to indicate the mirror has started emitting light
    public void StartEmittingLight()
    {
        if (!isEmittingLight)
        {
            isEmittingLight = true;
            puzzleController.MirrorStartedEmittingLight();
        }
    }

    // Call this method if the mirror stops emitting light
    public void StopEmittingLight()
    {
        if (isEmittingLight)
        {
            isEmittingLight = false;
            puzzleController.MirrorStoppedEmittingLight();
        }
    }
}
