
using Stellamod.Tiles.Abyss.Aurelus;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable
{
    public class AurelusDoor : ModItem
	{
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<AurelusDoorClosed>());
			Item.width = 14;
			Item.height = 28;
			Item.value = 150;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

	}
}