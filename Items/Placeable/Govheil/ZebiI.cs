using Stellamod.Tiles.Structures.AlcadizNGovheil;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable.Govheil
{
    public class ZebiI : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("CurtainLeft");
			// Tooltip.SetDefault("Curtain");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Zebi>());
			Item.value = 150;
			Item.maxStack = 20;
			Item.width = 38;
			Item.height = 24;
		}
	}
}