
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials.Molds
{
    internal class MoldTooltipItem : ModItem
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        public Item MoldNeeded;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Clear();
            TooltipLine tooltipLine;
            if (MoldNeeded != null && !MoldNeeded.IsAir)
            {
                tooltipLine = new TooltipLine(Mod, "MoldNeeded",
                    Language.GetTextValue("Mods.Stellamod.Misc.MoldNeeded", MoldNeeded.Name));
                tooltipLine.OverrideColor = Color.Gray;
                tooltips.Add(tooltipLine);
            }
            else
            {
                AddNoMoldText(tooltips);
            }
        }

        private void AddNoMoldText(List<TooltipLine> tooltips)
        {
            var tooltipLine = new TooltipLine(Mod, "NoMoldNeeded",
             Language.GetTextValue("Mods.Stellamod.Misc.NoMoldNeeded"));
            tooltipLine.OverrideColor = Color.Gray;
            tooltips.Add(tooltipLine);
        }
    }
}
