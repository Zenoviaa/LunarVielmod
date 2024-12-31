using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable.Cathedral
{
    public class ShrineI : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Verlia's Shrine");
			// Tooltip.SetDefault("Use at your own risk");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Structures.Cathedral.VerliasShrine>());
			Item.value = 150;
			Item.maxStack = 20;
			Item.width = 38;
			Item.height = 24;
		}

		
	}
}