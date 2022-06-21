
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

namespace Stellamod.WorldG
{


	public class StellaWorld : ModSystem
	{

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			int MorrowGen = tasks.FindIndex(genpass => genpass.Name.Equals("Webs"));
			if (MorrowGen != -1)
			{

				tasks.Insert(MorrowGen + 1, new PassLegacy("World Gen Morrow", WorldGenMorrow));

			}

			int MorrowDig = tasks.FindIndex(genpass => genpass.Name.Equals("World Gen Morrow"));
			if (MorrowDig != -1)
			{

				tasks.Insert(MorrowDig + 1, new PassLegacy("World Gen Morrow Digging", WorldGenMorrowDig));

			}


			int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
			if (ShiniesIndex != -1)
			{

				tasks.Insert(ShiniesIndex + 1, new PassLegacy("World Gen Ores", WorldGenFlameOre));

			}


			

		}

		private void WorldGenMorrowDig(GenerationProgress progress, GameConfiguration configuration)
		{
			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Gild drilling";

			int xz = WorldGen.genRand.Next(0, Main.maxTilesX);
			int yz = WorldGen.genRand.Next((int)WorldGen.worldSurface, Main.maxTilesY);
			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
			{
				// 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.
				Tile tile = Main.tile[xz, yz];
				if (tile.HasTile && tile.TileType == ModContent.TileType<OvermorrowdirtTile>())
				{
					WorldGen.digTunnel(xz, yz, 1, 0.01f, 100, 4, false);
				}

				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.

				WorldGen.digTunnel(xz, yz, 1, 0.01f, 100, 5, false);
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
				int x = WorldGen.genRand.Next(0, Main.maxTilesX);
				int y = WorldGen.genRand.Next((int)WorldGen.rockLayerLow, Main.maxTilesY);

				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(2, 8), ModContent.TileType<VerianoreTile>());
			}
		}


		





		const string SavestringX = "Savestring1";
		const string SavestringY = "Savestring2";


		public static Point MorrowEdge = new Point(0, 0);


		private void WorldGenMorrow(GenerationProgress progress, GameConfiguration configuration)
		{
			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Gild settling in the ground";
			int xa = WorldGen.genRand.Next(0, Main.maxTilesX);
			int ya = WorldGen.genRand.Next((int)WorldGen.worldSurface, Main.maxTilesY);


			for (int da = 0; da < 1; da++)
			{
				WorldGen.TileRunner(xa, ya, WorldGen.genRand.Next(1100, 1100), WorldGen.genRand.Next(1100, 1100), ModContent.TileType<OvermorrowdirtTile>());


			}

			MorrowEdge.X = xa - 500;
			MorrowEdge.Y = ya - 100;




		

			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-07 + 86); k++)
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

			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-06 + 7); k++)
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
				Point Loc = new Point(MorrowEdge.X + Main.rand.Next(0, 1000), MorrowEdge.Y + Main.rand.Next(0, 1000));


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









	








