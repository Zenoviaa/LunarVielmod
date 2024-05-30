using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Desert
{
    internal class BabySwarmerSummon : ModNPC
    {
        private bool _spawn;
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.aiStyle = -1;
            NPC.damage = 42;
            NPC.defense = 8;
            NPC.lifeMax = 400;
            NPC.HitSound = SoundID.NPCHit48;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = 563f;
            NPC.knockBackResist = .45f;
        }

        public override void AI()
        {
            if (!_spawn && StellaMultiplayer.IsHost)
            {
                for(int i =0;i < Main.rand.Next(3, 8); i++)
                {
                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y,
                        ModContent.NPCType<BabySwarmer>());
                }


                _spawn = true;
                NPC.Kill();
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            //You can't be in the surface and underground at the same time so this should work
            //0.05f should make it 20 less common than normal spawns.
            if (!NPC.downedPlantBoss)
                return 0;
          
            return (SpawnCondition.DesertCave.Chance * 0.05f) + (SpawnCondition.OverworldDayDesert.Chance * 0.05f);
        }
    }
}
