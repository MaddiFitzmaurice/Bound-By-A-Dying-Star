using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISoftPuzzleReward
{
    public bool HeldInSoftPuzzle { get; set; }
    public void SetSoftPuzzle(SoftPuzzle softPuzzle);
}
