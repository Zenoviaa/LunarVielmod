using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Brooches;
using Stellamod.Buffs;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Flasks
{
    public class XixianFlask : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "xixian", "Use an insource to put something in the flask, then drink it! It acts like an infinite potion!")
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
            FlaskPlayer FlaskPlayer = player.GetModPlayer<FlaskPlayer>();

            //Check that this item is equipped

            //Check that you have advanced brooches since these don't work without
            if (!player.HasBuff<CannotUseFlask>())
            {
                //Give backglow to show that the effect is active
                DrawHelper.DrawAdvancedBroochGlow(Item, spriteBatch, position, new Color(198, 200, 124));
            }
            if (player.HasBuff<CannotUseFlask>() || player.HasBuff(BuffID.PotionSickness))
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
            FlaskPlayer FlaskPlayer = player.GetModPlayer<FlaskPlayer>();
            if (FlaskPlayer.hasHealthyInsource && !player.HasBuff<CannotUseFlask>())
            {
                player.statLife += 45;
                player.AddBuff(ModContent.BuffType<CannotUseFlask>(), 2400);
                player.AddBuff(BuffID.PotionSickness, 2400);
                return true;
            }

            if (FlaskPlayer.hasVitalityInsource && !player.HasBuff<CannotUseFlask>())
            {
                player.statLife += 102;
                player.statMana += 102;
                player.AddBuff(BuffID.Honey, 2400);
                player.AddBuff(BuffID.PotionSickness, 2400);
                player.AddBuff(ModContent.BuffType<CannotUseFlask>(), 2400);

                return true;
            }

            if (FlaskPlayer.hasFloweredInsource && !player.HasBuff<CannotUseFlask>())
            {

                player.AddBuff(BuffID.PotionSickness, 1200);
                player.AddBuff(ModContent.BuffType<CannotUseFlask>(), 1200);
                player.AddBuff(ModContent.BuffType<Friendzied>(), 300);
                return true;
            }

            if (FlaskPlayer.hasVialedInsource && !player.HasBuff<CannotUseFlask>())
            {

                player.AddBuff(BuffID.PotionSickness, 1200);
                player.AddBuff(ModContent.BuffType<CannotUseFlask>(), 1200);
                player.AddBuff(ModContent.BuffType<VialedUp>(), 600);
                return true;
            }


            if (FlaskPlayer.hasEpsidonInsource && !player.HasBuff<CannotUseFlask>())
            {
                player.statLife += 200;
                player.AddBuff(BuffID.Honey, 1200);
                player.AddBuff(BuffID.Swiftness, 1200);
                player.AddBuff(BuffID.Ironskin, 1200);
                player.AddBuff(BuffID.PotionSickness, 2400);
                player.AddBuff(ModContent.BuffType<CannotUseFlask>(), 2400);

                return true;
            }



            if (player.HasBuff<CannotUseFlask>() || player.HasBuff(BuffID.PotionSickness))
            {

                return false;
            }

                return true;
        }

       
    }
}