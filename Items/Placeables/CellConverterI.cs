using Terraria;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Tiles;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Materials;

namespace Stellamod.Items.Placeables
{
	public class CellConverterI : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Cell Converter");
		}


		public override void SetDefaults()
		{
			Item.width = Item.height = 16;
			Item.maxStack = 1;
			Item.rare = ItemRarityID.LightPurple;

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = Item.useAnimation = 25;

			Item.autoReuse = true;
			Item.consumable = true;


			Item.createTile = ModContent.TileType<CellConverter>();
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddRecipeGroup(RecipeGroupID.IronBar, 10);
			recipe.AddIngredient(ItemType<ArnchaliteBar>(), 10);
			recipe.AddIngredient(ItemType<UnknownCircuitry>(), 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
