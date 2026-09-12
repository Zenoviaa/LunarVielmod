using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Rendering.RTs;
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
        private BlackStarParticleManager _particleManager;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _blackStarDraws = new Queue<IDrawBlackStar>(100);
            _particleManager = new BlackStarParticleManager(200, 30);
            On_Main.DoDraw_DrawNPCsOverTiles += DrawBlackStarToScreen;
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
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

        private void DrawBlackStarToScreen(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            if(_blackStarDraws.Count > 0)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
                RenderTargetHandle maskTarget = RenderTargets.ScreenTarget;
                RenderTargetHandle blackStarTarget = RenderTargets.ScreenTarget;
                using(new RenderTargetContext(blackStarTarget))
                {
                    Texture2D starTexture = AssetReferences.Assets.NoiseTextures.Extra_62.Asset.Value;
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
                }

                using(new RenderTargetContext(maskTarget))
                {
                    spriteBatch.Begin();
                    while (_blackStarDraws.Count > 0)
                    {
                        IDrawBlackStar draw = _blackStarDraws.Dequeue();
                        draw.DrawBlackStar(spriteBatch);
                    }
                    spriteBatch.End();
                }

                Vector2 v = Vector2.UnitX * 2;
                Vector2 h = Vector2.UnitY * 2;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
       null, Main.GameViewMatrix.TransformationMatrix);

                spriteBatch.Draw(maskTarget, v, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(maskTarget, -v, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(maskTarget, h, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(maskTarget, -h, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(maskTarget, Vector2.Zero, null, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                spriteBatch.End();

                //Setup the shader
                MaskCombineShader maskCombine = MaskCombineShader.Instance;
                maskCombine.MixTexture = blackStarTarget;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                   maskCombine.Effect, Main.GameViewMatrix.TransformationMatrix);



                spriteBatch.Draw(maskTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
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
