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
        
    }

    private void OnDisable()
    {
        
    }

    // Receive the script as a TextAsset to create a Story object to parse through
    public void SetScript(TextAsset script)
    {
        _currentScript = new Story(script.text);
    }

    // Listens for UI response when in a dialogue
    public void NextLineHandler(object data)
    {
        // If there are lines to parse and no questions
        if (_currentScript.canContinue)
        {
            //_currentScript.Continue();
            
            HandleTags(_currentScript.currentTags);
        }
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
}
