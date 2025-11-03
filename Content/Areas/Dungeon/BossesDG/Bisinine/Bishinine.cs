using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;

using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine
{

    public class Bishinine : ScarletBoss,
         IDrawOutlines
    {

        /*
        * A bell drops to the ground in front of her and she hits it at you, running behind you and hitting back at you, increasing speed 


        Throws the hammerscythe at the ceiling and a bunch of bells fall and bounce on the ground 


        She points her finger up and a humongous growing bell appears and she throws it at you 
        as it bounces from wall to wall as she is balancing on it like lenny from mario bros (second phase attack)


        She runs over to you and does a jump and spike attack making a bunch of ghastly spikes poke from the ground
        (grimm poking into the ground attack basically)

        Holds her scythe and crosses the ground with fast dashes.


        Jumps backwards and charges in the air before she shoots a shotgun of magic missiles 


        She jumps in the air and floats as a bunch of a comets fall onto the ground with like an electric like impact, 
        this happens for a decent bit (second phase), Signature attack 


        Second phase she just becomes faster

        */

        private bool _afterImage;
        private float _afterImageTime;
        private float _starTrailTime;
        private bool _fall;
        private bool _contactDamage;
        private int _bellHitNPCIndex;

        private float _squishTimer;
        private Vector2 _startSquishScale;
        private Vector2 _squishScale;

        private Color _outlineColor;
        private Color TargetOutlineColor;
        private PatternManager<AIState> _patternManager;
        private ref float Timer => ref NPC.ai[0];

        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private ref float AttackTimer => ref NPC.ai[2];
        private ref float AttackNumber => ref NPC.ai[3];
        private enum AIState
        {
            Spawn,
            Idle,

            BellDrop_Start,
            BellDrop_RunToBell,
            BellDrop_Hit,
            BellDrop_End,

            BellFall_Start,
            BellFall_ThrowScythe,

            BellRoll_Start,
            BellRoll_Bounce,
            BellRoll_End,

            GrimmSpikes_RunToPlayer,
            GrimmSpikes_Jump,
            GrimmSpikes_Crash,

            ScytheDash_Startup,
            ScytheDash_Dash,
            ScytheDash_End,

            MagicMissle_Startup,
            MagicMissle_Barrage,
            MagicMissle_End,

            CometJump_Startup,
            CometJump_Float,
            CometJump_End,

            Phase2Transition,
            Despawn,
            Death
        }
        private bool InPhase2
        {
            get => NPC.life <= NPC.lifeMax / 2f;
        }

        private int RisingScytheDamage => 40;
        private int GrimmSpikesDamage => 60;
        private int MagicMissileDamage => 60;
        private int CometDamage => 90;
        private int BellBalancingBounceDamage => 100;

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_bellHitNPCIndex);
            writer.Write(_fall);
            writer.Write(_contactDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _bellHitNPCIndex = reader.ReadInt32();
            _fall = reader.ReadBoolean();
            _contactDamage = reader.ReadBoolean();
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _squishScale = Vector2.One;
            NPC.width = 64;
            NPC.height = 64;
            NPC.damage = 60;
            NPC.defense = 2;
            NPC.lifeMax = 10000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Bomb");
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(gold: 1);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.takenDamageMultiplier = 0.9f;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Bishinine");
        }

        public override bool? CanFallThroughPlatforms()
        {
            return _fall;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
        }

        #region Squishing
        private void LandingSquish()
        {
            _squishTimer = 0;
            _startSquishScale = new Vector2(1.34f, 0.75f);
            _squishScale = new Vector2(1.34f, 0.75f);
        }

        private void UnSquish()
        {
            const float time = 30f;
            _squishTimer++;
            float completionRatio = _squishTimer / time;
            float ease = EasingFunction.InOutSine(completionRatio);
            _squishScale = Vector2.Lerp(_startSquishScale, Vector2.One, ease);
        }
        #endregion

        public override void AI()
        {
            base.AI();


            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                if (!NPC.HasValidTarget)
                {
                    SwitchState(AIState.Despawn);
                }
            }
            NPC.spriteDirection = NPC.direction;
            if(NPC.collideY && NPC.velocity.Y > 1)
            {
                LandingSquish();
            }
            UnSquish();
            switch (State)
            {
                case AIState.Despawn:
                    AI_Despawn();
                    break;
                case AIState.Spawn:
                    AI_Spawn();
                    break;
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Death:
                    AI_Death();
                    break;


                case AIState.BellDrop_Start:
                    AI_BellDropStart();
                    break;
                case AIState.BellDrop_RunToBell:
                    AI_BellDropRunToBell();
                    break;
                case AIState.BellDrop_Hit:
                    AI_BellDropHit();
                    break;
                case AIState.BellDrop_End:
                    AI_BellDropEnd();
                    break;



                case AIState.BellFall_Start:
                    AI_ThrowScytheStartup();
                    break;
                case AIState.BellFall_ThrowScythe:
                    AI_ThrowScythe();
                    break;



                case AIState.GrimmSpikes_RunToPlayer:
                    AI_GrimChasePlayer();
                    break;
                case AIState.GrimmSpikes_Jump:
                    AI_GrimSpikesJump();
                    break;
                case AIState.GrimmSpikes_Crash:
                    AI_GrimSpikesCrash();
                    break;

                case AIState.ScytheDash_Startup:
                    AI_ScytheDashStartup();
                    break;
                case AIState.ScytheDash_Dash:
                    AI_ScytheDashDash();
                    break;
                case AIState.ScytheDash_End:
                    AI_ScytheDashEnd();
                    break;

                case AIState.CometJump_Startup:
                    AI_CometJumpStartup();
                    break;
                case AIState.CometJump_Float:
                    AI_CometJumpFloat();
                    break;
                case AIState.CometJump_End:
                    AI_CometJumpEnd();
                    break;

                case AIState.MagicMissle_Startup:
                    AI_MagicMissileStartup();
                    break;
                case AIState.MagicMissle_Barrage:
                    AI_MagicMissileBarrage();
                    break;
                case AIState.MagicMissle_End:
                    AI_MagicMissileEnd();
                    break;

                case AIState.BellRoll_Start:
                    AI_BellRollStart();
                    break;
                case AIState.BellRoll_Bounce:
                    AI_BellRollBounce();
                    break;
                case AIState.BellRoll_End:
                    AI_BellRollEnd();
                    break;
            }
        }

        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }



        #region Bell Roll
        private void AI_BellRollStart()
        {
            /*
             *         
             * 
             *  She points her finger up and a humongous growing bell appears and she throws it at you 
                as it bounces from wall to wall as she is balancing on it like lenny from mario bros (second phase attack)

             */
            TargetOutlineColor = Color.Yellow;
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
            }
            NPC.velocity.X *= 0.94f;
            NPC.rotation = NPC.velocity.X * 0.05f;
            if (Timer >= 30)
            {
                SwitchState(AIState.BellRoll_Bounce);
            }
        }

        private void AI_BellRollBounce()
        {
            Timer++;
            if (Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, -Vector2.UnitY * 4,
                        ModContent.ProjectileType<BellBalance>(), BellBalancingBounceDamage, 1, Main.myPlayer, ai2: NPC.whoAmI);
                }
            }


            //The projectile will control her movement here.
            //It'll lerp her position, hence why it needs a reference to her existence
            if (Timer >= 360)
            {
                SwitchState(AIState.BellRoll_End);
            }
        }

        private void AI_BellRollEnd()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.velocity.Y = -5;
            }
            if (NPC.collideY || Timer >= 60)
            {
                SwitchState(AIState.Idle);
            }
        }
        #endregion



        #region Magic Missile

        private void AI_MagicMissileStartup()
        {
            //  Jumps backwards and charges in the air before she shoots a shotgun of magic missiles 
            TargetOutlineColor = Color.Yellow;
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
                Vector2 jumpVelocity = new Vector2();
                jumpVelocity.Y = -10;
                jumpVelocity.X = -FacingDirectionToTarget * 15;
                NPC.velocity = jumpVelocity;
            }

            if (Timer >= 10f)
            {
                NPC.velocity.X *= 0.94f;
            }
            NPC.rotation = NPC.velocity.X * 0.05f;
            if (Timer >= 30)
            {
                SwitchState(AIState.MagicMissle_Barrage);
            }
        }

        private void AI_MagicMissileBarrage()
        {
            TargetOutlineColor = Color.Red;
            Timer++;
            if(Timer == 1)
            {

              
            }
            if (Timer % 5 == 0)
            {
                var p = Particle.NewParticle<GlowDonutParticle>(NPC.Center, Vector2.UnitY, newColor: Color.White);
                var p2 = Particle.NewParticle<GlowDonutParticle>(NPC.Center, Vector2.UnitY * 5, newColor: Color.White);
                p2.Scale *= 0.5f;
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, -Vector2.UnitY * Main.rand.NextFloat(1f, 5f),
                        ModContent.ProjectileType<BisinineMissile>(), MagicMissileDamage, 1, Main.myPlayer);
                }
            }
      
            if (Timer < 35)
            {
                _afterImageTime = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 10f));
                NPC.velocity.X = MathHelper.Lerp(0f, NPC.direction * 25, EasingFunction.InOutSine(Timer / 10f));
                NPC.velocity.Y = 0;
           
            }
            NPC.rotation = NPC.velocity.X * 0.05f;
            if (Timer >= 35)
            {
                SwitchState(AIState.MagicMissle_End);
            }
        }

        private void AI_MagicMissileEnd()
        {
            _afterImageTime *= 0.9f;
            TargetOutlineColor = Color.Transparent;
            Timer++;
            NPC.velocity.X *= 0.94f;
            NPC.rotation *= 0.94f;
            if (Timer >= 30)
            {
                SwitchState(AIState.Idle);
            }
        }
        #endregion



        #region Signature Comet Fall
        private void AI_CometJumpStartup()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                NPC.direction = TargetDirection;

            }

            if (Timer == 15)
            {
                NPC.velocity.Y = -14;
                float maxRads = MathHelper.ToRadians(45);
                var part = Particle.NewParticle<GlowDonutParticle>(NPC.Center, Vector2.UnitY);
                for(float f = 0; f < 8; f++)
                {
                    Vector2 vel = -Vector2.UnitY * 4;
                    vel = vel.RotatedByRandom(maxRads);
                    vel *= Main.rand.NextFloat(0.1f, 5);
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowSparkleDust>(), vel, newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                }
            }
            if (Timer >= 15)
            {
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction, 0.1f);
            }

            NPC.rotation = NPC.velocity.X * 0.05f;
            if (Timer >= 25)
            {
                SwitchState(AIState.CometJump_Float);
            }
        }

        private void AI_CometJumpFloat()
        {
            OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -252);
            TargetOutlineColor = Color.Yellow;
            Timer++;
            NPC.velocity.X *= 0.99f;
            if(Timer >= 15 && Timer <= 25)
            {
               // NPC.velocity.Y *= 0.95f;
            }

            if(Timer % 20 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowSparkleDust>(), newColor: Color.White, Scale: Main.rand.NextFloat(0f, 0.5f));
            }
            _afterImageTime = MathHelper.Lerp(0f, 0.5f, EasingFunction.InOutSine(Timer / 30f));
            NPC.direction = TargetDirection;

            if(Timer >= 64)
            {
                NPC.velocity.X += MathF.Sin(Timer * 0.1f) * 0.2f;
                NPC.velocity.Y = MathF.Cos(Timer * 0.2f) * 0.4f;
                float xDistance = MathF.Abs(MyTarget.Center.X - NPC.Center.X);
                if(xDistance > 64)
                {
                    NPC.velocity.X += TargetDirection * 0.1f;
                }
                NPC.noGravity = true;
            }
   
            NPC.rotation = NPC.velocity.X * 0.05f;
            if (Timer % 5 == 0)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 spawnPos = NPC.Center;
                    spawnPos.Y -= 1000;
                    spawnPos.X += Main.rand.NextFloat(-1000, 1000);
                    Projectile.NewProjectile(SourceFromThis, spawnPos, Vector2.UnitY * 4, ModContent.ProjectileType<BishinineComet>(), CometDamage, 1, Main.myPlayer);
                }
                AttackNumber++;
            }
            if (AttackNumber >= 100)
            {
                SwitchState(AIState.CometJump_End);
            }
        }
        private void AI_CometJumpEnd()
        {
            _afterImageTime *= 0.9f;
            Timer++;
            NPC.noGravity = false;
            NPC.velocity.X *= 0.9f;
            NPC.rotation = NPC.velocity.X * 0.05f;
            if (NPC.collideY || Timer >= 30)
            {
                LandingSquish();
                SwitchState(AIState.Idle);
            }
        }


        #endregion



        #region Scythe Dash

        private void AI_ScytheDashStartup()
        {
            _starTrailTime *= 0.8f;
            _afterImageTime *= 0.8f;
            TargetOutlineColor = Color.Yellow;
            //   Holds her scythe and crosses the ground with fast dashes.
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
            }

            if(Timer == 10)
            {
                NPC.velocity.X = -NPC.direction * 8;
                NPC.velocity.Y = -4;
            }


            NPC.velocity.X *= 0.94f;
            if (Timer >= 30 && NPC.collideY)
            {
                SwitchState(AIState.ScytheDash_Dash);
            }
            NPC.rotation = NPC.velocity.X * 0.015f;
    
        }

        private void AI_ScytheDashDash()
        {
            _contactDamage = true;
            _afterImageTime = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 5f));
            _starTrailTime = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 5f));
            TargetOutlineColor = Color.Red;
            Timer++;
            if (Timer == 1)
            {
                AttackNumber++;
                NPC.direction = TargetDirection;
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, NPC.Center - (NPC.direction * Vector2.UnitX * 40) - Vector2.UnitY * 320, Vector2.UnitY, 
                        ModContent.ProjectileType<DashLightning>(), NPC.damage, 1, Main.myPlayer);
                }
            }

            if (NPC.collideX)
            {
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
            }
            if(Timer % 5 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowSparkleDust>(), newColor: Color.White, Scale: Main.rand.NextFloat(0.2f, 1f));
            }
            if (Timer % 1 == 0)
            {
                var spark = Particle.NewParticle<SparkParticle>(NPC.Center + Main.rand.NextVector2Circular(64, 64), Vector2.Zero);
                spark.outerColor = Color.Blue;
                spark.fadeToColor = Color.Black;
            }
            if (Timer % 1 == 0)
            {
                Dust.NewDustPerfect(NPC.Bottom, ModContent.DustType<GlowSparkleDust>(), newColor: Color.White, Scale: 0.5f, Velocity: Vector2.Zero);
            }

            if (Main.rand.NextBool(4))
            {
                var p = Particle.NewBlackParticle<BlackSmokeParticle>(NPC.Bottom, Vector2.Zero, Color.DarkGray);
                p.Scale *= 0.25f;
                p.color *= 0.5f;
                p.fadeToColor = Color.Black;
                p.innerColor = Color.DarkGray;
                p.outerColor = Color.Black;
            }
            if (Timer >= 10)
            {
                NPC.velocity.X *= 0.9f;
            } else
            {
                NPC.velocity.X = MathHelper.Lerp(0, 80 * NPC.direction, EasingFunction.InOutSine(Timer / 10f));

            }
            NPC.rotation = NPC.velocity.X * 0.005f;
            if (Timer >= 10)
            {
                SwitchState(AIState.ScytheDash_End);
            }
        }

        private void AI_ScytheDashEnd()
        {
            _starTrailTime *= 0.8f;
            _afterImageTime *= 0.8f;
            TargetOutlineColor = Color.Transparent;
            Timer++;
            NPC.velocity.X *= 0.9f;
            NPC.rotation = NPC.velocity.X * 0.005f;
            
            if(AttackNumber >= 4)
            {
                if(Timer >= 30)
                {
                    SwitchState(AIState.Idle);
                }
            }
            else
            {

                if (Timer >= 5)
                {
                    SwitchState(AIState.ScytheDash_Startup);
                }
    
            }
        }
        #endregion



        #region Grim Poking Attack

        private void AI_GrimChasePlayer()
        {
            TargetOutlineColor = Color.Yellow;
            /*
             *     She runs over to you and does a jump and spike attack making a bunch of ghastly spikes poke from the ground
        (grimm poking into the ground attack basically)/
            */
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                
            }
            NPC.direction = TargetDirection;

            float side = AttackTimer % 2 == 0 ? 1 : -1;
            Vector2 targetCenter = MyTarget.Center;
            targetCenter.X += side * 32;

            float xDistance = MathF.Abs(targetCenter.X - NPC.Center.X);
            float yDistance = MathF.Abs(targetCenter.Y - NPC.Center.Y);
            float maxRunSpeed = 12;
            float accel = 1;
            if (NPC.collideX)
            {
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
            }

            if (xDistance > 90)
            {
                //Zoom zoom, we gotta run up to the bell
                if (NPC.Center.X < MyTarget.Center.X)
                {
                    if (NPC.velocity.X < maxRunSpeed)
                    {
                        NPC.velocity.X += accel;
                    }
                }
                else if (NPC.Center.X > MyTarget.Center.X)
                {
                    if (NPC.velocity.X > -maxRunSpeed)
                    {
                        NPC.velocity.X -= accel;
                    }
                }
            }
            else if (NPC.collideY)
            {
                SwitchState(AIState.GrimmSpikes_Jump);
            }
        }

        private void AI_GrimSpikesJump()
        {
            TargetOutlineColor = Color.Yellow;
            Timer++;
            if (Timer == 1)
            {
                NPC.velocity.Y = -17;
            }

            if (Timer >= 15)
            {
                if(Timer <= 45)
                {
                    NPC.velocity.Y *= 0.95f;
                    NPC.rotation = NPC.velocity.X * 0.05f;
                }
                else
                {
                    _afterImageTime = MathHelper.Lerp(_afterImageTime, 1f, 0.1f);
                    NPC.rotation = -NPC.velocity.X * 0.05f;
                    NPC.velocity.X += NPC.direction * 0.1f;
                    NPC.velocity.Y *= 1.01f;
                }

            }


            NPC.velocity.X *= 0.94f;

            if (Timer >= 10 && NPC.collideY)
            {
                SwitchState(AIState.GrimmSpikes_Crash);
            }
        }

        private void AI_GrimSpikesCrash()
        {
            _afterImageTime *= 0.94f;
            TargetOutlineColor = Color.Red;
            Timer++;
            NPC.velocity.X = 0;
            NPC.rotation = 0;
            if (Timer == 1)
            {
                SoundStyle bellHitSound = AssetRegistry.Sounds.Bishinine.BellHit1;
                bellHitSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(bellHitSound, NPC.position);
                MyPlayer myPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();
                myPlayer.ShakeAtPosition(NPC.Center, 1024f, 30f);
                ShakeModSystem.Shake = 2;
                SoundStyle boom = SoundID.DD2_ExplosiveTrapExplode;
                boom.PitchVariance = 0.3f;
                SoundEngine.PlaySound(boom, NPC.position);
                for (int i = 0; i < 16; i++)
                {
                    float radius = 150;
                    Vector2 offset = Vector2.UnitX * Main.rand.Next(-1, 1);
                    offset *= Main.rand.NextFloat(1f, radius);
                    offset += new Vector2(radius / 2, 0);

                    Vector2 velocity = Vector2.UnitX * Main.rand.Next(-1, 1);
                    velocity *= Main.rand.NextFloat(1f, 2f);
                    var p = Particle.NewBlackParticle<BlackSmokeParticle>(NPC.Bottom + offset, velocity, Color.DarkGray);
                    p.Scale *= 0.25f;
                    p.color *= 0.5f;
                    p.fadeToColor = Color.Black;
                    p.innerColor = Color.DarkGray;
                    p.outerColor = Color.Black;
                }

                FXUtil.GlowCircleBoom(NPC.Bottom,
                   innerColor: Color.White,
                   glowColor: Color.Black,
                   outerGlowColor: Color.Black, duration: 25, baseSize: 0.24f);
                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(240);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(NPC.Bottom,
                        innerColor: Color.White,
                        glowColor: Color.Black,
                        outerGlowColor: Color.Black,
                        baseSize: 0.24f);
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }

                for (int i = 0; i < 7; i++)
                {
                    Vector2 velocity = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(15f, 35f);
                    var particle = FXUtil.GlowStretch(NPC.Bottom, velocity);
                    particle.InnerColor = Color.White;
                    particle.GlowColor = Color.LightCyan;
                    particle.OuterGlowColor = Color.Black;
                    particle.Duration = Main.rand.NextFloat(25, 50);
                    particle.BaseSize = Main.rand.NextFloat(0.045f, 0.09f);
                    particle.VectorScale *= 0.5f;
                }

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Vinger"), NPC.position);

                FXUtil.ShakeCamera(NPC.position, 1024, 16);
                FXUtil.PunchCamera(NPC.position, Vector2.UnitY, 8, 8, 8);
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, NPC.Bottom, Vector2.Zero,
                        ModContent.ProjectileType<BellSpikeSummon>(), GrimmSpikesDamage, 1, Main.myPlayer);
                }
            }

            if (Timer >= 60)
            {
                SwitchState(AIState.Idle);
            }
        }
        #endregion



        #region Throw Scythe

        private void AI_ThrowScytheStartup()
        {
            TargetOutlineColor = Color.Yellow;
            NPC.velocity.X *= 0.94f;
            NPC.rotation = 0;
            //Throws the hammerscythe at the ceiling and a bunch of bells fall and bounce on the ground
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }
            if (Timer >= 30)
            {
                SwitchState(AIState.BellFall_ThrowScythe);
            }

        }

        private void AI_ThrowScythe()
        {
            TargetOutlineColor = Color.Red;
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 velocity = -Vector2.UnitY * 8;
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity,
                        ModContent.ProjectileType<RisingScythe>(), RisingScytheDamage, 1, Main.myPlayer);
                }
            }

            NPC.velocity.X *= 0.94f;
            NPC.rotation = 0;
            if (Timer >= 120)
            {
                SwitchState(AIState.Idle);
            }
        }
        #endregion



        #region Bell Drop Attack

        private void AI_BellDropStart()
        {
            Timer++;
            if(Timer == 1)
            {
                SoundStyle fallingBell = AssetRegistry.Sounds.Bishinine.FallingBell;
                fallingBell.PitchVariance = 0.2f;
                SoundEngine.PlaySound(fallingBell, NPC.position);
            }

            TargetOutlineColor = Color.Yellow;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                NPC.direction = TargetDirection;
                if (MultiplayerHelper.IsHost)
                {

                    Vector2 spawnPosition = NPC.Center;
                    spawnPosition.X += NPC.direction * 80;
                    spawnPosition.Y += -700;
                    _bellHitNPCIndex = NPC.NewNPC(SourceFromThis, (int)spawnPosition.X, (int)spawnPosition.Y,
                        ModContent.NPCType<BellBaseball>());
                    NPC.netUpdate = true;
                }
                NPC.velocity.Y = -8;
            }

            //Just sit here really
            NPC.velocity.X *= 0.9f;
            NPC.rotation = NPC.velocity.X * 0.03f;

            NPC bellToHit = Main.npc[_bellHitNPCIndex];
            if (Timer >= 60 && bellToHit.ai[3] > 0)
            {
                SwitchState(AIState.BellDrop_RunToBell);
            }
            else if (Timer >= 300)
            {
                //Failsafe
                //Just go back to idle state if this attack don't work some reason
                SwitchState(AIState.Idle);
            }
        }

        private void AI_BellDropRunToBell()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                AttackTimer++;
            }

            TargetOutlineColor = Color.Yellow;

            NPC bellToHit = Main.npc[_bellHitNPCIndex];
            NPC.direction = bellToHit.Center.X > NPC.Center.X ? 1 : -1;
      
            float side = MyTarget.Center.X < bellToHit.Center.X ? 1 : -1;
            Vector2 targetCenter = bellToHit.Center;
            targetCenter.X += side * 32;

            float xDistance = MathF.Abs(targetCenter.X - NPC.Center.X);
            float yDistance = MathF.Abs(targetCenter.Y - NPC.Center.Y);
            float maxRunSpeed = 25;
            float accel = 1;
            if (NPC.collideX)
            {
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
            }

            if (xDistance > 16)
            {
                float a = MathHelper.Lerp(0f, accel, EasingFunction.InOutSine(Timer / 15f));
                if(Timer % 2 == 0 && Timer >= 20)
                {
                    var p = Particle.NewBlackParticle<BlackSmokeParticle>(NPC.Bottom, Vector2.Zero, Color.DarkGray);
                    p.Scale *= 0.25f;
                    p.color *= 0.5f;
                    p.fadeToColor = Color.Black;
                    p.innerColor = Color.DarkGray;
                    p.outerColor = Color.Black;
                }
                _afterImageTime = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 15f));
                //Zoom zoom, we gotta run up to the bell
                if (NPC.Center.X < bellToHit.Center.X)
                {
                    if (NPC.velocity.X < maxRunSpeed)
                    {
                        NPC.velocity.X += a;
                    }
                }
                else if (NPC.Center.X > bellToHit.Center.X)
                {
                    if (NPC.velocity.X > -maxRunSpeed)
                    {
                        NPC.velocity.X -= a;
                    }
                }
            }
            else if (yDistance > 48)
            {
                //We met the x distance requirement so we have to slow down or we'll overshoot
                NPC.velocity.X *= 0.9f;
                if (NPC.collideY && NPC.Bottom.Y < bellToHit.Bottom.Y)
                {
                    NPC.velocity.Y = -10;
                    _fall = false;
                }
                else if (NPC.Bottom.Y > bellToHit.Bottom.Y - 32)
                {
                    _fall = true;
                }
            }
            else if (NPC.collideY)
            {
                SwitchState(AIState.BellDrop_Hit);
            }
        }

        private void AI_BellDropHit()
        {
            Timer++;
            _afterImageTime *= 0.95f;
            TargetOutlineColor = Color.Red;
            NPC.velocity.X *= 0.7f;
            NPC.velocity.Y = 0;


            NPC bellToHit = Main.npc[_bellHitNPCIndex];
            NPC.direction = bellToHit.Center.X > NPC.Center.X ? 1 : -1;

            if(Timer == 20)
            {
                NPC.velocity.X = NPC.direction * 4;
                NPC.rotation = NPC.direction * 0.2f;
            }
            if (Timer == 30)
            {
                NPC.velocity.X = -NPC.direction * 8;
                NPC.rotation = -NPC.direction * 0.2f;
                Particle.NewParticle<GlowDonutParticle>(bellToHit.Center, -NPC.direction * Vector2.UnitX);
                var p = Particle.NewParticle<GlowDonutParticle>(bellToHit.Center, -NPC.direction * Vector2.UnitX * 5);
                p.Scale *= 0.5f;
                for (float f = 0; f < 8; f++)
                {
                    Vector2 vel = Vector2.UnitX * NPC.direction;
                    vel *= Main.rand.NextFloat(1f, 15f);
                    vel = vel.RotatedByRandom(MathHelper.ToRadians(45));
                    Vector2 position = bellToHit.Center;
                    position += Main.rand.NextVector2Circular(32, 32);
                    Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), vel, newColor: Color.White, Scale: Main.rand.NextFloat(0.2f, 2f));

                    if (Main.rand.NextBool(4))
                    {
                        FXUtil.GlowStretch(position, vel);
                    }
                }


                SoundStyle bellHit = AssetRegistry.Sounds.Bishinine.BellHit1;
                bellHit.PitchVariance = 0.2f;
                SoundEngine.PlaySound(bellHit, NPC.position);
                FXUtil.ShakeCamera(NPC.position, 1024, 8);
                FXUtil.PunchCamera(NPC.position, Vector2.UnitX * NPC.direction, 8, 8, 8);
                AttackNumber++;
                if (MultiplayerHelper.IsHost)
                {
                    float hitDirection = (MyTarget.Center - NPC.Center).ToRotation();
                    bellToHit.ai[1] = hitDirection;
                    bellToHit.netUpdate = true;
                }
            } else if (Timer >= 30)
            {
                NPC.rotation *= 0.9f;
            }

            if (Timer >= 60)
            {
                if (AttackNumber >= 6)
                {
                    SwitchState(AIState.BellDrop_End);
                }
                else
                {
                    SwitchState(AIState.BellDrop_RunToBell);
                }
            }
        }

        private void AI_BellDropEnd()
        {
            NPC bellToHit = Main.npc[_bellHitNPCIndex];
            bellToHit.ai[2] = 1;
            Timer++;
            NPC.velocity.X *= 0.94f;
            if (Timer >= 30)
            {
                SwitchState(AIState.Idle);
            }
        }

        #endregion



        #region Idle and Spawning

        private void AI_Spawn()
        {
            TargetOutlineColor = Color.Transparent;
            Timer++;
            NPC.velocity.X *= 0.9f;
            if (Timer >= 120)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_Idle()
        {
            //Set some default vars here
            _contactDamage = false;
            TargetOutlineColor = Color.Transparent;
            Timer++;
            AttackTimer = 0;
            AttackNumber = 0;
            NPC.velocity.X *= 0.9f;
            NPC.rotation = NPC.velocity.X * 0.2f;
            NPC.noGravity = false;
            float timeToWait = 30;
            if (InPhase2)
                timeToWait /= 2;
            if (Timer >= timeToWait)
            {
                ChooseAttack();
            }
        }

        private void AI_Despawn()
        {
            TargetOutlineColor = Color.Transparent;
            Timer++;
            float interpolant = Timer / 60f;
            float ease = EasingFunction.InOutSine(interpolant);
            NPC.scale = MathHelper.Lerp(NPC.scale, 0f, ease);
            if (Timer >= 60f)
            {
                NPC.active = false;
            }
        }

        private void AI_Death()
        {
            TargetOutlineColor = Color.Transparent;
            Timer++;
            if (Timer >= 240)
            {
                NPC.Kill();
            }
        }


        private void ChooseAttack()
        {
            if (!MultiplayerHelper.IsHost)
                return;
            if (_patternManager == null)
            {
                if (InPhase2)
                {
                    _patternManager = new PatternManager<AIState>(
                       new Tuple<AIState, float>(AIState.BellDrop_Start, 1.0f),
                       new Tuple<AIState, float>(AIState.BellFall_Start, 1.0f),
                       new Tuple<AIState, float>(AIState.GrimmSpikes_RunToPlayer, 1.0f),
                       new Tuple<AIState, float>(AIState.ScytheDash_Startup, 1.0f),
                       new Tuple<AIState, float>(AIState.MagicMissle_Startup, 1.0f),
                       new Tuple<AIState, float>(AIState.CometJump_Startup, 0.2f),
                       new Tuple<AIState, float>(AIState.BellRoll_Start, 1.0f));
                }
                else
                {
                    _patternManager = new PatternManager<AIState>(
                        new Tuple<AIState, float>(AIState.BellDrop_Start, 1.0f),
                        new Tuple<AIState, float>(AIState.BellFall_Start, 1.0f),
                        new Tuple<AIState, float>(AIState.GrimmSpikes_RunToPlayer, 1.0f),
                        new Tuple<AIState, float>(AIState.ScytheDash_Startup, 1.0f),
                        new Tuple<AIState, float>(AIState.MagicMissle_Startup, 1.0f));
                }
            }

            AIState state = _patternManager.NextPattern();
            SwitchState(state);
            SwitchState(AIState.ScytheDash_Startup);
        }
        #endregion


        #region Draw Code
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            string texturePath = Texture;
            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            float drawRotation = NPC.rotation;
            Vector2 drawScale = _squishScale * NPC.scale;
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;
                float f = i;
                float interpolant = f / (float)NPC.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.White, Color.Transparent, interpolant) * 0.25f;
                fadeColor *= _afterImageTime;
                oldDrawPos += NPC.Size / 2f;
                spriteBatch.Draw(texture, oldDrawPos, NPC.frame, fadeColor, NPC.oldRot[i], drawOrigin, drawScale, spriteEffects, 0f);
            }


            Texture2D starTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Vector2 sdrawOrigin = starTexture.Size() / 2f;
            Color cometColor = Color.GhostWhite;
            cometColor.A = 0;
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;
                float f = i;
                float interpolant = f / (float)NPC.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.White, Color.Blue, interpolant) * 0.25f;
                fadeColor *= (1.0f - interpolant);
                fadeColor.A = 0;
                oldDrawPos += NPC.Size / 2f;
                spriteBatch.Draw(starTexture, oldDrawPos, null, fadeColor * _starTrailTime, NPC.oldRot[i], sdrawOrigin, NPC.scale * 1.5f, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            return false;
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            string texturePath = Texture;
            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
            Vector2 drawPos = NPC.Center - Main.screenPosition;
            Vector2 drawOrigin = NPC.frame.Size() / 2f;
            float drawRotation = NPC.rotation;
            Vector2 drawScale = _squishScale * NPC.scale;
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


            float outlineOffset = 2;
            Vector2 left = drawPos + Vector2.UnitX * -outlineOffset;
            Vector2 right = drawPos + Vector2.UnitX * outlineOffset;
            Vector2 up = drawPos + Vector2.UnitY * -outlineOffset;
            Vector2 down = drawPos + Vector2.UnitY * outlineOffset;
            Color outlineColor = _outlineColor;

            spriteBatch.Draw(texture, left, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            spriteBatch.Draw(texture, right, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            spriteBatch.Draw(texture, up, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
            spriteBatch.Draw(texture, down, NPC.frame, outlineColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
        }
        #endregion
    }
}
