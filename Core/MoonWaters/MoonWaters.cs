using Accord.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Core.MoonWaters
{
    [Autoload(Side = ModSide.Client)]
    public class MoonWaters : ModSystem
    {
        private struct HeightDraw
        {
            public Vector2 tilePoint;
            public float height;
        }

        //So that we can have accurate gradients, we need to draw a height map based on how far the water tile is from the surface
        private bool _hasLoaded;
        private Point _oldRenderTargetSize;
        private RenderTarget2D _waterHeightMapRT;
        private RenderTarget2D _waterTextureRT;
        private RenderTarget2D _waterTextureRTSwap;
        private RenderTarget2D _waterTextureRTOutput;
        private RenderTarget2D _combineWaterRT;
        private List<HeightDraw> _heightsToDraw = new();

        private float _time;
        private Effect _waterEffect;
        private Rectangle _drawLocation;
        private Texture2D _waterCaustics;
        private Texture2D _perlinNoise;
        private Texture2D _waterNoise1;
        private Texture2D _waterNoise2;
        public int DownSamples => 2;
        public Vector2 Tiling => new Vector2(1f, 1.5f) * 0.75f;
        public override void Load()
        {
            ResizeRenderTargets();
            On_Main.CheckMonoliths += RenderHook;
            On_Main.DoDraw += RenderHook;
            On_Main.RenderWater += ApplyWaterShader;
        }
        public override void Unload()
        {
            base.Unload();
            On_Main.CheckMonoliths -= RenderHook;
            On_Main.DoDraw -= RenderHook;
            On_Main.RenderWater -= ApplyWaterShader;

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
            _waterCaustics = LoadTexture("WaterCaustics");
            _waterNoise1 = LoadTexture("WaterNoise1");
            _waterNoise2 = LoadTexture("WaterNoise2");
            _perlinNoise = LoadTexture("PerlinNoise");
        }

        private void ApplyWaterShader(On_Main.orig_RenderWater orig, Main self)
        {
            orig(self);
  
            if (!Main.drawToScreen)
            {
     
                //We can do whatever we want with the water target :)

                SpriteBatch spriteBatch = Main.spriteBatch;
                GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
                graphicsDevice.SetRenderTarget(_combineWaterRT);
                graphicsDevice.Clear(Color.Transparent);

                //Grab the shader and combine all of our water textures together

                _waterEffect.CurrentTechnique = _waterEffect.Techniques["CombineRTDrawing"];
                // _waterEffect.Parameters["startGradient"].SetValue(Color.Lerp(Color.Aqua, Color.Black, 0.25f).ToVector3());
                //  _waterEffect.Parameters["endGradient"].SetValue(Color.Lerp(Color.Blue, Color.Black, 0.25f).ToVector3());

                //_waterEffect.Parameters["HeightMapTexture"].SetValue(_waterHeightMapRT);
                _waterEffect.Parameters["WaterTexture"].SetValue(_waterTextureRTOutput);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                    _waterEffect); ;
                spriteBatch.Draw(Main.waterTarget, Vector2.Zero, Color.White);
                //  spriteBatch.Draw(_waterHeightMapRT, new Vector2(Main.offScreenRange), Color.White);

                spriteBatch.End();


                //Output the new combined result to the water render target
                graphicsDevice.SetRenderTarget(Main.waterTarget);
                graphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null);
                spriteBatch.Draw(_combineWaterRT, Vector2.Zero, Color.White * 0.75f);

                spriteBatch.End();
                graphicsDevice.SetRenderTarget(null);
            }
          

        }

        private void RenderHook(On_Main.orig_CheckMonoliths orig)
        {
            orig();
            if (_hasLoaded && !Main.gameMenu)
            {
                RenderIntoHeightMapTarget();
                RenderIntoWaterTextureTarget();
    
            }
        }

        private void RenderHook(On_Main.orig_DoDraw orig, Main self, GameTime gameTime)
        {
            orig(self, gameTime);
            if(_hasLoaded && !Main.gameMenu)
            {
            //   DrawWaterTextureToScreen();
      
            }
        }

        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            ResizeRenderTargets();
        }

        private void DrawWaterBase(SpriteBatch spriteBatch)
        {
            _waterEffect.Parameters["tiling"].SetValue(Vector2.One * 2 * Tiling);
            _waterEffect.Parameters["time"].SetValue(_time);
            _waterEffect.Parameters["levels"].SetValue(18);
            _waterEffect.Parameters["distortion"].SetValue(0.05f);
            _waterEffect.CurrentTechnique = _waterEffect.Techniques["SpriteDrawing"];
            Vector2 stretchScale = new Vector2(1, 0.5f);

            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_waterTextureRT);
            graphicsDevice.Clear(Color.LightSeaGreen);

            //Draw the base texture
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            spriteBatch.Draw(_waterNoise1, _drawLocation, null, Color.CornflowerBlue * 0.75f);
            spriteBatch.End();


            //Brigthten it up a bit
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            spriteBatch.Draw(_waterNoise2, _drawLocation, null, Color.White * 0.5f);
            spriteBatch.End();
        }

        private void DrawWaterGradient(SpriteBatch spriteBatch)
        {
            //gradient gonna have to be added later
            
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            //Set gradient effect values
            _waterEffect.CurrentTechnique = _waterEffect.Techniques["GradientDrawing"];
            _waterEffect.Parameters["startGradient"].SetValue(Color.Aqua.ToVector3());
            _waterEffect.Parameters["endGradient"].SetValue(Color.Lerp(Color.SeaGreen, Color.Black, 0.75f).ToVector3());


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
            _waterEffect.Parameters["tiling"].SetValue(Vector2.One * 6 * Tiling);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            spriteBatch.Draw(_waterCaustics, _drawLocation,  null, Color.SeaGreen * 0.75f);
            spriteBatch.End();
        }

        private void DrawWaterSparkle(SpriteBatch spriteBatch)
        {
            _waterEffect.CurrentTechnique = _waterEffect.Techniques["SparklingCausticsDrawing"];
            _waterEffect.Parameters["time"].SetValue(_time * 2);
            _waterEffect.Parameters["distortion"].SetValue(0.05f);
            _waterEffect.Parameters["tiling"].SetValue(Vector2.One * 8 * Tiling);
            _waterEffect.Parameters["HeightMapTexture"].SetValue(_waterHeightMapRT);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            spriteBatch.Draw(_waterCaustics, _drawLocation, null, Color.White * 0.5f);
            spriteBatch.End();
        }

        private void DrawWaterFoam(SpriteBatch spriteBatch)
        {
            _waterEffect.CurrentTechnique = _waterEffect.Techniques["FoamDrawing"];
            _waterEffect.Parameters["time"].SetValue(_time * 2);
            _waterEffect.Parameters["distortion"].SetValue(0.05f);
            _waterEffect.Parameters["tiling"].SetValue(Vector2.One * 2 * Tiling);
            _waterEffect.Parameters["HeightMapTexture"].SetValue(_waterHeightMapRT);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);

            Rectangle draw = _drawLocation;

            spriteBatch.Draw(_perlinNoise, draw, null, Color.White * 1);
            spriteBatch.Draw(_perlinNoise, draw, null, Color.White * 1);

            spriteBatch.End();
        }

        private void RenderIntoWaterTextureTarget()
        {
            LoadAssets();
            _drawLocation = new Rectangle(0, 0, _waterTextureRT.Width  , _waterTextureRT.Height);
            SpriteBatch spriteBatch = Main.spriteBatch;
            DrawWaterBase(spriteBatch);
            DrawWaterGradient(spriteBatch);
            DrawWaterCaustics(spriteBatch);
            DrawWaterSparkle(spriteBatch);
            DrawWaterFoam(spriteBatch);

            //Draw Caustics
            _waterEffect.Parameters["levels"].SetValue(10);
            _waterEffect.CurrentTechnique = _waterEffect.Techniques["PosterizeDrawing"];

            _drawLocation = new Rectangle(0, 0, _waterTextureRTOutput.Width, _waterTextureRTOutput.Height);
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_waterTextureRTOutput);
            graphicsDevice.Clear(Color.DeepSkyBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            spriteBatch.Draw(_waterTextureRTSwap, _drawLocation, null, Color.Blue);
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
        }

        private void RenderIntoHeightMapTarget()
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
                    if (tile == null)
                        continue;

                    if(tile.LiquidAmount > 0)
                    { 
                        //Move upward until we hit an air tile, so we know how deep this water tile is
                        int height = 0;
                        while(height < maxGradientHeight)
                        {
                            Tile aboveTile = Main.tile[j, i - height];
                            if (aboveTile.LiquidAmount == 0)
                            {
                                break;
                            }
                            height++;
                        }

                        HeightDraw heightDraw = new HeightDraw();
                        heightDraw.tilePoint = new Vector2(j, i).ToWorldCoordinates();

                        //Calculate the height value between 0-1
                        float heightSmoothing = (float)height / (float)maxGradientHeight;
                        heightDraw.height = 1f - heightSmoothing; 
                        _heightsToDraw.Add(heightDraw);
                    }
                }
            }

            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_waterHeightMapRT);
            graphicsDevice.Clear(Color.Transparent);
            Texture2D heightTile = TextureAssets.BlackTile.Value;

            spriteBatch.Begin();
            foreach(HeightDraw heightDraw in _heightsToDraw)
            {
                Vector2 drawPosition = heightDraw.tilePoint - Main.screenPosition;
                spriteBatch.Draw(heightTile, drawPosition + new Vector2(Main.offScreenRange), Color.Red * heightDraw.height);
            }
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
        }

        private void DrawHeightMapToScreen()
        {
            //This is just for testing purposes to make sure the texture looks the way we want it to
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(_waterHeightMapRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
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

        private void ResizeRenderTargets()
        {
            if (Main.gameMenu)
                return;

            Point screenSize = new Point(Main.waterTarget.Width, Main.waterTarget.Height);
            if (_oldRenderTargetSize != screenSize)
            {
                Main.QueueMainThreadAction(() =>
                {
                    _waterHeightMapRT.Release();
                    _combineWaterRT.Release();
                    _waterTextureRT.Release();
                    _waterTextureRTOutput.Release();
                    _waterTextureRTSwap.Release();

                    _waterTextureRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X / DownSamples, screenSize.Y / DownSamples);
                    _waterTextureRTSwap = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X / DownSamples, screenSize.Y / DownSamples);
                    _waterTextureRTOutput = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X, screenSize.Y);
                    _waterHeightMapRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X, screenSize.Y);
                    _combineWaterRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X, screenSize.Y);
                    _hasLoaded = true;
                });
                _oldRenderTargetSize = screenSize;
            }
        }
    }
}
