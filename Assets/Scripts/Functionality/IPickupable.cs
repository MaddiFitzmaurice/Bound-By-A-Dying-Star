using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickupable
{
    public void BePickedUp(PlayerBase player);
    public void BeDropped(Transform newParent);
    public void SetParent(Transform newParent);
    public void PickupLocked(bool flag);
}
