using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Niivi.Projectiles;
using Stellamod.Particles;
using Stellamod.UI.Dialogue;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi
{
    internal partial class Niivi
    {
        Player Target => Main.player[NPC.target];
        IEntitySource EntitySource => NPC.GetSource_FromThis();
        float DirectionToTarget
        {
            get
            {
                if (Target.position.X < NPC.position.X)
                    return -1;
                return 1;
            }
        }

        int AttackCount;
        int AttackSide;
        bool Slowdown;
        float TintOpacity;
        Vector2 AttackPos;
        private void AIBossFight()
        {
            switch (BossState)
            {
                case BossActionState.Idle:
                    AI_Idle();
                    break;
                case BossActionState.Swoop_Out:
                    AI_SwoopOut();
                    break;
                case BossActionState.PrepareAttack:
                    AI_PrepareAttack();
                    break;
                case BossActionState.Frost_Breath:
                    AI_FrostBreath();
                    break;
                case BossActionState.Laser_Blast:
                    AI_LaserBlast();
                    break;
                case BossActionState.Star_Wrath:
                    AI_StarWrath();
                    break;
                case BossActionState.Charge:
                    AI_Charge();
                    break;
                case BossActionState.Thunderstorm:
                    AI_Thunderstorm();
                    break;
                case BossActionState.Baby_Dragons:
                    AI_BabyDragons();
                    break;
            }
            UpdateOrientation();
        }

        private void AI_Idle()
        {
            NPC.TargetClosest();
            Timer++;
            if(Timer >= 180)
            {
                ResetState(BossActionState.PrepareAttack);
                NextAttack = BossActionState.Frost_Breath;
            }

            UpdateOrientation();
            NPC.velocity *= 0.98f;
        }

        private void ChargeVisuals<T>(float timer, float maxTimer) where T : Particle
        {
            float progress = timer / maxTimer;
            float minParticleSpawnSpeed = 8;
            float maxParticleSpawnSpeed = 2;
            int particleSpawnSpeed = (int)MathHelper.Lerp(minParticleSpawnSpeed, maxParticleSpawnSpeed, progress);
            if (timer % particleSpawnSpeed == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(168, 168);
                    Vector2 vel = (NPC.Center - pos).SafeNormalize(Vector2.Zero) * 4;
                    ParticleManager.NewParticle<T>(pos, vel, Color.White, 1f);
                }
            }
        }

        private void AI_FrostBreath()
        {

            /*
            //Ok so how is this attack going to work
            Step 1: Niivi takes aim at you for a few seconds, then rotates her head 135 degrees upward
            
            Step 2: Snowflake and snow particles circle around into her magic sigil thing
            
            Step 3: Then they form a frosty rune/sigil thingy
            
            Step 4: A second or two later, Niivi starts breathing the ice breath while slowly rotating her head
            
            Step 5: The attack goes 180 degrees, or a little more, so you'll need to move behind her
            
            Step 6: The breath spews out a windy looking projectile that quickly expands and dissipates
            
            Step 7: Stars and snowflake particles also come out
            
            Step 8: The screen is tinted blue/white during this attack (shader time!)
            
            Step 9: When the breath collides with tiles (including platforms if possible), 
                there is a chance for it to form large icicles, these are NPCs, you can break them
                they have a snowy aura

            Step 10: Niivi stops breathing once she reaches the edge of her range,
                she turns around towards you and fires three frost blasts while slowly flying away

            Step 11: The frost blasts travel a short distance, (slowing down over time)
                when they come to complete stop, they explode into icicles that are affected by gravity

            Step 12: Niivi flies away to decide a new attack
           
            In Phase 2:
                Niivi does frost balls before doing the breath, and rotates her head slightly faster
            */

            if(AttackTimer == 0)
            {
                //Taking aim
                Timer++;

                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();
                if(Timer >= 60)
                {
                    NPC.velocity = -Vector2.UnitY;
                    Timer = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 1)
            {
                //Rotate head 90-ish degrees upward
                Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);

                float targetRotation = MathHelper.PiOver2 * -AttackSide;
                Vector2 rotatedDirection = directionToTarget.RotatedBy(targetRotation);
                TargetHeadRotation = rotatedDirection.ToRotation();

                //Slowly accelerate up while charging
                NPC.velocity *= 1.002f;

                Timer++;
                if(Timer == 1)
                {                 
                    Vector2 velocity = Vector2.Zero;
                    int type = ModContent.ProjectileType<NiiviFrostTelegraphProj>();
                    Projectile.NewProjectile(EntitySource, NPC.Center, velocity, type,
                        0, 0, Main.myPlayer);
                }

                //Charge up
                ChargeVisuals<SnowFlakeParticle>(Timer, 60);
                if(Timer >= 60)
                {
                    Timer = 0;
                    AttackTimer++;
                }
            } 
            else if (AttackTimer == 2)
            {
                //Get the shader system!
                ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                shaderSystem.TintScreen(Color.Cyan, 0.16f);
                shaderSystem.DistortScreen(TextureRegistry.NormalNoise1, new Vector2(0.001f, 0.001f), blend: 0.05f);
                shaderSystem.VignetteScreen(-1f);

                //Slowdown over time
                NPC.velocity *= 0.99f;

                //Slowly rotate while shooting projectile
                float length = 720;
                float amountToRotateBy = 3 * MathHelper.TwoPi / length;
                amountToRotateBy = amountToRotateBy * AttackSide;
                TargetHeadRotation += amountToRotateBy;
        
                Timer++;

                if (Timer % 4 == 0)
                {
                    float particleSpeed = 16;
                    Vector2 velocity = TargetHeadRotation.ToRotationVector2() * particleSpeed;
                    velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 8);

                    Color[] colors = new Color[] { Color.Cyan, Color.LightCyan, Color.DarkCyan, Color.White };
                    Color color = colors[Main.rand.Next(0, colors.Length)];

                    //Spawn Star and Snowflake Particles
                    if (Main.rand.NextBool(2))
                    {
                        //Snowflake particle
                        ParticleManager.NewParticle<SnowFlakeParticle>(NPC.Center, velocity, color, 0.5f);
                    }
                    else
                    {
                        //Star particle
                        ParticleManager.NewParticle<StarParticle2>(NPC.Center, velocity, color, 0.5f);
                    }
                }

                if(Timer % 4 == 0 && StellaMultiplayer.IsHost)
                {
                    float speed = 16;
                    Vector2 velocity = TargetHeadRotation.ToRotationVector2() * speed;

                    //Add some random offset to the attack
                    velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 8);

                    int type = ModContent.ProjectileType<NiiviFrostBreathProj>();
                    int damage = NPC.ScaleFromContactDamage(0.25f);
                    float knockback = 1;
                    Projectile.NewProjectile(EntitySource, NPC.Center, velocity, type,
                        damage, knockback, Main.myPlayer);
                }

                if(Timer >= length)
                {
        
                    Timer = 0;
                    AttackTimer++;
                }
            } 
            else if (AttackTimer == 3)
            {
                //Untint the screen
                ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                shaderSystem.UnTintScreen();
                shaderSystem.UnDistortScreen();
                shaderSystem.UnVignetteScreen();

                Timer++;
                if(Timer == 1)
                {            
                    //Retarget, just incase target died ya know
                    NPC.TargetClosest();

                    //Re-orient incase target went behind
                    LookDirection = DirectionToTarget;
                    OrientArching();
                    FlipToDirection();

                    Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
                    TargetHeadRotation = directionToTarget.ToRotation();
                }

                if(Timer >= 30)
                {
                    float speed = 24;
                    Vector2 velocity = TargetHeadRotation.ToRotationVector2() * speed;

                    //Add some random offset to the attack
                    velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 8);

                    int type = ModContent.ProjectileType<NiiviFrostBombProj>();

                    Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(128, 128);
                    velocity *= Main.rand.NextFloat(0.5f, 1f);

                    int damage = NPC.ScaleFromContactDamage(0.25f);
                    float knockback = 1;
                    Projectile.NewProjectile(EntitySource, spawnPos, velocity, type,
                        damage, knockback, Main.myPlayer);

                    Timer = 0;
                    AttackCount++;
                }

                if(AttackCount >= 6)
                {
                    Timer = 0;
                    AttackCount = 0;
                    AttackTimer++;
                }
            } 
            else if (AttackTimer == 4)
            {
                ResetState(BossActionState.Swoop_Out);
            }
        }

        private void AI_SwoopOut()
        {
            Timer++;
            if(Timer == 1)
            {
                OrientationSpeed = 0.03f;
   
                OrientStraight();
                FlipToDirection();
          
                float swoopOutDistance = 768;
                AttackPos = NPC.Center + (-AttackSide * Vector2.UnitX * swoopOutDistance);
                NPC.velocity = NPC.Center.DirectionTo(AttackPos);
                TargetHeadRotation = NPC.velocity.ToRotation();
            }

            NPC.velocity *= 1.02f;
            NPC.velocity.Y -= 0.05f;
            if (Timer >= 120)
            {
                ResetState(BossActionState.Idle);
            }
        }

        private void AI_PrepareAttack()
        {
            Timer++;
            if (Timer == 1)
            {
                Slowdown = false;

                //Initialize Attack
                NPC.TargetClosest();
                LookDirection = DirectionToTarget;
                OrientArching();
                FlipToDirection();

                if (NPC.position.X > Target.position.X)
                {
                    AttackSide = 1;
                }
                else
                {
                    AttackSide = -1;
                }

                //Values
                float offsetDistance = 384;
                float hoverDistance = 180;

                //Get the direction
                Vector2 targetCenter = Target.Center + (AttackSide * Vector2.UnitX * offsetDistance) + new Vector2(0, -hoverDistance);
                AttackPos = targetCenter;
            }

            //Rotate Head
            TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();

            if (AttackTimer == 0)
            {
                float flySpeed = 2;
                float distance = Vector2.Distance(NPC.Center, AttackPos);
                if (distance <= flySpeed)
                {
                    Slowdown = true;
                }
                else
                {
                    //Velocity
                    Vector2 targetVelocity = NPC.Center.DirectionTo(AttackPos) * flySpeed;
                    if(distance >= 384)
                    {
                        targetVelocity *= distance / 16;
                    }
                    NPC.velocity = targetVelocity;
                }
            }

            if (Slowdown)
            {
                AttackTimer++;
                NPC.velocity *= 0.98f;
                if(AttackTimer >= 60)
                {
                    ResetState(NextAttack);
                }
            }
        }

        private void AI_LaserBlast()
        {

        }

        private void AI_StarWrath()
        {

        }

        private void AI_Charge()
        {

        }

        private void AI_Thunderstorm()
        {

        }

        private void AI_BabyDragons()
        {

        }
    }
}
