using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.NPCs.Catacombs.Fire.BlazingSerpent;
using Stellamod.NPCs.Catacombs.Fire;
using Stellamod.NPCs.Catacombs.Trap.Cogwork;
using Stellamod.NPCs.Catacombs.Trap.Sparn;
using Stellamod.NPCs.Catacombs.Water.WaterCogwork;
using Stellamod.NPCs.Catacombs.Water.WaterJellyfish;
using Stellamod.Particles;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Stellamod.Tiles.Catacombs;

namespace Stellamod.NPCs.Catacombs
{
    internal class CatacombsBossSpawn : ModNPC
    {
        private int _bossType = -1;
        private float _centerSparkleSize = 0.1f;
        private ref float ai_Timer => ref NPC.ai[0];
        public override void SetDefaults()
        {
            NPC.lifeMax = 1;
            NPC.damage = 1;
            NPC.friendly = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.width = 1;
            NPC.height = 1;
        }

        private bool InBiome(int tileType)
        {
            int searchRange = 50;
            Point start = NPC.position.ToTileCoordinates();
            for(int i = start.X; i < start.X + searchRange && i < Main.maxTilesX; i++)
            {
                for(int j = start.Y; j < start.Y + searchRange && j < Main.maxTilesY; j++)
                {
                    if (Main.tile[i, j].TileType == tileType)
                        return true;
                }
            }
            return false;
        }
        public override void AI()
        {
            if(_bossType == -1)
            {
                if (InBiome(ModContent.TileType<CatacombStoneFire>()))
                {
                    _bossType = 0;
                }
                else if (InBiome(ModContent.TileType<CatacombStoneTrap>()))
                {
                    _bossType = 1;
                }
                else if (InBiome(ModContent.TileType<CatacombStoneWater>()))
                {
                    _bossType = 2;
                }
            }

            _centerSparkleSize += 0.02f;
            if(ai_Timer == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RisingSummon"));
                Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(NPC.Center, 9f);
            }

            ai_Timer++;
            if(ai_Timer % 4 == 0)
            {
                for(int i = 0; i  < 6; i++)
                {
                    Particle p = ParticleManager.NewParticle(NPC.Center, Vector2.Zero, ParticleManager.NewInstance<BurnParticle>(), default(Color), _centerSparkleSize);
                    p.timeLeft = 8;
                }
            }

            int duration = 180;
            if (ai_Timer < duration)
            {
                ShakeModSystem.Shake = ai_Timer/18;
                if (ai_Timer % 4 == 0)
                {
                    for (int i = 0; i < Main.rand.Next(1, 3); i++)
                    {
                        float distance = 128;
                        float particleSpeed = 8;
                        Vector2 position = NPC.Center + Main.rand.NextVector2CircularEdge(distance, distance);
                        Vector2 speed = (NPC.Center - position).SafeNormalize(Vector2.Zero) * particleSpeed;
                        Particle sparkle = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<BurnParticle>(), default(Color), Main.rand.NextFloat(0.6f, 0.8f));
                        sparkle.timeLeft = 21;
                    }

                    //Spawn Particle
                    Vector2 edge = Main.rand.NextVector2CircularEdge(8, 8);
                    Vector2 spawnPosition = NPC.Center + edge;
                    Vector2 velocity = NPC.DirectionFrom(spawnPosition);
                    Particle p = ParticleManager.NewParticle(spawnPosition, velocity, ParticleManager.NewInstance<BurnParticle>(), default(Color), Main.rand.NextFloat(0.6f, 0.8f));
                    p.timeLeft = 16;
                }

                if (Main.rand.NextBool(4))
                {
                    Vector2 edge = Main.rand.NextVector2CircularEdge(32, 32);
                    Vector2 spawnPosition = NPC.Center + edge;
                    Vector2 velocity = NPC.DirectionFrom(spawnPosition);
                    var d = Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, velocity, Scale: 1.5f);
                    d.noGravity = true;
                }
            } 
            else if(ai_Timer > duration)
            {

                for (int i = 0; i < 64; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var d = Dust.NewDustPerfect(NPC.Center, DustID.GemDiamond, speed * 8, Scale: 1.5f);
                    d.noGravity = true;
                }

                ShakeModSystem.Shake = 0;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1024f, 32f);

                //oh wait i need net code
                // NPC.NewNPC(, (int)ai_Boss_Spawn);


                int[] fireBosses = new int[]
                {
                    ModContent.NPCType<BlazingSerpentHead>(),
                    ModContent.NPCType<PandorasFlamebox>()
                };

                int[] waterBosses = new int[]
                {
                    ModContent.NPCType<WaterCogwork>(),
                    ModContent.NPCType<WaterJellyfish>()
                };

                int[] trapBosses = new int[]
                {
                    ModContent.NPCType<Cogwork>(),
                    ModContent.NPCType<Sparn>()
                };

                int[] bosses;
                Vector2 offset = Vector2.Zero;
                switch (_bossType)
                {
                    default:
                    case 0:
                        bosses = fireBosses;
                        offset = new Vector2(32 * 16, -1 * 16);
                        break;
                    case 1:
                        bosses = trapBosses;
                        offset = new Vector2(-25 * 16, -30 * 16);
                        break;
                    case 2:
                        bosses = waterBosses;
                        break;
                }


                int bossType = bosses[Main.rand.Next(0, bosses.Length)];
                if (StellaMultiplayer.IsHost)
                {
                    int npcID = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, bossType);

			if(_bossType != 2)
			{
                    		Projectile.NewProjectile(NPC.GetSource_FromThis(), 
                        (int)NPC.Center.X + offset.X, 
                        (int)NPC.Center.Y + offset.Y, 0, 0,
                        ModContent.ProjectileType<CatacombsBeamBarrier>(), 100, 1, Main.myPlayer);
			}
                    Main.npc[npcID].netUpdate2 = true;
                }

         

                NPC.Kill();
                ai_Timer = 0;
            }
        }
    }
}
