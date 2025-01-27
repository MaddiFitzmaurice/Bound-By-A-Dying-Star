using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTrigger : MonoBehaviour
{
    #region INTERNAL DATA
    // Components
    private FixedCamSystem _system;
    // Gizmo color
    private Color _color = Color.blue;
    #endregion

    #region FRAMEWORK FUNCTIONS
    private void Awake()
    {
        // Set up Components
        _system = GetComponentInParent<FixedCamSystem>();
    }

    // Draw the colliders in editor so we can see them even if they're not selected
#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = _color;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
#endif

    public void OnTriggerEnter(Collider other)
    {
        // Check if Player 1 entered trigger
        var p1 = other.GetComponent<Player1>();

        if (p1 != null)
        {
            
            _system.TriggerEntered(p1);
            return;
        }

        // If no Player 1, check Player 2
        var p2 = other.GetComponent<Player2>();

        if (p2 != null)
        {
            _system.TriggerEntered(p2);
            return;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        // Check if Player 1 exited trigger
        var p1 = other.GetComponent<Player1>();

        if (p1 != null)
        {
            _system.TriggerExited(p1);
            return;
        }

        // If no Player 1, check Player 2
        var p2 = other.GetComponent<Player2>();

        if (p2 != null)
        {
            _system.TriggerExited(p2);
            return;
        }
    }
    #endregion

    // Changes color if parent or cam is selected
#if UNITY_EDITOR
    public void IsSelected(bool selected)
    {
        _color = selected ? Color.green : Color.blue;
    }
#endif
}
