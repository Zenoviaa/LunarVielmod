using Microsoft.Xna.Framework;

using Stellamod.Common.Bases;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    [AutoloadEquip(EquipType.Waist)]
    public class RadiatedBroochesBackpack : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Advanced Brooch Knapsack");
            /* Tooltip.SetDefault("Increased Regeneration!" +
				"\n +10% damage" +
				"\n Allows you to equip advanced brooches! (Very useful :P)" +
				"\n Allows the effects of the Hiker's Backpack! "); */

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");


            line = new TooltipLine(Mod, "RADBBP", "S+ Accessory!")
            {
                OverrideColor = new Color(220, 87, 24)

            };
            tooltips.Add(line);




        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 25);
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            BroochSpawnerPlayer broochSpawnerPlayer = player.GetModPlayer<BroochSpawnerPlayer>();
            broochSpawnerPlayer.hasAdvancedBrooches = true;
            broochSpawnerPlayer.hasRadiantBrooches = true;
            player.GetModPlayer<MyPlayer>().HikersBSpawn = true;
            player.GetDamage(DamageClass.Generic) *= 1.08f; // Increase ALL player damage by 100%
        }
    }
}