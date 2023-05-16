
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Stellamod.Items.Harvesting;

namespace Stellamod.Items.Materials
{
	internal class RippedFabric : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Ripped Fabric");
			/* Tooltip.SetDefault("From the knowledge of the Gilded to robes on enemies, wonderous!" +
				   "\nA very magical material although.." +
			"\nI feel like the item calls my name..."); */
		}
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 999;
			Item.value = Item.sellPrice(silver: 2);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(4);
			recipe.AddIngredient(ModContent.ItemType<Fabric>(), 1);
			recipe.AddIngredient(ModContent.ItemType<Cinderscrap>(), 3);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}
