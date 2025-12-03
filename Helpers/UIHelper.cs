using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;

namespace Stellamod.Helpers
{
    public static class UIHelper
    {
        /// <summary>
        /// Helper function for setting the mouse interface to true
        /// </summary>
        /// <param name="uiElement"></param>
        /// <returns></returns>
        public static Rectangle MouseInterfaceInteraction(UIElement uiElement)
        {
            Rectangle rectangle = uiElement.GetDimensions().ToRectangle();
            bool contains = uiElement.ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            return rectangle;
        }

        public static void QuickOutline(SpriteBatch spriteBatch, Asset<Texture2D> texture, Vector2 drawPosition, Color outlineColor, float scale = 1f)
        {
            QuickOutline(spriteBatch, texture.Value, drawPosition, outlineColor, scale);
        }

        public static void QuickOutline(SpriteBatch spriteBatch, Texture2D texture, Vector2 drawPosition, Color outlineColor, float scale = 1f)
        {
            var whiteShader = SpriteWhiteShader.Instance;
            float outlineOffset = 2;
            Vector2 h = Vector2.UnitX * outlineOffset;
            Vector2 v = Vector2.UnitY * outlineOffset;
            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            spriteBatch.Draw(texture, drawPosition + h, null, outlineColor, 0f, default, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition - h, null, outlineColor, 0f, default, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition + v, null, outlineColor, 0f, default, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition - v, null, outlineColor, 0f, default, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, default, Main.UIScaleMatrix);

           
        }
    }
}
