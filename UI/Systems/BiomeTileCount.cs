using Stellamod.Tiles;
using System;
using Terraria.ModLoader;

namespace Stellamod.UI.Systems
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
