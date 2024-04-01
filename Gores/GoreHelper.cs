using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Gores
{
    internal class GoreHelper
    {
        public static int TypeFallingLeafWhite => ModContent.Find<ModGore>("Stellamod/FallingLeafWhite").Type;
        public static int TypeFallingLeafRed => ModContent.Find<ModGore>("Stellamod/FallingLeafRed").Type;
        public static int TypeFallingIllurianVine => ModContent.Find<ModGore>("Stellamod/FallingIllurianVine").Type;
        public static int TypeSplashBlack => ModContent.Find<ModGore>("Stellamod/SplashBlack").Type;
        public static int TypeSplashBlue => ModContent.Find<ModGore>("Stellamod/SplashBlue").Type;
        public static int TypeSplashGreen => ModContent.Find<ModGore>("Stellamod/SplashGreen").Type;
        public static int TypeSplashOrange => ModContent.Find<ModGore>("Stellamod/SplashOrange").Type;
        public static int TypeSplashRed => ModContent.Find<ModGore>("Stellamod/SplashRed").Type;
        public static int TypeSplashYellow => ModContent.Find<ModGore>("Stellamod/SplashYellow").Type;
    }
}
