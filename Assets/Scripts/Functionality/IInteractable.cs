using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public bool InteractLocked { get; set; }
    public bool IsHighlighted { get; set; }
    public void PlayerInRange(PlayerBase player);
    public void PlayerNotInRange(PlayerBase player);

    public void PlayerStartInteract(PlayerBase player);
    public void PlayerHoldInteract(PlayerBase player);
    public void PlayerReleaseHoldInteract(PlayerBase player);
}
