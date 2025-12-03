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

namespace Stellamod.Helpers
{
    public static class UIHelper
    {
        public static void QuickOutline(SpriteBatch spriteBatch, Asset<Texture2D> texture, Vector2 drawPosition, Color outlineColor)
        {
            QuickOutline(spriteBatch, texture.Value, drawPosition, outlineColor);
        }

        public static void QuickOutline(SpriteBatch spriteBatch, Texture2D texture, Vector2 drawPosition, Color outlineColor)
        {
            var whiteShader = SpriteWhiteShader.Instance;
            float outlineOffset = 2;
            Vector2 h = Vector2.UnitX * outlineOffset;
            Vector2 v = Vector2.UnitY * outlineOffset;
            spriteBatch.Restart(effect: whiteShader.Effect);


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, whiteShader.Effect, Main.UIScaleMatrix);

            spriteBatch.Draw(texture, drawPosition + h, null, outlineColor, 0f, default, 1, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition - h, null, outlineColor, 0f, default, 1, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition + v, null, outlineColor, 0f, default, 1, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition - v, null, outlineColor, 0f, default, 1, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
        }
    }
}
