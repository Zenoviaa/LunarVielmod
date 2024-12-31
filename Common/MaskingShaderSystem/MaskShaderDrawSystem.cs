using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Common.MaskingShaderSystem
{
    internal interface IPreDrawMaskShader
    {
        void PreDrawMask(SpriteBatch spriteBatch);
    }
    internal interface IDrawMaskShader
    {
        MiscShaderData GetMaskDrawShader();
        void DrawMask(SpriteBatch spriteBatch);
    }

    internal class MaskShaderDrawSystem : ModSystem
    {
        private List<IDrawMaskShader> _draws;
        private RenderTarget2D _preMaskDrawRenderTarget;
        private RenderTarget2D _maskRenderTarget;
        public List<IDrawMaskShader> Draws
        {
            get
            {
                _draws ??= new List<IDrawMaskShader>();
                _draws.Clear();
                foreach (var projectile in Main.ActiveProjectiles)
                {
                    if (projectile.ModProjectile is IDrawMaskShader draw)
                        _draws.Add(draw);
                }

                //Sort by shader
              //  _draws.Sort((x, y) => x.GetMaskDrawShader().ToString().CompareTo(y.GetMaskDrawShader().ToString()));
                return _draws;
            }
        }

        public Asset<Texture2D> MaskTexture { get; set; }
        public override void Load()
        {
            base.Load();
            MaskTexture = TextureRegistry.StarNoise2;
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
            _preMaskDrawRenderTarget ??= new RenderTarget2D(Main.instance.GraphicsDevice, width, height);
            _maskRenderTarget ??= new RenderTarget2D(Main.instance.GraphicsDevice, width, height);

            int drawCount = 0;
       //     Console.WriteLine(Draws.Count);
            //This should be in front of NPCS
            //Just need to loop over all of them and draw them
            //We do it this way so it's super duper optimized
            //Uhhh

            //Get the shader this thing is using to draw
            if (Draws.Count != 0)
            {
                GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
                graphicsDevice.SetRenderTarget(_preMaskDrawRenderTarget);
                graphicsDevice.Clear(Color.Transparent);

                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                    null, Main.GameViewMatrix.TransformationMatrix);


                for (int i = 0; i < Draws.Count; i++)
                {
                    var draw = Draws[i];
                    if (draw is IPreDrawMaskShader preDrawMaskShader)
                    {
                        preDrawMaskShader.PreDrawMask(spriteBatch);
                    }
                }

                spriteBatch.End();
                Main.graphics.GraphicsDevice.SetRenderTarget(null);
            }
            if (Draws.Count != 0)
            {
                GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
                graphicsDevice.SetRenderTarget(_maskRenderTarget);
                graphicsDevice.Clear(Color.Transparent);

                MiscShaderData drawShader = Draws[0].GetMaskDrawShader();
                drawShader.Apply();

                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                    drawShader.Shader, Main.GameViewMatrix.TransformationMatrix);


                for (int i = 0; i < Draws.Count; i++)
                {
                    var draw = Draws[i];
                    var currentShader = draw.GetMaskDrawShader();
                    if (drawShader != currentShader)
                    {
                        //Restart the batch
                        drawShader = currentShader;
                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                            drawShader.Shader, Main.GameViewMatrix.TransformationMatrix);

                    }

                    draw.DrawMask(spriteBatch);
                }
                spriteBatch.End();
                Main.graphics.GraphicsDevice.SetRenderTarget(null);
            }

            orig();
        }

        private void DrawTarget(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            if (Draws.Count != 0)
                DrawScaledTargetShaderLess(_preMaskDrawRenderTarget);
            orig(self);
            if (Draws.Count != 0)
                DrawScaledTarget(_maskRenderTarget);
        }

        private void DrawScaledTargetShaderLess(RenderTarget2D target)
        {
            if (target == null)
                return;
            if (Draws.Count == 0)
                return;

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None,
                Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
        }

        private void DrawScaledTarget(RenderTarget2D target)
        {
            if (target == null)
                return;
            if (Draws.Count == 0)
                return;

            var shaderData = GameShaders.Misc["LunarVeil:SimpleMasking"];
            shaderData.Shader.Parameters["uImageSize0"].SetValue(target.Size());
            shaderData.Shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 2);
            shaderData.Shader.Parameters["maskingColor"].SetValue(Color.White.ToVector3());
            shaderData.Shader.Parameters["maskingTexture"].SetValue(MaskTexture.Value);
            shaderData.Shader.Parameters["maskingTextureSize"].SetValue(MaskTexture.Value.Size());
            shaderData.Apply();
            //shaderData.Shader.Parameters["distortion"].SetValue(Distortion);
            //shaderData.Shader.Parameters["distortingNoiseTexture"].SetValue(DistortingNoiseTexture.Value);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None,
                Main.Rasterizer, shaderData.Shader, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
        }
    }
}
