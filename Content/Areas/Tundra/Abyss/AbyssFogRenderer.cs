using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;
using Stellamod.Content.Biomes;
using Stellamod.Core;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.Palettes;
using Stellamod.Core.Rendering.RTs;
using Stellamod.WorldG;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.Tundra.Abyss;


public struct WaterfallDraw
{
    public Rectangle waterfallRect;
    public Color waterfallColor;
}


[Autoload(Side = ModSide.Client)]
public class AbyssEffectsRenderer : ModSystem
{
    private float _thickFogAlpha;
    private Vector2 _abyssWaterFall;
    
    private int _renderCountdown;
    public Vector2 sceneWaterfallPos;

    /// <summary>
    /// When set to true, finds all waterfalls in the abyss and puts them in a list, then sets it to false
    /// </summary>
    public static bool rebuildWaterfalls;
    public static Color TileGlowColor;

    /// <summary>
    /// All waterfalls that should be rendered to the screen
    /// </summary>
    public static readonly List<WaterfallDraw> AbyssWaterfallPoints = new();
    
    /// <summary>
    /// All waterfalls in the world
    /// </summary>
    public static readonly List<WaterfallDraw> AllWaterfalls = new();
    public static readonly List<Action> OverWater = new();
    public override void Load()
    {
        base.Load();
        On_Main.Ambience += WaterfallAmbience;
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
            if (Main.GameUpdateCount % 15 == 0)
            {
                float lowestDistance = 9999999;
                Vector2 worldPos = Vector2.Zero;
                foreach (var rect in AbyssWaterfallPoints)
                {
                    float d = Vector2.DistanceSquared(rect.waterfallRect.Bottom(), Main.LocalPlayer.Center);
                    if (d < lowestDistance)
                    {
                        worldPos = rect.waterfallRect.Bottom();
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


    private void PrepareWaterfallTargetContent()
    {
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
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.WhiteSquare.Asset, Vector2.Zero);
        drawer.drawOrigin = Vector2.Zero;

        using (new SpritebatchContext(spriteBatch, SpritebatchParams.InWorldAndZoomed() with
        {
            effect = pass.Shader,
            blendState = CustomBlendStates.Max,
            samplerState = SamplerState.AnisotropicWrap
        }))
        {
            //int i = 0;
            int spX = (int)Main.screenPosition.X;
            int spY = (int)Main.screenPosition.Y;
            foreach (var wf in AbyssWaterfallPoints)
            {
                Rectangle screenREct = wf.waterfallRect;
                screenREct.X -= spX;
                screenREct.Y -= spY;
                drawer.dstRect = screenREct;
                drawer.color = wf.waterfallColor;
                spriteBatch.Draw(drawer);
            }

        }
        sceneWaterfallPos = Main.screenPosition;
    }

    private void DrawOverWater(On_Main.orig_DrawInfernoRings orig, Main self)
    {
        orig(self);
        if (OverWater.Count <= 0)
            return;
        using (new SpritebatchContext(Main.spriteBatch, SpritebatchParams.InWorldAndZoomed()))
        {
            foreach (var action in OverWater)
            {
                action();
            }
        }

    }
    public override void ClearWorld()
    {
        base.ClearWorld();
        rebuildWaterfalls = true;
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
    private bool BiomeWaterfallsAvailable()
    {
        return Main.LocalPlayer.ZoneAbyss || Main.LocalPlayer.GetModPlayer<MyPlayer>().ZoneAurelus;
    }
    private void ResetSpecialPoints(On_Main.orig_RenderWalls orig, Main self)
    {

        orig(self);
    }

    private void ResetSpecialPoints(On_Main.orig_RenderTiles orig, Main self)
    {

        ref bool justEnteredAbyss = ref Main.LocalPlayer.GetModPlayer<BiomePlayer>().justEnteredAbyss;
        if (justEnteredAbyss)
        {
            AllWaterfalls.Clear();
            justEnteredAbyss = false;
            // var watch = Stopwatch.StartNew();
            Rectangle abRect = VeilGen.AbyssRectangle;
            for(int x = abRect.Left; x <= abRect.Right; x++)
            {
                for(int y = abRect.Top; y <= abRect.Bottom; y++)
                {
                    Tile tile = Main.tile[x, y];
                    Tile tileAbove = Main.tile[x, y - 1];
                    if (tile.LiquidAmount > 0 && !tileAbove.HasTile && tileAbove.LiquidAmount <= 0)
                    {
                        int w = 4;
                        int h = 4;
                        Rectangle rect = new Rectangle(x - w / 2, y, w, h);
                        rect = TileUtilities.Clamp(rect);
                        float pct = VeilGen.CountLiquidsPercent(rect);
                        if (pct > 0.35f)
                        {
                            ScanUpforWaterfall(x, y);
                        }
                    }
                }
            }
            /*
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
                        int w = 4;
                        int h = 4;
                        Rectangle rect = new Rectangle(x - w / 2, y, w, h);
                        rect = TileUtilities.Clamp(rect);
                        float pct = VeilGen.CountLiquidsPercent(rect);
                        if (pct > 0.35f)
                        {
                            ScanUpforWaterfall(x, y);
                        }
                    }
                }
            }*/

         //   watch.Stop();
        }

        orig(self);

        if (BellFlowerSystem.WhisperingAlpha < 0.01f)
            return;

        var pass = AssetReferences.Effects.Generic.Outliner.CreatePixelPass();
        pass.Parameters.texelSize = Main.instance.tileTarget.GetTexelSize() * 2;
        pass.Apply();
        SpriteBatch spriteBatch = Main.spriteBatch;
        RenderTargetHandle tileTargetSwap = RenderTargets.TileTarget;
        using(new RenderTargetContext(tileTargetSwap))
        {
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                pass.Shader);
            spriteBatch.Draw(Main.instance.tileTarget, Vector2.Zero, AbyssEffectsRenderer.TileGlowColor);
            spriteBatch.End();
        }

        spriteBatch.GraphicsDevice.SetRenderTarget(Main.instance.tileTarget);
        spriteBatch.GraphicsDevice.Clear(Color.Transparent);
        spriteBatch.Begin();
        spriteBatch.Draw(tileTargetSwap, Vector2.Zero, Color.White);
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
            rect = rect.CenterPad(32);

            WaterfallDraw waterfallDraw = new WaterfallDraw
            {
                waterfallRect = rect,
                waterfallColor = LunarColor.AbyssWaterfall * 0.35f * ExtraMath.Osc(0.7f, 1f, speed: 0, offset: rect.X)
            };

            AllWaterfalls.Add(waterfallDraw);
        }
    }

    private void FindWaterfallsToRender()
    {
  
        Rectangle screnRect = new Rectangle(
            (int)Main.screenPosition.X,
            (int)Main.screenPosition.Y,
            Main.screenWidth,
            Main.screenHeight);
        screnRect = screnRect.CenterPad(512);
        AbyssWaterfallPoints.Clear();
        foreach(var wf in AllWaterfalls)
        {
            var crashRect = wf.waterfallRect;
            if (!screnRect.Intersects(crashRect) && !screnRect.Contains(crashRect))
                continue;
            AbyssWaterfallPoints.Add(wf);
        }
   
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        float targetAlpha = BellFlowerSystem.RungBellFlowerCount / 2f;
        if (BellFlowerSystem.WhisperingCountdown > 0)
            targetAlpha = 0;
        _thickFogAlpha = MathHelper.Lerp(_thickFogAlpha, targetAlpha, 0.05f);
        if (AllWaterfalls.Count <= 0)
            return;
        if (!BiomeWaterfallsAvailable())
            return;


        float gameUpdateCount = Main.GameUpdateCount;
        var rand = Main.rand;
        if(Main.GameUpdateCount % 15 == 0)
        {
            FindWaterfallsToRender();
        }

        if (AbyssWaterfallPoints.Count <= 0)
            return;

        foreach (var wf in AbyssWaterfallPoints)
        {
            var crashRect = wf.waterfallRect;

 
            if (rand.Next(0, 16) != 0)
                continue;

            Vector2 crashPoint = crashRect.Bottom();
            Vector2 crashParticlePoint = crashPoint;
            float updateCount = gameUpdateCount + wf.waterfallRect.X;
            crashParticlePoint.X += rand.Next(-128, 128);
            Particles.WaterfallCrashDust.Spawn(WaterfallCrashDustData.Default with
            {
                position = crashParticlePoint,
                velocity = Main.rand.NextVector2Circular(8, 4),
                timeLeft = 50,
                rotation = updateCount % 3.14f,
                scale = ((updateCount % 0.5f) + 1) * 0.9f
            });
        }

    }


    private void DrawPostProcessingPasses(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
    {
        if (layer == RenderLayers.All && beginSpriteBatch && !Main.gameMenu && LightingHelper.CanRenderPostProcessingEffects)
        {
            if (BiomeWaterfallsAvailable())
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
            if (BellFlowerSystem.WhisperingAlpha > 0)
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
        if (BiomeWaterfallsAvailable())
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
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.EndOut(out var oldParameters);
            
            //Render to waterfall render target
            RenderTargetHandle handle = RenderTargets.ScreenTarget;

            //A target is needed to properly blend the waterfalls together
            using(new RenderTargetContext(handle))
            {
                PrepareWaterfallTargetContent();
            }

            //Render waterfall render target to screen
            var pass = AssetReferences.Effects.Abyss.WaterfallOutline.CreatePixelPass();
            pass.Parameters.texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight);
            pass.Apply();
            using (new SpritebatchContext(spriteBatch, oldParameters with 
            { 
                effect = pass.Shader, 
                matrix = Matrix.identity
            }))
            {
                spriteBatch.Draw(handle, sceneWaterfallPos - Main.screenPosition, Color.White);
            }

            //resume sprite batch
            spriteBatch.Begin(oldParameters);
        }
    }
}
