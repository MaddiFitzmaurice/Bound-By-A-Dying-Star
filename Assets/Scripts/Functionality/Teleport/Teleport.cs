using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Linq.Expressions;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Teleport : MonoBehaviour
{
    #region EXTERNAL DATA
    [HideInInspector] public bool IsTwoWays;
    [HideInInspector] public int ActiveTeleTrigger;
    #endregion

    #region INTERNAL DATA
    private List<TeleportTrigger> _teleTriggers;
    private TeleportTrigger _telePos1;
    private TeleportTrigger _telePos2;
    private TeleportTrigger _lastTeleport = null;
    #endregion

    private void Awake()
    {
        _teleTriggers = new List<TeleportTrigger>();
        _teleTriggers = GetComponentsInChildren<TeleportTrigger>().ToList();

        if (_teleTriggers.Count != 2)
        {
            Debug.LogError("Can only have two telepos objects as children of Teleport");
        }

        _telePos1 = _teleTriggers[0];
        _telePos1.SetTeleportParent(this);
        _telePos2 = _teleTriggers[1];
        _telePos2.SetTeleportParent(this);

        if (IsTwoWays)
        {
            _telePos1.Active = true;
            _telePos2.Active = true;
        }
        else
        {
            _teleTriggers[ActiveTeleTrigger - 1].Active = true;
        }
    }

    // Active telePos notifies Teleport, and sends Players to Target telePos
    public void TeleportPlayer(TeleportTrigger trigger)
    {
        if (_lastTeleport == null)
        {
            _lastTeleport = trigger;
            Transform target = trigger == _telePos1 ? _telePos2.transform : _telePos1.transform;
            //EventManager.EventTrigger(EventType.TELEPORT_PLAYERS, target);
        }
    }

    public void ResetTeleport(TeleportTrigger trigger)
    {
        if (_lastTeleport != trigger)
        {
            _lastTeleport = null;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Teleport))]
public class TeleportEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Teleport script = (Teleport)target;

        script.IsTwoWays = EditorGUILayout.Toggle("Is Two Ways", script.IsTwoWays);

        if (!script.IsTwoWays)
        {
            script.ActiveTeleTrigger = EditorGUILayout.IntSlider("Active Tele Trigger", script.ActiveTeleTrigger, 1, 2);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(script);
        }
    }
}
#endif
