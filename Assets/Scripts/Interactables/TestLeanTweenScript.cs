using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLeanTweenScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.moveX(gameObject, 10, 6);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
