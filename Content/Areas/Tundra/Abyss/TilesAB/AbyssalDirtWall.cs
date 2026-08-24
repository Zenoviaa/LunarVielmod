using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.TilesAB;

public class AbyssalDirtWallItem : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableWall(ModContent.WallType<AbyssalDirtWall>());
    }
}

public class AbyssalDirtWall : ModWall
{
    public override void SetStaticDefaults()
    {
        Main.wallHouse[Type] = false;
        AddMapEntry(new Color(20, 20, 125));
    }
}
