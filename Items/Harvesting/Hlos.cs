using Stellamod.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
	internal class Hlos : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Magical Handle");
			/* Tooltip.SetDefault("Magical Handle omg!" +
			"\nBest use for arcanal weapons"); */

		
			Item.width = 20;
			Item.height = 20;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(silver: 20);
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
		}


		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 1);
			recipe.AddIngredient(ItemID.IronBar, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();


			Recipe recipe2 = CreateRecipe();
			recipe2.AddIngredient(ModContent.ItemType<FrileBar>(), 1);
			recipe2.AddIngredient(ItemID.LeadBar, 1);
			recipe2.AddTile(TileID.Anvils);
			recipe2.Register();
		}
	}
}
