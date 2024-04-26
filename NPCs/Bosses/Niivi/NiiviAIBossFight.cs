using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Niivi.Projectiles;
using Stellamod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
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
        bool DoAttack;
        bool IsCharging;
        Vector2 AttackPos;
        Vector2 ChargeDirection;
        Vector2 LaserAttackPos;
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
            if(Timer >= 15)
            {
                ResetState(BossActionState.PrepareAttack);
         
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
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, NPC.Center, velocity, type,
                        0, 0, Main.myPlayer);
                    }
                }

                //Charge up
                ChargeVisuals<SnowFlakeParticle>(Timer, 60);
                if(Timer >= 60)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        int type = ModContent.ProjectileType<NiiviFrostCircleProj>();
                        int damage = 0;
                        int knockback = 0;
                        Projectile.NewProjectile(EntitySource, NPC.Center, Vector2.Zero, 
                            type, damage, knockback, Main.myPlayer);
                    }

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

                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, NPC.Center, velocity, type,
                        damage, knockback, Main.myPlayer);
                    }
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
                    
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, spawnPos, velocity, type,
                        damage, knockback, Main.myPlayer);
                    }

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
                NextAttack = BossActionState.Laser_Blast;
                ResetState(BossActionState.Swoop_Out);
            }
        }

        private void AI_SwoopOut()
        {
            Timer++;
            if(Timer == 1)
            {
                OrientationSpeed = 0.03f;
                NPC.velocity = -Vector2.UnitY * 0.02f;
               // TargetHeadRotation = NPC.velocity.ToRotation();
            }

            NPC.velocity *= 1.008f;
            NPC.velocity.Y -= 0.001f;
            if (Timer >= 60)
            {
                ResetState(BossActionState.Idle);
            }
        }


        private void AI_MoveToward(Vector2 targetCenter, float speed = 8)
        {
            //chase target
            Vector2 directionToTarget = NPC.Center.DirectionTo(targetCenter);
            float distanceToTarget = Vector2.Distance(NPC.Center, targetCenter);
            if (distanceToTarget < speed)
            {
                speed = distanceToTarget;
            }

            Vector2 targetVelocity = directionToTarget * speed;

            if (NPC.velocity.X < targetVelocity.X)
            {
                NPC.velocity.X++;
                if (NPC.velocity.X >= targetVelocity.X)
                {
                    NPC.velocity.X = targetVelocity.X;
                }
            }
            else if (NPC.velocity.X > targetVelocity.X)
            {
                NPC.velocity.X--;
                if (NPC.velocity.X <= targetVelocity.X)
                {
                    NPC.velocity.X = targetVelocity.X;
                }
            }

            if (NPC.velocity.Y < targetVelocity.Y)
            {
                NPC.velocity.Y++;
                if (NPC.velocity.Y >= targetVelocity.Y)
                {
                    NPC.velocity.Y = targetVelocity.Y;
                }
            }
            else if (NPC.velocity.Y > targetVelocity.Y)
            {
                NPC.velocity.Y--;
                if (NPC.velocity.Y <= targetVelocity.Y)
                {
                    NPC.velocity.Y = targetVelocity.Y;
                }
            }
        }

        private void AI_PrepareAttack()
        {
            Timer++;
            if (Timer == 1)
            {
                DoAttack = false;

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
                float hoverDistance = 90;

                //Get the direction
                Vector2 targetCenter = Target.Center + (AttackSide * Vector2.UnitX * offsetDistance) + new Vector2(0, -hoverDistance);
                AttackPos = targetCenter;
            }

            //Rotate Head
            TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();

            if (AttackTimer == 0)
            {
                AI_MoveToward(AttackPos);
                if(Timer >= 360 || Vector2.Distance(NPC.Center, AttackPos) <= 8)
                {
                    DoAttack = true;
                }
            }

            if (DoAttack)
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
            /*
             * Step 1: Look at target for a bit
             * 
             * Step 2: Charge begin charging massive laser, white particles come in and slowly build up
             * 
             * Step 3: Fire the laser, twice?, Nah maybe three times
             */

            if(AttackTimer == 0)
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
                else if (Timer >= 120)
                {
                    //SHOOT LOL
                    Vector2 fireDirection = TargetHeadRotation.ToRotationVector2();
                    float distance = Vector2.Distance(NPC.Center, LaserAttackPos);

                    int type = ModContent.ProjectileType<NiiviLaserBlastProj>();
                    int damage = NPC.ScaleFromContactDamage(1f);
                    float knockback = 1;
                    if (StellaMultiplayer.IsHost)
                    {
                        float size = 5.5f;
                        float beamLength = distance;
                        Projectile.NewProjectile(EntitySource, NPC.Center, fireDirection, type,
                        damage, knockback, Main.myPlayer, 
                            ai0: size, 
                            ai1: beamLength);
                    }

                    for(int i = 0; i < 16; i++)
                    {
                        Vector2 particleVelocity = fireDirection * 16;
                        particleVelocity = particleVelocity.RotatedByRandom(MathHelper.PiOver4 / 3);
                        particleVelocity *= Main.rand.NextFloat(0.3f, 1f);
                        ParticleManager.NewParticle(NPC.Center, particleVelocity,
                            ParticleManager.NewInstance<StarParticle>(), Color.White, sparkleSize);
                    }

                    Timer = 0;
                    AttackCount++;
                }

                if(AttackCount >= 3)
                {
                    NextAttack = BossActionState.Star_Wrath;
                    ResetState(BossActionState.Swoop_Out);
                }
            }
        }

        private void AI_StarWrath()
        {
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
                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();
                if(Timer % 8 == 0 && StellaMultiplayer.IsHost)
                {
                    int type = ModContent.ProjectileType<NiiviCometProj>();
                    int damage = NPC.ScaleFromContactDamage(0.33f);
                    int knockback = 1;

                    float height = 768;
                    Vector2 targetCenter = Target.Center;
                    Vector2 cometOffset = -Vector2.UnitY * height + new Vector2(Main.rand.NextFloat(512, 1750), 0);
                    Vector2 cometPos = targetCenter + cometOffset;

                    float speed = 12;
                    Vector2 velocity = new Vector2(-1, 1) * speed;
                    Projectile.NewProjectile(EntitySource, cometPos, velocity,
                        type, damage, knockback, Main.myPlayer);
                }

                if(Timer >= 360)
                {
                    NextAttack = BossActionState.Charge;
                    ResetState(BossActionState.Swoop_Out);
                }
            }
        }

        private void AI_Charge()
        {
            NPC.rotation = 0;
            Timer++;
            if(Timer < 100)
            {
                OrientArching();
                if (Timer == 1)
                {
                    NPC.TargetClosest();
                }

                NPC.velocity *= 0.8f;
                Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
                TargetHeadRotation = directionToTarget.ToRotation();

                LookDirection = DirectionToTarget;
                FlipToDirection();
            } 
            else if (Timer < 150)
            {
                ChargeDirection = NPC.Center.DirectionTo(Target.Center);
                NPC.velocity *= 0.3f;
                TargetHeadRotation = MathHelper.Lerp(TargetHeadRotation, ChargeDirection.ToRotation(), 0.08f);
                StartSegmentDirection = Vector2.Lerp(StartSegmentDirection, HeadRotation.ToRotationVector2() * -LookDirection, 0.04f);
                for (int i = 0; i < NPC.oldPos.Length; i++)
                {
                    NPC.oldPos[i] = NPC.position;
                }

                LookDirection = DirectionToTarget;
                FlipToDirection();
            }
            else if (Timer < 180)
            {
                IsCharging = true;
                TargetSegmentRotation = 0;
                StartSegmentDirection = Vector2.Lerp(StartSegmentDirection, HeadRotation.ToRotationVector2() * -LookDirection, 0.04f);

                //DrawChargeTrail = true;
                if (Timer == 151)
                {
                    SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/RekRoar");
                    SoundEngine.PlaySound(soundStyle, NPC.position);
                }
                NPC.velocity = ChargeDirection * 40;
            }
            else if (Timer < 240)
            {
                IsCharging = false;
                OrientArching();
                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.Pi / 60);
                NPC.velocity *= 0.96f;
               
            }
            else
            {
                IsCharging = false;
                Timer = 0;
                AttackCount++;
                if(AttackCount >= 3)
                {
                    NextAttack = BossActionState.Frost_Breath;
                    ResetState(BossActionState.Swoop_Out);
                }
            }
        }

        private void AI_Thunderstorm()
        {

        }

        private void AI_BabyDragons()
        {

        }
    }
}
