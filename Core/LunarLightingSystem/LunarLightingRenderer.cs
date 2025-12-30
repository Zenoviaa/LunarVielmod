
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{

    public class LightingPreDrawEdit : GlobalTile
    {
        public static bool DontRenderPreDraw;
        public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            if (type == TileID.FogMachine && DontRenderPreDraw)
                return false;
            if (type == TileID.FogMachine && NPC.AnyDanger())
                return false;
            return base.PreDraw(i, j, type, spriteBatch);
        }
    }
    [Autoload(Side = ModSide.Client)]
    public class LunarLightingRenderer : ModSystem,
        IPostProcessingPass
    {
        public int PostProcessPriority => 15;

        private static Color _backLightColor;
        private static Vector2 _previousScreenSize;
        private static RenderTarget2D _accumulatedLightRT;

        private static bool _initAtlas;
        private static RenderTarget2D _pointLightRT;
        private static RenderTarget2D _tempLightMapAtlasRT;
        private static RenderTarget2D _tileShadowMap;
        private static RenderTarget2D _tileBlurRT;
        private static RenderTarget2D _tileSunShadowRT;

        private static List<ILightEmitter> _emitters;
        private static List<IBackLightModifier> _backLightModifiers;

        private static int _ambientLightIndex;
        private static TileAmbientLight[] _ambientLights = new TileAmbientLight[Max_Ambient_Lights];

        public static Color BackLightColor;
        public static Color SunColor;
        public static Vector3 AmbientLight;
        public const int Max_Ambient_Lights = 2000;

        public static RenderTarget2D GetAccumulatedLightMap()
        {
            return _accumulatedLightRT;
        }

        public override void Load()
        {

            _backLightModifiers = new List<IBackLightModifier>();
            _emitters = new List<ILightEmitter>();
            ResizeRenderTarget(true);


            On_Main.CheckMonoliths += RenderToLightMaps;
            On_Main.DoDraw += LightRenderLoop;
            On_Main.DrawCachedNPCs += DrawShadowsBehindTiles;
        }
        public override void Unload()
        {
            base.Unload();
            On_Main.CheckMonoliths -= RenderToLightMaps;
            On_Main.DoDraw -= LightRenderLoop;
            On_Main.DrawCachedNPCs -= DrawShadowsBehindTiles;

        }

        private void RenderToLightMaps(On_Main.orig_CheckMonoliths orig)
        {
            RenderLightsV2();
            orig();
        }

        private void DrawShadowsBehindTiles(On_Main.orig_DrawCachedNPCs orig, Main self, List<int> npcCache, bool behindTiles)
        {
            if (behindTiles && DrawSunShadows2())
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.Draw(_tileSunShadowRT, Vector2.Zero, Color.White);
            }
            orig(self, npcCache, behindTiles);
        }


        private void DrawShadowsBehindTiles(On_Main.orig_DoDraw_DrawNPCsBehindTiles orig, Main self)
        {
            //Just draw it
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin();
            spriteBatch.Draw(_tileSunShadowRT, Vector2.Zero, Color.White);
            spriteBatch.End();
            orig(self);
        }

        public override void OnModLoad()
        {
            base.OnModLoad();
            PostProcessingRenderer.AddPass(this);
        }

        public override void ClearWorld()
        {
            base.ClearWorld();
            ClearLightingData();
        }

        private static void ClearLightingData()
        {
            _emitters.Clear();
            _backLightModifiers.Clear();
            ClearAmbientLights();
        }

        private static void DrawToScreen()
        {
            if (!ShouldRender())
                return;


            //PreviewLightMaps();
            DrawAccumulatedLightMapToScreen();
            DrawSoftGlows();
        }

        private static void DrawAtlasToScreen()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Rectangle screenRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            spriteBatch.Draw(_tempLightMapAtlasRT, screenRectangle, Color.White);
            spriteBatch.End();
        }

        private static void DrawAccumulatedLightMapToScreen()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, CustomBlendStates.Multiply);
            spriteBatch.Draw(_accumulatedLightRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.End();
        }


        private static bool DrawSunShadows2()
        {
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            return config.SunShadows;
        }

        private static void DrawTileShadowMapToScreen()
        {
            if (Main.gameMenu)
                return;

            if (!DrawSunShadows2())
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(_tileSunShadowRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.End();

        }
        private static void DrawSoftGlows()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            Texture2D glowTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SoftGlow").Value;
            Vector2 drawOrigin = glowTexture.Size() / 2f;

            for (int i = 0; i < PointLightManager.MAX_POINT_LIGHTS; i++)
            {
                if (PointLightManager.LightStates[i] == PointLightState.INACTIVE)
                    continue;

                ref PointLightData pointLightData = ref PointLightManager.PointLights[i];
                Vector2 drawPosition = pointLightData.position - Main.screenPosition;

                Point tilePosition = (pointLightData.position - new Vector2(8, 8)).ToTileCoordinates();
                Tile tile = Main.tile[tilePosition.X, tilePosition.Y];

                Color drawColor = Lighting.GetColor(tilePosition.X, tilePosition.Y);
                drawColor *= ExtraMath.Osc(0.9f, 1f, speed: 2, offset: tilePosition.X + tilePosition.Y);
                drawColor *= 0.23f;
                spriteBatch.Draw(glowTexture, drawPosition, null, drawColor, 0, drawOrigin, 2, SpriteEffects.None, 0);
            }

            spriteBatch.End();
        }

        private static void PreviewLightMaps()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);


            for (int i = 0; i < PointLightManager.MAX_POINT_LIGHTS; i++)
            {
                //These lights won't be iterated over/drawn
                switch (PointLightManager.LightStates[i])
                {
                    case PointLightState.CUSTOM:
                    case PointLightState.INACTIVE:
                    case PointLightState.NEEDS_UPDATING:
                    case PointLightState.NEEDS_BAKING:
                        continue;
                }

                ref PointLightData pointLightData = ref PointLightManager.PointLights[i];
                Rectangle atlasRectangle = PointLightManager.LightAtlasRectangles[i];
                Vector2 position = pointLightData.position;
                Vector2 drawOrigin = atlasRectangle.Size() / 2f;
                float scale = PointLightManager.POINT_LIGHT_DOWN_SAMPLES;
                spriteBatch.Draw(_tempLightMapAtlasRT, position - Main.screenPosition, atlasRectangle, Color.White, 0, drawOrigin, scale, SpriteEffects.None, 0);
            }

            spriteBatch.End();
        }

        private void LightRenderLoop(On_Main.orig_DoDraw orig, Main self, GameTime gameTime)
        {

            orig(self, gameTime);
            if (!DrawSunShadows2())
                return;
            RenderShadows();
        }

        private void RenderShadows()
        {
            if (Main.gameMenu)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            TileDrawing tilesRenderer = Main.instance.TilesRenderer;
            WallDrawing wallsRenderer = Main.instance.WallsRenderer;

            graphicsDevice.SetRenderTarget(_tileShadowMap);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            LightingPreDrawEdit.DontRenderPreDraw = true;
            tilesRenderer.PreDrawTiles(true, true, true);
            tilesRenderer.Draw(true, true, true);

            tilesRenderer.PreDrawTiles(false, true, true);
            tilesRenderer.Draw(false, true, true);
            spriteBatch.End();
            LightingPreDrawEdit.DontRenderPreDraw = false;

            graphicsDevice.SetRenderTarget(_tileBlurRT);  
            graphicsDevice.Clear(Color.Transparent);

            Effect effect = GameShaders.Misc["LunarVeil:SunShadow"].Shader;
            effect.Parameters["mipBias"].SetValue(0.1f);

            Vector2 sunDirection = SunLightManager.ShadowDirection.SafeNormalize(Vector2.Zero);
            effect.Parameters["sunDirection"].SetValue(-sunDirection * 1400);
            effect.Parameters["falloff"].SetValue(0.1f);
            effect.Parameters["uScreenResolution"].SetValue(Main.ScreenSize.ToVector2());
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, effect);

            Vector2 drawPosition = Vector2.Zero - new Vector2(196);
            drawPosition += sunDirection * 16;
            spriteBatch.Draw(_tileShadowMap, drawPosition, null, Color.Black * 0.9f, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
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
        public override void PostUpdateTime()
        {
            base.PostUpdateTime();
            BackLightColor = Color.Black;
            if (Main.LocalPlayer.ZoneUnderworldHeight)
            {
                BackLightColor = Color.White * 0.8f;
            }
     
            foreach (var backLightModifier in _backLightModifiers)
            {
                backLightModifier.ModifyBackLight(ref BackLightColor);
            }
           
            _backLightColor = Color.Lerp(_backLightColor, BackLightColor, 0.1f);
            SunColor = Color.Lerp(SunColor, Main.ColorOfTheSkies, 0.1f);

        }

        public static void AddBackLight(IBackLightModifier backLightModifier)
        {
            _backLightModifiers.Add(backLightModifier);
        }

        public static void RemoveBackLight(IBackLightModifier backLightModifier)
        {
            _backLightModifiers.Remove(backLightModifier);
        }

        public override void PostUpdateEverything()
        {
            ResizeRenderTarget(false);
        }

        public static void ClearAmbientLights()
        {
            _ambientLightIndex = 0;
        }

        public static void AddAmbientLight(TileAmbientLight ambientLight)
        {
            if (_ambientLightIndex < _ambientLights.Length)
            {
                int index = _ambientLightIndex;
                _ambientLights[index] = ambientLight;
                _ambientLightIndex++;
            }
        }

        private static bool ShouldRender()
        {
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.BeamingLights)
                return false;
            if (Main.gameMenu)
                return false;

            return true;
        }

        private static void RenderLightsV2()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            TileDrawing tilesRenderer = Main.instance.TilesRenderer;
            if (!ShouldRender())
                return;

            //First we need to bake the lights so

            int maxBakesPerFrame = 1;
            int bakes = 0;
            for (int i = 0; i < PointLightManager.MAX_POINT_LIGHTS; i++)
            {
                switch (PointLightManager.LightStates[i])
                {
                    case PointLightState.NEEDS_BAKING:
                        PointLightManager.BakeLight(i, _pointLightRT, _tempLightMapAtlasRT);
                        bakes++;
                        break;
                }
                if (bakes >= maxBakesPerFrame)
                    break;
            }

            //Prepare to draw to the accumulate light render target
     
            graphicsDevice.SetRenderTarget(_accumulatedLightRT);
            graphicsDevice.Clear(_backLightColor);

            //Render Sun
            SunLightManager.RenderSunLight();

    
            var effect = PointLightSoftenShader.Instance.Effect;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, null, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);


            for (int i = 0; i < PointLightManager.MAX_POINT_LIGHTS; i++)
            {
                //These lights won't be iterated over/drawn
                switch (PointLightManager.LightStates[i])
                {
                    case PointLightState.CUSTOM:
                    case PointLightState.INACTIVE:
                    case PointLightState.NEEDS_UPDATING:
                    case PointLightState.NEEDS_BAKING:
                        continue;
                }

                ref PointLightData pointLightData = ref PointLightManager.PointLights[i];
                Rectangle atlasRectangle = PointLightManager.LightAtlasRectangles[i];
                Vector2 position = pointLightData.position;
                Vector2 drawOrigin = atlasRectangle.Size() / 2f;
                float scale = PointLightManager.POINT_LIGHT_DOWN_SAMPLES;

                for (int k = 0; k < 1; k++)
                    spriteBatch.Draw(_tempLightMapAtlasRT, position - Main.screenPosition, atlasRectangle, Color.White, 0, drawOrigin, scale, SpriteEffects.None, 0);
            }

            spriteBatch.End();

            //Render the Player PointLight
            int playerLightIndex = PointLightManager.MAX_POINT_LIGHTS - 1;
            PointLightManager.RenderLight(playerLightIndex, _pointLightRT, _accumulatedLightRT);

            _emitters.Clear();
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.ModProjectile is ILightEmitter emitter)
                {
                    _emitters.Add(emitter);
                }
            }

            if (_emitters.Count > 0)
            {
                //Draw additional lights
                foreach (ILightEmitter emitter in _emitters)
                {
                    graphicsDevice.SetRenderTarget(_pointLightRT);
                    graphicsDevice.Clear(Color.Black);
                    emitter.RenderLight(spriteBatch);

                    graphicsDevice.SetRenderTarget(_accumulatedLightRT);
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, null, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    spriteBatch.Draw(_pointLightRT, Vector2.Zero, Color.White);
                    spriteBatch.End();
                }
            }

            ClearAmbientLights();
            //     FindAmbientLights();

            if (_ambientLightIndex > 0)
            {
                Texture2D glowMask = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SoftGlow").Value;
                Vector2 drawOrigin = glowMask.Size() / 2f;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, null, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < _ambientLightIndex; i++)
                {
                    TileAmbientLight ambientLight = _ambientLights[i];
                    spriteBatch.Draw(glowMask, ambientLight.position - Main.screenPosition, null, ambientLight.color, 0, drawOrigin, 2 * ambientLight.radius / 64f, SpriteEffects.None, 0);
                }
                spriteBatch.End();
            }
            graphicsDevice.SetRenderTarget(null);
        }


        private static void InitAtlas()
        {
            _tempLightMapAtlasRT = new RenderTarget2D(Main.graphics.GraphicsDevice, PointLightManager.MAX_ATLAS_SIZE, PointLightManager.MAX_ATLAS_SIZE, false,
                  SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            _initAtlas = true;
        }

        private void ResizeRenderTarget(bool load)
        {
            // If not in the game menu, and we arent a dedicated server,
            if (!Main.gameMenu && !Main.dedServ || load && !Main.dedServ)
            {
                // Get the current screen size.
                Vector2 currentScreenSize = new(Main.screenWidth, Main.screenHeight);
                // If it does not match the previous one, we need to update it.
                if (currentScreenSize != _previousScreenSize)
                {
                    // Render target stuff should be done on the main thread only.
                    Main.QueueMainThreadAction(() =>
                    {
                        if (_accumulatedLightRT != null && !_accumulatedLightRT.IsDisposed)
                            _accumulatedLightRT.Dispose();
                        if (_pointLightRT != null && !_pointLightRT.IsDisposed)
                            _pointLightRT.Dispose();
                        if (_tileShadowMap != null && !_tileShadowMap.IsDisposed)
                            _tileShadowMap.Dispose();
                        if (_tileBlurRT != null && !_tileBlurRT.IsDisposed)
                            _tileBlurRT.Dispose();
                        if (_tileSunShadowRT != null && !_tileSunShadowRT.IsDisposed)
                            _tileSunShadowRT.Dispose();
                        if (!_initAtlas)
                        {
                            InitAtlas();
                        }
                        _tileSunShadowRT = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                        _tileBlurRT = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                        _tileShadowMap = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth + Main.offScreenRange * 2, Main.screenHeight + Main.offScreenRange * 2);
                        _pointLightRT = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight, false,
                            SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                        _accumulatedLightRT = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight, false,
                            SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

                    });

                }
                // Set the current one to the previous one for next frame.
                _previousScreenSize = currentScreenSize;
            }
        }

        public void RenderToScreen()
        {
            DrawToScreen();
        }
    }
}
