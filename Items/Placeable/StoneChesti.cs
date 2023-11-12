using Stellamod.Tiles.Structures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable
{
    public class StoneChesti : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Gilded Chest");
			// Tooltip.SetDefault("A chest dedicated to the feral warriors of the morrow");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 22;
			Item.maxStack = 99;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.value = 500;
			Item.createTile = ModContent.TileType<StoneChest>();
		 Item.placeStyle = 1; // Use this to place the chest in its locked style
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		
	}
}