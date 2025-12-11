using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Pixelation;
using Stellamod.Core.ScreenSystems;
using Stellamod.Core.Waters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace Stellamod.Core.MoonWaters
{

   

    [Autoload(Side = ModSide.Client)]
    public class MoonWaterSystem : ModSystem
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
        private RenderTarget2D _reflectionRT;

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
            On_Main.DrawDust += CopyScreenTarget;
            On_OverlayManager.Draw += ApplyWaterShader;
            On_Main.DoDraw += CopyScreenTarget;
        }
        public override void Unload()
        {
            base.Unload();
            On_Main.CheckMonoliths -= RenderHook;
            On_Main.DrawDust -= CopyScreenTarget;
            On_OverlayManager.Draw -= ApplyWaterShader;
            On_Main.DoDraw -= CopyScreenTarget;
        }

        private void CopyScreenTarget(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            if (_reflectionRT == null)
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

        private void CopyScreenTarget(On_Main.orig_DoDraw orig, Main self, GameTime gameTime)
        {
            orig(self, gameTime);
            if(Main.mouseMiddle)
                DrawHeightMapToScreen();
            }



        private void ApplyWaterShader(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
        {
            orig(self, spriteBatch, layer, beginSpriteBatch);

            if(layer == RenderLayers.ForegroundWater)
            {
                //This is called right before the front water gets drawn
                //We can apply our shader here.
                //It should work, I think
                if (_waterEffect == null)
                    return;
               
                spriteBatch.End();
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

        private void RenderHook(On_Main.orig_CheckMonoliths orig)
        {
            orig();
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.LiquidsToggle)
                return;

            if (_hasLoaded && !Main.gameMenu)
            {
                RenderIntoHeightMapTarget();
                RenderIntoWaterTextureTarget();
                ForceRenderIntoWaterTarget();
            }
        }


        private Type[] _invokeTypes;
        private object[] _invokeParams;
        private void ForceRenderIntoWaterTarget()
        {
            //We're going to force the water target to update every frame
            //Might be terrible for performance but I'm not sure
            //The alternative of drawing a texture everywhere and hoping it works seems worse for performance?
            //Wait is that even an alternative
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


        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            ResizeRenderTargets();
        }

        private void DrawWaterBase(SpriteBatch spriteBatch)
        {
            _waterEffect.CurrentTechnique = _waterEffect.Techniques["SpriteDrawing"];
            _waterEffect.Parameters["tiling"].SetValue(Vector2.One * 2 * Tiling);
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

            Color baseColor = Color.CornflowerBlue * 0.75f;
            baseColor = baseColor.MultiplyRGB(Main.ColorOfTheSkies);
            spriteBatch.Draw(_waterNoise1, _drawLocation, null, baseColor);
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
            ApplyScreenOffset();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            spriteBatch.Draw(_waterCaustics, _drawLocation, null, Color.SeaGreen * 0.75f);
            spriteBatch.End();
        }

        private void DrawWaterSparkle(SpriteBatch spriteBatch)
        {
            _waterEffect.CurrentTechnique = _waterEffect.Techniques["SparklingCausticsDrawing"];
            _waterEffect.Parameters["time"].SetValue(_time * 2);
            _waterEffect.Parameters["distortion"].SetValue(0.05f);
            _waterEffect.Parameters["tiling"].SetValue(Vector2.One * 8 * Tiling);
            _waterEffect.Parameters["HeightMapTexture"].SetValue(_waterHeightMapRT);
            ApplyScreenOffset();

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
            ApplyScreenOffset();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            spriteBatch.Draw(_perlinNoise, _drawLocation, null, Color.White);
            spriteBatch.End();
        }

        private void ApplyScreenOffset()
        {
            Vector2 screenOffset = Main.screenPosition;

            _waterEffect.Parameters["screenOffset"].SetValue(screenOffset * 0.0005f);
        }
        private void RenderReflectionRT(SpriteBatch spriteBatch)
        {
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_reflectionRT);
            graphicsDevice.Clear(Color.Transparent);
    
            spriteBatch.Begin();
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            /*
            _waterEffect.CurrentTechnique = _waterEffect.Techniques["ReflectionDrawing"];
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            Player player = Main.LocalPlayer;
            Vector2 drawPosition = player.position;
            drawPosition.Y += player.gfxOffY;
            drawPosition += new Vector2(Main.offScreenRange, Main.offScreenRange);
            drawPosition.Y += 48;
            float rotation = player.fullRotation;

            IPlayerRenderer playerRenderer = Main.PlayerRenderer;
            playerRenderer.DrawPlayer(Main.Camera, player, drawPosition, rotation, player.fullRotationOrigin);
            spriteBatch.End();*/
        }


        private void DrawReflection(SpriteBatch spriteBatch)
        {
            _drawLocation = new Rectangle(0, 0, _waterTextureRT.Width, _waterTextureRT.Height);

            float mipBias = 1f;
            float reflectionDistance = 200;
            Vector2 reflectionTexelSize = (Vector2.One * mipBias) / new Vector2((float)Main.screenWidth, (float)Main.screenHeight);


            _waterEffect.CurrentTechnique = _waterEffect.Techniques["ReflectionDrawing"];
            _waterEffect.Parameters["reflectionDistance"].SetValue(reflectionDistance);
            _waterEffect.Parameters["reflectionTexelSize"].SetValue(reflectionTexelSize);
            _waterEffect.Parameters["HeightMapTexture"].SetValue(_waterHeightMapRT);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, _waterEffect);
            spriteBatch.Draw(_reflectionRT, Vector2.Zero, null, Color.White,0, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0);
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
            spriteBatch.Draw(_waterTextureRTSwap, _drawLocation, null, Color.Blue);
            spriteBatch.End();

            /*
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null);
            Vector2 drawPosition = Vector2.Zero + new Vector2(Main.offScreenRange);
            drawPosition.Y += 64;
            spriteBatch.Draw(_reflectionRT, drawPosition, null, Color.Blue * 0.8f);
            spriteBatch.End();*/

            graphicsDevice.SetRenderTarget(null);
        }

        private void RenderIntoWaterTextureTarget()
        {
            LoadAssets();
            _drawLocation = new Rectangle(0, 0, _waterTextureRT.Width, _waterTextureRT.Height);
            SpriteBatch spriteBatch = Main.spriteBatch;

  
            DrawWaterBase(spriteBatch);
            DrawWaterGradient(spriteBatch);
            DrawWaterCaustics(spriteBatch);
            DrawWaterSparkle(spriteBatch);
            DrawWaterFoam(spriteBatch);
            DrawReflection(spriteBatch);
            DrawPosterization(spriteBatch);
        }

        private void RenderIntoHeightMapTarget()
        {
            if (_waterEffect == null)
                return;
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

                    if (tile.LiquidAmount > 0)
                    {
                        //Move upward until we hit an air tile, so we know how deep this water tile is
                        int height = 0;
                        while (height < maxGradientHeight)
                        {
                            Tile aboveTile = Main.tile[j, i - height];
                            if (aboveTile.LiquidAmount == 0)
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

            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_waterHeightMapRT);
            graphicsDevice.Clear(Color.Transparent);
            Texture2D heightTile = TextureAssets.BlackTile.Value;


            _waterEffect.CurrentTechnique = _waterEffect.Techniques["HeightDrawing"];
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, _waterEffect);
            foreach (HeightDraw heightDraw in _heightsToDraw)
            {
                Vector2 drawPosition = heightDraw.tilePoint - Main.screenPosition;
                Color drawColor = new Color(1, 1, 1, heightDraw.height);
                spriteBatch.Draw(heightTile, drawPosition + new Vector2(Main.offScreenRange), drawColor);
            }
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
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
            spriteBatch.Draw(_waterHeightMapRT, - new Vector2(Main.offScreenRange), null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
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
                    _reflectionRT.Release();
                    _waterTextureRT.Release();
                    _waterTextureRTOutput.Release();
                    _waterTextureRTSwap.Release();

                    _waterTextureRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X / DownSamples, screenSize.Y / DownSamples);
                    _waterTextureRTSwap = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X / DownSamples, screenSize.Y / DownSamples);
                    _waterTextureRTOutput = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X, screenSize.Y);
                    _waterHeightMapRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X, screenSize.Y);
                    _reflectionRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X / DownSamples, screenSize.Y / DownSamples);
                    _hasLoaded = true;
                });
                _oldRenderTargetSize = screenSize;
            }
        }
    }
}
