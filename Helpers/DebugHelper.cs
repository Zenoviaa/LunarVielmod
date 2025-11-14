using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Helpers
{
    public static class DebugHelper
    {
        public static void NewTextOnlyInTesting(object o, Color? color = null)
        {
            if (Main.gameMenu)
                return;

            Main.NewText(o, color);
        }
    }
}
