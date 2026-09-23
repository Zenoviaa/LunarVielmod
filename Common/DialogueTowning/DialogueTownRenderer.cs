using ReLogic.Content;
using Stellamod.Common.GooberDialogue;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Rendering.RTs;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Common.DialogueTowning;

/// <summary>
/// Used to draw over the dialogue box, any custom styles should be implemented here
/// </summary>
public interface IBoxStyle
{
    void Render(SpriteBatch spriteBatch, Quad<VertexPositionColorTexture> quad);
}


public struct ZuiDialogueStyle : IBoxStyle
{
    public void Render(SpriteBatch spriteBatch, Quad<VertexPositionColorTexture> quad)
    {
        //Here we can assume we're already drawing to the box render target
        //so let's just do whatever we awnt
        //gonna draw a big glow ball to test
        var topLeft = quad.vertices[0].Position.XY();
        var bottomRight = quad.vertices[3].Position.XY();
        var rect = ExtraMath.CreateRectangle(topLeft, bottomRight);

        var zuiWaves = AssetReferences.Effects.Dialogue.ZuiWaves.CreatePixelPass();
        zuiWaves.Parameters.time = Main.GlobalTimeWrappedHourly;
        zuiWaves.Apply();
        using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity, effect = zuiWaves.Shader }))
        {
            var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.NoiseTextures.JaggedWaves.Asset, quad.vertices[1].Position.XY());
            drawer.dstRect = rect;
            drawer.drawOrigin = Vector2.Zero;
            drawer.color = Color.White;
            spriteBatch.Draw(drawer);
        }

        /*
        using(new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity }))
        {
            var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, quad.vertices[1].Position.XY());
            drawer.worldPosition += Main.screenPosition;
            drawer.worldPosition.Y += 128;
            drawer.color = Color.White;
            drawer.color.A = 0;
            drawer.scale *= 2.0f;
            spriteBatch.Draw(drawer);
        }
        */
        //       throw new System.NotImplementedException();
    }
}
public struct SpeechBoxTalkingParameters
{
    public GooberProfile profile;
    public Vector2 size;
    public string text;
}


public static class DialogueTownRenderer
{
    //The bubble is renderered slightly off cause the edge points of the box are distorted
    private static Vector2 BubbleOffset => new Vector2(64);
    private static readonly Quad<VertexPositionColorTexture> _squareQuad = new();
    private static void PrepareQuad(Vector2 anchorPoint, Vector2 size, Color startColor, Color endColor)
    {
        float yRange = 4;
        Vector3 topLeftOffset = new Vector3();
    //    topLeftOffset.X = ExtraMath.Osc(-16f, 16f, speed: 1);
   //     topLeftOffset.Y = ExtraMath.Osc(-yRange, yRange, speed: 1);

        Vector3 topRightOffset = new Vector3();
     //   topRightOffset.X = ExtraMath.Osc(-16f, 16f, speed: 1);
    //    topRightOffset.Y = ExtraMath.Osc(-yRange, yRange, speed: 1, offset: 3.14f);

        Vector3 bottomLeftOffset = new Vector3();
    //    bottomLeftOffset.X = ExtraMath.Osc(-16f, 16f, speed: 1, offset: 3.14f);
    //    bottomLeftOffset.Y = ExtraMath.Osc(-yRange, yRange, speed: 1);

        Vector3 bottomRightOffset = new Vector3();
    //    bottomRightOffset.X = ExtraMath.Osc(-16f, 16f, speed: 1, offset: 3.14f);
   //     bottomRightOffset.Y = ExtraMath.Osc(-yRange, yRange, speed: 1, offset: 3.14f);

        //Top Left
        _squareQuad.vertices[0] = new VertexPositionColorTexture(
            new Vector3(anchorPoint.X, anchorPoint.Y, 0) + topLeftOffset, startColor, Vector2.Zero);

        //Top Right
        _squareQuad.vertices[1] = new VertexPositionColorTexture(
            new Vector3(anchorPoint.X + size.X, anchorPoint.Y , 0) + topRightOffset, endColor, new Vector2(1, 0));

        //Bottom Left
        _squareQuad.vertices[2] = new VertexPositionColorTexture(
            new Vector3(anchorPoint.X , anchorPoint.Y + size.Y, 0) + bottomLeftOffset, startColor, new Vector2(0, 1));

        //Bottom Right
        _squareQuad.vertices[3] = new VertexPositionColorTexture(
            new Vector3(anchorPoint.X + size.X , anchorPoint.Y + size.Y , 0) + bottomRightOffset, endColor, Vector2.One);
    }

    private static void DrawOutline(RenderTargetHandle src, RenderTargetHandle dst, SpriteBatch spriteBatch, Effect effect, Color outlineColor)
    {
        using (new RenderTargetContext(dst))
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                effect);
            spriteBatch.Draw(src, Vector2.Zero, outlineColor);
            spriteBatch.End();
        }
    }


    private static void RenderDialogueBoxToPixelTarget(in SpeechBoxTalkingParameters parameters,
        RenderTargetHandle pixelTarget, RenderTargetHandle boxRenderTarget, RenderTargetHandle boxRenderTargetSwap)
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        using (new RenderTargetContext(boxRenderTarget))
        {
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            HlslSampler noiseSpriteSampler = new();
            noiseSpriteSampler.Texture = AssetReferences.Assets.Noise.PerlinBlurred.Asset.Value;
            noiseSpriteSampler.Sampler = SamplerState.PointClamp;

            var pass = AssetReferences.Effects.Generic.Square.CreatePrimitivesPass();
            pass.Parameters.transformMatrix = TrailDrawer.ViewProjection;
            pass.Parameters.time = Main.GlobalTimeWrappedHourly;
            pass.Parameters.spriteSampler = noiseSpriteSampler;
            pass.Apply();
            pass.Shader.CurrentTechnique.Passes[0].Apply();
            PrepareQuad(
                BubbleOffset,
                parameters.size,
                parameters.profile.startGradientColor * 0.8f
                , parameters.profile.endGradientColor * 0.8f);
            _squareQuad.Draw();
        }

        HlslSampler spriteSampler = new();
        spriteSampler.Texture = boxRenderTarget;
        spriteSampler.Sampler = SamplerState.PointClamp;

        var texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight) * 2;
        var outlinerPass = AssetReferences.Effects.Generic.Outliner.CreatePixelPass();

        outlinerPass.Parameters.spriteSampler = spriteSampler;
        outlinerPass.Parameters.texelSize = texelSize;
        outlinerPass.Apply();
        DrawOutline(boxRenderTarget, boxRenderTargetSwap, spriteBatch, outlinerPass.Shader, Color.White);
        DrawOutline(boxRenderTargetSwap, boxRenderTarget, spriteBatch, outlinerPass.Shader, Color.White);
        DrawOutline(boxRenderTarget, boxRenderTargetSwap, spriteBatch, outlinerPass.Shader, parameters.profile.outlineColor);
        DrawOutline(boxRenderTargetSwap, boxRenderTarget, spriteBatch, outlinerPass.Shader, parameters.profile.outlineColor);

        var noisePass = AssetReferences.Effects.Generic.Scroll.CreatePixelPass();
        noisePass.Parameters.time = Main.GlobalTimeWrappedHourly * 4;

        //Now we'll use the box render target as a mask and draw whatever the heck we want
        using(new RenderTargetContext(boxRenderTargetSwap))
        {
            if(parameters.profile.boxStyle != null)
            {
                parameters.profile.boxStyle.Render(Main.spriteBatch, _squareQuad);
            }
        }



        using (new RenderTargetContext(pixelTarget))
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone);
            spriteBatch.Draw(boxRenderTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            spriteBatch.End();

            //now draw using mask combine
            var maskCombine = AssetReferences.Effects.CrystalShaders.MaskCombine.CreatePixelPass();
            maskCombine.Parameters.mixTexture = boxRenderTargetSwap;
            maskCombine.Apply();
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                maskCombine.Shader);
            spriteBatch.Draw(boxRenderTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }

    public static RenderTarget2D RenderSpeechWindow(in SpeechBoxTalkingParameters parameters)
    {
        RenderTargetHandle pixelTarget = RenderTargets.HalfScreenTarget;
        RenderTargetHandle boxRenderTarget = RenderTargets.ScreenTarget;
        RenderTargetHandle boxRenderTargetSwap = RenderTargets.ScreenTarget;
        RenderDialogueBoxToPixelTarget(parameters, pixelTarget, boxRenderTarget, boxRenderTargetSwap);
        return pixelTarget;
    }
}
