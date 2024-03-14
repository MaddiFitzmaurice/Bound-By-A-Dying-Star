using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    public void PlayerAboutToInteract(Collider player, bool aboutToInteract);
}
