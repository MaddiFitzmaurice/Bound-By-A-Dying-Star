using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artwork : MonoBehaviour, IInteractable
{
    #region EXTERNAL DATA
    public bool InteractLocked { get; set; } = false;
    public bool IsHighlighted { get; set; } = false;
    [SerializeField] private Sprite _artToShow;
    #endregion

    #region INTERNAL DATA
    bool _isArtShowing = false;
    #endregion

    private void ChangeLayers(LayerMask layer)
    {
        gameObject.layer = layer;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.layer = layer;
        }
    }

    #region IINTERACTABLE FUNCTIONS
    public void PlayerInRange(PlayerBase player)
    {
        if (!IsHighlighted)
        {
            ChangeLayers(LayerMask.NameToLayer("HighlightInteract"));
            IsHighlighted = true;
        }
    }

    public void PlayerNotInRange(PlayerBase player)
    {
        if (IsHighlighted)
        {
            ChangeLayers(LayerMask.NameToLayer("Interactables"));
            IsHighlighted = false;
        }
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
    #endregion
}
