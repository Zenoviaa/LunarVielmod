using Stellamod.Content.Areas.PunkerTown;
using Stellamod.Content.Areas.WaterSide;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common
{
    /// <summary>
    /// Classifies all of our NPCs, keep track of where they spawn and weights
    /// </summary>
    public class SpawnSets : ModSystem
    {
        public override void SetupContent()
        {
            HarmonicEnemy = new List<int>();
            MarshEnemy = new List<int>();
            ModifiedWeights = NPCID.Sets.Factory.CreateFloatSet(1f);
            base.SetupContent();

        }

        public static List<int> HarmonicEnemy;
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

        public static void AddToHarmonicCoralways(this ModNPC npc)
        {
            SpawnSets.HarmonicEnemy.Add(npc.Type);
        }

        public static void ModifySpawnWeight(this ModNPC npc, float multiplier)
        {
            SpawnSets.ModifiedWeights[npc.Type] = multiplier;
        }
    }

    public class NPCSpawnHelper : GlobalNPC
    {

        private void AddEnemiesFromSpawnSet(List<int> set, IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            for (int i = 0; i < set.Count; i++)
            {
                int enemyType = set[i];
                float totalWeight = 1f;
                float weight = totalWeight / (float)set.Count;

                //If we want to make an enemy rarer we'd do it here
                weight *= SpawnSets.ModifiedWeights[enemyType];
                pool.TryAdd(enemyType, weight);
            }
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            base.EditSpawnPool(pool, spawnInfo);
            if (spawnInfo.Player.InModBiome<BiomeMarsh>())
            {
                AddEnemiesFromSpawnSet(SpawnSets.MarshEnemy, pool, spawnInfo);
            }
            if (spawnInfo.Player.InModBiome<HarmonicCoralwaysBiome>())
            {
                pool.Clear();
                AddEnemiesFromSpawnSet(SpawnSets.HarmonicEnemy, pool, spawnInfo);
                pool.TryAdd(NPCID.Piranha, 0.1f);
                pool.TryAdd(NPCID.Shark, 0.1f);
                pool.TryAdd(NPCID.BlueJellyfish, 0.1f);
                pool.TryAdd(NPCID.PinkJellyfish, 0.1f);
                pool.TryAdd(NPCID.Squid, 0.1f);
                pool.TryAdd(NPCID.Crab, 0.1f);
            }
        }
    }
}
