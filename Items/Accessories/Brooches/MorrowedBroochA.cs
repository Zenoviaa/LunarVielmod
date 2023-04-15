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
	public class MorrowedBroochA : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brooch of The Huntria Morrow");
			Tooltip.SetDefault("Simple Brooch!" +
				"\nHeavy increased damage to your arrows" +
				"\n +2 Defense and increased ranged damage" +
				"\n Use the power of the deep dark morrow..");

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
			player.GetModPlayer<MyPlayer>().BroochMorrow = true;
			player.GetModPlayer<MyPlayer>().MorrowBCooldown--;

		}




	}
}