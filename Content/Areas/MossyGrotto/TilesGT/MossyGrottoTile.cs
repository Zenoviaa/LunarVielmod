using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MossyGrotto.TilesGT;

public class MossyGrottoTileBlock : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<MossyGrottoTile>());
    }
}
public class MossyGrottoTile : ModTile
{
    public override void SetStaticDefaults()
    {

        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;

        //Main.tileFrameImportant[Type] = true;
        Main.tileLargeFrames[Type] = 2;
        TileID.Sets.ChecksForMerge[Type] = true;
      
        RegisterItemDrop(ModContent.ItemType<MossyGrottoTileBlock>());
        AddMapEntry(new Color(40, 140, 40));
    }
}

