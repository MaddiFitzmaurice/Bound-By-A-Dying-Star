using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void PlayerInRange(Material mat);
    public void PlayerNotInRange();

    public void PlayerStartInteract();
}
