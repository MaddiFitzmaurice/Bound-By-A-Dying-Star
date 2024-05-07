using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PPBPairs
{
    public GameObject PP1;
    public GameObject PP2;
}

public class ViewConstellation : MonoBehaviour, IInteractable
{
    [SerializeField] private List<PPBPairs> _pairs;
    private void Awake()
    {
        
    }

    public void PlayerHoldInteract(PlayerBase player)
    {
        throw new System.NotImplementedException();
    }

    public void PlayerInRange(PlayerBase player)
    {
        throw new System.NotImplementedException();
    }

    public void PlayerNotInRange(PlayerBase player)
    {
        throw new System.NotImplementedException();
    }

    public void PlayerStartInteract(PlayerBase player)
    {
        throw new System.NotImplementedException();
    }

    public void PlayerReleaseHoldInteract(PlayerBase player)
    {
        throw new System.NotImplementedException();
    }
}
