using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Threading;
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
    /*
    public class RemoveTileRenderingEdit : ModSystem
    {
        public override void OnModLoad()
        {
            base.OnModLoad();

            On_Main.DoDraw_WallsAndBlacks += NoWallsAndBlacks;
            On_Main.DrawTiles += NoTilesDraw;
            On_Main.RenderTiles += NoTileRender;
            On_Main.RenderTiles2 += NoTileRender2;
            On_Main.DoDraw_Tiles_Solid += NoDrawTiles;
            On_Main.DoDraw_Tiles_NonSolid += NoDrawTiles;   
        }

        private void NoDrawTiles(On_Main.orig_DoDraw_Tiles_NonSolid orig, Main self)
        {
       
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.DoDraw_WallsAndBlacks -= NoWallsAndBlacks;
            On_Main.DrawTiles -= NoTilesDraw;
            On_Main.RenderTiles -= NoTileRender;
            On_Main.RenderTiles2 -= NoTileRender2;
            On_Main.DoDraw_Tiles_Solid -= NoDrawTiles;
            On_Main.DoDraw_Tiles_NonSolid -= NoDrawTiles;
        }
        private void NoWallsAndBlacks(On_Main.orig_DoDraw_WallsAndBlacks orig, Main self)
        {

        }

        private void NoTilesDraw(On_Main.orig_DrawTiles orig, Main self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets, int waterStyleOverride)
        {

        }
        private void NoDrawTiles(On_Main.orig_DoDraw_Tiles_Solid orig, Main self)
        {
      
        }

        private void NoTileRender2(On_Main.orig_RenderTiles2 orig, Main self)
        {

        }

        private void NoTileRender(On_Main.orig_RenderTiles orig, Main self)
        {

        }
    }*/
    public class LunarLighting : ModSystem
    {
        private static Vector2 _previousScreenSize;
        private static Texture2D _lightingTexture;
        private static RenderTarget2D _accumulatedLightRT;
        private static RenderTarget2D _pointLightRT;
        private static RenderTarget2D _downsizedLightRT;
        private static int _pointLightIndex;
        private static Color[] _lightingData;

        private static PointLight[] _pointLights = new PointLight[Max_Point_Lights];
        private static Dictionary<Vector3, VertexPositionColor[]> _shadowData;
        private static List<Vector3> _oldLights;
        private static List<Vector3> _currentLights;
        private static List<ILightEmitter> _emitters;

        public static int Width => Main.screenWidth / DownSamples;
        public static int Height => Main.screenHeight / DownSamples;
        public static int DownSamples => 4;

        public static Vector3 AmbientLight;
        public static Texture2D PointLightTexture;
        public const int Max_Point_Lights = 128;
        private static int IndexOf(int x, int y)
        {
            int index = x + y * Width;
            return index;
        }

        private static Vector3 RayTrace(Vector2 position, Vector2 lightPosition,
            Vector3 lightColor, float lightRadius, float lightIntensity)
        {
            Vector2 lightVector = (lightPosition - position);
            Vector2 normalizedDirection = lightVector.SafeNormalize(Vector2.Zero);
            if (normalizedDirection == Vector2.Zero)
                return Vector3.One;



            float distance = lightVector.Length();
            //Too far, skip the calculation
            if (distance > lightRadius)
                return Vector3.Zero;



            //Calculate how much to move in a single step
            float stepLength = 4;
            Vector2 stepDirection = normalizedDirection * stepLength;

            Vector2 rayPosition = position;
            float maxSteps = distance / stepLength;
            float fallOff = 0f;
            for (int i = 0; i < maxSteps; i++)
            {
                rayPosition += stepDirection;

                int x = (int)rayPosition.X / 16;
                int y = (int)rayPosition.Y / 16;
                if (!WorldGen.InWorld(x, y))
                    return AmbientLight;

                Tile tile = Main.tile[x, y];
                bool hasCollision = Main.tileSolid[tile.TileType] && tile.HasTile;
                bool openToSun = tile.WallType == WallID.None;
                if (hasCollision)
                {
                    fallOff += 0.1f;
                    if (fallOff >= 1f)
                    {
                        break;
                    }
                }
            }

            //Return the light
            //Calculating how much attenuation to give it
            float du = distance / (1 - distance / (lightRadius * lightRadius - 1));
            float denom = du / lightRadius + 1;

            //The attenuation is the falloff of the light depending on distance basically
            float attenuation = 1 / (denom * denom);
            Vector3 pixelRGB = AmbientLight * (lightColor * lightIntensity * attenuation * (1.0f - fallOff));
            return pixelRGB;
        }

        private static Vector3 RayTrace(Vector2 position, PointLight pointLight)
        {
            Vector3 lightColor = pointLight.color;
            float lightRadius = pointLight.radius;
            float lightIntensity = pointLight.intensity;
            return RayTrace(position, pointLight.position, lightColor, lightRadius, lightIntensity);
        }

        private static void CalculateLightingData()
        {
            Method1_CastARayFromEveryPixel();
        }


        private static void Method1_CastARayFromEveryPixel()
        {
  
            Vector2 cameraCenterWorld = Main.Camera.Center;
            Vector2 cameraTopLeft = cameraCenterWorld - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            FastParallel.For(0, Width, delegate (int start, int end, object context)
            {
                for (int x = start; x < end; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        int index = IndexOf(x, y);
                        _lightingData[index] = Color.Black;
                        for (int i = 0; i < _pointLightIndex; i++)
                        {
                            PointLight pointLight = _pointLights[i];
                            Vector2 position = new Vector2(x, y) * DownSamples + cameraTopLeft;
                            _lightingData[index] = RayTrace(position, pointLight).ToColor();
                        }
                    }
                }
            });
        }

        public override void Load()
        {
            _oldLights = new List<Vector3>();
            _shadowData = new Dictionary<Vector3, VertexPositionColor[]>();
            _currentLights = new List<Vector3>();
            _emitters = new List<ILightEmitter>();
            ResizeRenderTarget(true);

            On_OverlayManager.Draw += DrawLights;
            On_Main.DoDraw += LightRenderLoop;
        }

        private void DrawLights(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
        {
            if(layer == RenderLayers.All && beginSpriteBatch){
                DrawToScreen();
            }
            orig(self, spriteBatch, layer, beginSpriteBatch);
        }

        public override void Unload()
        {
            base.Unload();
            On_Main.DoDraw -= LightRenderLoop;
            On_OverlayManager.Draw -= DrawLights;
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

            //Add a point light to all torches
            float pointLightPixelRadius = 800;

            Vector2 cameraCenterWorld = Main.Camera.Center;
            Vector2 cameraTopLeft = cameraCenterWorld - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            Vector2 cameraBottomRight = cameraCenterWorld + new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            Point topLeftTile = cameraTopLeft.ToTileCoordinates();
            Point bottomRightTile = cameraBottomRight.ToTileCoordinates();
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
                            var keys = TileShading.PrepareTilesForShading(pointLight);
                            _shadowData.Add(shadowKey, keys);
                        }
                        _currentLights.Add(shadowKey);
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
            graphicsDevice.Clear(Color.Black);


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
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, null, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (ILightEmitter emitter in _emitters)
                {
                    emitter.RenderLight(spriteBatch);
                }
                spriteBatch.End();
            }
            graphicsDevice.SetRenderTarget(null);
        }

        public static float GetPlayerLightIntensity()
        {
            Player player = Main.LocalPlayer;
            Item heldItem = player.HeldItem;
            if (ItemID.Sets.Torches[heldItem.type])
            {
                return 1;
            }
            else
            {
                return 0.5f;
            }
        }

        public static Vector3 GetPlayerLightColor()
        {
            Player player = Main.LocalPlayer;
            Item heldItem = player.HeldItem;
            if (ItemID.Sets.Torches[heldItem.type])
            {
                TorchID.TorchColor(heldItem.type, out float R, out float G, out float B);
                Vector3 torchColor = new Vector3();
                torchColor.X = R;
                torchColor.Y = G;
                torchColor.Z = B;
                return torchColor;
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
            if (ItemID.Sets.Torches[heldItem.type])
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
                return;


            Vector2 sunLeft = Main.Camera.Center + new Vector2(-Main.screenWidth / 2, -Main.screenHeight / 2);
            Vector2 sunRight = Main.Camera.Center + new Vector2(Main.screenWidth / 2, -Main.screenHeight / 2);

   
            float dayProgress = Main.dayTime ? (float)Main.time / (float)Main.dayLength : (float)Main.time / (float)Main.nightLength;
            float radians = MathHelper.Lerp(MathHelper.ToRadians(-45), MathHelper.ToRadians(45), dayProgress);
            Vector2 sunDirection = Vector2.UnitY.RotatedBy(radians) * 500;
            Vector2 sunPosition = Main.Camera.Center + new Vector2(0, -Main.screenHeight / 2);
            PointLight sunLight = new PointLight
            {
                position = sunPosition,
                color = Main.ColorOfTheSkies.ToVector3(),
                radius = 4000,
                intensity = 1,
                extraRenders=4,
                calculateVertices= ModContent.GetInstance<LunarVeilClientConfig>().SunShadows,
                faint=true,
                directionOverride = sunDirection
            };
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
            if(pointLight.extraRenders > 0)
            {
                for(int i = 0; i < pointLight.extraRenders; i++)
                {
                    spriteBatch.Draw(LunarLighting.PointLightTexture, Vector2.Zero, color * ExtraMath.Osc(0.9f, 1f, speed: 2));
                }
            }
            spriteBatch.End();

            //This is bad, just calculat them once :sob:

            Vector3 shadowKey = PointLightToKey(pointLight);
            if (_shadowData.ContainsKey(shadowKey))
            {
                TileShading.DrawVertices(_shadowData[shadowKey]);
            } else if (pointLight.calculateVertices)
            {
                var keys = TileShading.PrepareTilesForShading(pointLight);
                TileShading.DrawVertices(keys);
            }



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
                        if (_lightingTexture != null && !_lightingTexture.IsDisposed)
                            _lightingTexture.Dispose();
                        if (_downsizedLightRT != null && !_downsizedLightRT.IsDisposed)
                            _downsizedLightRT.Dispose();
                        PointLightTexture ??= new Texture2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                        _accumulatedLightRT = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                        _pointLightRT = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                        _downsizedLightRT = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth / DownSamples, Main.screenHeight / DownSamples);
                        _lightingData = new Color[Width * Height];
                        _lightingTexture = new Texture2D(Main.graphics.GraphicsDevice, Width, Height);
                    });

                }
                // Set the current one to the previous one for next frame.
                _previousScreenSize = currentScreenSize;
            }
        }
    }
}
