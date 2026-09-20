using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.TilesAB;

public class SwirlyWoodWallItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableWall(ModContent.WallType<SwirlyWoodWall>());
    }
}

public class SwirlyWoodWall : ModWall
{
    public override void SetStaticDefaults()
    {
        Main.wallHouse[Type] = false;
        AddMapEntry(new Color(120, 120, 25));
    }
}
