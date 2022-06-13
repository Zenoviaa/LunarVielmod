using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Input;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using Stellamod.Tiles;

namespace Stellamod
{
	class WorldGenThingy : ModSystem
	{
		public static bool JustPressed(Keys key)
		{
			return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
		}

		public override void PostUpdateWorld()
		{
			if (JustPressed(Keys.D1))
				TestMethod((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
		}

		private void TestMethod(int x, int y)
		{
			Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, Color.YellowGreen, null);
			//WorldGen.digTunnel(x, y, 1, 0.01f, 100, 4, false);
			//WorldGen.TileRunner(x, y, WorldGen.genRand.Next(600, 700), WorldGen.genRand.Next(500, 600), ModContent.TileType<OvermorrowdirtTile>());
			// Code to test placed here:
			WorldGen.TileRunner(x, y, WorldGen.genRand.Next(600, 700), WorldGen.genRand.Next(500, 600), ModContent.TileType<OvermorrowdirtTile>());
		}
	}
}