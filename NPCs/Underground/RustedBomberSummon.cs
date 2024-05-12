using Stellamod.Assets.Biomes;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Underground
{
    internal class RustedBomberSummon : ModNPC
    {
        private bool _spawn;
        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.aiStyle = -1;
            NPC.damage = 42;
            NPC.defense = 8;
            NPC.lifeMax = 170;
            NPC.HitSound = SoundID.NPCHit48;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = 563f;
            NPC.knockBackResist = .45f;
        }

        public override void AI()
        {
            if (!_spawn && StellaMultiplayer.IsHost)
            {
                NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X , (int)NPC.Center.Y,
                    ModContent.NPCType<RustedBomber>());

                if (Main.rand.NextBool(4))
                {
                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 30, (int)NPC.Center.Y,
                        ModContent.NPCType<MechanicalMinor>());

                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X - 45, (int)NPC.Center.Y,
                        ModContent.NPCType<MechanicalMinor>());
                }

                _spawn = true;
                NPC.Kill();
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnRates.GetMechanicalEnemySpawnChance(spawnInfo);
        }
    }
}
