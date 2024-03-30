using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Sylia.Projectiles;
using Stellamod.Particles;
using Stellamod.Projectiles.Swords;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Sylia
{
    public partial class Sylia
    {
        private void AIIntro()
        {
            var npcSource = NPC.GetSource_FromThis();
            NPC.TargetClosest();
            
            //Ok so,
            //Her spawn animation
            //A bunch of ripping particles appear around the rift
            Timer++;
            if(Timer == 1)
            {
                ArenaCenter = NPC.Center;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(NPC.Center, 9f);
            }

            //Ripping Particles
            //it should get faster over time
            float progress = Timer / 120;
            float minParticleSpawnSpeed = 8;
            float maxParticleSpawnSpeed = 3;
            int particleSpawnSpeed = (int)MathHelper.Lerp(minParticleSpawnSpeed, maxParticleSpawnSpeed, progress);
            if(Timer < 120 && Timer % particleSpawnSpeed == 0)
            {
                //Spawn a particle at sthe center of the arena
                if (StellaMultiplayer.IsHost)
                {
                    Vector2 randOffset = Main.rand.NextVector2Circular(4, 4);
                    Projectile.NewProjectile(npcSource, ArenaCenter + randOffset, randOffset,
                        ModContent.ProjectileType<RipperSlashProjSmall>(), 0, 0, Main.myPlayer);
                }
            }

            if(Timer == 120)
            {
                //Time to emerge
                Particle telegraphPart1 = ParticleManager.NewParticle(ArenaCenter, Vector2.Zero,
                    ParticleManager.NewInstance<RipperSlashTelegraphParticle>(), Color.White, 1f);

                Particle telegraphPart2 = ParticleManager.NewParticle(ArenaCenter, Vector2.Zero,
                    ParticleManager.NewInstance<RipperSlashTelegraphParticle>(), Color.White, 1f);

                telegraphPart1.rotation = MathHelper.ToRadians(-45);
                telegraphPart2.rotation = MathHelper.ToRadians(45);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlashTelegraph"));
            }

            //Until one big slash and she emerges
            if (Timer == 150)
            {
                DrawSylia = true;
                if (StellaMultiplayer.IsHost)
                {
                    //X Slash
                    Vector2 velocity = Vector2.UnitX.RotatedBy(-MathHelper.PiOver4);
                    Projectile.NewProjectile(npcSource, NPC.Center, velocity,
                        ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0, Main.myPlayer);
                    velocity = velocity.RotatedBy(-MathHelper.PiOver2);
                    Projectile.NewProjectile(npcSource, NPC.Center, velocity,
                        ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0, Main.myPlayer);
                }
            }

            //Then she flies down to the center, summons her hands and magic circle
            if(Timer == 180)
            {
                DrawMagicCircle = true;
            }

            //Then she summons the void barriers
            if(Timer == 240)
            {
                float voidBarrierRadius = ArenaRadius;
                Vector2 leftBarrierPos = ArenaCenter + voidBarrierRadius * Vector2.UnitX;
                Vector2 rightBarrierPos = ArenaCenter + voidBarrierRadius * -Vector2.UnitX;
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(npcSource, leftBarrierPos, -Vector2.UnitY,
                        ModContent.ProjectileType<VoidBeamBarrier>(), NPC.ScaleFromContactDamage(1f), 4, Main.myPlayer);
                    Projectile.NewProjectile(npcSource, rightBarrierPos, -Vector2.UnitY,
                        ModContent.ProjectileType<VoidBeamBarrier>(), NPC.ScaleFromContactDamage(1f), 4, Main.myPlayer);
                }

                //Summon the portals
                Dust.QuickDustLine(NPC.position, leftBarrierPos, 50, ColorFunctions.MiracleVoid);
                Dust.QuickDustLine(NPC.position, rightBarrierPos, 50, ColorFunctions.MiracleVoid);

                SoundStyle soundStyle = SoundID.Item100;
                soundStyle.Pitch = -0.75f;
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
            }

            if(Timer == 300)
            {
                Phase = ActionPhase.Phase_1;
                Timer = 0;
                NPC.netUpdate = true;
            }
        }
    }
}
