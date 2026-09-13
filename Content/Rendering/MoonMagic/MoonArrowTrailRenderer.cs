using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.Meshes;
using System.Collections.Generic;

namespace Stellamod.Content.Rendering.Abyssal;

public sealed class MoonArrowTrailRenderer : MeshRenderer<VertexPositionColorTexture>
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


        var bloom = BloomTrailShader.Instance;
        bloom.InnerColor = Color.LightBlue * 0.5f;
        bloom.OuterColor = Color.DarkBlue;
        bloom.Apply();
        DrawUtilities.DrawUserIndexedPrimitivesWithEffect(arr2, indices, bloom);

        var shader2 = RichLaserShader.Instance;
        shader2.LaserColor = Color.White;
        shader2.LaserTexture = TrailRegistry.StarTrail;
        shader2.InnerColor = Color.Turquoise * 0.5f;
        shader2.OuterColor = Color.DarkTurquoise;
        shader2.Apply();
        DrawUtilities.DrawUserIndexedPrimitivesWithEffect(arr, indices, shader2);

     
        _verticesToRenderer.Clear();
        _bigVerticesToRender.Clear();
    }
}