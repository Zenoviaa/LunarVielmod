using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Core.XixianFlaskSystem
{
    public static class InsourceHelper
    {
        public static TooltipLine AddCooldownLine(Mod mod, List<TooltipLine> lines, int addedTime)
        {
            float ticks = addedTime;
            float seconds = ticks / 60;
            string secondsString = seconds.ToString("#.#");
            TooltipLine line = new TooltipLine(mod, "AmountOfInsourceTime",
                LangText.Common("InsourceTime", secondsString));
            line.OverrideColor = Color.Lerp(new Color(80, 187, 180), Color.Black, 0.25f);
            lines.Add(line);
            return line;
        }
    }
}
