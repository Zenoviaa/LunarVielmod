using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Utilities;

namespace Stellamod.Helpers
{
    internal static class ColorFunctions
    {
        public static Color AcidFlame => new Color(24, 142, 61);
        public static Color MiracleVoid => new Color(60, 0, 118);
        public static Color OrbWeaponType => new Color(0, Main.DiscoG, 150, 0f);

        public static Color SteinWeaponType => new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0f);
        public static Color JugglerWeaponType => new Color(Main.DiscoR, 0, Main.DiscoB, 0f);
        public static Color GreatswordWeaponType => new Color(Main.DiscoR, 150, 150, 0f);
        public static Color GunHolsterWeaponType => new Color(0, 150, Main.DiscoB, 0f);

        public static Color Niivin => new Color(72, 67, 200);
        public static Color MoonGreen => new Color(102, 222, 179);
        public static Color IceBlue => new Color(38, 237, 217);
        public static Color RadianceYellow => new Color(255, 207, 79);
        public static Color RoyalMagicPink => new Color(255, 112, 170);
        public static Color IceLightBlue => new Color(38, 204, 255);
        public static Color VeilPink => new Color(169, 101, 255);
        public static Color DreadRed => new Color(175, 24, 34);
        public static Color WindPurple => new Color(195, 158, 255);
        public static Color NaturalGreen => new Color(95, 106, 47);
        public static Color NextColor(this UnifiedRandom rand, params Color[] colors)
        {
            return colors[rand.Next(0, colors.Length)];
        }
    }
}
