using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Rendering;

public abstract class AbstractMaterial : IDrawBatch
{
    public bool Sent { get; set; }
    public DrawLayer DrawLayer { get; set; }

    /// <summary>
    /// Returns whether this material needs to be flushed and drawn to the screen
    /// </summary>
    /// <returns></returns>
    public abstract bool NeedsFlushing();

    /// <summary>
    /// Flushes all vertices and draws to the screen
    /// </summary>
    /// <param name="graphicsDevice"></param>
    public abstract void Flush(GraphicsDevice graphicsDevice);
}

public abstract class Material<T, U, V> : AbstractMaterial
    where T : struct, IVertexType
    where U : BaseShader, new()
    where V : Material<T, U, V>, new()
{
    private static V? _instance;
    private readonly List<T> _vertices = new List<T>(capacity: 100);
    public Material()
    {

    }

    public abstract void SetShaderParameters(U shader);
    public void AddVertices(IEnumerable<T> vertices)
    {
        _vertices.AddRange(vertices);
    }

    public void Clear()
    {
        _vertices.Clear();
    }

    public override bool NeedsFlushing() => _vertices.Count > 0;
    public override void Flush(GraphicsDevice graphicsDevice)
    {
        var shader = ShaderContent.GetInstance<U>();
        SetShaderParameters(shader);
        T[] vertices = _vertices.ToArray();

        //TODO: setup index buffer before any draws happen and only update if there's not enough indices?
        //That way we don't have to prepare indices again
        int[] indices = DrawUtilities.QuadIndices(vertices.Length);

        //Since this is rendeirng all before everything else, the graphics device will be restored afterwards by the spritebatch
        //So we can set it to whatever we want
        graphicsDevice.BlendState = BlendState.AlphaBlend;
        graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        graphicsDevice.DepthStencilState = DepthStencilState.None;
        graphicsDevice.RasterizerState = RasterizerState.CullNone;

        foreach (var pass in shader.Effect.CurrentTechnique.Passes)
        {
            pass.Apply();
        }

        graphicsDevice.DrawUserIndexedPrimitives<T>(
          PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, vertices.Length / 2);

        Sent = false;
        Clear();
    }

    public static void PrepareRender(IEnumerable<T> vertices)
    {
        _instance ??= new();
        _instance.AddVertices(vertices);
        if (!_instance.Sent)
        {
            ShaderRenderPipeline.QueueBatch(_instance);
            _instance.Sent = true;
        }
    }
}
