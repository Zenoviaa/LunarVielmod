using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins.Projectiles;
using Stellamod.Content.Gores;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Core.VerletIntegration;
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

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins
{
    //The thing with this boss is that it's a dual synced boss
    //I think the easiest way to do that is to have a single twin npc, and a controller npc
    //That basically sends commands to them telling them what to do
    //In that case, let's create a base class
    //I'm also going to use partial classing here to see how I feel about organizing with it

    public partial class DescendingTwin : ModNPC,
        IDrawOutlines
    {
        public enum TwinAIState
        {
            SpawnSpazz,
            SpawnRetina,

            Idle,


            SimpleDashStart,
            SimpleDash,
            SimpleDashEnd,

            DashDanceStart,
            DashDancePrepare,
            DashDance,
            DashDanceTwirl,
            DashDanceEnd,


            FlameSwordStart,
            FlameSwordWindup,
            FlameSwordContinuous,
            FlameSwordEnd,

            HighSpeedCrashStart,
            HighSpeedCrashQuickStart,
            HighSpeedCrashPreDash,
            HighSpeedCrashWindup,
            HighSpeedCrashCrash,
            HIghSpeedCrashEnd,

            BouncingDashStartAnchor,
            BouncingDashStart,
            BouncingDashIn,
            BouncingDashOut,
            BouncingDashEnd,

            SpazzNodeLayWindup,
            SpazzNodeLayShoot,
            RetineNodeLayStart,
            RetinaNodeLayWindup,
            RetinaNodeLayShoot,
            NodeEnd,

            FlameTornadoStart,
            FlameTornadoWindup,
            FlameTornadoShoot,
            FlameTornadoEnd,

            PhaseShiftStart,
            PhaseShiftEnd,

            SpeedyDashStart,
            SpeedyDashWindup,
            SpeedyDashLoop,
            SpeedyDashEnd,

            ElectricBallStart,
            ElectricBallWindup,
            ElectricBallShoot,
            ElectricBallEnd,

            SuperCrashStart,
            SuperCrashWindup,
            SuperCrashCrash,
            SuperCrashEnd,

            SpiralLaserStart,
            SpiralLaserWindup,
            SpiralLaserLoop,
            SpiralLaserEnd,

            Despawn,
            Death
        }


        private enum TwinVariant
        {
            Spazz,
            Retina
        }

        private float _deathRotationOffset;
        private Vector2 _deathPositionOffset;

        private bool _phaseShift;
        private bool _contactDamage;
        private float _rotationTimer;
        private int _parentIndex;

        private Vector2 _teleportPosition;
        private ref float Timer => ref NPC.ai[0];
        private TwinAIState State
        {
            get => (TwinAIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private TwinAIState NextCommandState
        {
            get => (TwinAIState)NPC.ai[2];
            set => NPC.ai[2] = (float)value;
        }

        private ref float AttackNumber => ref NPC.ai[3];
        private TwinVariant Variant;
        private int GetVariant()
        {
            switch (Variant)
            {
                default: 
                case TwinVariant.Spazz:
                    if (_phaseShift)
                    {
                        return 3;
                    }
                    return 0;
                case TwinVariant.Retina:
                    if (_phaseShift)
                    {
                        return 2;
                    }
                    return 1;

            }
        }
        private int FlameSwordDamage => 20;
        private int DescendingBigBoomDamage => 30;

        private int DescendingFireDamage => 15;
        private int DescendingNodeLaserDamage => 15;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 64;
            NPC.height = 64;
            NPC.damage = 100;
            NPC.defense = 20;
            NPC.lifeMax = 18000;
            NPC.scale = 1f;
            NPC.aiStyle = -1;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Descender");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_simpleDashNormal);
            writer.WriteVector2(_highSpeedTargetPosition);
            writer.WriteVector2(_teleportPosition);
            writer.Write((float)Variant);
            writer.Write(_parentIndex);
            writer.Write(_phaseShift);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _simpleDashNormal = reader.ReadVector2();
            _highSpeedTargetPosition = reader.ReadVector2();
            _teleportPosition = reader.ReadVector2();
            Variant = (TwinVariant)reader.ReadSingle();
            _parentIndex = reader.ReadInt32();
            _phaseShift = reader.ReadBoolean();
        }

        private void SwitchState(TwinAIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        private void SetTargetToCommanderTarget()
        {
            NPC.target = Commander.NPC.target;
        }
        private void ReceiveTeleport()
        {
            if (_teleportPosition != Vector2.Zero)
            {
                NPC.position.X = _teleportPosition.X;
                NPC.position.Y = _teleportPosition.Y;
                _teleportPosition = Vector2.Zero;
            }
        }

        public override void AI()
        {
            base.AI();
            ReceiveTeleport();
            //If we don't have a valid target automatically retarget.
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
            }

            if (NPC.life <= 1 && State != TwinAIState.Death)
                SwitchState(TwinAIState.Death);

            if (_phaseShift)
            {
                //Idk cool ig
                if (Main.rand.NextBool(12))
                {
                    var zap = Particle.NewParticle<ZapParticle>(NPC.Center + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(1, 1), Color.White, 1f);
                    zap.innerColor = GetTwinColor();
                    zap.outerColor = Color.Lerp(zap.innerColor, Color.Black, 0.5f);
                    zap.fadeToColor = Color.Lerp(zap.outerColor, Color.Black, 0.5f);
            
                }
            }

            _contactDamage = false;
            switch (State)
            {
                case TwinAIState.SpawnSpazz:
                    AI_SpawnSpazz();
                    break;
                case TwinAIState.SpawnRetina:
                    AI_SpawnRetina();
                    break;

                case TwinAIState.Idle:
                    AI_Idle();
                    break;

                case TwinAIState.Death:
                    AI_Death();
                    break;

                case TwinAIState.Despawn:
                    AI_Despawn();
                    break;

                case TwinAIState.SimpleDashStart:
                    AI_SimpleDashStart();
                    break;
                case TwinAIState.SimpleDash:
                    AI_SimpleDash();
                    break;
                case TwinAIState.SimpleDashEnd:
                    AI_SimpleDashEnd();
                    break;

                case TwinAIState.DashDanceStart:
                    AI_DashDanceStart();
                    break;
                case TwinAIState.DashDancePrepare:
                    AI_DashDancePrepare();
                    break;
                case TwinAIState.DashDanceTwirl:
                    AI_DashDanceTwirl();
                    break;
                case TwinAIState.DashDance:
                    AI_DashDance();
                    break;
                case TwinAIState.DashDanceEnd:
                    AI_DashDanceEnd();
                    break;

                case TwinAIState.FlameSwordStart:
                    AI_FlameSwordStart();
                    break;
                case TwinAIState.FlameSwordWindup:
                    AI_FlameSwordAim();
                    break;
                case TwinAIState.FlameSwordContinuous:
                    AI_FlameSwordContinuous();
                    break;
                case TwinAIState.FlameSwordEnd:
                    AI_FlameSwordEnd();
                    break;

                case TwinAIState.HighSpeedCrashStart:
                    AI_HighSpeedCrashStart();
                    break;
                case TwinAIState.HighSpeedCrashQuickStart:
                    AI_HighSpeedCrashQuickStart();
                    break;
                case TwinAIState.HighSpeedCrashPreDash:
                    AI_HighSpeedCrashPreDash();
                    break;
                case TwinAIState.HighSpeedCrashWindup:
                    AI_HighSpeedCrashWindup();
                    break;
                case TwinAIState.HighSpeedCrashCrash:
                    AI_HighSpeedCrashCrash();
                    break;
                case TwinAIState.HIghSpeedCrashEnd:
                    AI_HighSpeedCrashEnd();
                    break;

                case TwinAIState.BouncingDashStart:
                    AI_BouncingDashStart();
                    break;
                case TwinAIState.BouncingDashStartAnchor:
                    AI_BouncingDashAnchor();
                    break;
                case TwinAIState.BouncingDashIn:
                    AI_BouncingDashIn();
                    break;
                case TwinAIState.BouncingDashOut:
                    AI_BouncingDashOut();
                    break;
                case TwinAIState.BouncingDashEnd:
                    AI_BouncingDashEnd();
                    break;

                case TwinAIState.SpazzNodeLayWindup:
                    AI_SpazzNodeLayWindup();
                    break;
                case TwinAIState.SpazzNodeLayShoot:
                    AI_SpazzNodeLayShoot();
                    break;
                case TwinAIState.RetineNodeLayStart:
                    AI_RetinaNodeLaySlowStart();
                    break;
                case TwinAIState.RetinaNodeLayWindup:
                    AI_RetinaNodeLayWindup();
                    break;
                case TwinAIState.RetinaNodeLayShoot:
                    AI_RetinaNodeLayShoot();
                    break;
                case TwinAIState.NodeEnd:
                    AI_NodeEnd();
                    break;

                case TwinAIState.FlameTornadoStart:
                    AI_FlameTornadoStart();
                    break;
                case TwinAIState.FlameTornadoWindup:
                    AI_FlameTornadoWindup();
                    break;
                case TwinAIState.FlameTornadoShoot:
                    AI_FlameTornadoShoot();
                    break;
                case TwinAIState.FlameTornadoEnd:
                    AI_FlameTornadoEnd();
                    break;

                case TwinAIState.PhaseShiftStart:
                    AI_PhaseShiftStart();
                    break;
                case TwinAIState.PhaseShiftEnd:
                    AI_PhaseShiftEnd();
                    break;

                case TwinAIState.SpeedyDashStart:
                    AI_SpeedyDashStart();
                    break;
                case TwinAIState.SpeedyDashWindup:
                    AI_SpeedyDashWindup();
                    break;
                case TwinAIState.SpeedyDashLoop:
                    AI_SpeedyDashLoop();
                    break;
                case TwinAIState.SpeedyDashEnd:
                    AI_SpeedyDashEnd();
                    break;

                case TwinAIState.ElectricBallStart:
                    AI_ElectricBallStart();
                    break;
                case TwinAIState.ElectricBallWindup:
                    AI_ElectricBallWindup();
                    break;
                case TwinAIState.ElectricBallShoot:
                    AI_ElectricBallShoot();
                    break;
                case TwinAIState.ElectricBallEnd:
                    AI_ElectricBallEnd();
                    break;


                case TwinAIState.SuperCrashStart:
                    AI_SuperCrashStart();
                    break;
                case TwinAIState.SuperCrashWindup:
                    AI_SuperCrashWindup();
                    break;
                case TwinAIState.SuperCrashCrash:
                    AI_SuperCrashCrash();
                    break;
                case TwinAIState.SuperCrashEnd:
                    AI_SuperCrashEnd();
                    break;

                case TwinAIState.SpiralLaserStart:
                    AI_SpiralLaserStart();
                    break;
                case TwinAIState.SpiralLaserWindup:
                    AI_SpiralLaserWindup();
                    break;
                case TwinAIState.SpiralLaserLoop:
                    AI_SpiralLaserLoop();
                    break;
                case TwinAIState.SpiralLaserEnd:
                    AI_SpiralLaserEnd();
                    break;

            }
            Lighting.AddLight(NPC.Center, Variant == TwinVariant.Spazz ? TorchID.Cursed : TorchID.Red);
            UpdateDraw();
        }

        private Player Target => Main.player[NPC.target];
        private Vector2 TargetNormal => NPC.DirectionTo(Target.Center);
        private DescendingTwins Commander => (DescendingTwins)Main.npc[_parentIndex].ModNPC;

        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
            if (NPC.life <= 0)
            {
                NPC.life = 1;
            }
        }
        private void AI_Despawn()
        {
            Timer++;
            NPC.velocity.X *= 0.9f;
            NPC.velocity.Y -= 0.2f;
            NPC.rotation = Utils.AngleLerp(NPC.rotation, TargetNormal.ToRotation(), 0.1f);
            if(Timer >= 100)
            {
                NPC.active = false;
            }
        }

        private void AI_Death()
        {
            Timer++;
            if (Timer == 1)
            {
      
            }

            float deathTime = 300f;
            if (Timer % 5 == 0)
            {
                SpawnSteamParticle();
            }

            if(Timer % 2 == 0)
            {
                _deathRotationOffset = Main.rand.NextFloat(-2f, 2f);
                _deathPositionOffset = Main.rand.NextVector2Circular(2, 2);
            }

            if (Timer % 12 == 0)
            {
                Vector2 spawnPoint = NPC.Top;
                spawnPoint.X += Main.rand.NextFloat(-64f, 64f);
                var fireDust = Dust.NewDustPerfect(spawnPoint, DustID.FireworkFountain_Red, Scale: Main.rand.NextFloat(0.5f, 1f));
                fireDust.noGravity = false;
            }

            NPC.velocity = Vector2.Zero;

            _telegraphLineAlpha = MathHelper.Lerp(_telegraphLineAlpha, 0f, 0.1f);
            _afterImageAlpha = MathHelper.Lerp(_afterImageAlpha, 0f, 0.1f);
            _contactDamage = false;

            if (Timer >= deathTime)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DescendingBigBoom>(),
                        DescendingBigBoomDamage, 1, Main.myPlayer, ai1: (int)Variant);
                }

                for (int i = 0; i < 16; i++)
                {
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<TSmokeDust>(),
                        (Vector2.One * Main.rand.Next(5, 15)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
                }
                for (float f = 0; f < 12; f++)
                {
                    Vector2 v = Main.rand.NextVector2Circular(128, 128);
                    FXUtil.GlowStretch(NPC.Center, v);
                }

                float numSteam = 32;
                for(float n = 0; n < numSteam; n++)
                {
                    Vector2 spawnPosition = NPC.Center;
                    spawnPosition.X += Main.rand.NextFloat(-64, 64);
                    spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                    Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);

                    float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                    var steamParticle = Particle.NewParticle<BlackSmokeParticle>(spawnPosition, spawnVelocity, Scale: spawnScale);
                    steamParticle.innerColor = Color.DarkGray;
                    steamParticle.outerColor = Color.Black;
                    steamParticle.fadeToColor = Color.Black;
                }
                int[] gores = AutoGoreLoader.FindGores("MechanicalEye");
                foreach (int g in gores)
                {
                    Gore.NewGore(NPC.GetSource_FromThis(),
                        NPC.Center,
                        Main.rand.NextVector2Circular(8, 8).RotatedByRandom(MathHelper.ToRadians(20)), g, 1f);
                }

                var screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                screenShaderSystem.TintScreen(Color.White, 0.5f, 30f);

                float numDust = 32;
                for (float n = 0; n < numDust; n++)
                {
                    Vector2 dustVelocity = Main.rand.NextVector2Circular(32, 32);
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), dustVelocity,
                        newColor: Color.Red,
                        Scale: Main.rand.NextFloat(0.5f, 1.5f));
                }
                SoundStyle explosionSound = new SoundStyle("Stellamod/Assets/Sounds/GlocketRouncher");
                explosionSound.Pitch = -0.5f;
                SoundEngine.PlaySound(explosionSound, NPC.position);
                FXUtil.ShakeCamera(NPC.position, 1024, 8);
                var boom = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.Yellow, Color.Red);
                boom.Scale *= 3f;
                ShakeModSystem.Shake = 16;
                var p = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.Red, Color.Black);
                NPC.Kill();
            }
        }

        private void AI_SpawnRetina()
        {
            Variant = TwinVariant.Retina;
            _parentIndex = (int)NPC.ai[2];
            NPC.ai[2] = (float)TwinAIState.Idle;
            SwitchState(TwinAIState.Idle);
        }

        private void AI_SpawnSpazz()
        {
            Variant = TwinVariant.Spazz;
            _parentIndex = (int)NPC.ai[2];
            NPC.ai[2] = (float)TwinAIState.Idle;
            SwitchState(TwinAIState.Idle);
        }

        private void IdleMovement()
        {

            //So we should slowly move towards the player if they're far, if not we'll just hover in place.
            //Step 1. Look towards the player, we can do this by calculating a target normal, calculating an angle and then lerping to it
            Vector2 targetNormal = TargetNormal;
            float targetAngle = targetNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);

            //Step 2. Check the distance between this current twin and the player
            //If the distance is too far we'll move closer to them, if not we just slow down/sit there
            float distanceToTarget = Vector2.Distance(NPC.Center, Target.Center);
            float maxDistance = 400;
            if (distanceToTarget > maxDistance)
            {
                //We should scale the movement velocity based on the distance, so the farther they are the faster we'll move
                Vector2 movementVelocity = targetNormal * distanceToTarget / 32f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, movementVelocity, 0.05f);
            }
            else
            {
                //Otherwise, we'll just slow down
                //We want to keep a little bit of movement velocity so it's not just completely static
                NPC.velocity *= 0.8f;

                //Stpe 3. Add a little bit of hovering velocity for a cool effect
                float yHover = MathF.Sin(Timer * 0.1f) * 0.5f;
                NPC.velocity.Y += yHover;
            }
        }

        private void AI_Idle()
        {
            _rotationTimer = 0f;

            //Ok, so in the idle state, the goober is basically waiting on a command from the commander
            //So it should just slowly wander around and target the player
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }


            //Reset draw variables
            _scale = Vector2.One;
            _afterImageAlpha = 0f;
            IdleMovement();

            //Remember, we're just waiting on a command from up above, so we don't actually need to do anything else here
            //However, we will create a few steam particles just for funsies
            if (Timer % 10 == 0)
            {
                Particle.NewParticle<BlackSmokeParticle>(
                    NPC.Center + Main.rand.NextVector2Circular(64, 64),
                    -Vector2.UnitY * Main.rand.NextFloat(0.2f, 0.5f), newColor: Color.White);
            }

            TargetOutlineColor = Color.Transparent;
            AttackNumber = 0f;

            //Receive the next command state.
            //This should be automatically netcoded btw
            if (NextCommandState != TwinAIState.Idle)
            {
                SwitchState(NextCommandState);
                NextCommandState = TwinAIState.Idle;
            }
        }

    
    }
}
