using Stellamod.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable
{
    public class SyliaBossRel : ModItem
	{
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SyliaBossRelic>());
			Item.value = 150;
			Item.maxStack = 20;
			Item.width = 32;
			Item.height = 44;

			Item.rare = ItemRarityID.Master;
			Item.master = true; // This makes sure that "Master" displays in the tooltip, as the rarity only changes the item name color
			Item.value = Item.buyPrice(0, 5);
		}
	}
}