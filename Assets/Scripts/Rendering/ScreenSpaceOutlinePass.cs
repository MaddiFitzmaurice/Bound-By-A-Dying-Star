using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class ScreenSpaceOutlinePass : ScriptableRenderPass
{
    #region INTERNAL DATA
    private RTHandle _tempBuffer;
    private RTHandle _cameraColorTarget;
    private readonly Material _screenSpaceOutlineMaterial;
    #endregion

    public ScreenSpaceOutlinePass(RenderPassEvent renderPassEvent)
    {
        this.renderPassEvent = renderPassEvent;
        _screenSpaceOutlineMaterial = new Material(Shader.Find("Hidden/OutlinesShader"));
    }

    // Called before render pass
    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
    }

    // Execute render pass
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        // If override material does not exist, exit the pass
        if (_screenSpaceOutlineMaterial == null)
        {
            return;
        }

        // Get command buffer
        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, new ProfilingSampler("ScreenSpaceOutlines")))
        {
            // Undefined behaviour if we blit using single render target as source and dest
            Blit(cmd, _cameraColorTarget, _tempBuffer, _screenSpaceOutlineMaterial);
            Blit(cmd, _tempBuffer, _cameraColorTarget, _screenSpaceOutlineMaterial);
        }
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    // Called when camera is finished rendering
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        base.OnCameraCleanup(cmd);
    }
}
