using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ScreenSpaceOutline : ScriptableRendererFeature
{
    #region EXTERNAL DATA
    [SerializeField] private RenderPassEvent _renderPassEvent; // When passes should execute
    [SerializeField] LayerMask _outlinesLayerMask;
    #endregion
    #region INTERNAL DATA
    private ViewSpaceNormalsTexturePass _viewSpaceNormalsTexturePass; // Generates scene view space normals texture
    private ScreenSpaceOutlinePass _screenSpaceOutlinePass; // Handles outlines
    #endregion

    public override void Create()
    {
        _screenSpaceOutlinePass = new ScreenSpaceOutlinePass(_renderPassEvent);
        _viewSpaceNormalsTexturePass = new ViewSpaceNormalsTexturePass(_renderPassEvent, _outlinesLayerMask);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_viewSpaceNormalsTexturePass);
        renderer.EnqueuePass(_screenSpaceOutlinePass);
    }    
}
