using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestingManager : MonoBehaviour
{
    #region EXTERNAL DATA
    [SerializeField] private Button _movementTypeButton;
    [SerializeField] private TextMeshProUGUI _movementTypeText;
    #endregion

    #region INTERNAL DATA
    private bool _tankControls = false;
    #endregion

    private void Awake()
    {
        // Event Inits
        EventManager.EventInitialise(EventType.TEST_CONTROLS);
        _movementTypeText.text = _tankControls ? "MovementType: Tank" : "Movement Type: Norm";
    }

    public void ChangeMovementType()
    {
        _tankControls = !_tankControls;
        EventManager.EventTrigger(EventType.TEST_CONTROLS, _tankControls);
        _movementTypeText.text = _tankControls ? "MovementType: Tank" : "Movement Type: Norm";
    }
}
