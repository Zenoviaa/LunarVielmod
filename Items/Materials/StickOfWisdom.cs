using Stellamod.Items.Harvesting;
using Stellamod.Items.Ores;
using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
    internal class StickOfWisdom : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 50;
			Item.height = 52;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.sellPrice(silver: 5);
			Item.rare = ItemRarityID.LightRed;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Stick>(), 10);
			recipe.AddIngredient(ModContent.ItemType<EldritchSoul>(), 5);
			recipe.AddIngredient(ModContent.ItemType<GraftedSoul>(), 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
		}
	}
}
