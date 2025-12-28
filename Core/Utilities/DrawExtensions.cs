using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Core.Utilities
{
    public static class DrawExtensions
    {
        public delegate void SpriteDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor);
        public static void DrawOutline(SpriteDraw drawFunction, SpriteBatch spriteBatch, Vector2 screenPos, Color outlineColor)
        {
            float outlineOffset = 2;
            Vector2 v = Vector2.UnitX * outlineOffset;
            Vector2 h = Vector2.UnitY * outlineOffset;
            drawFunction(spriteBatch, screenPos + v, outlineColor);
            drawFunction(spriteBatch, screenPos - v, outlineColor);
            drawFunction(spriteBatch, screenPos + h, outlineColor);
            drawFunction(spriteBatch, screenPos - h, outlineColor);
        }
    }
}
