using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkyConstController : MonoBehaviour
{
    [SerializeField] private List<BeamEmitter> _beamList;

    void Awake()
    {
        if (_beamList.Count != 6)
        {
            Debug.LogError("The SkyConstController's _beamList was NOT 6!!!");
            Debug.LogWarning("The SkyConstController's _beamList must have a length of 6, as it is hard-coded sadly");
        }
    }

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.LVL1_STAR_ACTIVATE, StarActivateHandler);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.LVL1_STAR_ACTIVATE, StarActivateHandler);
    }

    public void StarActivateHandler(object data)
    {
        if (data is not int)
        {
            Debug.LogError("StarActivateHandler has not received a int.");
        }

        int beamId = (int)data;
        Debug.Log(beamId);

        switch (beamId)
        {
            case 0:
                _beamList[0].SetBeamStatus(true);
            break;
            case 1:
                _beamList[1].SetBeamStatus(true);
            break;
            case 2:
                _beamList[2].SetBeamStatus(true);
                _beamList[3].SetBeamStatus(true);
            break;
            case 3:
                _beamList[4].SetBeamStatus(true);
            break;
            case 4:
                _beamList[5].SetBeamStatus(true);
            break;
            case 5:
                // Do nothing
            break;
            default:
                Debug.LogError("beamId out of range! should be between 0 and 5");
                Debug.Log("beamId was " + beamId);
            break;
        }
    }
}