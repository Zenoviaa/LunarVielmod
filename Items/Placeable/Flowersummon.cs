using Stellamod.Items.Harvesting;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable
{
	public class Flowersummon : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Morrowed Flower Batch");
			Tooltip.SetDefault("Do not right click or disturb the evil");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.FlowerSummon>());
			Item.value = 150;
			Item.maxStack = 20;
			Item.width = 38;
			Item.height = 24;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.Anvils);
			recipe.AddIngredient(ItemID.HellstoneBar, 22);
			recipe.AddIngredient(ModContent.ItemType<VerianBar>(), 20);
			recipe.AddIngredient(ModContent.ItemType<FlowerBatch>(), 5);
			recipe.AddIngredient(ModContent.ItemType<DreasFlower>(), 5);
			recipe.AddIngredient(ModContent.ItemType<Stick>(), 10);
			recipe.AddIngredient(ModContent.ItemType<Fabric>(), 35);
			recipe.AddIngredient(ModContent.ItemType<MorrowRocks>(), 50);
			recipe.AddIngredient(ItemID.Bone, 20);

			recipe.Register();
		}
	}
}