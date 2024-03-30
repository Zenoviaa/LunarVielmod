using Terraria.ModLoader;

namespace Stellamod.Gores
{
    internal class GoreHelper
    {
        public static int TypeFallingLeafWhite => ModContent.Find<ModGore>("Stellamod/FallingLeafWhite").Type;
        public static int TypeFallingLeafRed => ModContent.Find<ModGore>("Stellamod/FallingLeafRed").Type;
        public static int TypeFallingIllurianVine => ModContent.Find<ModGore>("Stellamod/FallingIllurianVine").Type;
    }
}
