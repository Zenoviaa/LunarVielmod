using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    public class DefaultSpecialRarity : ModRarity
    {
        public override Color RarityColor => Color.Cyan;

        internal static List<RaritySparkle> SparkleList = new();

        public static void DrawCustomTooltipLine(DrawableTooltipLine tooltipLine)
        {
            // Draw the base tooltip text and glow.
            RarityHelper.DrawBaseTooltipTextAndGlow(tooltipLine, Color.Cyan, Color.Lerp(Color.DeepSkyBlue, Color.LightBlue, VectorHelper.Osc(0, 1, 2)), new Color(12, 26, 47));

            // Draw base sparkles.
            RarityHelper.SpawnAndUpdateTooltipParticles(tooltipLine, ref SparkleList, 7, SparkleType.DefaultSparkle);
        }
    }

    public class SirestiasSpecialRarity : ModRarity
    {
        public override Color RarityColor => Color.White;

        internal static List<RaritySparkle> SparkleList = new();

        public static void DrawCustomTooltipLine(DrawableTooltipLine tooltipLine)
        {
            // Draw the base tooltip text and glow.
            RarityHelper.DrawBaseTooltipTextAndGlow(tooltipLine, Color.Black, Color.Lerp(Color.White, Color.Black, VectorHelper.Osc(0, 1, 2)), new Color(12, 26, 47), RarityTextureRegistry.ThornedRarityGlow);

            // Draw base sparkles.
            RarityHelper.SpawnAndUpdateTooltipParticles(tooltipLine, ref SparkleList, 7, SparkleType.MagicCircle);
        }
    }

    public class SirestiasSwappedRarity : ModRarity
    {
        public override Color RarityColor => Color.White;

        internal static List<RaritySparkle> SparkleList = new();

        public static void DrawCustomTooltipLine(DrawableTooltipLine tooltipLine)
        {
            // Draw the base tooltip text and glow.
            RarityHelper.DrawBaseTooltipTextAndGlow(tooltipLine, 
                Color.DarkGoldenrod, 
                Color.Lerp(Color.DarkGoldenrod, Color.LightGoldenrodYellow, VectorHelper.Osc(0, 1, 2)), new Color(12, 26, 47), RarityTextureRegistry.ThornedRarityGlow, glowScaleOffset: new Vector2(0.75f, 0.5f));

            // Draw base sparkles.
            RarityHelper.SpawnAndUpdateTooltipParticles(tooltipLine, ref SparkleList, 7, SparkleType.DefaultSparkle);
        }
    }

    public class NiiviSpecialRarity : ModRarity
    {
        public override Color RarityColor => Color.White;

        internal static List<RaritySparkle> SparkleList = new();

        public static void DrawCustomTooltipLine(DrawableTooltipLine tooltipLine)
        {
            // Draw the base tooltip text and glow.
            RarityHelper.DrawBaseTooltipTextAndGlow(tooltipLine,
                Color.DarkSlateBlue,
                Color.Lerp(Color.LightSkyBlue, Color.DarkSlateBlue, VectorHelper.Osc(0, 1, 2)), new Color(12, 26, 47), RarityTextureRegistry.ThornedRarityGlow, glowScaleOffset: new Vector2(0.75f, 0.5f));

            // Draw base sparkles.
            RarityHelper.SpawnAndUpdateTooltipParticles(tooltipLine, ref SparkleList, 7, SparkleType.DefaultSparkle);
        }
    }

    public class GothiviaSpecialRarity : ModRarity
    {
        public override Color RarityColor => Color.White;

        internal static List<RaritySparkle> SparkleList = new();

        public static void DrawCustomTooltipLine(DrawableTooltipLine tooltipLine)
        {
            // Draw the base tooltip text and glow.
            RarityHelper.DrawBaseTooltipTextAndGlow(tooltipLine,
                Color.BlueViolet,
                Color.Lerp(Color.MediumVioletRed, Color.SpringGreen, VectorHelper.Osc(0, 1, 2)), new Color(52, 56, 57), RarityTextureRegistry.ThornedRarityGlow, glowScaleOffset: new Vector2(0.75f, 0.5f));

            // Draw base sparkles.
            RarityHelper.SpawnAndUpdateTooltipParticles(tooltipLine, ref SparkleList, 7, SparkleType.DefaultSparkle);
        }
    }

    public class GoldenSpecialRarity : ModRarity
    {
        public override Color RarityColor => Color.White;

        internal static List<RaritySparkle> SparkleList = new();

        public static void DrawCustomTooltipLine(DrawableTooltipLine tooltipLine)
        {
            // Draw the base tooltip text and glow.
            RarityHelper.DrawBaseTooltipTextAndGlow(tooltipLine,
                Color.DarkGoldenrod,
                Color.Lerp(Color.DarkGoldenrod, Color.Goldenrod, VectorHelper.Osc(0, 1, 2)), new Color(12, 26, 47), glowScaleOffset: new Vector2(0.75f, 0.5f));

            // Draw base sparkles.
            RarityHelper.SpawnAndUpdateTooltipParticles(tooltipLine, ref SparkleList, 7, SparkleType.DefaultSparkle);
        }
    }
}
