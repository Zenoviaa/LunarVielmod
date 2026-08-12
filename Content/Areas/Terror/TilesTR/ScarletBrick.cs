using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.TilesTR;

internal class ScarletBrick : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<ScarletBrickTile>());
    }
}
internal class ScarletBrickTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;

        //Main.tileFrameImportant[Type] = true;
        Main.tileLargeFrames[Type] = 2;
        TileID.Sets.ChecksForMerge[Type] = true;
        MineResist = 2f;
        MinPick = 225;
        RegisterItemDrop(ModContent.ItemType<ScarletBrick>());
        AddMapEntry(new Color(200, 40, 40));
    }
}

internal class ScarletDiamondBrick : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<ScarletDiamondBrickTile>());
    }
}
internal class ScarletDiamondBrickTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;

        //Main.tileFrameImportant[Type] = true;
        Main.tileLargeFrames[Type] = 2;
        TileID.Sets.ChecksForMerge[Type] = true;
        MineResist = 2f;
        MinPick = 225;
        RegisterItemDrop(ModContent.ItemType<ScarletBrick>());
        AddMapEntry(new Color(200, 40, 40));
    }
}