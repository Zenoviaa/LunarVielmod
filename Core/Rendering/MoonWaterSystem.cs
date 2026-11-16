using Microsoft.Xna.Framework.Graphics.PackedVector;
using MonoMod.Cil;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Assets.ContentReader.Pal;
using Stellamod.Common;
using Stellamod.Common.Metaballs;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.Biomes;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.Rendering.RTs;
using Stellamod.WorldG;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Liquid;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
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
        Palette = null;
        TilingMultiplier = Vector2.One;
        affectsLava = false;
        noLighting = false;
        vibrant = false;
        ignoreSkyColor = false;
        noReflection = false;
        reflectionAlpha = 1f;
        maxHeight = 32;
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
    public Palette Palette;
    public bool noLighting;
    public bool vibrant;
    public bool ignoreSkyColor;
    public bool affectsLava;
    public bool noReflection;
    public float reflectionAlpha;
    public int maxHeight;
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
    private Vector3[] _surfaceLiquidPointsBuffer = new Vector3[252];
    private int _surfaceLiquidPointsLength;
    private WaterSurfacePointLightMap _waterSurfacePointLightMap;
    private readonly HashSet<Point> _edgeWaterPoints = new();
    private LazyRenderTargetProvider _reflectionRT = new(RenderTargetParameters.DownsizedFunc(GetWaterTargetSize, 2));
    private LazyRenderTargetProvider _waterTextureRT = new(RenderTargetParameters.DownsizedFunc(GetWaterTargetSize, 2));
    private LazyRenderTargetProvider _waterTextureRTSwap = new(RenderTargetParameters.DownsizedFunc(GetWaterTargetSize, 2));

    private LazyRenderTargetProvider _waterTextureRTOutput = new(RenderTargetParameters.DownsizedFunc(GetWaterTargetSize, 1));
    private LazyRenderTargetProvider _waterLightMapRT = new(RenderTargetParameters.DownsizedFunc(GetWaterTargetSize, 1));

    private LazyRenderTargetProvider _waterHeightMapRT = new(RenderTargetParameters.DownsizedFunc(GetWaterTargetSize, 1));

    private LazyRenderTargetProvider _waterEdgeShadingRT = new(() =>
    {
        RenderTargetParameters p = RenderTargetParameters.DefaultScreenTarget;
        p.Width = Main.instance.tileTarget.Width;
        p.Height = Main.instance.tileTarget.Height;
        p.SurfaceFormat = SurfaceFormat.Alpha8;
        return p;
    });

    private PixelWaterStyle[] _pixelWaterStyles;
    private PixelWaterStyle _activePixelWaterStyle;
    private PixelWater _pixelWater;
    private PixelWaterStyleComparer _pixelWaterComparer;

    private List<HeightDraw> _heightsToDraw = new();
    private List<Rectangle> _gradientRects = new();

    private float _time;
    private Effect _waterEffect;
    private Rectangle _drawLocation;
    private Texture2D _perlinNoise;
    private Texture2D _waterNoise1;
    //This will give us a cool pixelation effect
    public int DownSamples => 2;
    public Vector2 Tiling => new Vector2(1.5f, 1.5f) * 0.75f;
    public float waterAlpha;
    public static event Action<SpriteBatch> DrawWaterMask;
    public override void Load()
    {
        IL_Main.DoDraw += DrawOverWaterTarget;
        On_Main.DrawDust += CopyScreenTarget;
        On_Main.RenderWater += RenderCoolWaters;
    }

    private void DrawOverWaterTarget(ILContext il)
    {
        try
        {

            ILCursor c = new ILCursor(il);

            if (c.TryGotoNext(
                MoveType.After,
                i => i.MatchCall<Main>($"{nameof(Main.DrawWires)}")
            ))
            {
                if (c.TryGotoNext(
                    MoveType.After,
                    i => i.MatchCallvirt<SpriteBatch>($"{nameof(SpriteBatch.Draw)}")
                ))
                {
                    c.EmitDelegate(NewDraw);
                }
            }


        }
        catch (Exception)
        {
            MonoModHooks.DumpIL(ModContent.GetInstance<Stellamod>(), il);
        }
    }

    void NewDraw()
    {
  
        if (!LunarVeilClientConfig.Instance.LiquidsToggle)
            return;
        if (_waterEffect == null)
            return;

        var sb = Main.spriteBatch;
        Vector2 pos = Main.sceneWaterPos - Main.screenPosition;
        sb.EndOut(out var oldParameters);
        using var screenTargetOne = RT.Context(RenderTargets.ScreenTarget);

        using var coolAssWaterTarget = RT.Context(RenderTargets.WaterTarget);

        using var screenTargetTwo = RT.Context(RenderTargets.ScreenTarget);
        using var reflectionGradientRT = RT.Context(RenderTargets.ScreenTarget);
        using (RT.Clear(screenTargetTwo, Color.Transparent))
        {
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);
            sb.Draw(_reflectionRT, Vector2.Zero - new Vector2(Main.offScreenRange), null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
            sb.End();

        }
        using (RT.Clear(reflectionGradientRT, Color.Transparent))
        {
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);
            sb.Draw(_waterHeightMapRT, Main.sceneWaterPos - Main.screenPosition, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            sb.End();

        }


        //Posterization needs to happen after the reflection gets drawn.
        //It should be the last thing that happens

        using (RT.Clear(coolAssWaterTarget, Color.Transparent))
        {
            UpscaleCoolAssTarget(_waterTextureRTSwap, sb);

            //  DrawPosterization(prePosterized.Target, sb);
        }

        using (RT.Clear(screenTargetOne, Color.Transparent))
        {


            if (_pixelWater.affectsLava)
            {
                //      Main.NewText("yuh");

                SuperLavaShader lavaShader = ShaderContent.GetInstance<SuperLavaShader>();
                lavaShader.NormalNoiseTexture = coolAssWaterTarget;
                lavaShader.HeightMap = _waterHeightMapRT;
                lavaShader.OutlineColor = Color.Lerp(Color.White, Color.Goldenrod, 0.3f);
                lavaShader.OutlineTexelSize = Main.waterTarget;
                lavaShader.Effect.CurrentTechnique = lavaShader.Effect.Techniques["Combine"];
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                    lavaShader.Effect, Main.Transform);

                sb.Draw(Main.waterTarget, pos, Color.White * waterAlpha);


                Color c = Color.White * 0.3f;
                c.A = 0;
                sb.Draw(Main.waterTarget, pos, c * waterAlpha);
                sb.End();
            }
            else
            {
                if (_pixelWater.Palette != null)
                {
                    _waterEffect.CurrentTechnique = _waterEffect.Techniques["CombinePaletteRTDrawing"];
                    _waterEffect.Parameters["ColorSpectrumTexture"].SetValue(_pixelWater.Palette.ColorAtlas);
                    _waterEffect.Parameters["EdgeTexture"].SetValue(_waterEdgeShadingRT);
                }
                else
                {
                    _waterEffect.CurrentTechnique = _waterEffect.Techniques["CombineRTDrawing"];
                }

                _waterEffect.Parameters["WaterTexture"].SetValue(coolAssWaterTarget);

                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                    _waterEffect, Main.Transform);
                sb.Draw(Main.waterTarget, pos, Color.White * waterAlpha);

                Color c = Color.White * 0.43f;
                c.A = 0;
                sb.Draw(Main.waterTarget, pos, c * waterAlpha);
                sb.End();

            }

            if (!_pixelWater.noReflection)
            {
             //   DrawReflection(sb);
                
                Effect reflectionCombineEffect = GameShaders.Misc["LunarVeil:SingularReflection"].Shader;
                float mipBias = 1;
                float reflectionDistance = 256;
                Vector2 reflectionTexelSize = (Vector2.One * mipBias) / new Vector2(screenTargetTwo.Width, screenTargetTwo.Height);

                reflectionCombineEffect.Parameters["reflectionDistance"].SetValue(reflectionDistance);
                reflectionCombineEffect.Parameters["reflectionTexelSize"].SetValue(reflectionTexelSize);
                reflectionCombineEffect.Parameters["reflectionPower"].SetValue(4);
                reflectionCombineEffect.Parameters["HeightMapTexture"].SetValue(reflectionGradientRT);

                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, reflectionCombineEffect);
                sb.Draw(screenTargetTwo, Vector2.Zero, Color.White);
                sb.End();
            }

        }

        using (RT.Clear(screenTargetTwo, Color.Transparent))
        {
            //Finally draw to the screen posterized
            _drawLocation = new Rectangle(0, 0, _waterHeightMapRT.Width, _waterHeightMapRT.Height);

            _waterEffect.CurrentTechnique = _waterEffect.Techniques["PosterizeDrawing"];
            _waterEffect.Parameters["levels"].SetValue(10);
            sb.Begin(oldParameters with { effect = _waterEffect });
            sb.Draw(screenTargetOne, Vector2.Zero, null, Color.White * 1f);
            sb.End();

            if (!_pixelWater.noLighting)
            {
                _waterEffect.CurrentTechnique = _waterEffect.Techniques["BlurDrawing"];
                sb.Begin(SpriteSortMode.Deferred, CustomBlendStates.Multiply, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
                sb.Draw(_waterLightMapRT, Main.sceneWaterPos - Main.screenPosition, null, Color.White * 1);
                sb.End();
            }
        }

        sb.Begin(oldParameters);
        sb.Draw(screenTargetTwo, Vector2.Zero, null, Color.White * 1f);

    }

    private void RenderCoolWaters(On_Main.orig_RenderWater orig, Main self)
    {
        orig(self);
        RenderFancyWaters();
    //    RenderIntoEdgeShadeMap();
    }

    public override void Unload()
    {
        base.Unload();

        _pixelWaterStyles = null;
        _heightsToDraw.Clear();
        _gradientRects.Clear();
    }
    public override void ClearWorld()
    {
        base.ClearWorld();
        _waterSurfacePointLightMap = new(252, 8, Main.maxTilesX, Main.maxTilesY);
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
        using(RT.Clear(_reflectionRT.GetTarget(), Color.Black))
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero + new Vector2(Main.offScreenRange) / 2f, null, Color.White, 0, Vector2.Zero, 1f / DownSamples, SpriteEffects.None, 0f);
            spriteBatch.End();
        }

    }


    private void CalculateEdgePoints()
    {
        _edgeWaterPoints.Clear();
        (Point topLeft, Point bottomRight) = TileUtilities.CameraTileBounds(382, inside: 2);
        for (int x = topLeft.X; x < bottomRight.X; x++)
        {
            for (int y = topLeft.Y; y < bottomRight.Y; y++)
            {
                Tile tile = Main.tile[x, y];
                if (!tile.HasTile && tile.LiquidAmount > 0)
                {
                    Tile tileLeft = Main.tile[x - 1, y];
                    Tile tileRight = Main.tile[x + 1, y];
                    Tile tileBottom = Main.tile[x, y + 1];

                    if (tileLeft.HasTile && Main.tileSolid[tileLeft.TileType])
                    {
                        _edgeWaterPoints.Add(new Point(x - 1, y));
                    }

                    if (tileRight.HasTile && Main.tileSolid[tileRight.TileType])
                    {
                        _edgeWaterPoints.Add(new Point(x + 1, y));
                    }

                    if (tileBottom.HasTile && Main.tileSolid[tileBottom.TileType])
                    {
                        _edgeWaterPoints.Add(new Point(x, y + 1));
                    }
                }
            }
        }
    }

    private void RenderIntoEdgeShadeMap()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        graphicsDevice.SetRenderTarget(_waterEdgeShadingRT);
        graphicsDevice.Clear(Color.Transparent);
        if (Main.GameUpdateCount % 30 == 0)
        {
            CalculateEdgePoints();
        }

        spriteBatch.Begin();
        SpritebatchDrawer glowDrawer = SpritebatchDrawer.FromTextureGetLightingColor(
            AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, Vector2.Zero);
        glowDrawer.scale *= 0.2f;
        glowDrawer.color = Color.White * 0.13f;
        glowDrawer.color.A = 0;
        foreach (Point edgeWaterPoint in _edgeWaterPoints)
        {
            Vector2 pos = edgeWaterPoint.ToWorldCoordinates();
            pos += new Vector2(Main.offScreenRange);
            glowDrawer.worldPosition = pos;
            spriteBatch.Draw(glowDrawer);
        }
        spriteBatch.End();
        graphicsDevice.SetRenderTarget(null);
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

    int _renderCountdown;
    void RenderFancyWaters()
    {
        if (!LunarVeilClientConfig.Instance.LiquidsToggle)
            return;
        if (Main.gameMenu)
            return;
        _waterEffect = ModContent.Request<Effect>("Stellamod/Effects/MoonWaters").Value;
        if (_waterEffect == null)
            return;
        if (!Lighting.UsingNewLighting)
            return;
        _renderCountdown--;
        //V1 Gradientizing
        void V1HeightGradient()
        {
            if (_renderCountdown <= 0)
            {
                CalculateHeightsToDraw();
                _renderCountdown = 3;
            }
            RenderIntoHeightMapTarget();
        }

        void V2HeightGradient()
        {
            //_waterSurfacePointLightMap = new();
            if (_renderCountdown <= 0)
            {
                WaterSurfaceScanner.Scan(TileUtilities.CameraTileRectangle(384), ref _surfaceLiquidPointsBuffer, ref _surfaceLiquidPointsLength);
                _waterSurfacePointLightMap.ApplyWaterSurfacePoints(_surfaceLiquidPointsBuffer, _surfaceLiquidPointsLength);
                _renderCountdown = 3;
            }

            RenderIntoHeightMapTargetV2();
        }

        //V2 Gradientizing
        if (Main.LocalPlayer.ZoneAbyss)
        {
            V2HeightGradient();
        }
        else
            V1HeightGradient();
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
        WaterHelpers.DrawWaters(Main.instance, isBackground: false);

        Main.sceneWaterPos.X = Main.screenPosition.X - Main.offScreenRange;
        Main.sceneWaterPos.Y = Main.screenPosition.Y - Main.offScreenRange;

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
            using (RT.Clear(_waterTextureRTSwap, Color.DarkGoldenrod))
            {

                //Draw the base texture
                spriteBatch.Begin(SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.AnisotropicClamp,
                    DepthStencilState.None,
                    RasterizerState.CullNone,
                    lavaShader.Effect);

                Color drawColor = _pixelWater.BackgroundColor * 0.8f;
                if (!_pixelWater.ignoreSkyColor)
                {
                    drawColor = drawColor.MultiplyRGB(Main.ColorOfTheSkies);
                }
                spriteBatch.Draw(_pixelWater.CausticsTexture.Value, _drawLocation, null, drawColor);
                spriteBatch.End();
            }
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
        _waterEffect.Parameters["HeightMapTexture"].SetValue(_waterHeightMapRT);


        _waterEffect.Parameters["foamLava"].SetValue(_pixelWater.affectsLava ? 0 : 1);
        ApplyScreenOffset(scale: 2);
        Vector2 stretchScale = new Vector2(1, 0.5f);
        using(RT.Clear(_waterTextureRTSwap, Color.LightSeaGreen))
        {
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

    }

    private void DrawReflection(SpriteBatch spriteBatch)
    {
        _drawLocation = new Rectangle(0, 0, _waterTextureRT.Width, _waterTextureRT.Height);

        float mipBias = 1;
        float reflectionDistance = 128;
        Vector2 reflectionTexelSize = (Vector2.One * mipBias) / new Vector2(_reflectionRT.Width, _reflectionRT.Height);


        _waterEffect.CurrentTechnique = _waterEffect.Techniques["ReflectionDrawing"];
        _waterEffect.Parameters["reflectionDistance"].SetValue(reflectionDistance);
        _waterEffect.Parameters["reflectionTexelSize"].SetValue(reflectionTexelSize);
        _waterEffect.Parameters["reflectionPower"].SetValue(3.5f);
        _waterEffect.Parameters["HeightMapTexture"].SetValue(_waterHeightMapRT);


        _waterEffect.Parameters["time"].SetValue(_time * 2);
        _waterEffect.Parameters["distortion"].SetValue(0.005f);
        _waterEffect.Parameters["NoiseTexture"].SetValue(_pixelWater.CausticsTexture.Value);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
        spriteBatch.Draw(_reflectionRT, new Vector2(-Main.offScreenRange), null, Color.White * _pixelWater.reflectionAlpha, 0, Vector2.Zero, new Vector2(2f, 2f), SpriteEffects.None, 0);
        spriteBatch.End();
    }
    private void MultiplyLightMap(RenderTarget2D target, SpriteBatch spriteBatch)
    {
        _drawLocation = new Rectangle(0, 0, _waterHeightMapRT.Width, _waterHeightMapRT.Height);

        if (!_pixelWater.noLighting)
        {
            _waterEffect.CurrentTechnique = _waterEffect.Techniques["BlurDrawing"];
            spriteBatch.Begin(SpriteSortMode.Deferred, CustomBlendStates.Multiply, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            spriteBatch.Draw(_waterLightMapRT, _drawLocation, null, Color.White * 1);
            spriteBatch.End();
        }
    }
    private void UpscaleCoolAssTarget(RenderTarget2D target, SpriteBatch spriteBatch)
    {
        _drawLocation = new Rectangle(0, 0, _waterHeightMapRT.Width, _waterHeightMapRT.Height);
        
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null);
        spriteBatch.Draw(target, _drawLocation, null, Color.White * 1f);
        spriteBatch.End();
    }

    private void DrawPosterizationNoUpscale(RenderTarget2D target, SpriteBatch spriteBatch)
    {
     
    }

    private void DrawPosterization(RenderTarget2D target, SpriteBatch spriteBatch)
    {
        _drawLocation = new Rectangle(0, 0, _waterHeightMapRT.Width, _waterHeightMapRT.Height);
        
        _waterEffect.CurrentTechnique = _waterEffect.Techniques["PosterizeDrawing"];
        _waterEffect.Parameters["levels"].SetValue(10);

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
        spriteBatch.Draw(target, _drawLocation, null, Color.White * 1f);
        spriteBatch.End();

        if (!_pixelWater.noLighting)
        {
            _waterEffect.CurrentTechnique = _waterEffect.Techniques["BlurDrawing"];
            spriteBatch.Begin(SpriteSortMode.Deferred, CustomBlendStates.Multiply, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            spriteBatch.Draw(_waterLightMapRT, _drawLocation, null, Color.White * 1);
            spriteBatch.End();
        }
    }

    private void DrawPosterization(SpriteBatch spriteBatch)
    {
        _drawLocation = new Rectangle(0, 0, _waterHeightMapRT.Width, _waterHeightMapRT.Height);

        _waterEffect.CurrentTechnique = _waterEffect.Techniques["PosterizeDrawing"];
        _waterEffect.Parameters["levels"].SetValue(10);

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
        spriteBatch.Draw(_waterTextureRTSwap, _drawLocation, null, Color.White * 1f);
        spriteBatch.End();

        if (!_pixelWater.noLighting)
        {
            _waterEffect.CurrentTechnique = _waterEffect.Techniques["BlurDrawing"];
            spriteBatch.Begin(SpriteSortMode.Deferred, CustomBlendStates.Multiply, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            spriteBatch.Draw(_waterLightMapRT, _drawLocation, null, Color.White * 1);
            spriteBatch.End();
        }
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

        /*
        if (!_pixelWater.noReflection)
            DrawReflection(spriteBatch);
        DrawPosterization(spriteBatch);*/
    }

    private void CalculateHeightsToDraw()
    {
        TileDrawing tilesRenderer = Main.instance.TilesRenderer;
        Vector2 unscaledPosition = Main.Camera.UnscaledPosition;
        Vector2 vector = new Vector2(Main.offScreenRange, Main.offScreenRange);
        WaterHelpers.GetScreenDrawArea(tilesRenderer, unscaledPosition, vector, out int firstTileX, out int lastTileX, out int firstTileY, out int lastTileY);
        int maxGradientHeight = _pixelWater.maxHeight;
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
                    if (Main.LocalPlayer.ZoneOverworldHeight)
                    {
                        while (height < maxGradientHeight)
                        {
                            Tile aboveTile = Main.tile[j, i - height];
                            if (aboveTile.LiquidAmount == 0 && !aboveTile.IsHalfBlock)
                            {
                                break;
                            }
                            height++;
                        }

                    }
                    else
                    {
                        Point waterSurface = VeilGen.FindSurfaceOfWater(new Point(j, i), 16);

                        while (height < maxGradientHeight)
                        {
                            Tile aboveTile = Main.tile[j, i - height];
                            if (aboveTile.LiquidAmount == 0 && !WorldGen.SolidTile(j, i - height) && !aboveTile.IsHalfBlock)
                            {
                                break;
                            }
                            height++;
                        }

                    }

                    HeightDraw heightDraw = new HeightDraw();
                    heightDraw.tilePoint = new Vector2(j, i).ToWorldCoordinates(0, 0);

                    //Calculate the height value between 0-1
                    float heightSmoothing = height / (float)maxGradientHeight;
                    heightDraw.height = 1f - heightSmoothing;
                    _heightsToDraw.Add(heightDraw);
                }
            }
        }
    }
    void DrawLavaballs(SpriteBatch spriteBatch)
    {
        using var rt = RT.Context(RenderTargets.ScreenTarget);
        MetaballRenderer.PrepareMetaballContent(rt, LavaMetaballRenderer.metaballs);
        spriteBatch.Begin();
        spriteBatch.Draw(rt, Vector2.Zero, Color.White * 0.4F);
        spriteBatch.End();
    }

    private void RenderIntoHeightMapTargetV2()
    {
        //For soem reason we need to apply the tile offset afterwards and can't just do it on the original draw??
        //I'm so confused ngl
        //I'll look again at this later since it feels like an unnecessary render target use, wait,
        //Make we could just translate it after it projects?
        GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
        using var mask = RT.Context(RenderTargets.ScreenTarget);
        using (RT.Clear(mask, Color.Transparent))
        {
            var waterlightPass = AssetReferences.Effects.Generic.WaterLight.CreatePixelPass();
            waterlightPass.Parameters.transformMatrix = TrailDrawer.UnscaledWorldViewPoint;
            waterlightPass.Apply();
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
            graphicsDevice.BlendState = CustomBlendStates.Max;
            graphicsDevice.DepthStencilState = DepthStencilState.None;
            _waterSurfacePointLightMap.Draw(graphicsDevice);
        }

        using(RT.Clear(_waterHeightMapRT, Color.Transparent))
        {

            var sb = Main.spriteBatch;
            sb.Begin();

            sb.Draw(mask, new Vector2(Main.offScreenRange), Color.White);
            sb.End();

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
        using(RT.Clear(binding, Color.Transparent))
        {
            Texture2D heightTile = TextureAssets.BlackTile.Value;


            _waterEffect.CurrentTechnique = _waterEffect.Techniques["HeightDrawing"];
            _waterEffect.Parameters["maxDepth"].SetValue(_pixelWater.maxHeight);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            foreach (HeightDraw heightDraw in _heightsToDraw)
            {
                Point lightTilePoint = heightDraw.tilePoint.ToTileCoordinates();
                Vector2 drawPosition = heightDraw.tilePoint - Main.screenPosition;


                Vector3 lightColor = Lighting.GetColor(lightTilePoint).ToVector3();
                if (_pixelWater.noLighting)
                    lightColor = Vector3.One;
                Color drawColor = new Color(lightColor.X, lightColor.Y, lightColor.Z, heightDraw.height);

                spriteBatch.Draw(heightTile, drawPosition + new Vector2(Main.offScreenRange), drawColor);
            }

            spriteBatch.End();
            // DrawLavaballs(spriteBatch);
            /*
            _waterEffect.CurrentTechnique = _waterEffect.Techniques["ParticleHeightDrawing"];
            spriteBatch.Begin(SpriteSortMode.Deferred, CustomBlendStates.Max,
                SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);

            var rawer = SpritebatchDrawer.FromTextureWhite(AssetReferences.Assets.GlowMasks.WhiteSquare.Asset);
            rawer.worldPosition = Main.LocalPlayer.Center;
            rawer.scale *= 6;

            spriteBatch.Draw(rawer);
            spriteBatch.End();*/
        }
    }

}
