using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Stellamod;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Ores;
using Stellamod.Tiles;

namespace Stellamod.Items.Accessories.Brooches
{
	public class SlimeBroochA : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brooch of The cute slimes");
			Tooltip.SetDefault("Simple Brooch!" +
				"\nJump Higher!" +
				"\nBouncy and fast fall" +
				"\nAwww so cute :P");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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