using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Rendering.RTs;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.EnemiesMT;

[Autoload(Side = ModSide.Client)]
public class PearlbornSlimeRenderer : ModSystem
{

    private VertexPositionColorTexture[] _vertices;
    private short[] _indices;

    private VertexPositionColorTexture[] _outlineVertices;
    private short[] _outlineIndices;
    public record struct SlimeDrawData(Vector2[] Points, Func<float, Color> GetTrailColor, Func<float, float> GetTrailWidth, Vector2 TrailOffset);
    static readonly Queue<Action<SpriteBatch>> _spriteDrawQueue = new();
    static readonly Queue<SlimeDrawData> _drawQueue = new();

    static readonly Queue<Action<SpriteBatch>> _spriteOutlineDrawQueue = new();
    static readonly Queue<SlimeDrawData> _outlineDrawQueue = new();

   
    public override void Load()
    {
        base.Load();
        On_Main.DrawPlayers_AfterProjectiles += DrawSlime;
    }

    private void Batch()
    {
        var batch = new List<VertexPositionColorTexture>();
        while (_drawQueue.Count > 0)
        {
            var drawData = _drawQueue.Dequeue();
            var vertices = DrawUtilities.PrepareSimpleTrailing(drawData.Points, drawData.GetTrailColor, drawData.GetTrailWidth, drawData.TrailOffset);
            batch.AddRange(vertices);
        }
        _vertices = batch.ToArray();
        _indices = DrawUtilities.PrepareIndicesForDrawing(batch.Count / 4);
    }

    void BatchOutline()
    {
        var batch = new List<VertexPositionColorTexture>();
        while (_outlineDrawQueue.Count > 0)
        {
            var drawData = _outlineDrawQueue.Dequeue();
           var outlineVertices = DrawUtilities.PrepareSimpleTrailing(drawData.Points, drawData.GetTrailColor, drawData.GetTrailWidth, drawData.TrailOffset);
            batch.AddRange(outlineVertices);
        }
        _outlineVertices = batch.ToArray();
        _outlineIndices = DrawUtilities.PrepareIndicesForDrawing(batch.Count / 4);
    }

    void DrawOutlines(SpriteBatch sb)
    {

        BatchOutline();
        using var slimeTarget = RT.Context(RenderTargets.ScreenTarget);
        using var pixelSlimeTarget = RT.Context(RenderTargets.ScreenTarget);
        using (RT.Clear(pixelSlimeTarget, Color.Transparent))
        {
            //May need to render this to a render target first so we can outline it, will see how it looks first.
            var pass = AssetReferences.Effects.Generic.WhiteTrail.CreatePrimitivesPass();
            pass.Parameters.time = Main.GlobalTimeWrappedHourly;
            pass.Parameters.spriteSampler = new()
            {
                Sampler = SamplerState.PointWrap,
                Texture = AssetReferences.Content.Areas.Tundra.MoonspiralTower.EnemiesMT.PearlbornSlime_Spike.Asset.Value
            };
            pass.Parameters.noiseSampler = new()
            {
                Sampler = SamplerState.PointWrap,
                Texture = AssetReferences.Assets.LaserTextures.FlameTrail.Asset.Value
            };
            pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
            pass.Apply();
            sb.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            //Now we just need to make and draw the shader
            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(_outlineVertices, _outlineIndices, pass.Shader);
        }

        using (RT.Clear(slimeTarget, Color.Transparent))
        {
            using (sb.Ctx(SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.Identity }))
            {
                while (_spriteOutlineDrawQueue.Count > 0)
                {
                    _spriteOutlineDrawQueue.Dequeue()(sb);
                }
            }

            var pixelattePass = AssetReferences.Effects.CrystalShaders.Pixelate.CreatePixelPass();
            pixelattePass.Parameters.width = pixelSlimeTarget.Width / 2;
            pixelattePass.Parameters.height = pixelSlimeTarget.Height / 2;
            pixelattePass.Apply();

            sb.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                Main.Rasterizer,
                pixelattePass.Shader);
            sb.Draw(pixelSlimeTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            sb.End();
        }

        OutlineShader outlineShader = OutlineShader.Instance;
        Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight) * 2;
        outlineShader.TexelSize = texelSize;
        sb.Begin(SpritebatchParams.InWorldAndZoomed() with { effect = outlineShader });
        sb.Draw(slimeTarget, Vector2.Zero, Color.White);
        sb.End();
    }

    private void DrawSlime(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
    {
        orig(self);
        if (Main.gameMenu)
            return;

        if (_drawQueue.Count <= 0 && _spriteDrawQueue.Count <= 0)
            return;

        Batch();


        var sb = Main.spriteBatch;
        using var slimeTarget = RT.Context(RenderTargets.ScreenTarget);
        using var pixelSlimeTarget = RT.Context(RenderTargets.ScreenTarget);
        using (RT.Clear(pixelSlimeTarget, Color.Transparent))
        {
            //May need to render this to a render target first so we can outline it, will see how it looks first.
            var pass = AssetReferences.Effects.Generic.SlimeTrail.CreatePrimitivesPass();
            pass.Parameters.time = Main.GlobalTimeWrappedHourly;
            pass.Parameters.spriteSampler = new()
            {
                Sampler = SamplerState.PointWrap,
                Texture = AssetReferences.Content.Areas.Tundra.MoonspiralTower.EnemiesMT.PearlbornSlime_Spike.Asset.Value
            };
            pass.Parameters.noiseSampler = new()
            {
                Sampler = SamplerState.PointWrap,
                Texture = AssetReferences.Assets.LaserTextures.FlameTrail.Asset.Value
            };
            pass.Parameters.transformMatrix = TrailDrawer.WorldViewPoint2;
            pass.Apply();
            sb.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            //Now we just need to make and draw the shader
            DrawUtilities.DrawUserIndexedPrimitivesWithEffect(_vertices, _indices, pass.Shader);
        }

        using (RT.Clear(slimeTarget, Color.Transparent))
        {
            sb.Begin(SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.Identity });
            while (_spriteDrawQueue.Count > 0)
            {
                _spriteDrawQueue.Dequeue()(sb);
            }
            sb.End();

            var pixelattePass = AssetReferences.Effects.CrystalShaders.Pixelate.CreatePixelPass();
            pixelattePass.Parameters.width = pixelSlimeTarget.Width / 2;
            pixelattePass.Parameters.height = pixelSlimeTarget.Height / 2;
            pixelattePass.Apply();

            sb.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                Main.Rasterizer,
                pixelattePass.Shader);
            sb.Draw(pixelSlimeTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            sb.End();
        }

        var color = new Color(21, 4, 206);
        var outliner = AssetReferences.Effects.Generic.OutlinerNoTransparencyThreshold.CreatePixelPass();
        outliner.Parameters.texelSize = slimeTarget.Target.GetTexelSize() * 2f;
        outliner.Parameters.threshold = 0;
        outliner.Apply();
        sb.Begin(SpritebatchParams.InWorldAndZoomed() with { effect = outliner.Shader });
        sb.Draw(slimeTarget, Vector2.Zero, color);
        sb.End();



        if (_outlineDrawQueue.Count > 0)
        {
            DrawOutlines(sb);
            _outlineDrawQueue.Clear();
        }
    }

    public static void PrepareForRenderingSpriteOutline(Action<SpriteBatch> drawAction)
    {
        _spriteOutlineDrawQueue.Enqueue(drawAction);
    }

    public static void PrepareForRenderingOutline(SlimeDrawData drawData)
    {
        _outlineDrawQueue.Enqueue(drawData);
    }

    public static void PrepareForRenderingSprite(Action<SpriteBatch> drawAction)
    {
        _spriteDrawQueue.Enqueue(drawAction);
    }

    public static void PrepareForRendering(SlimeDrawData drawData)
    {
        _drawQueue.Enqueue(drawData);
    }
}

