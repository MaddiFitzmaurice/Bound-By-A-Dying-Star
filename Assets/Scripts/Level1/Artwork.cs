using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artwork : MonoBehaviour, IInteractable
{
    #region EXTERNAL DATA
    public bool InteractLocked { get; set; } = false;
    [SerializeField] private Sprite _artToShow;
    #endregion

    #region INTERNAL DATA
    bool _isArtShowing = false;
    #endregion

    public void PlayerInRange(PlayerBase player)
    {
    }

    public void PlayerNotInRange(PlayerBase player)
    {
    }

    public void PlayerStartInteract(PlayerBase player)
    {
        if (!_isArtShowing)
        {
            EventManager.EventTrigger(EventType.ARTWORK_SHOW, _artToShow);
            EventManager.EventTrigger(EventType.CAN_MOVE, false);
            _isArtShowing = true;
        }
        else
        {
            EventManager.EventTrigger(EventType.ARTWORK_HIDE, null);
            EventManager.EventTrigger(EventType.CAN_MOVE, true);
            _isArtShowing = false;
        }
    }

    public void PlayerHoldInteract(PlayerBase player)
    {
    }

    public void PlayerReleaseHoldInteract(PlayerBase player)
    {
    }
}
