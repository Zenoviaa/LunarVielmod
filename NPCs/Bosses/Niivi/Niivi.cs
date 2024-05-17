using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Items.Placeable;
using Stellamod.NPCs.Bosses.Niivi.Projectiles;
using Stellamod.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Niivi
{
    [AutoloadBossHead]
    internal partial class Niivi : ModNPC
    {
        public enum ActionState
        {
            Spawn,
            Idle,
            Frost_Breath,
            Laser_Blast,
            Star_Wrath,
            Star_Storm,
            Thunderstorm,
            Baby_Dragons,
            Swoop_Out,
            PrepareAttack,
            Calm_Down,
            Frost_Breath_V2,
            Laser_Blast_V2,
            Star_Wrath_V2,
            Charge_V2,
            Thunderstorm_V2
        }

        public ActionState State
        {
            get
            {
                return (ActionState)NPC.ai[0];
            }
            set
            {
                NPC.ai[0] = (float)value;
            }
        }


        //Damage Values
        private int P1_LightningDamage => 240;
        private int P1_FrostBreathDamage => 120;
        private int P1_StarWrathDamage => 40;
        private int P1_LaserDamage => 500;
        private int P1_StarStormDamage => 40;

        //AI
        private bool _resetTimers;
        ref float Timer => ref NPC.ai[1];
        ref float AttackTimer => ref NPC.ai[2];
        public ActionState NextAttack
        {
            get => (ActionState)NPC.ai[3];
            set => NPC.ai[3] = (float)value;
        }
        private float CrystalTimer;

        int ScaleDamageCounter;
        int AggroDamageCounter;
  
        private Player Target => Main.player[NPC.target];
        private IEntitySource EntitySource => NPC.GetSource_FromThis();
        private float DirectionToTarget
        {
            get
            {
                if (Target.position.X < NPC.position.X)
                    return -1;
                return 1;
            }
        }

        //Phase Switches
        private bool InPhase2 => NPC.life <= (NPC.lifeMax * 0.66f);
        private bool TriggeredPhase2;

        private bool InPhase3 => NPC.life <= (NPC.lifeMax * 0.22f);
        private bool TriggeredPhase3;

        private bool InPhase4 => NPC.life <= (NPC.lifeMax * 0.01f);
        private bool TriggeredPhase4;

        private int AttackCount;
        private int AttackSide;
        private bool DoAttack;
        private bool IsCharging;
        private Vector2 AttackPos;
        private Vector2 ChargeDirection;
        private Vector2 LaserAttackPos;

        private void FinishResetTimers()
        {
            if (_resetTimers)
            {
                Timer = 0;
                AttackTimer = 0;
                AttackCount = 0;
                _resetTimers = false;
            }
        }

        private void ResetTimers()
        {
            if (StellaMultiplayer.IsHost)
            {
                _resetTimers = true;
                NPC.netUpdate = true;
            }
        }


        private void ResetState(ActionState bossActionState)
        {
            State = bossActionState;
            ResetTimers();
            NPC.netUpdate = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(_resetTimers);
            writer.Write(ScaleDamageCounter);
            writer.Write(AggroDamageCounter);
            writer.Write(CrystalTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            _resetTimers = reader.ReadBoolean();
            ScaleDamageCounter = reader.ReadInt32();
            AggroDamageCounter = reader.ReadInt32();
            CrystalTimer = reader.ReadSingle();
        }

        public override void SetStaticDefaults()
        {
            //Don't want her to be hit by any debuffs
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.TrailCacheLength[Type] = Total_Segments;
            NPCID.Sets.TrailingMode[Type] = 2;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
            drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/Niivi/NiiviPreview";
            drawModifiers.PortraitScale = 0.8f; // Portrait refers to the full picture when clicking on the icon in the bestiary
            drawModifiers.PortraitPositionYOverride = 0f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("Niivi, The First Dragon")
            });
        }

        public override void SetDefaults()
        {
            //Stats
            NPC.lifeMax = 232000;
            NPC.defense = 110;
            NPC.damage = 240;
            NPC.width = (int)NiiviHeadSize.X;
            NPC.height = (int)NiiviHeadSize.Y;
            //It won't be considered a boss or take up slots until the fight actually starts
            //So the values are like this for now
            NPC.boss = true;
            NPC.npcSlots = 0f;
            NPC.aiStyle = -1;

            //She'll tile collide and have gravity while on the ground, but not while airborne.
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0;

            NPC.BossBar = ModContent.GetInstance<NiiviBossBar>();
            NPC.HitSound = SoundID.DD2_WitherBeastCrystalImpact;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Niivi");
            }

            Crystals = new NiiviCrystalDraw[3];
            Crystals[0] = new NiiviCrystalDraw(NPC, ModContent.Request<Texture2D>($"{BaseProjectileTexturePath}NiiviCrystalFrost").Value);
            Crystals[1] = new NiiviCrystalDraw(NPC, ModContent.Request<Texture2D>($"{BaseProjectileTexturePath}NiiviCrystalLightning").Value);
            Crystals[2] = new NiiviCrystalDraw(NPC, ModContent.Request<Texture2D>($"{BaseProjectileTexturePath}NiiviCrystalStars").Value);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            //Return false for no Contact Damage
            if (State == ActionState.Star_Storm)
                return IsCharging;
            return false;
        }


        public override void AI()
        {
            FinishResetTimers();
            AI_PhaseSwaps();

            if (!NPC.HasValidTarget)
            {
                //Despawn basically
                NPC.TargetClosest();
                if (!NPC.HasValidTarget && State != ActionState.Calm_Down)
                {
                    ResetState(ActionState.Calm_Down);
                }
            }

            if(State != ActionState.Spawn)
            {
                Crystals[0].Draw = !NPC.AnyNPCs(ModContent.NPCType<NiiviCrystalFrost>());
                Crystals[1].Draw = !NPC.AnyNPCs(ModContent.NPCType<NiiviCrystalLightning>());
                Crystals[2].Draw = !NPC.AnyNPCs(ModContent.NPCType<NiiviCrystalStars>());
            }
            CrystalTimer++;

            if (ChargeCrystals == false)
            {
                ChargeCrystalTimer -= 1 / 60f;
            }
            else
            {
                ChargeCrystalTimer += 1 / 60f;
            }
            ChargeCrystalTimer = MathHelper.Clamp(ChargeCrystalTimer, 0f, 1f);

            switch (State)
            {
                case ActionState.Spawn:
                    AI_Spawn();
                    break;
                case ActionState.Idle:
                    AI_Idle();
                    break;
                case ActionState.Swoop_Out:
                    AI_SwoopOut();
                    break;
                case ActionState.PrepareAttack:
                    AI_PrepareAttack();
                    break;
                case ActionState.Frost_Breath:
                    AI_FrostBreath();
                    break;
                case ActionState.Laser_Blast:
                    AI_LaserBlast();
                    break;
                case ActionState.Star_Wrath:
                    AI_StarWrath();
                    break;
                case ActionState.Star_Storm:
                    AI_StarStorm();
                    break;
                case ActionState.Thunderstorm:
                    AI_Thunderstorm();
                    break;
                case ActionState.Baby_Dragons:
                    AI_BabyDragons();
                    break;
                case ActionState.Calm_Down:
                    AI_CalmDown();
                    break;

                //Phase 2
                case ActionState.Frost_Breath_V2:
                    AI_FrostBreath_V2();
                    break;
                case ActionState.Thunderstorm_V2:
                    AI_Thunderstorm_V2();
                    break;
                case ActionState.Laser_Blast_V2:
                    AI_LaserBlast_V2();
                    break;
                case ActionState.Star_Wrath_V2:
                    AI_StarWrath_V2();
                    break;
                case ActionState.Charge_V2:
                    AI_Charge_V2();
                    break;
            }

            UpdateOrientation();
        }

        private void AI_PhaseSwaps()
        {
            //Trigger Phase 2
            if (InPhase2 && !TriggeredPhase2)
            {
                AI_Phase2_Reset();
                TriggeredPhase2 = true;
                return;
            }

            if (InPhase3 && !TriggeredPhase3)
            {
                AI_Phase3_Reset();
                TriggeredPhase3 = true;
                return;
            }
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

        #region Phase 1
        private void AI_Spawn()
        {
            NPC.TargetClosest();
            Timer++;
            if (Timer == 1)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(EntitySource, NPC.Center, Vector2.Zero, ModContent.ProjectileType<NiiviSpawnExplosionProj>(),
                        0, 0, Main.myPlayer);
                }

                for (int i = 0; i < Crystals.Length; i++)
                {
                    Crystals[i].Draw = true;
                }
                NPC.velocity = -Vector2.UnitY;
            }

            OrientArching();
            UpdateOrientation();
            LookAtTarget();
            FlipToDirection();
            //Slowdown over time
            float length = 720;
            NPC.velocity = NPC.velocity.RotatedBy(MathHelper.TwoPi / length);

            if (Timer >= 60)
            {
                NextAttack = ActionState.Frost_Breath;
                ResetState(ActionState.Idle);

            }
        }

        private void AI_Idle()
        {
            NPC.TargetClosest();
            Timer++;
            if (Timer >= 1)
            {
                ResetState(ActionState.PrepareAttack);
            }

            UpdateOrientation();
            NPC.velocity *= 0.98f;
        }

        private void AI_CalmDown()
        {
            Timer++;
            if (Timer >= 60)
            {
                if (StellaMultiplayer.IsHost)
                {
                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NiiviRoaming>());
                }

                ResetShaders();
                NPC.active = false;
            }
          
            OrientArching();
            UpdateOrientation();
            NPC.velocity *= 0.98f;
        }

        private void AI_SwoopOut()
        {
            Timer++;
            if (Timer == 1)
            {
                OrientationSpeed = 0.03f;
                NPC.velocity = -Vector2.UnitY * 0.02f;
            }

            AI_MoveToward(Target.Center);
            //NPC.velocity *= 1.016f;
           // NPC.velocity.Y -= 0.002f;
            if (Timer >= 30)
            {
                ResetState(ActionState.Idle);
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
                if (Timer >= 360 || Vector2.Distance(NPC.Center, AttackPos) <= 8)
                {
                    DoAttack = true;
                }
            }

            if (DoAttack)
            {
                AttackTimer++;
                NPC.velocity *= 0.98f;
                if (AttackTimer >= 3)
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
                if(Timer == 1)
                {
                    ChargeCrystals = true;
                }

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
                
                if(Timer > 61)
                {
                    NPC.velocity *= 0.98f;
                }

                //Rotate head 90-ish degrees upward
                if (Timer < 60)
                {
              
                    LaserAttackPos = Target.Center;
                    Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
                    TargetHeadRotation = directionToTarget.ToRotation();

                    AI_MoveToward(Target.Center, 3);
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
                    ChargeCrystals = false;
                    //SHOOT LOL
                    Vector2 fireDirection = TargetHeadRotation.ToRotationVector2();
                    float distance = Vector2.Distance(NPC.Center, LaserAttackPos);

                    int type = ModContent.ProjectileType<NiiviLaserBlastProj>();
                    int damage = P1_LaserDamage;
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

                    for (int i = 0; i < 16; i++)
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

                if (AttackCount >= 3)
                {
                    ChargeCrystals = false;
                    NextAttack = ActionState.Star_Wrath;
                    ResetState(ActionState.Swoop_Out);
                }
            }
        }

        private void AI_FrostBreath()
        {
            ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
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


            if (AttackTimer == 0)
            {
                //Taking aim
                Timer++;
                ChargeCrystals = true;
                shaderSystem.VignetteScreen(-0.5f);
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
                shaderSystem.UnVignetteScreen();
                //Rotate head 90-ish degrees upward
                Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);

                float targetRotation = MathHelper.PiOver2 * -AttackSide;
                Vector2 rotatedDirection = directionToTarget.RotatedBy(targetRotation);
                TargetHeadRotation = rotatedDirection.ToRotation();

                //Slowly accelerate up while charging
                NPC.velocity *= 1.002f;

                Timer++;
                if (Timer == 1)
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
                if (Timer >= 60)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        int type = ModContent.ProjectileType<NiiviFrostCircleProj>();
                        int damage = 0;
                        int knockback = 0;
                        Projectile.NewProjectile(EntitySource, NPC.Center, Vector2.Zero,
                            type, damage, knockback, Main.myPlayer);

                        NPC.NewNPC(EntitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NiiviCrystalFrost>());
                    }

                    Timer = 0;
                    AttackTimer++;
                    NPC.velocity = -Vector2.UnitY;
                }
            }
            else if (AttackTimer == 2)
            {
                ChargeCrystals = false;

                //Get the shader system!

                shaderSystem.TintScreen(Color.Cyan, 0.1f);
                shaderSystem.DistortScreen(TextureRegistry.NormalNoise1, new Vector2(0.001f, 0.001f), blend: 0.025f);
                shaderSystem.VignetteScreen(-1f);

                //Slowdown over time
                float length = 720;
                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.TwoPi / length);

                //Re-orient incase target went behind
                LookDirection = DirectionToTarget;
                OrientArching();
                FlipToDirection();

                Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
                TargetHeadRotation = directionToTarget.ToRotation();
                Timer++;
                if (Timer >= length)
                {

                    Timer = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 3)
            {
                //Untint the screen
                ResetShaders();

                Timer++;
                if (Timer == 1)
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

                if (Timer >= 30)
                {
                    float speed = 24;
                    Vector2 velocity = TargetHeadRotation.ToRotationVector2() * speed;

                    //Add some random offset to the attack
                    velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 8);

                    int type = ModContent.ProjectileType<NiiviFrostBombProj>();

                    Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(128, 128);
                    velocity *= Main.rand.NextFloat(0.5f, 1f);

                    int damage = P1_FrostBreathDamage / 2;
                    float knockback = 1;

                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, spawnPos, velocity, type,
                        damage, knockback, Main.myPlayer);
                    }

                    Timer = 0;
                    AttackCount++;
                }

                if (AttackCount >= 6)
                {
                    Timer = 0;
                    AttackCount = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 4)
            {
                NextAttack = ActionState.Laser_Blast;
                ResetState(ActionState.Swoop_Out);
            }
        }

        private void AI_StarWrath()
        {
            ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            if (AttackTimer == 0)
            {
                Timer++;
                if (Timer == 1)
                {
                    shaderSystem.VignetteScreen(1);
                }
                ChargeCrystals = true;
                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();
                if (Timer >= 60)
                {
                    NPC.velocity = -Vector2.UnitY;
                    Timer = 0;
                    AttackTimer++;
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnCenter = Target.Center + new Vector2(0, -128);
                        NPC.NewNPC(EntitySource, (int)spawnCenter.X, (int)spawnCenter.Y, ModContent.NPCType<NiiviCrystalStars>());
                    }
                }
            }
            else if (AttackTimer == 1)
            {
                ChargeCrystals = false;
                Timer++;
                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();

                //Slowdown over time
                float length = 720;
                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.TwoPi / length);

                if (Timer >= 360)
                {
                    NextAttack = ActionState.Star_Storm;
                    ResetState(ActionState.Swoop_Out);
                }
            }
        }

        private void AI_StarStorm()
        {
            ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            NPC.rotation = 0;
            if(AttackTimer == 0)
            {
                Timer++;
                if(Timer == 1)
                {
                    shaderSystem.VignetteScreen(1);
                }

                OrientArching();
                UpdateOrientation();
                LookAtTarget();
                FlipToDirection();
                //Slowdown over time
                float length = 720;
                NPC.velocity = NPC.velocity.RotatedBy(MathHelper.TwoPi / length);

                ChargeCrystals = true;
                if(Timer >= 60)
                {
      
                    Timer = 0;
                    AttackTimer++;
                }
            }
            else if (AttackTimer == 1)
            {
                Timer++;
                if(Timer % 30 == 0)
                {
                    shaderSystem.UnVignetteScreen();
                    shaderSystem.FlashTintScreen(Color.White, 0.2f, 15);
                    NPC.velocity = -NPC.Center.DirectionTo(Target.Center) * 16;
                    if (StellaMultiplayer.IsHost)
                    {
                        float speed = Main.rand.NextFloat(24, 42);
                        Vector2 spawnCenter = NPC.Center + Main.rand.NextVector2Circular(128, 128);
                        Vector2 directionToTarget = NPC.Center.DirectionTo(Target.Center);
                        Vector2 velocityToTarget = directionToTarget * speed;
                        Projectile.NewProjectile(EntitySource, spawnCenter, velocityToTarget, 
                            ModContent.ProjectileType<NiiviStarBounceProj>(), P1_StarStormDamage, 1, Main.myPlayer);
                    }
                }
                else
                {
                    NPC.velocity *= 0.92f;
                }

                if(Timer >= 120)
                {
                    Timer = 0;
                    AttackTimer++;
                }
            }     
            else if (AttackTimer == 2)
            {
                ChargeCrystals = false;
                Timer++;
                if(Timer >= 60)
                {
                    NextAttack = ActionState.Thunderstorm;
                    ResetState(ActionState.Swoop_Out);
                }
            }     
        }

        private void AI_Thunderstorm()
        {
            ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            //Aight, this shouldn't be too hard to do
            //She flies up and rains down lightning
            if (AttackTimer == 0)
            {
                Timer++;
                ChargeCrystals = true;

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
                shaderSystem.TintScreen(Color.Black, 0.5f);
                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();

                NPC.velocity *= 1.05f;
                if (Timer >= 60)
                {
                    Timer = 0;
                    AttackTimer++;
                    if (StellaMultiplayer.IsHost)
                    {
                        Vector2 spawnCenter = Target.Center + new Vector2(0, -128);
                        NPC.NewNPC(EntitySource, (int)spawnCenter.X, (int)spawnCenter.Y, ModContent.NPCType<NiiviCrystalLightning>());
                    }

                }
            }
            else if (AttackTimer == 2)
            {
                ChargeCrystals = false;
                NPC.velocity *= 0.98f;
                Timer++;

                //Rotate Head
                TargetHeadRotation = NPC.Center.DirectionTo(Target.Center).ToRotation();

                if (Timer == 1)
                {
                    LaserAttackPos = Target.Center;
                }

                if (Timer >= 90)
                {
                    AttackCount++;
                    Timer = 0;
                }
                if (AttackCount >= 4)
                {
                    AttackTimer++;
                    Timer = 0;
                }
            }
            else if (AttackTimer == 3)
            {
                NPC.velocity *= 0.98f;
                Timer++;
                if (Timer >= 90)
                {
                    shaderSystem.UnTintScreen();
                    NextAttack = ActionState.Frost_Breath;
                    ResetState(ActionState.Swoop_Out);
                }
            }
        }

        private void AI_BabyDragons()
        {

        }
        #endregion

        #region Phase 2
        private void AI_Phase2_Reset()
        {
            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            screenShaderSystem.FlashTintScreen(Color.White, 0.3f, 5);
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.position);
            ResetShaders();
            ResetState(ActionState.Swoop_Out);
            NextAttack = ActionState.Laser_Blast_V2;
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
                    NPC.rotation = HeadRotation;
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(EntitySource, NPC.Center, fireDirection, type,
                        damage, knockback, Main.myPlayer, ai1: NPC.whoAmI);
                    }


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
                    NextAttack = ActionState.Laser_Blast_V2;
                    ResetState(ActionState.Swoop_Out);
                }
            }
        }

        private void AI_StarWrath_V2()
        {

        }

        private void AI_Thunderstorm_V2()
        {

        }
        #endregion

        #region Phase 3
        private void AI_Phase3_Reset()
        {
            ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            screenShaderSystem.FlashTintScreen(Color.White, 0.3f, 5);
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.position);
            ResetShaders();
            ResetState(ActionState.Swoop_Out);
            NextAttack = ActionState.Laser_Blast_V2;
        }
        #endregion

        public override void OnKill()
        {
            ResetShaders();
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedNiiviBoss, -1);
        }

        private void ResetShaders()
        {
            ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
            shaderSystem.UnTintScreen();
            shaderSystem.UnDistortScreen();
            shaderSystem.UnVignetteScreen();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<PureHeart>()));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<NiiviBossRel>()));
        }
    }
}
