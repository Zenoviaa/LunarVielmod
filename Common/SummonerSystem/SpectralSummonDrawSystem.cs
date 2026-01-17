using Stellamod.Common.Shaders;
using Stellamod.Core.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.SummonerSystem
{
    public interface IDrawSpectral
    {
        void DrawSpectralWhites(SpriteBatch spriteBatch);
        void DrawSpectral(SpriteBatch spriteBatch);
    }

    [Autoload(Side = ModSide.Client)]
    public class SpectralSummonDrawSystem : ModSystem
    {
        private ManagedRenderTarget _spectralRenderTarget;
        private static List<IDrawSpectral> _spectralDraws = new();
        public override void OnModLoad()
        {
            base.OnModLoad();
            _spectralRenderTarget = ManagedRenderTarget.New();
        }

        public override void Load()
        {
            On_Main.CheckMonoliths += DrawToCustomRenderTargets;
            On_Main.DoDraw_DrawNPCsOverTiles += DrawPixelRenderTarget;
        }

        public override void Unload()
        {
            On_Main.CheckMonoliths -= DrawToCustomRenderTargets;
            On_Main.DoDraw_DrawNPCsOverTiles -= DrawPixelRenderTarget;
            _spectralDraws?.Clear();
            _spectralDraws = null;
            _spectralRenderTarget = null;
        }


        private void DrawPixelRenderTarget(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            orig(self);
            if (Main.gameMenu)
                return;

            var shader = SpectralShader.Instance;
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                shader.Effect, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(_spectralRenderTarget, Vector2.Zero, null, Color.White * 0.87f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
        }

        private void DrawToCustomRenderTargets(On_Main.orig_CheckMonoliths orig)
        {
            if (!Main.gameMenu)
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
                DrawToRenderTarget(_spectralRenderTarget, _spectralDraws);

                // Clear the current render target.
                Main.graphics.GraphicsDevice.SetRenderTarget(null);

            }

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
    }
}
