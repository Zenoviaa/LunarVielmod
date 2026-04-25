using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.TilesTR;

public class AegislavSand : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<AegislavSandTile>());
    }
}

public class AegislavSandTile : ModTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.tileSolid[Type] = true;
        Main.tileMerge[Type][Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileMerge[TileID.Dirt][Type] = true;
        Main.tileMerge[TileID.Grass][Type] = true;
        Main.tileBlendAll[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileBlockLight[Type] = true;
        RegisterItemDrop(ModContent.ItemType<AegislavSand>());
        AddMapEntry(Color.IndianRed);
    }
}