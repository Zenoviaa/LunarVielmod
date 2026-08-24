using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Content.Areas.Tundra.Abyss.TilesAB;

public class AbyssalDirtItem : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<AbyssalDirt>());
    }
}

public class AbyssalDirt : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMerge[Type][Type] = true;
        Main.tileBlockLight[Type] = false;
        Main.tileLargeFrames[Type] = 2;
        Main.tileMerge[TileID.IceBlock][Type] = true;
        Main.tileMerge[TileID.SnowBlock][Type] = true;
        Main.tileMerge[ModContent.TileType<AbyssalIce>()][Type] = true;
        Main.tileBlendAll[Type] = true;
        RegisterItemDrop(ModContent.ItemType<AbyssalDirtItem>());
        AddMapEntry(new Color(57, 55, 172));
    }

    public override void RandomUpdate(int i, int j)
    {

        Tile tile = Framing.GetTileSafely(i, j);
        Tile tileBelow = Framing.GetTileSafely(i, j + 1);
        int[] pool = new int[]
        {
            ModContent.TileType<BlueFlower>(),
            ModContent.TileType<BlueFlower2>(),
            ModContent.TileType<TealBulb>(),
            ModContent.TileType<TealBulb2>(),
            ModContent.TileType<TealBulb3>()
        };

        if (!Main.rand.NextBool(32))
            return;


        //Tile tileAbove = Framing.GetTileSafely(i, j - 1);
        if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0)//grass
        {
            WorldGen.PlaceTile(i, j - 1, pool[Main.rand.Next(0, pool.Length)], true);
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendTileSquare(-1, i, j - 1, TileChangeType.None);
            }
        }
     
        //Try place vine
        if (WorldGen.genRand.NextBool(3) && !tileBelow.HasTile && !(tileBelow.LiquidType == LiquidID.Lava))
        {
            if (!tile.BottomSlope)
            {
                tileBelow.TileType = (ushort)ModContent.TileType<AbyssalVines>();
                tileBelow.HasTile = true;
                WorldGen.SquareTileFrame(i, j + 1, true);
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                }
            }
        }
        if (WorldGen.genRand.NextBool(3) && !tileBelow.HasTile && !(tileBelow.LiquidType == LiquidID.Lava))
        {
            if (!tile.BottomSlope)
            {
                tileBelow.TileType = (ushort)ModContent.TileType<AbyssalVines2>();
                tileBelow.HasTile = true;
                WorldGen.SquareTileFrame(i, j + 1, true);
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                }
            }
        }
    }
}