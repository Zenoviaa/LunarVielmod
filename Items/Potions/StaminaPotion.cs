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
    internal class PotionPlayer : ModPlayer
    {
        public bool hasHealthyInsource;
        public bool hasVitalityInsource;
        public bool hasTime;




    }

    public class CannotUseStaminaPotion : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Charm Buff!");
            // Description.SetDefault("A true warrior such as yourself knows no bounds");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            PotionPlayer PotionPlayer = player.GetModPlayer<PotionPlayer>();
            PotionPlayer.hasTime = true;


        }
    }
    public class StaminaPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "stamina", "Drink up the stamina to restore your stamina on a time! It's an infinite potion!")
            {
                OverrideColor = new Color(308, 71, 255)

            };
            tooltips.Add(line);
        }

        public override void SetDefaults()
        {

            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.maxStack = 1;
            Item.useStyle = ItemUseStyleID.DrinkLong;
            Item.value = Item.buyPrice(0, 3, 3, 40);
            Item.rare = ItemRarityID.Green;
            Item.consumable = false;
            Item.UseSound = SoundID.Item2;


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
            if (player.HasBuff<CannotUseStaminaPotion>() || player.HasBuff(BuffID.PotionSickness))
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
            if (!player.HasBuff<CannotUseStaminaPotion>())
            {
                player.statLife += 35;
                PotionPlayer.Stamina = PotionPlayer.MaxStamina;
                player.AddBuff(ModContent.BuffType<CannotUseStaminaPotion>(), 2400);
                player.AddBuff(BuffID.PotionSickness, 2400);
                return true;
            }


            if (player.HasBuff<CannotUseStaminaPotion>() || player.HasBuff(BuffID.PotionSickness))
            {

                return false;
            }

            return true;
        }


    }
}