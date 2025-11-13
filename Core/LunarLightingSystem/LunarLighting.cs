using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{
    [Autoload(Side = ModSide.Client)]
    public class LunarLighting : ModSystem
    {
        private static Color _backLightColor;
        private static Vector2 _previousScreenSize;
        private static RenderTarget2D _accumulatedLightRT;

        private static bool _initAtlas;
        private static RenderTarget2D _pointLightRT;
        private static RenderTarget2D _tempLightMapAtlasRT;

        private static List<ILightEmitter> _emitters;
        private static List<IBackLightModifier> _backLightModifiers;

        private static int _ambientLightIndex;
        private static TileAmbientLight[] _ambientLights = new TileAmbientLight[Max_Ambient_Lights];

        public static Color BackLightColor;
        public static Color SunColor;
        public static Vector3 AmbientLight;

        public static int MaxAtlasSize => 2000;
        public const int Max_Ambient_Lights = 2000;
        public override void Load()
        {
            _backLightModifiers = new List<IBackLightModifier>();
            _emitters = new List<ILightEmitter>();
            ResizeRenderTarget(true);

            On_OverlayManager.Draw += DrawLights;
            On_Main.DoDraw += LightRenderLoop;
        }


        public override void Unload()
        {
            base.Unload();
            On_Main.DoDraw -= LightRenderLoop;
            On_OverlayManager.Draw -= DrawLights;
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

        private void DrawLights(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
        {
            if (layer == RenderLayers.All && beginSpriteBatch)
            {
                DrawToScreen();
            }
            orig(self, spriteBatch, layer, beginSpriteBatch);
        }

        private static void DrawToScreen()
        {
            if (!ShouldRender())
                return;


            //PreviewLightMaps();
            DrawAccumulatedLightMapToScreen();
            DrawSoftGlows();
          //  DrawAtlasToScreen();
        }

        private static void DrawAtlasToScreen()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(_tempLightMapAtlasRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.End();
        }

        private static void DrawAccumulatedLightMapToScreen()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, CustomBlendStates.Multiply, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(_accumulatedLightRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.End();
        }

        private static void DrawSoftGlows()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

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
            RenderLightsV2();
            orig(self, gameTime);
        }


        public override void PostUpdateTime()
        {
            base.PostUpdateTime();
            BackLightColor = Color.Black;
            if (Main.LocalPlayer.ZoneUnderworldHeight)
            {
                BackLightColor = Color.White * 0.5f;
            }

            foreach (var backLightModifier in _backLightModifiers)
            {
                backLightModifier.ModifyBackLight(ref BackLightColor);
            }


            _backLightColor = Color.Lerp(_backLightColor, BackLightColor, 0.01f);
            SunColor = Color.Lerp(SunColor, Main.ColorOfTheSkies, 0.01f);

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
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_accumulatedLightRT);
            graphicsDevice.Clear(BackLightColor);

            //Render Sun
            SunLightManager.RenderSunLight();

            SpriteBatch spriteBatch = Main.spriteBatch;
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

                for(int k = 0; k < 1; k++)
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

        private static void RenderLights()
        {
            /*
            if (!ShouldRender())
                return;

            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;


            // CalculateLightingData();
            CalculatePointLightSources();

            BakePointLights();
            //Mask drawing
            //Clear the final light render target
            graphicsDevice.SetRenderTarget(_accumulatedLightRT);
            graphicsDevice.Clear(BackLightColor);
            RenderPointLights();

            _playerPointLight.position = Main.LocalPlayer.Center;
            _playerPointLight.color = new Color(GetPlayerLightColor());
            _playerPointLight.intensity = 1;
            _playerPointLight.radius = GetPlayerLightRadius();
            _playerPointLight.name = "Player Light";
            _playerPointLight.Update();
            RenderPointLightRealtime(_playerPointLight);
            RenderSun();

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
            */
        }


        private static void InitAtlas()
        {
            _tempLightMapAtlasRT = new RenderTarget2D(Main.graphics.GraphicsDevice, MaxAtlasSize, MaxAtlasSize, false,
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

                        if (!_initAtlas)
                        {
                            InitAtlas();
                        }
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
    }
}
