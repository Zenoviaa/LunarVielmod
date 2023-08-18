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
	public class GovheilHolsterBroochA : ModItem
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
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;


		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<MyPlayer>().BroochGovheill = true;
			player.GetModPlayer<MyPlayer>().GovheillBCooldown--;

		}




	}
}