using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic
{
    public class SlotTooltipItem : ModItem
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        public bool isTimedSlot;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Clear();
            TooltipLine tooltipLine;

            if (isTimedSlot)
            {
                tooltipLine = new TooltipLine(Mod, "EnchantmentTimedHelp",
                    Language.GetTextValue("Mods.Stellamod.Enchantments.EnchantmentTimerSlotHelp"));
                tooltipLine.OverrideColor = Color.White;
                tooltips.Add(tooltipLine);
            }
            else
            {
                tooltipLine = new TooltipLine(Mod, "EnchantmentHelp",
                    Language.GetTextValue("Mods.Stellamod.Enchantments.EnchantmentNormalSlotHelp"));
                tooltipLine.OverrideColor = Color.White;
                tooltips.Add(tooltipLine);
            }


        }

        private void AddNoSynergyText(List<TooltipLine> tooltips)
        {
            var tooltipLine = new TooltipLine(Mod, "NoSynergyHelp",
             Language.GetTextValue("Mods.Stellamod.Enchantments.EnchantmentCommonNoSynergy"));
            tooltipLine.OverrideColor = Color.Gray;
            tooltips.Add(tooltipLine);
        }
    }
}
