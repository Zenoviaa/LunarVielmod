using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.Meshes;
using System.Collections.Generic;

namespace Stellamod.Content.Rendering.GenericEffects;

public class SpiralingWindTrailRenderer : MeshRenderer<VertexPositionColorTexture>
{
    private readonly List<VertexPositionColorTexture> _bigVerticesToRender = new();
    public override void Load()
    {
        base.Load();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady += QueuePixelatedRendering;
    }

    public override void Unload()
    {
        base.Unload();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady -= QueuePixelatedRendering;
    }

    private void QueuePixelatedRendering()
    {
        if (_verticesToRenderer.Count <= 0)
            return;
        PixelationManager.QueuePrimitivesDrawAction(Render, DrawLayer.OverNPCs);
    }

    public void PrepareForBigRendering(IEnumerable<VertexPositionColorTexture> vertices)
    {
        _bigVerticesToRender.AddRange(vertices);
    }

    private void Render(GraphicsDevice graphicsDevice)
    {
        VertexPositionColorTexture[] arr = _verticesToRenderer.ToArray();
        VertexPositionColorTexture[] arr2 = _bigVerticesToRender.ToArray();
        short[] indices = DrawUtilities.PrepareIndicesForDrawing(arr.Length / 2);

        graphicsDevice.RasterizerState = RasterizerState.CullNone;
        graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

        BasicLaserShader bloomShader = ShaderContent.GetInstance<BasicLaserShader>();
        bloomShader.LaserTexture = AssetManager.LaserTextures.CometTrail;
        bloomShader.InnerColor = Color.SkyBlue;
        bloomShader.OuterColor = Color.DarkBlue;
        bloomShader.Apply();
        DrawUtilities.DrawUserIndexedPrimitivesWithEffect(arr2, indices, bloomShader);

        BasicLaserShader basicLaserShader = ShaderContent.GetInstance<BasicLaserShader>();
        basicLaserShader.LaserTexture = AssetManager.LaserTextures.Aura;
        basicLaserShader.InnerColor = Color.SkyBlue;
        basicLaserShader.OuterColor = Color.DarkBlue;
        basicLaserShader.Apply();
        DrawUtilities.DrawUserIndexedPrimitivesWithEffect(arr2, indices, basicLaserShader);

        basicLaserShader.InnerColor = Color.White;
        basicLaserShader.OuterColor = Color.DarkGray;
        basicLaserShader.Apply();
        DrawUtilities.DrawUserIndexedPrimitivesWithEffect(arr, indices, basicLaserShader);
        _verticesToRenderer.Clear();
        _bigVerticesToRender.Clear();
    }
}

