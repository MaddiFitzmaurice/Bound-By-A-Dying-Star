using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    [SerializeField] private float _maxFrames = 60f; // Max avg frames

    private float _lastFPSCalculated = 0f;
    private List<float> _frameTimes = new List<float>();

    // Use this for initialization
    void Start()
    {
        _lastFPSCalculated = 0f;
        _frameTimes.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        AddFrame();
        _lastFPSCalculated = CalculateFPS();
    }

    private void AddFrame()
    {
        _frameTimes.Add(Time.unscaledDeltaTime);

        if (_frameTimes.Count > _maxFrames)
        {
            _frameTimes.RemoveAt(0);
        }
    }

    private float CalculateFPS()
    {
        float newFPS = 0f;

        float totalTimeOfAllFrames = 0f;

        foreach (float frame in _frameTimes)
        {
            totalTimeOfAllFrames += frame;
        }
        
        newFPS = ((float)(_frameTimes.Count)) / totalTimeOfAllFrames;

        return newFPS;
    }

    public float GetCurrentFPS()
    {
        return _lastFPSCalculated;
    }
}
