using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarConstController : MonoBehaviour
{

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LVL1_STAR_ACTIVATE, StarActivateHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LVL1_STAR_ACTIVATE, StarActivateHandler);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StarActivateHandler(object data)
    {
        if (data is not int)
        {
            Debug.LogError("StarActivateHandler has not received a int.");
        }

        
    }
}
