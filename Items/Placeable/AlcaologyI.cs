using Stellamod.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable
{
    public class AlcaologyI : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Alcaology Station");
            // Tooltip.SetDefault("This table is used for dusts and useful Materials!");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AlcaologyTable>());
            Item.value = 150;
            Item.maxStack = 20;
            Item.width = 38;
            Item.height = 24;
        }
    }
}