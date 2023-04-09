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
	public class FlyingFishBroochA : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brooch of The Shielded Fish");
			Tooltip.SetDefault("Simple Brooch!" +
				"\nIncreases Speed and + 2 Defense!" +
				"\n Use the power of the speedy fish that fly???");

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
			player.GetModPlayer<MyPlayer>().BroochFlyfish = true;
			player.GetModPlayer<MyPlayer>().FlyfishBCooldown--;

		}




	}
}