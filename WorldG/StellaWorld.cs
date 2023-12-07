
using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Armors.Stone;
using Stellamod.Items.Consumables;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Items.Placeable;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Melee.Spears;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Ranged.Crossbows;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Items.Weapons.Whips;
using Stellamod.Tiles;
using Stellamod.Tiles.Abyss;
using Stellamod.Tiles.Acid;
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

		public static bool SoulStorm;
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{

			


			int MorrowGen = tasks.FindIndex(genpass => genpass.Name.Equals("Lakes"));
			if (MorrowGen != -1)
			{
				tasks.Insert(MorrowGen + 1, new PassLegacy("World Gen Abysm", WorldGenAbysm));
				tasks.Insert(MorrowGen + 2, new PassLegacy("World Gen Virulent", WorldGenVirulent));
				tasks.Insert(MorrowGen + 3, new PassLegacy("World Gen Alcad", WorldGenAlcadSpot));
			}

	//		int FableGen = tasks.FindIndex(genpass => genpass.Name.Equals("Pyramids"));
		//	if (FableGen != -1)
		//	{
		//		tasks.Insert(FableGen + 1, new PassLegacy("World Gen Fable", WorldGenFabiliaRuin));

		//	}

			int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Lakes"));
			if (ShiniesIndex != -1)
			{

				tasks.Insert(ShiniesIndex + 1, new PassLegacy("World Gen Other stones", WorldGenDarkstone));
				tasks.Insert(ShiniesIndex + 2, new PassLegacy("World Gen Ice Ores", WorldGenFrileOre));
				tasks.Insert(ShiniesIndex + 3, new PassLegacy("World Gen Starry Ores", WorldGenArncharOre));			
				tasks.Insert(ShiniesIndex + 4, new PassLegacy("World Gen Flame Ores", WorldGenFlameOre));
				tasks.Insert(ShiniesIndex + 5, new PassLegacy("World Gen Flame Ores", WorldGenVeriplantBlobs));
				tasks.Insert(ShiniesIndex + 6, new PassLegacy("World Gen Govheil Castle", WorldGenRoyalCapital));
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
			
				tasks.Insert(CathedralGen2 + 1, new PassLegacy("World Gen Morrowed Structures", WorldGenMorrowedStructures));
				tasks.Insert(CathedralGen2 + 2, new PassLegacy("World Gen Fable", WorldGenFabiliaRuin));
				tasks.Insert(CathedralGen2 + 3, new PassLegacy("World Gen Village", WorldGenVillage));
				tasks.Insert(CathedralGen2 + 4, new PassLegacy("World Gen AureTemple", WorldGenAurelusTemple));
				tasks.Insert(CathedralGen2 + 5, new PassLegacy("World Gen More skies", WorldGenBig));
				tasks.Insert(CathedralGen2 + 6, new PassLegacy("World Gen More skies", WorldGenMed));
				tasks.Insert(CathedralGen2 + 7, new PassLegacy("World Gen Sunstalker", WorldGenStalker));
				tasks.Insert(CathedralGen2 + 8, new PassLegacy("World Gen Virulent Structures", WorldGenVirulentStructures));
				tasks.Insert(CathedralGen2 + 9, new PassLegacy("World Gen Govheil Castle", WorldGenGovheilCastle));
				tasks.Insert(CathedralGen2 + 10, new PassLegacy("World Gen Stone Castle", WorldGenStoneCastle));
				tasks.Insert(CathedralGen2 + 11, new PassLegacy("World Gen Bridget", WorldGenBridget));			
				tasks.Insert(CathedralGen2 + 12, new PassLegacy("World Gen Cathedral", WorldGenCathedral));
				tasks.Insert(CathedralGen2 + 13, new PassLegacy("World Gen Cathedral", WorldGenSeaTemple));
				tasks.Insert(CathedralGen2 + 14, new PassLegacy("World Gen Cathedral", WorldGenUnderworldSpice));
				tasks.Insert(CathedralGen2 + 15, new PassLegacy("World Gen Cathedral", WorldGenSylia));
			}


			




		}




		private void WorldGenAmbience(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Golden Ambience ruining the world";


            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next(0, Main.maxTilesY);
                if (Main.tile[X, Y].TileType == TileID.Dirt)
                {
                    WorldGen.PlaceObject(X, Y, (ushort)ModContent.TileType<Tiles.Ambient.OwlTrunck1>());
                  
                }
            }
            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next(0, Main.maxTilesY);
                if (Main.tile[X, Y].TileType == TileID.Dirt)
                {
                    WorldGen.PlaceObject(X, Y, (ushort)ModContent.TileType<Tiles.Ambient.OwlTrunck2>());
                  
                }
            }
            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next(0, Main.maxTilesY);
                if (Main.tile[X, Y].TileType == TileID.Dirt)
                {
                    WorldGen.PlaceObject(X, Y, (ushort)ModContent.TileType<Tiles.Ambient.OwlTrunck3>());
                  
                }
            }


            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 9.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
                if (Main.tile[X, Y].TileType == TileID.Stone || Main.tile[X, Y].TileType == TileID.ClayBlock)
                {
                    WorldGen.PlaceObject(X, Y, (ushort)ModContent.TileType<Tiles.Ambient.BigRock1>());
             
                }
            }
            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 9.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
                if (Main.tile[X, Y].TileType == TileID.Stone || Main.tile[X, Y].TileType == TileID.ClayBlock)
                {
                    WorldGen.PlaceObject(X, Y, (ushort)ModContent.TileType<Tiles.Ambient.BigRock2>());
                
                }
            }
            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 9.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
                if (Main.tile[X, Y].TileType == TileID.Stone || Main.tile[X, Y].TileType == TileID.ClayBlock)
                {
                    WorldGen.PlaceObject(X, Y, (ushort)ModContent.TileType<Tiles.Ambient.BigRock3>());
                
                }
            }
            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 9.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
                if (Main.tile[X, Y].TileType == TileID.Stone || Main.tile[X, Y].TileType == TileID.ClayBlock)
                {
                    WorldGen.PlaceObject(X, Y, (ushort)ModContent.TileType<Tiles.Ambient.BigRock4>());
                  
                }
            }
            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
                if (Main.tile[X, Y].TileType == TileID.Stone || Main.tile[X, Y].TileType == TileID.ClayBlock)
                {
                    WorldGen.PlaceObject(X, Y, (ushort)ModContent.TileType<Tiles.Ambient.Stalagmite1>());
               
                }
            }
            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
                if (Main.tile[X, Y].TileType == TileID.Stone || Main.tile[X, Y].TileType == TileID.ClayBlock)
                {
                    WorldGen.PlaceObject(X, Y, (ushort)ModContent.TileType<Tiles.Ambient.Stalagmite2>());
                  
                }
            }
            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
                if (Main.tile[X, Y].TileType == TileID.Stone || Main.tile[X, Y].TileType == TileID.ClayBlock)
                {
                    WorldGen.PlaceObject(X, Y, (ushort)ModContent.TileType<Tiles.Ambient.Stalagmite3>());
                  
                }
            }
            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 20.2f) * 6E-03); k++)
            {
                int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
                if (Main.tile[X, Y].TileType == TileID.Stone || Main.tile[X, Y].TileType == TileID.ClayBlock)
                {
                    WorldGen.PlaceObject(X, Y, (ushort)ModContent.TileType<Tiles.Ambient.Stalagmite4>());
                   
                }
            }


			for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.2f) * 6E-03); k++)
			{
				int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
				int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY / 2);
				if (Main.tile[X, Y].TileType == TileID.Dirt)
				{
					WorldGen.PlaceObject(X, Y, (ushort)ModContent.TileType<Tiles.Ambient.TreeOver1>());

				}


			}

			for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 2.2f) * 6E-03); k++)
			{
				int X = WorldGen.genRand.Next(100, Main.maxTilesX - 20);
				int Y = WorldGen.genRand.Next(0, Main.maxTilesY);
				if (Main.tile[X, Y].TileType == TileID.Dirt)
				{
					WorldGen.PlaceObject(X, Y, (ushort)ModContent.TileType<Tiles.Ambient.TreeOver2>());

				}
			}


		}



		private void WorldGenFabiliaRuin(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Buring the landscape with Cinder and Fable";



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
				if (!(tile.TileType == TileID.Sand
					|| tile.TileType == TileID.Dirt
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
					Point Loc = new Point(smx + 10, smy + 340 );
					StructureLoader.ReadStruct(Loc, "Struct/Morrow/FableBiomeNew");

					Point Loc2 = new Point(smx + 10, smy + 380);
					WorldGen.digTunnel(Loc2.X - 10, Loc2.Y + 10, 1, 0, 1, 10, false);

		
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
					GenVars.structures.AddProtectedStructure(new Rectangle(smx, smy, 433, 100));
					//WorldGen.TileRunner(Loc2.X - 10, Loc2.Y - 60, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(120, 120), TileID.Grass);
					//WorldGen.TileRunner(Loc3.X - 20, Loc2.Y, WorldGen.genRand.Next(40, 43), WorldGen.genRand.Next(100, 100), TileID.Grass);
					//WorldGen.TileRunner(Loc3.X - 20, Loc3.Y + 20, WorldGen.genRand.Next(40, 43), WorldGen.genRand.Next(100, 100), TileID.Grass);
					placed = true;
				}


			}

		}







		private void WorldGenStoneCastle(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Creating life near spawn :)";



			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 10000000)
			{
				// Select a place in the first 6th of the world, avoiding the oceans
				int smx = WorldGen.genRand.Next((Main.maxTilesX / 2) - 150, (Main.maxTilesX / 2) - 140); // from 50 since there's a unaccessible area at the world's borders
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
					Point Loc = new Point(smx, smy + Main.rand.Next(10, 20));

					int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Overworld/StoneTemple");
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
					int smx = WorldGen.genRand.Next(0, Main.maxTilesX); // from 50 since there's a unaccessible area at the world's borders
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
						Point Loc = new Point(smx, smy - Main.rand.Next(150, 200));

						int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Overworld/Overworld2");
						foreach (int chestIndex in ChestIndexs)
						{
							var chest = Main.chest[chestIndex];
							// etc

							// itemsToAdd will hold type and stack data for each item we want to add to the chest
							var itemsToAdd = new List<(int type, int stack)>();

							// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
							int specialItem = new Terraria.Utilities.WeightedRandom<int>(

								Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5)


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

								case 9:
									itemsToAdd.Add((ItemID.PlatinumBar, Main.rand.Next(1, 20)));
									itemsToAdd.Add((ModContent.ItemType<GintzlMetal>(), Main.rand.Next(2, 10)));
									itemsToAdd.Add((ModContent.ItemType<FrileOre>(), Main.rand.Next(9, 15)));
									itemsToAdd.Add((ItemID.RegenerationPotion, Main.rand.Next(1, 3)));
									itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 2)));
									itemsToAdd.Add((ModContent.ItemType<GhostExcalibur>(), Main.rand.Next(1, 1)));
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
					int smx = WorldGen.genRand.Next(500, (Main.maxTilesX) - 500); // from 50 since there's a unaccessible area at the world's borders
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
						Point Loc = new Point(smx, smy - Main.rand.Next(150, 200));

						int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Overworld/Overworld3");
						foreach (int chestIndex in ChestIndexs)
						{
							var chest = Main.chest[chestIndex];
							// etc

							// itemsToAdd will hold type and stack data for each item we want to add to the chest
							var itemsToAdd = new List<(int type, int stack)>();

							// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
							int specialItem = new Terraria.Utilities.WeightedRandom<int>(

								Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5)


							// Choose no item with a high weight of 7.
							);
							if (specialItem != ItemID.None)
							{
								itemsToAdd.Add((specialItem, 1));
							}
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
									itemsToAdd.Add((ModContent.ItemType<EaglesGrace>(), Main.rand.Next(1, 1)));
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
			progress.Message = "Bird building alters";


			for (int k = 0; k < 1; k++)
			{
				bool placed = false;
				int attempts = 0;
				while (!placed && attempts++ < 1000000)
				{
					// Select a place in the first 6th of the world, avoiding the oceans
					int smx = WorldGen.genRand.Next(300, (Main.maxTilesX) - 130); // from 50 since there's a unaccessible area at the world's borders
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


					// place the Rogue
					//	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
					//Main.npc[num].homeTileX = -1;
					//	Main.npc[num].homeTileY = -1;
					//	Main.npc[num].direction = 1;
					//	Main.npc[num].homeless = true;



					for (int da = 0; da < 1; da++)
					{
						Point Loc = new Point(smx, smy + 5);
						int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Ocean/SunAlter2");
						foreach (int chestIndex in ChestIndexs)
						{
							var chest = Main.chest[chestIndex];
							// etc

							// itemsToAdd will hold type and stack data for each item we want to add to the chest
							var itemsToAdd = new List<(int type, int stack)>();

							// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
							int specialItem = new Terraria.Utilities.WeightedRandom<int>(
									Tuple.Create((int)ModContent.ItemType<CinderBraker>(), 0.1)


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

		private void WorldGenSeaTemple(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Definitely not making elder guardians from minecraft.";


			for (int k = 0; k < 1; k++)
			{
				bool placed = false;
				int attempts = 0;
				while (!placed && attempts++ < 1000000)
				{
					// Select a place in the first 6th of the world, avoiding the oceans
					int smx = WorldGen.genRand.Next((Main.maxTilesX) - 160, (Main.maxTilesX) - 120); // from 50 since there's a unaccessible area at the world's borders
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
							|| tile.TileType == TileID.Dirt
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
						Point Loc = new Point(smx, smy + 350);
						int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Ocean/SeaTemple");
						foreach (int chestIndex in ChestIndexs)
						{
							var chest = Main.chest[chestIndex];
							// etc

							// itemsToAdd will hold type and stack data for each item we want to add to the chest
							var itemsToAdd = new List<(int type, int stack)>();

							// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
							int specialItem = new Terraria.Utilities.WeightedRandom<int>(
									Tuple.Create((int)ModContent.ItemType<SunClaw>(), 0.1)


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


		private void WorldGenBridget(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "The Almighty weapon being burried";


			for (int k = 0; k < 1; k++)
			{
				bool placed = false;
				int attempts = 0;
				while (!placed && attempts++ < 1000000)
				{
					// Select a place in the first 6th of the world, avoiding the oceans
					int smx = WorldGen.genRand.Next(((Main.maxTilesX) / 2) - 2, (Main.maxTilesX) / 2); // from 50 since there's a unaccessible area at the world's borders
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
						Point Loc = new Point(smx, smy + 200);
						StructureLoader.ReadStruct(Loc, "Struct/Overworld/Bridget");

					}

					placed = true;
				}
			}

		}



		private void WorldGenAurelusTemple(GenerationProgress progress, GameConfiguration configuration)
		{
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
					
					int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Aurelus/AurelusTemple");
					foreach (int chestIndex in ChestIndexs)
					{
						var chest = Main.chest[chestIndex];
						// etc

						// itemsToAdd will hold type and stack data for each item we want to add to the chest
						var itemsToAdd = new List<(int type, int stack)>();

						// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
						int specialItem = new Terraria.Utilities.WeightedRandom<int>(

							Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
							Tuple.Create(ModContent.ItemType<ConvulgingMater>(), 0.1),
							Tuple.Create(ModContent.ItemType<GildedBag1>(), 0.4)

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
								itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
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
								itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
								itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
								itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
								break;
							case 2:
								itemsToAdd.Add((ModContent.ItemType<Neptune8Card>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
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
								itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 30)));
								itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
								itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
								break;

							case 5:
								itemsToAdd.Add((ModContent.ItemType<AurelusBlightBroochA>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<ConvulgingMater>(), Main.rand.Next(2, 10)));
								itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
								break;

							case 6:
								itemsToAdd.Add((ModContent.ItemType<AbyssalPowder>(), Main.rand.Next(1, 1)));
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





		private void WorldGenGovheilCastle(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Gothivia marrying Paraffin";



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
					

					int[] ChestIndexs = StructureLoader.ReadStruct(pointL, "Struct/Huntria/GovheilCastle");
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
								itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));;
								itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
								break;
							case 1:
								itemsToAdd.Add((ModContent.ItemType<GreekLantern>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
								itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<LostScrap>(), Main.rand.Next(2, 30)));
								itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
								break;
							case 2:
								itemsToAdd.Add((ModContent.ItemType<Kilvier>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
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
								itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
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
								itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
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
					
					WorldGen.TileRunner(Loc.X, Loc.Y, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(150, 150), ModContent.TileType<AbyssalDirt>());





				

				}

				



			}
		}

	


		public void WorldGenVirulent(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Virulifying the Morrow";


			
			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 100000)
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
				int abysmy = (int)(Main.worldSurface - 50);

				// We go down until we hit a solid tile or go under the world's surface
				while (!WorldGen.SolidTile(abysmx, abysmy) && abysmy <= Main.worldSurface)
				{
					abysmy++;
				}

				// If we went under the world's surface, try again
				if (abysmy > Main.worldSurface)
				{
					continue;
				}
				Tile tile = Main.tile[abysmx, abysmy];
				// If the type of the tile we are placing the tower on doesn't match what we want, try again
				if (!(tile.TileType == TileID.Mud))
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
					Point Loc7 = new Point(abysmx, abysmy);
					WorldGen.TileRunner(Loc7.X + 200, Loc7.Y, 500, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>(), false, 0f, 0f, true, true);
					WorldGen.TileRunner(Loc7.X + 200, Loc7.Y + 300, 400, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>(), false, 0f, 0f, true, true);
					WorldGen.TileRunner(Loc7.X + 200, Loc7.Y + 600, 300, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>(), false, 0f, 0f, true, true);

					Point Loc = new Point(abysmx + 50, abysmy + 255);
					pointL = new Point(abysmx + 50, abysmy + 255);

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

		Point pointL;

		#region Royal Capital

		public void WorldGenAlcadSpot(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Fighting the Virulent with magic";





			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 10000000)
			{
				// Select a place in the first 6th of the world, avoiding the oceans
				int smx = WorldGen.genRand.Next(((Main.maxTilesX) / 5), (Main.maxTilesX / 2) - 700); // from 50 since there's a unaccessible area at the world's borders
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
					|| tile.TileType == TileID.IceBlock
					|| tile.TileType == TileID.SnowBlock
					|| tile.TileType == TileID.Ebonstone
					|| tile.TileType == TileID.Crimstone
					|| tile.TileType == TileID.BlueDungeonBrick
					|| tile.TileType == TileID.PinkDungeonBrick
					|| tile.TileType == TileID.GreenDungeonBrick
					|| tile.TileType == ModContent.TileType<AcidialDirt>()
					|| tile.TileType == ModContent.TileType<AcidicTreeSapling>()
					|| tile.TileType == TileID.Sandstone))
				{
					continue;
				}
				
				for (int da = 0; da < 1; da++)
				{
					Point Loc7 = new Point(smx, smy);
					WorldGen.TileRunner(Loc7.X, Loc7.Y, 1, 2, ModContent.TileType<Tiles.StarbloomDirt>(), false, 0f, 0f, true, true);

					Point Loc = new Point(smx + 50, smy + 255);

					placed = true;
				}

				// place the Rogue
				//	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
				//Main.npc[num].homeTileX = -1;
				//	Main.npc[num].homeTileY = -1;
				//	Main.npc[num].direction = 1;
				//	Main.npc[num].homeless = true;




			}
		}

		public void WorldGenRoyalCapital(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Fighting the Virulent with magic";





			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 10000000)
			{
				// Select a place in the first 6th of the world, avoiding the oceans
				int smx = WorldGen.genRand.Next(220, (Main.maxTilesX) / 15); // from 50 since there's a unaccessible area at the world's borders
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
					Point Loc = new Point(smx - 10, smyy + 60);
					StructureLoader.ReadStruct(Loc, "Struct/Alcad/RoyalCapital");


					Point Loc2 = new Point(smx - 10, smyy);
					Point Loc4 = new Point(smx - 30, smyy + 40);

					Point Loc5 = new Point(smx - 30, smyy + 60);
					//	WorldUtils.Gen(Loc2, new Shapes.Mound(60, 90), new Actions.SetTile(TileID.Dirt));
					//	WorldUtils.Gen(Loc4, new Shapes.Rectangle(220, 105), new Actions.SetTile(TileID.Dirt));
					//new Shapes.Rectangle(220, 50), new Actions.SetTile(TileID.Dirt))
					WorldUtils.Gen(Loc4, new Shapes.Mound(40, 50), new Actions.SetTile((ushort)ModContent.TileType<StarbloomDirt>()));
					WorldUtils.Gen(Loc5, new Shapes.Rectangle(590, 70), new Actions.SetTile(TileID.Dirt));
					Point Loc3 = new Point(smx + 555, smyy + 60);
					WorldUtils.Gen(Loc3, new Shapes.Mound(40, 50), new Actions.SetTile((ushort)ModContent.TileType<StarbloomDirt>()));
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
					GenVars.structures.AddProtectedStructure(new Rectangle(smx, smyy, 433, 100));
					//WorldGen.TileRunner(Loc2.X - 20, Loc2.Y - 60, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(120, 120), TileID.Grass);
				//	WorldGen.TileRunner(Loc3.X + 30, Loc2.Y - 60, WorldGen.genRand.Next(40, 43), WorldGen.genRand.Next(100, 100), TileID.Grass);
					placed = true;
				}
			}
		}


		public void WorldGenSylia(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Leaving the Royal Capital";





			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 10000000)
			{
				// Select a place in the first 6th of the world, avoiding the oceans
				int smx = WorldGen.genRand.Next(220, (Main.maxTilesX) / 15); // from 50 since there's a unaccessible area at the world's borders
																			 // 50% of choosing the last 6th of the world
																			 // Choose which side of the world to be on randomly
				///if (WorldGen.genRand.NextBool())
				///{
				///	towerX = Main.maxTilesX - towerX;
				///}

				//Start at 200 tiles above the surface instead of 0, to exclude floating islands
				int smy = ((int)(Main.UnderworldLayer - 200));

				// We go down until we hit a solid tile or go under the world's surface
				while (!WorldGen.SolidTile(smx, smy) && smy <= Main.maxTilesY)
				{
					smy++;
				}

				// If we went under the world's surface, try again
				if (smy > Main.maxTilesY - 30)
				{
					continue;
				}
				Tile tile = Main.tile[smx, smy];
				// If the type of the tile we are placing the tower on doesn't match what we want, try again
				if ((tile.TileType == TileID.Ash))
				{
					continue;
				}

				int smxx = smx;
				int smyy = smy + Main.maxTilesY / 18;
				// place the Rogue
				//	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
				//Main.npc[num].homeTileX = -1;
				//	Main.npc[num].homeTileY = -1;
				//	Main.npc[num].direction = 1;
				//	Main.npc[num].homeless = true;



				for (int da = 0; da < 1; da++)
				{
					Point Loc = new Point(smx - 10, smyy + 180);
					StructureLoader.ReadStruct(Loc, "Struct/Underworld/UnderworldRuins");


					Point Loc2 = new Point(smx - 10, smyy);
					Point Loc4 = new Point(smx - 30, smyy + 40);
					//	WorldUtils.Gen(Loc2, new Shapes.Mound(60, 90), new Actions.SetTile(TileID.Dirt));
					//	WorldUtils.Gen(Loc4, new Shapes.Rectangle(220, 105), new Actions.SetTile(TileID.Dirt));
			
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
					GenVars.structures.AddProtectedStructure(new Rectangle(smx, smyy, 433, 100));
					//WorldGen.TileRunner(Loc2.X - 20, Loc2.Y - 60, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(120, 120), TileID.Grass);
					//	WorldGen.TileRunner(Loc3.X + 30, Loc2.Y - 60, WorldGen.genRand.Next(40, 43), WorldGen.genRand.Next(100, 100), TileID.Grass);
					placed = true;
				}
			}
		}

		#endregion
		// 6. This is the actual world generation code.
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
				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 9), WorldGen.genRand.Next(2, 9), ModContent.TileType<VerianoreTile>());

				
			}

		
				// 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
				
			
		}
		private void WorldGenArncharOre(GenerationProgress progress, GameConfiguration configuration)
		{
			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Scorching Arnchar into the world";


			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
			{
			

				int xz = WorldGen.genRand.Next(0, Main.maxTilesX);
				int yz = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY - 200);

				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				WorldGen.TileRunner(xz, yz, WorldGen.genRand.Next(4, 13), WorldGen.genRand.Next(5, 9), ModContent.TileType<Arnchar>());
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
			}

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



		private void WorldGenVeriplantBlobs(GenerationProgress progress, GameConfiguration configuration)
		{
			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Veribloom forgetting their memories";


			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-07 + 3); k++)
			{
				// 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
				int x = WorldGen.genRand.Next(0, Main.maxTilesX);
				int y = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY - 300);

				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(50, 100), WorldGen.genRand.Next(100, 200), ModContent.TileType<VeriplantDirt>());
			}


			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-07); k++)
			{

				int xa = WorldGen.genRand.Next(0, Main.maxTilesX );
				int ya = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY - 300);
				Point Loc = new Point(xa, ya);

				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				Tile tile = Main.tile[Loc.X, Loc.Y];

				if (!(tile.TileType == TileID.Stone))
				{
					continue;
				}

				if (tile.HasTile)
				{
					switch (Main.rand.Next(5))
					{
						case 0:
							StructureLoader.ReadStruct(Loc, "Struct/Veriplant/Veriplant1");
							break;
						case 1:
							StructureLoader.ReadStruct(Loc, "Struct/Veriplant/Veriplant2");
							break;
						case 2:
							StructureLoader.ReadStruct(Loc, "Struct/Veriplant/Veriplant3");
							break;
						case 3:
						
							StructureLoader.ReadStruct(Loc, "Struct/Veriplant/Veriplant4");
							break;
						
						case 4:
							WorldGen.digTunnel(Loc.X, Loc.Y, 0, 1, 30, 3, false);
					
							break;

					}

					

				}

			}
		}






		private void WorldGenUnderworldSpice(GenerationProgress progress, GameConfiguration configuration)
		{
			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Sylia using magic in the Underworld";





			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-06); k++)
			{

				int xa = WorldGen.genRand.Next(0, Main.maxTilesX);
				int ya = WorldGen.genRand.Next(Main.maxTilesY - 300, Main.maxTilesY - 50);
				Point Loc = new Point(xa, ya);

				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				Tile tile = Main.tile[Loc.X, Loc.Y];

				if (!(tile.TileType == TileID.Ash))
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
										itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
										itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
										break;
									case 1:
										itemsToAdd.Add((ItemID.FlameDye, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
										itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
										break;
									case 2:
										itemsToAdd.Add((ItemID.LavaproofTackleBag, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
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
										itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
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
										itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
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
										itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
										itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
										break;
									case 1:
										itemsToAdd.Add((ItemID.FlameDye, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
										itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
										itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
										itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
										break;
									case 2:
										itemsToAdd.Add((ItemID.LavaproofTackleBag, Main.rand.Next(1, 1)));
										itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
										itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
										itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
										itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
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
										itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
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
										itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
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
											itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
											itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
											itemsToAdd.Add((ItemID.WormholePotion, Main.rand.Next(1, 7)));
											itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
											break;
										case 1:
											itemsToAdd.Add((ItemID.FlameDye, Main.rand.Next(1, 3)));
											itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
											itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
											itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
											itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
											itemsToAdd.Add((ItemID.WrathPotion, Main.rand.Next(1, 7)));
											itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 7)));
											break;
										case 2:
											itemsToAdd.Add((ItemID.LavaproofTackleBag, Main.rand.Next(1, 1)));
											itemsToAdd.Add((ItemID.Moonglow, Main.rand.Next(2, 5)));
											itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
											itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
											itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
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
											itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
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
											itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
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
							Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
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
								itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
								itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));

								itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
								break;
							case 1:
								itemsToAdd.Add((ItemID.JungleSpores, 7));
							
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
								itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
								itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
								itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
								break;
							case 2:
							
								itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
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
							Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
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
								itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
								itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));

								itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
								break;
							case 1:
								itemsToAdd.Add((ItemID.JungleSpores, 7));
			
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
								itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
								itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
								itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
								break;
							case 2:
			
								itemsToAdd.Add((ItemID.Daybloom, Main.rand.Next(2, 5)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
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






























			private void WorldGenMorrowedStructures(GenerationProgress progress, GameConfiguration configuration)
		{
			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Hunters settling down";


			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-07); k++)
			{
				// 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
				int xa = WorldGen.genRand.Next(300, Main.maxTilesX - 300);
				int ya = WorldGen.genRand.Next((int)GenVars.rockLayerLow + 200, (int)GenVars.rockLayerHigh + 200);
				Point Loc = new Point(xa, ya);

				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				Tile tile = Main.tile[Loc.X, Loc.Y];
				if (tile.HasTile && Main.tile[xa, ya].TileType != TileID.LihzahrdBrick)
				{
					int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowStructHouse1");
					foreach (int chestIndex in ChestIndexs)
					{
						var chest = Main.chest[chestIndex];
  // etc

						// itemsToAdd will hold type and stack data for each item we want to add to the chest
						var itemsToAdd = new List<(int type, int stack)>();

						// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
						int specialItem = new Terraria.Utilities.WeightedRandom<int>(
							Tuple.Create((int)ItemID.Acorn, 0.1),
							Tuple.Create(ModContent.ItemType<MorrowSalface>(), 0.1),
							Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
								Tuple.Create(ModContent.ItemType<MorrowValswa>(), 0.6),
								Tuple.Create(ModContent.ItemType<MorrowSword>(), 0.9),
								Tuple.Create(ModContent.ItemType<MorrowRapier>(), 0.7),
								Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.8),
							Tuple.Create(ModContent.ItemType<Bongos>(), 0.4) // Choose no item with a high weight of 7.
						);
						if (specialItem != ItemID.None)
						{
							itemsToAdd.Add((specialItem, 1));
						}
						// Using a switch statement and a random choice to add sets of items.
						switch (Main.rand.Next(4))
						{
							case 0:
								itemsToAdd.Add((ModContent.ItemType<MorrowSalface>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
								itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
								itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));

								itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
								break;
							case 1:
								itemsToAdd.Add((ItemID.Duck, 1));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
								itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
								itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
								itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
								itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
								break;
							case 2:
								itemsToAdd.Add((ModContent.ItemType<MorrowRapier>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
								itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
								itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
								break;
							case 3:
								itemsToAdd.Add((ModContent.ItemType<MorrowWhipI>(), Main.rand.Next(1, 1)));
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

























			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-07); k++)
			{
				// 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
				int xa = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
				int ya = WorldGen.genRand.Next((int)GenVars.rockLayerLow + 150, (int)GenVars.rockLayerHigh + 150);
				Point Loc = new Point(xa, ya);

				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				Tile tile = Main.tile[Loc.X, Loc.Y];
				if (tile.HasTile && tile.TileType == TileID.Dirt && Main.tile[xa, ya].TileType != TileID.LihzahrdBrick)
				{
					int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowedSmallStruct");
					Chest c = Main.chest[ChestIndexs[0]];
					// itemsToAdd will hold type and stack data for each item we want to add to the chest
					var itemsToAdd = new List<(int type, int stack)>();

					// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
					int specialItem = new Terraria.Utilities.WeightedRandom<int>(
						Tuple.Create((int)ItemID.Acorn, 0.1),
						Tuple.Create(ModContent.ItemType<MorrowSalface>(), 0.1),
						Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
							Tuple.Create(ModContent.ItemType<MorrowValswa>(), 0.6),
							Tuple.Create(ModContent.ItemType<MorrowSword>(), 0.9),
							Tuple.Create(ModContent.ItemType<MorrowRapier>(), 0.7),
							Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.8),
						Tuple.Create(ModContent.ItemType<Bongos>(), 0.4) // Choose no item with a high weight of 7.
					);
					if (specialItem != ItemID.None)
					{
						itemsToAdd.Add((specialItem, 1));
					}
					// Using a switch statement and a random choice to add sets of items.
					switch (Main.rand.Next(4))
					{
						case 0:
							itemsToAdd.Add((ModContent.ItemType<MorrowSalface>(), Main.rand.Next(1, 1)));
							itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
							itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
							itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
							itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
							itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
							itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));

							itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
							itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
							break;
						case 1:
							itemsToAdd.Add((ItemID.Duck, 1));
							itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
							itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
							itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
							itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
							itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
							itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
							itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
							itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
							itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
							break;
						case 2:
							itemsToAdd.Add((ModContent.ItemType<MorrowRapier>(), Main.rand.Next(1, 1)));
							itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
							itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
							itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
							itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
							itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
							itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
							itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
							break;
						case 3:
							itemsToAdd.Add((ModContent.ItemType<MorrowWhipI>(), Main.rand.Next(1, 1)));
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
						c.item[chestItemIndex] = item;
						chestItemIndex++;
						if (chestItemIndex >= 40)
							break; // Make sure not to exceed the capacity of the chest
					}
				}
			}









			for (int g = 0; g < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-07 + 40); g++)
			{
				int xab = WorldGen.genRand.Next(0, Main.maxTilesX);
				int yab = WorldGen.genRand.Next((int)GenVars.rockLayerHigh, Main.maxTilesY);
				Point Loc = new Point(xab, yab);


				if (Loc.X < 0 || Loc.X > Main.maxTilesX || Loc.Y < 0 || Loc.Y > Main.maxTilesX)
				{

					continue;
				}

				Tile tile = Main.tile[Loc.X, Loc.Y];
				if (tile.HasTile && Main.tile[xab, yab].TileType != TileID.LihzahrdBrick)
				{
					StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowUnder1");
				}








			}






			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-07 + 30); k++)
			{
				int xab = WorldGen.genRand.Next(0, Main.maxTilesX);
				int yab = WorldGen.genRand.Next((int)GenVars.rockLayerHigh + 100, Main.maxTilesY);
				Point Loc = new Point(xab, yab);

				if (Loc.X < 0 || Loc.X > Main.maxTilesX || Loc.Y < 0 || Loc.Y > Main.maxTilesX)
				{

					continue;
				}

				Tile tile = Main.tile[Loc.X, Loc.Y];
				if (tile.HasTile)
				{
					int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowStructHouseM");
					Chest c = Main.chest[ChestIndexs[0]];
					// itemsToAdd will hold type and stack data for each item we want to add to the chest
					var itemsToAdd = new List<(int type, int stack)>();

					// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
					int specialItem = new Terraria.Utilities.WeightedRandom<int>(
						Tuple.Create((int)ItemID.Acorn, 0.1),
						Tuple.Create(ModContent.ItemType<MorrowSalface>(), 0.1),
						Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
							Tuple.Create(ModContent.ItemType<MorrowValswa>(), 0.6),
							Tuple.Create(ModContent.ItemType<MorrowSword>(), 0.9),
							Tuple.Create(ModContent.ItemType<MorrowRapier>(), 0.7),
							Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.8),
						Tuple.Create(ModContent.ItemType<Bongos>(), 0.4) // Choose no item with a high weight of 7.
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
							itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
							itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
							itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
							itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
							itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
							itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
							break;
						case 1:
							itemsToAdd.Add((ItemID.Duck, 1));
							itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
							itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
							itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
							itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
							itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
							itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
							itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
							itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
							itemsToAdd.Add((ModContent.ItemType<MorrowWhipI>(), Main.rand.Next(1, 1)));
							break;
						case 2:
							itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
							itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
							itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
							itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
							itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
							itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
							itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
							break;
						case 3:
							itemsToAdd.Add((ModContent.ItemType<MorrowSalface>(), Main.rand.Next(1, 1)));
							itemsToAdd.Add((ModContent.ItemType<BroochesTableI>(), Main.rand.Next(1, 1)));
							itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(10, 15)));
							itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
							itemsToAdd.Add((ModContent.ItemType<Starrdew>(), Main.rand.Next(2, 10)));
							itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
							itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
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
						c.item[chestItemIndex] = item;
						chestItemIndex++;
						if (chestItemIndex >= 40)
							break; // Make sure not to exceed the capacity of the chest
					}
				}








			}
		}
	
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
	
				
				// place the Rogue
				//	int num = NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (towerX + 12) * 16, (towerY - 24) * 16, ModContent.NPCType<BoundGambler>(), 0, 0f, 0f, 0f, 0f, 255);
				//Main.npc[num].homeTileX = -1;
				//	Main.npc[num].homeTileY = -1;
				//	Main.npc[num].direction = 1;
				//	Main.npc[num].homeless = true;
				


				for (int da = 0; da < 1; da++)
				{

					Point Loc = new Point(towerX, towerY - 50);


					// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				









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
							Tuple.Create(ModContent.ItemType<EmptyMoonflameLantern>(), 0.9)
						);
						if (specialItem != ItemID.None)
						{
							itemsToAdd.Add((specialItem, 1));
						}
						// Using a switch statement and a random choice to add sets of items.
						switch (Main.rand.Next(4))
						{
							case 0:
								itemsToAdd.Add((ModContent.ItemType<BroochesTableI>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.Sundial, 1));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
								itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 5)));
								itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.Star, Main.rand.Next(1, 50)));
								break;
							case 1:
								itemsToAdd.Add((ModContent.ItemType<BroochesTableI>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.Sundial, 1));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
								itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
								itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 5)));
								itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
								itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.Star, Main.rand.Next(1, 50)));
								break;
							case 2:
								itemsToAdd.Add((ModContent.ItemType<BroochesTableI>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ItemID.Sundial, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
								itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
								itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
								itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 5)));
								itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.Star, Main.rand.Next(1, 50)));
								break;
							case 3:
								itemsToAdd.Add((ModContent.ItemType<BroochesTableI>(), Main.rand.Next(1, 1)));
								itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(10, 15)));
								itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
								itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
								itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
								itemsToAdd.Add((ItemID.IronskinPotion, Main.rand.Next(1, 7)));
								itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(3, 7)));

								break;
						}

						// Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
						int chestItemIndex = 0;
						foreach (var itemToAdd in itemsToAdd)
						{
							Item item = new Item();
							item.SetDefaults(itemToAdd.type);
							item.stack = itemToAdd.stack;
							c.item[chestItemIndex] = item;
							chestItemIndex++;
							if (chestItemIndex >= 40)
								break; // Make sure not to exceed the capacity of the chest
						}
					}



					placed = true;

				}
			}

		}




	










		private void WorldGenVillage(GenerationProgress progress, GameConfiguration configuration)
		{



			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Huntria Ark";






			for (int da = 0; da < 1; da++)
			{

				int xa = WorldGen.genRand.Next(Main.maxTilesX / 3, Main.maxTilesX / 2 );
				int ya = WorldGen.genRand.Next((int)GenVars.rockLayerLow + 300, (int)GenVars.rockLayer + 300);
				Point Loc = new Point(xa, ya);


				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				Tile tile = Main.tile[Loc.X, Loc.Y];









				int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Huntria/HuntriasOutpost");
				Chest c = Main.chest[ChestIndexs[0]];

				for (int db = 0; db < 7; db++)
				{
					// itemsToAdd will hold type and stack data for each item we want to add to the chest
					var itemsToAdd = new List<(int type, int stack)>();

					// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
					int specialItem = new Terraria.Utilities.WeightedRandom<int>(
						Tuple.Create((int)ItemID.Acorn, 0.1),
						Tuple.Create(ModContent.ItemType<MorrowSalface>(), 0.1),
						Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
							Tuple.Create(ModContent.ItemType<MorrowValswa>(), 0.6),
							Tuple.Create(ModContent.ItemType<MorrowSword>(), 0.9),
							Tuple.Create(ModContent.ItemType<MorrowRapier>(), 0.7),
							Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.8),
						Tuple.Create(ModContent.ItemType<Bongos>(), 0.4) // Choose no item with a high weight of 7.
					);
					if (specialItem != ItemID.None)
					{
						itemsToAdd.Add((specialItem, 1));
					}
					// Using a switch statement and a random choice to add sets of items.
					switch (Main.rand.Next(4))
					{
						case 0:
							itemsToAdd.Add((ModContent.ItemType<BroochesTableI>(), Main.rand.Next(1, 1)));
							itemsToAdd.Add((ModContent.ItemType<MorrowSalface>(), Main.rand.Next(1, 1)));
							itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
							itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
							itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
							itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
							itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));

							itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
							itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
							break;
						case 1:
							itemsToAdd.Add((ModContent.ItemType<BroochesTableI>(), Main.rand.Next(1, 1)));
							itemsToAdd.Add((ItemID.Duck, 1));
							itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
							itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
							itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
							itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
							itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
							itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
							itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
							itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
							break;
						case 2:
							itemsToAdd.Add((ModContent.ItemType<BroochesTableI>(), Main.rand.Next(1, 1)));
							itemsToAdd.Add((ModContent.ItemType<MorrowRapier>(), Main.rand.Next(1, 1)));
							itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
							itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
							itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
							itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
							itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
							itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
							break;
						case 3:
							itemsToAdd.Add((ModContent.ItemType<BroochesTableI>(), Main.rand.Next(1, 1)));
							itemsToAdd.Add((ModContent.ItemType<MorrowWhipI>(), Main.rand.Next(1, 1)));
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
						c.item[chestItemIndex] = item;
						chestItemIndex++;
						if (chestItemIndex >= 40)
							break; // Make sure not to exceed the capacity of the chest
					}
				}





			}


		}












		const string SavestringX = "Savestring1";
		const string SavestringY = "Savestring2";


		public static Point MorrowEdge = new Point(0, 0);
		public static Point MorrowEdgeY = new Point(0, 0);

		private void WorldGenMorrow(GenerationProgress progress, GameConfiguration configuration)
		{
			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Gild settling in the ground";
			int xa = WorldGen.genRand.Next(0, Main.maxTilesX / 2);
			int ya = WorldGen.genRand.Next((int)GenVars.worldSurface, Main.maxTilesY / 2);

			int yb = WorldGen.genRand.Next((int)GenVars.worldSurface, (int)GenVars.worldSurface);


			for (int da = 0; da < 1; da++)
			{
				WorldGen.TileRunner(xa, ya, WorldGen.genRand.Next(1100, 1100), WorldGen.genRand.Next(1100, 1100), ModContent.TileType<OvermorrowdirtTile>());


			}

			MorrowEdge.X = xa - 500;
			MorrowEdge.Y = ya - 100;
			MorrowEdgeY.Y = yb - 100;
			MorrowEdgeY.X = xa - 100;




			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-07 + 56); k++)
			{
				Point Loc = new Point(MorrowEdge.X + Main.rand.Next(0, 900), MorrowEdge.Y + Main.rand.Next(0, 900));


				if (Loc.X < 0 || Loc.X > Main.maxTilesX || Loc.Y < 0 || Loc.Y > Main.maxTilesX)
				{

					continue;
				}

				Tile tile = Main.tile[Loc.X, Loc.Y];
				if (tile.HasTile && tile.TileType == ModContent.TileType<OvermorrowdirtTile>())
				{
					StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowUnder1");
				}








			}

			


			



			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-06 + 6); k++)
			{
				Point Loc = new Point(MorrowEdge.X + Main.rand.Next(0, 1100), MorrowEdge.Y + Main.rand.Next(0, 1000));


				if (Loc.X < 0 || Loc.X > Main.maxTilesX || Loc.Y < 0 || Loc.Y > Main.maxTilesX)
				{

					continue;
				}

				Tile tile = Main.tile[Loc.X, Loc.Y];
				if (tile.HasTile && tile.TileType == ModContent.TileType<OvermorrowdirtTile>())
				{
					int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowStructHouse1");
					Chest c = Main.chest[ChestIndexs[0]];
					// itemsToAdd will hold type and stack data for each item we want to add to the chest
					var itemsToAdd = new List<(int type, int stack)>();

					// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
					int specialItem = new Terraria.Utilities.WeightedRandom<int>(
						Tuple.Create((int)ItemID.Acorn, 0.1),
						Tuple.Create(ModContent.ItemType<MorrowSalface>(), 0.1),
						Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
							Tuple.Create(ModContent.ItemType<MorrowValswa>(), 0.6),
							Tuple.Create(ModContent.ItemType<MorrowSword>(), 0.9),
							Tuple.Create(ModContent.ItemType<MorrowRapier>(), 0.7),
							Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.8),
						Tuple.Create(ModContent.ItemType<Bongos>(), 0.4) // Choose no item with a high weight of 7.
					);
					if (specialItem != ItemID.None)
					{
						itemsToAdd.Add((specialItem, 1));
					}
					// Using a switch statement and a random choice to add sets of items.
					switch (Main.rand.Next(4))
					{
						case 0:
							itemsToAdd.Add((ModContent.ItemType<MorrowSalface>(), Main.rand.Next(1, 1)));
							itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
							itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
							itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
							itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
							itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));

							itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
							itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
							break;
						case 1:
							itemsToAdd.Add((ItemID.Duck, 1));
							itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
							itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
							itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
							itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
							itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
							itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
							itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
							itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
							break;
						case 2:
							itemsToAdd.Add((ModContent.ItemType<MorrowRapier>(), Main.rand.Next(1, 1)));
							itemsToAdd.Add((ItemID.FireblossomSeeds, Main.rand.Next(2, 5)));
							itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
							itemsToAdd.Add((ModContent.ItemType<CondensedDirt>(), Main.rand.Next(20, 30)));
							itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
							itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
							itemsToAdd.Add((ItemID.LifeforcePotion, Main.rand.Next(1, 7)));
							break;
						case 3:
							itemsToAdd.Add((ModContent.ItemType<MorrowWhipI>(), Main.rand.Next(1, 1)));
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
						c.item[chestItemIndex] = item;
						chestItemIndex++;
						if (chestItemIndex >= 40)
							break; // Make sure not to exceed the capacity of the chest
					}
				}
			}








			

			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-07 + 9); k++)
			{
				Point Loc = new Point(MorrowEdge.X + Main.rand.Next(0, 1000), MorrowEdge.Y + Main.rand.Next(0, 800));


				if (Loc.X < 0 || Loc.X > Main.maxTilesX || Loc.Y < 0 || Loc.Y > Main.maxTilesX)
				{

					continue;
				}

				Tile tile = Main.tile[Loc.X, Loc.Y];
				if (tile.HasTile && tile.TileType == ModContent.TileType<OvermorrowdirtTile>())
				{
					int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowStructHouseM");
					Chest c = Main.chest[ChestIndexs[0]];
					// itemsToAdd will hold type and stack data for each item we want to add to the chest
					var itemsToAdd = new List<(int type, int stack)>();

					// Here is an example of using WeightedRandom to choose randomly with different weights for different items.
					int specialItem = new Terraria.Utilities.WeightedRandom<int>(
						Tuple.Create((int)ItemID.Acorn, 0.1),
						Tuple.Create(ModContent.ItemType<MorrowSalface>(), 0.1),
						Tuple.Create(ModContent.ItemType<MorrowChestKey>(), 0.5),
							Tuple.Create(ModContent.ItemType<MorrowValswa>(), 0.6),
							Tuple.Create(ModContent.ItemType<MorrowSword>(), 0.9),
							Tuple.Create(ModContent.ItemType<MorrowRapier>(), 0.7),
							Tuple.Create(ModContent.ItemType<GrassDirtPowder>(), 0.8),
						Tuple.Create(ModContent.ItemType<Bongos>(), 0.4) // Choose no item with a high weight of 7.
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
							itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
							itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
							itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
							itemsToAdd.Add((ItemID.PotionOfReturn, Main.rand.Next(1, 7)));
							itemsToAdd.Add((ItemID.SpelunkerPotion, Main.rand.Next(1, 7)));
							break;
						case 1:
							itemsToAdd.Add((ItemID.Duck, 1));
							itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(9, 15)));
							itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
							itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
							itemsToAdd.Add((ModContent.ItemType<Morrowshroom>(), Main.rand.Next(20, 30)));
							itemsToAdd.Add((ItemID.ManaCrystal, Main.rand.Next(3, 7)));
							itemsToAdd.Add((ItemID.LifeCrystal, Main.rand.Next(1, 3)));
							itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 7)));
							itemsToAdd.Add((ModContent.ItemType<MorrowWhipI>(), Main.rand.Next(1, 1)));
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
							itemsToAdd.Add((ModContent.ItemType<MorrowSalface>(), Main.rand.Next(1, 1)));
							itemsToAdd.Add((ModContent.ItemType<VerianOre>(), Main.rand.Next(10, 15)));
							itemsToAdd.Add((ModContent.ItemType<Cinderscrap>(), Main.rand.Next(5, 20)));
							itemsToAdd.Add((ItemID.Dynamite, Main.rand.Next(1, 3)));
							itemsToAdd.Add((ItemID.Bomb, Main.rand.Next(3, 7)));
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
						c.item[chestItemIndex] = item;
						chestItemIndex++;
						if (chestItemIndex >= 40)
							break; // Make sure not to exceed the capacity of the chest
					}
				}








			}


			for (int da = 0; da < 1; da++)
			{
				Point Loc = new Point(MorrowEdge.X + Main.rand.Next(500, 500), MorrowEdge.Y + Main.rand.Next(25, 25));


				if (Loc.X < 0 || Loc.X > Main.maxTilesX || Loc.Y < 0 || Loc.Y > Main.maxTilesX)
				{

					continue;
				}

				Tile tile = Main.tile[Loc.X, Loc.Y];
				if (tile.HasTile && tile.TileType == ModContent.TileType<OvermorrowdirtTile>())
				{
					StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowOutpost");
				}








			}

			

			








			for (int i = MorrowEdge.X; i < MorrowEdge.X + 1000; i++)
			{
				for (int j = MorrowEdge.Y; j < MorrowEdge.Y + 600; j++)
				{
					WorldGen.PlaceWall(i, j, ModContent.WallType<OvermorrowdirtWall>());
				}



			}
		}














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


















