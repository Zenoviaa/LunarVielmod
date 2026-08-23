using Stellamod.Content.Areas.Tundra.Abyss.TilesAB.Aurelus;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable;

public class AurelusBorderI : ModItem
{
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Bar");
        // Tooltip.SetDefault("Thing");

        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<AurelusBorderS>());
        Item.value = 150;
        Item.maxStack = 40;
        Item.width = 38;
        Item.height = 24;
    }
}