using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    [Autoload(Side = ModSide.Client)]
    public class BlackStarRenderer : ModSystem
    {
        private List<IDrawBlackStar> _blackStarDraws;
        private ManagedRenderTarget _maskTarget;
        private ManagedRenderTarget _blackStarTarget;
        private BlackStarParticleManager _particleManager;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _blackStarDraws = new List<IDrawBlackStar>();
            _blackStarTarget = ManagedRenderTarget.New(GetScreenSize);
            _maskTarget = ManagedRenderTarget.New(GetScreenSize);
            _particleManager = new BlackStarParticleManager(200, 30);
            On_Main.CheckMonoliths += Render;
            On_Main.DoDraw_DrawNPCsOverTiles += DrawBlackStarToScreen;
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.CheckMonoliths -= Render;
            On_Main.DoDraw_DrawNPCsOverTiles -= DrawBlackStarToScreen;
        }

        public override void PostUpdateDusts()
        {
            base.PostUpdateDusts();
            _particleManager.Update(new Vector2(Main.screenWidth, Main.screenHeight));
        }

        private void Render(On_Main.orig_CheckMonoliths orig)
        {
            RenderBlackStarMask();
            RenderBlackStar();
            orig();
        }

        private void RenderBlackStarMask()
        {
            _blackStarDraws.Clear();
            foreach (var projectile in Main.ActiveProjectiles)
            {
                if (projectile.ModProjectile is IDrawBlackStar draw)
                {
                    _blackStarDraws.Add(draw);
                }
            }

            if (_blackStarDraws.Count > 0)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
                graphicsDevice.SetRenderTarget(_maskTarget);
                graphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin();
                foreach (IDrawBlackStar draw in _blackStarDraws)
                {
                    draw.DrawBlackStar(spriteBatch);
                }
                spriteBatch.End();
                graphicsDevice.SetRenderTarget(null);
            }
        }

        private void RenderBlackStar()
        {
            if (_blackStarDraws.Count <= 0)
                return;
            if (InputHelper.KeyDown(Microsoft.Xna.Framework.Input.Keys.L))
            {
                _particleManager = new BlackStarParticleManager(200, 30);
            }
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_blackStarTarget);
            graphicsDevice.Clear(Color.Transparent);

            Texture2D starTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_62").Value;
            Vector2 drawOrigin = starTexture.Size() / 2f;

            spriteBatch.Begin();
            for (int i = 0; i < _particleManager.MaxParticleCount; i++)
            {
                ref var particle = ref _particleManager.Particles[i];
                Color drawColor = Color.White;
                drawColor.A = 0;

                float ratio = particle.time / _particleManager.Duration;
                float ease = EasingFunction.QuadraticBump(ratio);
                drawColor *= ease;

                Vector2 scale = Vector2.One;
                scale *= 0.5f;
                scale *= ExtraMath.Osc(0f, 2f, offset: i);
                spriteBatch.Draw(starTexture, particle.position, null, drawColor, 0, drawOrigin, scale, SpriteEffects.None, 0);
            }
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
        }

        private void DrawBlackStarToScreen(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            orig(self);
            if (_blackStarDraws.Count <= 0)
                return;
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;


            Vector2 v = Vector2.UnitX * 2;
            Vector2 h = Vector2.UnitY * 2;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
   null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(_maskTarget, v, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_maskTarget, -v, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_maskTarget, h, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_maskTarget, -h, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_maskTarget, Vector2.Zero, null, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            spriteBatch.End();

            //Setup the shader
            MaskCombineShader maskCombine = MaskCombineShader.Instance;
            maskCombine.MixTexture = _blackStarTarget;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
               maskCombine.Effect, Main.GameViewMatrix.TransformationMatrix);



            spriteBatch.Draw(_maskTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.End();
        }

        private Point GetScreenSize()
        {
            return new Point(Main.screenWidth, Main.screenHeight);
        }
    }
}
