using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Stellamod.Gores
{
    internal class GoreHelper
    {
        public static int TypeFallingLeafWhite => ModContent.Find<ModGore>("Stellamod/FallingLeafWhite").Type;
        public static int TypeFallingLeafRed => ModContent.Find<ModGore>("Stellamod/FallingLeafRed").Type;
    }
}
