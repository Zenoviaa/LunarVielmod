using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.WaterSide.BossesWS;
using Stellamod.Content.Biomes;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Liquid;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Light;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Core.Rendering;

public class SuperLavaShader : CrystalShader<SuperLavaShader>
{
    public Texture2D RockTexture
    {
        set
        {
            Effect.Parameters["RockTexture"].SetValue(value);
        }
    }
    public Texture2D GlowMap
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[3] = value;
            Main.graphics.GraphicsDevice.SamplerStates[3] = SamplerState.AnisotropicClamp;
        }
    }
    public Texture2D HeightMap
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[2] = value;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.AnisotropicClamp;
        }
    }
    public Texture2D WaterTexture
    {
        set
        {
            Effect.Parameters["WaterTexture"].SetValue(value);
        }
    }
    public float Time
    {
        set
        {
            Effect.Parameters["Time"].SetValue(value);
        }
    }
    public float Distortion
    {
        set
        {
            Effect.Parameters["Distortion"].SetValue(value);
        }
    }
    public Vector2 ScreenOffset
    {
        set
        {
            Effect.Parameters["ScreenOffset"].SetValue(value);
        }
    }
    public Vector2 Tiling
    {
        set
        {
            Effect.Parameters["Tiling"].SetValue(value);
        }
    }
    public Color InnerColor
    {
        set
        {
            Effect.Parameters["InnerColor"].SetValue(value.ToVector3());
        }
    }

    public Color BloomColor
    {
        set
        {
            Effect.Parameters["BloomColor"].SetValue(value.ToVector3());
        }
    }
    public Color StartGradient
    {
        set
        {
            Effect.Parameters["StartGradient"].SetValue(value.ToVector3());
        }
    }

    public Color EndGradient
    {
        set
        {
            Effect.Parameters["EndGradient"].SetValue(value.ToVector3());
        }
    }

    public float Quantize
    {
        set
        {
            Effect.Parameters["Quantize"].SetValue(value);
        }
    }

    public float NormalDistortionStrength
    {
        set
        {
            Effect.Parameters["NormalDistortionStrength"].SetValue(value);
        }
    }

    public Texture2D NormalNoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
        }
    }

    public Texture2D OutlineTexelSize
    {
        set
        {
            Effect.Parameters["outlineTexelSize"].SetValue(value.GetTexelSize() * 2);
        }
    }

    public Color OutlineColor
    {
        set
        {
            Effect.Parameters["outlineColor"].SetValue(value.ToVector4());
        }
    }
}

public static class WaterHelpers
{
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "DrawWaters")]
    public static extern void DrawWaters(Main instance, bool isBackground);


    //GetScreenDrawArea(unscaledPosition, vector + (Main.Camera.UnscaledPosition - Main.Camera.ScaledPosition), out var firstTileX, out var lastTileX, out var firstTileY, out var lastTileY);
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "GetScreenDrawArea")]
    public static extern void GetScreenDrawArea(TileDrawing tileDrawing, Vector2 unscaledPosition, Vector2 offset, out int firstTileX, out int lastTileX, out int firstTileY, out int lastTileY);


}
public abstract class PixelWaterStyle : ModType
{
    /// <summary>
    /// If multiple waters are active, this determines which one gets used
    /// </summary>
    public int priority;
    protected sealed override void Register()
    {
        ModTypeLookup<PixelWaterStyle>.Register(this);
    }
    public sealed override void SetupContent()
    {
        base.SetupContent();
        SetStaticDefaults();
    }

    public virtual bool IsActive(Player player)
    {
        return true;
    }
    public virtual void ModifyPixelWater(ref PixelWater pixelWater)
    {

    }
}


public class PixelWater
{
    public PixelWater()
    {

    }

    public void SetDefaults()
    {
        StartGradientColor = Color.Aqua;
        EndGradientColor = Color.Lerp(Color.SeaGreen, Color.Black, 0.75f);
        BackgroundColor = Color.CornflowerBlue;
        CausticsColor = Color.SeaGreen * 0.75f;
        CausticsTexture = LoadTexture("WaterCaustics");
        NoiseTexture = LoadTexture("WaterNoise2");
        TilingMultiplier = Vector2.One;
        affectsLava = false;
        noLighting = false;
        vibrant = false;
        ignoreSkyColor = false;
        noReflection = false;
    }

    private Asset<Texture2D> LoadTexture(string fileName)
    {
        return ModContent.Request<Texture2D>($"Stellamod/Assets/NoiseTextures/{fileName}");
    }
    public Color StartGradientColor;
    public Color EndGradientColor;
    public Color BackgroundColor;
    public Color CausticsColor;
    public Vector2 TilingMultiplier;
    public Asset<Texture2D> NoiseTexture;
    public Asset<Texture2D> CausticsTexture;
    public bool noLighting;
    public bool vibrant;
    public bool ignoreSkyColor;
    public bool affectsLava;
    public bool noReflection;
}

public class PixelWaterStyleComparer : IComparer<PixelWaterStyle>
{
    public int Compare(PixelWaterStyle x, PixelWaterStyle y)
    {
        return y.priority.CompareTo(x.priority);
    }
}

[Autoload(Side = ModSide.Client)]
public class MoonWaterSystem : ModSystem
{
    private struct HeightDraw
    {
        public Vector2 tilePoint;
        public float height;
    }
    private static Point GetWaterTargetSize()
    {
        return new Point(Main.waterTarget.Width, Main.waterTarget.Height);
    }

    private RenderTargetProvider _reflectionRT = new RenderTargetProvider(RenderTargetParameters.DownsizedFunc(GetWaterTargetSize, 2));
    private RenderTargetProvider _waterTextureRT = new RenderTargetProvider(RenderTargetParameters.DownsizedFunc(GetWaterTargetSize, 2));
    private RenderTargetProvider _waterTextureRTSwap = new RenderTargetProvider(RenderTargetParameters.DownsizedFunc(GetWaterTargetSize, 2));

    private RenderTargetProvider _waterTextureRTOutput = new RenderTargetProvider(RenderTargetParameters.DownsizedFunc(GetWaterTargetSize, 1));
    private RenderTargetProvider _waterLightMapRT = new RenderTargetProvider(RenderTargetParameters.DownsizedFunc(GetWaterTargetSize, 1));

    private RenderTargetProvider _waterHeightMapRT = new RenderTargetProvider(() =>
    {
        RenderTargetParameters p = RenderTargetParameters.DefaultScreenTarget;
        p.Width = GetWaterTargetSize().X;
        p.Height = GetWaterTargetSize().Y;
        p.SurfaceFormat = SurfaceFormat.Alpha8;
        return p;
    });



    private PixelWaterStyle[] _pixelWaterStyles;
    private PixelWaterStyle _activePixelWaterStyle;
    private PixelWater _pixelWater;
    private PixelWaterStyleComparer _pixelWaterComparer;

    private List<HeightDraw> _heightsToDraw = new();

    private float _time;
    private Effect _waterEffect;
    private Rectangle _drawLocation;
    private Texture2D _perlinNoise;
    private Texture2D _waterNoise1;
    private bool _allowDraw;

    //This will give us a cool pixelation effect
    public int DownSamples => 2;
    public Vector2 Tiling => new Vector2(1.5f, 1.5f) * 0.75f;
    public float waterAlpha;
    public static event Action<SpriteBatch> DrawWaterMask;
    public override void Load()
    {
        On_Main.CheckMonoliths += RenderHook;
        On_Main.DrawDust += CopyScreenTarget;
        On_OverlayManager.Draw += ApplyWaterShader;
        On_Main.DrawWaters += StopDrawWater;
    }

    private void DrawWaterMaskT(On_LiquidEdgeRenderer.orig_DrawTileMask orig, SpriteBatch spriteBatch, RenderTarget2D tileTarget, Vector2 tileTargetOffset)
    {
        orig(spriteBatch, tileTarget, tileTargetOffset);
        DrawWaterMask?.Invoke(spriteBatch);
    }

    public override void Unload()
    {
        base.Unload();
        On_Main.CheckMonoliths -= RenderHook;
        On_Main.DrawDust -= CopyScreenTarget;
        On_OverlayManager.Draw -= ApplyWaterShader;
        On_Main.DrawWaters -= StopDrawWater;
        _pixelWaterStyles = null;
        _heightsToDraw.Clear();
    }
    private void StopDrawWater(On_Main.orig_DrawWaters orig, Main self, bool isBackground)
    {
        if (!_allowDraw)
            return;

        orig(self, isBackground);
    }


    public override void OnModLoad()
    {
        base.OnModLoad();
        LoadAssets();
        //Get all of our available pixel water styles and sort them
        _pixelWater = new PixelWater();
        _pixelWaterStyles = ModContent.GetContent<PixelWaterStyle>().ToArray();
    }


    public RenderTarget2D GetReflectionRenderTarget()
    {
        return _reflectionRT;
    }

    private PixelWaterStyle GetActivePixelWaterStyle()
    {
        for (int i = 0; i < _pixelWaterStyles.Length; i++)
        {
            PixelWaterStyle pixelWaterStyle = _pixelWaterStyles[i];
            if (pixelWaterStyle.IsActive(Main.LocalPlayer))
                return pixelWaterStyle;
        }

        //This will never happen since the default water is always true, lol.
        return _pixelWaterStyles[0];
    }

    private void CopyScreenTarget(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        if (_reflectionRT == null)
            return;
        if (Main.gameMenu)
            return;
        if (!LightingHelper.CanRenderPostProcessingEffects)
            return;


        SpriteBatch spriteBatch = Main.spriteBatch;

        //Copy the current screen target for reflections
        //If we do this after the water renders we get an infinite reflection loops lmao that's bad.
        GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
        graphicsDevice.SetRenderTarget(Main.screenTargetSwap);
        graphicsDevice.Clear(Color.Black);
        spriteBatch.Begin();
        spriteBatch.Draw(Main.screenTarget, Vector2.Zero, null, Color.White);
        spriteBatch.End();

        graphicsDevice.SetRenderTarget(_reflectionRT);
        graphicsDevice.Clear(Color.Black);

        spriteBatch.Begin();
        spriteBatch.Draw(Main.screenTarget, Vector2.Zero + new Vector2(Main.offScreenRange) / 2f, null, Color.White, 0, Vector2.Zero, 1f / (float)DownSamples, SpriteEffects.None, 0f);
        spriteBatch.End();

        //Draw the current render back so no data is loss
        graphicsDevice.SetRenderTarget(Main.screenTarget);
        graphicsDevice.Clear(Color.Black);
        spriteBatch.Begin();
        spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
        spriteBatch.End();
    }

    private void ApplyWaterShader(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
    {
        orig(self, spriteBatch, layer, beginSpriteBatch);
        var config = ModContent.GetInstance<LunarVeilClientConfig>();

        if (!config.LiquidsToggle)
            return;
        if (Main.gameMenu)
            return;
        if (_waterEffect == null)
            return;
        if (!LightingHelper.CanRenderPostProcessingEffects)
        {
            _allowDraw = true;
            return;
        }

        if (layer == RenderLayers.ForegroundWater)
        {
            //This is called right before the front water gets drawn
            //We can apply our shader here.
            //It should work, I think
            if (_waterEffect == null)
                return;

            spriteBatch.End();
            CopyScreenTargetToSwap();

            _allowDraw = true;
            CopyWaterTarget();
            //    _allowDraw = false;
            CopySwapToScreenTarget();

            if (_pixelWater.affectsLava)
            {
                //      Main.NewText("yuh");

                SuperLavaShader lavaShader = ShaderContent.GetInstance<SuperLavaShader>();
                lavaShader.NormalNoiseTexture = _waterTextureRTOutput;
                lavaShader.HeightMap = _waterHeightMapRT;
                lavaShader.OutlineColor = Color.Lerp(Color.White, Color.Goldenrod, 0.3f);
                lavaShader.OutlineTexelSize = Main.waterTarget;
                lavaShader.Effect.CurrentTechnique = lavaShader.Effect.Techniques["Combine"];
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                    lavaShader.Effect, Main.Transform);

                Vector2 pos = Main.sceneWaterPos - Main.screenPosition;
                spriteBatch.Draw(Main.waterTarget, pos, Color.White * waterAlpha);


                Color c = Color.White * 0.3f;
                c.A = 0;
                spriteBatch.Draw(Main.waterTarget, pos, c * waterAlpha);
                spriteBatch.End();
             
                
                
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
            else
            {

                _waterEffect.CurrentTechnique = _waterEffect.Techniques["CombineRTDrawing"];
                _waterEffect.Parameters["WaterTexture"].SetValue(_waterTextureRTOutput);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                    _waterEffect, Main.Transform);

                Vector2 pos = Main.sceneWaterPos - Main.screenPosition;
                spriteBatch.Draw(Main.waterTarget, pos, Color.White * waterAlpha);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
  
            //DrawWaterBaseToScreen();
        }

    }

    private void CopyScreenTargetToSwap()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.GraphicsDevice.SetRenderTarget(Main.screenTargetSwap);
        spriteBatch.GraphicsDevice.Clear(Color.Transparent);
        spriteBatch.Begin();
        spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
        spriteBatch.End();

    }
    private void CopySwapToScreenTarget()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.GraphicsDevice.SetRenderTarget(Main.screenTarget);
        spriteBatch.GraphicsDevice.Clear(Color.Transparent);
        spriteBatch.Begin();
        spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
        spriteBatch.End();

    }
    public override void PostUpdateTime()
    {
        base.PostUpdateTime();
        _time += 0.0025f;
        float targetWaterAlpha = 1f;
        if (Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneDeepBelowCoralways)
            targetWaterAlpha = 0.1f;
        if (Main.LocalPlayer.GetModPlayer<MyPlayer>().ZoneWonder)
            targetWaterAlpha = 0.95f;
        waterAlpha = MathHelper.Lerp(waterAlpha, targetWaterAlpha, 0.1f);
    }

    private Texture2D LoadTexture(string fileName)
    {
        return ModContent.Request<Texture2D>($"Stellamod/Assets/NoiseTextures/{fileName}").Value;
    }

    private void LoadAssets()
    {
        if (Main.gameMenu)
            return;

        _waterNoise1 = LoadTexture("WaterNoise1");
        _perlinNoise = LoadTexture("PerlinNoise");
    }

    private void RenderHook(On_Main.orig_CheckMonoliths orig)
    {

        orig();
        var config = ModContent.GetInstance<LunarVeilClientConfig>();
        if (!config.LiquidsToggle)
            return;
        if (Main.gameMenu)
            return;

        _waterEffect = ModContent.Request<Effect>("Stellamod/Effects/MoonWaters").Value;
        if (_waterEffect == null)
            return;

        if (!Lighting.UsingNewLighting)
            return;

        //var sw = Stopwatch.StartNew();
        if(Main.GameUpdateCount % 8 == 0)
            CalculateHeightsToDraw();
        RenderIntoHeightMapTarget();
        RenderIntoWaterTextureTarget();

    }


    private void CopyWaterTarget()
    {
        //So we'er copying the water target here cause it doesn't render every frame
        //This seems kinda stupid for performance but I'm not sure how to fix that jiterring issue otherwise?
        GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
        graphicsDevice.SetRenderTarget(Main.waterTarget);
        graphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);

        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Begin();
        try
        {
            WaterHelpers.DrawWaters(Main.instance, isBackground: false);
            //Anything else that wants to draw to the water target, such as particles
            //Which will create some nice visuals, fake metaballs basically

            //DrawWaterMask?.Invoke(spriteBatch);
        }
        catch
        {
        }
        Main.sceneWaterPos.X = Main.screenPosition.X - (float)Main.offScreenRange;
        Main.sceneWaterPos.Y = Main.screenPosition.Y - (float)Main.offScreenRange;

        spriteBatch.End();
        graphicsDevice.SetRenderTarget(null);
    }


    private void ApplyScreenOffset(float scale)
    {
        //Apply an offset so the texture doesn't move when you're moving
        //This will wrap inside the shader
        Vector2 texelSize = Vector2.One / new Vector2(_drawLocation.Width, _drawLocation.Height);
        Vector2 screenoffset = Main.screenPosition * texelSize;
        screenoffset *= (1f / scale);
        _waterEffect.Parameters["screenOffset"].SetValue(screenoffset);
    }

    private Vector2 CalculateScreenOffset(float scale)
    {
        Vector2 texelSize = Vector2.One / new Vector2(_drawLocation.Width, _drawLocation.Height);
        Vector2 screenoffset = Main.screenPosition * texelSize;
        screenoffset *= (1f / scale);
        return screenoffset;
    }
    private void DrawWaterBase(SpriteBatch spriteBatch)
    {
        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        if (_pixelWater.affectsLava)
        {
            SuperLavaShader lavaShader = ShaderContent.GetInstance<SuperLavaShader>();
            lavaShader.ScreenOffset = CalculateScreenOffset(scale: 2f);
            lavaShader.Tiling = Vector2.One * 2 * Tiling * _pixelWater.TilingMultiplier;
            lavaShader.Time = Main.GlobalTimeWrappedHourly * 1.5f;
            lavaShader.Quantize = 9;
            lavaShader.Distortion = 0.05f;
            lavaShader.StartGradient = _pixelWater.StartGradientColor;
            lavaShader.EndGradient = _pixelWater.EndGradientColor;
            lavaShader.NormalDistortionStrength = 0.25f;
            lavaShader.NormalNoiseTexture = _pixelWater.NoiseTexture.Value;
            lavaShader.InnerColor = Color.Lerp(Color.Yellow, Color.Red, 0.5f);
            lavaShader.BloomColor = Color.Red;
            lavaShader.RockTexture = AssetManager.LoadBackground("LavaRocks").Value;
            lavaShader.GlowMap = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Clouds6").Value;
            lavaShader.HeightMap = _waterHeightMapRT;
            lavaShader.Effect.CurrentTechnique = lavaShader.Effect.Techniques["SpriteDrawing"];

            graphicsDevice.SetRenderTarget(_waterTextureRTSwap);
            graphicsDevice.Clear(Color.DarkGoldenrod);

            //Draw the base texture
            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.AnisotropicClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                lavaShader.Effect);

            Color drawColor = _pixelWater.BackgroundColor * 0.8f ;
            if (!_pixelWater.ignoreSkyColor)
            {
                drawColor = drawColor.MultiplyRGB(Main.ColorOfTheSkies);
            }
            spriteBatch.Draw(_pixelWater.CausticsTexture.Value, _drawLocation, null, drawColor);
            spriteBatch.End();
            return;
        }

        _waterEffect.CurrentTechnique = _waterEffect.Techniques["SpriteDrawing"];
        _waterEffect.Parameters["tiling"].SetValue(Vector2.One * 2 * Tiling * _pixelWater.TilingMultiplier);
        _waterEffect.Parameters["time"].SetValue(_time);
        _waterEffect.Parameters["levels"].SetValue(18);
        _waterEffect.Parameters["distortion"].SetValue(0.05f);
        _waterEffect.Parameters["startGradient"].SetValue(_pixelWater.StartGradientColor.ToVector3());
        _waterEffect.Parameters["endGradient"].SetValue(_pixelWater.EndGradientColor.ToVector3());
        _waterEffect.Parameters["causticsColor"].SetValue(_pixelWater.CausticsColor.ToVector4());
        _waterEffect.Parameters["foamLava"].SetValue(_pixelWater.affectsLava ? 0 : 1);
        ApplyScreenOffset(scale: 2);
        Vector2 stretchScale = new Vector2(1, 0.5f);

        graphicsDevice.SetRenderTarget(_waterTextureRTSwap);
        graphicsDevice.Clear(Color.LightSeaGreen);

        Main.graphics.GraphicsDevice.Textures[1] = _pixelWater.NoiseTexture.Value;
        Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;

        Main.graphics.GraphicsDevice.Textures[2] = _pixelWater.CausticsTexture.Value;
        Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;

        Main.graphics.GraphicsDevice.Textures[3] = _perlinNoise;
        Main.graphics.GraphicsDevice.SamplerStates[3] = SamplerState.PointWrap;

        //Draw the base texture
        spriteBatch.Begin(SpriteSortMode.Deferred,
            BlendState.AlphaBlend, 
            SamplerState.AnisotropicClamp,
            DepthStencilState.None, 
            RasterizerState.CullNone, 
            _waterEffect);

        Color baseColor = _pixelWater.BackgroundColor * 0.75f;
        if (!_pixelWater.ignoreSkyColor)
        {
            baseColor = baseColor.MultiplyRGB(Main.ColorOfTheSkies);
        }
        spriteBatch.Draw(_waterNoise1, _drawLocation, null, baseColor);
        spriteBatch.End();
    }

    private void DrawWaterGradient(SpriteBatch spriteBatch)
    {
        //gradient gonna have to be added later

        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        //Set gradient effect values
        _waterEffect.CurrentTechnique = _waterEffect.Techniques["GradientDrawing"];
    
        //  ApplyScreenOffset();
        //Draw gradient
        graphicsDevice.SetRenderTarget(_waterTextureRTSwap);
        graphicsDevice.Clear(Color.Transparent);
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
        spriteBatch.Draw(_waterTextureRT, _drawLocation, null, Color.White);
        spriteBatch.End();
    }

    private void DrawWaterCaustics(SpriteBatch spriteBatch)
    {
        //Draw Caustics
        _waterEffect.CurrentTechnique = _waterEffect.Techniques["CausticsDrawing"];
        _waterEffect.Parameters["time"].SetValue(_time * 2);
        _waterEffect.Parameters["distortion"].SetValue(0.05f);
        _waterEffect.Parameters["tiling"].SetValue(Vector2.One * 6 * Tiling * _pixelWater.TilingMultiplier);

        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        graphicsDevice.SetRenderTarget(_waterTextureRT);
        graphicsDevice.Clear(Color.Transparent);
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
        spriteBatch.Draw(_pixelWater.CausticsTexture.Value, _drawLocation, null, _pixelWater.CausticsColor);
        spriteBatch.End();

        graphicsDevice.SetRenderTarget(_waterTextureRTSwap);

        _waterEffect.CurrentTechnique = _waterEffect.Techniques["WrapDrawing"];
        ApplyScreenOffset(scale: 2);
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
            _waterEffect);
        spriteBatch.Draw(_waterTextureRT, _drawLocation, null, Color.White);
        spriteBatch.End();
    }

    private void DrawWaterSparkle(SpriteBatch spriteBatch)
    {
        _waterEffect.CurrentTechnique = _waterEffect.Techniques["SparklingCausticsDrawing"];
        _waterEffect.Parameters["time"].SetValue(_time * 2);
        _waterEffect.Parameters["distortion"].SetValue(0.05f);
        _waterEffect.Parameters["tiling"].SetValue(Vector2.One * 8 * Tiling * _pixelWater.TilingMultiplier);
        _waterEffect.Parameters["HeightMapTexture"].SetValue(_waterHeightMapRT);

        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        graphicsDevice.SetRenderTarget(_waterTextureRT);
        graphicsDevice.Clear(Color.Transparent);

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
        spriteBatch.Draw(_pixelWater.CausticsTexture.Value, _drawLocation, null, Color.White * 0.5f);
        spriteBatch.End();


        graphicsDevice.SetRenderTarget(_waterTextureRTSwap);
        _waterEffect.CurrentTechnique = _waterEffect.Techniques["WrapDrawing"];
        ApplyScreenOffset(scale: 2);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
            _waterEffect);
        spriteBatch.Draw(_waterTextureRT, _drawLocation, null, Color.White);
        spriteBatch.End();
    }

    private void DrawWaterFoam(SpriteBatch spriteBatch)
    {
        _waterEffect.CurrentTechnique = _waterEffect.Techniques["FoamDrawing"];
        _waterEffect.Parameters["time"].SetValue(_time * 2);
        _waterEffect.Parameters["distortion"].SetValue(0.05f);
        _waterEffect.Parameters["tiling"].SetValue(Vector2.One * 2 * Tiling * _pixelWater.TilingMultiplier);
        _waterEffect.Parameters["HeightMapTexture"].SetValue(_waterHeightMapRT);
        ApplyScreenOffset(scale: 1f);
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
        spriteBatch.Draw(_perlinNoise, _drawLocation, null, Color.White);
        spriteBatch.End();
    }


    private void DrawReflection(SpriteBatch spriteBatch)
    {
        _drawLocation = new Rectangle(0, 0, _waterTextureRT.Width, _waterTextureRT.Height);

        float mipBias = 1;
        float reflectionDistance = 128;
        Vector2 reflectionTexelSize = (Vector2.One * mipBias) / new Vector2((float)_reflectionRT.Width, (float)_reflectionRT.Height);


        _waterEffect.CurrentTechnique = _waterEffect.Techniques["ReflectionDrawing"];
        _waterEffect.Parameters["reflectionDistance"].SetValue(reflectionDistance);
        _waterEffect.Parameters["reflectionTexelSize"].SetValue(reflectionTexelSize);
        _waterEffect.Parameters["reflectionPower"].SetValue(3.5f);
        _waterEffect.Parameters["HeightMapTexture"].SetValue(_waterHeightMapRT);


        _waterEffect.Parameters["time"].SetValue(_time * 2);
        _waterEffect.Parameters["distortion"].SetValue(0.005f);
        _waterEffect.Parameters["NoiseTexture"].SetValue(_pixelWater.CausticsTexture.Value);
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
        spriteBatch.Draw(_reflectionRT, Vector2.Zero, null, Color.White * 1f, 0, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0);
        spriteBatch.End();
    }

    private void DrawPosterization(SpriteBatch spriteBatch)
    {
        _drawLocation = new Rectangle(0, 0, _waterTextureRTOutput.Width, _waterTextureRTOutput.Height);
        _waterEffect.CurrentTechnique = _waterEffect.Techniques["PosterizeDrawing"];
        _waterEffect.Parameters["levels"].SetValue(10);

   

        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        graphicsDevice.SetRenderTarget(_waterTextureRTOutput);
        graphicsDevice.Clear(Color.DeepSkyBlue);

        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
        spriteBatch.Draw(_waterTextureRTSwap, _drawLocation, null, Color.White * 1f);
        spriteBatch.End();

        if (!_pixelWater.noLighting)
        {
            _waterEffect.CurrentTechnique = _waterEffect.Techniques["BlurDrawing"];
            spriteBatch.Begin(SpriteSortMode.Immediate, CustomBlendStates.Multiply, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            spriteBatch.Draw(_waterLightMapRT, _drawLocation, null, Color.White * 1);
            spriteBatch.End();
        }

        graphicsDevice.SetRenderTarget(null);
    }

    private void UpdatePixelWater()
    {
        _pixelWaterComparer ??= new PixelWaterStyleComparer();
        Array.Sort(_pixelWaterStyles, _pixelWaterComparer);
        _pixelWater.SetDefaults();
        _activePixelWaterStyle = GetActivePixelWaterStyle();
        _activePixelWaterStyle.ModifyPixelWater(ref _pixelWater);


        
    }

    private void RenderIntoWaterTextureTarget()
    {
        LoadAssets();
        _drawLocation = new Rectangle(0, 0, _waterTextureRTSwap.Width, _waterTextureRTSwap.Height);
        SpriteBatch spriteBatch = Main.spriteBatch;
        
        UpdatePixelWater();
        DrawWaterBase(spriteBatch);
        if(!_pixelWater.noReflection)
            DrawReflection(spriteBatch);
        // 
        DrawPosterization(spriteBatch);
    }

    private void CalculateHeightsToDraw()
    {
        TileDrawing tilesRenderer = Main.instance.TilesRenderer;
        Vector2 unscaledPosition = Main.Camera.UnscaledPosition;
        Vector2 vector = new Vector2((float)Main.offScreenRange, (float)Main.offScreenRange);
        WaterHelpers.GetScreenDrawArea(tilesRenderer, unscaledPosition, vector, out int firstTileX, out int lastTileX, out int firstTileY, out int lastTileY);
        int maxGradientHeight = 32;
        _heightsToDraw.Clear();
        for (int i = firstTileY; i < lastTileY + 4; i++)
        {
            for (int j = firstTileX - 2; j < lastTileX + 2; j++)
            {
                Tile tile = Main.tile[j, i];
                Tile firstAboveTile = Main.tile[j, i - 1];
                if (tile == null)
                    continue;

                int height = 0;
                if (firstAboveTile.LiquidAmount > 0)
                {
                    height++;
                }
                if (tile.LiquidAmount > 0 || firstAboveTile.LiquidAmount > 0)
                {
                    //Move upward until we hit an air tile, so we know how deep this water tile is
                    while (height < maxGradientHeight)
                    {
                        Tile aboveTile = Main.tile[j, i - height];
                        if (aboveTile.LiquidAmount == 0 && !aboveTile.HasTile)
                        {
                            break;
                        }
                        height++;
                    }

                    HeightDraw heightDraw = new HeightDraw();
                    heightDraw.tilePoint = new Vector2(j, i).ToWorldCoordinates(0, 0);

                    //Calculate the height value between 0-1
                    float heightSmoothing = (float)height / (float)maxGradientHeight;
                    heightDraw.height = 1f - heightSmoothing;
                    _heightsToDraw.Add(heightDraw);
                }
            }
        }
    }
    private void RenderIntoHeightMapTarget()
    {
        var _waterEffect = ModContent.Request<Effect>("Stellamod/Effects/MoonWaters").Value;
        if (_waterEffect == null)
            return;

        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        RenderTargetBinding[] binding = new RenderTargetBinding[]
        {
            new RenderTargetBinding(_waterHeightMapRT),
            new RenderTargetBinding(_waterLightMapRT)
        };
        graphicsDevice.SetRenderTargets(binding);
        graphicsDevice.Clear(Color.Transparent);
        Texture2D heightTile = TextureAssets.BlackTile.Value;


        _waterEffect.CurrentTechnique = _waterEffect.Techniques["HeightDrawing"];
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, 
            SamplerState.AnisotropicClamp, DepthStencilState.None,RasterizerState.CullNone, _waterEffect);
        foreach (HeightDraw heightDraw in _heightsToDraw)
        {
            Point lightTilePoint = heightDraw.tilePoint.ToTileCoordinates();
            Vector2 drawPosition = heightDraw.tilePoint - Main.screenPosition;


            Vector3 lightColor = Lighting.GetColor(lightTilePoint).ToVector3();

            Color drawColor = new Color(lightColor.X, lightColor.Y, lightColor.Z, heightDraw.height);

            spriteBatch.Draw(heightTile, drawPosition + new Vector2(Main.offScreenRange), drawColor);
        }
        spriteBatch.End();
        graphicsDevice.SetRenderTarget(null);
    }

}
