
using Terraria.ModLoader;
using System;
using Terraria.ModLoader;
using Stellamod.Tiles.Abyss;
using Stellamod.Tiles.Acid;

namespace Stellamod
{
    public class BiomeTileCounts : ModSystem
    {
        public int AbyssCount;
        public static bool InAbyss => ModContent.GetInstance<BiomeTileCounts>().AbyssCount > 80;


        public int AcidCount;
        public static bool InAcid => ModContent.GetInstance<BiomeTileCounts>().AcidCount > 80;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            AcidCount = tileCounts[ModContent.TileType<AcidialDirt>()];
            AbyssCount = tileCounts[ModContent.TileType<AbyssalDirt>()];

        }
    }
}