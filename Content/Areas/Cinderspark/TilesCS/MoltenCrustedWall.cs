using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.TilesCS;

public class MoltenCrustedWall : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableWall(ModContent.WallType<MoltenCrustedWallTile>());
    }
}


public class MoltenCrustedWallTile : ModWall
{
    public override void SetStaticDefaults()
    {
        Main.wallHouse[Type] = false;
        RegisterItemDrop(ModContent.ItemType<MoltenCrustedWall>());
        AddMapEntry(new Color(100, 25, 40));
    }
}