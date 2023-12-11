using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Foods
{
    public class BowledCream : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "BowledC", "Put one on as an accessory, eat for temporary effects!")
            {
                OverrideColor = new Color(308, 71, 99)

            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(48, 34, BuffID.WellFed, 20600);
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.maxStack = 9999;
            Item.useStyle = ItemUseStyleID.DrinkLong;
            Item.value = Item.buyPrice(0, 0, 3, 40);
            Item.rare = ItemRarityID.Green;
            Item.consumable = true;
            Item.UseSound = SoundID.Item2;
            Item.accessory = true;
        }

        public override bool CanUseItem(Player player)
        {
            player.AddBuff(BuffID.Honey, 8000);
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //  player.statDefense += 2;
            player.moveSpeed += 0.4f;
        }
    }
}