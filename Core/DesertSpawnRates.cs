using Stellamod.Content.Areas.Collosseum.NPCsCL;
using Stellamod.Content.Biomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core
{
    public class DesertSpawnRates : GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            base.EditSpawnPool(pool, spawnInfo);
            if (Main.dayTime)
            {
                if (pool.ContainsKey(NPCID.Vulture))
                {
                    pool[NPCID.Vulture] = 0f;
                }

                if (pool.ContainsKey(NPCID.Antlion))
                {
                    pool[NPCID.Antlion] = 0f;
                }

                int desertPerson = ModContent.NPCType<DesertPerson>();
                if (pool.ContainsKey(desertPerson))
                {
                    pool[desertPerson] *= 2;
                }
            }
        }
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            base.EditSpawnRate(player, ref spawnRate, ref maxSpawns);
            //More towns people
            if (Main.dayTime && player.GetModPlayer<BiomePlayer>().ZoneDesertTown)
            {
                float spRate = spawnRate;
                spawnRate = (int)(spRate * 0.3f);
                maxSpawns *= 2;
            }
        }
    }
}
