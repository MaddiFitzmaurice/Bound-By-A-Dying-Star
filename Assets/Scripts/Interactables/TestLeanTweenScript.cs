using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLeanTweenScript : MonoBehaviour
{
    public FMODUnity.EventReference sound;
    
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.moveX(gameObject, 10, 6);
        FMODUnity.RuntimeManager.PlayOneShotAttached(sound, gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
