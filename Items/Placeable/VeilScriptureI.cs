
using Stellamod.Tiles;
using Stellamod.Tiles.Abyss.Aurelus;
using Stellamod.Tiles.Catacombs;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable
{
	public class VeilScriptureI : ModItem
	{
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<VeilScripture>());
			Item.width = 14;
			Item.height = 28;
			Item.value = 150;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

	}
}