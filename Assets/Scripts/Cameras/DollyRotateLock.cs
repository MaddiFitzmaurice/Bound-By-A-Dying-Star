using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyRotateLock : CinemachineExtension
{
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Finalize)
        {
            var rot = state.RawOrientation.eulerAngles;
            state.RawOrientation = Quaternion.Euler(0, rot.y, 0);
        }
    }
}
