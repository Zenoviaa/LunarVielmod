using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
	internal class AlcadizMetal : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Alcadiz metal");
			/* Tooltip.SetDefault("Woa this is super hard and magical.." +
			"\nWhere does this ore come from?" +
			"\nBest use for metalic weapons and magical items!"); */
		}
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 999;
			Item.value = Item.sellPrice(silver: 20);
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FrileOre>(), 1);
			recipe.AddIngredient(ItemID.DemoniteBar, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();

			Recipe recipe3 = CreateRecipe();
			recipe3.AddIngredient(ModContent.ItemType<FrileOre>(), 1);
			recipe3.AddIngredient(ItemID.CrimtaneBar, 1);
			recipe3.AddTile(TileID.Anvils);
			recipe3.Register();


			Recipe recipe2 = CreateRecipe();
			recipe2.AddIngredient(ModContent.ItemType<AlcadizScrap>(), 3);
			recipe2.AddTile(TileID.Furnaces);
			recipe2.Register();
		}
	}


}
