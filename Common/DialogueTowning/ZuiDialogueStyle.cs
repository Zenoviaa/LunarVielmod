using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Palettes;
using Stellamod.Core.Rendering.RTs;

using Terraria;

namespace Stellamod.Common.DialogueTowning;

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

        using var target1 = RT.Context(RenderTargets.ScreenTarget);
        using var target2 = RT.Context(RenderTargets.ScreenTarget);
        using var pixelTarget = RT.Context(RenderTargets.QuarterScreenTarget);
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


        using (RT.Clear(target1, Color.Transparent))
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
        using (RT.Clear(pixelTarget,Color.Transparent))
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
        using(RT.Clear(output, Color.Transparent))
        {
            using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity }))
            {
                spriteBatch.Draw(pixelTarget, Vector2.Zero, null, Color.White, 0, Vector2.zeroVector, 4, SpriteEffects.None, 0);
            }

        }


        using (RT.Clear(target2, Color.Transparent))
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
        using (RT.Clear(pixelTarget, Color.Transparent))
        {
            using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity }))
            {
                spriteBatch.Draw(target2, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.25f, SpriteEffects.None, 0);
            }
        }
        using (RT.Clear(output))
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


        
        using (RT.Clear(target1, Color.Transparent))
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
        

        using (RT.Clear(output))
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

    public void Render(RenderTargetHandle output, SpriteBatch spriteBatch, Quad<VertexPositionColorTexture> quad)
    {
        DrawV1(output, spriteBatch, quad);
    }

    public void RenderMini(RenderTargetHandle output, SpriteBatch spriteBatch, Quad<VertexPositionColorTexture> quad)
    {
        Color outlineColor = new Color(179, 45, 11);
        outlineColor = Color.Lerp(outlineColor, Color.Black, 0.3f);

        using var target1 = RT.Context(RenderTargets.ScreenTarget);
        using var target2 = RT.Context(RenderTargets.ScreenTarget);
        using var pixelTarget = RT.Context(RenderTargets.HalfScreenTarget);

        //Here we can assume we're already drawing to the box render target
        //so let's just do whatever we awnt
        //gonna draw a big glow ball to test
        var topLeft = quad.vertices[0].Position.XY();
        var topRight = quad.vertices[1].Position.XY();
        var bottomRight = quad.vertices[3].Position.XY();
        var rect = ExtraMath.CreateRectangle(topLeft, bottomRight);

        var drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.NoiseTextures.JaggedWaves.Asset, quad.vertices[1].Position.XY());
        drawer.worldPosition = topRight;
        drawer.worldPosition += Main.screenPosition;
        drawer.VerticalFrame(0, 2);
        drawer.CenterOrigin();

        using (RT.Clear(target1, Color.Transparent))
        {
            var color = new Color(179, 44, 11);
            color = Color.Lerp(color, Color.Black, 0.6f);

            using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity, blendState = BlendState.Additive }))
            {
                var scale = 0.5f;
                var num = 32;
                for (float f = 0; f < num; f++)
                {
                    float time = Main.GlobalTimeWrappedHourly * 0.15f;
                    time += (f / num) * 3f;
                    time %= 3f;

                    var circleDrawer = drawer;
                    var ratio = time / 3f;
                    circleDrawer.color = color * 0.5f;
                    circleDrawer.scale *= time * scale;
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
                    circleDrawer.scale *= time * scale;
                    circleDrawer.rotation = f;
                    spriteBatch.Draw(circleDrawer);
                }
            }
        }

        using (RT.Clear(pixelTarget, Color.Transparent))
        {
            var zuiWaves = AssetReferences.Effects.Dialogue.ZuiWaves.CreatePixelPass();
            zuiWaves.Parameters.time = Main.GlobalTimeWrappedHourly;
            zuiWaves.Parameters.texelSize = target1.Target.GetTexelSize() * 4;
            zuiWaves.Apply();
            using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity, effect = zuiWaves.Shader }))
            {
                spriteBatch.Draw(target1, Vector2.Zero, null, outlineColor * 0.7f, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            }
        }

        //output gradient and whatnot
        using (RT.Clear(output, Color.Transparent))
        {
            using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity }))
            {
                spriteBatch.Draw(pixelTarget, Vector2.Zero, null, Color.White, 0, Vector2.zeroVector, 2, SpriteEffects.None, 0);
            }
        }

        using (RT.Clear(target2, Color.Transparent))
        {
            var sunpass = AssetReferences.Effects.Dialogue.ZuiSun.CreatePixelPass();
            sunpass.Parameters.time = Main.GlobalTimeWrappedHourly;
            sunpass.Apply();
            using (new SpritebatchContext(spriteBatch,
                SpritebatchParams.InWorldAndZoomed() with
                {
                    matrix = Matrix.identity,
                    effect = sunpass.Shader
                }))
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

        using (RT.Clear(pixelTarget, Color.Transparent))
        {
            using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity }))
            {
                spriteBatch.Draw(target2, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            }
        }

        using (RT.Clear(output))
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
                spriteBatch.Draw(pixelTarget, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
            }
        }


        using (RT.Clear(target1, Color.Transparent))
        {
            using (new SpritebatchContext(spriteBatch,
                SpritebatchParams.InWorldAndZoomed() with { matrix = Matrix.identity }))
            {
                var sunDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.Textures.UI.Dialogue.ZuiSunSwirl.Asset, Vector2.Zero);
                sunDrawer.worldPosition = drawer.worldPosition;
                sunDrawer.color = Color.White;
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


        using (RT.Clear(output))
        {
            var zuiSwirlPass = AssetReferences.Effects.Dialogue.ZuiSunSwirl.CreatePixelPass();
            zuiSwirlPass.Parameters.time = Main.GlobalTimeWrappedHourly * 2f;
            zuiSwirlPass.Parameters.texelSize = AssetReferences.Assets.Textures.UI.Dialogue.ZuiSunLineLong.Asset.Value.GetTexelSize();
            zuiSwirlPass.Apply();
            using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with
            {
                matrix = Matrix.identity,
                blendState = BlendState.AlphaBlend,
                effect = zuiSwirlPass.Shader
            }))
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

            var outlineOnly = AssetReferences.Effects.Generic.OutlineOnly.CreatePixelPass();
            outlineOnly.Parameters.texelSize = target1.Target.GetTexelSize() * 6;
            outlineOnly.Apply();
            using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with
            {
                matrix = Matrix.identity,
                effect = outlineOnly.Shader
            }))
            {
                spriteBatch.Draw(target1, Vector2.Zero, null, SunOutlineColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
        }
    }
}
