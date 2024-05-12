using Stellamod.Assets.Biomes;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs
{
    internal static class SpawnRates
    {
        public const float Mechanical_Enemy_Spawn_Chance = 0.06f;
        public const float Flower_Spawn_Chance = 0.04f;
        public const float Rune_Spawn_Chance = 0.01f;

        public static float GetFlowerSpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<DrakonicManor>() || 
                spawnInfo.Player.InModBiome<CindersparkBiome>() || 
                spawnInfo.Player.InModBiome<IshtarBiome>())
                return 0;
            return (SpawnCondition.Cavern.Chance * SpawnRates.Flower_Spawn_Chance);
        }


        public static float GetMechanicalEnemySpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<DrakonicManor>() || 
                spawnInfo.Player.InModBiome<CindersparkBiome>() ||
                spawnInfo.Player.InModBiome<IshtarBiome>())
                return 0;
            return (SpawnCondition.Cavern.Chance * SpawnRates.Mechanical_Enemy_Spawn_Chance);
        }

        public static float GetIshtarEnemySpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!spawnInfo.Player.InModBiome<IshtarBiome>())
                return 0;
            return SpawnCondition.Cavern.Chance;
        }
    }
}
