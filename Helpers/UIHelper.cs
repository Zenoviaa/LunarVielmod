using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Shaders;
using Stellamod.UI;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;

namespace Stellamod.Helpers
{
    public static class UIHelper
    {
        public const int width = 480;
        public const int height = 155;

        public static int BookLeftPageX => Main.screenWidth / 2 - width / 2 - 64;
        public static int BookLeftPageY => Main.screenHeight / 2 - height / 2 - 196;
 

        public static float GetTotalPanelHeight(this UIPanel panel)
        {


            var rect = panel.GetInnerDimensions().ToRectangle();
            float top = rect.Y;
            float lowestTop = rect.Y + rect.Height;
            foreach (var child in panel.Children)
            {
                var dimensions = child.GetInnerDimensions().ToRectangle();
                float bottom = dimensions.Y + dimensions.Height;
                if(bottom > lowestTop)
                {
                    lowestTop = bottom;
                }
            }

            return lowestTop - top;

        }
        public static float GetTotalPanelHeight(this UIPanel panel, float startingHeight)
        {


            var rect = panel.GetInnerDimensions().ToRectangle();
            float top = rect.Y;
            float lowestTop = rect.Y + startingHeight;
            foreach (var child in panel.Children)
            {
                var dimensions = child.GetInnerDimensions().ToRectangle();
                float bottom = dimensions.Y + dimensions.Height;
                if (bottom > lowestTop)
                {
                    lowestTop = bottom;
                }
            }

            return lowestTop - top;

        }

        public static void SizePanelandScrollbar(FancyScrollbar scrollbar, UIPanel panel, float height, float totalHeight)
        {

            panel.Height.Pixels = totalHeight + 32;
            float progress = panel.Height.Pixels / height;
            progress = MathHelper.Clamp(progress, 0f, 1f);
            scrollbar.Height.Set(height * progress, 0);
            //Hacky way to get invisible scrollbar when there's no need for it
            if (panel.Height.Pixels < height)
            {
                scrollbar.Top.Set(500000, 0f);
            }
            else
            {
                scrollbar.Top.Set(0, 0f);
            }
        }
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
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, anisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            spriteBatch.Draw(texture, drawPosition + h, null, outlineColor, 0f, default, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition - h, null, outlineColor, 0f, default, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition + v, null, outlineColor, 0f, default, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition - v, null, outlineColor, 0f, default, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, default, Main.UIScaleMatrix);


        }
        public static void QuickOutline(SpriteBatch spriteBatch, Texture2D texture, Rectangle frame, Vector2 drawPosition, Color outlineColor, float scale = 1f)
        {
            var whiteShader = SpriteWhiteShader.Instance;
            float outlineOffset = 2;
            Vector2 h = Vector2.UnitX * outlineOffset;
            Vector2 v = Vector2.UnitY * outlineOffset;
            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, anisotropicClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            spriteBatch.Draw(texture, drawPosition + h, frame, outlineColor, 0f, default, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition - h, frame, outlineColor, 0f, default, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition + v, frame, outlineColor, 0f, default, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition - v, frame, outlineColor, 0f, default, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, default, Main.UIScaleMatrix);


        }
    }
}
