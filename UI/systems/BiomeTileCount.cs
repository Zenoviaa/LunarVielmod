using System;
using Stellamod.Tiles;
using Terraria.ModLoader;

namespace Stellamod.UI.systems
{
	public class BiomeTileCount : ModSystem
	{
		public int BlockCount;

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
		{
			BlockCount = tileCounts[ModContent.TileType<OvermorrowdirtTile>()];
		}
	}
}
