using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class HubCutsceneTrigger : MonoBehaviour
{
    #region EXTERNAL DATA
    [SerializeField] private PlayableAsset _hubCutscene;
    [SerializeField] private Transform _movePlayersTo;
    #endregion

    private void Awake()
    {
        if (_hubCutscene == null)
        {
            Debug.LogError("Please assign a cutscene to HubCutsceneTrigger!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            EventManager.EventTrigger(EventType.CUTSCENE_PLAY, _hubCutscene);
            EventManager.EventTrigger(EventType.LEVEL_SPAWN, _movePlayersTo);
        }
    }
}
