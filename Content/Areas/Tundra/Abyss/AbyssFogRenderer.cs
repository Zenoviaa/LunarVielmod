using Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;
using Stellamod.Core;
using Stellamod.Core.LunarLightingSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss;

[Autoload(Side = ModSide.Client)]
public class AbyssFogRenderer : ModSystem
{
    private float _thickFogAlpha;
    public override void Load()
    {
        base.Load();
        On_Main.DoDraw_WallsAndBlacks += RenderAroundWalls;
        On_OverlayManager.Draw += DrawPostProcessingPasses;
    }
    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        float targetAlpha = BellFlowerSystem.RungBellFlowerCount / 2f;
        if (BellFlowerSystem.WhisperingCountdown > 0)
            targetAlpha = 0;
        _thickFogAlpha = MathHelper.Lerp(_thickFogAlpha, targetAlpha, 0.05f);
    }

    private void DrawPostProcessingPasses(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
    {
        if (layer == RenderLayers.All && beginSpriteBatch && !Main.gameMenu && LightingHelper.CanRenderPostProcessingEffects)
        {
            if (Main.LocalPlayer.GetModPlayer<MyPlayer>().ZoneAbyss)
            {
                var noiseSprite = AssetReferences.Assets.NoiseTextures.Clouds.Asset.Value;
                var ditherSprite = AssetReferences.Assets.Dithering.Dither8x8DoubleScaled.Asset.Value;
                var pass = AssetReferences.Effects.Abyss.AbyssFog.CreateBlackPass();
                HlslSampler sampler = new HlslSampler();
                sampler.Sampler = SamplerState.PointWrap;
                sampler.Texture = noiseSprite;
                pass.Parameters.spriteSampler = sampler;

                HlslSampler ditherSampler = new HlslSampler();
                ditherSampler.Sampler = SamplerState.PointWrap;
                ditherSampler.Texture = ditherSprite;
                pass.Parameters.ditherSampler = ditherSampler;

                pass.Parameters.time = Main.GlobalTimeWrappedHourly * 0.4f;
                pass.Parameters.ditherTexelSize = ditherSprite.GetTexelSize();
                pass.Parameters.spriteSize = noiseSprite.Size();
                pass.Parameters.screenOffset = DrawUtilities.CalculateScreenOffset(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight)) * 0.9f + new Vector2(Main.GlobalTimeWrappedHourly * -0.02f, 0);
                pass.Apply();
 

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, Main.Rasterizer, pass.Shader, Main.GameViewMatrix.TransformationMatrix);


                Color fogColor = Color.Lerp(Color.White, Color.Blue, 0.7f);

                float alpha = 0.23f;
                alpha += _thickFogAlpha * 0.3f;
               
                spriteBatch.Draw(noiseSprite, Vector2.Zero, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), fogColor * alpha);


                spriteBatch.Draw(noiseSprite, Vector2.Zero, new Rectangle(512, 512, Main.screenWidth, Main.screenHeight), fogColor * _thickFogAlpha);

                spriteBatch.End();
            }
        }
        orig(self, spriteBatch, layer, beginSpriteBatch);
    }

    private void RenderAroundWalls(On_Main.orig_DoDraw_WallsAndBlacks orig, Main self)
    {
        if (Main.LocalPlayer.GetModPlayer<MyPlayer>().ZoneAbyss)
        {
            var noiseSprite = AssetReferences.Assets.NoiseTextures.Clouds.Asset.Value;
            var ditherSprite = AssetReferences.Assets.Dithering.Dither8x8DoubleScaled.Asset.Value;
            var pass = AssetReferences.Effects.Abyss.AbyssFog.CreateBlackPass();
            HlslSampler sampler = new HlslSampler();
            sampler.Sampler = SamplerState.PointWrap;
            sampler.Texture = noiseSprite;
            pass.Parameters.spriteSampler = sampler;

            HlslSampler ditherSampler = new HlslSampler();
            ditherSampler.Sampler = SamplerState.PointWrap;
            ditherSampler.Texture = ditherSprite;
            pass.Parameters.ditherSampler = ditherSampler;

            pass.Parameters.time = Main.GlobalTimeWrappedHourly * 0.4f;
            pass.Parameters.ditherTexelSize = ditherSprite.GetTexelSize();
            pass.Parameters.spriteSize = noiseSprite.Size();
            pass.Parameters.screenOffset = DrawUtilities.CalculateScreenOffset(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight)) * 0.3f;
            pass.Apply();
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, pass.Shader, Main.GameViewMatrix.TransformationMatrix);


            Color fogColor = Color.Lerp(Color.White, Color.Blue, 0.3f);

            spriteBatch.Draw(noiseSprite, Vector2.Zero, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), fogColor * 0.36f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

        }

        orig(self);
    }
}
