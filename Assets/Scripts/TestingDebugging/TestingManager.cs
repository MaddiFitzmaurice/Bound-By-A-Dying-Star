using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ControlType
{
    NORMAL,
    TANK,
    FIXEDCAM
}

public class TestingManager : MonoBehaviour
{
    #region EXTERNAL DATA
    [SerializeField] private Button _movementTypeButton;
    [SerializeField] private TextMeshProUGUI _movementTypeText;
    #endregion

    #region INTERNAL DATA
    private ControlType _controlType = ControlType.NORMAL;
    #endregion

    private void Awake()
    {
        // Event Inits
        EventManager.EventInitialise(EventType.TEST_CONTROLS);
        _movementTypeText.text = "Movement Type: " + _controlType.ToString();
    }

    public void ChangeMovementType()
    {
        int i = (int)_controlType;

        _ = i == 2 ? i = 0 : i++;

        _controlType = (ControlType)i;
        
        EventManager.EventTrigger(EventType.TEST_CONTROLS, _controlType);
        _movementTypeText.text = "Movement Type: " + _controlType.ToString();
    }
}
