using Stellamod.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable
{
    public class BroochesTableI : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Brooches Jewelry Station");
			// Tooltip.SetDefault("This bench allows you to craft exquisite brooches and combine them! ");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<BroochesTable>());
			Item.value = 150;
			Item.maxStack = 20;
			Item.width = 38;
			Item.height = 24;
		}

		
	}
}