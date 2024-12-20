using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic
{
    internal class SynergyTooltipItem : ModItem
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        public BaseElement PrimaryElement;
        public BaseEnchantment Enchantment;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Clear();
            TooltipLine tooltipLine;
            if (Enchantment != null && PrimaryElement != null)
            {
                ElementMatch match = ElementMatch.Neutral;
                match = PrimaryElement.GetMatch(Enchantment);
                switch (match)
                {
                    case ElementMatch.Neutral:
                        AddNoSynergyText(tooltips);
                        break;
                    case ElementMatch.Match:
                        tooltipLine = new TooltipLine(Mod, "SynergyHelp",
                                Language.GetTextValue("Mods.Stellamod.Enchantments.EnchantmentCommonSynergy", 4));
                        tooltips.Add(tooltipLine);
                        break;
                    case ElementMatch.Mismatch:
                        tooltipLine = new TooltipLine(Mod, "SynergyFail",
                                Language.GetTextValue("Mods.Stellamod.Enchantments.EnchantmentCommonFailSynergy", 4));
                        tooltips.Add(tooltipLine);
                        break;
                }
            }
            else
            {
                AddNoSynergyText(tooltips);
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
