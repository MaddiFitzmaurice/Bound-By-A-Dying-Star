using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class InkManager : MonoBehaviour
{
    private Story _currentScript;

    private void Awake()
    {
       
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.NPC_SEND_DIALOGUE, ReceiveTextAssetHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.NPC_SEND_DIALOGUE, ReceiveTextAssetHandler);
    }

    // Create a Story object from a TextAsset to begin parsing through
    public void SetScript(TextAsset script)
    {
        _currentScript = new Story(script.text);
    }

    // Handle any tags that are above the lines in the script
    public void HandleTags(List<string> currentTags)
    {
        if (currentTags.Count != 0)
        {
            foreach (string tag in currentTags)
            {
            }
        }
    }

    #region EVENT HANDLERS
    // Listens for UI response when in a dialogue
    public void ReceiveTextAssetHandler(object data)
    {
        if (data is not TextAsset)
        {
            Debug.LogError("InkManager has not received a TextAsset.");
        }

        SetScript(data as TextAsset);
    }

    public void NextLineHandler(object data)
    {
        // If there are lines to parse and no questions
        if (_currentScript.canContinue)
        {
            _currentScript.Continue();
            
            HandleTags(_currentScript.currentTags);
        }
    }
    #endregion
}