using Stellamod.Tiles.Structures.AlcadizNGovheil;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Shrines.GovheilNAlca
{
    public class RestingGroundsI : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Alcaology Station");
			// Tooltip.SetDefault("This table is used for dusts and useful Materials!");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<RestingGrounds>());
			ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
			Item.value = 150;
			Item.maxStack = 9999;
			Item.width = 38;
			Item.height = 24;
		}

		
	}
}