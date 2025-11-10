using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteelSeries.GameSense;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;

using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{

    [Autoload(Side = ModSide.Client)]
    public class LunarLighting : ModSystem
    {
        private static Color _backLightColor;
        private static float _overSunTimer;
        private static Vector2 _previousScreenSize;
        private static RenderTarget2D _accumulatedLightRT;
        private static RenderTarget2D _pointLightRT;

        private static int _pointLightIndex;


        private static PointLight[] _pointLights = new PointLight[Max_Point_Lights];
        private static Dictionary<Vector3, TileShadow> _shadowData;
        private static List<Vector3> _oldLights;
        private static List<Vector3> _currentLights;
        private static List<ILightEmitter> _emitters;
        private static List<IBackLightModifier> _backLightModifiers;

        private static int _ambientLightIndex;
        private static TileAmbientLight[] _ambientLights = new TileAmbientLight[Max_Ambient_Lights];

        public static Color BackLightColor;
        public static Color SunColor;
        public static Vector3 AmbientLight;
        public static Texture2D PointLightTexture;
        public const int Max_Point_Lights = 128;
        public const int Max_Ambient_Lights = 2000;
        public override void Load()
        {
            _backLightModifiers = new List<IBackLightModifier>();
            _oldLights = new List<Vector3>();
            _shadowData = new Dictionary<Vector3, TileShadow>();
            _currentLights = new List<Vector3>();
            _emitters = new List<ILightEmitter>();
            ResizeRenderTarget(true);

            On_OverlayManager.Draw += DrawLights;
            On_Main.DoDraw += LightRenderLoop;
            On_Lighting.AddLight_int_int_float_float_float += NoAddLight;
            On_Lighting.AddLight_int_int_int_float += NoAddLight;
            On_Lighting.AddLight_Vector2_int += NoAddLight;
            On_Lighting.AddLight_Vector2_Vector3 += NoAddLight;
            On_Lighting.AddLight_Vector2_float_float_float += NoAddLight;
        }


        public override void Unload()
        {
            base.Unload();
            On_Main.DoDraw -= LightRenderLoop;
            On_OverlayManager.Draw -= DrawLights;
            On_Lighting.AddLight_int_int_float_float_float -= NoAddLight;
            On_Lighting.AddLight_int_int_int_float -= NoAddLight;
            On_Lighting.AddLight_Vector2_int -= NoAddLight;
            On_Lighting.AddLight_Vector2_Vector3 -= NoAddLight;
            On_Lighting.AddLight_Vector2_float_float_float -= NoAddLight;
        }
        private void NoAddLight(On_Lighting.orig_AddLight_Vector2_float_float_float orig, Vector2 position, float r, float g, float b)
        {
            /*
            TileAmbientLight tileAmbientLight = new TileAmbientLight();
            tileAmbientLight.position = position;
            tileAmbientLight.color = new Color(r, g, b);
            AddAmbientLight(tileAmbientLight);*/
        }

        private void NoAddLight(On_Lighting.orig_AddLight_int_int_float_float_float orig, int i, int j, float r, float g, float b)
        {
            /*
            TileAmbientLight tileAmbientLight = new TileAmbientLight();
            tileAmbientLight.position = new Vector2(i * 16, j * 16);
            tileAmbientLight.color = new Color(r, g, b);
            AddAmbientLight(tileAmbientLight);*/
        }
        private void NoAddLight(On_Lighting.orig_AddLight_Vector2_Vector3 orig, Vector2 position, Vector3 rgb)
        {
            /*
            TileAmbientLight tileAmbientLight = new TileAmbientLight();
            tileAmbientLight.position = position;
            tileAmbientLight.color = new Color(rgb);
            AddAmbientLight(tileAmbientLight);*/
        }

        private void NoAddLight(On_Lighting.orig_AddLight_Vector2_int orig, Vector2 position, int torchID)
        {

        }

        private void NoAddLight(On_Lighting.orig_AddLight_int_int_int_float orig, int i, int j, int torchID, float lightAmount)
        {

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

            //New d raw to screen
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;


            spriteBatch.Begin(SpriteSortMode.Immediate, CustomBlendStates.Multiply, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(_accumulatedLightRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);

            spriteBatch.End();


            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D glowTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SoftGlow").Value;
            Vector2 drawOrigin = glowTexture.Size() / 2f;
            for (int i = 0; i < _pointLightIndex; i++)
            {
                PointLight pointLight = _pointLights[i];
                Vector2 drawPosition = pointLight.position - Main.screenPosition;

                Point tilePosition = (pointLight.position - new Vector2(8, 8)).ToTileCoordinates();
                Tile tile = Main.tile[tilePosition.X, tilePosition.Y];

                Color drawColor = Lighting.GetColor(tilePosition.X, tilePosition.Y);
                drawColor *= ExtraMath.Osc(0.9f, 1f, speed: 2, offset: i);
                drawColor *= 0.35f;
                spriteBatch.Draw(glowTexture, drawPosition, null, drawColor, 0, drawOrigin, 2, SpriteEffects.None, 0);
            }

            spriteBatch.End();
        }
        private void LightRenderLoop(On_Main.orig_DoDraw orig, Main self, GameTime gameTime)
        {
            RenderLights();
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

        public static Vector3 PointLightToKey(PointLight pointLight)
        {
            Vector3 vector3 = new Vector3(pointLight.position.X, pointLight.position.Y, pointLight.radius);
            return vector3;
        }

        public static void ClearLights()
        {
            _pointLightIndex = 0;
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

        public static PointLight QueueLight(Vector2 position, Vector3 color, float intensity, float radius)
        {
            if (_pointLightIndex < _pointLights.Length)
            {
                int index = _pointLightIndex;
                _pointLights[index].position = position;
                _pointLights[index].color = color;
                _pointLights[index].radius = radius;
                _pointLights[index].intensity = intensity;
                _pointLightIndex++;
                return _pointLights[index];
            }
            return _pointLights[_pointLights.Length - 1];
        }

        private static void CalculatePointLightSources()
        {
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.BeamingLights)
                return;
            //We don't need to check for lights on screen every single frame, just often enough
            //Small optimization tbh
            if (Main.GameUpdateCount % 3 != 0)
                return;


            _oldLights.Clear();
            foreach (Vector3 key in _currentLights)
            {
                _oldLights.Add(key);
            }

            _currentLights.Clear();
            ClearLights();
            ClearAmbientLights();
            //Add a point light to all torches
            float pointLightPixelRadius = 800;

            Vector2 cameraCenterWorld = Main.Camera.Center;
            Vector2 cameraTopLeft = cameraCenterWorld - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            Vector2 cameraBottomRight = cameraCenterWorld + new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            cameraTopLeft -= new Vector2(128);
            cameraBottomRight += new Vector2(128);

            Point topLeftTile = cameraTopLeft.ToTileCoordinates();
            Point bottomRightTile = cameraBottomRight.ToTileCoordinates();
            bool shouldCalculateShadows = false;


            for (int x = topLeftTile.X; x < bottomRightTile.X; x++)
            {
                for (int y = topLeftTile.Y; y < bottomRightTile.Y; y++)
                {
                    if (!WorldGen.InWorld(x, y))
                        continue;
                    Tile tile = Main.tile[x, y];

                    if (TileID.Sets.Torch[tile.TileType])
                    {

                        Vector2 position = new Point(x, y).ToWorldCoordinates();
                        Vector3 lightColor = Lighting.GetColor(x, y).ToVector3();
                        PointLight pointLight = QueueLight(position, lightColor, 1, pointLightPixelRadius);
                        Vector3 shadowKey = PointLightToKey(pointLight);

                        if (!_shadowData.ContainsKey(shadowKey))
                        {
                            shouldCalculateShadows = true;
                        }
                        else
                        {
                            _pointLights[_pointLightIndex - 1].tileShadow = _shadowData[shadowKey];
                        }

                        _currentLights.Add(shadowKey);
                    }

                    if (tile.LiquidType == LiquidID.Lava)
                    {
                        Vector2 position = new Point(x, y).ToWorldCoordinates();
                        TileAmbientLight tileAmbientLight = new TileAmbientLight();
                        tileAmbientLight.position = position;
                        tileAmbientLight.radius = 64;
                        tileAmbientLight.color = Color.Red;
                        AddAmbientLight(tileAmbientLight);
                    }
                    if (tile.LiquidType == LiquidID.Shimmer)
                    {
                        Vector2 position = new Point(x, y).ToWorldCoordinates();
                        TileAmbientLight tileAmbientLight = new TileAmbientLight();
                        tileAmbientLight.position = position;
                        tileAmbientLight.radius = 64;
                        tileAmbientLight.color = Color.White * 0.7f;
                        AddAmbientLight(tileAmbientLight);
                    }
                    if (LightingSets.EmissiveTiles[tile.TileType].A > 0)
                    {
                        Color lightingColor = LightingSets.EmissiveTiles[tile.TileType];
                        Vector2 position = new Point(x, y).ToWorldCoordinates();
                        TileAmbientLight tileAmbientLight = new TileAmbientLight();
                        tileAmbientLight.position = position;
                        tileAmbientLight.radius = 384;
                        tileAmbientLight.color = lightingColor * 0.7f;
                        AddAmbientLight(tileAmbientLight);
                    }

                  
                }
            }

            if (shouldCalculateShadows)
            {
                TileShadowVertexBuffer.Clear();
                for (int i = 0; i < _pointLightIndex; i++)
                {
                    PointLight pointLight = _pointLights[i];
                    TileShadow tileShadow = TileShading.PrepareTilesForShading(pointLight, useSunBuffer: false);
                    Vector3 shadowKey = PointLightToKey(pointLight);
                    if (_shadowData.ContainsKey(shadowKey))
                    {
                        _shadowData[shadowKey] = tileShadow;
                        _pointLights[i].tileShadow = tileShadow;
                    }
                    else
                    {
                        _shadowData.Add(shadowKey, tileShadow);
                    }
                }
            }



            foreach (Vector3 key in _oldLights)
            {
                if (!_currentLights.Contains(key))
                {
                    _shadowData.Remove(key);
                }
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
        private static void RenderLights()
        {
            if (!ShouldRender())
                return;

            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;
            // CalculateLightingData();
            CalculatePointLightSources();


            //Mask drawing
            //Clear the final light render target
            graphicsDevice.SetRenderTarget(_accumulatedLightRT);
            graphicsDevice.Clear(BackLightColor);


            for (int i = 0; i < _pointLightIndex; i++)
            {


                //We're drawing everything for how this light perceives the world onto this render target
                //Then we're going to move it to another one so everything blends properly!

                PointLight pointLight = _pointLights[i];
                RenderPointLight(pointLight);
            }

            //REnder player point light
            PointLight playerLight = new PointLight
            {
                position = Main.LocalPlayer.Center,
                color = GetPlayerLightColor(),
                intensity = 1,
                radius = GetPlayerLightRadius()
            };

            TileSunShadowVertexBuffer.Clear();
            playerLight.tileShadow = TileShading.PrepareTilesForShading(playerLight, useSunBuffer: true);
            RenderPointLight(playerLight);
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
        }

        public static Vector3 GetPlayerLightColor()
        {
            Player player = Main.LocalPlayer;
            Item heldItem = player.HeldItem;
            if (LightingSets.EmissiveHeldItems[heldItem.type].A > 0)
            {

                int c = TorchLightingHelper.TorchItemToTorchID(heldItem.type);
                if(c != -1)
                {
                    TorchID.TorchColor(c, out float r, out float g, out float b);
                    Color myColor = new Color(r, g, b);
                    return myColor.ToVector3();
                }
         

                Vector3 color = LightingSets.EmissiveHeldItems[heldItem.type].ToVector3();
                return color;
   
            }
            else
            {
                return Vector3.One;
            }
        }

        public static float GetPlayerLightRadius()
        {
            Player player = Main.LocalPlayer;
            Item heldItem = player.HeldItem;
            if (LightingSets.EmissiveHeldItems[heldItem.type].A > 0)
            {
                return 800f;
            }
            else
            {
                return 400f;
            }
        }

        private static void RenderSun()
        {
            if (!Main.LocalPlayer.ZoneOverworldHeight)
            {
                _overSunTimer--;
                if (_overSunTimer <= 0)
                    return;
            }
            else
            {
                _overSunTimer++;
            }


            _overSunTimer = MathHelper.Clamp(_overSunTimer, 0, 120);
            float interpolant = _overSunTimer / 120f;


            Vector2 sunLeft = Main.Camera.Center + new Vector2(-Main.screenWidth / 2, -Main.screenHeight / 2);
            Vector2 sunRight = Main.Camera.Center + new Vector2(Main.screenWidth / 2, -Main.screenHeight / 2);


            float dayProgress = Main.dayTime ? (float)Main.time / (float)Main.dayLength : (float)Main.time / (float)Main.nightLength;
            float radians = MathHelper.Lerp(MathHelper.ToRadians(-45), MathHelper.ToRadians(45), dayProgress);
            Vector2 sunDirection = Vector2.UnitY.RotatedBy(radians) * 500;
            if (dayProgress <= 0.1f || dayProgress >= 0.9f)
            {
                sunDirection *= Vector2.Zero;
            }


            Vector2 sunPosition = Main.Camera.Center + new Vector2(0, 0);
            PointLight sunLight = new PointLight
            {
                position = sunPosition,
                color = SunColor.ToVector3(),
                radius = 3000,
                intensity = 1 * interpolant,
                extraRenders = 4,
                faint = true,
                directionOverride = sunDirection
            };


            if (ModContent.GetInstance<LunarVeilClientConfig>().SunShadows)
                sunLight.tileShadow = TileShading.PrepareTilesForShading(sunLight, useSunBuffer: true);
            RenderPointLight(sunLight);
        }

        private static void RenderPointLight(PointLight pointLight)
        {
            var pointlightShader = PointLightShader.Instance;
            pointlightShader.ScreenResolution = new Vector2(Main.screenWidth, Main.screenHeight);

            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;
            graphicsDevice.SetRenderTarget(_pointLightRT);
            graphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, null, Main.Rasterizer, pointlightShader.Effect, Main.GameViewMatrix.TransformationMatrix);

            var shader = PointLightShader.Instance;
            Vector4 colorAndIntensity = new Vector4(pointLight.color, pointLight.intensity);
            Color color = new Color(colorAndIntensity);
            shader.LightRadius = pointLight.radius * 2 / Main.screenWidth;

            //Convert to screen space
            Vector2 lightingWorldPosition = pointLight.position;
            Vector2 lightingScreenPosition = lightingWorldPosition - Main.screenPosition;
            lightingScreenPosition.X /= Main.screenWidth;
            lightingScreenPosition.Y /= Main.screenHeight;
            shader.LightPosition = lightingScreenPosition;
            spriteBatch.Draw(LunarLighting.PointLightTexture, Vector2.Zero, color * ExtraMath.Osc(0.9f, 1f, speed: 2));
            if (pointLight.extraRenders > 0)
            {
                for (int i = 0; i < pointLight.extraRenders; i++)
                {
                    spriteBatch.Draw(LunarLighting.PointLightTexture, Vector2.Zero, color * ExtraMath.Osc(0.9f, 1f, speed: 2));
                }
            }
            spriteBatch.End();

            //This is bad, just calculat them once :sob:
            TileShading.DrawVertices(pointLight.tileShadow);

            //Add it to the final light RT
            graphicsDevice.SetRenderTarget(_accumulatedLightRT);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, null, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(_pointLightRT, Vector2.Zero, Color.White);
            spriteBatch.End();
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


                        PointLightTexture ??= new Texture2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                        _accumulatedLightRT = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                        _pointLightRT = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    });

                }
                // Set the current one to the previous one for next frame.
                _previousScreenSize = currentScreenSize;
            }
        }
    }
}
