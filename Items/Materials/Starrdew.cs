using Stellamod.Items.Harvesting;
using Stellamod.Items.Ores;
using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
	internal class Starrdew : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Starr Dew");
			Tooltip.SetDefault("Ew! Its sticky! I wonder what else is sticky..." +
			"\nA sticky substance that resonates with the stars and the morrow!");
		}
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 999;
			Item.value = Item.sellPrice(silver: 5);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(4);
			recipe.AddIngredient(ModContent.ItemType<CondensedDirt>(), 3);
			recipe.AddIngredient(ModContent.ItemType<FrileOre>(), 1);
			recipe.AddIngredient(ModContent.ItemType<VerianOre>(), 1);
			recipe.AddIngredient(ItemID.FallenStar, 1);
			recipe.AddIngredient(ItemID.BottledWater, 1);
			recipe.AddTile(ModContent.TileType<AlcaologyTable>());
			recipe.Register();
		}
	}
}
