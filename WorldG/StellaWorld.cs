using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.AlcadChests;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Armors.Alcalite;
using Stellamod.Items.Armors.Stone;
using Stellamod.Items.Armors.Windmillion;
using Stellamod.Items.Consumables;
using Stellamod.Items.Flasks;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.Items.Placeable;
using Stellamod.Items.Quest.Merena;
using Stellamod.Items.Special;
using Stellamod.Items.Special.MinerLogs;
using Stellamod.Items.Tools;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Melee.Greatswords;
using Stellamod.Items.Weapons.Melee.Spears;
using Stellamod.Items.Weapons.Melee.Yoyos;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Ranged.Crossbows;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.Items.Weapons.Whips;
using Stellamod.Tiles;
using Stellamod.Tiles.Abyss;
using Stellamod.Tiles.Acid;
using Stellamod.Tiles.Illuria;
using Stellamod.WorldG.StructureManager;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;


namespace Stellamod.WorldG
{


    public class StellaWorld : ModSystem
	{
		private void DisableGenTask(List<GenPass> tasks, string passName)
		{
            tasks.Find(x => x.Name.Equals(passName)).Disable();
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			int MorrowGen = tasks.FindIndex(genpass => genpass.Name.Equals("Lakes"));
            int RoyalGen = tasks.FindIndex(genpass => genpass.Name.Equals("Underworld"));

            //Disable Some Passes
            DisableGenTask(tasks, "Terrain");
            DisableGenTask(tasks, "Tunnels");
            DisableGenTask(tasks, "Mount Caves");
            DisableGenTask(tasks, "Surface Caves");
            DisableGenTask(tasks, "Mountain Caves");
          //  DisableGenTask(tasks, "Surface Chests");
            DisableGenTask(tasks, "Wavy Caves");
            DisableGenTask(tasks, "Living Trees");

            tasks[tasks.FindIndex(x => x.Name.Equals("Terrain"))] = new VanillaTerrainPass();




            if (RoyalGen != -1)
            {

                tasks.Insert(RoyalGen + 1, new PassLegacy("World Gen Royal Castle", WorldGenRoyalCapital));
            }
            //Replace Terrain Pass
            
            if (MorrowGen != -1)
			{
				tasks.Insert(MorrowGen + 1, new PassLegacy("World Gen Abysm", WorldGenAbysm));
                tasks.Insert(MorrowGen + 2, new PassLegacy("World Gen Abysm Caves", NewCaveFormationAbysm));
                tasks.Insert(MorrowGen + 3, new PassLegacy("World Gen Virulent", WorldGenVirulent));	
				tasks.Insert(MorrowGen + 4, new PassLegacy("World Gen Other stones", WorldGenDarkstone));
				tasks.Insert(MorrowGen + 5, new PassLegacy("World Gen Ice Ores", WorldGenFrileOre));
				tasks.Insert(MorrowGen + 6, new PassLegacy("World Gen Flame Ores", WorldGenFlameOre));
				tasks.Insert(MorrowGen + 7, new PassLegacy("World Gen Illuria", WorldGenIlluria));
				tasks.Insert(MorrowGen + 8, new PassLegacy("World Gen Cinderspark", WorldGenCinderspark));
				tasks.Insert(MorrowGen + 9, new PassLegacy("World Gen Cinderspark", WorldGenMoreFlameOre));
				tasks.Insert(MorrowGen + 10, new PassLegacy("World Gen Ice Ores", WorldGenFrileOre));
				tasks.Insert(MorrowGen + 11, new PassLegacy("World Gen Veiled Spot", WorldGenVeilSpot));
                tasks.Insert(MorrowGen + 12, new PassLegacy("World Gen Dungeon Location", WorldGenDungeonLocation));
            }

			int CathedralGen3 = tasks.FindIndex(genpass => genpass.Name.Equals("Buried Chests"));
			if (CathedralGen3 != -1)
			{
				tasks.Insert(CathedralGen3 + 1, new PassLegacy("World Gen Ambience", WorldGenAmbience));
			}
				
			int CathedralGen2 = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
			if (CathedralGen2 != -1)
			{

                //tasks.Insert(CathedralGen2 + 1, new PassLegacy("World Gen Virulent Structures", WorldGenVirulentStructures));
                //	tasks.Insert(CathedralGen2 + 1, new PassLegacy("World Gen Virulent", WorldGenVirulent));

                tasks.Insert(CathedralGen2 + 1, new PassLegacy("World Gen Abandoned Mineshafts", WorldGenAbandonedMineshafts));
                tasks.Insert(CathedralGen2 + 2, new PassLegacy("World Gen AureTemple", WorldGenAurelusTemple));
				tasks.Insert(CathedralGen2 + 3, new PassLegacy("World Gen Fable", WorldGenFabiliaRuin));
				tasks.Insert(CathedralGen2 + 4, new PassLegacy("World Gen More skies", WorldGenBig));
				tasks.Insert(CathedralGen2 +5, new PassLegacy("World Gen More skies", WorldGenMed));
				tasks.Insert(CathedralGen2 + 6, new PassLegacy("World Gen Virulent Structures", WorldGenVirulentStructures));
				tasks.Insert(CathedralGen2 + 7, new PassLegacy("World Gen Govheil Castle", WorldGenGovheilCastle));
				tasks.Insert(CathedralGen2 + 8, new PassLegacy("World Gen Stone Castle", WorldGenStoneCastle));
				tasks.Insert(CathedralGen2 + 9, new PassLegacy("World Gen Veil Underground", WorldGenVU));
				tasks.Insert(CathedralGen2 + 10, new PassLegacy("World Gen Veldris", WorldGenVeldris));
				tasks.Insert(CathedralGen2 + 11, new PassLegacy("World Gen Cathedral", WorldGenCathedral));
				tasks.Insert(CathedralGen2 + 12, new PassLegacy("World Gen Underworld rework", WorldGenUnderworldSpice));
				tasks.Insert(CathedralGen2 + 13, new PassLegacy("World Gen Rallad", WorldGenRallad));
				tasks.Insert(CathedralGen2 + 14, new PassLegacy("World Gen Xix Village", WorldGenXixVillage));
				tasks.Insert(CathedralGen2 + 15, new PassLegacy("World Gen Windmills Village", WorldGenWindmills));
				tasks.Insert(CathedralGen2 + 16, new PassLegacy("World Gen Manor", WorldGenManor));
				tasks.Insert(CathedralGen2 + 17, new PassLegacy("World Gen Mechanic spot", WorldGenMechShop));
				tasks.Insert(CathedralGen2 + 18, new PassLegacy("World Gen Gia's House", WorldGenGiaHouse));
                tasks.Insert(CathedralGen2 + 19, new PassLegacy("World Gen Worshiping Towers", WorldGenWorshipingTowers));
				tasks.Insert(CathedralGen2 + 20, new PassLegacy("World Gen Bridget", WorldGenFabledTrees));
                tasks.Insert(CathedralGen2 + 21, new PassLegacy("World Gen Graving", WorldGenGraving));
                tasks.Insert(CathedralGen2 + 22, new PassLegacy("World Gen Sunstalker", WorldGenStalker));
                tasks.Insert(CathedralGen2 + 23, new PassLegacy("World Gen Blood Catherdal", WorldGenBloodCathedral));
            }
		}


       

        private void WorldGenDungeonLocation(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Moving the dungeon, smh";

			//GenVars.dungeonLocation is the x value of the dungeon
			GenVars.dungeonLocation = Main.maxTilesX - 1000;
        }

        private void WorldGenFabledTrees(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "The Veiled people planting trees!";
			for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.4f) * 6E-02); k++)
			{
				int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
				int Y = WorldGen.genRand.Next(0, Main.UnderworldLayer);
				int yBelow = Y + 1;
				if (!WorldGen.SolidTile(X, yBelow))
					continue;

				if (Main.tile[X, yBelow].TileType == ModContent.TileType<CatagrassBlock>())
				{
					WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Fable.FableTreeSapling>());
				}
			}
		}

		private void WorldGenAmbience(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Golden Ambience ruining the world";
			for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next(0, Main.UnderworldLayer);
                int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.Dirt)
                {
                    WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.OwlTrunck1>());               
                }
            }

            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next(0, Main.UnderworldLayer);
                int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.Dirt)
                {
                    WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.OwlTrunck2>());                 
                }
            }

            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next(0, Main.UnderworldLayer);
                int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.Dirt)
                {
                    WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.OwlTrunck3>());   
                }
            }


            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 9.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
                int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.Stone || 
					Main.tile[X, yBelow].TileType == TileID.ClayBlock)
                {
                    WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.BigRock1>());
                }
            }

            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 9.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
                int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.Stone || 
					Main.tile[X, yBelow].TileType == TileID.ClayBlock)
                {
                    WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.BigRock2>());   
                }
            }

            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 9.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
                int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.Stone || 
					Main.tile[X, yBelow].TileType == TileID.ClayBlock)
                {
                    WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.BigRock3>());   
                }
            }

            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 9.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
                int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.Stone || 
					Main.tile[X, yBelow].TileType == TileID.ClayBlock)
                {
                    WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.BigRock4>());
                }
            }

            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
                int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.Stone || 
					Main.tile[X, yBelow].TileType == TileID.ClayBlock)
                {
                    WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.Stalagmite1>());            
                }
            }

            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
                int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.Stone || 
					Main.tile[X, yBelow].TileType == TileID.ClayBlock)
                {
                    WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.Stalagmite2>());       
                }
            }

            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
                int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.Stone || Main.tile[X, yBelow].TileType == TileID.ClayBlock)
                {
                    WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.Stalagmite3>());
                }
            }

			for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
			{
				int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
				int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
                int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.Stone || Main.tile[X, yBelow].TileType == TileID.ClayBlock)
				{
					WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.Mushroom3>());
				}
			}

			for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
			{
				int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
				int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
                int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.Stone || Main.tile[X, yBelow].TileType == TileID.ClayBlock)
				{
					WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.Mushroom2>());
				}
			}

			for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
			{
				int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
				int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
                int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.Stone || 
					Main.tile[X, yBelow].TileType == TileID.ClayBlock)
				{
					WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.Mushroom1>());
				}
			}

			for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.UnderworldLayer);
                int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.Stone || 
					Main.tile[X, yBelow].TileType == TileID.ClayBlock)
                {
                    WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.Stalagmite4>());
                }
            }

            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int Y = WorldGen.genRand.Next(0, Main.UnderworldLayer);
                int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.Mud ||
                    Main.tile[X, yBelow].TileType == TileID.JungleGrass)
                {
                    WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Structures.LogS>());
                }
            }
			//

            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.2f) * 6E-03); k++)
			{
				int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
				int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY / 2);
				int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.Dirt)
				{
					WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.TreeOver1>());
				}
			}

			for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.2f) * 6E-03); k++)
			{
				int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
				int Y = WorldGen.genRand.Next(0, Main.UnderworldLayer);
                int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.Dirt)
				{
					WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.TreeOver2>());
					
				}
			}

			//Purple Tree

            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next(0, Main.UnderworldLayer);
                int yBelow = Y + 1;
                if (!WorldGen.SolidTile(X, yBelow))
                    continue;

                if (Main.tile[X, yBelow].TileType == TileID.Dirt)
                {
                    WorldGen.PlaceObject(X, Y, ModContent.TileType<Tiles.Ambient.TreeOver3>());

                }
            }



        }
        Point pointVeri;
		Point pointAlcadthingy;
		private void WorldGenFabiliaRuin(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Buring the landscape with Cinder and Fable";

			
			int[] tileBlend = new int[]
			{
				TileID.RubyGemspark
			};

			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 10000000)
			{
				// Select a place in the first 6th of the world, avoiding the oceans
				int smx = WorldGen.genRand.Next(((Main.maxTilesX) / 2) + 50, (Main.maxTilesX / 2) + 200); // from 50 since there's a unaccessible area at the world's borders
																								 // 50% of choosing the last 6th of the world
																								 // Choose which side of the world to be on randomly
				///if (WorldGen.genRand.NextBool())
				///{
				///	towerX = Main.maxTilesX - towerX;
				///}

				//Start at 200 tiles above the surface instead of 0, to exclude floating islands
				int smy = ((int)(Main.worldSurface - 250));

				// We go down until we hit a solid tile or go under the world's surface
				while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
				{
					smy++;
				}

				// If we went under the world's surface, try again
				if (smy > Main.worldSurface - 20)
				{
					continue;
				}
				Tile tile = Main.tile[smx, smy];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == TileID.Dirt
                    || tile.TileType == TileID.Stone
                    || tile.TileType == TileID.Grass))
                {
                    continue;
                }



                // place the Rogue
                //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                //Main.npc[num].homeTileX = -1;
                //	Main.npc[num].homeTileY = -1;
                //	Main.npc[num].direction = 1;
                //	Main.npc[num].homeless = true;



                for (int da = 0; da < 1; da++)
				{
					Point Loc = new Point(smx + 10, smy + 53);
					NPCs.Town.AlcadSpawnSystem.FableTile = Loc;
                    Structurizer.ReadStruct(Loc, "Struct/Huntria/FableBiomeFinal", tileBlend);
                    Structurizer.ProtectStructure(Loc, "Struct/Huntria/FableBiomeFinal");
                    //This code just places
                    int width = 253;
					int height = 50;
                    //ShapeData shapeData = new ShapeData();
					//Point dirtLoc = Loc;
					//dirtLoc.Y -= 338;
                    //WorldUtils.Gen(dirtLoc, new Shapes.Rectangle(width, height), new Actions.Blank().Output(shapeData));
                    //WorldUtils.Gen(dirtLoc, new ModShapes.All(shapeData), new Actions.SetTile(TileID.Dirt, true));
                    //WorldUtils.Gen(dirtLoc, new ModShapes.All(shapeData), new Actions.Smooth());

                  

					//Point Loc2 = new Point(smx + 10, smy + 380);
					//WorldGen.digTunnel(Loc2.X - 10, Loc2.Y + 10, 1, 0, 1, 10, false);
					
					//Point Loc22 = new Point(smx +10, smy - 33);
		//			WorldUtils.Gen(Loc22, new Shapes.Rectangle(240, -40), new Actions.ClearTile(true));
				//	StructureLoader.ReadStruct(Loc22, "Struct/Morrow/Morrowtop");
					pointVeri = new Point(smx + 10, smy + 500);
					//Point Loc4 = new Point(smx + 233, smy + 45);
				//	WorldUtils.Gen(Loc2, new Shapes.Mound(60, 90), new Actions.SetTile(TileID.Dirt));
				//	WorldUtils.Gen(Loc4, new Shapes.Rectangle(220, 105), new Actions.SetTile(TileID.Dirt));

					//Point Loc5 = new Point(smx + 10, smy + 45);
				//	WorldUtils.Gen(Loc5, new Shapes.Rectangle(220, 50), new Actions.SetTile(TileID.Dirt));



					//Point Loc3 = new Point(smx + 455, smy + 30);
				//	WorldUtils.Gen(Loc3, new Shapes.Mound(40, 50), new Actions.SetTile(TileID.Dirt));
					//Point Loc6 = new Point(smx + 455, smy + 40);
				//	WorldUtils.Gen(Loc6, new Shapes.Circle(40), new Actions.SetTile(TileID.Dirt));
					//	Point resultPoint;
					//	bool searchSuccessful = WorldUtils.Find(Loc, Searches.Chain(new Searches.Right(200), new GenCondition[]
					//	{
					//new Conditions.IsSolid().AreaAnd(10, 10),
					//new Conditions.IsTile(TileID.Sand).AreaAnd(10, 10),
					//	}), out resultPoint);
					//		if (searchSuccessful)
					//		{
					//			WorldGen.TileRunner(resultPoint.X, resultPoint.Y, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(150, 150), TileID.Dirt);
					//		}
					//GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));
					//WorldGen.TileRunner(Loc2.X - 10, Loc2.Y - 60, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(120, 120), TileID.Grass);
					//WorldGen.TileRunner(Loc3.X - 20, Loc2.Y, WorldGen.genRand.Next(40, 43), WorldGen.genRand.Next(100, 100), TileID.Grass);
					//WorldGen.TileRunner(Loc3.X - 20, Loc3.Y + 20, WorldGen.genRand.Next(40, 43), WorldGen.genRand.Next(100, 100), TileID.Grass);
					placed = true;
				}


			}

		}




        #region Manor N Cinderpark

        private void WorldGenCinderspark(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Shifting Shadows deep in the Ice";



            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 300000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int abysmx = WorldGen.genRand.Next(250, Main.maxTilesX - 250); // from 50 since there's a unaccessible area at the world's borders
                                                                               // 50% of choosing the last 6th of the world
                                                                               // Choose which side of the world to be on randomly
                ///if (WorldGen.genRand.NextBool())
                ///{
                ///	towerX = Main.maxTilesX - towerX;
                ///}

                //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                int abysmy = (Main.UnderworldLayer - (Main.maxTilesY / 20));

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(abysmx, abysmy) && abysmy <= Main.UnderworldLayer)
                {
                    abysmy++;
                }

                // If we went under the world's surface, try again
                if (abysmy > Main.UnderworldLayer - 50)
                {
                    continue;
                }
                Tile tile = Main.tile[abysmx, abysmy];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == TileID.Stone
                    || tile.TileType == TileID.Ash
                    || tile.TileType == TileID.Dirt
                    || tile.TileType == TileID.Mud
                    || tile.TileType == TileID.IceBlock))
                {
                    continue;
                }


                // place the Rogue
                //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                //Main.npc[num].homeTileX = -1;
                //	Main.npc[num].homeTileY = -1;
                //	Main.npc[num].direction = 1;
                //	Main.npc[num].homeless = true;


                Point Loc = new Point(abysmx, abysmy);

                for (int da = 0; da < 1; da++)
                {

                    WorldGen.TileRunner(Loc.X, Loc.Y, WorldGen.genRand.Next(150, 150), WorldGen.genRand.Next(500, 500), ModContent.TileType<CindersparkDirt>());







                }





            }
        }

        private void WorldGenManor(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Ereshkigal secretly hiding Sigfried";


			int[] tileBlend = new int[]
			{
				TileID.RubyGemspark
			};

			int[] tileBlend2 = new int[]
			{
				TileID.Stone
			};

			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 10000000)
			{
				// Select a place in the first 6th of the world, avoiding the oceans
				int smx = WorldGen.genRand.Next(((Main.maxTilesX) / 2) - 200, (Main.maxTilesX / 2) + 50); // from 50 since there's a unaccessible area at the world's borders
																										  // 50% of choosing the last 6th of the world
																										  // Choose which side of the world to be on randomly
				///if (WorldGen.genRand.NextBool())
				///{
				///	towerX = Main.maxTilesX - towerX;
				///}

				//Start at 200 tiles above the surface instead of 0, to exclude floating islands
				int smy = Main.UnderworldLayer - 400;

				// We go down until we hit a solid tile or go under the world's surface
				Tile tile = Main.tile[smx, smy];

				while (!WorldGen.SolidTile(smx, smy) && smy <= Main.UnderworldLayer && (!(tile.TileType == ModContent.TileType<CindersparkDirt>())))
				{
					smy++;
					tile = Main.tile[smx, smy];
				}

				// If we went under the world's surface, try again
				if (smy > Main.UnderworldLayer - 20)
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



				for (int da = 0; da < 1; da++)
				{
					Point Loc = new Point(smx, smy + 350);
					Point Loc2 = new Point(smx, smy + 100);
					//StructureLoader.ReadStruct(Loc, "Struct/Underground/Manor", tileBlend);
					string path = "Struct/Underground/Manor";//






                    NPCs.Town.AlcadSpawnSystem.OrdinTile = Loc;
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path, tileBlend);
                    StructureLoader.ProtectStructure(Loc, path);
                    foreach (int chestIndex in ChestIndexs)
					{
						var chest = Main.chest[chestIndex];
						// etc

						// itemsToAdd will hold type and stack data for each item we want to add to the chest
						var itemsToAdd = new List<(int type, int stack)>();

						// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
						int specialItem = new Terraria.Utilities.WeightedRandom<int>(

							Tuple.Create(ModContent.ItemType<VerianBar>(), 0.5)


						// Choose no item with a high weight of 7.
						);
						if (specialItem != ItemID.None)
						{
							itemsToAdd.Add((specialItem, 1));
						}
						// Using a switch statement and a random choice to add sets of items.
						switch (Main.rand.Next(4))
						{
							case 0:
								itemsToAdd.Add((ModContent.ItemType<CinderedCard>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner5>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
								itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
								itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
								itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
								break;
							case 1:
								itemsToAdd.Add((ModContent.ItemType<Volcant>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<VerianBar>(), Main.rand.Next(1, 10)));
                                itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner5>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
								itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
								itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
								itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
								break;
							case 2:
								itemsToAdd.Add((ModContent.ItemType<VeroshotBow>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
								itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
								itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
								itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
								break;
							case 3:
								itemsToAdd.Add((ModContent.ItemType<CinderNeedle>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(10, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner5>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(3, 10)));
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

					
					GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));
					//WorldGen.TileRunner(Loc2.X - 10, Loc2.Y - 60, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(120, 120), TileID.Grass);
					//WorldGen.TileRunner(Loc3.X - 20, Loc2.Y, WorldGen.genRand.Next(40, 43), WorldGen.genRand.Next(100, 100), TileID.Grass);
					//WorldGen.TileRunner(Loc3.X - 20, Loc3.Y + 20, WorldGen.genRand.Next(40, 43), WorldGen.genRand.Next(100, 100), TileID.Grass);



					string path2 = "Struct/Underground/Ishtar";//
					int[] ChestIndexs2 = StructureLoader.ReadStruct(Loc2, path2, tileBlend2);
					NPCs.Town.AlcadSpawnSystem.IshPinTile = Loc2;
					NPCs.Town.AlcadSpawnSystem.EreshTile = Loc2;
					NPCs.Town.AlcadSpawnSystem.PULSETile = Loc2;

					StructureLoader.ProtectStructure(Loc2, path2);
					foreach (int chestIndex in ChestIndexs2)
					{
						var chest = Main.chest[chestIndex];
						// etc

						// itemsToAdd will hold type and stack data for each item we want to add to the chest
						var itemsToAdd = new List<(int type, int stack)>();

						// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
						int specialItem = new Terraria.Utilities.WeightedRandom<int>(

							Tuple.Create(ModContent.ItemType<IshtarCandle>(), 0.5)


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
								itemsToAdd.Add((ModContent.ItemType<IshtarCard>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
								itemsToAdd.Add((ItemID.RagePotion, Main.rand.Next(1, 3)));
								break;
							case 1:
								itemsToAdd.Add((ModContent.ItemType<ImperfectionStaff>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
								itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
								break;
							case 2:
								itemsToAdd.Add((ModContent.ItemType<RazzleDazzle>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
								itemsToAdd.Add((ItemID.FlipperPotion, Main.rand.Next(1, 3)));
								break;
							case 3:
								itemsToAdd.Add((ModContent.ItemType<PoisonPistol>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
								itemsToAdd.Add((ItemID.NightOwlPotion, Main.rand.Next(1, 3)));

								break;
							case 4:
                                itemsToAdd.Add((ModContent.ItemType<EreshkinPowder>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
								itemsToAdd.Add((ItemID.NightOwlPotion, Main.rand.Next(1, 3)));
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







					placed = true;
				}


			}

		}


        #endregion

        #region Small Surface Structures
        private void WorldGenStoneCastle(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Creating life near spawn :)";

			int[] tileBlend = new int[]
			{
				TileID.RubyGemspark
			};

			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 10000000)
			{
				// Select a place in the first 6th of the world, avoiding the oceans
				int smx = WorldGen.genRand.Next((Main.maxTilesX / 2) - 200, (Main.maxTilesX / 2) - 150); // from 50 since there's a unaccessible area at the world's borders
																								   // 50% of choosing the last 6th of the world
																								   // Choose which side of the world to be on randomly
				///if (WorldGen.genRand.NextBool())
				///{
				///	towerX = Main.maxTilesX - towerX;
				///}

				//Start at 200 tiles above the surface instead of 0, to exclude floating islands
				int smy = ((int)(Main.worldSurface - 250));

				// We go down until we hit a solid tile or go under the world's surface
				while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
				{
					smy++;
				}

				// If we went under the world's surface, try again
				if (smy > Main.worldSurface - 20)
				{
					continue;
				}
				Tile tile = Main.tile[smx, smy];
				// If the type of the tile we are placing the tower on doesn't match what we want, try again
				if (!(tile.TileType == TileID.Sand
					|| tile.TileType == TileID.Dirt
					|| tile.TileType == ModContent.TileType<VeriplantGrass>()
					|| tile.TileType == TileID.Grass
					|| tile.TileType == TileID.Stone
					|| tile.TileType == TileID.Sandstone))
				{
					continue;
				}


				// place the Rogue
				//	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
				//Main.npc[num].homeTileX = -1;
				//	Main.npc[num].homeTileY = -1;
				//	Main.npc[num].direction = 1;
				//	Main.npc[num].homeless = true;



				for (int da = 0; da < 1; da++)
				{
					Point Loc = new Point(smx, smy + 450);
					string path = "Struct/Underground/StoneGolem";//
					int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path, tileBlend);
					StructureLoader.ProtectStructure(Loc, path);
					foreach (int chestIndex in ChestIndexs)
					{
						var chest = Main.chest[chestIndex];
						// etc

						// itemsToAdd will hold type and stack data for each item we want to add to the chest
						var itemsToAdd = new List<(int type, int stack)>();

						// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
						int specialItem = new Terraria.Utilities.WeightedRandom<int>(

							Tuple.Create(ModContent.ItemType<StoniaBroochA>(), 0.5)


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
								itemsToAdd.Add((ModContent.ItemType<Gutinier>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
								itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
								break;
							case 1:
								itemsToAdd.Add((ItemID.ShinyRedBalloon, Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
								break;
							case 2:
								itemsToAdd.Add((ItemID.EndlessQuiver, Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
								break;
							case 3:
								itemsToAdd.Add((ItemID.SlimeStaff, Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));

								break;
							case 4:
								itemsToAdd.Add((ModContent.ItemType<StoniaHat>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<StoniaBoots>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<StoniaChestplate>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.GenderChangePotion, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
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
				}

				placed = true;
				

			}

		}
		private void WorldGenXixVillage(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Witches spreading love all inside you!";



			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 10000000)
			{
				// Select a place in the first 6th of the world, avoiding the oceans
				int smx = WorldGen.genRand.Next((Main.maxTilesX / 2) - 300, (Main.maxTilesX / 2) - 150); // from 50 since there's a unaccessible area at the world's borders
																										 // 50% of choosing the last 6th of the world
																										 // Choose which side of the world to be on randomly
				///if (WorldGen.genRand.NextBool())
				///{
				///	towerX = Main.maxTilesX - towerX;
				///}

				//Start at 200 tiles above the surface instead of 0, to exclude floating islands
				int smy = ((int)(Main.worldSurface - 200));

				// We go down until we hit a solid tile or go under the world's surface
				while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
				{
					smy++;
				}

				// If we went under the world's surface, try again
				if (smy > Main.worldSurface - 20)
				{
					continue;
				}
				Tile tile = Main.tile[smx, smy];
				// If the type of the tile we are placing the tower on doesn't match what we want, try again
				if (!(tile.TileType == TileID.Sand
					|| tile.TileType == TileID.Dirt
					|| tile.TileType == ModContent.TileType<VeriplantGrass>()
					|| tile.TileType == TileID.Grass
					|| tile.TileType == TileID.Stone
					|| tile.TileType == TileID.Sandstone))
				{
					continue;
				}


				// place the Rogue
				//	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
				//Main.npc[num].homeTileX = -1;
				//	Main.npc[num].homeTileY = -1;
				//	Main.npc[num].direction = 1;
				//	Main.npc[num].homeless = true;



				for (int da = 0; da < 1; da++)
				{
					Point Loc = new Point(smx, smy + 15);
                    Point Loc22 = new Point(smx, smy + 58);
                    string path = "Struct/Overworld/WitchTown";
					var tileBlend = new int[]
					{
						TileID.RubyGemspark
					};

                    int[] ChestIndexs = Structurizer.ReadStruct(Loc, path, tileBlend);
                    Structurizer.ProtectStructure(Loc, path);
                    NPCs.Town.AlcadSpawnSystem.LittleWitchTownTile = Loc;
					foreach (int chestIndex in ChestIndexs)
					{
						var chest = Main.chest[chestIndex];
						// etc

						// itemsToAdd will hold type and stack data for each item we want to add to the chest
						var itemsToAdd = new List<(int type, int stack)>();

						// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
						int specialItem = new Terraria.Utilities.WeightedRandom<int>(

							Tuple.Create(ModContent.ItemType<Ivythorn>(), 0.5)


						// Choose no item with a high weight of 7.
						);
						if (specialItem != ItemID.None)
						{
							itemsToAdd.Add((specialItem, 1));
						}
						// Using a switch statement and a random choice to add sets of items.
						switch (Main.rand.Next(4))
						{
							case 0:
								itemsToAdd.Add((ModContent.ItemType<PerfectionStaff>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
								itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
								break;
							case 1:
								itemsToAdd.Add((ItemID.CordageGuide, Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
								break;
							case 2:
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 50)));
								itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
								break;
							case 3:
								itemsToAdd.Add((ItemID.SlimeStaff, Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));

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
				}
				
				placed = true;

				
			}

		}

        private void WorldGenBloodCathedral(GenerationProgress progress, GameConfiguration configuration)
        {
            StructureMap structures = GenVars.structures;
            Rectangle rectangle = StructureLoader.ReadRectangle("Struct/Boss/SanguimiBoss");
            progress.Message = "Building a Bloody Cathedral";

            int[] tileBlend = new int[]
            {
                TileID.RubyGemspark
            };

            for (int k = 0; k < 1; k++)
            {
                bool placed = false;
                int attempts = 0;
                while (!placed && attempts++ < 1000000)
                {
                    // Select a place in the first 6th of the world, avoiding the oceans
                    int range = Main.maxTilesX / 8;
                    int offset = WorldGen.genRand.Next(-range, range);
                    int smx = Main.maxTilesX / 2 + offset;

                    //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                    int smy = ((int)(Main.worldSurface - 200));

                    // We go down until we hit a solid tile or go under the world's surface
                    while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
                    {
                        smy++;
                    }

                    // If we went under the world's surface, try again
                    if (smy > Main.worldSurface)
                    {
                        continue;
                    }

                    Tile tile = Main.tile[smx, smy];
                    if (!(tile.TileType == TileID.Sand
                      || tile.TileType == TileID.Dirt
                      || tile.TileType == ModContent.TileType<VeriplantGrass>()
                      || tile.TileType == TileID.Grass
                      || tile.TileType == TileID.Stone
                      || tile.TileType == TileID.Sandstone))
                    {
                        continue;
                    }

                    Tile tileAbove = Main.tile[smx, smy - 1];
                    if (tileAbove.LiquidAmount > 0)
                        continue;

                    Point Loc = new Point(smx, smy + 10);
                    string path = "Struct/Boss/SanguimiBoss";
                    if (!Structurizer.TryPlaceAndProtectStructure(Loc, path))
                        continue;
                    int[] chests = Structurizer.ReadStruct(Loc, path, tileBlend);
                    placed = true;
                }
            }
        }
        private void WorldGenGraving(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "You aren't escaping the Kill Pillars";


			int smx = 0;
            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 10000000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
               ; // from 50 since there's a unaccessible area at the world's borders
                switch (Main.rand.Next(2))
                {


                    case 0:
						{
                           smx = WorldGen.genRand.Next(1000, (Main.maxTilesX / 2) - 350);
                        }
					


                     
                        break;

                    case 1:
						{
                            smx = WorldGen.genRand.Next((Main.maxTilesX / 2) + 350, (Main.maxTilesX) - 1000);
                        }
					


                     
                        break;

                }                                                                                        // 50% of choosing the last 6th of the world
                                                                                                         // Choose which side of the world to be on randomly
                ///if (WorldGen.genRand.NextBool())
                ///{
                ///	towerX = Main.maxTilesX - towerX;
                ///}

                //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                int smy = ((int)(Main.worldSurface - 200));

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
                {
                    smy++;
                }

                // If we went under the world's surface, try again
                if (smy > Main.worldSurface - 20)
                {
                    continue;
                }
                Tile tile = Main.tile[smx, smy];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == TileID.Dirt
                    || tile.TileType == ModContent.TileType<VeriplantGrass>()
                    || tile.TileType == TileID.Grass
                    || tile.TileType == TileID.Stone))
                {
                    continue;
                }


                // place the Rogue
                //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                //Main.npc[num].homeTileX = -1;
                //	Main.npc[num].homeTileY = -1;
                //	Main.npc[num].direction = 1;
                //	Main.npc[num].homeless = true;



                for (int da = 0; da < 1; da++)
                {
                    Point Loc = new Point(smx, smy + 3);

                    string path = "Struct/Overworld/Graving";

                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path);
                    StructureLoader.ProtectStructure(Loc, path);
                    NPCs.Town.AlcadSpawnSystem.DaedenTile = Loc;
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                            Tuple.Create(ModContent.ItemType<Ivythorn>(), 0.5)


                        // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(4))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<PerfectionStaff>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.CordageGuide, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;
                            case 2:
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Book, Main.rand.Next(1, 50)));
                                itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
                                break;
                            case 3:
                                itemsToAdd.Add((ItemID.SlimeStaff, Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));

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
                }

                for (int da = 0; da < 1; da++)
                {
                    Point Loc2 = new Point(smx, smy + 3);
                    WorldUtils.Gen(Loc2, new Shapes.Rectangle(75, 20), new Actions.SetTile(TileID.Grass));



                }
                placed = true;


            }

        }

        private void WorldGenWindmills(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Adding life to the world!";

			for (int k = 0; k < 2; k++)
			{

				bool placed = false;
				int attempts = 0;
				while (!placed && attempts++ < 10000000)
				{
					// Select a place in the first 6th of the world, avoiding the oceans
					int smx = WorldGen.genRand.Next(400, (Main.maxTilesX / 3)); // from 50 since there's a unaccessible area at the world's borders
																				// 50% of choosing the last 6th of the world
																				// Choose which side of the world to be on randomly
					///if (WorldGen.genRand.NextBool())
					///{
					///	towerX = Main.maxTilesX - towerX;
					///}

					//Start at 200 tiles above the surface instead of 0, to exclude floating islands
					int smy = ((int)(Main.worldSurface - 250));

					// We go down until we hit a solid tile or go under the world's surface
					while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
					{
						smy++;
					}

					// If we went under the world's surface, try again
					if (smy > Main.worldSurface - 20)
					{
						continue;
					}
					Tile tile = Main.tile[smx, smy];
					// If the type of the tile we are placing the tower on doesn't match what we want, try again
					if (!(tile.TileType == TileID.Dirt
						|| tile.TileType == TileID.Stone
						|| tile.TileType == TileID.Grass))
					{
						continue;
					}


					// place the Rogue
					//	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
					//Main.npc[num].homeTileX = -1;
					//	Main.npc[num].homeTileY = -1;
					//	Main.npc[num].direction = 1;
					//	Main.npc[num].homeless = true;



					for (int da = 0; da < 1; da++)
					{
						Point Loc = new Point(smx, smy + 1);

						int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Overworld/Windmill");
						foreach (int chestIndex in ChestIndexs)
						{
							var chest = Main.chest[chestIndex];
							// etc

							// itemsToAdd will hold type and stack data for each item we want to add to the chest
							var itemsToAdd = new List<(int type, int stack)>();

							// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
							int specialItem = new Terraria.Utilities.WeightedRandom<int>(

								Tuple.Create(ModContent.ItemType<Ivythorn>(), 0.5)


							// Choose no item with a high weight of 7.
							);
							if (specialItem != ItemID.None)
							{
								itemsToAdd.Add((specialItem, 1));
							}
							// Using a switch statement and a random choice to add sets of items.
							switch (Main.rand.Next(4))
							{
								case 0:
									itemsToAdd.Add((ModContent.ItemType<WindmillShuriken>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
									break;
								case 1:
									itemsToAdd.Add((ModContent.ItemType<WindmillionRobe>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<WindmillionHat>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<WindmillionBoots>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									break;
								case 2:
									itemsToAdd.Add((ModContent.ItemType<WindedQuiver>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									break;
								case 3:
									itemsToAdd.Add((ItemID.BabyBirdStaff, Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));

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
					}

					for (int da = 0; da < 1; da++)
					{
						Point Loc2 = new Point(smx, smy + 2);
						WorldUtils.Gen(Loc2, new Shapes.Rectangle(34, 10), new Actions.SetTile(TileID.Grass));



					}
					placed = true;


				}




			}

			for (int k = 0; k < 2; k++)
			{

				bool placed = false;
				int attempts = 0;
				while (!placed && attempts++ < 10000000)
				{
					// Select a place in the first 6th of the world, avoiding the oceans
					int smx = WorldGen.genRand.Next((Main.maxTilesX) - (Main.maxTilesX / 3), (Main.maxTilesX) - 200); // from 50 since there's a unaccessible area at the world's borders
																				// 50% of choosing the last 6th of the world
																				// Choose which side of the world to be on randomly
					///if (WorldGen.genRand.NextBool())
					///{
					///	towerX = Main.maxTilesX - towerX;
					///}

					//Start at 200 tiles above the surface instead of 0, to exclude floating islands
					int smy = ((int)(Main.worldSurface - 250));

					// We go down until we hit a solid tile or go under the world's surface
					while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
					{
						smy++;
					}

					// If we went under the world's surface, try again
					if (smy > Main.worldSurface - 20)
					{
						continue;
					}
					Tile tile = Main.tile[smx, smy];
					// If the type of the tile we are placing the tower on doesn't match what we want, try again
					if (!(tile.TileType == TileID.Dirt
						|| tile.TileType == TileID.Stone
						|| tile.TileType == ModContent.TileType<VeriplantGrass>()
						|| tile.TileType == TileID.Grass))
					{
						continue;
					}


					// place the Rogue
					//	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
					//Main.npc[num].homeTileX = -1;
					//	Main.npc[num].homeTileY = -1;
					//	Main.npc[num].direction = 1;
					//	Main.npc[num].homeless = true;



					for (int da = 0; da < 1; da++)
					{
						Point Loc = new Point(smx, smy + 1);
						

						int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Overworld/Windmill");
						foreach (int chestIndex in ChestIndexs)
						{
							var chest = Main.chest[chestIndex];
							// etc

							// itemsToAdd will hold type and stack data for each item we want to add to the chest
							var itemsToAdd = new List<(int type, int stack)>();

							// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
							int specialItem = new Terraria.Utilities.WeightedRandom<int>(

								Tuple.Create(ModContent.ItemType<Ivythorn>(), 0.5)


							// Choose no item with a high weight of 7.
							);
							if (specialItem != ItemID.None)
							{
								itemsToAdd.Add((specialItem, 1));
							}
							// Using a switch statement and a random choice to add sets of items.
							switch (Main.rand.Next(4))
							{
								case 0:
									itemsToAdd.Add((ModContent.ItemType<WindmillShuriken>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
									break;
								case 1:
									itemsToAdd.Add((ModContent.ItemType<WindmillionRobe>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<WindmillionHat>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<WindmillionBoots>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									break;
								case 2:
									itemsToAdd.Add((ModContent.ItemType<WindedQuiver>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									break;
								case 3:
									itemsToAdd.Add((ItemID.BabyBirdStaff, Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));

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
					}


					for (int da = 0; da < 1; da++)
					{
						Point Loc2 = new Point(smx, smy + 2);
						WorldUtils.Gen(Loc2, new Shapes.Rectangle(34, 10), new Actions.SetTile(TileID.Grass));



					}






					placed = true;


				}

			}

		}

		private void WorldGenMed(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Building Gintze houses";


			for (int k = 0; k < 3; k++)
			{
				bool placed = false;
				int attempts = 0;
				while (!placed && attempts++ < 1000000)
				{
					// Select a place in the first 6th of the world, avoiding the oceans
					int smx = WorldGen.genRand.Next(750, Main.maxTilesX); // from 50 since there's a unaccessible area at the world's borders
																			// 50% of choosing the last 6th of the world
																			// Choose which side of the world to be on randomly
					///if (WorldGen.genRand.NextBool())
					///{
					///	towerX = Main.maxTilesX - towerX;
					///}

					//Start at 200 tiles above the surface instead of 0, to exclude floating islands
					int smy = ((int)(Main.worldSurface - 200));

					// We go down until we hit a solid tile or go under the world's surface
					while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
					{
						smy++;
					}

					// If we went under the world's surface, try again
					if (smy > Main.worldSurface - 20)
					{
						continue;
					}
					Tile tile = Main.tile[smx, smy];
					// If the type of the tile we are placing the tower on doesn't match what we want, try again
					if (!(tile.TileType == TileID.Dirt
						|| tile.TileType == ModContent.TileType<Tiles.Acid.AcidialDirt>()
						
						|| tile.TileType == TileID.Mud))

					{
						continue;
					}


					// place the Rogue
					//	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
					//Main.npc[num].homeTileX = -1;
					//	Main.npc[num].homeTileY = -1;
					//	Main.npc[num].direction = 1;
					//	Main.npc[num].homeless = true;



					for (int da = 0; da < 1; da++)
					{
						Point Loc = new Point(smx, smy - Main.rand.Next(125, 150));
                        if (!StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Overworld/Overworld2"))
                            continue;
                        int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Overworld/Overworld2");
						foreach (int chestIndex in ChestIndexs)
						{
							var chest = Main.chest[chestIndex];
							// etc

							// itemsToAdd will hold type and stack data for each item we want to add to the chest
							var itemsToAdd = new List<(int type, int stack)>();

							// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
							// Using a switch statement and a random choice to add sets of items.
							switch (Main.rand.Next(9))
							{
								case 0:
									itemsToAdd.Add((ModContent.ItemType<Gutinier>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
									break;
								case 1:
									itemsToAdd.Add((ItemID.WandofFrosting, Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									break;
								case 2:
									itemsToAdd.Add((ItemID.EndlessQuiver, Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									break;
								case 3:
									itemsToAdd.Add((ItemID.SlimeStaff, Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));

									break;
								case 4:
									itemsToAdd.Add((ItemID.Diamond, Main.rand.Next(1, 20)));
									itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.GenderChangePotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									break;

								case 5:
									itemsToAdd.Add((ItemID.CloudinaBottle, Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.GenderChangePotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									break;

								case 6:
									itemsToAdd.Add((ItemID.ShinyRedBalloon, Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.GenderChangePotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									break;

								case 7:
									itemsToAdd.Add((ItemID.BandofRegeneration, Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									break;

								case 8:
									itemsToAdd.Add((ItemID.BandofStarpower, Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
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
					}

					placed = true;
				}
			}

		}

		private void WorldGenBig(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Building Gintze houses";


			for (int k = 0; k < 5; k++)
			{
				bool placed = false;
				int attempts = 0;
				while (!placed && attempts++ < 1000000)
				{
					// Select a place in the first 6th of the world, avoiding the oceans
					int smx = WorldGen.genRand.Next(1000, (Main.maxTilesX) - 500); // from 50 since there's a unaccessible area at the world's borders
																		// 50% of choosing the last 6th of the world
																			// Choose which side of the world to be on randomly
					///if (WorldGen.genRand.NextBool())
					///{
					///	towerX = Main.maxTilesX - towerX;
					///}

					//Start at 200 tiles above the surface instead of 0, to exclude floating islands
					int smy = ((int)(Main.worldSurface - 200));

					// We go down until we hit a solid tile or go under the world's surface
					while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
					{
						smy++;
					}

					// If we went under the world's surface, try again
					if (smy > Main.worldSurface - 20)
					{
						continue;
					}
					Tile tile = Main.tile[smx, smy];
					// If the type of the tile we are placing the tower on doesn't match what we want, try again
					if (!(tile.TileType == TileID.Dirt
						|| tile.TileType == TileID.Sand
						|| tile.TileType == TileID.Mud))

					{
						continue;
					}


					// place the Rogue
					//	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
					//Main.npc[num].homeTileX = -1;
					//	Main.npc[num].homeTileY = -1;
					//	Main.npc[num].direction = 1;
					//	Main.npc[num].homeless = true;



					for (int da = 0; da < 1; da++)
					{
						Point Loc = new Point(smx, smy - Main.rand.Next(125, 150));
                        if (!StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Overworld/Overworld3"))
                            continue;

                        int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Overworld/Overworld3");
						foreach (int chestIndex in ChestIndexs)
						{
							var chest = Main.chest[chestIndex];
							// etc

							// itemsToAdd will hold type and stack data for each item we want to add to the chest
							var itemsToAdd = new List<(int type, int stack)>();

							// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
							// Using a switch statement and a random choice to add sets of items.
							switch (Main.rand.Next(11))
							{
								case 0:
									itemsToAdd.Add((ModContent.ItemType<Gutinier>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
									break;
								case 1:
									itemsToAdd.Add((ItemID.WandofFrosting, Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									break;
								case 2:
									itemsToAdd.Add((ItemID.EndlessQuiver, Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									break;
								case 3:
									itemsToAdd.Add((ItemID.SlimeStaff, Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));

									break;
								case 4:
									itemsToAdd.Add((ItemID.Diamond, Main.rand.Next(1, 20)));
									itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.GenderChangePotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									break;

								case 5:
									itemsToAdd.Add((ModContent.ItemType<IronCrossbow>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									break;

								case 6:
									//itemsToAdd.Add((ModContent.ItemType<EaglesGrace>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									break;

								case 7:
									itemsToAdd.Add((ItemID.ShinyRedBalloon, Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.GenderChangePotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									break;

								case 8:
									itemsToAdd.Add((ItemID.BandofRegeneration, Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									break;

								case 9:
									itemsToAdd.Add((ItemID.BandofStarpower, Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									break;

								case 10:
									itemsToAdd.Add((ItemID.PlatinumBar, Main.rand.Next(1, 20)));
									itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
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
					}

					placed = true;
				}
			}

		}

		private void WorldGenStalker(GenerationProgress progress, GameConfiguration configuration)
		{
			StructureMap structures = GenVars.structures;
			Rectangle rectangle = StructureLoader.ReadRectangle("Struct/Ocean/SunAlter2");
			progress.Message = "Bird building alters";
            
			int[] tileBlend = new int[]
        {
                TileID.RubyGemspark
        };

            for (int k = 0; k < 1; k++)
			{
				bool placed = false;
				int attempts = 0;
				while (!placed && attempts++ < 1000000)
				{
					// Select a place in the first 6th of the world, avoiding the oceans
					int smx = WorldGen.genRand.Next(350, (Main.maxTilesX) - 350); // from 50 since there's a unaccessible area at the world's borders
																						  // 50% of choosing the last 6th of the world
																										  // Choose which side of the world to be on randomly
					///if (WorldGen.genRand.NextBool())
					///{
					///	towerX = Main.maxTilesX - towerX;
					///}

					//Start at 200 tiles above the surface instead of 0, to exclude floating islands
					int smy = ((int)(Main.worldSurface - 200));

					// We go down until we hit a solid tile or go under the world's surface
					while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
					{
						smy++;
					}

					// If we went under the world's surface, try again
					if (smy > Main.worldSurface - 5)
					{
						continue;
					}
					Tile tile = Main.tile[smx, smy];
					// If the type of the tile we are placing the tower on doesn't match what we want, try again
					if (!(tile.TileType == TileID.Sand
							|| tile.TileType == TileID.HardenedSand
						|| tile.TileType == TileID.Sandstone))

					{
						continue;
					}

                    Point Loc = new Point(smx, smy + 10);
                    string path = "Struct/Ocean/SunAlter2";
					if (!StructureLoader.TryPlaceAndProtectStructure(Loc, path))
						continue; 

					for (int da = 0; da < 1; da++)
					{          
                        int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path, tileBlend);
                        foreach (int chestIndex in ChestIndexs)
						{
							var chest = Main.chest[chestIndex];
							// etc

							// itemsToAdd will hold type and stack data for each item we want to add to the chest
							var itemsToAdd = new List<(int type, int stack)>();

							// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
							int specialItem = new Terraria.Utilities.WeightedRandom<int>(
									Tuple.Create(ModContent.ItemType<CinderBraker>(), 0.1)


							);
							if (specialItem != ItemID.None)
							{
								itemsToAdd.Add((specialItem, 1));
							}
							// Using a switch statement and a random choice to add sets of items.
							switch (Main.rand.Next(1))
							{
								case 0:

									itemsToAdd.Add((ModContent.ItemType<OceanScroll>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<OceanRuneI>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
									itemsToAdd.Add((ItemID.AntlionMandible, Main.rand.Next(5, 10)));
									itemsToAdd.Add((ItemID.Coral, Main.rand.Next(1, 25)));
									itemsToAdd.Add((ItemID.SharkFin, Main.rand.Next(1, 25)));
									itemsToAdd.Add((ItemID.MasterBait, Main.rand.Next(1, 25)));

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
						
						
					}

					placed = true;
				}
			}

		}

		private void WorldGenGiaHouse(GenerationProgress progress, GameConfiguration configuration)
		{
			StructureMap structures = GenVars.structures;
			Rectangle rectangle = StructureLoader.ReadRectangle("Struct/Acid/GiaHouse");
			progress.Message = "Gia living fruitfully";


			for (int k = 0; k < 1; k++)
			{
				bool placed = false;
				int attempts = 0;
				while (!placed && attempts++ < 1000000)
				{
					// Select a place in the first 6th of the world, avoiding the oceans
					int smx = WorldGen.genRand.Next(250, (Main.maxTilesX) - 250); // from 50 since there's a unaccessible area at the world's borders
																				  // 50% of choosing the last 6th of the world
																				  // Choose which side of the world to be on randomly
					///if (WorldGen.genRand.NextBool())
					///{
					///	towerX = Main.maxTilesX - towerX;
					///}

					//Start at 200 tiles above the surface instead of 0, to exclude floating islands
					int smy = ((int)(Main.worldSurface - 200));

					// We go down until we hit a solid tile or go under the world's surface
					while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
					{
						smy++;
					}

					// If we went under the world's surface, try again
					if (smy > Main.worldSurface - 5)
					{
						continue;
					}
					Tile tile = Main.tile[smx, smy];
					// If the type of the tile we are placing the tower on doesn't match what we want, try again
					if (!(tile.TileType == ModContent.TileType<AcidialDirt>()))
					{
						continue;
					}



					// place the Rogue
					//	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
					//Main.npc[num].homeTileX = -1;
					//	Main.npc[num].homeTileY = -1;
					//	Main.npc[num].direction = 1;
					//	Main.npc[num].homeless = true;



					for (int da = 0; da < 1; da++)
					{
						Point Loc = new Point(smx, smy + 5);
						rectangle.Location = Loc;
						StructureLoader.ReadStruct(Loc, "Struct/Acid/GiaHouse");
						NPCs.Town.AlcadSpawnSystem.GiaTile = Loc;
						
						


					}

					placed = true;
				}
			}

		}

        #endregion


        #region Virulent N Govheil


        public void WorldGenVirulent(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Virulifying the Morrow";



            bool placed = false;
            int attempts = 0;
            int leftmostJungleTileX = int.MaxValue;
            int rightmostJungleTileX = int.MinValue;
            for (int x = 500; x < Main.maxTilesX - 500; x++)
            {
                int jungleY = (int)(Main.worldSurface - 50);
                while (!WorldGen.SolidTile(x, jungleY) && jungleY <= Main.worldSurface)
                {
                    jungleY++;
                }

                Tile tile = Main.tile[x, jungleY];
                if (tile.TileType == TileID.Mud)
                {
                    if (leftmostJungleTileX > x)
                        leftmostJungleTileX = x;
                    if (rightmostJungleTileX < x)
                        rightmostJungleTileX = x;
                }
            }



            while (!placed && attempts++ < 100000)
            {
                // Select a place in the first 6th of the world, avoiding the oceans
                int minX = leftmostJungleTileX + 400;
                int maxX = rightmostJungleTileX - 400;
                if (maxX < minX)
                    maxX = minX + 1;
                int abysmx = WorldGen.genRand.Next(minX, maxX); // from 50 since there's a unaccessible area at the world's borders

                //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                int abysmy = (int)(Main.worldSurface - 50);

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(abysmx, abysmy) && abysmy <= Main.worldSurface)
                {
                    abysmy++;
                }


                for (int da = 0; da < 1; da++)
                {
                    Point Loc7 = new Point(abysmx, abysmy);
                    WorldGen.TileRunner(Loc7.X + 200, Loc7.Y, 500, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>(), false, 0f, 0f, true, true);
                    WorldGen.TileRunner(Loc7.X + 200, Loc7.Y + 200, 600, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>(), false, 0f, 0f, true, true);
                    WorldGen.TileRunner(Loc7.X + 200, Loc7.Y + 400, 600, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>(), true, 0f, 0f, true, true);
                    WorldGen.TileRunner(Loc7.X + 200, Loc7.Y + 600, 700, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>(), true, 0f, 0f, true, true);
                    WorldGen.TileRunner(Loc7.X + 200, Loc7.Y + 800, 700, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>(), true, 0f, 0f, true, true);
                    WorldGen.TileRunner(Loc7.X + 200, Loc7.Y + 1000, 700, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>(), true, 0f, 0f, true, true);

                    Point Loc = new Point(abysmx + 50, abysmy + 255);
                    pointL = new Point(abysmx + 50, abysmy + 255);//

                    WorldGen.DirtyRockRunner(0, Main.maxTilesX - 50);
                    placed = true;
                }
            }

            for (int fa = 0; fa < 20; fa++)
            {
                int abysmxd = WorldGen.genRand.Next(500, Main.maxTilesX - 500);
                int abysmyd = (int)(Main.worldSurface - 50);

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(abysmxd, abysmyd) && abysmyd <= Main.worldSurface)
                {
                    abysmyd++;
                }

                // If we went under the world's surface, try again
                if (abysmyd > Main.worldSurface)
                {
                    continue;
                }
                Tile tile = Main.tile[abysmxd, abysmyd];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == ModContent.TileType<Tiles.Acid.AcidialDirt>()))
                {
                    continue;
                }
                for (int da = 0; da < 1; da++)
                {
                    Point Loc = new Point(abysmxd, abysmyd);


                    WorldGen.digTunnel(Loc.X, Loc.Y, 0, 1, 130, 3, false);
                }




            }


        }

        private void WorldGenGovheilCastle(GenerationProgress progress, GameConfiguration configuration)
		{
			StructureMap structures = GenVars.structures;
			Rectangle rectangle = StructureLoader.ReadRectangle("Struct/Huntria/Govheil2");
			progress.Message = "Irradia marrying Paraffin instead of Delgrim";
			
			int[] tileBlend = new int[]
			{
				TileID.RubyGemspark
			};


			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 1000000)
			{


				int abysmx = WorldGen.genRand.Next(300, Main.maxTilesX - 300); // from 50 since there's a unaccessible area at the world's borders

				// Select a place in the first 6th of the world, avoiding the oceans
				int abysmy = ((Main.maxTilesY / 2));

				// We go down until we hit a solid tile or go under the world's surface
				while (!WorldGen.SolidTile(abysmx, abysmy) && abysmy <= Main.UnderworldLayer)
				{
					abysmy++;
				}

				// If we went under the world's surface, try again
				if (abysmy > Main.UnderworldLayer - 50)
				{
					continue;
				}

				


				Tile tile = Main.tile[abysmx, abysmy];
				


				// If the type of the tile we are placing the tower on doesn't match what we want, try again
				if (!(tile.TileType == ModContent.TileType<AcidialDirt>() || tile.TileType == TileID.Sand))
				{
					continue;
				}



				// place the Rogue
				//	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
				//Main.npc[num].homeTileX = -1;
				//	Main.npc[num].homeTileY = -1;
				//	Main.npc[num].direction = 1;
				//	Main.npc[num].homeless = true;



				for (int da = 0; da < 1; da++)
				{
					string path = "Struct/Huntria/Govheil2";

					int[] ChestIndexs = StructureLoader.ReadStruct(pointL, path, tileBlend);
					rectangle.Location = pointL;
					NPCs.Town.AlcadSpawnSystem.IrrTile = pointL;
                    NPCs.Town.AlcadSpawnSystem.GothTile = pointL;
                    StructureLoader.ProtectStructure(pointL, path);
					foreach (int chestIndex in ChestIndexs)
					{
						var chest = Main.chest[chestIndex];
						// etc

						// itemsToAdd will hold type and stack data for each item we want to add to the chest
						var itemsToAdd = new List<(int type, int stack)>();

						// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
						int specialItem = new Terraria.Utilities.WeightedRandom<int>(

							Tuple.Create(ModContent.ItemType<AlcadizScrap>(), 0.5),
							Tuple.Create(ModContent.ItemType<LostScrap>(), 0.4),
							Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.1)

						// Choose no item with a high weight of 7.
						);
						if (specialItem != ItemID.None)
						{
							itemsToAdd.Add((specialItem, 1));
						}
						// Using a switch statement and a random choice to add sets of items.
						switch (Main.rand.Next(10))
						{
							case 0:
								itemsToAdd.Add((ModContent.ItemType<GovheilPowder>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(5, 20)));								 
								itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
								break;
							case 1:
								itemsToAdd.Add((ModContent.ItemType<GreekLantern>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
								
								itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 30)));
								itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
								break;
							case 2:
								itemsToAdd.Add((ModContent.ItemType<Kilvier>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								
								itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
								break;
							case 3:
								itemsToAdd.Add((ModContent.ItemType<Galvinie>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
								itemsToAdd.Add((ModContent.ItemType<AlcadizScrap>(), Main.rand.Next(5, 20)));
								itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 30)));
								itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

								break;
							case 4:
								itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.JungleSpores, Main.rand.Next(3, 7)));
								
								itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
								itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
								break;

							case 5:
								itemsToAdd.Add((ModContent.ItemType<GovhenShield>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
								break;

							case 6:
								itemsToAdd.Add((ModContent.ItemType<TheBurningRod>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 15)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
								itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
								break;


							case 7:
								itemsToAdd.Add((ModContent.ItemType<GovheilHolsterBroochA>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 15)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
								itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
								break;

							case 8:
								itemsToAdd.Add((ModContent.ItemType<Blackdot>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
								
								itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 30)));
								itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
								break;

							case 9:
                                itemsToAdd.Add((ModContent.ItemType<SrTetanus>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 15)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
                                itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
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









				}
					for (int daa = 0; daa < 1; daa++)
					{

						rectangle.Location = pointL;
						Point ponta = new Point(pointL.X + 150, pointL.Y + 300);
						int[] ChestIndexs = StructureLoader.ReadStruct(ponta, "Struct/Acid/Lab");
						StructureLoader.ProtectStructure(ponta, "Struct/Acid/Lab");
		
						NPCs.Town.AlcadSpawnSystem.LabTile = ponta;

						foreach (int chestIndex in ChestIndexs)
						{
							var chest = Main.chest[chestIndex];
							// etc

							// itemsToAdd will hold type and stack data for each item we want to add to the chest
							var itemsToAdd = new List<(int type, int stack)>();

							// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
							int specialItem = new Terraria.Utilities.WeightedRandom<int>(

								Tuple.Create(ModContent.ItemType<AlcadizScrap>(), 0.5),
								Tuple.Create(ModContent.ItemType<LostScrap>(), 0.1),
								Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

							// Choose no item with a high weight of 7.
							);
							if (specialItem != ItemID.None)
							{
								itemsToAdd.Add((specialItem, 1));
							}
							// Using a switch statement and a random choice to add sets of items.
							switch (Main.rand.Next(9))
							{
								case 0:
									itemsToAdd.Add((ModContent.ItemType<GovheilPowder>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(5, 20)));
									 ;
									itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
									itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
									break;
								case 1:
									itemsToAdd.Add((ModContent.ItemType<GreekLantern>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
									
									itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 30)));
									itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
									itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
									break;
								case 2:
									itemsToAdd.Add((ModContent.ItemType<Kilvier>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
									itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									
									itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
									break;
								case 3:
									itemsToAdd.Add((ModContent.ItemType<Galvinie>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
									itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
									itemsToAdd.Add((ModContent.ItemType<AlcadizScrap>(), Main.rand.Next(5, 20)));
									itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 30)));
									itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

									break;
								case 4:
									itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
									itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.JungleSpores, Main.rand.Next(3, 7)));
									
									itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
									itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
									break;

								case 5:
									itemsToAdd.Add((ModContent.ItemType<GovhenShield>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
									itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
									break;

								case 6:
									itemsToAdd.Add((ModContent.ItemType<TheBurningRod>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 15)));
									itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
									itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
									break;


								case 7:
									itemsToAdd.Add((ModContent.ItemType<GovheilHolsterBroochA>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 15)));
									itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
									itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
									itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
									itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
									break;

								case 9:
									itemsToAdd.Add((ModContent.ItemType<Blackdot>(), Main.rand.Next(1, 1)));
									itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
									
									itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 30)));
									itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
									itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
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
					}

				placed = true;
			}


		}


        private void WorldGenVirulentStructures(GenerationProgress progress, GameConfiguration configuration)
        {
            // 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
            progress.Message = "Hunters getting kicked out";


            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-06 - 5); k++)
            {
                // 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
                int xa = WorldGen.genRand.Next(500, Main.maxTilesX - 500);
                int ya = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)GenVars.rockLayerHigh);
                Point Loc = new Point(xa, ya);

                // 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
                Tile tile = Main.tile[Loc.X, Loc.Y];

                if (!(tile.TileType == ModContent.TileType<Tiles.Acid.AcidialDirt>() || tile.TileType == TileID.Mud))
                {
                    continue;
                }

                if (tile.HasTile)
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Acid/A3");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                                Tuple.Create((int)ItemID.Acorn, 0.1),
                                Tuple.Create((int)ItemID.ManaCrystal, 0.1),
                                Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.7)

                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(4))
                        {
                            case 0:

                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));


                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));

                                itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.JungleSpores, 7));

                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));


                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                break;
                            case 2:

                                itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:

                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

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
                }
            }




            for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-06 - 4); k++)
            {
                // 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
                int xa = WorldGen.genRand.Next(500, Main.maxTilesX - 200);
                int ya = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, (int)GenVars.rockLayerHigh);
                Point Loc = new Point(xa, ya);

                // 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
                Tile tile = Main.tile[Loc.X, Loc.Y];

                if (!(tile.TileType == ModContent.TileType<Tiles.Acid.AcidialDirt>() || tile.TileType == TileID.Mud || tile.TileType == TileID.Stone))
                {
                    continue;
                }

                if (tile.HasTile)
                {
                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Acid/A3");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                            Tuple.Create((int)ItemID.Acorn, 0.1),
                                Tuple.Create((int)ItemID.ManaCrystal, 0.1),
                                Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.7)

                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(4))
                        {
                            case 0:


                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));


                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));

                                itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ItemID.JungleSpores, 7));

                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));


                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                break;
                            case 2:

                                itemsToAdd.Add((ItemID.Daybloom, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:

                                itemsToAdd.Add((ModContent.ItemType<ArncharChunk>(), Main.rand.Next(30, 55)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

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
                }
            }

        }

        #endregion


        #region Abyss

        private void WorldGenAbysm(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Shifting Shadows deep in the Ice";



			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 10000)
			{
				// Select a place in the first 6th of the world, avoiding the oceans
				int abysmx = WorldGen.genRand.Next(500, Main.maxTilesX - 500); // from 50 since there's a unaccessible area at the world's borders
																			   // 50% of choosing the last 6th of the world
																			   // Choose which side of the world to be on randomly
				///if (WorldGen.genRand.NextBool())
				///{
				///	towerX = Main.maxTilesX - towerX;
				///}

				//Start at 200 tiles above the surface instead of 0, to exclude floating islands
				int abysmy = ((Main.maxTilesY / 2) - 200);

				// We go down until we hit a solid tile or go under the world's surface
				while (!WorldGen.SolidTile(abysmx, abysmy) && abysmy <= Main.UnderworldLayer)
				{
					abysmy++;
				}

				// If we went under the world's surface, try again
				if (abysmy > Main.UnderworldLayer - 50)
				{
					continue;
				}
				Tile tile = Main.tile[abysmx, abysmy];
				// If the type of the tile we are placing the tower on doesn't match what we want, try again
				if (!(tile.TileType == TileID.CorruptIce
					|| tile.TileType == TileID.SnowBlock
					|| tile.TileType == TileID.FleshIce
					|| tile.TileType == TileID.IceBlock
					|| tile.TileType == TileID.Slush))
				{
					continue;
				}


				// place the Rogue
				//	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
				//Main.npc[num].homeTileX = -1;
				//	Main.npc[num].homeTileY = -1;
				//	Main.npc[num].direction = 1;
				//	Main.npc[num].homeless = true;


				Point Loc = new Point(abysmx, abysmy);

				for (int da = 0; da < 1; da++)
				{
					
					WorldGen.TileRunner(Loc.X, Loc.Y, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(450, 450), ModContent.TileType<AbyssalDirt>(), true);





				

				}

				



			}
		}
        private void NewCaveFormationAbysm(GenerationProgress progress, GameConfiguration configuration)
        {




            int attempts = 0;
            while (attempts++ < 1000000)
            {
                // Select a place 
                int smx = WorldGen.genRand.Next(1, Main.maxTilesX); // from 50 since there's a unaccessible area at the world's borders
                                                                    // 50% of choosing the last 6th of the world
                                                                    // Choose which side of the world to be on randomly
                ///if (WorldGen.genRand.NextBool())
                ///{
                ///	towerX = Main.maxTilesX - towerX;
                ///}

                //Start at 200 tiles above the surface instead of 0, to exclude floating islands
                int smy = ((Main.maxTilesY / 2) - 200);

                // We go down until we hit a solid tile or go under the world's surface
                Tile tile = Main.tile[smx, smy];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == ModContent.TileType<AbyssalDirt>()))
                {
                    continue;
                }
                while (!WorldGen.SolidTile(smx, smy) && smy <= Main.UnderworldLayer)
                {
                    //seperation
                    smx += 1;
                    smy += 30;
                    tile = Main.tile[smx, smy];
                }

                // If we went under the world's surface, try again
                if (smy > Main.UnderworldLayer - 20)
                {
                    continue;
                }

                // If the type of the tile we are placing the tower on doesn't match what we want, try again


                for (int da = 0; da < 1; da++)
                {
                    Point Loc = new Point(smx, smy + 350);
                    Point Loc2 = new Point(smx, smy + 100);
                    //StructureLoader.ReadStruct(Loc, "Struct/Underground/Manor", tileBlend);

                    WorldGen.digTunnel(smx, smy, 2, 1, 10, 2, false);
                    WorldGen.digTunnel(smx, smy, 2, 1, 5, 2, true);

                    // WorldGen.digTunnel(smx, smy - 300, 3, 1, 50, 2, true);




                }


            }

        }
        private void WorldGenAurelusTemple(GenerationProgress progress, GameConfiguration configuration)
        {
            StructureMap structures = GenVars.structures;
            Rectangle rectangle = StructureLoader.ReadRectangle("Struct/Aurelus/AurelusTemple2");
            progress.Message = "Singularities singing!";

            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 1000000)
            {
                int abysmx = WorldGen.genRand.Next(500, Main.maxTilesX - 500); // from 50 since there's a unaccessible area at the world's borders

                // Select a place in the first 6th of the world, avoiding the oceans
                int abysmy = ((Main.maxTilesY / 2));

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(abysmx, abysmy) && abysmy <= Main.UnderworldLayer)
                {
                    abysmy++;
                }

                // If we went under the world's surface, try again
                if (abysmy > Main.UnderworldLayer - 50)
                {
                    continue;
                }
                Tile tile = Main.tile[abysmx, abysmy];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == ModContent.TileType<AbyssalDirt>()))
                {
                    continue;
                }


                // place the Rogue
                //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                //Main.npc[num].homeTileX = -1;
                //	Main.npc[num].homeTileY = -1;
                //	Main.npc[num].direction = 1;
                //	Main.npc[num].homeless = true;



                for (int da = 0; da < 1; da++)
                {
                    Point Loc = new Point(abysmx - 150, abysmy + 100);
                    rectangle.Location = Loc;
                    StructureLoader.ProtectStructure(Loc, "Struct/Aurelus/AurelusTemple2");

                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Aurelus/AurelusTemple2");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(7))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<MagnusMagnum>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ModContent.ItemType<Venatici>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));

                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                break;
                            case 2:
                                itemsToAdd.Add((ModContent.ItemType<Neptune8Card>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner8>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:
                                itemsToAdd.Add((ModContent.ItemType<TON618Crossbow>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                break;
                            case 4:
                                itemsToAdd.Add((ModContent.ItemType<HolmbergScythe>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));

                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                break;

                            case 5:
                                itemsToAdd.Add((ModContent.ItemType<AurelusBlightBroochA>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner8>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 10)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;

                            case 6:
                                itemsToAdd.Add((ModContent.ItemType<AbyssalPowder>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner8>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Shiverthorn, Main.rand.Next(2, 15)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 10)));
                                itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
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
                }

                placed = true;
            }


        }


        private void WorldGenRallad(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Rallad killing people";



            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 1000000)
            {


                int abysmx = WorldGen.genRand.Next(500, Main.maxTilesX - 500); // from 50 since there's a unaccessible area at the world's borders

                // Select a place in the first 6th of the world, avoiding the oceans
                int abysmy = ((Main.maxTilesY / 2));

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(abysmx, abysmy) && abysmy <= Main.UnderworldLayer)
                {
                    abysmy++;
                }

                // If we went under the world's surface, try again
                if (abysmy > Main.UnderworldLayer - 50)
                {
                    continue;
                }
                Tile tile = Main.tile[abysmx, abysmy];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == ModContent.TileType<AbyssalDirt>()))
                {
                    continue;
                }


                // place the Rogue
                //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                //Main.npc[num].homeTileX = -1;
                //	Main.npc[num].homeTileY = -1;
                //	Main.npc[num].direction = 1;
                //	Main.npc[num].homeless = true;



                for (int da = 0; da < 1; da++)
                {
                    Point Loc = new Point(abysmx - 150, abysmy + 200);

                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Aurelus/Rallad");
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                            Tuple.Create(ModContent.ItemType<OldCarianTome>(), 0.5)


                        // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(7))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<MagnusMagnum>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
                                break;
                            case 1:
                                itemsToAdd.Add((ModContent.ItemType<Venatici>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(100, 1500)));

                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                break;
                            case 2:
                                itemsToAdd.Add((ModContent.ItemType<Neptune8Card>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(100, 1500)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));

                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;
                            case 3:
                                itemsToAdd.Add((ModContent.ItemType<TON618Crossbow>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(100, 1500)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

                                break;
                            case 4:
                                itemsToAdd.Add((ModContent.ItemType<HolmbergScythe>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
                                itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
                                itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(100, 1500)));

                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
                                itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
                                break;

                            case 5:
                                itemsToAdd.Add((ModContent.ItemType<AurelusBlightBroochA>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(100, 1500)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 10)));
                                itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
                                break;

                            case 6:
                                itemsToAdd.Add((ModContent.ItemType<AbyssalPowder>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ItemID.Shiverthorn, Main.rand.Next(2, 15)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
                                itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(100, 1500)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 10)));
                                itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
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
                }

                placed = true;
            }


        }
        #endregion


        #region Veil Biome

        Point pointL;
		Point pointLil;

		private void WorldGenVeilSpot(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Residents of the veil believing in a god";


			int[] tileBlend = new int[]
			{
				TileID.RubyGemspark
			};

			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 10000000)
			{
				// Select a place in the first 6th of the world, avoiding the oceans
				int smx = WorldGen.genRand.Next( 300, (Main.maxTilesX - 1000)); // from 50 since there's a unaccessible area at the world's borders
																										  // 50% of choosing the last 6th of the world
																										  // Choose which side of the world to be on randomly
				///if (WorldGen.genRand.NextBool())
				///{
				///	towerX = Main.maxTilesX - towerX;
				///}

				//Start at 200 tiles above the surface instead of 0, to exclude floating islands
				int smy = ((int)(Main.worldSurface - 200));

				// We go down until we hit a solid tile or go under the world's surface
				while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
				{
					smy++;
				}

				// If we went under the world's surface, try again
				if (smy > Main.worldSurface - 20)
				{
					continue;
				}
				Tile tile = Main.tile[smx, smy];
				// If the type of the tile we are placing the tower on doesn't match what we want, try again
				if (!(tile.TileType == TileID.IceBlock
					|| tile.TileType == TileID.SnowBlock))
				{
					continue;
				}


				// place the Rogue
				//	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
				//Main.npc[num].homeTileX = -1;
				//	Main.npc[num].homeTileY = -1;
				//	Main.npc[num].direction = 1;
				//	Main.npc[num].homeless = true;



				for (int da = 0; da < 1; da++)
				{




					Point Loc = new Point(smx, smy + 343);
					NPCs.Town.AlcadSpawnSystem.LiberatTile = Loc;
					//


					for (int daa = 0; daa < 1; daa++)
					{
						Point Loc7 = new Point(smx, smy);
						WorldGen.TileRunner(Loc7.X + 275, Loc7.Y + 100, 600, 2, ModContent.TileType<Tiles.CatagrassBlock>(), false, 0f, 0f, true, true);



					}
					
					pointLil = new Point(smx + 80, smy + 330);


					//This code just places

					
					//	WorldUtils.Gen(Loc6, new Shapes.Circle(40), new Actions.SetTile(TileID.Dirt));
					//	Point resultPoint;
					//	bool searchSuccessful = WorldUtils.Find(Loc, Searches.Chain(new Searches.Right(200), new GenCondition[]
					//	{
					//new Conditions.IsSolid().AreaAnd(10, 10),
					//new Conditions.IsTile(TileID.Sand).AreaAnd(10, 10),
					//	}), out resultPoint);
					//		if (searchSuccessful)
					//		{
					//			WorldGen.TileRunner(resultPoint.X, resultPoint.Y, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(150, 150), TileID.Dirt);
					//		}

				

			
					//WorldGen.TileRunner(Loc2.X - 10, Loc2.Y - 60, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(120, 120), TileID.Grass);
					//WorldGen.TileRunner(Loc3.X - 20, Loc2.Y, WorldGen.genRand.Next(40, 43), WorldGen.genRand.Next(100, 100), TileID.Grass);
					//WorldGen.TileRunner(Loc3.X - 20, Loc3.Y + 20, WorldGen.genRand.Next(40, 43), WorldGen.genRand.Next(100, 100), TileID.Grass);
					placed = true;
				}


			}

		}


		private void WorldGenVU(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Residents of the veil crafting chasms";


			int[] tileBlend = new int[]
			{
				TileID.RubyGemspark
			};

			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 10000000)
			{
				// Select a place in the first 6th of the world, avoiding the oceans
				int smx = WorldGen.genRand.Next(((Main.maxTilesX) / 2) + 300, (Main.maxTilesX - 1000)); // from 50 since there's a unaccessible area at the world's borders
																										// 50% of choosing the last 6th of the world
																										// Choose which side of the world to be on randomly
				///if (WorldGen.genRand.NextBool())
				///{
				///	towerX = Main.maxTilesX - towerX;
				///}

				//Start at 200 tiles above the surface instead of 0, to exclude floating islands
				int smy = ((int)(Main.worldSurface - 200));

				// We go down until we hit a solid tile or go under the world's surface
				while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
				{
					smy++;
				}

				// If we went under the world's surface, try again
				if (smy > Main.worldSurface - 20)
				{
					continue;
				}
				Tile tile = Main.tile[smx, smy];
				// If the type of the tile we are placing the tower on doesn't match what we want, try again
				if (!(tile.TileType == TileID.Dirt
					|| tile.TileType == TileID.Grass
					|| tile.TileType == TileID.Mud
					|| tile.TileType == TileID.Stone))
				{
					continue;
				}


				// place the Rogue
				//	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
				//Main.npc[num].homeTileX = -1;
				//	Main.npc[num].homeTileY = -1;
				//	Main.npc[num].direction = 1;
				//	Main.npc[num].homeless = true;



				for (int da = 0; da < 1; da++)
				{




				
					NPCs.Town.AlcadSpawnSystem.LiberatTile = pointLil;
                    NPCs.Town.AlcadSpawnSystem.JhoviaTile = pointLil;
                    //

                    //This code just places
                    int width = 253;
					int height = 50;
					ShapeData shapeData = new ShapeData();

					StructureLoader.ReadStruct(pointLil, "Struct/Underground/Catacombz", tileBlend);


					Point Loc22 = new Point(pointLil.X + 40, pointLil.Y - 335);

					StructureLoader.ReadStruct(Loc22, "Struct/Morrow/Morrowtop");
					
			
					//			WorldUtils.Gen(Loc22, new Shapes.Rectangle(240, -40), new Actions.ClearTile(true));

					
					Point Loc4 = new Point(smx + 233, smy + 45);
					//	WorldUtils.Gen(Loc2, new Shapes.Mound(60, 90), new Actions.SetTile(TileID.Dirt));
					//	WorldUtils.Gen(Loc4, new Shapes.Rectangle(220, 105), new Actions.SetTile(TileID.Dirt));

					Point Loc5 = new Point(smx + 10, smy + 45);
					//	WorldUtils.Gen(Loc5, new Shapes.Rectangle(220, 50), new Actions.SetTile(TileID.Dirt));



					Point Loc3 = new Point(smx + 455, smy + 30);
					//	WorldUtils.Gen(Loc3, new Shapes.Mound(40, 50), new Actions.SetTile(TileID.Dirt));
					Point Loc6 = new Point(smx + 455, smy + 40);
					//	WorldUtils.Gen(Loc6, new Shapes.Circle(40), new Actions.SetTile(TileID.Dirt));
					//	Point resultPoint;
					//	bool searchSuccessful = WorldUtils.Find(Loc, Searches.Chain(new Searches.Right(200), new GenCondition[]
					//	{
					//new Conditions.IsSolid().AreaAnd(10, 10),
					//new Conditions.IsTile(TileID.Sand).AreaAnd(10, 10),
					//	}), out resultPoint);
					//		if (searchSuccessful)
					//		{
					//			WorldGen.TileRunner(resultPoint.X, resultPoint.Y, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(150, 150), TileID.Dirt);
					//		}



					GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 233, 346));
					//WorldGen.TileRunner(Loc2.X - 10, Loc2.Y - 60, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(120, 120), TileID.Grass);
					//WorldGen.TileRunner(Loc3.X - 20, Loc2.Y, WorldGen.genRand.Next(40, 43), WorldGen.genRand.Next(100, 100), TileID.Grass);
					//WorldGen.TileRunner(Loc3.X - 20, Loc3.Y + 20, WorldGen.genRand.Next(40, 43), WorldGen.genRand.Next(100, 100), TileID.Grass);
					placed = true;
				}


			}

		}

        #endregion


        #region Royal Capital
      

		public void WorldGenRoyalCapital(GenerationProgress progress, GameConfiguration configuration)
		{
			StructureMap structures = GenVars.structures;
			Rectangle rectangle = StructureLoader.ReadRectangle("Struct/Alcad/RoyalCapital3");
			progress.Message = "Fighting the Virulent with magic";





			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 10000000)
			{
				// Select a place in the first 6th of the world, avoiding the oceans
				int smx = WorldGen.genRand.Next(400, (Main.maxTilesX) / 15); // from 50 since there's a unaccessible area at the world's borders
																			// 50% of choosing the last 6th of the world
																			// Choose which side of the world to be on randomly
				///if (WorldGen.genRand.NextBool())
				///{
				///	towerX = Main.maxTilesX - towerX;
				///}

				//Start at 200 tiles above the surface instead of 0, to exclude floating islands
				int smy = ((int)(Main.worldSurface - 200));

				// We go down until we hit a solid tile or go under the world's surface
				while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
				{
					smy++;
				}

				// If we went under the world's surface, try again
				if (smy > Main.worldSurface - 20)
				{
					continue;
				}
				Tile tile = Main.tile[smx, smy];
				// If the type of the tile we are placing the tower on doesn't match what we want, try again
				if ((tile.TileType == TileID.Grass))
				{
					continue;
				}

				int smxx = smx;
				int smyy = smy;
				// place the Rogue
				//	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
				//Main.npc[num].homeTileX = -1;
				//	Main.npc[num].homeTileY = -1;
				//	Main.npc[num].direction = 1;
				//	Main.npc[num].homeless = true;



				for (int da = 0; da < 1; da++)
				{
                    pointAlcadthingy = new Point(smx - 10, smyy + 3);

					    WorldGen.TileRunner(pointAlcadthingy.X + 260, pointAlcadthingy.Y + 100, 500, 2, ModContent.TileType<Tiles.StarbloomDirt>(), false, 0f, 0f, true, false);
                        WorldGen.TileRunner(pointAlcadthingy.X + 260, pointAlcadthingy.Y + 250, 300, 2, ModContent.TileType<Tiles.StarbloomDirt>(), true, 0f, 0f, true, true);
                        WorldGen.TileRunner(pointAlcadthingy.X + 260, pointAlcadthingy.Y + 400, 500, 2, ModContent.TileType<Tiles.StarbloomDirt>(), true, 0f, 0f, true, true);
                        WorldGen.TileRunner(pointAlcadthingy.X + 260, pointAlcadthingy.Y + 600, 500, 2, ModContent.TileType<Tiles.StarbloomDirt>(), true, 0f, 0f, true, true);


                       
                    

                    Point Loc = new Point(smx + 20, smyy + 3);
					
					rectangle.Location = Loc;
                    NPCs.Town.AlcadSpawnSystem.AlcadTile = Loc;
                    Structurizer.ProtectStructure(Loc, "Struct/Alcad/RoyalCapital3");
                    int[] ChestIndexs = Structurizer.ReadStruct(Loc, "Struct/Alcad/RoyalCapital3");
					foreach (int chestIndex in ChestIndexs)
					{
						var chest = Main.chest[chestIndex];
						// etc

						// itemsToAdd will hold type and stack data for each item we want to add to the chest
						var itemsToAdd = new List<(int type, int stack)>();

						// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
						int specialItem = new Terraria.Utilities.WeightedRandom<int>(

							Tuple.Create(ModContent.ItemType<LostScrap>(), 0.1),
							Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

						// Choose no item with a high weight of 7.
						);
						if (specialItem != ItemID.None)
						{
							itemsToAdd.Add((specialItem, 1));
						}
						// Using a switch statement and a random choice to add sets of items.
						switch (Main.rand.Next(6))
						{
							case 0:
								itemsToAdd.Add((ModContent.ItemType<LittleWand>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));

								 ;
								itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
								break;
							case 1:
								itemsToAdd.Add((ModContent.ItemType<AlcaricQuiver>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
								

								itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
								break;
							case 2:
								itemsToAdd.Add((ModContent.ItemType<BlackRose>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
								itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(20, 30)));

								itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
								break;
							case 3:
								itemsToAdd.Add((ModContent.ItemType<FloweredInsource>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
								itemsToAdd.Add((ModContent.ItemType<AlcadizScrap>(), Main.rand.Next(5, 20)));

								itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

								break;
							case 4:
								itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(20, 30)));

								itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
								break;

							case 5:
								itemsToAdd.Add((ItemID.FuneralHat, Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
								itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
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
				}

				placed = true;



				
			}
		}

        #endregion

        #region Illuria
        public void WorldGenIlluria(GenerationProgress progress, GameConfiguration configuration)
		{
			StructureMap structures = GenVars.structures;
			Rectangle rectangle = StructureLoader.ReadRectangle("Struct/Overworld/Illuria");
			progress.Message = "Niivi protecting the cities above.";





			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 10000000)
			{
				// Select a place in the first 6th of the world, avoiding the oceans
				int smx = WorldGen.genRand.Next((Main.maxTilesX) - (Main.maxTilesX / 15), (Main.maxTilesX - 250)); // from 50 since there's a unaccessible area at the world's borders
																			 // 50% of choosing the last 6th of the world
																			 // Choose which side of the world to be on randomly
				///if (WorldGen.genRand.NextBool())
				///{
				///	towerX = Main.maxTilesX - towerX;
				///}

				//Start at 200 tiles above the surface instead of 0, to exclude floating islands
				int smy = ((int)(Main.worldSurface - 200));

				// We go down until we hit a solid tile or go under the world's surface
				while (!WorldGen.SolidTile(smx, smy) && smy <= Main.worldSurface)
				{
					smy++;
				}

				// If we went under the world's surface, try again
				if (smy > Main.worldSurface - 20)
				{
					continue;
				}
				Tile tile = Main.tile[smx, smy];
				// If the type of the tile we are placing the tower on doesn't match what we want, try again
				if ((tile.TileType == TileID.Grass))
				{
					continue;
				}

				int smxx = smx;
				int smyy = smy - 20;
				// place the Rogue
				//	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
				//Main.npc[num].homeTileX = -1;
				//	Main.npc[num].homeTileY = -1;
				//	Main.npc[num].direction = 1;
				//	Main.npc[num].homeless = true;



				for (int da = 0; da < 1; da++)
				{
					Point Loc = new Point(smx - 270, smyy - 20);

					rectangle.Location = Loc;
					// NPCs.Town.AlcadSpawnSystem.AlcadTile = Loc;
					StructureLoader.ProtectStructure(Loc, "Struct/Overworld/Illuria");
					int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Overworld/Illuria");
					NPCs.Town.AlcadSpawnSystem.IlluriaTile = Loc;
					foreach (int chestIndex in ChestIndexs)
					{
						var chest = Main.chest[chestIndex];
						// etc

						// itemsToAdd will hold type and stack data for each item we want to add to the chest
						var itemsToAdd = new List<(int type, int stack)>();

						// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
						int specialItem = new Terraria.Utilities.WeightedRandom<int>(

							Tuple.Create(ModContent.ItemType<AlcadizScrap>(), 0.5),
							Tuple.Create(ModContent.ItemType<LostScrap>(), 0.1),
							Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

						// Choose no item with a high weight of 7.
						);
						if (specialItem != ItemID.None)
						{
							itemsToAdd.Add((specialItem, 1));
						}
						// Using a switch statement and a random choice to add sets of items.
						switch (Main.rand.Next(6))
						{
							case 0:
								itemsToAdd.Add((ModContent.ItemType<LittleWand>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(5, 20)));
								 ;
								itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
								break;
							case 1:
								itemsToAdd.Add((ModContent.ItemType<AlcaricQuiver>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
								
								itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
								itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
								break;
							case 2:
								itemsToAdd.Add((ModContent.ItemType<BlackRose>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
								itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
								itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
								break;
							case 3:
								itemsToAdd.Add((ModContent.ItemType<FloweredInsource>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
								itemsToAdd.Add((ModContent.ItemType<AlcadizScrap>(), Main.rand.Next(5, 20)));
								itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
								itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

								break;
							case 4:
								itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
								itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
								break;

							case 5:
								itemsToAdd.Add((ItemID.FuneralHat, Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
								itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
								itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
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
				}

				placed = true;




			}
		}

        private void WorldGenWorshipingTowers(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Praising our lord and savior Niivi";

            int leftmostJungleTileX = int.MaxValue;
            int rightmostJungleTileX = int.MinValue;
            for (int x = 500; x < Main.maxTilesX - 500; x++)
            {
                int jungleY = (int)(Main.worldSurface - 50);
                while (!WorldGen.SolidTile(x, jungleY) && jungleY <= Main.worldSurface)
                {
                    jungleY++;
                }

                Tile tile = Main.tile[x, jungleY];
                if (tile.TileType == TileID.Mud)
                {
                    if (leftmostJungleTileX > x)
                        leftmostJungleTileX = x;
                    if (rightmostJungleTileX < x)
                        rightmostJungleTileX = x;
                }
            }

            string[] structures = new string[]
            {
                "Struct/Jungle/WorshipingTower1",
                "Struct/Jungle/WorshipingTower2",
                "Struct/Jungle/WorshipingTower3"
            };

            int[] tileBlend = new int[]
            {
                TileID.RubyGemspark
            };


            int numberToPlace = Main.rand.Next(10, 17);
            int attempts = 0;
            int maxAttempts = 100000;
            for (int k = 0; k < numberToPlace; k++)
            {
                bool placed = false;
                if (attempts > maxAttempts)
                    break;

                while (!placed)
                {
                    attempts++;
                    if (attempts > maxAttempts)
                        break;
                    int xa = WorldGen.genRand.Next(leftmostJungleTileX, rightmostJungleTileX);
                    int ya = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY);
                    Point Loc = new Point(xa, ya);

                    // 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
                    Tile tile = Main.tile[Loc.X, Loc.Y];

                    if (!(tile.TileType == TileID.Mud))
                    {
                        continue;
                    }

                    if (!tile.HasTile)
                        continue;

                    string randomStructure = structures[Main.rand.Next(0, structures.Length)];


                    //Avoid the temple
                    Rectangle structureRect = StructureLoader.ReadRectangle(randomStructure);
                    structureRect.Location = Loc;
                    if (!StructureLoader.TryPlaceAndProtectStructure(Loc, randomStructure))
                        continue;

                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, randomStructure, tileBlend);

                    //GUARDS!!!
                    IllurianGuardSpawnSystem.Add(Loc, randomStructure);
                    placed = true;
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        int tileType = Main.tile[chest.x, chest.y].TileType;
                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        if (tileType == ModContent.TileType<IlluriaChest>())
                        {
                            //Illura Chest Loot here
                            switch (Main.rand.Next(6))
                            {
                                case 0:
                                    //Illuria Brooch
                                    itemsToAdd.Add((ModContent.ItemType<IllurianBroochA>(), 1));
                                    break;
                                case 1:
                                    //Alcalite Set
                                    itemsToAdd.Add((ModContent.ItemType<AlcaliteMask>(), 1));
                                    itemsToAdd.Add((ModContent.ItemType<AlcaliteRobe>(), 1));
                                    itemsToAdd.Add((ModContent.ItemType<AlcaliteTrunks>(), 1));
                                    break;
                                case 2:
                                    //Illurite Dril
                                    itemsToAdd.Add((ModContent.ItemType<IlluriteDrill>(), 1));
                                    break;
                                case 3:
                                    itemsToAdd.Add((ModContent.ItemType<IllurianLoveLocket>(), 1));
                                    break;
                                case 4:
                                    itemsToAdd.Add((ModContent.ItemType<MsFreeze>(), 1));
                                    break;
                                case 5:
                                    itemsToAdd.Add((ModContent.ItemType<IllurianBible>(), 1));
                                    break;
                            }

                            switch (Main.rand.Next(1))
                            {
                                case 0:
                                    itemsToAdd.Add((ModContent.ItemType<IllurineScale>(), Main.rand.Next(2, 5)));
                                    itemsToAdd.Add((ItemID.LifeFruit, Main.rand.Next(1, 4)));
                                    itemsToAdd.Add((ItemID.Ectoplasm, Main.rand.Next(2, 5)));
                                    break;
                            }
                        }
                        else
                        {
                            //Jungle Loot Here
                            int specialItem = new Terraria.Utilities.WeightedRandom<int>(
                                Tuple.Create((int)ItemID.AnkletoftheWind, 0.5),
                                Tuple.Create((int)ItemID.StaffofRegrowth, 0.5),
                                Tuple.Create((int)ItemID.FlowerBoots, 0.5),
                                Tuple.Create((int)ItemID.Boomstick, 0.5),
                                Tuple.Create(ModContent.ItemType<JungleRuneI>(), 0.15)
                                );

                            itemsToAdd.Add((specialItem, 1));
                            switch (Main.rand.Next(4))
                            {
                                case 0:
                                    itemsToAdd.Add((ModContent.ItemType<FlowerBatch>(), Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.JungleSpores, Main.rand.Next(2, 5)));
                                    itemsToAdd.Add((ItemID.Stinger, Main.rand.Next(3, 7)));
                                    break;
                                case 1:
                                    itemsToAdd.Add((ItemID.Vine, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.JungleSpores, Main.rand.Next(2, 5)));
                                    itemsToAdd.Add((ItemID.Stinger, Main.rand.Next(3, 7)));
                                    break;
                                case 2:
                                    itemsToAdd.Add((ModContent.ItemType<FlowerBatch>(), Main.rand.Next(1, 1)));
                                    itemsToAdd.Add((ItemID.Vine, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.CalmingPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                    break;
                                case 3:
                                    itemsToAdd.Add((ItemID.Stinger, Main.rand.Next(3, 7)));
                                    itemsToAdd.Add((ItemID.RecallPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 3)));
                                    itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                    break;
                            }
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
                }
            }
        }

        #endregion
        
        #region Ores
        private void WorldGenFlameOre(GenerationProgress progress, GameConfiguration configuration)
		{
			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Scorching Gild and Arnchar burning into the world";


			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
			{
				// 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
				int x = WorldGen.genRand.Next(0, Main.maxTilesX / 2);
				int y = WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY);

				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 14), WorldGen.genRand.Next(2, 9), ModContent.TileType<VerianoreTile>());

				
			}

		
				// 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
				
			
		}
	
		private void WorldGenMoreFlameOre(GenerationProgress progress, GameConfiguration configuration)
		{
			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Scorching more Arnchar into the world";


			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
			{


				int xz = WorldGen.genRand.Next(0, Main.maxTilesX);
				int yz = WorldGen.genRand.Next(Main.UnderworldLayer - (Main.maxTilesY / 20), Main.UnderworldLayer);

				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				WorldGen.TileRunner(xz, yz, WorldGen.genRand.Next(4, 20), WorldGen.genRand.Next(5, 15), ModContent.TileType<VerianoreTile>(),false,0,0,true,true,-1);
			}




			// 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.


		}
		private void WorldGenFrileOre(GenerationProgress progress, GameConfiguration configuration)
		{
			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Freezing the world with Frile";


			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
			{
				// 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
				int x = WorldGen.genRand.Next(0, Main.maxTilesX);
				int y = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY);

				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 10), WorldGen.genRand.Next(2, 10), ModContent.TileType<FrileOreTile>());
			}
		}
		private void WorldGenDarkstone(GenerationProgress progress, GameConfiguration configuration)
		{
			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Blackening Stones for racist effect";


			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
			{
				// 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
				int x = WorldGen.genRand.Next(0, Main.maxTilesX);
				int y = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY);

				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 50), WorldGen.genRand.Next(2, 150), ModContent.TileType<DiminishedStone>());
			}


			//NO MOSSY STONES
			/*
			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
			{


				int xz = WorldGen.genRand.Next(0, Main.maxTilesX);
				int yz = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY - 300);
				Tile tile = Main.tile[xz, yz];
				// If the type of the tile we are placing the tower on doesn't match what we want, try again
				if ((tile.TileType == TileID.JungleGrass
					|| tile.TileType == TileID.Mud
					|| tile.TileType == TileID.Stone))
				{
					continue;
				}
				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				WorldGen.TileRunner(xz, yz, WorldGen.genRand.Next(3, 50), WorldGen.genRand.Next(2, 100), ModContent.TileType<MossyStone>());
			}*/

			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
			{


				int xz = WorldGen.genRand.Next(0, Main.maxTilesX);
				int yz = WorldGen.genRand.Next((int)GenVars.worldSurface, Main.maxTilesY - 300);
				Tile tile = Main.tile[xz, yz];
				// If the type of the tile we are placing the tower on doesn't match what we want, try again
				if ((tile.TileType == TileID.Stone
					|| tile.TileType == TileID.Grass))
				{
					continue;
				}
				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				WorldGen.TileRunner(xz, yz, WorldGen.genRand.Next(3, 50), WorldGen.genRand.Next(2, 100), ModContent.TileType<VeriplantGrass>());
			}

		}



        #endregion

        #region Small Underground Structures
        private void WorldGenAbandonedMineshafts(GenerationProgress progress, GameConfiguration configuration)
		{
			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Getting shafted";
			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05 - 3); k++)
			{

				int xa = WorldGen.genRand.Next(500, Main.maxTilesX - 500);
				int ya = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY);
				Point Loc = new Point(xa, ya);

				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				Tile tile = Main.tile[Loc.X, Loc.Y];

				if (!(tile.TileType == TileID.Stone))
				{
					continue;
				}

				if (tile.HasTile)
				{
					int Sounda = Main.rand.Next(1, 6);
					if (Sounda == 1)
					{
						for (int da = 0; da < 1; da++)
						{
                            if (!StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Underground/AbandonedMineshaft1"))
                                continue;

                            int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Underground/AbandonedMineshaft1");
							foreach (int chestIndex in ChestIndexs)
							{
								var chest = Main.chest[chestIndex];
								// etc

								// itemsToAdd will hold type and stack data for each item we want to add to the chest
								var itemsToAdd = new List<(int type, int stack)>();

								// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
								int specialItem = new Terraria.Utilities.WeightedRandom<int>(

									Tuple.Create(ModContent.ItemType<AlcadizMetal>(), 0.5),
									Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

								// Choose no item with a high weight of 7.
								);
								if (specialItem != ItemID.None)
								{
									itemsToAdd.Add((specialItem, 1));
								}
								// Using a switch statement and a random choice to add sets of items.
								switch (Main.rand.Next(9))
								{
									case 0:
										itemsToAdd.Add((ModContent.ItemType<LifeSeekingVial>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 3)));
										itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.TinOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
										break;
									case 1:
										itemsToAdd.Add((ModContent.ItemType<KnivedQuiver>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 3)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
										
										itemsToAdd.Add((ItemID.CopperOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
										break;
									case 2:
										itemsToAdd.Add((ItemID.MiningPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 3)));
										itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.CopperOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										break;
									case 3:
                                        itemsToAdd.Add((ItemID.MiningHelmet, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 4)));
										itemsToAdd.Add((ItemID.MiningPants, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 100)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
										itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

										break;
									case 4:
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 10)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 10)));
										itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.LeadOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
										break;

									case 5:
										itemsToAdd.Add((ItemID.IronBar, Main.rand.Next(1, 40)));
										itemsToAdd.Add((ItemID.Deathweed, Main.rand.Next(2, 25)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
										break;

									case 6:
										itemsToAdd.Add((ModContent.ItemType<AlcadizDagger>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.Waterleaf, Main.rand.Next(2, 25)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(2, 10)));
										itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
										break;


									case 7:
										itemsToAdd.Add((ItemID.WaterWalkingBoots, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.IronOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
										break;

									case 8:
										itemsToAdd.Add((ItemID.MiningShirt, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<StumpBuster>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 6)));
										
										itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.MiningPotion, Main.rand.Next(1, 7)));
										break;

									case 9:
										itemsToAdd.Add((ModContent.ItemType<VeiledScriptureMiner1>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 6)));
										
										itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.MiningPotion, Main.rand.Next(1, 7)));
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





						}
					}



					if (Sounda == 2)
					{


						for (int da = 0; da < 1; da++)
						{

                            if (!StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Underground/AbandonedMineshaft2"))
                                continue;

                            int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Underground/AbandonedMineshaft2");
							foreach (int chestIndex in ChestIndexs)
							{
								var chest = Main.chest[chestIndex];
								// etc

								// itemsToAdd will hold type and stack data for each item we want to add to the chest
								var itemsToAdd = new List<(int type, int stack)>();

								// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
								int specialItem = new Terraria.Utilities.WeightedRandom<int>(

									Tuple.Create(ModContent.ItemType<AlcadizMetal>(), 0.5),
									Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

								// Choose no item with a high weight of 7.
								);
								if (specialItem != ItemID.None)
								{
									itemsToAdd.Add((specialItem, 1));
								}
								// Using a switch statement and a random choice to add sets of items.
								switch (Main.rand.Next(9))
								{
									case 0:
										itemsToAdd.Add((ModContent.ItemType<LifeSeekingVial>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										
										itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.TinOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
										break;
									case 1:
										itemsToAdd.Add((ModContent.ItemType<KnivedQuiver>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
										
										itemsToAdd.Add((ItemID.CopperOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
										break;
									case 2:
										itemsToAdd.Add((ItemID.MiningPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.CopperOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										break;
									case 3:
										itemsToAdd.Add((ItemID.MiningHelmet, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.MiningPants, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 100)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
										itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

									
										break;
									case 4:
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 10)));
										itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.LeadOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
										break;

									case 5:
										itemsToAdd.Add((ItemID.IronBar, Main.rand.Next(1, 40)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Deathweed, Main.rand.Next(2, 25)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
										break;

									case 6:
										itemsToAdd.Add((ModContent.ItemType<AlcadizDagger>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.Waterleaf, Main.rand.Next(2, 25)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(2, 10)));
										itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
										break;


									case 7:
										itemsToAdd.Add((ItemID.WaterWalkingBoots, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 30)));
										itemsToAdd.Add((ModContent.ItemType<StumpBuster>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.IronOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
										break;

									case 8:
										itemsToAdd.Add((ItemID.MiningShirt, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 6)));
										
										itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.MiningPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 3)));
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





						}
					}


					if (Sounda == 3)
					{


						for (int da = 0; da < 1; da++)
						{

                            if (!StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Underground/AbandonedMineshaft2"))
                                continue;

                            int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Underground/AbandonedMineshaft2");
							foreach (int chestIndex in ChestIndexs)
							{
								var chest = Main.chest[chestIndex];
								// etc

								// itemsToAdd will hold type and stack data for each item we want to add to the chest
								var itemsToAdd = new List<(int type, int stack)>();

								// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
								int specialItem = new Terraria.Utilities.WeightedRandom<int>(

									Tuple.Create(ModContent.ItemType<AlcadizMetal>(), 0.5),
									Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

								// Choose no item with a high weight of 7.
								);
								if (specialItem != ItemID.None)
								{
									itemsToAdd.Add((specialItem, 1));
								}
								// Using a switch statement and a random choice to add sets of items.
								switch (Main.rand.Next(9))
								{
									case 0:
										itemsToAdd.Add((ModContent.ItemType<LifeSeekingVial>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 3)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										
										itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.TinOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
										break;
									case 1:
										itemsToAdd.Add((ModContent.ItemType<KnivedQuiver>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 3)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
										
										itemsToAdd.Add((ItemID.CopperOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
										break;
									case 2:
										itemsToAdd.Add((ItemID.MiningPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.CopperOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										break;
									case 3:
										itemsToAdd.Add((ItemID.MiningHelmet, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.MiningPants, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 100)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
										itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

										break;
									case 4:
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 10)));
										itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.LeadOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
										break;

									case 5:
										itemsToAdd.Add((ItemID.IronBar, Main.rand.Next(1, 40)));
										itemsToAdd.Add((ItemID.Deathweed, Main.rand.Next(2, 25)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
										break;

									case 6:
										itemsToAdd.Add((ModContent.ItemType<AlcadizDagger>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.Waterleaf, Main.rand.Next(2, 25)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 3)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(2, 10)));
										itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
										break;


									case 7:
										itemsToAdd.Add((ItemID.WaterWalkingBoots, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.IronOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
										break;

									case 8:
										itemsToAdd.Add((ItemID.MiningShirt, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(1, 10)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 6)));
										
										itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.MiningPotion, Main.rand.Next(1, 7)));
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





						}







					}



					if (Sounda == 4)
					{


						for (int da = 0; da < 1; da++)
						{

                            if (!StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Underground/AbandonedMineshaft3"))
                                continue;

                            int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Underground/AbandonedMineshaft3");
							foreach (int chestIndex in ChestIndexs)
							{
								var chest = Main.chest[chestIndex];
								// etc

								// itemsToAdd will hold type and stack data for each item we want to add to the chest
								var itemsToAdd = new List<(int type, int stack)>();

								// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
								int specialItem = new Terraria.Utilities.WeightedRandom<int>(

									Tuple.Create(ModContent.ItemType<AlcadizMetal>(), 0.5),
									Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

								// Choose no item with a high weight of 7.
								);
								if (specialItem != ItemID.None)
								{
									itemsToAdd.Add((specialItem, 1));
								}
								// Using a switch statement and a random choice to add sets of items.
								switch (Main.rand.Next(9))
								{
									case 0:
										itemsToAdd.Add((ModContent.ItemType<LifeSeekingVial>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										
										itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.TinOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
										break;
									case 1:
										itemsToAdd.Add((ModContent.ItemType<KnivedQuiver>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
										
										itemsToAdd.Add((ItemID.CopperOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
										break;
									case 2:
										itemsToAdd.Add((ItemID.MiningPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 3)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.CopperOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										break;
									case 3:
										itemsToAdd.Add((ItemID.MiningHelmet, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.MiningPants, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 100)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 3)));
										itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

										break;
									case 4:
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 10)));
										itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.LeadOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 3)));
										itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
										break;

									case 5:
										itemsToAdd.Add((ItemID.IronBar, Main.rand.Next(1, 40)));
										itemsToAdd.Add((ItemID.Deathweed, Main.rand.Next(2, 25)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(1, 30)));
										itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
										break;

									case 6:
										itemsToAdd.Add((ModContent.ItemType<AlcadizDagger>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 30)));
										itemsToAdd.Add((ItemID.Waterleaf, Main.rand.Next(2, 25)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(2, 3)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(2, 10)));
										itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
										break;


									case 7:
										itemsToAdd.Add((ItemID.WaterWalkingBoots, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.IronOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
										break;

									case 8:
										itemsToAdd.Add((ItemID.MiningShirt, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<StumpBuster>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 6)));
										
										itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.MiningPotion, Main.rand.Next(1, 7)));
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


						}




					}

					if (Sounda == 5)
					{


						for (int da = 0; da < 1; da++)
						{
                            if (!StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Underground/AbandonedMineshaft4"))
                                continue;


                            int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Underground/AbandonedMineshaft4");
							foreach (int chestIndex in ChestIndexs)
							{
								var chest = Main.chest[chestIndex];
								// etc

								// itemsToAdd will hold type and stack data for each item we want to add to the chest
								var itemsToAdd = new List<(int type, int stack)>();

								// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
								int specialItem = new Terraria.Utilities.WeightedRandom<int>(

									Tuple.Create(ModContent.ItemType<AlcadizMetal>(), 0.5),
									Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

								// Choose no item with a high weight of 7.
								);
								if (specialItem != ItemID.None)
								{
									itemsToAdd.Add((specialItem, 1));
								}
								// Using a switch statement and a random choice to add sets of items.
								switch (Main.rand.Next(9))
								{
									case 0:
										itemsToAdd.Add((ModContent.ItemType<LifeSeekingVial>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										
										itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.TinOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
										break;
									case 1:
										itemsToAdd.Add((ModContent.ItemType<KnivedQuiver>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 3)));
										itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
										
										itemsToAdd.Add((ItemID.CopperOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
										break;
									case 2:
										itemsToAdd.Add((ItemID.MiningPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.CopperOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										break;
									case 3:
										itemsToAdd.Add((ItemID.MiningHelmet, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.MiningPants, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 100)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(2, 20)));
										itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
										itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

										break;
									case 4:
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 10)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(1, 3)));
										itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.LeadOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
										break;

									case 5:
										itemsToAdd.Add((ItemID.IronBar, Main.rand.Next(1, 40)));
										itemsToAdd.Add((ItemID.Deathweed, Main.rand.Next(2, 25)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
										break;

									case 6:
										itemsToAdd.Add((ModContent.ItemType<AlcadizDagger>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.Waterleaf, Main.rand.Next(2, 25)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(2, 10)));
										itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
										break;


									case 7:
										itemsToAdd.Add((ItemID.WaterWalkingBoots, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.IronOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
										break;

									case 8:
										itemsToAdd.Add((ItemID.MiningShirt, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 6)));
										itemsToAdd.Add((ModContent.ItemType<UnknownCircuitry>(), Main.rand.Next(1, 3)));
										
										itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.MiningPotion, Main.rand.Next(1, 7)));
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


						}




					}



					if (Sounda == 6)
					{


						for (int da = 0; da < 1; da++)
						{
                            if (!StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Underground/AbandonedMineshaft4"))
                                continue;


                            int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Underground/AbandonedMineshaft4");
							foreach (int chestIndex in ChestIndexs)
							{
								var chest = Main.chest[chestIndex];
								// etc

								// itemsToAdd will hold type and stack data for each item we want to add to the chest
								var itemsToAdd = new List<(int type, int stack)>();

								// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
								int specialItem = new Terraria.Utilities.WeightedRandom<int>(

									Tuple.Create(ModContent.ItemType<AlcadizMetal>(), 0.5),
									Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

								// Choose no item with a high weight of 7.
								);
								if (specialItem != ItemID.None)
								{
									itemsToAdd.Add((specialItem, 1));
								}
								// Using a switch statement and a random choice to add sets of items.
								switch (Main.rand.Next(9))
								{
									case 0:
										itemsToAdd.Add((ModContent.ItemType<LifeSeekingVial>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										
										itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.TinOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
										break;
									case 1:
										itemsToAdd.Add((ModContent.ItemType<KnivedQuiver>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
										
										itemsToAdd.Add((ItemID.CopperOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
										break;
									case 2:
										itemsToAdd.Add((ItemID.MiningPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.CopperOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										break;
									case 3:
										itemsToAdd.Add((ItemID.MiningHelmet, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.MiningPants, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 100)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
										itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

										break;
									case 4:
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 10)));
										itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.LeadOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
										break;

									case 5:
										itemsToAdd.Add((ItemID.IronBar, Main.rand.Next(1, 40)));
										itemsToAdd.Add((ItemID.Deathweed, Main.rand.Next(2, 25)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
										break;

									case 6:
										itemsToAdd.Add((ModContent.ItemType<AlcadizDagger>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.Waterleaf, Main.rand.Next(2, 25)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(2, 10)));
										itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
										break;


									case 7:
										itemsToAdd.Add((ItemID.WaterWalkingBoots, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.IronOre, Main.rand.Next(1, 100)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
										break;

									case 8:
										itemsToAdd.Add((ItemID.MiningShirt, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 6)));
										
										itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.MiningPotion, Main.rand.Next(1, 7)));
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


						}




					}









					


				}
			}
		}

		private void WorldGenUnderworldSpice(GenerationProgress progress, GameConfiguration configuration)
		{
			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Sylia using magic in the Underworld";





			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-06 + 10); k++)
			{

				int xa = WorldGen.genRand.Next(0, Main.maxTilesX);
				int ya = WorldGen.genRand.Next(Main.maxTilesY - 400, Main.maxTilesY - 50);
				Point Loc = new Point(xa, ya);

				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				Tile tile = Main.tile[Loc.X, Loc.Y];

				if (!(tile.TileType == TileID.Ash ||
					tile.TileType == TileID.Stone ||
					tile.TileType == ModContent.TileType<CindersparkDirt>()))
				{
					continue;
				}

				if (tile.HasTile)
				{
					int Sounda = Main.rand.Next(1, 6);
					if (Sounda == 1)
					{


						for (int da = 0; da < 1; da++)
						{


							int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Underworld/Underworld1");
							foreach (int chestIndex in ChestIndexs)
							{
								var chest = Main.chest[chestIndex];
								// etc

								// itemsToAdd will hold type and stack data for each item we want to add to the chest
								var itemsToAdd = new List<(int type, int stack)>();

								// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
								int specialItem = new Terraria.Utilities.WeightedRandom<int>(

									Tuple.Create(ModContent.ItemType<AlcaricMush>(), 0.5),
									Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

								// Choose no item with a high weight of 7.
								);
								if (specialItem != ItemID.None)
								{
									itemsToAdd.Add((specialItem, 1));
								}
								// Using a switch statement and a random choice to add sets of items.
								switch (Main.rand.Next(9))
								{
									case 0:
										itemsToAdd.Add((ModContent.ItemType<Infernis>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										
										itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
										break;
									case 1:
										itemsToAdd.Add((ItemID.FlameDye, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
										
										itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
										break;
									case 2:
										itemsToAdd.Add((ItemID.LavaproofTackleBag, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										
										itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
										break;
									case 3:
										itemsToAdd.Add((ItemID.ObsidianRose, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
										itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

										break;
									case 4:
										itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										
										itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
										itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
										break;

									case 5:
										itemsToAdd.Add((ItemID.LavaCharm, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
										break;

									case 6:
										itemsToAdd.Add((ItemID.Obsidian, Main.rand.Next(1, 20)));
										itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 15)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
										itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
										break;


									case 7:
										itemsToAdd.Add((ItemID.WaterWalkingBoots, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.Fireblossom, Main.rand.Next(2, 15)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
										break;

									case 8:
										itemsToAdd.Add((ItemID.ObsidianSkull, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										
										itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
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





						}
					}










					if (Sounda == 2)
					{


						for (int da = 0; da < 1; da++)
						{


							int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Underworld/Underworld2");
							foreach (int chestIndex in ChestIndexs)
							{
								var chest = Main.chest[chestIndex];
								// etc

								// itemsToAdd will hold type and stack data for each item we want to add to the chest
								var itemsToAdd = new List<(int type, int stack)>();

								// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
								int specialItem = new Terraria.Utilities.WeightedRandom<int>(

									Tuple.Create(ModContent.ItemType<Gambit>(), 0.5),
									Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

								// Choose no item with a high weight of 7.
								);
								if (specialItem != ItemID.None)
								{
									itemsToAdd.Add((specialItem, 1));
								}
								// Using a switch statement and a random choice to add sets of items.
								switch (Main.rand.Next(9))
								{
									case 0:
										itemsToAdd.Add((ModContent.ItemType<Infernis>(), Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										
										itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
										break;
									case 1:
										itemsToAdd.Add((ItemID.FlameDye, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
										
										itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
										break;
									case 2:
										itemsToAdd.Add((ItemID.LavaproofTackleBag, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										
										itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
										break;
									case 3:
										itemsToAdd.Add((ItemID.ObsidianRose, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
										itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

										break;
									case 4:
										itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										
										itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
										itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
										break;

									case 5:
										itemsToAdd.Add((ItemID.LavaCharm, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
										break;

									case 6:
										itemsToAdd.Add((ItemID.Obsidian, Main.rand.Next(1, 20)));
										itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 15)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
										break;


									case 7:
										itemsToAdd.Add((ItemID.WaterWalkingBoots, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.Fireblossom, Main.rand.Next(2, 15)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
										itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
										break;

									case 8:
										itemsToAdd.Add((ItemID.ObsidianSkull, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										
										itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
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





						}






						if (Sounda == 3 || Sounda == 4)
						{


							for (int da = 0; da < 1; da++)
							{


								int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Underworld/Underworld4");
								foreach (int chestIndex in ChestIndexs)
								{
									var chest = Main.chest[chestIndex];
									// etc

									// itemsToAdd will hold type and stack data for each item we want to add to the chest
									var itemsToAdd = new List<(int type, int stack)>();

									// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
									int specialItem = new Terraria.Utilities.WeightedRandom<int>(

										Tuple.Create(ModContent.ItemType<AlcaricMush>(), 0.5),
										Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

									// Choose no item with a high weight of 7.
									);
									if (specialItem != ItemID.None)
									{
										itemsToAdd.Add((specialItem, 1));
									}
									// Using a switch statement and a random choice to add sets of items.
									switch (Main.rand.Next(9))
									{
										case 0:
											itemsToAdd.Add((ModContent.ItemType<Infernis>(), Main.rand.Next(1, 1)));
											itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
											
											itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
											itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
											itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
											break;
										case 1:
											itemsToAdd.Add((ItemID.FlameDye, Main.rand.Next(1, 3)));
											itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
											itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
											itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
											
											itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
											itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
											break;
										case 2:
											itemsToAdd.Add((ItemID.LavaproofTackleBag, Main.rand.Next(1, 1)));
											itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
											itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
											itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
											
											itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
											break;
										case 3:
											itemsToAdd.Add((ItemID.ObsidianRose, Main.rand.Next(1, 1)));
											itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
											itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
											itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
											itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

											break;
										case 4:
											itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
											itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
											
											itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
											itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
											break;

										case 5:
											itemsToAdd.Add((ItemID.LavaCharm, Main.rand.Next(1, 1)));
											itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
											itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
											itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
											itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
											break;

										case 6:
											itemsToAdd.Add((ItemID.Obsidian, Main.rand.Next(1, 20)));
											itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 15)));
											itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
											itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
											itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 10)));
											itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
											break;


										case 7:
											itemsToAdd.Add((ItemID.WaterWalkingBoots, Main.rand.Next(1, 1)));
											itemsToAdd.Add((ItemID.Fireblossom, Main.rand.Next(2, 15)));
											itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 33)));
											itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
											itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 7)));
											break;

										case 8:
											itemsToAdd.Add((ItemID.ObsidianSkull, Main.rand.Next(1, 1)));
											itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
											itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
											
											itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
											itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
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





							}
						}




						if (Sounda == 5)
						{
							StructureLoader.ReadStruct(Loc, "Struct/Underworld/Underworld3");


						}


					
					}



				}
			}
		}

        private void WorldGenMechShop(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Finding a place for the shop";



            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 1000000)
            {


                int abysmx = WorldGen.genRand.Next(700, Main.maxTilesX - 700); // from 50 since there's a unaccessible area at the world's borders

                //This code makes it avoid teh center
                int distanceBetween = Math.Abs(Main.spawnTileX - abysmx);
                for (int i = 0; i < 1000; i++)
                {
                    abysmx = WorldGen.genRand.Next(700, Main.maxTilesX - 700);
                    distanceBetween = Math.Abs(Main.spawnTileX - abysmx);
                    if (distanceBetween > 900)
                        break;
                }

                // Select a place in the first 6th of the world, avoiding the oceans
                int abysmy = ((Main.maxTilesY / 2));

                // We go down until we hit a solid tile or go under the world's surface
                while (!WorldGen.SolidTile(abysmx, abysmy) && abysmy <= Main.UnderworldLayer)
                {
                    abysmy++;
                }

                // If we went under the world's surface, try again
                if (abysmy > Main.UnderworldLayer - 50)
                {
                    continue;
                }
                Tile tile = Main.tile[abysmx, abysmy];
                // If the type of the tile we are placing the tower on doesn't match what we want, try again
                if (!(tile.TileType == TileID.Stone))
                {
                    continue;
                }


                // place the Rogue
                //	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
                //Main.npc[num].homeTileX = -1;
                //	Main.npc[num].homeTileY = -1;
                //	Main.npc[num].direction = 1;
                //	Main.npc[num].homeless = true;



                for (int da = 0; da < 1; da++)
                {
                    Point Loc = new Point(abysmx, abysmy + 100);
                    string path = "Struct/Underground/MechanicShop";

                    int[] ChestIndexs = StructureLoader.ReadStruct(Loc, path);
                    StructureLoader.ProtectStructure(Loc, path);
                    NPCs.Town.AlcadSpawnSystem.MechanicsTownTile = Loc;
                    foreach (int chestIndex in ChestIndexs)
                    {
                        var chest = Main.chest[chestIndex];
                        // etc

                        // itemsToAdd will hold type and stack data for each item we want to add to the chest
                        var itemsToAdd = new List<(int type, int stack)>();

                        // Here is an example of using WeightedRandom to choose randomly with different weights for different items.
                        int specialItem = new Terraria.Utilities.WeightedRandom<int>(

                            Tuple.Create(ModContent.ItemType<OldCarianTome>(), 0.5)


                        // Choose no item with a high weight of 7.
                        );
                        if (specialItem != ItemID.None)
                        {
                            itemsToAdd.Add((specialItem, 1));
                        }
                        // Using a switch statement and a random choice to add sets of items.
                        switch (Main.rand.Next(1))
                        {
                            case 0:
                                itemsToAdd.Add((ModContent.ItemType<MagnusMagnum>(), Main.rand.Next(1, 1)));
                                itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
                                itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
                                itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));

                                itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
                                itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
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
                }

                placed = true;
            }


        }


        #endregion


        #region Ice Biome
        private void WorldGenCathedral(GenerationProgress progress, GameConfiguration configuration)
					{

						// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
						progress.Message = "Verlia Ark";

						bool placed = false;
						int attempts = 0;
						while (!placed && attempts++ < 100000)
						{
							// Select a place in the first 6th of the world, avoiding the oceans
							int towerX = WorldGen.genRand.Next(0, Main.maxTilesX - 200); // from 50 since there's a unaccessible area at the world's borders
																						 // 50% of choosing the last 6th of the world
																						 // Choose which side of the world to be on randomly
							///if (WorldGen.genRand.NextBool())
							///{
							///	towerX = Main.maxTilesX - towerX;
							///}

							//Start at 200 tiles above the surface instead of 0, to exclude floating islands
							int towerY = (int)Main.worldSurface - 200;

							// We go down until we hit a solid tile or go under the world's surface
							while (!WorldGen.SolidTile(towerX, towerY) && towerY <= Main.worldSurface)
							{
								towerY++;
							}

							// If we went under the world's surface, try again
							if (towerY > Main.worldSurface)
							{
								continue;
							}
							Tile tile = Main.tile[towerX, towerY];
							// If the type of the tile we are placing the tower on doesn't match what we want, try again
							if (!(tile.TileType == TileID.IceBlock
								|| tile.TileType == TileID.SnowBlock))
							{
								continue;
							}


							for (int da = 0; da < 1; da++)
							{
								Point Loc = new Point(towerX, towerY - 50);

								// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
								StructureMap structures = GenVars.structures;
								StructureLoader.ProtectStructure(Loc, "Struct/Ice/VerliasCathedral");
								int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Ice/VerliasCathedral");
								Chest c = Main.chest[ChestIndexs[0]];

								foreach (int chestIndex in ChestIndexs)
								{
									var chest = Main.chest[chestIndex];
									// etc

									// itemsToAdd will hold type and stack data for each item we want to add to the chest
									var itemsToAdd = new List<(int type, int stack)>();

									// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
									int specialItem = new Terraria.Utilities.WeightedRandom<int>(

										Tuple.Create(ModContent.ItemType<EmptyMoonflameLantern>(), 0.5)

									// Choose no item with a high weight of 7.
									);
									if (specialItem != ItemID.None)
									{
										itemsToAdd.Add((specialItem, 1));
									}
									// Using a switch statement and a random choice to add sets of items.
									switch (Main.rand.Next(6))
									{
										case 0:
											itemsToAdd.Add((ModContent.ItemType<LittleWand>(), Main.rand.Next(1, 1)));
											itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
											itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(5, 20)));
											 ;
											itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
											itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
											itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
											break;
										case 1:
											itemsToAdd.Add((ModContent.ItemType<AlcaricQuiver>(), Main.rand.Next(1, 1)));
											itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
											itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
											itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
											
											itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
											itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
											itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
											break;
										case 2:
											itemsToAdd.Add((ModContent.ItemType<BlackRose>(), Main.rand.Next(1, 1)));
											itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
											itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(20, 30)));
											itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
											itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
											itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
											break;
										case 3:
											itemsToAdd.Add((ModContent.ItemType<FloweredInsource>(), Main.rand.Next(1, 1)));
											itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
											itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
											itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
											itemsToAdd.Add((ModContent.ItemType<AlcadizScrap>(), Main.rand.Next(5, 20)));
											itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
											itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

											break;
										case 4:
											itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
											itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
											itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
											itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(20, 30)));
											itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
											itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
											itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
											break;

										case 5:
											itemsToAdd.Add((ItemID.FuneralHat, Main.rand.Next(1, 1)));
											itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
											itemsToAdd.Add((ModContent.ItemType<CarianWood>(), Main.rand.Next(20, 30)));
											itemsToAdd.Add((ModContent.ItemType<AlcaricMush>(), Main.rand.Next(2, 30)));
											itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
											itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
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


								placed = true;

							}
						}

					}




		private void WorldGenVeldris(GenerationProgress progress, GameConfiguration configuration)
		{

			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Veldris Building his house";

			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 100000)
			{
				// Select a place in the first 6th of the world, avoiding the oceans
				int towerX = WorldGen.genRand.Next(0, Main.maxTilesX - 200); // from 50 since there's a unaccessible area at the world's borders
																			 // 50% of choosing the last 6th of the world
																			 // Choose which side of the world to be on randomly
				///if (WorldGen.genRand.NextBool())
				///{
				///	towerX = Main.maxTilesX - towerX;
				///}

				//Start at 200 tiles above the surface instead of 0, to exclude floating islands
				int towerY = (int)Main.worldSurface - 200;

				// We go down until we hit a solid tile or go under the world's surface
				while (!WorldGen.SolidTile(towerX, towerY) && towerY <= Main.worldSurface)
				{
					towerY++;
				}

				// If we went under the world's surface, try again
				if (towerY > Main.worldSurface)
				{
					continue;
				}
				Tile tile = Main.tile[towerX, towerY];
				// If the type of the tile we are placing the tower on doesn't match what we want, try again
				if (!(tile.TileType == TileID.IceBlock
					|| tile.TileType == TileID.SnowBlock))
				{
					continue;
				}


				for (int da = 0; da < 1; da++)
				{
					Point Loc = new Point(towerX, towerY + 14);

				
				
					

					// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
					StructureMap structures = GenVars.structures;
					if (!StructureLoader.TryPlaceAndProtectStructure(Loc, "Struct/Ice/VeldrisHouse"))
						continue;
					int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Ice/VeldrisHouse");
					NPCs.Town.AlcadSpawnSystem.VelTile = Loc;
					Chest c = Main.chest[ChestIndexs[0]];

					foreach (int chestIndex in ChestIndexs)
					{
						var chest = Main.chest[chestIndex];
						// etc

						// itemsToAdd will hold type and stack data for each item we want to add to the chest
						var itemsToAdd = new List<(int type, int stack)>();

						// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
						int specialItem = new Terraria.Utilities.WeightedRandom<int>(

							Tuple.Create(ModContent.ItemType<FrostSwing>(), 0.5)

						// Choose no item with a high weight of 7.
						);
						if (specialItem != ItemID.None)
						{
							itemsToAdd.Add((specialItem, 1));
						}
						// Using a switch statement and a random choice to add sets of items.
						switch (Main.rand.Next(6))
						{
							case 0:
								itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
								break;
							case 1:
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
								itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
								break;

							case 2:
								itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
								itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
								break;
							case 3:
								itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(10, 15)));
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
								itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));

								break;
							case 4:
								itemsToAdd.Add((ModContent.ItemType<Gambit>(), Main.rand.Next(1, 4)));
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
								break;

							case 5:
								itemsToAdd.Add((ItemID.FuneralHat, Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
								itemsToAdd.Add((ItemID.ObsidianSkinPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
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


					placed = true;

				}
			}

		}



        #endregion








        const string SavestringX = "Savestring1";
					const string SavestringY = "Savestring2";

	
		public static Point MorrowEdge = new Point(0, 0);
		public static Point MorrowEdgeY = new Point(0, 0);

		public override void SaveWorldData(TagCompound tag)
		{
			tag[SavestringX] = MorrowEdge.X;
			tag[SavestringY] = MorrowEdge.Y;
		}

		public override void LoadWorldData(TagCompound tag)
		{
			int x = tag.Get<int>(SavestringX);
			int y = tag.Get<int>(SavestringY);
			MorrowEdge = new Point(x, y);
		}	
	}
}