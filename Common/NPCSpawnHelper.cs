using Stellamod.Content.Areas.PunkerTown;
using Stellamod.Content.Biomes;
using Stellamod.Core.NPCHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common
{
    public class SpawnSets : ModSystem
    {
        public override void SetupContent()
        {
            MarshEnemy = new List<int>();
            ModifiedWeights = NPCID.Sets.Factory.CreateFloatSet(1f);
            base.SetupContent();

        }
        public static List<int> MarshEnemy;
        public static float[] ModifiedWeights;
    }

    public static class NPCSpawnExtensions
    {
        //Wrapper functions for this functionality just incase we want to change how this works
        public static void AddToMarsh(this ModNPC npc)
        {
            SpawnSets.MarshEnemy.Add(npc.Type);
        }

        public static void ModifySpawnWeight(this ModNPC npc, float multiplier)
        {
            SpawnSets.ModifiedWeights[npc.Type] = multiplier;
        }
    }

    public class NPCSpawnHelper : GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            base.EditSpawnPool(pool, spawnInfo);
            if (spawnInfo.Player.InModBiome<BiomeMarsh>())
            {
                for (int i = 0; i < SpawnSets.MarshEnemy.Count; i++)
                {
                    int marshEnemyType = SpawnSets.MarshEnemy[i];
                    float totalWeight = 1f;
                    float weight = totalWeight / (float)SpawnSets.MarshEnemy.Count;

                    //If we want to make an enemy rarer we'd do it here
                    weight *= SpawnSets.ModifiedWeights[marshEnemyType];
                    pool.TryAdd(marshEnemyType, weight);
                }
            }
        }
    }
}
