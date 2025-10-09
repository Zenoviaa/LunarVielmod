using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.NPCs
{
    public class WondrousGlobalNPC : GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            base.EditSpawnPool(pool, spawnInfo);
            if (!spawnInfo.Player.GetModPlayer<MyPlayer>().ZoneWonder)
                return;

         //   pool[NPCID.Shimmerfly] *= 0.5f;
        //    pool[NPCID.ShimmerSlime] *= 0.5f;
        //    pool[NPCID.GraniteFlyer] *= 0.5f;
        //    pool[NPCID.GraniteGolem] *= 0.5f;
        }



        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            base.EditSpawnRate(player, ref spawnRate, ref maxSpawns);
            if (!player.GetModPlayer<MyPlayer>().ZoneWonder)
                return;
            spawnRate = (int)((float)spawnRate * 0.5f);
            maxSpawns = (int)((float)maxSpawns * 1.5f);
        }
    }
}
