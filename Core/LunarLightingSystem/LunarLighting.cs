

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Threading;
using Stellamod.Core.Shaders;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{
    public class LunarLighting : ModSystem
    {
        private static Vector2 _previousScreenSize;
        private static Texture2D _lightingTexture;
        private static RenderTarget2D _accumulatedLightRT;
        private static int _pointLightIndex;
        private static Color[] _lightingData;

        private static bool _initLightCasts;
        private static Color[][] _lightCastData = new Color[Max_Cast_Lights][];
        private static Texture2D[] _lightCastTextures = new Texture2D[Max_Cast_Lights];
        private static PointLight[] _pointLights = new PointLight[Max_Cast_Lights];
        private static Dictionary<Vector3, ILightSource> _lightSources;

        //We'll use this for managing indices that can be used for calculating lighting data
        private static Queue<int> _freeLights;
        private static Queue<PointLight> _lightSourceCreationQueue;
        public static int Width => Main.screenWidth / DownSamples;
        public static int Height => Main.screenHeight / DownSamples;
        public static int DownSamples => 10;

        public static Vector3 AmbientLight;

        public const int Max_Cast_Lights = 32;
        public const int Max_Light_Cast_Texture_Size = 400;

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
            InitializeLightingTextures();
            ResizeRenderTarget(true);
            On_Main.CheckMonoliths += RenderLights;
            On_Main.DrawInfernoRings += FinalDraw;
        }

        public override void Unload()
        {
            base.Unload();
            On_Main.CheckMonoliths -= RenderLights;
            On_Main.DrawInfernoRings -= FinalDraw;
        }

        public override void PostUpdateEverything()
        {
            ResizeRenderTarget(false);
        }

        private static void InitializeLightingTextures()
        {
            _freeLights = new();
            _lightSources = new();

            for(int i = 0; i < Max_Cast_Lights; i++)
            {
                _freeLights.Enqueue(i);
            }

            AmbientLight = Color.White.ToVector3();
        }

        public static ref Color[] GetLightingData(int index)
        {
            return ref _lightCastData[index];
        }

        public static ref Texture2D GetLightCastTexture(int index)
        {
            return ref _lightCastTextures[index];
        }

        public static int UseLightingIndex()
        {
            if (_freeLights.Count > 0)
                return _freeLights.Dequeue();
            return -1;
        }

        public static void ReleaseLightingIndex(int index)
        {
            if (_freeLights.Contains(index))
                return;
            _freeLights.Enqueue(index); 
        }


        private void FinalDraw(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            orig(self);
            DrawToScreen();
            if(Main.mouseRight && Main.mouseRightRelease)
            {
                _lightSources.Clear();
            }
        }

        private void RenderLights(On_Main.orig_CheckMonoliths orig)
        {
            RenderLights();
            orig();
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

        public static void QueueLight(Vector2 position, Vector3 color, float intensity, float radius)
        {
            if(_pointLightIndex < _pointLights.Length)
            {
                _pointLights[_pointLightIndex].position = position;
                _pointLights[_pointLightIndex].color = color;
                _pointLights[_pointLightIndex].radius = radius;
                _pointLights[_pointLightIndex].intensity = intensity;
                _pointLightIndex++;
            }
        }

        public static void CreateLightSource(PointLight pointLight)
        {
            Vector3 key = PointLightToKey(pointLight);
            if (_lightSources.ContainsKey(key))
                return;

            ILightSource lightSource = new TorchLightSource();
            lightSource.ReCalculateLights(pointLight);
            _lightSources.Add(key, lightSource);
        }

        public static void RemoveLightSource(PointLight pointLight)
        {
            Vector3 key = PointLightToKey(pointLight);
            if (!_lightSources.ContainsKey(key))
                return;

            ILightSource lightSource = _lightSources[key];
            lightSource.ReleaseLights();
            _lightSources.Remove(key);
        }

        public static void RemoveLightSource(Vector3 key)
        {
            ILightSource lightSource = _lightSources[key];
            lightSource.ReleaseLights();
            _lightSources.Remove(key);
        }

        private static void CalculatePointLightSources()
        {

            //Find all torches
            //We're gonna add lights to them!
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
                        QueueLight(position, lightColor, 1, Max_Light_Cast_Texture_Size);
                    }
                }
            }

            //Get our light sources
            for (int i = 0; i < _pointLightIndex; i++)
            {
                PointLight pointLight = _pointLights[i];
                CreateLightSource(pointLight);
            }

            ClearLights();
              
            Rectangle cameraBounds = new Rectangle((int)cameraTopLeft.X, (int)cameraTopLeft.Y,(int)(cameraBottomRight.X - cameraTopLeft.X), (int)(cameraBottomRight.Y - cameraTopLeft.Y));
            List<Vector3> lightsToRemove = new List<Vector3>();
            foreach (var lightKvp in _lightSources)
            {
                if (!cameraBounds.Contains((int)lightKvp.Key.X, (int)lightKvp.Key.Y))
                {
                    //Not on screen
                    //So we should get rid of the light
                    lightsToRemove.Add(lightKvp.Key);
                }
            }

            foreach(Vector3 ltr in lightsToRemove)
            {
                RemoveLightSource(ltr);
            }
        }

        private static void RenderLights()
        {
            if (Main.gameMenu)
                return;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;
            // CalculateLightingData();
            CalculatePointLightSources();

            graphicsDevice.SetRenderTarget(_accumulatedLightRT);
            graphicsDevice.Clear(Color.Black);


            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            for (int i = 0; i < 1; i++)
            {
                foreach (var lightKvp in _lightSources)
                {
                    lightKvp.Value.DrawLights(spriteBatch);
                }
            }

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
                        if (_lightingTexture != null && !_lightingTexture.IsDisposed)
                            _lightingTexture.Dispose();

                        if (!_initLightCasts)
                        {                        //Pre initialize our cast light texture array data
                                                 //64 800x800 textures should be fine I think?
                                                 //This will take a little time to initialize but not too bad
                                                 //We'd reference with an index instead of instantiating in our torch lights
                            for (int i = 0; i < Max_Cast_Lights; i++)
                            {
                                int pxCount = Max_Light_Cast_Texture_Size * Max_Light_Cast_Texture_Size;
                                _lightCastData[i] = new Color[pxCount];
                                _lightCastTextures[i] = new Texture2D(Main.graphics.GraphicsDevice,
                                    Max_Light_Cast_Texture_Size, Max_Light_Cast_Texture_Size);
                            }
                            _initLightCasts = true;
                        }

                        //Downscale for optimization and a more pixelated look
                        _accumulatedLightRT = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                        _lightingData = new Color[Width * Height];
                        _lightingTexture = new Texture2D(Main.graphics.GraphicsDevice, Width, Height);
                    });

                }
                // Set the current one to the previous one for next frame.
                _previousScreenSize = currentScreenSize;
            }
        }

        public static void DrawToScreen()
        {
            if (Main.gameMenu)
                return;


            //New d raw to screen

            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, CustomBlendStates.Multiply, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            
            spriteBatch.Draw(_accumulatedLightRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }
    }
}
