using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core
{
    public class NoRandomEnemySpawn : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            base.EditSpawnRate(player, ref spawnRate, ref maxSpawns);
            if (Main.CurrentFrameFlags.AnyActiveBossNPC)
            {
                spawnRate = 0;
                maxSpawns = 0;
            }
        }
    }
}
