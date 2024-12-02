using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Trails;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Stellamod.Items.MoonlightMagic
{
    internal static class EnchantmentDrawHelper
    {
        public static Asset<Texture2D> GlowTextureOverride;
        public static void DrawTextShader(SpriteBatch spriteBatch,
            Item item, DrawableTooltipLine line, ref int yOffset,
            Color glowColor, Color primaryColor, Color noiseColor)
        {
            Vector2 textPosition = new Vector2(line.X, line.Y);
            //Draw BackGlow
            var glowTexture = TrailRegistry.GlowTrail;
            if (GlowTextureOverride != null)
                glowTexture = GlowTextureOverride;
            GlowTextureOverride = null;
            Vector2 scale = new Vector2(0.45f, 0.15f);
            glowColor.A = 0;
            glowColor *= 0.5f;
            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.AlphaBlend, default, default, default, default, Main.UIScaleMatrix);
            spriteBatch.Draw(glowTexture.Value, textPosition, null, glowColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);


            //Draw Flaming Text
            ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, line.Text, textPosition, line.Color, line.Rotation, line.Origin, line.BaseScale);
            var shader = FirePixelShader.Instance;
            shader.PrimaryColor = primaryColor;
            shader.NoiseColor = noiseColor;
            shader.Distortion = 0.0075f;
            shader.Speed = 10;
            shader.Power = 0.01f;
            shader.Apply();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, default, default, default, default, Main.UIScaleMatrix);

            shader.Data.Apply(null);
            ChatManager.DrawColorCodedString(Main.spriteBatch, line.Font, line.Text, textPosition, line.Color, line.Rotation, line.Origin, line.BaseScale);

            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
        }
    }
}
