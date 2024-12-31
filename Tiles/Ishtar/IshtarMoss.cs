
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Tiles.Ishtar
{
    public class IshtarMoss : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Type] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileMerge[TileID.ClayBlock][Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileBlockLight[Type] = true;
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(40, 40, 54), name);
        }
        public override bool CanExplode(int i, int j) => false;
        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0 && j <= Main.worldSurface - 150)//grass
            {
                if (Main.rand.NextBool(12))
                {
                    WorldGen.PlaceTile(i, j - 1, TileType<IshtarWeb1>(), true);
                }
            }
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0 && j <= Main.worldSurface - 150)//grass
            {
                if (Main.rand.NextBool(12))
                {
                    WorldGen.PlaceTile(i, j - 1, TileType<IshtarWeb2>(), true);
                }
            }
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0 && j <= Main.worldSurface - 150)//grass
            {
                if (Main.rand.NextBool(12))
                {
                    WorldGen.PlaceTile(i, j - 1, TileType<IshtarWeb3>(), true);
                }
            }


            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0 && j >= Main.worldSurface - 150)//grass
            {
                if (Main.rand.NextBool(3))
                {
                    WorldGen.PlaceTile(i, j - 2, TileType<IshtarWeb1>(), true);
                }
            }
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0 && j >= Main.worldSurface - 150)//grass
            {
                if (Main.rand.NextBool(3))
                {
                    WorldGen.PlaceTile(i, j - 2, TileType<IshtarWeb2>(), true);
                }
            }
            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0 && j >= Main.worldSurface - 150)//grass
            {
                if (Main.rand.NextBool(3))
                {
                    WorldGen.PlaceTile(i, j - 2, TileType<IshtarWeb3>(), true);
                }
            }

            if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0 && j >= Main.worldSurface - 150)//grass
            {
                if (Main.rand.NextBool(3))
                {
                    WorldGen.PlaceTile(i, j - 2, TileType<IshtarWeb4>(), true);
                }
            }

            if (WorldGen.genRand.NextBool(1) && !tileBelow.HasTile && !(tileBelow.LiquidType == LiquidID.Lava))
            {
                if (!tile.BottomSlope)
                {
                    tileBelow.TileType = (ushort)ModContent.TileType<IshtarVines>();
                    tileBelow.HasTile = true;
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                    }
                }
            }

            //try place foliage
            if (WorldGen.genRand.NextBool(6) && !tileAbove.HasTile && !(tileBelow.LiquidType == LiquidID.Lava))
            {
                if (!tile.BottomSlope && !tile.TopSlope && !tile.IsHalfBlock && !tile.TopSlope)
                {
                    tileAbove.TileType = (ushort)ModContent.TileType<IshtarFoliage>();
                    tileAbove.HasTile = true;
                    tileAbove.TileFrameY = 0;
                    tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(8) * 18);
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
                }
            }
        }

        
    }
}