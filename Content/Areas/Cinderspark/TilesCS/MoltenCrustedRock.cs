using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.TilesCS;

public class MoltenCrustedRock : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<MoltenCrustedRockTile>());
    }
}

public class MoltenCrustedRockTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMerge[Type][Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileBlendAll[Type] = true;
        Main.tileBlockLight[Type] = true;
        AddMapEntry(new Color(100, 25, 40));
        RegisterItemDrop(ModContent.ItemType<MoltenCrustedRock>());
    }
}

