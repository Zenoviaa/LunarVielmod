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
	public class FrileBroochA : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brooch of The Spragald");
			Tooltip.SetDefault("Simple Brooch!" +
				"\nEnemies now are frosted with icy glazes when hit!" +
				"\n Use the power of the Ice biome :P");

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

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CondensedDirt>(), 30);
			recipe.AddIngredient(ItemID.Silk, 5);
			recipe.AddIngredient(ItemID.IceBlock, 50);
			recipe.AddIngredient(ItemID.FrostDaggerfish, 10);
			recipe.AddIngredient(ItemID.IceBlade, 1);
			recipe.AddTile(ModContent.TileType<BroochesTable>());
			recipe.Register();
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<MyPlayer>().BroochFrile = true;
			player.GetModPlayer<MyPlayer>().FrileBCooldown--;
			player.GetModPlayer<MyPlayer>().FrileBDCooldown--;

		}




	}
}