
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using System;
using Terraria.IO;
using Stellamod.Tiles;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;
using Stellamod.Helpers;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Ores;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Placeable;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Items.Weapons.Whips;
using Stellamod.Items.Materials;
using Stellamod.Tiles.Abyss;
using Stellamod.Tiles.Acid;

namespace Stellamod.WorldG
{


    public class StellaWorld : ModSystem
    {
     
        public static bool SoulStorm;
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			/* int MorrowGen = tasks.FindIndex(genpass => genpass.Name.Equals("Webs"));
				if (MorrowGen != -1)
				{

					tasks.Insert(MorrowGen + 1, new PassLegacy("World Gen Morrow", WorldGenMorrow));

				}

				int MorrowDig = tasks.FindIndex(genpass => genpass.Name.Equals("World Gen Morrow"));
				if (MorrowDig != -1)
				{

					tasks.Insert(MorrowDig + 1, new PassLegacy("World Gen Morrow Digging", WorldGenMorrowDig));

				}
			*/

			int MorrowGen = tasks.FindIndex(genpass => genpass.Name.Equals("Lakes"));
			if (MorrowGen != -1)
			{
				tasks.Insert(MorrowGen + 1, new PassLegacy("World Gen Abysm", WorldGenAbysm));
				tasks.Insert(MorrowGen + 2, new PassLegacy("World Gen Virulent", WorldGenVirulent));
				tasks.Insert(MorrowGen + 3, new PassLegacy("World Gen Morrowed Structures", WorldGenMorrowedStructures));

			}

			int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
			if (ShiniesIndex != -1)
			{

				tasks.Insert(ShiniesIndex + 1, new PassLegacy("World Gen Flame Ores", WorldGenFlameOre));
				tasks.Insert(ShiniesIndex + 2, new PassLegacy("World Gen Ice Ores", WorldGenFrileOre));
				
			}

			int CathedralGen = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
			if (CathedralGen != -1)
			{
				tasks.Insert(CathedralGen + 3, new PassLegacy("World Gen Cathedral", WorldGenCathedral));
				tasks.Insert(CathedralGen + 4, new PassLegacy("World Gen Village", WorldGenVillage));
				tasks.Insert(CathedralGen + 5, new PassLegacy("World Gen Testing", WorldGenTest));
				tasks.Insert(CathedralGen + 6, new PassLegacy("World Gen Fable", WorldGenFabiliaRuin));
			}

			

			

			


		}

		//private void WorldGenMorrowDig(GenerationProgress progress, GameConfiguration configuration)
		//{
		// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
		//	progress.Message = "Gild drilling";

		//	int xz = WorldGen.genRand.Next(0, Main.maxTilesX);
		//	int yz = WorldGen.genRand.Next((int)WorldGen.worldSurface, Main.maxTilesY);
		//	for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
		//	{
		// 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
		//		Tile tile = Main.tile[xz, yz];
		//		if (tile.HasTile && tile.TileType == ModContent.TileType<OvermorrowdirtTile>())
		//		{

		//		}
		//
		// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.


		//	}
		//}





		private void WorldGenFabiliaRuin(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Buring the landscape with Cinder and Fable";



			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 100000)
			{
				// Select a place in the first 6th of the world, avoiding the oceans
				int smx = WorldGen.genRand.Next(Main.maxTilesX / 2, (Main.maxTilesX / 2) + 600 ); // from 50 since there's a unaccessible area at the world's borders
																			   // 50% of choosing the last 6th of the world
																			   // Choose which side of the world to be on randomly
				///if (WorldGen.genRand.NextBool())
				///{
				///	towerX = Main.maxTilesX - towerX;
				///}

				//Start at 200 tiles above the surface instead of 0, to exclude floating islands
				int smy = ((int)(Main.worldSurface - 50));

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
					Point Loc = new Point(smx, smy + 70);
					StructureLoader.ReadStruct(Loc, "Struct/Huntria/FableBiome");


					Point Loc2 = new Point(smx - 10, smy - 70);
					WorldUtils.Gen(Loc2, new Shapes.Circle(15, 20), new Actions.SetTile(TileID.Dirt));
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

					placed = true;
				}


			}

		}
		private void WorldGenAbysm(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Shifting Shadows deep in the Ice";



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



				for (int da = 0; da < 1; da++)
				{
					Point Loc = new Point(abysmx, abysmy);
					WorldGen.TileRunner(Loc.X, Loc.Y, WorldGen.genRand.Next(100, 100), WorldGen.genRand.Next(150, 150), ModContent.TileType<AbyssalDirt>());
					

					
					

				}
			}

		}

		private void WorldGenVirulent(GenerationProgress progress, GameConfiguration configuration)
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
					Point Loc = new Point(abysmx, abysmy);
					WorldGen.TileRunner(Loc.X, Loc.Y, 500, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>(), false, 0f, 0f, true, true);
					WorldGen.TileRunner(Loc.X, Loc.Y + 300, 400, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>(), false, 0f, 0f, true, true);
					WorldGen.TileRunner(Loc.X, Loc.Y + 600, 300, 2, ModContent.TileType<Tiles.Acid.AcidialDirt>(), false, 0f, 0f, true, true);
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
		// 6. This is the actual world generation code.
		private void WorldGenFlameOre(GenerationProgress progress, GameConfiguration configuration)
		{
			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Scorching Gild burning into the world";


			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
			{
				// 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
				int x = WorldGen.genRand.Next(0, Main.maxTilesX / 3);
				int y = WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY);

				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 9), WorldGen.genRand.Next(2, 9), ModContent.TileType<VerianoreTile>());
			}
		}
		private void WorldGenFrileOre(GenerationProgress progress, GameConfiguration configuration)
		{
			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Freezing the world with Frile";


			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
			{
				// 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
				int x = WorldGen.genRand.Next(0, Main.maxTilesX);
				int y = WorldGen.genRand.Next((int)GenVars.rockLayerLow, Main.maxTilesY);

				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 13), WorldGen.genRand.Next(2, 10), ModContent.TileType<FrileOreTile>());
			}
		}

		private void WorldGenMorrowedStructures(GenerationProgress progress, GameConfiguration configuration)
		{
			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Hunters settling down";


			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-07 + 10); k++)
			{
				// 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
				int xa = WorldGen.genRand.Next(0, Main.maxTilesX );
				int ya = WorldGen.genRand.Next((int)GenVars.rockLayerLow, (int)GenVars.rockLayerHigh);
				Point Loc = new Point(xa, ya);

				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				Tile tile = Main.tile[Loc.X, Loc.Y];
				if (tile.HasTile)
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

























			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-07 + 30); k++)
			{
				// 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
				int xa = WorldGen.genRand.Next(0, Main.maxTilesX);
				int ya = WorldGen.genRand.Next((int)GenVars.rockLayerLow, (int)GenVars.rockLayerHigh);
				Point Loc = new Point(xa, ya);

				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				Tile tile = Main.tile[Loc.X, Loc.Y];
				if (tile.HasTile && tile.TileType == TileID.Dirt)
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









			for (int g = 0; g < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-07 + 60); g++)
			{
				int xab = WorldGen.genRand.Next(0, Main.maxTilesX);
				int yab = WorldGen.genRand.Next((int)GenVars.rockLayerHigh, Main.maxTilesY);
				Point Loc = new Point(xab, yab);


				if (Loc.X < 0 || Loc.X > Main.maxTilesX || Loc.Y < 0 || Loc.Y > Main.maxTilesX)
				{

					continue;
				}

				Tile tile = Main.tile[Loc.X, Loc.Y];
				if (tile.HasTile)
				{
					StructureLoader.ReadStruct(Loc, "Struct/Morrow/MorrowUnder1");
				}








			}






			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-07 + 40); k++)
			{
				int xab = WorldGen.genRand.Next(0, Main.maxTilesX);
				int yab = WorldGen.genRand.Next((int)GenVars.rockLayerHigh, Main.maxTilesY);
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
				int towerX = WorldGen.genRand.Next(Main.maxTilesX - 300, Main.maxTilesX - 200); // from 50 since there's a unaccessible area at the world's borders
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
				if (!(tile.TileType == TileID.Dirt
					|| tile.TileType == TileID.SnowBlock
					|| tile.TileType == TileID.Sand))
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

					for (int db = 0; db < 7; db++)
					{
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




		private void WorldGenTest(GenerationProgress progress, GameConfiguration configuration)
		{

			progress.Message = "The J";




			bool placed = false;
			int attempts = 0;
			while (!placed && attempts++ < 300000)
			{
				// Select a place in the first 6th of the world, avoiding the oceans
				int towerX = WorldGen.genRand.Next(Main.maxTilesX / 2, Main.maxTilesX - 1000 ); // from 50 since there's a unaccessible area at the world's borders
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
				if (!(tile.TileType == TileID.Mud
					|| tile.TileType == TileID.SnowBlock
					|| tile.TileType == TileID.Sand))
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

					Point Loc = new Point(towerX, towerY + 1);


					// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.










					int[] ChestIndexs = StructureLoader.ReadStruct(Loc, "Struct/Test/Chest2");
					Chest c = Main.chest[ChestIndexs[0]];

					for (int db = 0; db < 7; db++)
					{
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









	








