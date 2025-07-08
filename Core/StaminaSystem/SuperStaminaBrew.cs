using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Helpers;
using Stellamod.Core.SwingSystem;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.StaminaSystem
{

    public class SuperStaminaBrew : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "stamina", "Drink up the stamina to restore some stamina. ")
            {
                OverrideColor = new Color(308, 71, 255)

            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(silver: 8);
            /*
            Item.buffType = ModContent.BuffType<Buffs.ExampleCrateBuff>(); // Specify an existing buff to be applied when used.
            Item.buffTime = 3 * 60 * 60;*/


        }

        public override bool? UseItem(Player player)
        {
            SwingPlayer potionPlayer = player.GetModPlayer<SwingPlayer>();
            potionPlayer.InfiniteStamina = !potionPlayer.InfiniteStamina;
            return true;
        }
    }
}