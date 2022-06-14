
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

namespace Stellamod.WorldG
{


	public class StellaWorld : ModSystem
	{

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			int MorrowGen = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle"));
			if (MorrowGen != -1)
			{

				tasks.Insert(MorrowGen + 1, new PassLegacy("World Gen Morrow", WorldGenMorrow));

			}

			int MorrowDig = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
			if (MorrowDig != -1)
			{

				tasks.Insert(MorrowDig + 1, new PassLegacy("World Gen Morrow Digging", WorldGenMorrowDig));

			}


			int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
			if (ShiniesIndex != -1)
			{

				tasks.Insert(ShiniesIndex + 1, new PassLegacy("World Gen Ores", WorldGenFlameOre));

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










		public static Point MorrowEdge = new Point(0, 0);


		private void WorldGenMorrow(GenerationProgress progress, GameConfiguration configuration)
		{
			// 7. Setting a progress message is always a good idea. This is the message the user sees during world generation and can be useful for identifying infinite loops.      
			progress.Message = "Gild settling in the ground";
			int xa = WorldGen.genRand.Next(0, Main.maxTilesX);
			int ya = WorldGen.genRand.Next((int)WorldGen.worldSurface, Main.maxTilesY);


			for (int da = 0; da < 1; da++)
			{
				WorldGen.TileRunner(xa, ya, WorldGen.genRand.Next(1200, 1200), WorldGen.genRand.Next(1000, 1000), ModContent.TileType<OvermorrowdirtTile>());


			}

			MorrowEdge.X = xa - 500;
			MorrowEdge.Y = ya - 200;


			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-05); k++)
			{
				// 10. We randomly choose an x and y coordinate. The x coordinate is choosen from the far left to the far right coordinates. The y coordinate, however, is choosen from between WorldGen.worldSurfaceLow and the bottom of the map. We can use this technique to determine the depth that our ore should spawn at.


				// 11. Finally, we do the actual world generation code. In this example, we use the WorldGen.TileRunner method. This method spawns splotches of the Tile type we provide to the method. The behavior of TileRunner is detailed in the Useful Methods section below.
				Tile tile = Main.tile[xa, ya];
				if (tile.HasTile && tile.TileType == ModContent.TileType<OvermorrowdirtTile>())
				{
					WorldGen.digTunnel(xa, ya, 1, 0.01f, 100, 4, false);
				}

			}

			//Point Loc = new Point(MorrowEdge.X + Main.rand.Next(0, 1100), MorrowEdge.Y + Main.rand.Next(0, 1000));
			
		 	//	WorldGen.PlaceWall(Loc.X, Loc.Y, ModContent.WallType<OvermorrowdirtWall>());
		 
            

			
            

			for (int i = MorrowEdge.X; i < MorrowEdge.X + 1000; i++){
				for (int j = MorrowEdge.Y; j < MorrowEdge.Y + 800; j++)
			{
				WorldGen.PlaceWall(i, j, ModContent.WallType<OvermorrowdirtWall>());
			}
			}

		}
		



		const string SavestringX = "Savestring1";
		const string SavestringY = "Savestring2";




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
	}
}