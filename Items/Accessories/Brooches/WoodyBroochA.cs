﻿using Stellamod.Brooches;
using Stellamod.Buffs.Charms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;

namespace Stellamod.Items.Accessories.Brooches
{
    internal class WoodyBroochA : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Brooch of the Tale of Diari");
            /* Tooltip.SetDefault("Simple Brooch!" +
				"\n+ 4 Defense!" +
				"\nAuto swing capabilities!" +
				"\nFlame walking? Always Fed!" +
				"\n+40 Health and Mana"); */

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");

            line = new TooltipLine(Mod, "Brooch of the TaGo", "Advanced Brooch!")
            {
                OverrideColor = new Color(254, 128, 10)

            };
            tooltips.Add(line);

            line = new TooltipLine(Mod, "Brooch of the TaGo", "You need an Advanced Brooches Backpack for this!")
            {
                OverrideColor = new Color(198, 124, 225)

            };
            tooltips.Add(line);

        }


        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(0, 0, 90);
            Item.rare = ItemRarityID.LightPurple;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            BroochPlayer broochPlayer = player.GetModPlayer<BroochPlayer>();
            broochPlayer.hasWoodyBrooch = true;
        }


        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Player player = Main.player[Main.myPlayer];
            BroochPlayer broochPlayer = player.GetModPlayer<BroochPlayer>();

            //Check that this item is equipped
            if (player.HasItemEquipped(Item))
            {
                //Check that you have advanced brooches since these don't work without
                if (broochPlayer.hasAdvancedBrooches)
                {
                    //Give backglow to show that the effect is active
                    DrawHelper.DrawAdvancedBroochGlow(Item, spriteBatch, position, new Color(198, 124, 225));
                }
                else
                {
                    float sizeLimit = 28;
                    //Draw the item icon but gray and transparent to show that the effect is not active
                    Main.DrawItemIcon(spriteBatch, Item, position, Color.Gray * 0.8f, sizeLimit);
                    return false;
                }
            }

            return true;
        }
    }
}
