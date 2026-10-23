using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.DevTools;

public class DisableNPCSpawns : GlobalNPC
{
    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (WorldDevTools.DisableNPCSpawns)
        {
            spawnRate = 0;
            maxSpawns = 0;
        }
    }
}
