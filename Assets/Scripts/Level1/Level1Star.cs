using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Star : MonoBehaviour, IInteractable, ISoftPuzzleReward, IPickupable
{
    #region EXTERNAL DATA
    public bool InteractLocked { get; set; } = false;
    public bool HeldInSoftPuzzle { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool IsHighlighted { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    #endregion

    #region IPICKUPABLE FUNCTIONS
    public void BeDropped(Transform newParent)
    {
    }

    public void BePickedUp(PlayerBase player)
    {
    }

    public void PickupLocked(bool flag)
    {
    }

    public void SetParent(Transform newParent)
    {
    }
    #endregion


    #region IINTERACTABLE FUNCTIONS
    public void PlayerHoldInteract(PlayerBase player)
    {
    }

    public void PlayerInRange(PlayerBase player)
    {
    }

    public void PlayerNotInRange(PlayerBase player)
    {
    }

    public void PlayerReleaseHoldInteract(PlayerBase player)
    {
    }

    public void PlayerStartInteract(PlayerBase player)
    {
    }
    #endregion

    #region ISOFTPUZZLEREWARD FUNCTIONS
    public void SetSoftPuzzle(SoftPuzzle softPuzzle)
    {
    }

    public void SetRewardGrouper(Transform transform)
    {
    }
    #endregion
}
