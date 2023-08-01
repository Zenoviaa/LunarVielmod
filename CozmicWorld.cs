

using Terraria;
using Terraria.ModLoader;
using Terraria.IO;
using Terraria.WorldBuilding;
using Terraria.ID;
using Stellamod.Tiles.Abyss;
using Microsoft.Xna.Framework;
using Stellamod.Tiles;
using System.Collections.Generic;
using Stellamod.Tiles.Acid;
using Stellamod.Items.Materials;
using Terraria.DataStructures;

namespace Stellamod
{
    public class CozmicWorld : GenPass
    {
        public CozmicWorld(string name, float weight) : base(name, weight) { }
        public Vector2 AcidGenPos;

        private void ClearCircleAbyssWall(int i, int j)
        {
            int radius = 80;
            for (int y = j - radius; y <= j + radius; y++)
            {
                for (int x = i - radius; x <= i + radius + 1; x++)
                {
                    if ((int)Vector2.Distance(new Vector2(x, y), new Vector2(i, j)) <= radius)
                        WorldGen.KillWall(x, y);
                }
            }
        }
        private void PlaceCircleAbyssWall(int i, int j)
        {
            int radius = 80;
            for (int y = j - radius; y <= j + radius; y++)
            {
                for (int x = i - radius; x <= i + radius + 1; x++)
                {
                    if ((int)Vector2.Distance(new Vector2(x, y), new Vector2(i, j)) <= radius)
                        WorldGen.PlaceWall(x, y, (ushort)ModContent.WallType<AbyssWallNatural>());
                }
            }
        }
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            //ABYSS-----------------------------------------------------------------------------------------------------------------------
            progress.Message = "Spawning Tutorial Ores";

            int maxToSpawn = (int)(Main.maxTilesX * Main.maxTilesY * 6E-05);
            for (int i = 0; i < maxToSpawn; i++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int Y = WorldGen.genRand.Next((int)Main.rockLayer + 340, Main.maxTilesY - 150);
                Tile t = Framing.GetTileSafely(X, Y);

                if (t.TileType == TileID.IceBlock || t.TileType == TileID.CorruptIce || t.TileType == TileID.FleshIce || t.TileType == TileID.SnowBlock)
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(263, 265), WorldGen.genRand.Next(66, 67), ModContent.TileType<AbyssalDirt>());
                    ClearCircleAbyssWall(X, Y);
                    PlaceCircleAbyssWall(X, Y);
                }

            }
            progress.Message = "Spawning Tutorial Ores";

            for (int i = 0; i < maxToSpawn; i++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, Main.maxTilesY);
                Tile t = Framing.GetTileSafely(X, Y);

                if (t.TileType == (ushort)ModContent.TileType<AbyssalDirt>())
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(30, 50), WorldGen.genRand.Next(5, 65), ModContent.TileType<AbyssalStone>());
                }
            }
            for (int i = 0; i < maxToSpawn; i++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, Main.maxTilesY);
                Tile t = Framing.GetTileSafely(X, Y);

                if (t.TileType == (ushort)ModContent.TileType<AbyssalDirt>())
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(8, 13), WorldGen.genRand.Next(4, 11), ModContent.TileType<SoulOre>());
                }
            }

            //ACID-----------------------------------------------------------------------------------------------------------------------
            for (int i = 0; i < Main.maxTilesX / 300; i++)
            {
                int x = WorldGen.genRand.Next(100, Main.maxTilesX - 20);

                int y = WorldGen.genRand.Next((int)Main.worldSurface - 100, (int)Main.worldSurface + 30);
                Tile tile = Framing.GetTileSafely(x, y);
                if ((tile.TileType == TileID.Mud || tile.TileType == TileID.JungleGrass))
                {
                    WorldGen.TileRunner(x, y, 600, WorldGen.genRand.Next(200, 200), ModContent.TileType<Tiles.Acid.AcidialDirt>(), false, 0f, 0f, true, true);
                }


                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, Main.maxTilesY);
                Tile t = Framing.GetTileSafely(X, Y);
                if (t.TileType == (ushort)ModContent.TileType<AcidialDirt>())
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(11, 19), WorldGen.genRand.Next(8, 12), ModContent.TileType<AcidStone>());
                }

            }
            //ORES-----------------------------------------------------------------------------------------------------------------------
            GenerateZiggurat();
            for (int i = 0; i < maxToSpawn; i++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, Main.maxTilesY);
                Tile t = Framing.GetTileSafely(X, Y);
                if (t.TileType == TileID.Stone || t.TileType == TileID.Dirt || t.TileType == TileID.ClayBlock)
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(9, 17), WorldGen.genRand.Next(6, 10), ModContent.TileType<LostScrapT>());
                }
                if (t.TileType == TileID.IceBlock || t.TileType == TileID.SnowBlock)
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(11, 19), WorldGen.genRand.Next(8, 12), ModContent.TileType<LostScrapT>());
                }
            }
            for (int i = 0; i < maxToSpawn; i++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, Main.maxTilesY);
                Tile t = Framing.GetTileSafely(X, Y);

                if (t.TileType == TileID.Stone || t.TileType == TileID.Dirt || t.TileType == TileID.ClayBlock)
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(5, 12), WorldGen.genRand.Next(2, 7), ModContent.TileType<Arnchar>());
                }
                if (t.TileType == TileID.Mud)
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(8, 18), WorldGen.genRand.Next(4, 11), ModContent.TileType<Arnchar>());
                }
            }
            for (int i = 0; i < maxToSpawn; i++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, Main.maxTilesY);
                Tile t = Framing.GetTileSafely(X, Y);

                if (t.TileType == TileID.Stone || t.TileType == TileID.Dirt || t.TileType == TileID.ClayBlock)
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(7, 12), ModContent.TileType<CoreGemT>());
                }
            }

        }
        private static void GenerateZiggurat()
        {
            int[,] AlterShape = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,1,0,0,0,1,1,0,0,0,0,0,0,0,0,1,1,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0},
                {0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0},
                {0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0},
                {0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0},
                {0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0},
                {0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0},
                {0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0},
            };

            int[,] AlterWalls = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,1,1,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,1,1,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int[,] AlterLoot = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,6,0,0,0,0,7,0,0,7,0,0,0,0,5,5,7,0,0,0,7,0,0,0,0,0,6,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            bool placed = false;
            while (!placed)
            {
                // Select a place in the first 6th of the world
                int hideoutX = WorldGen.genRand.Next(Main.maxTilesX / 6, Main.maxTilesX / 6 * 5); // from 50 since there's a unaccessible area at the world's borders
                                                                                                  // 50% of choosing the last 6th of the world
                if (WorldGen.genRand.NextBool())
                {
                    hideoutX = Main.maxTilesX - hideoutX;
                }

                int hideoutY = 0;
                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(hideoutX, hideoutY) && hideoutY <= Main.worldSurface)
                {
                    hideoutY++;
                }

                // If we went under the world's surface, try again
                if (hideoutY > Main.worldSurface + 15)
                {
                    continue;
                }

                Tile tile = Main.tile[hideoutX, hideoutY];
                // If the type of the tile we are placing the hideout on doesn't match what we want, try again
                if (tile.TileType != TileID.Sand && tile.TileType != TileID.Sandstone)
                {
                    continue;
                }


                // place the hideout
                PlaceAlter(hideoutX, hideoutY - 1, AlterShape, AlterWalls, AlterLoot);
                placed = true;
            }
        }

        private static void PlaceAlter(int i, int j, int[,] BlocksArray, int[,] WallsArray, int[,] LootArray)
        {
            for (int y = 0; y < BlocksArray.GetLength(0); y++)
            {
                for (int x = 0; x < BlocksArray.GetLength(1); x++)
                {
                    int k = i - 3 + x;
                    int l = j - 6 + y;
                    if (WorldGen.InWorld(k, l, 30))
                    {
                        Tile tile = Framing.GetTileSafely(k, l);
                        switch (BlocksArray[y, x])
                        {
                            case 0:
                                break;
                            case 1:
                                WorldGen.KillWall(k, l);
                                Framing.GetTileSafely(k, l).ClearTile();
                                break;
                            case 2:
                                WorldGen.KillWall(k, l);
                                Framing.GetTileSafely(k, l).ClearTile();
                                break;
                            case 3:
                                WorldGen.KillWall(k, l);
                                Framing.GetTileSafely(k, l).ClearTile();
                                break;
                        }
                    }
                }
            }

            for (int y = 0; y < WallsArray.GetLength(0); y++)
            {
                for (int x = 0; x < WallsArray.GetLength(1); x++)
                {
                    int k = i - 3 + x;
                    int l = j - 6 + y;
                    if (WorldGen.InWorld(k, l, 30))
                    {
                        Tile tile = Framing.GetTileSafely(k, l);
                        switch (WallsArray[y, x])
                        {
                            case 0:
                                break;
                            case 1:
                                WorldGen.KillWall(k, l);
                                Framing.GetTileSafely(k, l).ClearTile();
                                break;
                            case 2:
                                WorldGen.KillWall(k, l);
                                Framing.GetTileSafely(k, l).ClearTile();
                                break;
                            case 3:
                                WorldGen.KillWall(k, l);
                                Framing.GetTileSafely(k, l).ClearTile();
                                break;
                        }
                    }
                }
            }

            for (int y = 0; y < BlocksArray.GetLength(0); y++)
            {
                for (int x = 0; x < BlocksArray.GetLength(1); x++)
                {
                    int k = i - 3 + x;
                    int l = j - 6 + y;
                    if (WorldGen.InWorld(k, l, 30))
                    {
                        Tile tile = Framing.GetTileSafely(k, l);
                        switch (BlocksArray[y, x])
                        {
                            case 0:
                                break;
                            case 1:
                                WorldGen.PlaceTile(k, l, 151);
                                tile.HasTile = true;
                                break;
                            case 2:
                                WorldGen.PlaceTile(k, l, 152);
                                tile.HasTile = true;
                                break;
                        }
                    }
                }
            }

            for (int y = 0; y < WallsArray.GetLength(0); y++)
            {
                for (int x = 0; x < WallsArray.GetLength(1); x++)
                {
                    int k = i - 3 + x;
                    int l = j - 6 + y;
                    if (WorldGen.InWorld(k, l, 30))
                    {
                        Tile tile = Framing.GetTileSafely(k, l);
                        switch (WallsArray[y, x])
                        {
                            case 0:
                                break;
                            case 1:
                                WorldGen.PlaceWall(k, l, 34);
                                break;
                            case 2:
                                WorldGen.PlaceWall(k, l, 35);
                                break;
                            case 3:
                                WorldGen.PlaceWall(k, l, 34);
                                break;
                        }
                    }
                }
            }

            for (int y = 0; y < LootArray.GetLength(0); y++)
            {
                for (int x = 0; x < LootArray.GetLength(1); x++)
                {
                    int k = i - 3 + x;
                    int l = j - 6 + y;
                    if (WorldGen.InWorld(k, l, 30))
                    {
                        Tile tile = Framing.GetTileSafely(k, l);
                        switch (LootArray[y, x])
                        {
                            case 0:
                                break;
                            case 4:

                                break;
                            case 5:
                                WorldGen.PlaceObject(k, l, (ushort)ModContent.TileType<SunAlter>());
                                break;
                            case 6:
                                WorldGen.PlaceObject(k, l, 37);
                                break;
                            case 7:
                                WorldGen.PlaceTile(k, l, 28);
                                break;
                            case 8:
                                WorldGen.PlaceTile(k, l, 102);
                                break;
                        }
                    }
                }
            }
        }
    }
}