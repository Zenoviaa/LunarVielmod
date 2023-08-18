using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Stellamod;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Ores;
using Stellamod.Tiles;
using System.Collections.Generic;

namespace Stellamod.Items.Accessories.Brooches
{
	public class DiariBroochA : ModItem
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

			line = new TooltipLine(Mod, "Brooch of the Tale of Diari", "Simple Brooch!")
			{
				OverrideColor = new Color(198, 124, 225)

			};
			tooltips.Add(line);

			line = new TooltipLine(Mod, "Brooch of the Tale of Diari", "Love you and have fun -Sirestias")
            {
                OverrideColor = new Color(220, 87, 24)

            };
            tooltips.Add(line);

            line = new TooltipLine(Mod, "Brooch of the Tale of Diari", "Please check out my game Diari!")
            {
                OverrideColor = new Color(244, 202, 59)

            };
            tooltips.Add(line);

            line = new TooltipLine(Mod, "Brooch of the Tale of Diari", "This mod is in relation to the game")
            {
                OverrideColor = new Color(244, 202, 59)

            };
            tooltips.Add(line);


        }
        public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(0, 0, 90);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;


		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<MyPlayer>().BroochDiari = true;
			player.GetModPlayer<MyPlayer>().DiariBCooldown--;

		}




	}
}