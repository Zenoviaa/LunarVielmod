using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Core.Rendering;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    [Autoload(Side = ModSide.Client)]
    public class BlackStarRenderer : ModSystem
    {
        private Queue<IDrawBlackStar> _blackStarDraws;
        private RenderTargetProvider _maskTarget = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
        private RenderTargetProvider _blackStarTarget = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
        private BlackStarParticleManager _particleManager;
        private bool _renderStars;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _blackStarDraws = new Queue<IDrawBlackStar>(100);
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

        public static void QueueBlackStarDraw(IDrawBlackStar blackStar)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            BlackStarRenderer renderer = ModContent.GetInstance<BlackStarRenderer>();   
            renderer._blackStarDraws.Enqueue(blackStar);
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
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_maskTarget);
            graphicsDevice.Clear(Color.Transparent);
            _renderStars = false;
            if (_blackStarDraws.Count > 0)
            {
                _renderStars = true;
                spriteBatch.Begin();
                while (_blackStarDraws.Count > 0)
                {
                    IDrawBlackStar draw = _blackStarDraws.Dequeue();
                    draw.DrawBlackStar(spriteBatch);
                }
                spriteBatch.End();
            }
            graphicsDevice.SetRenderTarget(null);
        }

        private void RenderBlackStar()
        {
            if (!_renderStars)
                return;
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
            if(_renderStars)
            {
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

            orig(self);
        }

        private Point GetScreenSize()
        {
            return new Point(Main.screenWidth, Main.screenHeight);
        }
    }
}
