using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Niivi.Projectiles;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi
{
    internal partial class Niivi
    {
        private void AI_Phase2_Reset()
        {
            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            screenShaderSystem.FlashTintScreen(Color.White, 0.3f, 5);
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.position);
            ResetShaders();
            ResetState(BossActionState.Swoop_Out);
            NextAttack = BossActionState.Laser_Blast_V2;
        }

        private void AI_FrostBreath_V2()
        {

        }

        private void AI_Charge_V2()
        {

        }

        private void AI_LaserBlast_V2()
        {
            /*
            * Step 1: Look at target for a bit
            * 
            * Step 2: Charge begin charging massive laser, white particles come in and slowly build up
            * 
            * Step 3: Fire the laser, twice?, Nah maybe three times
            */

            if (AttackTimer == 0)
            {
                Timer++;

                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();
                if (Timer >= 60)
                {
                    NPC.velocity = -Vector2.UnitY;
                    Timer = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 1)
            {
                Timer++;
                float progress = Timer / 60;
                progress = MathHelper.Clamp(progress, 0, 1);
                float sparkleSize = MathHelper.Lerp(0f, 4f, progress);
                if (Timer % 4 == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Particle p = ParticleManager.NewParticle(NPC.Center, Vector2.Zero,
                            ParticleManager.NewInstance<GoldSparkleParticle>(), Color.White, sparkleSize);
                        p.timeLeft = 8;
                    }
                }

                //Rotate head 90-ish degrees upward
                if (Timer < 60)
                {
                    LaserAttackPos = Target.Center;
                    Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
                    TargetHeadRotation = directionToTarget.ToRotation();

                    //Slowly accelerate up while charging
                    NPC.velocity *= 1.002f;

                    //Charge up
                    ChargeVisuals<StarParticle2>(Timer, 60);
                }
                else if (Timer == 61)
                {
                    Vector2 velocity = Vector2.Zero;
                    int type = ModContent.ProjectileType<NiiviFrostTelegraphProj>();
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, LaserAttackPos, velocity, type,
                            0, 0, Main.myPlayer);
                    }
                }
                else if (Timer == 120)
                {
                    //SHOOT LOL
                    Vector2 fireDirection = TargetHeadRotation.ToRotationVector2();
                    int type = ModContent.ProjectileType<NiiviLaserContinuousProj>();
                    int damage = P1_LaserDamage;
                    float knockback = 1;
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, NPC.Center, fireDirection, type,
                        damage, knockback, Main.myPlayer, ai1: NPC.whoAmI);
                    }
                    NPC.rotation = HeadRotation;

                    ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                    shaderSystem.TintScreen(Color.White, 0.3f);
                } 
                else if (Timer >= 120)
                {
                    NPC.velocity *= 0.98f;
                    TargetHeadRotation += 0.02f;
                    NPC.rotation += 0.02f;
                }
                
                if (Timer >= 720)
                {
                    ResetShaders();
                    NextAttack = BossActionState.Laser_Blast_V2;
                    ResetState(BossActionState.Swoop_Out);
                }
            }
        }

        private void AI_StarWrath_V2()
        {

        }

        private void AI_Thunderstorm_V2()
        {

        }
    }
}
