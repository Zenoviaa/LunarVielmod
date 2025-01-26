using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Lights
{
    internal interface IDrawLightCast
    {
        void DrawLightCast(SpriteBatch spriteBatch);
    }

    internal class LightCaster : ModSystem
    {
        private List<IDrawLightCast> _draws;
        private RenderTarget2D _lightingRenderTarget;
        public List<IDrawLightCast> Draws
        {
            get
            {
                _draws ??= new List<IDrawLightCast>();
                _draws.Clear();
                foreach (var projectile in Main.ActiveProjectiles)
                {
                    if (projectile.ModProjectile is IDrawLightCast draw)
                        _draws.Add(draw);
                }

                //Sort by shader
                return _draws;
            }
        }

        public override void Load()
        {
            base.Load();
            On_Main.CheckMonoliths += DrawMaskToRenderTarget;
            On_Main.DoDraw_DrawNPCsOverTiles += DrawTarget;
        }

        public override void Unload()
        {
            base.Unload();
            On_Main.CheckMonoliths -= DrawMaskToRenderTarget;
            On_Main.DoDraw_DrawNPCsOverTiles -= DrawTarget;
        }

        private void DrawMaskToRenderTarget(On_Main.orig_CheckMonoliths orig)
        {
            int width = Main.screenWidth;
            int height = Main.screenHeight;
            _lightingRenderTarget ??= new RenderTarget2D(Main.instance.GraphicsDevice, width, height);

            //This should be in front of NPCS
            //Just need to loop over all of them and draw them
            //We do it this way so it's super duper optimized
            //Uhhh

            //Get the shader this thing is using to draw
            if (Draws.Count != 0)
            {
                GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
                graphicsDevice.SetRenderTarget(_lightingRenderTarget);
                graphicsDevice.Clear(Color.Transparent);

                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                    null, Main.GameViewMatrix.TransformationMatrix);


                for (int i = 0; i < Draws.Count; i++)
                {
                    var draw = Draws[i];
                    draw.DrawLightCast(spriteBatch);
                }

                spriteBatch.End();
                Main.graphics.GraphicsDevice.SetRenderTarget(null);
            }

            orig();
        }

        private void DrawTarget(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            if (Draws.Count != 0)
                DrawScaledTargetShaderLess(_lightingRenderTarget);
            orig(self);
        }

        private void DrawScaledTargetShaderLess(RenderTarget2D target)
        {
            if (target == null)
                return;
            if (Draws.Count == 0)
                return;

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None,
                Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            Color drawColor = Color.White;
            drawColor *= 0.33f;
            Main.spriteBatch.Draw(target, Vector2.Zero, null, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
        }

        private void DrawScaledTarget(RenderTarget2D target)
        {
            if (target == null)
                return;
            if (Draws.Count == 0)
                return;

            //shaderData.Shader.Parameters["distortion"].SetValue(Distortion);
            //shaderData.Shader.Parameters["distortingNoiseTexture"].SetValue(DistortingNoiseTexture.Value);

            Color drawColor = Color.White;
            //  drawColor.A = 0;
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, CustomBlendState.Multiply, SamplerState.PointClamp, DepthStencilState.None,
                Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(target, Vector2.Zero, null, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
        }
    }
}
