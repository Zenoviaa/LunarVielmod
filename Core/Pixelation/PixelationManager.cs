using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Pixelation
{
    public interface IDrawPixelated
    {
        void DrawPixelated();
    }
    public static class RenderTargetExtensions
    {
        public static void Release(this RenderTarget2D rt)
        {
            if (rt != null && !rt.IsDisposed)
                rt.Dispose();
        }
    }

    [Autoload(Side = ModSide.Client)]
    public class PixelationManager : ModSystem
    {
        private Point _oldScreenSize;
        private RenderTarget2D _pixelRenderRT;
        private RenderTarget2D _pixelScreenRenderRT;
        private RenderTarget2D _smokePixelRenderRT;
        private RenderTarget2D _smokeScreenRenderRT;
        private List<IDrawPixelated> _draws = new List<IDrawPixelated>(100);

        public int DownSamples => 2;
        public static event Action OnDrawPixelation;
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Main.CheckMonoliths += RenderToPixelationRT;
            On_Main.DoDraw_DrawNPCsOverTiles += DrawPixelRTToScreen;
            Main.OnResolutionChanged += ResizeTargets;
        }

        public override void Load()
        {
            base.Load();
            ResizeRenderTargets();
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.CheckMonoliths -= RenderToPixelationRT;
            On_Main.DoDraw_DrawNPCsOverTiles -= DrawPixelRTToScreen;
            Main.OnResolutionChanged -= ResizeTargets;
        }

        private void RenderToPixelationRT(On_Main.orig_CheckMonoliths orig)
        {
            orig();
            if (Main.gameMenu)
                return;

            _draws.Clear();
            foreach(var proj in Main.ActiveProjectiles)
            {
                if(proj.ModProjectile is IDrawPixelated pixelated)
                {
                    _draws.Add(pixelated);
                }
            }
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_pixelScreenRenderRT);
            graphicsDevice.Clear(Color.Black);
            if (_draws.Count > 0)
            {
                //Alright, so what we're going to do is actually use two render targets to get around the issue of misplaced pixels
                //This costs a bit of extra performance but it'll look good
                //So, first draw at fully quality to the screen render target

                for (int i = 0; i < _draws.Count; i++)
                {
                    IDrawPixelated draw = _draws[i];
                    draw.DrawPixelated();
                }

            }
            OnDrawPixelation?.Invoke();

            //Now we take that output and downscale it to the pixel RT
            graphicsDevice.SetRenderTarget(_pixelRenderRT);
            graphicsDevice.Clear(Color.Transparent);

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            float denom = DownSamples;
            float scale = 1f / denom;


            spriteBatch.Draw(_pixelScreenRenderRT, Vector2.Zero, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            spriteBatch.End();
        }


        private void DrawPixelRTToScreen(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            orig(self);
            if (Main.gameMenu)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            float scale = DownSamples;

            spriteBatch.Draw(_pixelRenderRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.End();
        }

        private void ResizeRenderTargets()
        {
            Point screenSize = Main.ScreenSize;
            if (_oldScreenSize != screenSize)
            {
                Main.QueueMainThreadAction(() =>
                {
                    _pixelRenderRT.Release();
                    _pixelRenderRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X / DownSamples, screenSize.Y / DownSamples);

                    _pixelScreenRenderRT.Release();
                    _pixelScreenRenderRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X, screenSize.Y);


                    //Smoke screen render targets
                    _smokePixelRenderRT.Release();
                    _smokePixelRenderRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X / DownSamples, screenSize.Y / DownSamples);


                    _smokeScreenRenderRT.Release();
                    _smokeScreenRenderRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X, screenSize.Y);

                });
                _oldScreenSize = screenSize;
            }
        }
        private void ResizeTargets(Vector2 vector)
        {
            ResizeRenderTargets();
        }
    }
}
