using Stellamod.Tiles.RoyalCapital;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable.Alcadzia
{
    public class AowerTopI : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("CurtainLeft");
			// Tooltip.SetDefault("Curtain");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<AowerTop>());
			Item.value = 150;
			Item.maxStack = 20;
			Item.width = 38;
			Item.height = 24;
		}
	}
}