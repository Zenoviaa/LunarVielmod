using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Rendering.RTs;

using Terraria;

namespace Stellamod.Common.DialogueTowning;


public static class DialogueTownRenderer
{
    //The bubble is renderered slightly off cause the edge points of the box are distorted
    private static Vector2 BubbleOffset => new Vector2(64);
    private static void DrawOutline(RenderTargetHandle src, RenderTargetHandle dst, SpriteBatch spriteBatch, Effect effect, Color outlineColor)
    {
        using (RT.Clear(dst, Color.Transparent))
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
        var dstRect = new Rectangle(8, 8, (int)parameters.size.X, (int)parameters.size.Y - 16);
        using (RT.Clear(boxRenderTarget, Color.Transparent))
        {
            var pass = AssetReferences.Effects.Generic.GradientSquare.CreatePixelPass();
            pass.Parameters.time = Main.GlobalTimeWrappedHourly;
            pass.Parameters.noiseSampler = new() { Texture = AssetReferences.Assets.Noise.PerlinBlurred.Asset.Value, Sampler = SamplerState.PointClamp };
            pass.Parameters.startGradientColor = (parameters.profile.startGradientColor * 0.8f).ToVector4();
            pass.Parameters.endGradientColor = (parameters.profile.endGradientColor * 0.8f).ToVector4();
            pass.Apply();

            spriteBatch.Begin(SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.Identity, effect = pass.Shader });
            var squareDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.UI.DialogueBox.Asset, Vector2.Zero);
            squareDrawer.color = Color.White;
            squareDrawer.dstRect = dstRect;
            squareDrawer.drawOrigin = Vector2.Zero;
            spriteBatch.Draw(squareDrawer);
            spriteBatch.End();

            /*
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            HlslSampler noiseSpriteSampler = new();
            noiseSpriteSampler.Texture = AssetReferences.Assets.Noise.PerlinBlurred.Asset.Value;
            noiseSpriteSampler.Sampler = SamplerState.PointClamp;

            var pass = AssetReferences.Effects.Generic.Square.CreatePrimitivesPass();
            pass.Parameters.transformMatrix = TrailDrawer.UIViewProjection;
            pass.Parameters.time = Main.GlobalTimeWrappedHourly;
            pass.Parameters.spriteSampler = noiseSpriteSampler;
            pass.Apply();
            pass.Shader.CurrentTechnique.Passes[0].Apply();
 
            _squareQuad.Draw();*/
        }

        HlslSampler spriteSampler = new();
        spriteSampler.Texture = boxRenderTarget;
        spriteSampler.Sampler = SamplerState.PointClamp;

        var texelSize = Vector2.One / new Vector2(boxRenderTarget.Width, boxRenderTarget.Height) * 2;
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
        if (parameters.profile.boxStyle != null)
        {
            parameters.profile.boxStyle.Render(boxRenderTargetSwap, Main.spriteBatch, dstRect);
        }


        using (RT.Clear(pixelTarget, Color.Transparent))
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
        using var outputTarget = RT.Context(RenderTargets.HalfDialogueBoxTarget);
        using var boxRenderTarget = RT.Context(RenderTargets.DialogueBoxTarget);
        using var boxRenderTargetSwap = RT.Context(RenderTargets.DialogueBoxTarget);
        RenderDialogueBoxToPixelTarget(parameters, outputTarget, boxRenderTarget, boxRenderTargetSwap);
        return outputTarget.Target;
    }
}
