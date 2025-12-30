using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.SummonerSystem
{
    public interface IDrawSpectral
    {
        void DrawSpectralWhites(SpriteBatch spriteBatch);
        void DrawSpectral(SpriteBatch spriteBatch);
    }

    [Autoload(Side = ModSide.Client)]
    public class SpectralSummonDrawSystem : ModSystem
    {
        private Vector2 _prevScreenSize;

        private static RenderTarget2D _rt;
        private static List<IDrawSpectral> _spectralDraws = new();


        public override void Load()
        {
            On_Main.CheckMonoliths += DrawToCustomRenderTargets;
            On_Main.DoDraw_DrawNPCsOverTiles += DrawPixelRenderTarget;

            ResizeRenderTarget(true);
        }

        public override void Unload()
        {
            On_Main.CheckMonoliths -= DrawToCustomRenderTargets;
            On_Main.DoDraw_DrawNPCsOverTiles -= DrawPixelRenderTarget;
        }

        public override void PostUpdateEverything() => ResizeRenderTarget(false);

        private void DrawPixelRenderTarget(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            orig(self);
            var shader = SpectralShader.Instance;
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                shader.Effect, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(_rt, Vector2.Zero, null, Color.White * 0.47f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
        }

        private void DrawToCustomRenderTargets(On_Main.orig_CheckMonoliths orig)
        {
            // Clear our render target from the previous frame.
            _spectralDraws.Clear();
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.ModProjectile is IDrawSpectral minion)
                {
                    _spectralDraws.Add(minion);
                }
            }

            // Draw the prims. The render target gets set here.
            DrawToRenderTarget(_rt, _spectralDraws);

            // Clear the current render target.
            Main.graphics.GraphicsDevice.SetRenderTarget(null);

            // Call orig.
            orig();
        }

        private static void DrawToRenderTarget(RenderTarget2D renderTarget, List<IDrawSpectral> drawSpectrals)
        {
            // Swap to our custom render target.
            SwapToRenderTarget(renderTarget);
            if (drawSpectrals.Count > 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, SpriteWhiteShader.Instance.Effect);

                foreach (var drawer in drawSpectrals)
                {
                    drawer.DrawSpectralWhites(Main.spriteBatch);
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);

                foreach (var drawer in drawSpectrals)
                {
                    drawer.DrawSpectral(Main.spriteBatch);
                }

                Main.spriteBatch.End();
            }
        }

        private static void SwapToRenderTarget(RenderTarget2D renderTarget)
        {
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;

            // If we are in the menu, a server, or any of these are null, return.
            if (Main.gameMenu || Main.dedServ || renderTarget is null || graphicsDevice is null || spriteBatch is null)
                return;

            // Else, set the render target.
            graphicsDevice.SetRenderTarget(renderTarget);
            // "Flush" the screen, removing any previous things drawn to it.
            graphicsDevice.Clear(Color.Transparent);
        }

        private void ResizeRenderTarget(bool load)
        {
            // If not in the game menu, and we arent a dedicated server,
            if (!Main.gameMenu && !Main.dedServ || load && !Main.dedServ)
            {
                // Get the current screen size.
                Vector2 currentScreenSize = new(Main.screenWidth, Main.screenHeight);
                // If it does not match the previous one, we need to update it.
                if (currentScreenSize != _prevScreenSize)
                {
                    Main.QueueMainThreadAction(() =>
                    {
                        if (_rt != null && !_rt.IsDisposed)
                            _rt.Dispose();

                        _rt = new RenderTarget2D(Main.graphics.GraphicsDevice,
                            Main.screenWidth,
                            Main.screenHeight);
                    });

                }

                _prevScreenSize = currentScreenSize;
            }
        }
    }
}
