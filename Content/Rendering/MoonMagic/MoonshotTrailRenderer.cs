using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.Meshes;
using System.Collections.Generic;

namespace Stellamod.Content.Rendering.MoonMagic;

public class MoonshotTrailRenderer : MeshRenderer<VertexPositionColorTexture>
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
        PixelationManager.QueuePrimitivesDrawAction(Render, DrawLayer.OverNPCsAdditive);
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

        BloomTrailShader b = BloomTrailShader.Instance;
        b.InnerColor = Color.Blue;
        b.OuterColor = Color.DarkBlue;
        DrawUtilities.DrawUserIndexedPrimitivesWithEffect(arr, indices, b);

        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserColor = Color.White;
        laserShader.LaserTexture = TrailRegistry.StarTrail;
        DrawUtilities.DrawUserIndexedPrimitivesWithEffect(arr2, indices, laserShader);

        _verticesToRenderer.Clear();
        _bigVerticesToRender.Clear();
    }
}