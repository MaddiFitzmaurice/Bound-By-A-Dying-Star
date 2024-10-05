using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ControlType
{
    WORLDSTRAFE,
    TANK,
    FIXEDCAM,
    FIXEDCAM2,
}

public class TestingManager : MonoBehaviour
{
    #region EXTERNAL DATA
    [SerializeField] private Button _movementTypeButton;
    [SerializeField] private TextMeshProUGUI _movementTypeText;
    [SerializeField] private TextMeshProUGUI _fpsText;
    #endregion

    #region INTERNAL DATA
    private ControlType _controlType = ControlType.FIXEDCAM2;
    private FPS _fps;
    #endregion

    private void Awake()
    {
        // Event Inits
        EventManager.EventInitialise(EventType.TEST_CONTROLS);
        _movementTypeText.text = "Movement Type: " + _controlType.ToString();

        // Component Inits
        _fps = GetComponent<FPS>();
    }

    private void Update()
    {
        _fpsText.text = "FPS Avg: " + _fps.GetCurrentFPS();
    }

    public void ChangeMovementType()
    {
        int i = (int)_controlType;

        _ = i == 3 ? i = 0 : i++;

        _controlType = (ControlType)i;
        
        EventManager.EventTrigger(EventType.TEST_CONTROLS, _controlType);
        _movementTypeText.text = "Movement Type: " + _controlType.ToString();
    }
}
