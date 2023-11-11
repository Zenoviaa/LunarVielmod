using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Tiles.Abyss
{
    public class AbyssalDirt : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[TileID.IceBlock][Type] = true;
            Main.tileMerge[TileID.SnowBlock][Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(57, 55, 172));
        }
   
        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            //Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0)//grass
            {
                if (Main.rand.NextBool(3))
                {
                    WorldGen.PlaceTile(i, j - 1, TileType<BlueFlower>(), true);
                }
            }
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0)//grass
            {
                if (Main.rand.NextBool(3))
                {
                    WorldGen.PlaceTile(i, j - 1, TileType<BlueFlower2>(), true);
                }
            }
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0)//grass
            {
                if (Main.rand.NextBool(2))
                {
                    WorldGen.PlaceTile(i, j - 2, TileType<TealBulb>(), true);
                }
            }
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0)//grass
            {
                if (Main.rand.NextBool(2))
                {
                    WorldGen.PlaceTile(i, j, TileType<TealBulb2>(), true);
                }
            }
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0)//grass
            {
                if (Main.rand.NextBool(2))
                {
                    WorldGen.PlaceTile(i, j - 1, TileType<TealBulb3>(), true);
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

        private List<Point> OpenAdjacents(int i, int j, int type)
        {
            var p = new List<Point>();
            for (int k = -1; k < 2; ++k)
                for (int l = -1; l < 2; ++l)
                    if (!(l == 0 && k == 0) && Framing.GetTileSafely(i + k, j + l).HasTile && Framing.GetTileSafely(i + k, j + l).TileType == type)
                        p.Add(new Point(i + k, j + l));
            return p;
        }

        private bool HasOpening(int i, int j)
        {
            for (int k = -1; k < 2; ++k)
                for (int l = -1; l < 2; ++l)
                    if (!Framing.GetTileSafely(i + k, j + l).HasTile)
                        return true;
            return false;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
        }
    }
}