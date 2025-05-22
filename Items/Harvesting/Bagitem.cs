using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
    internal class Bagitem : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dust Bag");
			/* Tooltip.SetDefault("A bag made for igniter dusts!" +
			"\nWhere does this come from?" +
			"\nUsed in every dust recipe, bring to the dust bench!"); */
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.sellPrice(silver: 20);
			Item.rare = ItemRarityID.Blue;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(ModContent.TileType<AlcaologyTable>());
			recipe.Register();
			recipe.AddIngredient(ItemID.Silk, 2);
		}
	}


}
