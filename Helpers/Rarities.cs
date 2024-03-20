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
}
