using Stellamod.Assets.Biomes;
using Stellamod.Content.Areas.PunkerTown;
using Stellamod.Content.Areas.SpringHills;
using Stellamod.Content.Areas.Terror;
using Stellamod.Content.Areas.Tundra.Abyss;
using Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;
using Stellamod.Content.Areas.Tundra.Abyss.TilesAB.Aurelus;
using Stellamod.Content.Areas.Underground;
using Stellamod.Content.Areas.WaterSide;
using Stellamod.Core.NPCHelpers;
using Stellamod.Items.Placeable.Cathedral;
using Stellamod.WorldG;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common;

/// <summary>
/// Classifies all of our NPCs, keep track of where they spawn and weights
/// </summary>
public class SpawnSets : ModSystem
{
    public override void SetupContent()
    {
        SpringEnemy = new List<int>();
        HarmonicEnemy = new List<int>();
        MarshEnemy = new List<int>();
        AegislavSurfaceEnemy = new List<int>();
        HeatedDepthsEnemy = new List<int>();
        FableEnemy = new List<int>();
        AbyssEnemy = new List<int>();
        AbyssWaterEnemy = new List<int>();
        AbyssCritter = new List<int>();
        AbyssTempleEnemy = new List<int>();
   
        base.SetupContent();

    }

    public override void ResizeArrays()
    {
        base.ResizeArrays();
        ModifiedWeights = NPCID.Sets.Factory.CreateFloatSet(1f);
        TryNotToSpawnOnWater = NPCID.Sets.Factory.CreateBoolSet();
    }
    public static List<int> SpringEnemy;
    public static List<int> HarmonicEnemy;
    public static List<int> MarshEnemy;
    public static List<int> AegislavSurfaceEnemy;
    public static List<int> HeatedDepthsEnemy;
    public static List<int> FableEnemy;
    public static List<int> AbyssEnemy;
    public static List<int> AbyssCritter;
    public static List<int> AbyssWaterEnemy;
    public static List<int> AbyssTempleEnemy;
    public static float[] ModifiedWeights;
    public static bool[] TryNotToSpawnOnWater;

}

public static class NPCSpawnExtensions
{
    extension(NPCID.Sets)
    {
        public static bool[] TryNotToSpawnOnWater => SpawnSets.TryNotToSpawnOnWater;
    }

    public static void PreferLand(this ModNPC npc)
    {
        NPCID.Sets.TryNotToSpawnOnWater[npc.Type] = true;
    }


    //Wrapper functions for this functionality just incase we want to change how this works
    public static void AddToSpringHills(this ModNPC npc)
    {
        SpawnSets.SpringEnemy.Add(npc.Type);
    }

    public static void AddToMarsh(this ModNPC npc)
    {
        SpawnSets.MarshEnemy.Add(npc.Type);
    }

    public static void AddToHarmonicCoralways(this ModNPC npc)
    {
        SpawnSets.HarmonicEnemy.Add(npc.Type);
    }

    public static void AddToHeatedDepths(this ModNPC npc)
    {
        SpawnSets.HeatedDepthsEnemy.Add(npc.Type);
    }

    public static void AddToFable(this ModNPC npc)
    {
        SpawnSets.FableEnemy.Add(npc.Type);
    }
    public static void AddToAbyss(this ModNPC npc)
    {
        SpawnSets.AbyssEnemy.Add(npc.Type);
    }
    public static void AddToAbyssCritter(this ModNPC npc)
    {
        SpawnSets.AbyssCritter.Add(npc.Type);
    }
    public static void AddToAbyssTemple(this ModNPC npc)
    {
        SpawnSets.AbyssTempleEnemy.Add(npc.Type);
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
            if (spawnInfo.Water && NPCID.Sets.TryNotToSpawnOnWater[enemyType])
                continue;

            float totalWeight = 1f;
            float weight = totalWeight / (float)set.Count;

            //If we want to make an enemy rarer we'd do it here
            weight *= SpawnSets.ModifiedWeights[enemyType];
            pool.TryAdd(enemyType, weight);
        }
    }
    private void AddEnemiesFromSpawnSet(List<int> set, IDictionary<int, float> pool, NPCSpawnInfo spawnInfo, int[] tileTypesToIgnore)
    {
        for(int i = 0; i < tileTypesToIgnore.Length; i++)
        {
            int tileType = tileTypesToIgnore[i];
            if (spawnInfo.SpawnTileType == tileType)
                return;
        }

        for (int i = 0; i < set.Count; i++)
        {
            int enemyType = set[i];
            if (spawnInfo.Water && NPCID.Sets.TryNotToSpawnOnWater[enemyType])
                continue;

            float totalWeight = 1f;
            float weight = totalWeight / (float)set.Count;

            //If we want to make an enemy rarer we'd do it here
            weight *= SpawnSets.ModifiedWeights[enemyType];
            pool.TryAdd(enemyType, weight);
        }
    }
    private void AddEnemiesFromSpawnSet(List<int> set, IDictionary<int, float> pool, NPCSpawnInfo spawnInfo, Rectangle ignoreTileRectangle)
    {
        if (ignoreTileRectangle.Contains(new Point(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY)))
            return;

        for (int i = 0; i < set.Count; i++)
        {
            int enemyType = set[i];
            if (spawnInfo.Water && NPCID.Sets.TryNotToSpawnOnWater[enemyType])
                continue;

            float totalWeight = 1f;
            float weight = totalWeight / (float)set.Count;

            //If we want to make an enemy rarer we'd do it here
            weight *= SpawnSets.ModifiedWeights[enemyType];
            pool.TryAdd(enemyType, weight);
        }
    }
    public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
    {
        base.EditSpawnRate(player, ref spawnRate, ref maxSpawns);
        if (player.InModBiome<AbyssBiome>())
        {
            float sp = (float)spawnRate;
            sp *= 0.6f;
       //     spawnRate = (int)sp;


            float ms = (float)maxSpawns;
            ms *= 1.4f;
//maxSpawns = (int)ms;
        }
    }

    public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
    {
        base.EditSpawnPool(pool, spawnInfo);
        if (spawnInfo.Player.ZoneForest || spawnInfo.Player.ZonePurity || spawnInfo.Player.InModBiome<SpringHillsBiome>())
        {
            AddEnemiesFromSpawnSet(SpawnSets.SpringEnemy, pool, spawnInfo);
        }

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
        if (spawnInfo.Player.InModBiome<AegislavBiome>())
        {
            pool.Clear();
            AddEnemiesFromSpawnSet(SpawnSets.AegislavSurfaceEnemy, pool, spawnInfo);
            pool.TryAdd(NPCID.BloodCrawler, 0.1f);
            pool.TryAdd(NPCID.FaceMonster, 0.1f);
            pool.TryAdd(NPCID.Crimera, 0.1f);
        }
        if (spawnInfo.Player.InModBiome<HeatedDepthsBiome>())
        {
            AddEnemiesFromSpawnSet(SpawnSets.HeatedDepthsEnemy, pool, spawnInfo);
        }
        if (spawnInfo.Player.InModBiome<FableBiome>())
        {
            AddEnemiesFromSpawnSet(SpawnSets.FableEnemy, pool, spawnInfo);
        }
        if (spawnInfo.Player.InModBiome<AbyssBiome>())
        {
            pool.Clear();

            if(!BellFlowerSystem.Whispering)
                AddEnemiesFromSpawnSet(SpawnSets.AbyssEnemy, pool, spawnInfo, SavedGenerationParameters.AbyssTempleRectangle);
            AddEnemiesFromSpawnSet(SpawnSets.AbyssCritter, pool, spawnInfo, SavedGenerationParameters.AbyssTempleRectangle);
        }
        if (spawnInfo.Player.InModBiome<AurelusBiome>())
        {
            pool.Clear();
            if(SavedGenerationParameters.AbyssTempleRectangle.Contains(new Point(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY)))
            {
                AddEnemiesFromSpawnSet(SpawnSets.AbyssTempleEnemy, pool, spawnInfo);

            }

        }
    }
}
