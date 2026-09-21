using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.TilesAB;

public class RoyalTileBlock : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<RoyalTile>());
    }
}

public class RoyalTile : ModTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileLargeFrames[Type] = 2;
        TileID.Sets.ChecksForMerge[Type] = true;
        TileID.Sets.CantSpawnEnemies[Type] = true;

        MineResist = 2f;
        MinPick = 225;
        RegisterItemDrop(ModContent.ItemType<RoyalTileBlock>());
        AddMapEntry(new Color(31, 40, 69));
    }
}

