using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Particles.Sparkles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Stellamod.Helpers
{
    internal class RarityHelper
    {
        public static void DrawBaseTooltipTextAndGlow(DrawableTooltipLine tooltipLine, Color glowColor, Color textOuterColor, Color? textInnerColor = null, Texture2D glowTexture = null, Vector2? glowScaleOffset = null)
        {
            textInnerColor ??= Color.Black;
            glowScaleOffset ??= Vector2.One;
            // Get the text of the tooltip line.
            string text = tooltipLine.Text;
            // Get the size of the text in its font.
            Vector2 textSize = tooltipLine.Font.MeasureString(text);
            // Get the center of the text.
            Vector2 textCenter = textSize * 0.5f;
            // The position to draw the text.
            Vector2 textPosition = new(tooltipLine.X, tooltipLine.Y);
            // Get the position to draw the glow behind the text.
            Vector2 glowPosition = new(tooltipLine.X + textCenter.X, tooltipLine.Y + textCenter.Y / 1.5f);
            // Get the scale of the glow texture based off of the text size.
            Vector2 glowScale = new Vector2(textSize.X * 0.115f, 0.6f) * glowScaleOffset.Value;
            glowColor.A = 0;

            // Draw the glow texture.
            if(glowTexture != null)
            {
                Main.spriteBatch.Draw(glowTexture, glowPosition, null, glowColor * 0.85f, 0f, glowTexture.Size() * 0.5f, glowScale, SpriteEffects.None, 0f);
            }
       
            // Get an offset to the afterimageOffset based on a sine wave.
            float sine = (float)((1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2.5f)) / 2);
            float sineOffset = MathHelper.Lerp(0.5f, 1f, sine);

            // Draw text backglow effects.
            for (int i = 0; i < 12; i++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * i / 12f).ToRotationVector2() * (2f * sineOffset);
                // Draw the text. Rotate the position based on i.
                ChatManager.DrawColorCodedString(Main.spriteBatch, tooltipLine.Font, text, (textPosition + afterimageOffset).RotatedBy(MathHelper.TwoPi * (i / 12)), textOuterColor * 0.9f, tooltipLine.Rotation, tooltipLine.Origin, tooltipLine.BaseScale);
            }

            // Draw the main inner text.
            Color mainTextColor = Color.Lerp(glowColor, textInnerColor.Value, 0.9f);
            ChatManager.DrawColorCodedString(Main.spriteBatch, tooltipLine.Font, text, textPosition, mainTextColor, tooltipLine.Rotation, tooltipLine.Origin, tooltipLine.BaseScale);
        }

        public static void SpawnAndUpdateTooltipParticles(DrawableTooltipLine tooltipLine, ref List<RaritySparkle> sparklesList, int spawnChance, SparkleType sparkleType)
        {
            Vector2 textSize = tooltipLine.Font.MeasureString(tooltipLine.Text);

            // Randomly spawn sparkles.
            if (Main.rand.NextBool(spawnChance))
            {
                int lifetime;
                float scale;
                Vector2 position;
                Vector2 velocity;

                switch (sparkleType)
                {
                    case SparkleType.DefaultSparkle:
                        lifetime = (int)Main.rand.NextFloat(70f - 25f, 70f);
                        scale = Main.rand.NextFloat(0.3f * 0.5f, 0.3f);
                        position = Main.rand.NextVector2FromRectangle(new(-(int)(textSize.X * 0.5f), -(int)(textSize.Y * 0.4f), (int)textSize.X, (int)(textSize.Y * 0.35f)));
                        velocity = Vector2.UnitY * Main.rand.NextFloat(0.1f, 0.25f);
                        sparklesList.Add(new DefaultSparkle(lifetime, scale, 0f, 0f, position, velocity));
                        break;

                    case SparkleType.MagicCircle:
                        lifetime = (int)Main.rand.NextFloat(70f - 25f, 70f);
                        scale = Main.rand.NextFloat(0.03f * 0.5f, 0.03f);
                        position = Main.rand.NextVector2FromRectangle(new(-(int)(textSize.X * 0.5f), -(int)(textSize.Y * 0.4f), (int)textSize.X, (int)(textSize.Y * 0.35f)));
                        velocity = Vector2.UnitY * Main.rand.NextFloat(0.1f, 0.25f);
                        sparklesList.Add(new CircleSparkle(lifetime, scale, 0f, 0f, position, velocity));
                        break;
                }
            }

            // Update any active sparkles.
            for (int i = 0; i < sparklesList.Count; i++)
                sparklesList[i].Update();

            // Remove any sparkles that have existed long enough.
            sparklesList.RemoveAll((RaritySparkle s) => s.Time >= s.Lifetime);

            // Draw the sparkles.
            foreach (RaritySparkle sparkle in sparklesList)
                sparkle.Draw(Main.spriteBatch, new Vector2(tooltipLine.X, tooltipLine.Y) + textSize * 0.5f + sparkle.Position);
        }
    }
}
