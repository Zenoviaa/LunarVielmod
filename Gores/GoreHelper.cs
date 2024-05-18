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
        public static int TypePaper => ModContent.Find<ModGore>("Stellamod/Paper").Type;
        public static int TypeRibbonRed => ModContent.GoreType<RibbonRed>();


        public static int Niivi1 => ModContent.Find<ModGore>("Stellamod/Niivi1").Type;
        public static int Niivi2 => ModContent.Find<ModGore>("Stellamod/Niivi2").Type;
        public static int Niivi3 => ModContent.Find<ModGore>("Stellamod/Niivi3").Type;
        public static int Niivi4 => ModContent.Find<ModGore>("Stellamod/Niivi4").Type;
        public static int Niivi5 => ModContent.Find<ModGore>("Stellamod/Niivi5").Type;
        public static int Niivi6 => ModContent.Find<ModGore>("Stellamod/Niivi6").Type;
        public static int Niivi7 => ModContent.Find<ModGore>("Stellamod/Niivi7").Type;
    }
}
