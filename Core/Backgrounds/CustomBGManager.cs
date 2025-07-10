using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
            for(int i = 0; i < bg.Layers.Length; i++)
            {
                DrawBGLayer(bg.Layers[i], bg.Alpha);
            }
        }

        private void DrawBGLayer(CustomBGLayer bgLayer, float drawAlpha)
        {
            Color drawColor = Main.ColorOfTheSkies * drawAlpha;
            int parallaxX = (int)(Main.screenPosition.X * bgLayer.Parallax);
         
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(
                bgLayer.Texture.Value,
                Vector2.Zero,
                new Rectangle(parallaxX, 0, (int)bgLayer.Texture.Size().X, (int)bgLayer.Texture.Size().Y),
                drawColor,
                0f,
                default,
                scale: 2,
                SpriteEffects.None,
                0f
            );
        }
    }
}
