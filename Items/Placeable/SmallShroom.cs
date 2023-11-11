using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable
{
    public class SmallShroom : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Shroomies");
			// Tooltip.SetDefault("Thing");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Structures.SmallShroomS>());
			Item.value = 150;
			Item.maxStack = 20;
			Item.width = 38;
			Item.height = 24;
		}
	}
}