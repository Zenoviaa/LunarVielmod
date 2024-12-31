using Stellamod.Items.Harvesting;
using Stellamod.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable
{
    public class AlcaologyI : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Alcaology Station");
			// Tooltip.SetDefault("This table is used for dusts and useful Materials!");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<AlcaologyTable>());
			Item.value = 150;
			Item.maxStack = 20;
			Item.width = 38;
			Item.height = 24;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.WorkBench, 1);
			recipe.AddIngredient(ItemID.Solidifier, 1);
			recipe.AddIngredient(ItemID.Furnace, 1);
			recipe.AddIngredient(ModContent.ItemType<Cinderscrap>(), 30);
			recipe.AddIngredient(ModContent.ItemType<FlowerBatch>(), 2);
			recipe.AddIngredient(ModContent.ItemType<AlcadizMetal>(), 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}