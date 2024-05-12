using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.NPCs.Desert.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Desert
{
    internal class DustNadoSpawner : ModNPC
    {
        private ref float Timer => ref NPC.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            NPC.width = 1;
            NPC.height = 1;
            NPC.lifeMax = 1;
            NPC.defense = 1;
            NPC.damage = 1;
            NPC.HitSound = SoundID.NPCHit32;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.value = 563f;
            NPC.knockBackResist = .45f;
        }

        public override void AI()
        {
            NPC.TargetClosest();
            Timer++;
            if(Timer == 1)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Vector2 velocity = Vector2.Zero;
                    Player target = Main.player[NPC.target];
                    if(target.position.X > NPC.position.X)
                    {
                        velocity.X = 1.5f;
                    }
                    else
                    {
                        velocity.X = -1.5f;
                    }
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity,
                        ModContent.ProjectileType<DustNadoProj>(), 18, 1, Owner: Main.myPlayer);
                }
                NPC.Kill();
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneDesert)
            {
                return SpawnCondition.OverworldDayDesert.Chance * 1.6f;
            }
            return 0f;
        }
    }
}
