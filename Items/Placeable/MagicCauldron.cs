using Stellamod.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable
{
    internal class MagicCauldron : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Alcaology Station");
            // Tooltip.SetDefault("This table is used for dusts and useful Materials!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CauldronTile>());
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
            Item.value = 150;
            Item.maxStack = Item.CommonMaxStack;
            Item.width = 38;
            Item.height = 24;
        }
    }
}
