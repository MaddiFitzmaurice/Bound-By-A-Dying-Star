using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyRotateLock : CinemachineExtension
{
    private float _xRot;

    protected override void Awake()
    {
        base.Awake();
        _xRot = VirtualCamera.gameObject.transform.eulerAngles.x;
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Finalize)
        {
            var rot = state.RawOrientation.eulerAngles;
            state.RawOrientation = Quaternion.Euler(_xRot, rot.y, 0);
        }
    }
}
