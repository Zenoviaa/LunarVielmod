using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

/*

- Deer with a singularity for a head, in its spawn animation at first it looks like a normal deer before the head explodes and parts start orbiting it, ooo I know exactly how to code this

- The legs and everything are rigged, we’ll use forward kinematics to animate the boss, so we’ll have to make a run animation and idle animation

- Opens the fight with several exploding blood magic projectiles that loosely track the player

- Winds up a charge and then runs directly at the player really fast, and explodes into bloody bits before merging itself back together elsewhere

- Runs up into the sky and rains down acidic blood

- Walks slowly around the player as bloody boils explode from its body and then home back towards you

- Cracks form in its body and it violently erupts into multiple bloody geysers

- Winds up a charge and then keeps running at you while swerving around and trying to juke you out
 
- In phase 2 every attack gets more deadlier, triggers at under 50% health
 */
namespace Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity
{
    [Autoload(Side = ModSide.Client)]
    public class SanguineBloodRenderManager : ModSystem
    {

        private Point _oldScreenSize;
        private RenderTarget2D _bloodBGRenderRT;
        private RenderTarget2D _pixelRenderRT;
        private RenderTarget2D _pixelScreenRenderRT;
        private List<IDrawSanguineBlood> _draws = new List<IDrawSanguineBlood>(100);

        public int DownSamples => 4;
        public bool DrawBloodyBG;
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Main.CheckMonoliths += RenderToPixelationRT;
            On_Main.DoDraw_WallsTilesNPCs += DrawBloodRTToScreen;
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
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.ModProjectile is IDrawSanguineBlood pixelated)
                {
                    _draws.Add(pixelated);
                }
            }
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            if (_draws.Count > 0)
            {

                graphicsDevice.SetRenderTarget(_pixelScreenRenderRT);
                graphicsDevice.Clear(Color.Transparent);

                //Alright, so what we're going to do is actually use two render targets to get around the issue of misplaced pixels
                //This costs a bit of extra performance but it'll look good
                //So, first draw at fully quality to the screen render target

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null);
                for (int i = 0; i < _draws.Count; i++)
                {
                    var draw = _draws[i];
                    draw.DrawToSanguineMask(spriteBatch);
                }
                spriteBatch.End();


                //Now we take that output and downscale it to the pixel RT
                graphicsDevice.SetRenderTarget(_pixelRenderRT);
                graphicsDevice.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                float denom = DownSamples;
                float scale = 1f / denom;
                spriteBatch.Draw(_pixelScreenRenderRT, Vector2.Zero, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                spriteBatch.End();
            }
            graphicsDevice.SetRenderTarget(_bloodBGRenderRT);
            graphicsDevice.Clear(Color.White);
        }


        private void DrawBloodRTToScreen(On_Main.orig_DoDraw_WallsTilesNPCs orig, Main self)
   
        {
            if (DrawBloodyBG)
            {
                var bloodyShader = BloodyShader.Instance;
                bloodyShader.InnerColor = Color.Lerp(Color.Red, Color.Black, 0.9f);
                bloodyShader.OuterColor = Color.Black;
                bloodyShader.Distortion = 1;
                bloodyShader.Tiling = Vector2.One * 12;
                bloodyShader.Time = Main.GlobalTimeWrappedHourly * 0.25f;
                bloodyShader.NoiseTexture = TextureRegistry.Clouds6;
                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, bloodyShader.Effect, Main.Transform);
                spriteBatch.Draw(_bloodBGRenderRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
                DrawBloodyBG = false;
            }
          
            orig(self);
            if (Main.gameMenu)
                return;
         
        }

        private void DrawPixelRTToScreen(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            orig(self);
            if (Main.gameMenu)
                return;

            if (_draws.Count <= 0)
                return;

            var bloodyShader = BloodyShader.Instance;
            bloodyShader.InnerColor = Color.Red;
            bloodyShader.OuterColor = Color.Black;
            bloodyShader.Distortion = 0.1f;
            bloodyShader.Tiling = Vector2.One * 8;

            bloodyShader.NoiseTexture = TextureRegistry.CloudNoise2;
            SpriteBatch spriteBatch = Main.spriteBatch;
            float scale = DownSamples;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.Default, RasterizerState.CullNone, null);


            float outlineOffset = 2;
            Vector2 v = Vector2.UnitY * outlineOffset;
            Vector2 h = Vector2.UnitX * outlineOffset;
            spriteBatch.Draw(_pixelRenderRT, Vector2.Zero + v, null, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(_pixelRenderRT, Vector2.Zero - v, null, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(_pixelRenderRT, Vector2.Zero + h, null, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(_pixelRenderRT, Vector2.Zero - h, null, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.End();


            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, 
                DepthStencilState.Default, RasterizerState.CullNone, bloodyShader.Effect);

     

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
                    _pixelRenderRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X / DownSamples, screenSize.Y / DownSamples, false, SurfaceFormat.Color, DepthFormat.None);

                    _pixelScreenRenderRT.Release();
                    _pixelScreenRenderRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X, screenSize.Y, false, SurfaceFormat.Color, DepthFormat.None);
                  
                    _bloodBGRenderRT.Release();
                    _bloodBGRenderRT = new RenderTarget2D(Main.graphics.GraphicsDevice, screenSize.X, screenSize.Y, false, SurfaceFormat.Color, DepthFormat.None);

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
