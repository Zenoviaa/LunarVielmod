
using Stellamod.Items.Harvesting;
using Stellamod.Tiles;
using Stellamod.Tiles.Furniture;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable
{
	public class WeddingDay : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Alcaology Station");
			// Tooltip.SetDefault("This table is used for dusts and useful Materials!");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<GothivPainting>());
			Item.value = 150;
			Item.maxStack = 999;
			Item.width = 38;
			Item.height = 24;
		}

		
	}
}