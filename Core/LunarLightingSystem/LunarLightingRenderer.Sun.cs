using Stellamod.Common.Shaders;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem;

public static class LightingHelper
{
    /// <summary>
    /// Interpolates between 0-1 near the end of a day/night cycle, to make the transition a bit sooner
    /// </summary>
    public static float DayLightEase
    {
        get
        {
            float easingTime = 2400;
            float dayLength = (float)Main.dayLength;
            if (!Main.dayTime)
            {
                dayLength = (float)Main.nightLength;
            }

            float inTime = (float)Main.time;
            float inEasing = EasingFunction.InOutSine(inTime / easingTime);
            float outTime = (float)Main.time;
            float outDown = outTime - (dayLength - easingTime);
            float outEasing = EasingFunction.InOutSine(outDown / easingTime);
            float a = inEasing * MathHelper.Lerp(1f, 0f, outEasing);
            return a;
        }
    }
}

public partial class LunarLightingRenderer
{
    public Color GetSunColor()
    {
        Color[] sunColors = new Color[]
        {
                new Color(8, 79, 126).Towards(Color.White, 0.5f),
              Color.SkyBlue,

                new Color(255, 173, 63),
                   new Color(255, 173, 63),
                            new Color(255, 173, 63),
                                     new Color(255, 173, 63),
                                        new Color(255, 173, 63),



                Color.White,
               Color.White,
                    Color.White,
                         Color.White,
                              Color.White,
                Color.White,
               Color.White,
                    Color.White,
                         Color.White,
                              Color.White,

                new Color(255, 173, 63),
               new Color(255, 173, 63),
                        new Color(255, 173, 63),
                                 new Color(255, 173, 63),
                                    new Color(255, 173, 63),
                 Color.SkyBlue,
                new Color(8, 79, 126).Towards(Color.White, 0.5f),
        };

        float dayProgress = Main.dayTime ? (float)Main.time / (float)Main.dayLength : (float)Main.time / (float)Main.nightLength;
        Color interpolatedColor = DrawUtilities.InterpolateColorArray(dayProgress, sunColors);
        if (!Main.dayTime)
            interpolatedColor = sunColors[0];
        if (!Main.LocalPlayer.ZoneOverworldHeight && !Main.LocalPlayer.ZoneSkyHeight)
            interpolatedColor = SmoothedBackLightColor;
        if (ModContent.GetInstance<DomainExpansionManager>().hoveringPlatform)
            interpolatedColor = Color.White;
        return interpolatedColor;
    }

    private void RenderSunLight()
    {
        if (!Lighting.UsingNewLighting)
            return;

        Vector2 stepSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight);
        stepSize *= 4 * -SunLightManager.ShadowDirection;

        var shader = ShaderContent.GetInstance<SunLightShader>();
        shader.StepSize = stepSize;
        shader.ShadowAlpha = LightingHelper.DayLightEase;

        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
            DepthStencilState.None, RasterizerState.CullNone, shader.Effect, Main.GameViewMatrix.TransformationMatrix);


        Vector2 drawPosition = Vector2.Zero;
        spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition, null,
           SunColor, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);

        spriteBatch.End();
    }

    private void RenderShadows()
    {
        if (Main.gameMenu)
            return;
        if (!Lighting.UsingNewLighting)
            return;


        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;

        graphicsDevice.SetRenderTarget(_tileBlurRT);
        graphicsDevice.Clear(Color.Transparent);

        Effect effect = GameShaders.Misc["LunarVeil:SunShadow"].Shader;
        effect.Parameters["mipBias"].SetValue(0.1f);

        Vector2 sunDirection = SunLightManager.ShadowDirection.SafeNormalize(Vector2.Zero);
        effect.Parameters["sunDirection"].SetValue(-sunDirection * 1400);
        effect.Parameters["falloff"].SetValue(0.1f);
        effect.Parameters["uScreenResolution"].SetValue(Main.ScreenSize.ToVector2());
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, effect);
        spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition, null, Color.Black * 0.9f * LightingHelper.DayLightEase, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
        spriteBatch.End();


        graphicsDevice.SetRenderTarget(_tileSunShadowRT);
        graphicsDevice.Clear(Color.Transparent);
        Effect blurEffect = GameShaders.Misc["LunarVeil:SunBlur"].Shader;
        blurEffect.Parameters["mipBias"].SetValue(12);
        blurEffect.Parameters["uScreenResolution"].SetValue(Main.ScreenSize.ToVector2());
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, blurEffect);
        spriteBatch.Draw(_tileBlurRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
        spriteBatch.End();
    }
}
