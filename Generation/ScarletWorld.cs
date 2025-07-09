using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Stellamod.Content.Tiles.IceTiles;
using Stellamod.Content.Tiles.RainforestTiles;
using Stellamod.Core.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Generation
{
    internal class ScarletWorld : ModSystem
    {
        private void DoNothing(GenerationProgress progress, GameConfiguration configuration) { }
        private void ReplacePassLegacy(List<GenPass> tasks, string name, WorldGenLegacyMethod method)
        {
            var pass = new PassLegacy(name, method);
            int index = tasks.FindIndex(genpass => genpass.Name.Equals(name));
            if (index != -1)
            {
                tasks[index] = pass;
            }
        }

        private void RemoveMostPasses(List<GenPass> tasks)
        {
            for (int i = 2; i < tasks.Count; i++)
            {
                //Bring back an older pass
                if (tasks[i].Name == "Spawn Point")
                    continue;

                if (tasks[i].Name == "Quick Cleanup")
                    continue;

                if (tasks[i].Name == "Clean Up Dirt")
                    continue;

                if (tasks[i].Name == "Smooth World")
                    continue;

                if (tasks[i].Name == "Grass")
                    continue;

                if (tasks[i].Name == "Lakes")
                    continue;

                if (tasks[i].Name == "Spreading Grass")
                    continue;

                if (tasks[i].Name == "Gems In Ice Biome")
                    continue;

                if (tasks[i].Name == "Tile Cleanup")
                    continue;

                if (tasks[i].Name == "Cave Walls")
                    continue;

                tasks[i].Disable();
            }
        }

        private void DisableGenTask(List<GenPass> tasks, string passName)
        {
            tasks.Find(x => x.Name.Equals(passName)).Disable();
        }

        private int FindIndex(List<GenPass> tasks, string passName)
        {
            return tasks.FindIndex(genpass => genpass.Name.Equals(passName));
        }

        private int _indexToInsertAt;
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            RemoveMostPasses(tasks);
            int terrainIndex = FindIndex(tasks, "Terrain");
            if (terrainIndex != -1)
            {
                tasks.Insert(terrainIndex + 1, new VanillaTerrainPass());
            }

            int IceGen = FindIndex(tasks, "Tunnels");
            if (IceGen != -1)
            {
                tasks.Insert(IceGen + 1, new PassLegacy("Ice Clump", IceClump));
                tasks.Insert(IceGen + 2, new PassLegacy("Ice Caves Surface", IceyCaves));
                tasks.Insert(IceGen + 3, new PassLegacy("Ice Bridges", RuneBridges));
                tasks.Insert(IceGen + 4, new PassLegacy("Abysm Clumping", AbysmClump));
                tasks.Insert(IceGen + 4, new PassLegacy("Icy Waters", MakingIcyPonds));
                tasks.Insert(IceGen + 5, new PassLegacy("Icy Surface Gremlins", SurfaceIceHouses));
                tasks.Insert(IceGen + 6, new PassLegacy("The icy spook", MakingIcySnows));
                tasks.Insert(IceGen + 7, new PassLegacy("Walls ice underground", MakingIcyWalls));
                tasks.Insert(IceGen + 8, new PassLegacy("Icy Pikes Underground", MakingIcyUndergroundSpikes));
                tasks.Insert(IceGen + 9, new PassLegacy("Icy Pikes Overground", MakingIcyRandomness));
                tasks.Insert(IceGen + 10, new PassLegacy("Ice Caves Underground", IceyUndergroundCaves));
                tasks.Insert(IceGen + 11, new PassLegacy("Ice Crystals Spwning", IceCrystalsSpawning));
                tasks.Insert(IceGen + 12, new PassLegacy("Ice Fog", IceFog));
                tasks.Insert(IceGen + 13, new PassLegacy("Boreal Trees!", BorealTreeSpawning));
            }
        }

        Point AbysmStart;
        Point AbysmStart2;

        #region IceBiomeGeneration

        private void IceClump(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Ice biome mounding";
            int smx = 0;
            int smy = 0;
            int contdown = 0;
            int contdownx = 0;
            smx = Main.maxTilesX - 2100;
            smy = (int)GenVars.worldSurfaceHigh - 600;
            while (!WorldGen.SolidTile(smx, smy) && smy <= Main.UnderworldLayer)
            {
                //seperation
                smy += 1;
            }


            for (int da = 0; da < 1; da++)
            {
                Point Loc7 = new Point(smx, smy);
                Point Loc8 = new Point(smx, smy + 100);
                WorldUtils.Gen(Loc8, new Shapes.Mound(450, 300), Actions.Chain(new GenAction[]
                    {
                            new Actions.ClearWall(true),
                            new Actions.SetTile(TileID.SnowBlock),
                            new Actions.Smooth(true)
                    }));


                // Spawn in Ice Chunks
                WorldGen.TileRunner(Loc7.X, Loc7.Y, 1000, 6, TileID.SnowBlock, false, 0f, 0f, true, true);
                WorldGen.TileRunner(Loc7.X, Loc7.Y + 300, 1200, 7, TileID.IceBlock, false, 0f, 0f, true, true);
                WorldGen.TileRunner(Loc7.X, Loc7.Y + 600, 1000, 2, TileID.IceBlock, false, 0f, 0f, true, true);
                WorldGen.TileRunner(Loc7.X, Loc7.Y + 900, 500, 2, TileID.IceBlock, false, 0f, 0f, true, true);
                WorldGen.TileRunner(Loc7.X, Loc7.Y + 1200, 500, 2, TileID.IceBlock, false, 0f, 0f, true, true);
                WorldGen.TileRunner(Loc7.X, Loc7.Y + 1500, 500, 2, TileID.IceBlock, false, 0f, 0f, true, true);
                WorldGen.TileRunner(Loc7.X, Loc7.Y + 1800, 500, 2, TileID.IceBlock, false, 0f, 0f, true, true);
                WorldGen.TileRunner(Loc7.X, Loc7.Y + 1800, 700, 2, TileID.IceBlock, false, 0f, 0f, true, true);




                WorldUtils.Gen(Loc7, new Shapes.Circle(500, 300), Actions.Chain(new GenAction[]
                {
                        new Actions.ClearWall(true),
                        new Actions.PlaceWall(WallID.SnowWallUnsafe),
                        new Actions.Smooth(true)
                }));

                // Dig big chasm at top


            }

            for (int daa = 0; daa < 30; daa++)
            {
                contdown -= 10;
                contdownx -= 20;
                // Dig big chasm at top
                WorldGen.digTunnel(smx - Main.rand.Next(10), smy - 250 - contdown, 0, 1, 1, 15, false);

                WorldGen.digTunnel(smx - 300 - contdownx, smy + 1200, 0, 1, 1, Main.rand.Next(40) + 10, true);

                WorldGen.digTunnel(smx - 300 - contdownx, smy + 1500, 0, 1, 1, Main.rand.Next(40) + 10, true);

                WorldGen.digTunnel(smx - 300 - contdownx, smy + 1800, 0, 1, 1, Main.rand.Next(40) + 10, true);
                AbysmStart = new Point(smx, smy - 250 - contdown);
                AbysmStart2 = new Point(smx, smy - 250 - contdown);
            }


            AbysmStart = new Point(smx, smy - 250 - contdown);
            AbysmStart2 = new Point(smx, smy - 250 - contdown);



            int contdownx2 = 0;

            for (int daa = 0; daa < 23; daa++)
            {
                contdownx2 -= 20;

                // Dig big chasm at top
                Vector2 WallPosition = new Vector2(AbysmStart2.X - contdownx2 - 100, AbysmStart2.Y - 40);
                WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(60), Actions.Chain(new GenAction[]
                {
                    new Actions.ClearWall(true),
                //    new Actions.SetTile(TileID.IceBlock),
   
                }));

                Vector2 WallPosition2 = new Vector2(AbysmStart2.X + contdownx2 + 100, AbysmStart2.Y - 40);
                WorldUtils.Gen(WallPosition2.ToPoint(), new Shapes.Circle(60), Actions.Chain(new GenAction[]
                {
                new Actions.ClearWall(true),
                //   new Actions.SetTile(TileID.IceBlock),

                }));

                WorldGen.digTunnel(AbysmStart2.X - contdownx2 - 100, AbysmStart2.Y - 40 - Main.rand.Next(30), 0, 0, 1, Main.rand.Next(20) + 10, false);
                WorldGen.digTunnel(AbysmStart2.X + contdownx2 + 100, AbysmStart2.Y - 40 - Main.rand.Next(30), 0, 0, 1, Main.rand.Next(20) + 10, false);


            }

            /*
            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 8.2f) * 6E-04); k++)
            {
                int X = WorldGen.genRand.Next(1000, Main.maxTilesX - 1000);
                int Y = WorldGen.genRand.Next(AbysmStart2.Y - 90, AbysmStart2.Y + 90);
                int yBelow = Y + 1;
                Vector2 WallPosition = new Vector2(X, yBelow);
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.SnowBlock ||
                    Main.tile[X, yBelow].TileType == TileID.IceBlock)
                {

                    WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(WorldGen.genRand.Next(1, 17)), Actions.Chain(new GenAction[]
                       {
                            new Actions.PlaceWall(WallID.SnowWallUnsafe),
                            new Actions.Smooth(true)
                       }));
                }

            }*/

            int startX = AbysmStart2.X - 500;
            int endX = AbysmStart2.X + 500;
            int startY = AbysmStart2.Y - 90;
            int endY = AbysmStart2.Y + 90;

            startX = (int)MathHelper.Clamp(startX, 0, Main.maxTilesX);
            endX = (int)MathHelper.Clamp(endX, 0, Main.maxTilesX);
            startY = (int)MathHelper.Clamp(startY, 0, Main.maxTilesY);
            endY = (int)MathHelper.Clamp(endY, 0, Main.maxTilesY);

            int steps = 5000;
            for (int j = 0; j < steps; j++)
            {
                int x = Main.rand.Next(startX, endX);
                int y = Main.rand.Next(startY, endY);
                int yBelow = y + 1;
                Vector2 WallPosition = new Vector2(x, yBelow);
                if (!WorldGen.SolidTile(x, yBelow))
                    continue;

                if (Main.tile[x, yBelow].TileType == TileID.SnowBlock ||
                    Main.tile[x, yBelow].TileType == TileID.IceBlock)
                {

                    WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(WorldGen.genRand.Next(1, 17)), Actions.Chain(new GenAction[]
                       {
                            new Actions.PlaceWall(WallID.SnowWallUnsafe),
                            new Actions.Smooth(true)
                       }));
                }
            }
        }

        private void AbysmClump(GenerationProgress progress, GameConfiguration configuration)
        {
            int contdown = 0;
            int contdownx = 0;

            int caveSeed = WorldGen.genRand.Next();

            for (int i = 0; i < 12; i++)
            {


                int caveSteps = 200; // How many carves
                int Blockwidth = 9; //Block width for how far
                int Blockwidth3 = 15;

                Vector2 baseCaveDirection = Vector2.UnitY.RotatedBy(WorldGen.genRand.NextFloatDirection() * (i * 0.04f));
                Vector2 AbysmPosition = new Vector2(AbysmStart.X, AbysmStart.Y + contdown + 500);


                for (int j = 0; j < caveSteps; j++)
                {
                    contdown -= 20;
                    float caveOffsetAngleAtStep = WorldMath.PerlinNoise2D(j / 50f, j / 50f, 4, caveSeed) * MathHelper.Pi * 1.9f;
                    Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

                    // Carve out at the current position.
                    //Makes sure it doesn't go out of bouds
                    //  Vector2 AbysmPosition = new Vector2(AbysmStart.X, AbysmStart.Y + contdown);

                    if (AbysmStart.X < Main.maxTilesX - 15 && AbysmStart.X >= 15)
                    {
                        WorldUtils.Gen(AbysmPosition.ToPoint(), new Shapes.Circle(Blockwidth3), Actions.Chain(new GenAction[]
                     {
                            new Actions.SetTile(TileID.IceBlock),
                            //new Modifiers.Dither(.2),// Dithering
                            new Actions.Smooth(true)
                     }));




                    }

                    /*
                    if(AbysmStart.X < Main.maxTilesX - 15 && AbysmStart.X >= 15)
                    {


                        WorldUtils.Gen(AbysmPosition.ToPoint(), new Shapes.Circle(Blockwidth2), Actions.Chain(new GenAction[]
                       {
                            new Actions.SetTile(TileID.IceBlock),
                            new Actions.Smooth(true)
                       }));
                        //building 

                    }

                    if (AbysmStart.X < Main.maxTilesX - 15 && AbysmStart.X >= 15)
                    {

                        //building 
                        WorldUtils.Gen(AbysmPosition.ToPoint(), new Shapes.Circle(Blockwidth), Actions.Chain(new GenAction[]
                        {
                             new Actions.SetTile(TileID.SnowBlock),
                            new Actions.Smooth(true)
                        }));


                    }
                   */
                    // Update the cave position.
                    AbysmPosition += caveDirection * Blockwidth;
                }


                contdown = 0;



            }


            for (int i = 0; i < 15; i++)
            {

                int caveWidth = 1 + WorldGen.genRand.Next(1, 3); // Width
                int caveSteps = 1000; // How many carves
                                      //

                Vector2 baseCaveDirection = Vector2.UnitY.RotatedBy(WorldGen.genRand.NextFloatDirection() * (i * 0.035f));
                Vector2 AbysmPosition = new Vector2(AbysmStart.X, AbysmStart.Y + contdown);








                for (int j = 0; j < caveSteps; j++)
                {
                    contdown -= 20;
                    float caveOffsetAngleAtStep = WorldMath.PerlinNoise2D(j / 50f, j / 50f, 4, caveSeed) * MathHelper.Pi * 1.9f;
                    Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

                    // Carve out at the current position.
                    //Makes sure it doesn't go out of bouds
                    if (AbysmStart.X < Main.maxTilesX - 15 && AbysmStart.X >= 15)
                    {
                        //digging 

                        WorldGen.digTunnel(AbysmPosition.X, AbysmPosition.Y, caveDirection.X, caveDirection.Y, 1, (int)(caveWidth * 1.18f), false);
                        WorldUtils.Gen(AbysmPosition.ToPoint(), new Shapes.Circle(caveWidth), Actions.Chain(new GenAction[]
                        {
                            new Actions.ClearTile(true),
                            new Actions.Smooth(true)
                        }));
                    }

                    // Update the cave position.
                    AbysmPosition += caveDirection * caveWidth;
                }

                contdown = 0;
            }


            for (int i = 0; i < 10; i++)
            {

                int caveWidth = 1; // Width
                int caveSteps = 2000; // How many carves


                Vector2 baseCaveDirection = Vector2.UnitY.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);

                Vector2 AbysmPosition2 = new Vector2(AbysmStart.X + contdownx, AbysmStart.Y + contdown);







                for (int j = 0; j < caveSteps; j++)
                {
                    contdown -= 20;
                    contdownx -= 5;
                    float caveOffsetAngleAtStep = WorldMath.PerlinNoise2D(j / 50f, j / 50f, 4, caveSeed) * MathHelper.Pi * 1.9f;
                    Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

                    // Carve out at the current position.
                    //Makes sure it doesn't go out of bouds
                    if (AbysmStart.X < Main.maxTilesX - 15 && AbysmStart.X >= 15)
                    {
                        //digging 

                        WorldGen.digTunnel(AbysmPosition2.X, AbysmPosition2.Y, caveDirection.X, caveDirection.Y, 1, (int)(caveWidth * 1.18f), true);
                        WorldUtils.Gen(AbysmPosition2.ToPoint(), new Shapes.Circle(caveWidth), Actions.Chain(new GenAction[]
                        {
                            new Actions.ClearTile(true),
                            new Actions.Smooth(true)
                        }));
                    }

                    // Update the cave position.
                    AbysmPosition2 += caveDirection * caveWidth;
                }

                contdown = 0;
                contdownx = 0;
            }


            WorldUtils.Gen(AbysmStart2, new Shapes.HalfCircle(90), Actions.Chain(new GenAction[]
                  {
                            new Actions.ClearWall(true),
                            new Actions.ClearTile(true),
                            new Actions.Smooth(true)
                  }));




        }
        private void MakingIcyWalls(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Walls forming in the ice and abysm";

            int startX = AbysmStart.X - 500;
            int endX = AbysmStart.X + 500;
            int startY = AbysmStart.Y - 500;
            int endY = AbysmStart.Y + 3000;

            startX = (int)MathHelper.Clamp(startX, 0, Main.maxTilesX - 100);
            endX = (int)MathHelper.Clamp(endX, 0, Main.maxTilesX - 100);
            startY = (int)MathHelper.Clamp(startY, 0, Main.maxTilesY - 100);
            endY = (int)MathHelper.Clamp(endY, 0, Main.maxTilesY - 100);

            int steps = 30000;
            for (int j = 0; j < steps; j++)
            {
                int x = WorldGen.genRand.Next(startX, endX);
                int y = WorldGen.genRand.Next(startY, endY);

                if (WorldGen.genRand.NextBool(2))
                {
                    int yBelow = y + 1;
                    Vector2 WallPosition = new Vector2(x, yBelow);
                    if (!WorldGen.SolidTile(x, yBelow))
                        continue;

                    if (Main.tile[x, yBelow].TileType == TileID.SnowBlock ||
                        Main.tile[x, yBelow].TileType == TileID.IceBlock)
                    {

                        WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(WorldGen.genRand.Next(1, 17)), Actions.Chain(new GenAction[]
                           {
                            new Actions.PlaceWall(WallID.SnowWallUnsafe),
                            new Actions.Smooth(true)
                           }));



                    }
                }

                if (WorldGen.genRand.NextBool(2))
                {
                    int yBelow = y + 1;
                    Vector2 WallPosition = new Vector2(x, yBelow);
                    if (!WorldGen.SolidTile(x, yBelow))
                        continue;

                    if (Main.tile[x, yBelow].TileType == TileID.SnowBlock ||
                        Main.tile[x, yBelow].TileType == TileID.IceBlock)
                    {

                        WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(WorldGen.genRand.Next(1, 17)), Actions.Chain(new GenAction[]
                           {
                            new Actions.PlaceWall(WallID.IceEcho),
                            new Actions.Smooth(true)
                           }));
                    }
                }
                if (WorldGen.genRand.NextBool(2))
                {
                    int yBelow = y + 1;
                    Vector2 WallPosition = new Vector2(x, yBelow);
                    if (!WorldGen.SolidTile(x, yBelow))
                        continue;

                    if (Main.tile[x, yBelow].TileType == TileID.SnowBlock ||
                        Main.tile[x, yBelow].TileType == TileID.IceBlock)
                    {

                        WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(WorldGen.genRand.Next(1, 17)), Actions.Chain(new GenAction[]
                           {
                            new Actions.PlaceWall(WallID.SnowWallUnsafe),
                            new Actions.Smooth(true)
                           }));



                    }
                }

                if (WorldGen.genRand.NextBool(2))
                {
                    int yBelow = y + 1;
                    Vector2 WallPosition = new Vector2(x, yBelow);
                    if (!WorldGen.SolidTile(x, yBelow))
                        continue;

                    if (Main.tile[x, yBelow].TileType == TileID.SnowBlock ||
                        Main.tile[x, yBelow].TileType == TileID.IceBlock)
                    {

                        WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(WorldGen.genRand.Next(1, 17)), Actions.Chain(new GenAction[]
                           {
                            new Actions.PlaceWall(WallID.IceEcho),
                            new Actions.Smooth(true)
                           }));
                    }
                }
            }
        }
        private void MakingIcySnows(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Forming Snow around Ice";

            int startX = AbysmStart.X - 1000;
            int endX = AbysmStart.X + 1000;
            int startY = AbysmStart.Y - 500;
            int endY = AbysmStart.Y + 3000;

            startX = (int)MathHelper.Clamp(startX, 0, Main.maxTilesX - 100);
            endX = (int)MathHelper.Clamp(endX, 0, Main.maxTilesX - 100);
            startY = (int)MathHelper.Clamp(startY, 0, Main.maxTilesY - 100);
            endY = (int)MathHelper.Clamp(endY, 0, Main.maxTilesY - 100);

            int steps = 28000;
            for (int j = 0; j < steps; j++)
            {
                int x = WorldGen.genRand.Next(startX, endX);
                int y = WorldGen.genRand.Next(startY, endY);
                if (WorldGen.genRand.NextBool(4))
                {
                    int yBelow = y + 1;
                    Vector2 WallPosition = new Vector2(x, yBelow);
                    if (!WorldGen.SolidTile(x, yBelow))
                        continue;
                    if (Main.tile[x, yBelow].TileType == TileID.IceBlock)
                    {

                        WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(WorldGen.genRand.Next(1, 7)), Actions.Chain(new GenAction[]
                        {
                                new Actions.PlaceTile(TileID.SnowBlock),
                                new Actions.Smooth(true)
                        }));
                    }
                }

                if (WorldGen.genRand.NextBool(4))
                {
                    int yBelow = y + 1;
                    Vector2 WallPosition = new Vector2(x, yBelow);
                    if (!WorldGen.SolidTile(x, yBelow))
                        continue;

                    if (Main.tile[x, yBelow].TileType == TileID.SnowBlock ||
                        Main.tile[x, yBelow].TileType == TileID.IceBlock)
                    {

                        WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(WorldGen.genRand.Next(1, 5)), Actions.Chain(new GenAction[]
                           {
                            new Actions.PlaceWall(WallID.IceEcho),
                            new Actions.Smooth(true)
                           }));

                    }
                }
            }
        }
        private void MakingIcyRandomness(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Ice settling in the ground";




            // Select a place in the first 6th of the world, avoiding the oceans
            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 9.2f) * 6E-05); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int Y = WorldGen.genRand.Next(0, (int)Main.worldSurface);
                int yBelow = Y + 1;
                Vector2 WallPosition = new Vector2(X, yBelow);
                Vector2D WallPosition2 = new Vector2D(WorldGen.genRand.Next(-10, -2), WorldGen.genRand.Next(-20, -3));
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.SnowBlock)
                {
                    StructureMap structures = GenVars.structures;
                    Rectangle areaToPlaceIn = new Rectangle(
                        (int)WallPosition.X - 5,
                        (int)WallPosition.Y - 10,
                        10, 20);
                    if (!structures.CanPlace(areaToPlaceIn))
                        continue;

                    WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Tail(10, WallPosition2), Actions.Chain(new GenAction[]
                       {    
                           //new Actions.ClearWall(true),
                            new Actions.SetTile(TileID.IceBlock),
                            //new Actions.Smooth(true)
                       }));



                }







            }

            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int Y = WorldGen.genRand.Next(0, (int)Main.worldSurface);
                int yBelow = Y + 1;
                Vector2 WallPosition = new Vector2(X, yBelow);
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.SnowBlock)
                {
                    StructureMap structures = GenVars.structures;
                    Rectangle areaToPlaceIn = new Rectangle(
                        (int)WallPosition.X - 3,
                        (int)WallPosition.Y - 3,
                        6, 6);
                    if (!structures.CanPlace(areaToPlaceIn))
                        continue;
                    WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(WorldGen.genRand.Next(1, 3)), Actions.Chain(new GenAction[]
                       {
                            //new Actions.ClearWall(true),
                            new Actions.SetTile(TileID.IceBlock),
                            new Actions.Smooth(true)
                       }));



                }







            }

            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 9.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int Y = WorldGen.genRand.Next(0, (int)Main.worldSurface);
                int yBelow = Y + 1;
                Vector2 WallPosition = new Vector2(X, yBelow);
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.IceBlock)
                {

                    WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(WorldGen.genRand.Next(1, 3)), Actions.Chain(new GenAction[]
                       {
                            new Actions.ClearWall(true),
                            new Actions.PlaceWall(WallID.IceEcho),
                            new Actions.Smooth(true)
                       }));



                }







            }

            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 8.2f) * 6E-04); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 100);
                int yBelow = Y + 1;
                Vector2 WallPosition = new Vector2(X, yBelow);
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.SnowBlock)
                {

                    WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(WorldGen.genRand.Next(1, 4)), Actions.Chain(new GenAction[]
                       {
                            new Actions.SetTile(TileID.IceBlock),
                            new Actions.Smooth(true)
                       }));



                }







            }



        }
        private void MakingIcyUndergroundSpikes(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Ice settling in the underground";




            // Select a place in the first 6th of the world, avoiding the oceans
            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 13.2f) * 6E-05); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
                int yBelow = Y;
                Vector2 WallPosition = new Vector2(X, yBelow);
                Vector2D WallPosition2 = new Vector2D(WorldGen.genRand.Next(-40, 40), WorldGen.genRand.Next(-70, -3));
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.SnowBlock)
                {

                    WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Tail(10, WallPosition2), Actions.Chain(new GenAction[]
                       {
                           new Actions.ClearWall(true),
                            new Actions.SetTile(TileID.IceBlock),
                            //new Actions.Smooth(true)
                       }));



                }







            }

            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 9.2f) * 6E-04); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int Y = WorldGen.genRand.Next(0, Main.UnderworldLayer);
                int yBelow = Y + 1;
                Vector2 WallPosition = new Vector2(X, yBelow + 2);
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.IceBlock)
                {
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            //Start Left


                            WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(1), Actions.Chain(new GenAction[]
                 {
                            new Actions.SetTile(TileID.IceBlock),
                            //new Modifiers.Dither(.2),// Dithering
                          //  new Actions.ClearWall()

                 }));

                            //    WorldGen.PlaceWall(X, yBelow + 3, (ushort)ModContent.WallType<LargeIceyStone>());
                            break;


                        case 1:
                            //Start Right


                            WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(3), Actions.Chain(new GenAction[]
                 {
                            new Actions.SetTile(TileID.IceBlock),
                            new Actions.ClearWall()
                            //new Modifiers.Dither(.2),// Dithering
                            }));

                            //   WorldGen.PlaceWall(X, yBelow + 1, (ushort)ModContent.WallType<MediumIceyStone>());
                            break;


                    }






                }







            }


        }
        private void SurfaceIceHouses(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "The frozen folk making village homes";

            StructureMap circleStructures = new StructureMap();
            for (int k = 0; k < 5; k++)
            {
                bool placed = false;
                int attempts = 0;
                while (!placed && attempts++ < 10000000)
                {
                    // Select a place in the first 6th of the world, avoiding the oceans
                    int smx = WorldGen.genRand.Next(1000, Main.maxTilesX - 1000); // from 50 since there's a unaccessible area at the world's borders
                                                                                  // 50% of choosing the last 6th of the world
                                                                                  // Choose which side of the world to be on randomly
                    ///if (WorldGen.genRand.NextBool())
                    ///{
                    ///	towerX = Main.maxTilesX - towerX;
                    ///}

                    //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                    int smy = (int)GenVars.worldSurfaceHigh - 500;

                    // We go down until we hit a solid tile or go under the world's surface
                    Tile tile = Main.tile[smx, smy];

                    while (!WorldGen.SolidTile(smx, smy) && smy <= Main.UnderworldLayer
                        || !(tile.TileType == TileID.SnowBlock) && !(tile.TileType == ModContent.TileType<RunicIceCathedralTile>()) && WorldGen.SolidTile(smx, smy))
                    {
                        smy++;
                        tile = Main.tile[smx, smy];
                    }

                    // If we went under the world's surface, try again
                    if (smy > Main.worldSurface + 500)
                    {
                        continue;
                    }

                    // If the type of the tile we are placing the tower on doesn't match what we want, try again



                    // place the Rogue
                    //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                    //Main.npc[num].homeTileX = -1;
                    //	Main.npc[num].homeTileY = -1;
                    //	Main.npc[num].direction = 1;
                    //	Main.npc[num].homeless = true;
                    if (Main.tile[smx, smy].TileType == TileID.SnowBlock || Main.tile[smx, smy].TileType == ModContent.TileType<RunicIceCathedralTile>())
                    {
                        Vector2 WallPosition = new Vector2(smx + 8, smy + 1);

                        if (!WorldGen.SolidTile(smx, smy))
                            continue;

                        if (Main.tile[smx, smy].TileType == TileID.SnowBlock || Main.tile[smx, smy].TileType == ModContent.TileType<RunicIceCathedralTile>())
                        {

                            WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.Circle(12), Actions.Chain(new GenAction[]
                               {

                            new Actions.SetTile(TileID.SnowBlock)
                                   //new Actions.Smooth(true)
                               }));

                            Rectangle areaToPlaceIn = new Rectangle(
                                (int)WallPosition.X - 12,
                                (int)WallPosition.Y - 12,
                                24, 24);
                            bool success = circleStructures.CanPlace(areaToPlaceIn);
                            if (!success)
                                continue;
                            circleStructures.AddProtectedStructure(areaToPlaceIn);
                        }



                        switch (Main.rand.Next(2))
                        {
                            case 0:
                                //Start Left
                                for (int da = 0; da < 1; da++)
                                {
                                    Point Loc = new Point(smx, smy - 5);
                                    string path = "Structures/IceStruct/HouseSurfaceIce1";//

                                    Structurizer.ProtectStructure(Loc, path);
                                    int[] ChestIndexs = Structurizer.ReadStruct(Loc, path);

                                    foreach (int chestIndex in ChestIndexs)
                                    {
                                        var chest = Main.chest[chestIndex];
                                        // etc

                                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                                        var itemsToAdd = new List<(int type, int stack)>();

                                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.

                                        /*
                                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                            Tuple.Create(ModContent.ItemType<RainforestGrassBlock>(), 0.5)


                                        // Choose no item with a high weight of 7.
                                        );
                                        if (specialItem != ItemID.None)
                                        {
                                            itemsToAdd.Add((specialItem, 1));
                                        }
                                        */
                                        // Using a switch statement and a random choice to add sets of items.
                                        switch (Main.rand.Next(4))
                                        {
                                            case 0:

                                                itemsToAdd.Add((ItemID.ClimbingClaws, Main.rand.Next(1, 1)));
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                                itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                                itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                                break;


                                            case 1:
                                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                                itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                                itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                                break;


                                            case 2:
                                                //   itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
                                                //     itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                                                //  itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));


                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                                itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                                itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                                break;


                                            case 3:

                                                itemsToAdd.Add((ItemID.ShoeSpikes, Main.rand.Next(1, 1)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                                itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                                itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                                break;





                                        }

                                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
                                        int chestItemIndex = 0;
                                        foreach (var itemToAdd in itemsToAdd)
                                        {
                                            Item item = new Item();
                                            item.SetDefaults(itemToAdd.type);
                                            item.stack = itemToAdd.stack;
                                            chest.item[chestItemIndex] = item;
                                            chestItemIndex++;
                                            if (chestItemIndex >= 40)
                                                break; // Make sure not to exceed the capacity of the chest
                                        }
                                    }












                                    // GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));



                                }
                                break;
                            case 1:
                                for (int da = 0; da < 1; da++)
                                {
                                    Point Loc = new Point(smx, smy - 8);
                                    string path = "Structures/IceStruct/HouseSurfaceIce2";//

                                    Structurizer.ProtectStructure(Loc, path);
                                    int[] ChestIndexs = Structurizer.ReadStruct(Loc, path);

                                    foreach (int chestIndex in ChestIndexs)
                                    {
                                        var chest = Main.chest[chestIndex];
                                        // etc

                                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                                        var itemsToAdd = new List<(int type, int stack)>();

                                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                                        /*
                                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                            Tuple.Create(ModContent.ItemType<RainforestGrassBlock>(), 0.5)


                                        // Choose no item with a high weight of 7.
                                        );
                                        if (specialItem != ItemID.None)
                                        {
                                            itemsToAdd.Add((specialItem, 1));
                                        }

                                        */
                                        // Using a switch statement and a random choice to add sets of items.
                                        switch (Main.rand.Next(4))
                                        {
                                            case 0:

                                                itemsToAdd.Add((ItemID.ClimbingClaws, Main.rand.Next(1, 1)));
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                                itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                                itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                                break;


                                            case 1:
                                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                                itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                                itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                                break;


                                            case 2:
                                                //   itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
                                                //     itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                                                //  itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));


                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                                itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                                itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                                break;


                                            case 3:

                                                itemsToAdd.Add((ItemID.ShoeSpikes, Main.rand.Next(1, 1)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                                itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                                itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                                break;





                                        }

                                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
                                        int chestItemIndex = 0;
                                        foreach (var itemToAdd in itemsToAdd)
                                        {
                                            Item item = new Item();
                                            item.SetDefaults(itemToAdd.type);
                                            item.stack = itemToAdd.stack;
                                            chest.item[chestItemIndex] = item;
                                            chestItemIndex++;
                                            if (chestItemIndex >= 40)
                                                break; // Make sure not to exceed the capacity of the chest
                                        }
                                    }












                                    // GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));



                                }
                                break;





                        }

                        placed = true;

                    }

                }
            }


        }
        private void InGroundIceHouses(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Little Icey people making little forts";




            // Select a place in the first 6th of the world, avoiding the oceans
            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 13.2f * 6E-07) + 9); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int Y = WorldGen.genRand.Next(0, (int)Main.worldSurface);
                int yBelow = Y + 1;
                Vector2 WallPosition = new Vector2(X, yBelow);

                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.SnowBlock)
                {

                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            //Start Left
                            for (int da = 0; da < 1; da++)
                            {
                                Point Loc = new Point(X, yBelow + 5);
                                //Structurizer.ReadStruct(Loc, "Structures/Underground/Manor", tileBlend);
                                string path = "Structures/IceStruct/HouseSurfaceIce1";//
                                int[] ChestIndexs = Structurizer.ReadStruct(Loc, path);
                                Structurizer.ProtectStructure(Loc, path);
                                foreach (int chestIndex in ChestIndexs)
                                {
                                    var chest = Main.chest[chestIndex];
                                    // etc

                                    // itemsToAdd will hold type and stack data for each item we want to add to the chest
                                    var itemsToAdd = new List<(int type, int stack)>();

                                    // Here is an example of using WeightedRandom to choose randomly with different weights for different items.

                                    /*
                                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                        Tuple.Create(ModContent.ItemType<RainforestGrassBlock>(), 0.5)


                                    // Choose no item with a high weight of 7.
                                    );
                                    if (specialItem != ItemID.None)
                                    {
                                        itemsToAdd.Add((specialItem, 1));
                                    }
                                    */
                                    // Using a switch statement and a random choice to add sets of items.
                                    switch (Main.rand.Next(4))
                                    {
                                        case 0:

                                            itemsToAdd.Add((ItemID.ClimbingClaws, Main.rand.Next(1, 1)));
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                            itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                            itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                            break;


                                        case 1:
                                            itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                            itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                            itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                            break;


                                        case 2:
                                            //   itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
                                            //     itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                                            //  itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));


                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                            itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                            itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                            break;


                                        case 3:

                                            itemsToAdd.Add((ItemID.ShoeSpikes, Main.rand.Next(1, 1)));
                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                            itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                            itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                            break;





                                    }

                                    // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
                                    int chestItemIndex = 0;
                                    foreach (var itemToAdd in itemsToAdd)
                                    {
                                        Item item = new Item();
                                        item.SetDefaults(itemToAdd.type);
                                        item.stack = itemToAdd.stack;
                                        chest.item[chestItemIndex] = item;
                                        chestItemIndex++;
                                        if (chestItemIndex >= 40)
                                            break; // Make sure not to exceed the capacity of the chest
                                    }
                                }












                                // GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));



                            }
                            break;
                        case 1:
                            for (int da = 0; da < 1; da++)
                            {
                                Point Loc = new Point(X, yBelow + 3);
                                //Structurizer.ReadStruct(Loc, "Structures/Underground/Manor", tileBlend);
                                string path = "Structures/IceStruct/HouseSurfaceIce2";//
                                int[] ChestIndexs = Structurizer.ReadStruct(Loc, path);
                                Structurizer.ProtectStructure(Loc, path);
                                foreach (int chestIndex in ChestIndexs)
                                {
                                    var chest = Main.chest[chestIndex];
                                    // etc

                                    // itemsToAdd will hold type and stack data for each item we want to add to the chest
                                    var itemsToAdd = new List<(int type, int stack)>();

                                    // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                                    /*
                                    int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                        Tuple.Create(ModContent.ItemType<RainforestGrassBlock>(), 0.5)


                                    // Choose no item with a high weight of 7.
                                    );
                                    if (specialItem != ItemID.None)
                                    {
                                        itemsToAdd.Add((specialItem, 1));
                                    }

                                    */
                                    // Using a switch statement and a random choice to add sets of items.
                                    switch (Main.rand.Next(4))
                                    {
                                        case 0:

                                            itemsToAdd.Add((ItemID.ClimbingClaws, Main.rand.Next(1, 1)));
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                            itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                            itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                            break;


                                        case 1:
                                            itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                            itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                            itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                            break;


                                        case 2:
                                            //   itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
                                            //     itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                                            //  itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));


                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                            itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                            itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                            break;


                                        case 3:

                                            itemsToAdd.Add((ItemID.ShoeSpikes, Main.rand.Next(1, 1)));
                                            itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 2)));
                                            itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 10)));
                                            itemsToAdd.Add((ItemID.Torch, Main.rand.Next(1, 100)));
                                            itemsToAdd.Add((ItemID.Rope, Main.rand.Next(10, 100)));
                                            break;





                                    }

                                    // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
                                    int chestItemIndex = 0;
                                    foreach (var itemToAdd in itemsToAdd)
                                    {
                                        Item item = new Item();
                                        item.SetDefaults(itemToAdd.type);
                                        item.stack = itemToAdd.stack;
                                        chest.item[chestItemIndex] = item;
                                        chestItemIndex++;
                                        if (chestItemIndex >= 40)
                                            break; // Make sure not to exceed the capacity of the chest
                                    }
                                }












                                // GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));



                            }
                            break;

                    }



                }







            }

        }
        private void MakingIcyPonds(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Icy waters!";




            // Select a place in the first 6th of the world, avoiding the oceans
            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 13.2f) * 6E-06); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int Y = WorldGen.genRand.Next(0, (int)Main.worldSurface);
                int yBelow = Y + 1;
                Vector2 WallPosition = new Vector2(X, yBelow);

                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.SnowBlock)
                {
                    WorldGen.digTunnel(WallPosition.X, WallPosition.Y, 0, 0, 1, 10, true);
                    WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.HalfCircle(WorldGen.genRand.Next(30) + 5), Actions.Chain(new GenAction[]
                       {
                          // new Actions.ClearWall(true),
                           new Actions.ClearTile(true),
                            new Actions.Smooth(true)
                            //new Actions.Smooth(true)
                       }));



                }







            }


            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 13.2f) * 6E-06); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
                int yBelow = Y + 1;
                Vector2 WallPosition = new Vector2(X, yBelow);

                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.IceBlock)
                {
                    WorldGen.digTunnel(WallPosition.X, WallPosition.Y, 0, 0, 1, 20, true);
                    WorldUtils.Gen(WallPosition.ToPoint(), new Shapes.HalfCircle(WorldGen.genRand.Next(15) + 5), Actions.Chain(new GenAction[]
                       {
                          // new Actions.ClearWall(true),
                           new Actions.ClearTile(true),
                            new Actions.Smooth(true)
                            //new Actions.Smooth(true)
                       }));



                }







            }

        }
        private void RuneBridges(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "The frozen folk creating bridges";


            for (int k = 0; k < 25; k++)
            {
                bool placed = false;
                int attempts = 0;
                while (!placed && attempts++ < 10000000)
                {
                    // Select a place in the first 6th of the world, avoiding the oceans
                    int smx = WorldGen.genRand.Next(1000, Main.maxTilesX - 1000); // from 50 since there's a unaccessible area at the world's borders
                                                                                  // 50% of choosing the last 6th of the world
                                                                                  // Choose which side of the world to be on randomly
                    ///if (WorldGen.genRand.NextBool())
                    ///{
                    ///	towerX = Main.maxTilesX - towerX;
                    ///}

                    //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                    int smy = (int)GenVars.worldSurfaceHigh - 500;

                    // We go down until we hit a solid tile or go under the world's surface
                    Tile tile = Main.tile[smx, smy];

                    while (!WorldGen.SolidTile(smx, smy) && smy <= Main.UnderworldLayer || !(tile.TileType == TileID.SnowBlock) && WorldGen.SolidTile(smx, smy))
                    {
                        smy++;
                        tile = Main.tile[smx, smy];
                    }

                    // If we went under the world's surface, try again
                    if (smy > Main.worldSurface + 500)
                    {
                        continue;
                    }

                    // If the type of the tile we are placing the tower on doesn't match what we want, try again



                    // place the Rogue
                    //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                    //Main.npc[num].homeTileX = -1;
                    //	Main.npc[num].homeTileY = -1;
                    //	Main.npc[num].direction = 1;
                    //	Main.npc[num].homeless = true;
                    if (Main.tile[smx, smy].TileType == TileID.SnowBlock)
                    {
                        switch (Main.rand.Next(4))
                        {
                            case 0:
                                //Start Left
                                for (int da = 0; da < 1; da++)
                                {
                                    Point Loc = new Point(smx - 15, smy + 10);
                                    //Structurizer.ReadStruct(Loc, "Structures/Underground/Manor", tileBlend);
                                    string path = "Structures/IceStruct/BridgeIce1";//


                                    int[] ChestIndexs = Structurizer.ReadStruct(Loc, path);
                                    //Structurizer.ProtectStructure(Loc, path);
                                    foreach (int chestIndex in ChestIndexs)
                                    {
                                        var chest = Main.chest[chestIndex];
                                        // etc

                                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                                        var itemsToAdd = new List<(int type, int stack)>();

                                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                            Tuple.Create(ModContent.ItemType<RainforestGrassBlock>(), 0.5)


                                        // Choose no item with a high weight of 7.
                                        );
                                        if (specialItem != ItemID.None)
                                        {
                                            itemsToAdd.Add((specialItem, 1));
                                        }
                                        // Using a switch statement and a random choice to add sets of items.
                                        switch (Main.rand.Next(5))
                                        {
                                            case 0:
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 1:
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 2:
                                                //   itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
                                                //     itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                                                //  itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 3:
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));

                                                break;
                                            case 4:
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                                break;




                                        }

                                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
                                        int chestItemIndex = 0;
                                        foreach (var itemToAdd in itemsToAdd)
                                        {
                                            Item item = new Item();
                                            item.SetDefaults(itemToAdd.type);
                                            item.stack = itemToAdd.stack;
                                            chest.item[chestItemIndex] = item;
                                            chestItemIndex++;
                                            if (chestItemIndex >= 40)
                                                break; // Make sure not to exceed the capacity of the chest
                                        }
                                    }












                                    // GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));


                                    placed = true;
                                }
                                break;
                            case 1:
                                for (int da = 0; da < 1; da++)
                                {
                                    Point Loc = new Point(smx - 20, smy + 20);
                                    //Structurizer.ReadStruct(Loc, "Structures/Underground/Manor", tileBlend);
                                    string path = "Structures/IceStruct/BridgeIce2";//


                                    int[] ChestIndexs = Structurizer.ReadStruct(Loc, path);
                                    //Structurizer.ProtectStructure(Loc, path);
                                    foreach (int chestIndex in ChestIndexs)
                                    {
                                        var chest = Main.chest[chestIndex];
                                        // etc

                                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                                        var itemsToAdd = new List<(int type, int stack)>();

                                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                            Tuple.Create(ModContent.ItemType<RainforestGrassBlock>(), 0.5)


                                        // Choose no item with a high weight of 7.
                                        );
                                        if (specialItem != ItemID.None)
                                        {
                                            itemsToAdd.Add((specialItem, 1));
                                        }
                                        // Using a switch statement and a random choice to add sets of items.
                                        switch (Main.rand.Next(5))
                                        {
                                            case 0:
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 1:
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 2:
                                                //   itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
                                                //     itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                                                //  itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 3:
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));

                                                break;
                                            case 4:
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                                break;




                                        }

                                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
                                        int chestItemIndex = 0;
                                        foreach (var itemToAdd in itemsToAdd)
                                        {
                                            Item item = new Item();
                                            item.SetDefaults(itemToAdd.type);
                                            item.stack = itemToAdd.stack;
                                            chest.item[chestItemIndex] = item;
                                            chestItemIndex++;
                                            if (chestItemIndex >= 40)
                                                break; // Make sure not to exceed the capacity of the chest
                                        }
                                    }












                                    // GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));


                                    placed = true;
                                }
                                break;
                            case 2:
                                for (int da = 0; da < 1; da++)
                                {
                                    Point Loc = new Point(smx - 15, smy + 10);
                                    //Structurizer.ReadStruct(Loc, "Structures/Underground/Manor", tileBlend);
                                    string path = "Structures/IceStruct/BridgeIce3";//

                                    int[] ChestIndexs = Structurizer.ReadStruct(Loc, path);
                                    //Structurizer.ProtectStructure(Loc, path);
                                    foreach (int chestIndex in ChestIndexs)
                                    {
                                        var chest = Main.chest[chestIndex];
                                        // etc

                                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                                        var itemsToAdd = new List<(int type, int stack)>();

                                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                            Tuple.Create(ModContent.ItemType<RainforestGrassBlock>(), 0.5)


                                        // Choose no item with a high weight of 7.
                                        );
                                        if (specialItem != ItemID.None)
                                        {
                                            itemsToAdd.Add((specialItem, 1));
                                        }
                                        // Using a switch statement and a random choice to add sets of items.
                                        switch (Main.rand.Next(5))
                                        {
                                            case 0:
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 1:
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 2:
                                                //   itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
                                                //     itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                                                //  itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 3:
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));

                                                break;
                                            case 4:
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                                break;




                                        }

                                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
                                        int chestItemIndex = 0;
                                        foreach (var itemToAdd in itemsToAdd)
                                        {
                                            Item item = new Item();
                                            item.SetDefaults(itemToAdd.type);
                                            item.stack = itemToAdd.stack;
                                            chest.item[chestItemIndex] = item;
                                            chestItemIndex++;
                                            if (chestItemIndex >= 40)
                                                break; // Make sure not to exceed the capacity of the chest
                                        }
                                    }












                                    // GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));


                                    placed = true;
                                }
                                break;
                            case 3:
                                for (int da = 0; da < 1; da++)
                                {
                                    Point Loc = new Point(smx - 20, smy + 10);
                                    //Structurizer.ReadStruct(Loc, "Structures/Underground/Manor", tileBlend);
                                    string path = "Structures/IceStruct/BridgeIce3";//

                                    int[] ChestIndexs = Structurizer.ReadStruct(Loc, path);
                                    //Structurizer.ProtectStructure(Loc, path);
                                    foreach (int chestIndex in ChestIndexs)
                                    {
                                        var chest = Main.chest[chestIndex];
                                        // etc

                                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                                        var itemsToAdd = new List<(int type, int stack)>();

                                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                                            Tuple.Create(ModContent.ItemType<RainforestGrassBlock>(), 0.5)


                                        // Choose no item with a high weight of 7.
                                        );
                                        if (specialItem != ItemID.None)
                                        {
                                            itemsToAdd.Add((specialItem, 1));
                                        }
                                        // Using a switch statement and a random choice to add sets of items.
                                        switch (Main.rand.Next(5))
                                        {
                                            case 0:
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 1:
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 2:
                                                //   itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
                                                //     itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                                                //  itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 3:
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));

                                                break;
                                            case 4:
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                                break;




                                        }

                                        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
                                        int chestItemIndex = 0;
                                        foreach (var itemToAdd in itemsToAdd)
                                        {
                                            Item item = new Item();
                                            item.SetDefaults(itemToAdd.type);
                                            item.stack = itemToAdd.stack;
                                            chest.item[chestItemIndex] = item;
                                            chestItemIndex++;
                                            if (chestItemIndex >= 40)
                                                break; // Make sure not to exceed the capacity of the chest
                                        }
                                    }












                                    // GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));


                                    placed = true;
                                }
                                break;
                        }

                    }

                }
            }
        }
        private void IceyCaves(GenerationProgress progress, GameConfiguration configuration)
        {


            for (int id = 0; id < 10; id++)
            {
                bool placed = false;
                int attempts = 0;
                while (!placed && attempts++ < 1000000)
                {
                    // Select a place in the first 6th of the world, avoiding the oceans
                    int smx = WorldGen.genRand.Next(1000, Main.maxTilesX - 1000); // from 50 since there's a unaccessible area at the world's borders
                                                                                  // 50% of choosing the last 6th of the world
                                                                                  // Choose which side of the world to be on randomly
                    ///if (WorldGen.genRand.NextBool())
                    ///{
                    ///	towerX = Main.maxTilesX - towerX;
                    ///}

                    //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                    int smy = (int)GenVars.worldSurfaceHigh - 500;

                    // We go down until we hit a solid tile or go under the world's surface
                    Tile tile = Main.tile[smx, smy];

                    while (!WorldGen.SolidTile(smx, smy) && smy <= Main.UnderworldLayer || !(tile.TileType == TileID.SnowBlock) && WorldGen.SolidTile(smx, smy))
                    {
                        smy++;
                        tile = Main.tile[smx, smy];
                    }

                    // If we went under the world's surface, try again
                    if (smy > Main.worldSurface + 500)
                    {
                        continue;
                    }

                    // If the type of the tile we are placing the tower on doesn't match what we want, try again



                    // place the Rogue
                    //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                    //Main.npc[num].homeTileX = -1;
                    //	Main.npc[num].homeTileY = -1;
                    //	Main.npc[num].direction = 1;
                    //	Main.npc[num].homeless = true;
                    if (Main.tile[smx, smy].TileType == TileID.SnowBlock)
                    {
                        int caveWidth = WorldGen.genRand.Next(2, 4 + id); // Width
                        int caveSteps = WorldGen.genRand.Next(50, 120 + id); // How many carves

                        int caveSeed = WorldGen.genRand.Next();
                        Vector2 baseCaveDirection = Vector2.UnitY.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);
                        Vector2 cavePosition = new Vector2(smx, smy);

                        for (int j = 0; j < caveSteps; j++)
                        {
                            float caveOffsetAngleAtStep = WorldMath.PerlinNoise2D(1 / 50f, j / 50f, 4, caveSeed) * MathHelper.Pi * 1.9f;
                            Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

                            // Carve out at the current position.
                            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                            {
                                //digging 

                                WorldGen.digTunnel(cavePosition.X, cavePosition.Y, caveDirection.X, caveDirection.Y, 1, (int)(caveWidth * 1.18f), false);
                                WorldUtils.Gen(cavePosition.ToPoint(), new Shapes.Circle(caveWidth), Actions.Chain(new GenAction[]
                                {
                                     new Actions.ClearTile(true),
                                     new Actions.Smooth(true)
                                }));
                            }

                            // Update the cave position.
                            cavePosition += caveDirection * caveWidth;
                        }
                        placed = true;

                    }



                }
            }


        }
        private void IceyUndergroundCaves(GenerationProgress progress, GameConfiguration configuration)
        {

            var genRand = WorldGen.genRand;
            for (int id = 0; id < WorldGen.genRand.Next(150, 200); id++)
            {
                bool placed = false;
                int attempts = 0;
                while (!placed && attempts++ < 1000000)
                {
                    // Select a place in the first 6th of the world, avoiding the oceans
                    int smx = WorldGen.genRand.Next(1000, Main.maxTilesX - 1000); // from 50 since there's a unaccessible area at the world's borders
                                                                                  // 50% of choosing the last 6th of the world
                                                                                  //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                    int smy = (int)GenVars.worldSurfaceLow + genRand.Next(0, 2500);
                    smy = Math.Clamp(smy, 0, Main.maxTilesY - 100);

                    // We go down until we hit a solid tile or go under the world's surface
                    Tile tile = Main.tile[smx, smy];

                    if (!WorldGen.SolidTile(smx, smy) ||
                        tile.TileType != TileID.IceBlock && tile.TileType != TileID.SnowBlock)
                    {
                        continue;
                    }


                    if (genRand.NextBool(25))
                    {
                        //Make Open Area
                        for (int i = 0; i < 2; i++)
                        {
                            WorldGen.Caverer(smx, smy);
                        }

                        int caveWidth = genRand.Next(6, 10); // Width
                        int caveSteps = genRand.Next(30, 50); // How many carves

                        int caveSeed = genRand.Next();
                        Vector2 baseCaveDirection = Vector2.UnitY.RotatedBy(genRand.NextFloatDirection() * 0.54f);
                        Vector2 cavePosition = new Vector2(smx, smy);

                        for (int j = 0; j < caveSteps; j++)
                        {
                            float caveOffsetAngleAtStep = WorldMath.PerlinNoise2D(1 / 50f, j / 50f, 4, caveSeed) * MathHelper.TwoPi * 1.9f;
                            Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

                            // Carve out at the current position.
                            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                            {
                                //digging 
                                WorldGen.TileRunner(
                                    (int)cavePosition.X,
                                    (int)cavePosition.Y,
                                    genRand.NextFloat(15, 20),
                                    genRand.Next(5, 10), -1);
                            }

                            // Update the cave position.
                            cavePosition += caveDirection * caveWidth * 0.5f;
                        }
                    }
                    else
                    {
                        // If the type of the tile we are placing the tower on doesn't match what we want, try again
                        int caveWidth = genRand.Next(2, 6); // Width
                        int caveSteps = genRand.Next(200, 500); // How many carves

                        int caveSeed = genRand.Next();
                        Vector2 baseCaveDirection = Vector2.UnitY.RotatedBy(genRand.NextFloatDirection() * 0.54f);
                        Vector2 cavePosition = new Vector2(smx, smy);

                        for (int j = 0; j < caveSteps; j++)
                        {
                            float caveOffsetAngleAtStep = WorldMath.PerlinNoise2D(1 / 50f, j / 50f, 4, caveSeed) * MathHelper.Pi * 1.9f;
                            Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

                            // Carve out at the current position.
                            if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                            {
                                //digging 
                                WorldGen.TileRunner(
                                    (int)cavePosition.X,
                                    (int)cavePosition.Y,
                                    genRand.NextFloat(6, 11),
                                    genRand.Next(2, 5), -1);
                            }

                            // Update the cave position.
                            cavePosition += caveDirection * caveWidth * 0.5f;
                        }
                    }

                    placed = true;
                    /*
                    int caveWidth = WorldGen.genRand.Next(1, 6); // Width
                    int caveSteps = WorldGen.genRand.Next(200, 1500 + id); // How many carves

                    int caveSeed = WorldGen.genRand.Next();
                    Vector2 baseCaveDirection = Vector2.UnitY.RotatedBy(WorldGen.genRand.NextFloatDirection() * 0.54f);
                    Vector2 cavePosition = new Vector2(smx, smy + 10);

                    for (int j = 0; j < caveSteps; j++)
                    {
                        float caveOffsetAngleAtStep = WorldMath.PerlinNoise2D(1 / 50f, j / 50f, 4, caveSeed) * MathHelper.Pi * 1.9f;
                        Vector2 caveDirection = baseCaveDirection.RotatedBy(caveOffsetAngleAtStep);

                        // Carve out at the current position.
                        if (cavePosition.X < Main.maxTilesX - 15 && cavePosition.X >= 15)
                        {
                            //digging 

                            WorldGen.digTunnel(cavePosition.X, cavePosition.Y, caveDirection.X, caveDirection.Y, 1, (int)(caveWidth * 1.18f), false);
                            WorldUtils.Gen(cavePosition.ToPoint(), new Shapes.Circle(caveWidth), Actions.Chain(new GenAction[]
                            {
                                 new Actions.ClearTile(true)
                            }));
                        }

                        // Update the cave position.
                        cavePosition += caveDirection * caveWidth;
                    }*/



                }
            }


        }

        private void IceCrystalsSpawning(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Icy Crystals!";
            for (int k = 60; k < Main.maxTilesX - 60; k++)
            {
                if (k > 200 && k < Main.maxTilesX - 200 && WorldGen.genRand.NextBool(2)) //inner part of the world
                {
                    for (int y = 10; y < Main.UnderworldLayer; y++)
                    {
                        if (IsGroundIce(k, y, 8))
                        {
                            PlaceIcyCrystals(k, y, 1);
                            k += 1;

                            break;
                        }

                        if (!VeilGen.IsAir(k, y, 8))
                            break;
                    }
                }
            }


        }
        private void BorealTreeSpawning(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Icy trees!";
            for (int k = 60; k < Main.maxTilesX - 60; k++)
            {
                if (k > 200 && k < Main.maxTilesX - 200 && WorldGen.genRand.NextBool(1)) //inner part of the world
                {
                    for (int y = 10; y < Main.worldSurface; y++)
                    {
                        if (IsGroundSnow(k, y, 1))
                        {
                            PlaceBorealTrees(k, y, Main.rand.Next(1, 1));
                            k += 1;

                            break;
                        }

                        if (!VeilGen.IsAir(k, y, 2))
                            break;
                    }
                }
            }


        }
        private void IceFog(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "The fog is cuming";
            for (int k = 60; k < Main.maxTilesX - 60; k++)
            {
                if (k > 200 && k < Main.maxTilesX - 200 && WorldGen.genRand.NextBool(1)) //inner part of the world
                {
                    for (int y = 10; y < Main.worldSurface; y++)
                    {
                        if (IsGroundFoggy(k, y, 1))
                        {
                            PlaceTheFog(k, y, Main.rand.Next(1, 1));
                            k += 1;

                            break;
                        }

                        if (!VeilGen.IsAir(k, y, 2))
                            break;
                    }
                }

                if (k > 200 && k < Main.maxTilesX - 200 && WorldGen.genRand.NextBool(1)) //inner part of the world
                {
                    for (int y = 10; y < Main.worldSurface; y++)
                    {
                        if (IsGroundIceyFog(k, y, 1))
                        {
                            PlaceTheFog(k, y, Main.rand.Next(1, 1));
                            k += 1;

                            break;
                        }

                        if (!VeilGen.IsAir(k, y, 2))
                            break;
                    }
                }
            }
        }

        public static bool IsGroundIceyFog(int x, int y, int w)
        {
            for (int k = 0; k < w; k++)
            {
                Tile tile = Framing.GetTileSafely(x + k, y);
                if (!(tile.HasTile && tile.Slope == SlopeType.Solid && !tile.IsHalfBlock && tile.TileType == ModContent.TileType<RunicIceCathedralTile>()))
                    return false;

                Tile tile2 = Framing.GetTileSafely(x + k, y - 1);
                if (tile2.HasTile && Main.tileSolid[tile2.TileType])
                    return false;
            }

            return true;
        }

        public static bool IsGroundSnow(int x, int y, int w)
        {
            for (int k = 0; k < w; k++)
            {
                Tile tile = Framing.GetTileSafely(x + k, y);
                if (!(tile.HasTile && tile.Slope == SlopeType.Solid && !tile.IsHalfBlock && tile.TileType == TileID.SnowBlock))
                    return false;

                Tile tile2 = Framing.GetTileSafely(x + k, y - 1);
                if (tile2.HasTile && Main.tileSolid[tile2.TileType])
                    return false;
            }

            return true;
        }
        public static bool IsGroundIce(int x, int y, int w)
        {
            for (int k = 0; k < w; k++)
            {
                Tile tile = Framing.GetTileSafely(x + k, y);
                if (!(tile.HasTile && tile.Slope == SlopeType.Solid && !tile.IsHalfBlock && tile.TileType == ModContent.TileType<RunicIceCathedralTile>()))
                    return false;

                Tile tile2 = Framing.GetTileSafely(x + k, y - 1);
                if (tile2.HasTile && Main.tileSolid[tile2.TileType])
                    return false;
            }

            return true;
        }
        public static bool IsGroundFoggy(int x, int y, int w)
        {
            for (int k = 0; k < w; k++)
            {
                Tile tile = Framing.GetTileSafely(x + k, y);
                if (!(tile.HasTile && tile.Slope == SlopeType.Solid && !tile.IsHalfBlock && tile.TileType == TileID.IceBlock))
                    return false;

                Tile tile2 = Framing.GetTileSafely(x + k, y - 1);
                if (tile2.HasTile && Main.tileSolid[tile2.TileType])
                    return false;
            }

            return true;
        }
        public static void PlaceIcyCrystals(int treex, int treey, int height)
        {
            treey -= 1;

            if (treey - height < 1)
                return;

            for (int x = -1; x < 3; x++)
            {
                for (int y = 0; y < height + 2; y++)
                {
                    WorldGen.KillWall(treex + x, treey - y);
                }
            }

            // MultitileHelper.PlaceMultitile(new Point16(treex, treey - 1), ModContent.TileType<RainforestTreeBase>());
            switch (Main.rand.Next(3))
            {
                case 0:
                    //Start Left
                    WorldGen.PlaceWall(treex + 3, treey + 3, (ushort)ModContent.WallType<LargeIceyStone>());
                    break;

                case 1:
                    //Start Right
                    WorldGen.PlaceWall(treex + 3, treey + 2, (ushort)ModContent.WallType<MediumIceyStone>());
                    break;

                case 2:
                    //Start Right
                    WorldGen.PlaceWall(treex + 3, treey + 2, (ushort)ModContent.WallType<SmallIceyStone>());
                    break;
            }


            for (int x = -1; x < 3; x++)
            {
                for (int y = 0; y < height + 2; y++)
                {
                    WorldGen.TileFrame(treex + x, treey + y);
                }
            }
        }
        public static void PlaceBorealTrees(int treex, int treey, int height)
        {
            treey -= 1;

            if (treey - height < 1)
                return;

            for (int x = -1; x < 3; x++)
            {
                for (int y = 0; y < height + 2; y++)
                {
                    WorldGen.KillTile(treex + x, treey - y);
                }
            }

            // MultitileHelper.PlaceMultitile(new Point16(treex, treey - 1), ModContent.TileType<RainforestTreeBase>());

            for (int x = 0; x < 1; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    WorldGen.PlaceTile(treex + x, treey - y, TileID.Saplings, true, true);
                    WorldGen.GrowTree(treex + x, treey - y);
                }
            }

            for (int x = -1; x < 3; x++)
            {
                for (int y = 0; y < height + 2; y++)
                {
                    WorldGen.TileFrame(treex + x, treey + y);
                }
            }
        }
        public static void PlaceTheFog(int treex, int treey, int height)
        {
            treey -= 1;

            if (treey - height < 1)
                return;

            for (int x = -1; x < 3; x++)
            {
                for (int y = 0; y < height + 2; y++)
                {
                    WorldGen.KillTile(treex + x, treey - y);
                }
            }

            // MultitileHelper.PlaceMultitile(new Point16(treex, treey - 1), ModContent.TileType<RainforestTreeBase>());

            for (int x = 0; x < 1; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Point Loc = new Point(treex + x, treey - y);
                    //Structurizer.ReadStruct(Loc, "Structures/Underground/Manor", tileBlend);
                    string path = "Structures/IceStruct/FogSpammer";//
                    Structurizer.ReadStruct(Loc, path);
                    Structurizer.ProtectStructure(Loc, path);
                }
            }

            for (int x = -1; x < 3; x++)
            {
                for (int y = 0; y < height + 2; y++)
                {
                    WorldGen.TileFrame(treex + x, treey + y);
                }
            }
        }


        #endregion
    }
}
