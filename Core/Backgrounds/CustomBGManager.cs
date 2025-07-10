using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Effects;
using Stellamod.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Core.Backgrounds
{
    internal class CustomBGManager : ModSystem
    {
        private BlendState _currentBlendState;
        private Shader _currentShader;
        public List<CustomBG> Backgrounds = new List<CustomBG>();
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_Main.DrawBG += DrawBackground;
            Backgrounds = ModContent.GetContent<CustomBG>().ToList();
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.DrawBG -= DrawBackground;
        }

        private void DrawBackground(On_Main.orig_DrawBG orig, Main self)
        {
            //Call the original background
            orig(self);

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, 
                BlendState.AlphaBlend, 
                SamplerState.PointWrap, 
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);

            //Sort the list by their priority, so the higest priority one is in front
            foreach(var bg  in Backgrounds)
            {
 
                bg.Alpha += bg.IsActive() ? 0.01f : -0.01f;
                bg.Alpha = MathHelper.Clamp(bg.Alpha, 0, 1);
                if (bg.Alpha != 0)
                {
                    DrawBG(bg);
                }
            }

            spriteBatch.End();
            spriteBatch.Begin();
        }

        private void DrawBG(CustomBG bg)
        {
            _currentShader = null;
            for(int i = 0; i < bg.Layers.Count; i++)
            {
                DrawBGLayer(bg, bg.Layers[i], bg.Alpha);
            }
        }

        private void DrawBGLayer(CustomBG bg, CustomBGLayer bgLayer, float drawAlpha)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (bgLayer.BlendState != _currentBlendState || bgLayer.Shader != _currentShader)
            {
                Effect effect = bgLayer.Shader == null ? null : bgLayer.Shader.Effect;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred,
                    bgLayer.BlendState,
                    SamplerState.PointWrap,
                    DepthStencilState.None,
                    RasterizerState.CullCounterClockwise,
                    effect);
                _currentBlendState = bgLayer.BlendState;
                _currentShader = bgLayer.Shader;
                _currentShader?.ApplyToEffect();
            }
            Color drawColor = Main.ColorOfTheSkies * drawAlpha;
            int parallaxX = (int)(Main.screenPosition.X * bgLayer.Parallax * 0.75f);
            int width = (int)bgLayer.Texture.Size().X;
            int height = (int)bgLayer.Texture.Size().Y;

            int diffY = (int)((Main.worldSurface + 800) - Main.screenPosition.Y);
            int parallaxY = (int)(diffY * -0.1);

            Vector2 drawPosition = Vector2.Zero + bgLayer.DrawOffset + new Vector2(0, -parallaxY);
            drawPosition.Y += height / 2;
            int minY = -380;
            if (drawPosition.Y <= minY)
                drawPosition.Y = minY;
            drawPosition.Y -= 200;
            float drawScale = 2 * bg.DrawScale * bgLayer.DrawScale;
        
            spriteBatch.Draw(
                bgLayer.Texture.Value,
                drawPosition,
                new Rectangle(parallaxX, 0, width, height),
                drawColor,
                0f,
                default,
                scale: drawScale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
