using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Assets.ContentReader.Pal;
using Stellamod.Common.GooberDialogue;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Palettes;
using Stellamod.Core.Rendering.RTs;
using Stellamod.Effects.RoyalMagic;

using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Common.DialogueTowning;

/// <summary>
/// Used to draw over the dialogue box, any custom styles should be implemented here
/// </summary>
public interface IBoxStyle
{
    void Render(RenderTargetHandle output, SpriteBatch spriteBatch, Quad<VertexPositionColorTexture> quad);
}


public struct ZuiDialogueStyle : IBoxStyle
{
    private Color SunLineColor
    {
        get
        {
            var color = new Color(255, 173, 89);
            color = Color.Lerp(color, Color.White, 0.15f);
            color = Color.Lerp(color, Color.Orange, 0.15f);
            return color;
        }
    }
    private Color SunOutlineColor
    {
        get
        {
            var color = new Color(255, 173, 89);
            color = Color.Lerp(color, Color.White, 0.15f);
            color = Color.Lerp(color, Color.OrangeRed, 1f);
            color = Color.Lerp(color, Color.White, 0.15f);
            return color;
        }
    }
    private void DrawV1(RenderTargetHandle output, SpriteBatch spriteBatch, Quad<VertexPositionColorTexture> quad)
    {
        Color outlineColor = new Color(179, 45, 11);
        outlineColor = Color.Lerp(outlineColor, Color.Black, 0.3f);

        RenderTargetHandle target1 = RenderTargets.ScreenTarget;
        RenderTargetHandle target2 = RenderTargets.ScreenTarget;
        RenderTargetHandle pixelTarget = RenderTargets.QuarterScreenTarget;
        //Here we can assume we're already drawing to the box render target
        //so let's just do whatever we awnt
        //gonna draw a big glow ball to test
        var topLeft = quad.vertices[0].Position.XY();
        var topRight = quad.vertices[1].Position.XY();
        var bottomRight = quad.vertices[3].Position.XY();
        var rect = ExtraMath.CreateRectangle(topLeft, bottomRight);

        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.NoiseTextures.JaggedWaves.Asset, quad.vertices[1].Position.XY());
        drawer.worldPosition = topRight;
        drawer.worldPosition.X += 242;
        drawer.worldPosition += new Vector2(-1, 1).SafeNormalize(Vector2.Zero) * 96;
        drawer.worldPosition += Main.screenPosition;
        drawer.VerticalFrame(0, 2);
        drawer.CenterOrigin();


        rect.Width += 242;
        rect.Height += 48;
        rect.Y += 2;
        rect = rect.CenterPad(-4);


        using (new RenderTargetContext(target1))
        {
            var color = new Color(179, 44, 11);
            color = Color.Lerp(color, Color.Black, 0.6f);

            using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity, blendState = BlendState.Additive }))
            {
                var num = 32;
                for (float f = 0; f < num; f++)
                {      
                    float time = Main.GlobalTimeWrappedHourly * 0.15f;
                    time += (f / num) * 3f;
                    time %= 3f;

                    var circleDrawer = drawer;
                    var ratio = time / 3f;
                    circleDrawer.color = color * 0.5f;
                    circleDrawer.scale *= time;
                    circleDrawer.rotation = f;
                    spriteBatch.Draw(circleDrawer);
                }

                float t = 8;
                for (float f = 0; f < num; f++)
                {

                    float time = Main.GlobalTimeWrappedHourly * 0.15f;
                    time += (f / num) * t;
                    time %= t;

                    var circleDrawer = drawer;
                    var ratio = time / t;
                    circleDrawer.color = color * 0.47f * EasingFunction.QuadraticBump(ratio);
                    circleDrawer.scale *= time;
                    circleDrawer.rotation = f;
                    spriteBatch.Draw(circleDrawer);
                }
            }
        }
        using (new RenderTargetContext(pixelTarget))
        {
            var zuiWaves = AssetReferences.Effects.Dialogue.ZuiWaves.CreatePixelPass();
            zuiWaves.Parameters.time = Main.GlobalTimeWrappedHourly;
            zuiWaves.Parameters.texelSize = target1.Target.GetTexelSize() * 4;
            zuiWaves.Apply();
            using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity, effect = zuiWaves.Shader }))
            {
                spriteBatch.Draw(target1, Vector2.Zero, null, outlineColor * 0.7f, 0, Vector2.Zero, 0.25f, SpriteEffects.None, 0);
            }
        }

        //output gradient and whatnot
        using(new RenderTargetContext2(output, Color.Transparent))
        {
            using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity }))
            {
                spriteBatch.Draw(pixelTarget, Vector2.Zero, null, Color.White, 0, Vector2.zeroVector, 4, SpriteEffects.None, 0);
            }

        }


        using (new RenderTargetContext(target2))
        {

            var sunpass = AssetReferences.Effects.Dialogue.ZuiSun.CreatePixelPass();
            sunpass.Parameters.time = Main.GlobalTimeWrappedHourly;
            sunpass.Apply();
            using (new SpritebatchContext(spriteBatch, 
                SpritebatchParams.InWorldAndZoomed() with {
                    matrix = Matrix.identity, effect = sunpass.Shader }))
            {
                var sunDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.UI.Dialogue.ZuiSunSwirl.Asset, Vector2.Zero);
                sunDrawer.worldPosition = drawer.worldPosition;
                sunDrawer.color = Color.White;
                sunDrawer.scale *= ExtraMath.Osc(0.9f, 1.1f) * 0.9f;

                var backGlowDrawer = sunDrawer;
                backGlowDrawer.scale *= 1.5f;
                backGlowDrawer.color *= 0.09f;
                spriteBatch.Draw(backGlowDrawer);


                spriteBatch.Draw(sunDrawer);
            }
        }
        using (new RenderTargetContext(pixelTarget))
        {
            using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity }))
            {
                spriteBatch.Draw(target2, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.25f, SpriteEffects.None, 0);
            }
        }
        using (new RenderTargetContext2(output))
        {
            var paletteShader = PalettizerShader.Use(PaletteAssets.ZUISUN);
            paletteShader.DitherAlpha = 0f;
            using (new SpritebatchContext(spriteBatch,
                SpritebatchParams.InWorldAndZoomed() with
                {
                    matrix = Matrix.identity,
                    effect = paletteShader.Effect,
                    blendState = BlendState.Additive
                }))
            {
                spriteBatch.Draw(pixelTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 4, SpriteEffects.None, 0);
            }
        }


        
        using (new RenderTargetContext(target1))
        {
            using (new SpritebatchContext(spriteBatch, 
                SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity }))
            {
                var sunDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.UI.Dialogue.ZuiSunSwirl.Asset, Vector2.Zero);
                sunDrawer.worldPosition = drawer.worldPosition;
                sunDrawer.color = Color.White;

               // spriteBatch.Draw(sunDrawer);

                var sunOutlineColor = Color.White;
                DrawUtilities.DrawOutlinedRectangle(spriteBatch, rect, sunOutlineColor, 16);
            }
            var paletteShader = PalettizerShader.Use(PaletteAssets.ZUISUN);
            paletteShader.DitherAlpha = 0f;
            using (new SpritebatchContext(spriteBatch,
                SpritebatchParams.InWorldAndZoomed() with
                {
                    matrix = Matrix.identity
                }))
            {
                spriteBatch.Draw(target2, Vector2.Zero, Color.White);
            }
        }
        

        using (new RenderTargetContext2(output))
        {


            var zuiSwirlPass = AssetReferences.Effects.Dialogue.ZuiSunSwirl.CreatePixelPass();
            zuiSwirlPass.Parameters.time = Main.GlobalTimeWrappedHourly * 2f;
            zuiSwirlPass.Parameters.texelSize = AssetReferences.Assets.Textures.UI.Dialogue.ZuiSunLineLong.Asset.Value.GetTexelSize();
            zuiSwirlPass.Apply();
            using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with {
                matrix = Matrix.identity, 
                blendState = BlendState.AlphaBlend,
                effect = zuiSwirlPass.Shader}))
            {
                foreach (PositionVelocity posVel in new CircleOrientation(drawer.worldPosition, 420, 10))
                {
                    var swirl = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.UI.Dialogue.ZuiSunLineShort.Asset, Vector2.Zero);
                    var rotatedPos = posVel.position.RotatedBy(Main.GlobalTimeWrappedHourly * 0.25f + 0.28f, drawer.worldPosition);
                    swirl.worldPosition = rotatedPos;
                    swirl.color = SunLineColor;
                    swirl.scale.Y *= 4;
                    swirl.rotation = (rotatedPos - drawer.worldPosition).ToRotation() + MathHelper.PiOver2;
                    spriteBatch.Draw(swirl);
                }
                foreach (PositionVelocity posVel in new CircleOrientation(drawer.worldPosition, 217, 10))
                {
                    var swirl = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.UI.Dialogue.ZuiSunLineLong.Asset, Vector2.Zero);
                    var rotatedPos = posVel.position.RotatedBy(Main.GlobalTimeWrappedHourly * 0.25f, drawer.worldPosition);
                    swirl.worldPosition = rotatedPos;
                    swirl.color = SunLineColor;
                    swirl.rotation = (rotatedPos - drawer.worldPosition).ToRotation() + MathHelper.PiOver2;
                    spriteBatch.Draw(swirl);
                }
        
  
  
            }

            using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity, }))
            {

                var sunOutlineColor = Color.White;
                DrawUtilities.DrawOutlinedRectangle(spriteBatch, rect, sunOutlineColor, 16);
            }
            
            var outlineOnly = AssetReferences.Effects.Generic.OutlineOnly.CreatePixelPass();
            outlineOnly.Parameters.texelSize = target1.Target.GetTexelSize() * 6;
            outlineOnly.Apply();
            using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with {
                matrix = Matrix.identity, effect = outlineOnly.Shader }))
            {
                spriteBatch.Draw(target1, Vector2.Zero, null, SunOutlineColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
        }
    }
    private void DrawV2(RenderTargetHandle output, SpriteBatch spriteBatch, Quad<VertexPositionColorTexture> quad)
    {
        var zuiWaves = AssetReferences.Effects.Dialogue.ZuiWaves.CreatePixelPass();
        zuiWaves.Parameters.time = Main.GlobalTimeWrappedHourly;
        zuiWaves.Apply();

        using (new RenderTargetContext(output))
        {

            using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity, effect = zuiWaves.Shader }))
            {
                Color outlineColor = new Color(179, 45, 11);

                var pos = quad.vertices[1].Position.XY();
                pos.X -= 166;
                var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.NoiseTextures.SlashTrail1.Asset, pos);
                drawer.worldPosition += Main.screenPosition;
                drawer.color = outlineColor;
                drawer.scale *= 6;
                spriteBatch.Draw(drawer);
            }
        }

    }
    public void Render(RenderTargetHandle output, SpriteBatch spriteBatch, Quad<VertexPositionColorTexture> quad)
    {
        DrawV1(output, spriteBatch, quad);
       
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
        if (parameters.profile.boxStyle != null)
        {
            parameters.profile.boxStyle.Render(boxRenderTargetSwap, Main.spriteBatch, _squareQuad);
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
