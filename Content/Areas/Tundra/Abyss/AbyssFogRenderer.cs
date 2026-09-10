using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;
using Stellamod.Content.Areas.Tundra.Abyss.TilesAB;
using Stellamod.Content.Biomes;
using Stellamod.Core;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.Palettes;
using Stellamod.Core.Rendering;
using Stellamod.Core.WallBackgroundSystem;
using Stellamod.WorldG;
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
    private Vector2 _abyssWaterFall;
    public static Color TileGlowColor;
    public static readonly List<Rectangle> AbyssWaterfallPoints = new();
    public static readonly List<Action> OverWater = new();
    public RenderTargetProvider WaterfallTarget = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    public override void Load()
    {
        base.Load();
        _fastRandom = new FastRandom(2);
        On_Main.Ambience += WaterfallAmbience;
        On_Main.CheckMonoliths += RenderWaterfallTarget;
        On_Main.RenderTiles += ResetSpecialPoints;
        On_Main.RenderWalls += ResetSpecialPoints;

        On_Main.DoDraw_WallsAndBlacks += RenderAroundWalls;
        On_Main.DrawInfernoRings += DrawOverWater;
        On_OverlayManager.Draw += DrawPostProcessingPasses;
    }

    private void WaterfallAmbience(On_Main.orig_Ambience orig)
    {
        if (Main.LocalPlayer.ZoneAbyss)
        {
            if(Main.GameUpdateCount % 15 == 0)
            {
                float lowestDistance = 9999999;
                Vector2 worldPos = Vector2.Zero;
                foreach (var rect in AbyssWaterfallPoints)
                {
                    float d = Vector2.DistanceSquared(rect.Bottom(), Main.LocalPlayer.Center);
                    if (d < lowestDistance)
                    {
                        worldPos = rect.Bottom();
                        lowestDistance = d;
                    }
                }
                _abyssWaterFall = worldPos;
            }

            Main.ambientWaterfallX = _abyssWaterFall.X;
            Main.ambientWaterfallY = _abyssWaterFall.Y;
            Main.ambientWaterfallStrength = 600;
        }

        orig();
    }

    private void RenderWaterfallTarget(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (AbyssWaterfallPoints.Count <= 0)
            return;
        //Waterfall shading
        var pass = AssetReferences.Effects.Abyss.Waterfall.CreatePixelPass();
        pass.Parameters.time = Main.GlobalTimeWrappedHourly * 0.75f;

        var noiseSampler = new HlslSampler();
        noiseSampler.Sampler = SamplerState.LinearWrap;
        noiseSampler.Texture = AssetReferences.Assets.NoiseTextures.PerlinNoise.Asset.Value;
        pass.Parameters.noiseSampler = noiseSampler;

        var whrilySampler = new HlslSampler();
        whrilySampler.Sampler = SamplerState.LinearWrap;
        whrilySampler.Texture = AssetReferences.Assets.NoiseTextures.WaterCaustics.Asset.Value;

        pass.Parameters.whirlyNoiseSampler = whrilySampler;
        pass.Parameters.waveStrength = 0.5f;
        pass.Parameters.ColorSpectrumTexture = PaletteAssets.FromPaletteFile(PaletteAssets.ABYSSWATERFALL).Value.ColorAtlas;
        pass.Apply();

        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.GraphicsDevice.SetRenderTarget(WaterfallTarget);
        spriteBatch.GraphicsDevice.Clear(Color.Transparent);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.WhiteSquare.Asset, Vector2.Zero);


        using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with { 
            effect = pass.Shader, 
            blendState = CustomBlendStates.Max, 
            samplerState = SamplerState.AnisotropicWrap }))
        {
            //int i = 0;
            Color color = Color.Lerp(Color.White, Color.Cyan, 0.75f);
            color = Color.Lerp(color, Color.Blue, 0.5f);
            foreach (Rectangle rect in AbyssWaterfallPoints)
            {

                Rectangle screenREct = rect;
                screenREct.X -= (int)Main.screenPosition.X;
                screenREct.Y -= (int)Main.screenPosition.Y;
                screenREct = screenREct.CenterPad(32);
                drawer.dstRect = screenREct;


                drawer.drawOrigin = Vector2.Zero;
                drawer.color = color * 0.35f * ExtraMath.Osc(0.7f, 1f, speed: 0, offset: rect.X);
                spriteBatch.Draw(drawer);
            }

        }
        spriteBatch.GraphicsDevice.SetRenderTarget(null);
    }

    private void DrawOverWater(On_Main.orig_DrawInfernoRings orig, Main self)
    {
        orig(self);
        if (OverWater.Count <= 0)
            return;
        using(new SpritebatchContext(Main.spriteBatch, SpritebatchParams.InWorldAndZoomed()))
        {
            foreach (var action in OverWater)
            {
                action();
            }
        }

    }

    public override void PreUpdateNPCs()
    {
        base.PreUpdateNPCs();
        OverWater.Clear();
        Color glowColor = Color.Lerp(Color.LightGray, Color.SkyBlue, ExtraMath.Osc(0f, 1f, speed: 1)) * BellFlowerSystem.WhisperingAlpha;
        glowColor *= ExtraMath.Osc(0.6f, 1f);
        glowColor.A = 0;
        TileGlowColor = glowColor;
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
            (Point tl, Point bottomRight) = TileUtilities.CameraTileBounds(900);
            tl.Y += 1;
            for (int x = tl.X; x < bottomRight.X; x++)
            {
                for (int y = tl.Y; y < bottomRight.Y; y++)
                {
                    Tile tile = Main.tile[x, y];
                    Tile tileAbove = Main.tile[x, y - 1];
                    if (tile.LiquidAmount > 0 && !tileAbove.HasTile && tileAbove.LiquidAmount <= 0)
                    {
                        int w = 8;
                        int h = 8;
                        Rectangle rect = new Rectangle(x - w / 2, y, w, h);
                        rect = TileUtilities.Clamp(rect);
                        float pct = VeilGen.CountLiquidsPercent(rect);
                        if(pct > 0.35f)
                        {
                            ScanUpforWaterfall(x, y);
                        }
                    }
                }
            }
        }

        orig(self);

        if (BellFlowerSystem.WhisperingAlpha < 0.01f)
            return;
  
        var pass = AssetReferences.Effects.Generic.Outliner.CreatePixelPass();
        pass.Parameters.texelSize = Main.instance.tileTarget.GetTexelSize() * 2;
        pass.Apply();
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.GraphicsDevice.SetRenderTarget(ExtraRenderTargets.TileTargetSwap);
        spriteBatch.GraphicsDevice.Clear(Color.Transparent);
        spriteBatch.Begin(
            SpriteSortMode.Deferred, 
            BlendState.AlphaBlend, 
            SamplerState.PointClamp, 
            DepthStencilState.None, 
            RasterizerState.CullNone,
            pass.Shader);
        spriteBatch.Draw(Main.instance.tileTarget, Vector2.Zero, AbyssEffectsRenderer.TileGlowColor);
        spriteBatch.End();

        spriteBatch.GraphicsDevice.SetRenderTarget(Main.instance.tileTarget);
        spriteBatch.GraphicsDevice.Clear(Color.Transparent);
        spriteBatch.Begin();
        spriteBatch.Draw(ExtraRenderTargets.TileTargetSwap, Vector2.Zero, Color.White);
        spriteBatch.End();
        spriteBatch.GraphicsDevice.SetRenderTarget(null);
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
            rect.Height -= 8;
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
        Rectangle screnRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
        screnRect = screnRect.CenterPad(128);
        foreach(Rectangle crashRect in AbyssWaterfallPoints)
        {
            if(screnRect.Contains(crashRect) || screnRect.Intersects(crashRect))
            {
                Vector2 crashPoint = crashRect.Bottom();
                if (Main.rand.NextBool(16))
                {
                    Vector2 crashParticlePoint = crashPoint;
                    crashParticlePoint.X += _fastRandom.Next(-128, 128);
                    Particles.WaterfallCrashDust.Spawn(WaterfallCrashDustData.Default with
                    {
                        position = crashParticlePoint,
                        velocity = Main.rand.NextVector2Circular(8, 4),
                        timeLeft = Main.rand.Next(40, 60),
                        rotation = Main.rand.NextFloat(3.14f),
                        scale = Main.rand.NextFloat(1f, 1.5f) * 0.9f
                    });
                }
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
            if(BellFlowerSystem.WhisperingAlpha > 0)
            {
               
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(
                    AssetReferences.Assets.GlowMasks.WhiteSquare.Asset.Value, 
                    new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2), 
                    Color.DarkGray * 0.35f * BellFlowerSystem.WhisperingAlpha);
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
            var pass = AssetReferences.Effects.Abyss.WaterfallOutline.CreatePixelPass();
            pass.Parameters.texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight);
            pass.Apply();
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, pass.Shader);
            spriteBatch.Draw(WaterfallTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

        }
    }
}
