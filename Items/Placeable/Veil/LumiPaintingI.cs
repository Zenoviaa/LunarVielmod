using Stellamod.Tiles.Veil;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable.Veil
{
    public class LumiPaintingI : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("CurtainLeft");
            // Tooltip.SetDefault("Curtain");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LumiPainting>());
            Item.value = 150;
            Item.maxStack = 9999;
            Item.width = 38;
            Item.height = 24;
        }
    }
}