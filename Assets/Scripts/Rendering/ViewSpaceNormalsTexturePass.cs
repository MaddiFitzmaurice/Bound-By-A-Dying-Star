using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ViewSpaceNormalsTexturePass : ScriptableRenderPass
{
    private readonly RTHandle _normalsTexture; // Render texture handler
    private readonly List<ShaderTagId> _shaderTags; // Shaders 
    private readonly Material _normalsMaterial; // Override material
    private FilteringSettings _filteringSettings;

    public ViewSpaceNormalsTexturePass(RenderPassEvent renderPassEvent, LayerMask layerMask)
    {
        this.renderPassEvent = renderPassEvent;

        // Create filter setting that only targets those within the specified layers
        _filteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);

        // List to reference all objects that use CelShader 
        _shaderTags = new List<ShaderTagId>();
        _shaderTags.Add(new ShaderTagId("CelShader"));

        // Create override material
        _normalsMaterial = new Material(Shader.Find("Hidden/ViewSpaceNormalsShader"));

        // Associate shader property with render target
        _normalsTexture = RTHandles.Alloc("_SceneViewSpaceNormals", name: "_SceneViewSpaceNormals");
    }

    // Called before render pass
    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        // Create temp render texture and set it up as a global shader property
        cmd.GetTemporaryRT(Shader.PropertyToID(_normalsTexture.name), cameraTextureDescriptor, FilterMode.Point);
        // Use new RTHandle as render target for this pass
        ConfigureTarget(_normalsTexture);
        // Clear render target for this pass
        ConfigureClear(ClearFlag.All, Color.clear);
    }

    // Execute render pass
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        // If override material does not exist, exit the pass
        if (_normalsMaterial == null)
        {
            return;
        }
        // Get command buffer
        CommandBuffer cmd = CommandBufferPool.Get();

        // Create profiling scope so the frame debugger can profile the code
        using (new ProfilingScope(cmd, new ProfilingSampler("SceneViewSpaceNormalTextureCreation")))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            // Create draw settings with objects associated with celshader to look for,
            // and set the overriding material
            DrawingSettings drawSettings = CreateDrawingSettings(_shaderTags, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            drawSettings.overrideMaterial = _normalsMaterial;
            context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref _filteringSettings);
        }
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    // Called when camera is finished rendering
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        // Release temp render texture
        cmd.ReleaseTemporaryRT(Shader.PropertyToID(_normalsTexture.name));
    }
}
