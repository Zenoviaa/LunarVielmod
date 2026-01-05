using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Biomes;
using Stellamod.Core.Foreground;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Core.Waters;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Core.MoonWaters
{

    /// <summary>
    /// Default pixel water that looks like the ocean
    /// </summary>
    public class DefaultPixelWaterStyle : PixelWaterStyle
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //Set a priority to negative one so it goes dead last
            priority = -1;
        }
    }
    /// <summary>
    /// Default pixel water that looks like the ocean
    /// </summary>
    public class BeachPixelWaterStyle : PixelWaterStyle
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override bool IsActive(Player player)
        {
            return player.ZoneBeach;
        }
        public override void ModifyPixelWater(ref PixelWater pixelWater)
        {
            base.ModifyPixelWater(ref pixelWater);
            pixelWater.NoLighting = true;
        }
    }

    /// <summary>
    /// Pixel water style for the jungle, with greens, yellows, and leaves in the water!
    /// </summary>
    public class JunglePixelWaterStyle : PixelWaterStyle
    {
        private bool _inMarsh;
        public override bool IsActive(Player player)
        {
            _inMarsh = player.GetModPlayer<BiomePlayer>().ZoneMarsh;
            return player.ZoneJungle || _inMarsh;
        }

        public override void ModifyPixelWater(ref PixelWater pixelWater)
        {
            base.ModifyPixelWater(ref pixelWater);
            pixelWater.StartGradientColor = Color.LightGoldenrodYellow;
            pixelWater.EndGradientColor = Color.Green;
            pixelWater.BackgroundColor = Color.DarkGreen;
            pixelWater.CausticsColor = Color.Yellow * 0.75f;
            pixelWater.CausticsTexture = AssetRegistry.Textures.Noise.Clouds3;
            pixelWater.TilingMultiplier = Vector2.One ;

            if (_inMarsh)
            {
                float lerp = 0.8f;
                pixelWater.StartGradientColor = Color.Lerp(pixelWater.StartGradientColor, Color.LightSkyBlue, 0.8f);
                pixelWater.EndGradientColor = Color.Lerp(pixelWater.EndGradientColor, Color.LightSkyBlue, 0.8f);
                pixelWater.BackgroundColor = Color.Lerp(pixelWater.BackgroundColor, Color.LightSkyBlue, 0.8f);
                pixelWater.CausticsColor = Color.Lerp(pixelWater.CausticsColor, Color.LightSkyBlue, 0.8f);
            }
        }
    }

    /// <summary>
    /// Ice-y pixel water with little crystals in it
    /// </summary>
    public class IcePixelWaterStyle : PixelWaterStyle
    {
        public override bool IsActive(Player player)
        {
            return player.ZoneSnow;
        }

        public override void ModifyPixelWater(ref PixelWater pixelWater)
        {
            base.ModifyPixelWater(ref pixelWater);
            pixelWater.StartGradientColor = Color.White;
            pixelWater.EndGradientColor = Color.Cyan;
            pixelWater.BackgroundColor = Color.DarkBlue;
            pixelWater.CausticsColor = Color.Cyan * 0.75f;
            pixelWater.NoiseTexture = AssetRegistry.Textures.Noise.IceWaterCaustics;
            pixelWater.CausticsTexture = AssetRegistry.Textures.Noise.IceWaterCaustics;
            pixelWater.TilingMultiplier = new Vector2(0.3f, 0.5f);
        }
    }

    /// <summary>
    /// Shimmer pixel water with cool little wiggles in it
    /// </summary>
    public class ShimmerPixelWaterStyle : PixelWaterStyle
    {
        public override bool IsActive(Player player)
        {
            return (player.ZoneShimmer || player.GetModPlayer<MyPlayer>().ZoneWonder);
        }

        public override void ModifyPixelWater(ref PixelWater pixelWater)
        {
            base.ModifyPixelWater(ref pixelWater);
            pixelWater.StartGradientColor = Color.Purple;
            pixelWater.EndGradientColor = Color.DarkBlue;
            pixelWater.BackgroundColor = Color.Blue;
            pixelWater.CausticsTexture = AssetRegistry.Textures.Noise.ShimmerWaterCaustics;
            pixelWater.CausticsColor = Color.Violet;
            pixelWater.TilingMultiplier = new Vector2(1f, 2);
        }
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
        public bool NoLighting;
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

        private ManagedRenderTarget _waterHeightMapRT;
        private ManagedRenderTarget _waterTextureRT;
        private ManagedRenderTarget _waterTextureRTSwap;
        private ManagedRenderTarget _waterTextureRTOutput;
        private ManagedRenderTarget _reflectionRT;
        private ManagedRenderTarget _waterLightMapRT;

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


        //This will give us a cool pixelation effect
        public int DownSamples => 2;
        public Vector2 Tiling => new Vector2(1.5f, 1.5f) * 0.75f;
        public override void Load()
        {
            On_Main.CheckMonoliths += RenderHook;
            On_Main.DrawDust += CopyScreenTarget;
            On_OverlayManager.Draw += ApplyWaterShader;
        }
        public override void Unload()
        {
            base.Unload();
            On_Main.CheckMonoliths -= RenderHook;
            On_Main.DrawDust -= CopyScreenTarget;
            On_OverlayManager.Draw -= ApplyWaterShader;
        }
        public override void OnModLoad()
        {
            base.OnModLoad();
            LoadAssets();
            InitializeRenderTargets();

            //Get all of our available pixel water styles and sort them
            _pixelWater = new PixelWater();
            _pixelWaterStyles = ModContent.GetContent<PixelWaterStyle>().ToArray();
        }


        public ManagedRenderTarget GetReflectionRenderTarget()
        {
            return _reflectionRT;
        }

        private Point GetWaterTargetSize()
        {
            return new Point(Main.waterTarget.Width, Main.waterTarget.Height);
        }
        private void InitializeRenderTargets()
        {
            _reflectionRT = ManagedRenderTarget.New(GetWaterTargetSize, DownSamples);
            _waterTextureRT = ManagedRenderTarget.New(GetWaterTargetSize, DownSamples);
            _waterTextureRTSwap = ManagedRenderTarget.New(GetWaterTargetSize, DownSamples);
            _waterLightMapRT = ManagedRenderTarget.New(GetWaterTargetSize);
            _waterHeightMapRT = ManagedRenderTarget.New(GetWaterTargetSize, surfaceFormat: SurfaceFormat.Alpha8);
            _waterTextureRTOutput = ManagedRenderTarget.New(GetWaterTargetSize);
        }
        private PixelWaterStyle GetActivePixelWaterStyle()
        {
            for(int i = 0; i < _pixelWaterStyles.Length; i++)
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

       //     DrawWaterBaseToScreen();
        }

        private void ApplyWaterShader(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
        {
            orig(self, spriteBatch, layer, beginSpriteBatch);
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.LiquidsToggle)
                return;
            if (Main.gameMenu)
                return;

            if (layer == RenderLayers.ForegroundWater)
            {
                //This is called right before the front water gets drawn
                //We can apply our shader here.
                //It should work, I think
                if (_waterEffect == null)
                    return;

                spriteBatch.End();

                CopyScreenTargetToSwap();
                CopyWaterTarget();
                CopySwapToScreenTarget();

                _waterEffect.CurrentTechnique = _waterEffect.Techniques["CombineRTDrawing"];
                _waterEffect.Parameters["WaterTexture"].SetValue(_waterTextureRTOutput);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                    _waterEffect, Main.Transform);

                Vector2 pos = Main.sceneWaterPos - Main.screenPosition;

                spriteBatch.Draw(Main.waterTarget, pos, Color.White);
                spriteBatch.End();

  
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
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
        }

        private Texture2D LoadTexture(string fileName)
        {
            return ModContent.Request<Texture2D>($"Stellamod/Assets/NoiseTextures/{fileName}").Value;
        }

        private void LoadAssets()
        {
            if (Main.gameMenu)
                return;

            _waterEffect = GameShaders.Misc["LunarVeil:MoonWaters"].Shader;
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

            CalculateHeightsToDraw();
            RenderIntoHeightMapTarget();
            RenderIntoWaterTextureTarget();

        }


        private Type[] _invokeTypes;
        private object[] _invokeParams;
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

                _invokeTypes ??= new Type[]
                {
                        typeof(bool)
                };
                _invokeParams ??= new object[]
                {
                    false
                };
                MethodInfo methodInfo = typeof(Main).GetMethod("DrawWaters", BindingFlags.NonPublic | BindingFlags.Instance, _invokeTypes);
                methodInfo.Invoke(Main.instance, _invokeParams);
            }
            catch
            {
            }
            Main.sceneWaterPos.X = Main.screenPosition.X - (float)Main.offScreenRange;
            Main.sceneWaterPos.Y = Main.screenPosition.Y - (float)Main.offScreenRange;

            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
        }


        private void DrawWaterBase(SpriteBatch spriteBatch)
        {
            _waterEffect.CurrentTechnique = _waterEffect.Techniques["SpriteDrawing"];
            _waterEffect.Parameters["tiling"].SetValue(Vector2.One * 2 * Tiling * _pixelWater.TilingMultiplier);
            _waterEffect.Parameters["time"].SetValue(_time);
            _waterEffect.Parameters["levels"].SetValue(18);
            _waterEffect.Parameters["distortion"].SetValue(0.05f);
            ApplyScreenOffset();

            Vector2 stretchScale = new Vector2(1, 0.5f);
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_waterTextureRT);
            graphicsDevice.Clear(Color.LightSeaGreen);

            //Draw the base texture
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);

            Color baseColor = _pixelWater.BackgroundColor * 0.75f;
            baseColor = baseColor.MultiplyRGB(Main.ColorOfTheSkies);
            spriteBatch.Draw(_waterNoise1, _drawLocation, null, baseColor);
            spriteBatch.End();

            //Brigthten it up a bit
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            spriteBatch.Draw(_pixelWater.NoiseTexture.Value, _drawLocation, null, Color.White * 0.5f);
            spriteBatch.End();
        }

        private void DrawWaterGradient(SpriteBatch spriteBatch)
        {
            //gradient gonna have to be added later

            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            //Set gradient effect values
            _waterEffect.CurrentTechnique = _waterEffect.Techniques["GradientDrawing"];
            _waterEffect.Parameters["startGradient"].SetValue(_pixelWater.StartGradientColor.ToVector3());
            _waterEffect.Parameters["endGradient"].SetValue(_pixelWater.EndGradientColor.ToVector3());

            //Draw gradient
            graphicsDevice.SetRenderTarget(_waterTextureRTSwap);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            spriteBatch.Draw(_waterTextureRT, _drawLocation, null, Color.White);
            spriteBatch.End();
        }

        private void DrawWaterCaustics(SpriteBatch spriteBatch)
        {
            _waterEffect.CurrentTechnique = _waterEffect.Techniques["CausticsDrawing"];
            _waterEffect.Parameters["time"].SetValue(_time * 2);
            _waterEffect.Parameters["distortion"].SetValue(0.05f);
            _waterEffect.Parameters["tiling"].SetValue(Vector2.One * 6 * Tiling * _pixelWater.TilingMultiplier);
            ApplyScreenOffset();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            spriteBatch.Draw(_pixelWater.CausticsTexture.Value, _drawLocation, null, _pixelWater.CausticsColor);
            spriteBatch.End();
        }

        private void DrawWaterSparkle(SpriteBatch spriteBatch)
        {
            _waterEffect.CurrentTechnique = _waterEffect.Techniques["SparklingCausticsDrawing"];
            _waterEffect.Parameters["time"].SetValue(_time * 2);
            _waterEffect.Parameters["distortion"].SetValue(0.05f);
            _waterEffect.Parameters["tiling"].SetValue(Vector2.One * 8 * Tiling * _pixelWater.TilingMultiplier);
            _waterEffect.Parameters["HeightMapTexture"].SetValue(_waterHeightMapRT);
            ApplyScreenOffset();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            spriteBatch.Draw(_pixelWater.CausticsTexture.Value, _drawLocation, null, Color.White * 0.5f);
            spriteBatch.End();
        }

        private void DrawWaterFoam(SpriteBatch spriteBatch)
        {
            _waterEffect.CurrentTechnique = _waterEffect.Techniques["FoamDrawing"];
            _waterEffect.Parameters["time"].SetValue(_time * 2);
            _waterEffect.Parameters["distortion"].SetValue(0.05f);
            _waterEffect.Parameters["tiling"].SetValue(Vector2.One * 2 * Tiling * _pixelWater.TilingMultiplier);
            _waterEffect.Parameters["HeightMapTexture"].SetValue(_waterHeightMapRT);
            ApplyScreenOffset();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            spriteBatch.Draw(_perlinNoise, _drawLocation, null, Color.White);
            spriteBatch.End();
        }

        private Vector2 _movement;
        private void ApplyScreenOffset()
        {
            Vector2 screenOffset = Main.screenPosition;
            Vector2 diff = Main.screenPosition - Main.screenLastPosition;
            _movement += diff;
            //Ok so the screen position is indeed in pixels that's good
            //One pixel movement on the world needs to translate to one pixel movement on this water texture

            Vector2 texelSize = (Vector2.One) / new Vector2(_waterTextureRT.Width, _waterTextureRT.Height );

            //I have no clue what this number is or how to calculate it 
            //All I know is that there is a number somewhere around here that converts the world pixel space to local pixel space correctly
            //And makes the thing move, 0.28 is just an approximation so it still moves VERY slightly
            //Once I figure out how to calculate this and what it is I'll change it :P
            float pixelSpaceFixer = 0.28f;
            _waterEffect.Parameters["screenOffset"].SetValue(_movement * texelSize * 0.29f);
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
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
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

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            spriteBatch.Draw(_waterTextureRTSwap, _drawLocation, null, Color.White * 1f);
            spriteBatch.End();


            if (!_pixelWater.NoLighting)
            {
                _waterEffect.CurrentTechnique = _waterEffect.Techniques["BlurDrawing"];
                spriteBatch.Begin(SpriteSortMode.Deferred, CustomBlendState.Multiply, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
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
            _drawLocation = new Rectangle(0, 0, _waterTextureRT.Width, _waterTextureRT.Height);
            SpriteBatch spriteBatch = Main.spriteBatch;

            UpdatePixelWater();

            DrawWaterBase(spriteBatch);
    
            DrawWaterGradient(spriteBatch);
            DrawWaterCaustics(spriteBatch);

            DrawWaterSparkle(spriteBatch);
            DrawWaterFoam(spriteBatch);
            DrawReflection(spriteBatch);
            // 
            DrawPosterization(spriteBatch);
        }

        private void CalculateHeightsToDraw()
        {
            //Get the tile drawing area
            TileDrawing tilesRenderer = Main.instance.TilesRenderer;
            Vector2 unscaledPosition = Main.Camera.UnscaledPosition;
            Vector2 vector = new Vector2((float)Main.offScreenRange, (float)Main.offScreenRange);
            object[] args = new object[] { unscaledPosition, vector + (Main.Camera.UnscaledPosition - Main.Camera.ScaledPosition), null, null, null, null };
            typeof(TileDrawing).GetMethod("GetScreenDrawArea", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(tilesRenderer, args);

            int maxGradientHeight = 32;
            _heightsToDraw.Clear();
            for (int i = (int)args[4]; i < (int)args[5] + 4; i++)
            {
                for (int j = (int)args[2] - 2; j < (int)args[3] + 2; j++)
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
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, _waterEffect);
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

        private void RenderIntoTileLightTarget()
        {
            /*
            Texture2D heightTile = TextureAssets.BlackTile.Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_waterLightMapRT);
            graphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            foreach (HeightDraw heightDraw in _heightsToDraw)
            {
                Point lightTilePoint = heightDraw.tilePoint.ToTileCoordinates();
                Color lightColor = Lighting.GetColor(lightTilePoint);
                Vector2 drawPosition = heightDraw.tilePoint - Main.screenPosition;
                spriteBatch.Draw(heightTile, drawPosition + new Vector2(Main.offScreenRange), lightColor);
            }
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);*/
        }
        private void DrawWaterBaseToScreen()
        {
            //This is just for testing purposes to make sure the texture looks the way we want it to

            Main.graphics.GraphicsDevice.Clear(Color.Transparent);
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(_waterTextureRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2, SpriteEffects.None, 0f);
            spriteBatch.End();
        }


        private void DrawWaterTargetToScreen()
        {
            //This is just for testing purposes to make sure the texture looks the way we want it to
            if (Main.gameMenu)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            var device = spriteBatch.GraphicsDevice;
            device.SetRenderTarget(null);
            device.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(_waterLightMapRT, -new Vector2(Main.offScreenRange), null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.End();
        }

        private void DrawHeightMapToScreen()
        {
            //This is just for testing purposes to make sure the texture looks the way we want it to
            if (Main.gameMenu)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            var device = spriteBatch.GraphicsDevice;
            device.SetRenderTarget(null);
            device.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(_waterHeightMapRT, -new Vector2(Main.offScreenRange), null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.End();
        }
        private void DrawLightMapToScreen()
        {
            //This is just for testing purposes to make sure the texture looks the way we want it to
            if (Main.gameMenu)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            var device = spriteBatch.GraphicsDevice;
            device.SetRenderTarget(null);
            device.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(_waterLightMapRT, -new Vector2(Main.offScreenRange), null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.End();
        }
        private void DrawWaterTextureToScreen()
        {
            //This is just for testing purposes to make sure the texture looks the way we want it to
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(_waterTextureRTOutput, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.End();
        }

    }
}
