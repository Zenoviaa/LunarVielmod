using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;
using Stellamod.Content.Areas.Tundra.Abyss.TilesAB;
using Stellamod.Content.Biomes;
using Stellamod.Core;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.Palettes;
using Stellamod.Core.Rendering;
using Stellamod.Core.WallBackgroundSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;

using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.Tundra.Abyss;



[Autoload(Side = ModSide.Client)]
public class AbyssEffectsRenderer : ModSystem
{
    private float _timer;
    private FastRandom _fastRandom;
    private float _thickFogAlpha;
    public static readonly List<Rectangle> AbyssWaterfallPoints = new();
    public override void Load()
    {
        base.Load();
        _fastRandom = new FastRandom(2);
        On_Main.RenderTiles += ResetSpecialPoints;
        On_Main.RenderWalls += ResetSpecialPoints;

        On_Main.DoDraw_WallsAndBlacks += RenderAroundWalls;

        On_OverlayManager.Draw += DrawPostProcessingPasses;
    }


    private void ResetSpecialPoints(On_Main.orig_RenderWalls orig, Main self)
    {

        orig(self);
    }

    private void ResetSpecialPoints(On_Main.orig_RenderTiles orig, Main self)
    {
        AbyssWaterfallPoints.Clear();
        if (Main.LocalPlayer.ZoneAbyss)
        {
            (Point tl, Point bottomRight) = TileUtilities.CameraTileBounds(666);
            tl.Y += 1;
            for (int x = tl.X; x < bottomRight.X; x++)
            {
                for (int y = tl.Y; y < bottomRight.Y; y++)
                {
                    Tile tile = Main.tile[x, y];
                    Tile tileAbove = Main.tile[x, y - 1];
                    if (tile.LiquidAmount > 0 && !tileAbove.HasTile && tileAbove.LiquidAmount <= 0)
                    {
                        ScanUpforWaterfall(x, y);
                    }
                }
            }

        }

        orig(self);

    }

    private void ScanUpforWaterfall(int i, int j)
    {
        (Point ceil, int steps) = TileUtilities.FindCeiling(i, j, 64);
        if (steps == -1)
            return;
        if (steps > 30)
        {
            Point bottom = ceil + new Point(0, steps);

            Vector2 topWorld = ceil.ToWorldCoordinates();
            Vector2 bottomWorld = bottom.ToWorldCoordinates();
            Rectangle rect = new Rectangle((int)topWorld.X, (int)topWorld.Y, 32, (int)(bottomWorld.Y - topWorld.Y));
            
            AbyssWaterfallPoints.Add(rect);
        }
    }



    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        float targetAlpha = BellFlowerSystem.RungBellFlowerCount / 2f;
        if (BellFlowerSystem.WhisperingCountdown > 0)
            targetAlpha = 0;
        _thickFogAlpha = MathHelper.Lerp(_thickFogAlpha, targetAlpha, 0.05f);
        _timer++;
        foreach(Rectangle crashRect in AbyssWaterfallPoints)
        {
            Vector2 crashPoint = crashRect.Bottom();
            if(_timer % 8 == 0)
            {
                Vector2 crashParticlePoint = crashPoint;
                crashParticlePoint.X += _fastRandom.Next(-128, 128);
                Particles.WaterfallCrashDust.Spawn(WaterfallCrashDustData.Default with { 
                    position = crashParticlePoint, 
                    velocity = Main.rand.NextVector2Circular(8, 4),
                    timeLeft = Main.rand.Next(40, 60),
                    rotation = Main.rand.NextFloat(3.14f), 
                    scale = Main.rand.NextFloat(1f, 1.5f) * 0.9f });
            }
        }
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


                spriteBatch.Begin(
                    SpriteSortMode.Deferred, 
                    BlendState.AlphaBlend, 
                    SamplerState.PointWrap, 
                    DepthStencilState.Default, 
                    Main.Rasterizer, 
                    pass.Shader, 
                    Main.GameViewMatrix.TransformationMatrix);


                Color fogColor = Color.Lerp(Color.White, Color.Blue, 0.7f);

                float alpha = 0.23f;
                alpha += _thickFogAlpha * 0.3f;

                spriteBatch.Draw(noiseSprite, Vector2.Zero, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), fogColor * alpha);
                spriteBatch.Draw(noiseSprite, Vector2.Zero, new Rectangle(512, 512, Main.screenWidth, Main.screenHeight), fogColor * BellFlowerSystem.WhisperingDistanceAlpha * 0.4f);

                spriteBatch.End();
            }
        }
        orig(self, spriteBatch, layer, beginSpriteBatch);
    }

    private void RenderAroundWalls(On_Main.orig_DoDraw_WallsAndBlacks orig, Main self)
    {
        if (Main.LocalPlayer.ZoneAbyss)
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
   //     Main.NewText(AbyssWaterfallPoints.Count);
        if (AbyssWaterfallPoints.Count > 0)
        {
            //Waterfall shading
            var pass = AssetReferences.Effects.Abyss.Waterfall.CreatePixelPass();
            pass.Parameters.time = Main.GlobalTimeWrappedHourly * 0.75f;

            var noiseSampler = new HlslSampler();
            noiseSampler.Sampler = SamplerState.LinearWrap;
            noiseSampler.Texture = AssetReferences.Assets.NoiseTextures.PerlinNoise.Asset.Value;
            pass.Parameters.noiseSampler = noiseSampler;
            pass.Parameters.waveStrength = 0.5f;
            pass.Parameters.ColorSpectrumTexture = PaletteAssets.FromPaletteFile(PaletteAssets.ABYSSWATER).Value.ColorAtlas;
            pass.Apply();

            SpriteBatch spriteBatch = Main.spriteBatch;
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.WhiteSquare.Asset, Vector2.Zero);


            using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { effect = pass.Shader, blendState = BlendState.AlphaBlend, samplerState = SamplerState.PointWrap }))
            {
                //int i = 0;
                Color color = Color.Lerp(Color.White, Color.Cyan, 0.75f);
                color = Color.Lerp(color, Color.Blue, 0.5f);
                foreach(Rectangle rect in AbyssWaterfallPoints)
                {
                    
                    Rectangle screenREct = rect;
                    screenREct.X -= (int)Main.screenPosition.X;
                    screenREct.Y -= (int)Main.screenPosition.Y;
                    screenREct = screenREct.CenterPad(32);
                    drawer.dstRect = screenREct;

               
                    drawer.drawOrigin = Vector2.Zero;
                    drawer.color = color * 0.5f * ExtraMath.Osc(0.7f, 1f, speed: 0, offset: rect.X);
                    spriteBatch.Draw(drawer);
                }

            }

        }


    }
}
