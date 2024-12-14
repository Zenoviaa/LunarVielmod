using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Players;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Potions
{

    public class StaminaBrew : ModItem
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
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Player player = Main.player[Main.myPlayer];
            PotionPlayer PotionPlayer = player.GetModPlayer<PotionPlayer>();

            //Check that this item is equipped

            //Check that you have advanced brooches since these don't work without
            if (!player.HasBuff<CannotUseStaminaPotion>())
            {
                //Give backglow to show that the effect is active
                DrawHelper.DrawAdvancedShadingGlow(Item, spriteBatch, position, new Color(198, 200, 124));
            }
            if (player.HasBuff<CannotUseStaminaPotion>())
            {

                float sizeLimit = 28;
                //Draw the item icon but gray and transparent to show that the effect is not active
                Main.DrawItemIcon(spriteBatch, Item, position, Color.Gray * 0.8f, sizeLimit);
                return false;
            }
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            ComboPlayer PotionPlayer = player.GetModPlayer<ComboPlayer>();
            return !player.HasBuff<CannotUseStaminaPotion>();
        }

        public override bool? UseItem(Player player)
        {
            ComboPlayer potionPlayer = player.GetModPlayer<ComboPlayer>();
            potionPlayer.Stamina += 1;
            return true;
        }
    }
}