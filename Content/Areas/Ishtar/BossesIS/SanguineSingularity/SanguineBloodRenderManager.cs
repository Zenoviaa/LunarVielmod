using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
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
        public float FlickerTimer;
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Main.CheckMonoliths += RenderToPixelationRT;
            On_Main.DrawNPCs += DrawBlack;
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
            On_Main.DrawNPCs -= DrawBlack;
            On_Main.DoDraw_WallsTilesNPCs -= DrawBloodRTToScreen;
            On_Main.DoDraw_DrawNPCsOverTiles -= DrawPixelRTToScreen;
            Main.OnResolutionChanged -= ResizeTargets;
        }

        private float _beatCounter;
        private float _beatTimer;
        private bool _in;
        private float _timer;
        private float _scale;

        public void ResetMetronome()
        {
            _beatTimer = 0f;
            _beatCounter = 0f;
            _beatTimer = 0f;
            _in = false;
        }

        private void Metronome()
        {

            float beatsPerTick = 130 / 60f / 60f;
            _beatTimer += beatsPerTick;

  
            while (_beatTimer >= 1f)
            {
                _beatTimer -= 1f;
                _beatCounter++;
            }
            if(_beatCounter % 8 == 0)
            {
                _in = !_in;
            }

            if (_in)
            {
                _timer++;
            }
            else
            {
                _timer--;
            }

            float time = 600f;
            _timer = MathHelper.Clamp(_timer, 0, time);
            float completionRatio = _timer / time;
            _scale = MathHelper.Lerp(1f, 1.1f, completionRatio);
        }
        public override void PostUpdateNPCs()
        {
            base.PostUpdateNPCs();
            if (FlickerTimer > 0)
                FlickerTimer--;
            Metronome();
        }


        private void DrawBloodyBG1()
        {
            var bloodyShader = BloodyShader.Instance;
            bloodyShader.InnerColor = Color.Lerp(Color.Red, Color.Black, 0.7f);
            bloodyShader.OuterColor = Color.Black;
            bloodyShader.Distortion = 1;
            bloodyShader.Tiling = Vector2.One * 12;
            bloodyShader.Time = Main.GlobalTimeWrappedHourly * 3;
            bloodyShader.NoiseTexture = TextureRegistry.Clouds6;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, bloodyShader.Effect, Main.Transform);

            float alpha = FlickerTimer > 0 ? ExtraMath.Osc(0f, 1f, speed: 2) : 1;

            Vector2 centerOrigin = _bloodBGRenderRT.Size() / 2f;
            spriteBatch.Draw(_bloodBGRenderRT, centerOrigin, null, Color.White * alpha * 0.55f, 0f, centerOrigin, _scale, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }

        private void DrawBloodyBG2()
        {
            var bloodStormShader = BloodStormShader.Instance;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, bloodStormShader.Effect, Main.Transform);

            Vector2 centerOrigin = _bloodBGRenderRT.Size() / 2f;

            float alpha = FlickerTimer > 0 ? ExtraMath.Osc(0f, 1f, speed: 2) : 1;
            Texture2D vortexTexture = AssetRegistry.Textures.Noise.JungleWaterCaustics.Value;
            Vector2 scaleMult = _bloodBGRenderRT.Size() / vortexTexture.Size();
            spriteBatch.Draw(vortexTexture, centerOrigin, null, Color.White * alpha, 0f, vortexTexture.Size() / 2f, scaleMult * _scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }
        private void DrawBlack(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (DrawBloodyBG)
            {
                GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
                graphicsDevice.Clear(Color.Transparent);
                DrawBloodyBG2();
                DrawBloodyBG1();
                DomainExpansionManager singularityFallSystem = ModContent.GetInstance<DomainExpansionManager>();
                if (singularityFallSystem.hoveringPlatform)
                {
                    Texture2D bloomLine = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
                    Vector2 drawOrigin = new Vector2(bloomLine.Size().X / 2, 0);
                    float rotation = MathHelper.PiOver2;
                    Color drawColor = Color.Red;
                    drawColor.A = 0;
                    drawColor *= 0.5f;
                    drawColor *= ExtraMath.Osc(0.5f, 1f);
                    Vector2 drawPosition = new Vector2(Main.LocalPlayer.Center.X, singularityFallSystem.hoverPlatformY);
                    drawPosition -= Main.screenPosition;
                    drawPosition.Y += 48;
                    Vector2 drawScale = new Vector2(1, 2);
                    spriteBatch.Draw(bloomLine, drawPosition, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                    spriteBatch.Draw(bloomLine, drawPosition, null, drawColor, -rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                }
                DrawBloodyBG = false;
            }
  
            orig(self, behindTiles);
        }

        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            ResizeRenderTargets();
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

            float redOutlineOffset = 4;
            Vector2 v = Vector2.UnitY * redOutlineOffset;
            Vector2 h = Vector2.UnitX * redOutlineOffset;
            spriteBatch.Draw(_pixelRenderRT, Vector2.Zero + v, null, Color.Red, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(_pixelRenderRT, Vector2.Zero - v, null, Color.Red, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(_pixelRenderRT, Vector2.Zero + h, null, Color.Red, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(_pixelRenderRT, Vector2.Zero - h, null, Color.Red, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            float outlineOffset = 2;
            v = Vector2.UnitY * outlineOffset;
            h = Vector2.UnitX * outlineOffset;


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
