using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Tiles.Abyss
{
    public class AbyssalIce : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[TileID.IceBlock][Type] = true;
            Main.tileMerge[TileID.SnowBlock][Type] = true;
            Main.tileMerge[ModContent.TileType<AbyssalDirt>()][Type] = true;
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
                    WorldGen.PlaceTile(i, j - 1, ModContent.TileType<BlueFlower>(), true);
                }
            }
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0)//grass
            {
                if (Main.rand.NextBool(3))
                {
                    WorldGen.PlaceTile(i, j - 1, ModContent.TileType<BlueFlower2>(), true);
                }
            }
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0)//grass
            {
                if (Main.rand.NextBool(2))
                {
                    WorldGen.PlaceTile(i, j - 2, ModContent.TileType<TealBulb>(), true);
                }
            }
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0)//grass
            {
                if (Main.rand.NextBool(2))
                {
                    WorldGen.PlaceTile(i, j, ModContent.TileType<TealBulb2>(), true);
                }
            }
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0)//grass
            {
                if (Main.rand.NextBool(2))
                {
                    WorldGen.PlaceTile(i, j - 1, ModContent.TileType<TealBulb3>(), true);
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
}
