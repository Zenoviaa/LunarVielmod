using Stellamod.Items;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    public class CustomRarityGlobalItem : GlobalItem
    { 
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            // If the item is of the rarity, and the line is the item name.
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                if (item.rare == ModContent.RarityType<DefaultSpecialRarity>())
                {
                    // Draw the custom tooltip line.    
                    DefaultSpecialRarity.DrawCustomTooltipLine(line);
                    return false;
                }
                if(item.rare == ModContent.RarityType<SirestiasSpecialRarity>())
                {
                    SirestiasSpecialRarity.DrawCustomTooltipLine(line);
                    return false;
                }
                if(item.rare == ModContent.RarityType<NiiviSpecialRarity>())
                {
                    NiiviSpecialRarity.DrawCustomTooltipLine(line);
                    return false;
                }

                if (item.rare == ModContent.RarityType<GothiviaSpecialRarity>())
                {
                    GothiviaSpecialRarity.DrawCustomTooltipLine(line);
                    return false;
                }
                if (item.rare == ModContent.RarityType<GoldenSpecialRarity>())
                {
                    GoldenSpecialRarity.DrawCustomTooltipLine(line);
                    return false;
                }
            }
            if(line.Name == "SirestiasTokenSwap" && item.ModItem is ClassSwapItem)
            {
                SirestiasSpecialRarity.DrawCustomTooltipLine(line);
                return false;
            } 
            else if(line.Name == "SirestiasTokenSwitched" && item.ModItem is ClassSwapItem)
            {
                SirestiasSwappedRarity.DrawCustomTooltipLine(line);
                return false;
            }
            return true;
        }
    }
}
