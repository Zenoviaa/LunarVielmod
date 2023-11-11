using Stellamod.Tiles.RoyalCapital;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable.Alcadzia
{
    public class SkullpileI : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("CurtainLeft");
			// Tooltip.SetDefault("Curtain");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Skullpile>());
			Item.value = 150;
			Item.maxStack = 20;
			Item.width = 38;
			Item.height = 24;
		}
	}
}