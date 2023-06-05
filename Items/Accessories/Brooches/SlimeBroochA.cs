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
	public class SlimeBroochA : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Brooch of The cute slimes");
			/* Tooltip.SetDefault("Simple Brooch!" +
				"\nJump Higher!" +
				"\nBouncy and fast fall" +
				"\nAwww so cute :P"); */

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			// Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
			var line = new TooltipLine(Mod, "", "");

			line = new TooltipLine(Mod, "Brooch of slimers", "Simple Brooch!")
			{
				OverrideColor = new Color(198, 124, 225)

			};
			tooltips.Add(line);



		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.value = Item.buyPrice(90);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;


		}


		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<MyPlayer>().BroochSlime = true;
			player.GetModPlayer<MyPlayer>().SlimeBCooldown--;
			player.frogLegJumpBoost = true;  // Increase ALL player damage by 100%
		}




	}
}