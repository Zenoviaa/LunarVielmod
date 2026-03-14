using Stellamod.Common.Shaders;
using Stellamod.Helpers;
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

        private static bool _isLoaded;
        private static bool _initAtlas;
        private static RenderTarget2D _pointLightRT;
        private static RenderTarget2D _tempLightMapAtlasRT;
        private static RenderTarget2D _tileShadowMap;
        private static RenderTarget2D _tileBlurRT;
        private static RenderTarget2D _tileSunShadowRT;
        private static RenderTarget2D _pixelLightRT;

        private static List<ILightEmitter> _emitters;
        private static List<IBackLightModifier> _backLightModifiers;

        public static Color BackLightColor;
        public static Color SunColor;
        public static Vector3 AmbientLight;
        public static RenderTarget2D GetAccumulatedLightMap()
        {
            return _accumulatedLightRT;
        }

        public override void Load()
        {

            _backLightModifiers = new List<IBackLightModifier>();
            _emitters = new List<ILightEmitter>();

            On_Main.CheckMonoliths += RenderToLightMaps;
            On_Main.DoDraw += LightRenderLoop;
            On_Main.DrawCachedNPCs += DrawShadowsBehindTiles;
        }
        public override void Unload()
        {
            base.Unload();
            Main.QueueMainThreadAction(UnloadRenderTargets);
            On_Main.CheckMonoliths -= RenderToLightMaps;
            On_Main.DoDraw -= LightRenderLoop;
            On_Main.DrawCachedNPCs -= DrawShadowsBehindTiles;

        }

        private static bool IsActive
        {
            get
            {
                DomainExpansionManager domainExpansionManager = ModContent.GetInstance<DomainExpansionManager>();
                return !domainExpansionManager.inSpace;
            }
        }
        private void RenderToLightMaps(On_Main.orig_CheckMonoliths orig)
        {

            if (IsActive && _isLoaded)
            {
                RenderLightsV2();
                if (DrawSunShadows2())
                {
                    RenderShadows();
                }
            }



            orig();
        }

        private void DrawShadowsBehindTiles(On_Main.orig_DrawCachedNPCs orig, Main self, List<int> npcCache, bool behindTiles)
        {
            if (behindTiles && DrawSunShadows2() && IsActive && _isLoaded)
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
        }

        private static void DrawToScreen()
        {
            if (!ShouldRender())
                return;
            if (!_isLoaded)
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
            Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight);
            SimpleBlurShader simpleBlurShader = SimpleBlurShader.Instance;
            simpleBlurShader.TexelSize = texelSize;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, CustomBlendStates.Multiply, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, simpleBlurShader.Effect);
            spriteBatch.Draw(_accumulatedLightRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
        }


        private static bool DrawSunShadows2()
        {
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            return config.SunShadows2;
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
            /*
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

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
                drawColor *= 0.13f;
                drawColor.A = 0;
                spriteBatch.Draw(glowTexture, drawPosition, null, drawColor, 0, drawOrigin, 2, SpriteEffects.None, 0);
            }

            spriteBatch.End();*/
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

        private static bool ShouldRender()
        {
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.BeamingLights)
                return false;
            if (Main.gameMenu)
                return false;
            if (!IsActive)
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
            graphicsDevice.SetRenderTarget(null);

            graphicsDevice.SetRenderTarget(_pixelLightRT);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, Main.Rasterizer, null);
            spriteBatch.Draw(_accumulatedLightRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);

        }


        private static void InitAtlas()
        {
            _tempLightMapAtlasRT = new RenderTarget2D(Main.graphics.GraphicsDevice, PointLightManager.MAX_ATLAS_SIZE, PointLightManager.MAX_ATLAS_SIZE, false,
                  SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            _initAtlas = true;
        }

        private void UnloadRenderTargets()
        {
            _accumulatedLightRT?.Dispose();
            _tileShadowMap?.Dispose();
            _pointLightRT?.Dispose();
            _tileBlurRT?.Dispose();
            _tileSunShadowRT?.Dispose();
            _pixelLightRT?.Dispose();
            _tempLightMapAtlasRT?.Dispose();

            _tempLightMapAtlasRT = null;
            _pixelLightRT = null;
            _accumulatedLightRT = null;
            _tileShadowMap = null;
            _pointLightRT = null;
            _tileBlurRT = null;
            _tileSunShadowRT = null;
            _isLoaded = false;
        }

        private void ResizeRenderTargets()
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
            if (_pixelLightRT != null && !_pixelLightRT.IsDisposed)
                _pixelLightRT.Dispose();
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
            _pixelLightRT = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)(Main.screenWidth * 0.5f), (int)(Main.screenHeight * 0.5f), false,
                SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            _isLoaded = true;
        }

        private void ResizeRenderTarget(bool load)
        {
            if (Main.gameMenu)
                return;
            if (Main.netMode == NetmodeID.Server)
                return;
            Vector2 currentScreenSize = new(Main.screenWidth, Main.screenHeight);
            if (currentScreenSize == _previousScreenSize)
                return;
            Main.QueueMainThreadAction(ResizeRenderTargets);
            _previousScreenSize = currentScreenSize;
        }

        public void RenderToScreen()
        {
            DrawToScreen();
        }
    }
}
