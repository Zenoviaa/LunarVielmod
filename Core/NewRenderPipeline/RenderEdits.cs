using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.Palettes;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Core.NewRenderPipeline
{
    [Autoload(Side = ModSide.Client)]
    public class RenderEdits : ModSystem
    {
        private bool _activeThisFrame;
        private float _oldProgress;
        private Vector2 _previousScreenSize;
        private Effect _paletteShader;
        private RenderTarget2D _solidTileRT;
        private RenderTarget2D _tempScreenRT;

        public override void Load()
        {
            ResizeRenderTarget(true);
        }

     
        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            ResizeRenderTarget(false);
            _paletteShader = null;
            if (!_activeThisFrame)
            {
                var shader = PalettizerShader.Instance;
                _oldProgress -= 0.05f;
                if(_oldProgress <= 0f)
                {
                    _oldProgress = 0f;
                }
                shader.Progress = _oldProgress;
            }
            _activeThisFrame = false;
        }

        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Main.RenderTiles += NewRenderTiles;
            On_Main.RenderWalls += NewRenderWalls;
            On_Main.RenderTiles2 += NewRenderTiles2;
            On_Main.DoDraw_Tiles_Solid += NewTilesSolid;
            On_Main.DoDraw += DrawLoop;
            On_Main.DoDraw_WallsTilesNPCs += NewWallsTilesNPCs;
        }



        private void DrawLoop(On_Main.orig_DoDraw orig, Main self, GameTime gameTime)
        {
            RenderToSolidTilesRT();
            orig(self, gameTime);
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.RenderTiles -= NewRenderTiles;
            On_Main.RenderWalls -= NewRenderWalls;
            On_Main.RenderTiles2 -= NewRenderTiles2;
            On_Main.DoDraw_Tiles_Solid -= NewTilesSolid;
            On_Main.DoDraw -= DrawLoop;
            On_Main.DoDraw_WallsTilesNPCs -= NewWallsTilesNPCs;
        }

        private void NewRenderTiles2(On_Main.orig_RenderTiles2 orig, Main self)
        {
            orig(self);
        }

        private void NewRenderWalls(On_Main.orig_RenderWalls orig, Main self)
        {
            orig(self);

        }

        private void NewRenderTiles(On_Main.orig_RenderTiles orig, Main self)
        {
            orig(self);

            /*
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.SplitPaletteShaders)
                return;

            //Since the palette shader is optimized now we should be able to do this
            //What I'm thinking of doing is multiplying the lighting rendering target here?
            //That means we'll multiple lighting twice but that's ok
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            RenderTarget2D accumulatedLightRT = LunarLightingRenderer.GetAccumulatedLightMap();
            graphicsDevice.SetRenderTarget(Main.instance.tileTarget);

            //Hopefully the render target content is preserved and not lost
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, CustomBlendStates.Multiply, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(accumulatedLightRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);*/
        }

        private void RenderToSolidTilesRT()
        {
            /*
            //This is basically just a clone of how it works in terraria's source code, except we're rendering into a Render Target and doing it elsewhere
            //Since we can't swap render targets without losing data we have to do it this way
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_solidTileRT);
            graphicsDevice.Clear(Color.Transparent);

            SpriteBatch spriteBatch = Main.spriteBatch;
            TileBatch tileBatch = Main.tileBatch;

            Main.instance.TilesRenderer.PreDrawTiles(solidLayer: true, !Main.drawToScreen, intoRenderTargets: false);
            tileBatch.Begin(Main.Rasterizer, Main.Transform);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            try
            {
                if (Main.drawToScreen)
                {
                    Main.instance.TilesRenderer.Draw(true, !Main.drawToScreen, false, -1);
                }
                else
                {
                    spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition, Microsoft.Xna.Framework.Color.White);
                    TimeLogger.DetailedDrawTime(17);
                }
            }
            catch (Exception e)
            {
                TimeLogger.DrawException(e);
            }

            tileBatch.End();
            spriteBatch.End();
            Main.instance.TilesRenderer.PostDrawTiles(true, !Main.drawToScreen, false);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            try
            {
                Main.player[Main.myPlayer].hitReplace.DrawFreshAnimations(spriteBatch);
                Main.player[Main.myPlayer].hitTile.DrawFreshAnimations(spriteBatch);
            }
            catch (Exception e2)
            {
                TimeLogger.DrawException(e2);
            }

            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);*/
        }
        private void NewTilesSolid(On_Main.orig_DoDraw_Tiles_Solid orig, Main self)
        {
            orig(self);
        }
        private void NewWallsTilesNPCs(On_Main.orig_DoDraw_WallsTilesNPCs orig, Main self)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();

            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_tempScreenRT);
            graphicsDevice.Clear(Color.Transparent);


            //We do this we we don't lose the contents
     
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();


            graphicsDevice.SetRenderTarget(_solidTileRT);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            orig(self);

            spriteBatch.End();

            //Multiply lights with our rendering
            var lightingTarget = LunarLightingRenderer.GetAccumulatedLightMap();
            spriteBatch.Begin(SpriteSortMode.Immediate, CustomBlendStates.Multiply, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            spriteBatch.Draw(lightingTarget, Vector2.Zero, Color.White);
            spriteBatch.End();


            //Output back to the screen, restore the contents that were on the screen
            Main.graphics.GraphicsDevice.SetRenderTarget(Main.screenTarget);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            spriteBatch.Draw(_tempScreenRT, Vector2.Zero, Color.White);
            spriteBatch.End();


            var paletteShader = PalettizerShader.Instance;
        //    paletteShader.Progress = 1f;
        //    paletteShader.PaletteTexture = PaletteHelper.GetColorSpectrum("Fable.pal");



            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, paletteShader.Effect, Main.Transform);
            spriteBatch.Draw(_solidTileRT, Vector2.Zero, Color.White);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }

        public void UsePaletteShader(string palFile, bool isActive, ref float progress)
        {
            if (isActive)
            {
                _activeThisFrame = true;
                _oldProgress = progress;
                float speed = 0.05f;
                if (isActive)
                {
                    progress += speed;
                }
                else
                {
                    progress -= speed;
                }
                progress = MathHelper.Clamp(progress, 0f, 1f);

                var paletteShader = PalettizerShader.Instance;
                paletteShader.Progress = progress;
                paletteShader.PaletteTexture = PaletteHelper.GetColorSpectrum(palFile);

      
            }
      
        }

        private FilterManager FilterManager => Filters.Scene;
        public Effect GetPaletteShader(string palFile, bool isActive, ref float progress)
        {
            float speed = 0.05f;
            if (isActive)
            {
                progress += speed;
            }
            else
            {
                progress -= speed;
            }
            progress = MathHelper.Clamp(progress, 0f, 1f);

            string screenShaderName = $"LunarVeil:{palFile}";
            if (!ShaderRegistry.ScreenShaders.Contains(screenShaderName))
            {
                return null;
            }


            if (progress > 0)
            {
                ScreenShaderData screenShaderData = FilterManager[screenShaderName].GetShader();
                Effect effect = screenShaderData.Shader;
                effect.Parameters["uProgress"].SetValue(progress);
                effect.Parameters["ColorSpectrumTexture"].SetValue(PaletteHelper.GetColorSpectrum(palFile));

                return effect;
            }

            return null;
        }
        private void ResizeRenderTarget(bool load)
        {
            if (!Main.gameMenu && !Main.dedServ || load && !Main.dedServ)
            {
                Vector2 currentScreenSize = new(Main.screenWidth, Main.screenHeight);
                if (currentScreenSize != _previousScreenSize)
                {
                    Main.QueueMainThreadAction(() =>
                    {
                        if (_solidTileRT != null && !_solidTileRT.IsDisposed)
                            _solidTileRT.Dispose();
                        if (_tempScreenRT != null && !_tempScreenRT.IsDisposed)
                            _tempScreenRT.Dispose();

                        _solidTileRT = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight, false,
                            SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                        _tempScreenRT = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight, false,
                            SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                    });

                }

                _previousScreenSize = currentScreenSize;
            }
        }
    }
}
