using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity.Projectiles;
using Stellamod.Content.Gores;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

/*

- Deer with a singularity for a head, in its spawn animation at first it looks like a normal deer before the head explodes and parts start orbiting it, ooo I know exactly how to code this

- The legs and everything are rigged, we’ll use forward kinematics to animate the boss, so we’ll have to make a run animation and idle animation

- Opens the fight with several exploding blood magic projectiles that loosely track the player

- Winds up a charge and then runs directly at the player really fast, and explodes into bloody bits before merging itself back together elsewhere

- Runs up into the sky and rains down acidic blood

- Walks slowly around the player as bloody boils explode from its body and then home back towards you

- Cracks form in its body and it violently erupts into multiple bloody geysers

- Winds up a charge and then keeps running at you while swerving around and trying to juke you out
 
- In phase 2 every attack gets more deadlier, triggers at under 50% health
 */
namespace Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity
{

    public class SanguineSingularity : ScarletBoss,
        IDrawOutlines
    {
        public struct SanguineDraw
        {
            public Color outlineColor;
            public Vector2 scale;
            public Vector2 shake;
            public Vector2 singularityScale;
            public float alpha;
            public float flashAlpha;
            public float afterImageAlpha;
            public bool headless;


            public void SetDefaults()
            {
                outlineColor = Color.Transparent;
                scale = Vector2.One;
                shake = Vector2.Zero;
                singularityScale = Vector2.Zero;
                alpha = 1f;
                flashAlpha = 0f;
                afterImageAlpha = 0f;
                headless = false;
            }
        }
        private enum AnimationState
        {
            Idle,
            Walk,
            Run
        }

        private enum AIState
        {
            Spawn,
            Idle,
            Despawn,
            Death,

            BloodyBurst_Start,
            BloodyBurst,
            BloodyBurst_End,

            BloodyCharge_Start,
            BloodyCharge_Rush,
            BloodyCharge_End,

            BloodRain_Start,
            BloodRain_Rain,
            BloodRain_End,

            BloodBoil_Start,
            BloodBoil_Walk,
            BloodBoil_End,

            BloodCrack_Start,
            BloodCrack_Geyser,
            BloodCrack_End,

            GhastlyBloodDash_Start,
            GhastlyBloodDash_Run,
            GhastlyBloodDash_End,

            Phase2Transition
        }

        private float _bloodyBurstTimer;
        private float _incresionDiskFrameBottom;
        private float _incresionDiskFrameTop;
        private bool _contactDamage;
   
        private Vector2 _runDirection;

        private SanguineDraw _draw;
        private SanguineGoreManager _goreManagerBacking;
        private SanguineGoreManager GoreManager
        {
            get
            {
                if(_goreManagerBacking == null)
                {
                    _goreManagerBacking = new SanguineGoreManager(ModContent.Request<Texture2D>(Texture + "_HeadGore", AssetRequestMode.ImmediateLoad).Value, 5);
                }
                return _goreManagerBacking;
            }
        }
        private Vector2 _teleportPosition;

        private Color TargetOutlineColor;
        private ref float Timer => ref NPC.ai[0];

        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        private ref float AttackNumber => ref NPC.ai[2];
        private ref float Timer2 => ref NPC.ai[3];
        private int BloodyBurstDamage => 26;
        private int BloodyChargeBoomDamage => 30;
        private int BloodRainDamage => 18;
        private int BloodGeyserDamage => 26;

        private AnimationState _animation;
        private PatternManager<AIState> _patternManagerBacking;
        private PatternManager<AIState> PatternManager
        {
            get
            {
                if (_patternManagerBacking == null)
                {
                    _patternManagerBacking = new PatternManager<AIState>(
                        new Tuple<AIState, float>(AIState.BloodyBurst_Start, 1f),
                        new Tuple<AIState, float>(AIState.BloodyCharge_Start, 1f),
                        new Tuple<AIState, float>(AIState.BloodRain_Start, 1f),
                        new Tuple<AIState, float>(AIState.BloodBoil_Start, 1f),
                        new Tuple<AIState, float>(AIState.BloodCrack_Start, 1f),
                        new Tuple<AIState, float>(AIState.GhastlyBloodDash_Start, 1f));
                }
                return _patternManagerBacking;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_teleportPosition);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _teleportPosition = reader.ReadVector2();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 28;
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
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

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/SanguineSingularity");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                State = state;
                Timer = 0;
                Timer2 = 0;
                NPC.netUpdate = true;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
        }

        public override void AI()
        {
            base.AI();

            DrawHelper.UpdateFrame(ref _incresionDiskFrameBottom, 0.8f, 1, 40);
            DrawHelper.UpdateFrame(ref _incresionDiskFrameTop, 0.8f, 1, 76);
            _draw.outlineColor = Color.Lerp(_draw.outlineColor, TargetOutlineColor, 0.1f);
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                if (!NPC.HasValidTarget && State != AIState.Despawn)
                {
                    SwitchState(AIState.Despawn);
                }
            }

            if (_teleportPosition != Vector2.Zero)
            {
                NPC.position.X = _teleportPosition.X;
                NPC.position.Y = _teleportPosition.Y;
                NPC.velocity = Vector2.Zero;
                _teleportPosition = Vector2.Zero;
            }
            _draw.flashAlpha *= 0.99f;
            if (_draw.headless)
            {
                GoreManager.orbitingRadius = 60f;
                GoreManager.draw = true;
            }
            else
            {
                GoreManager.orbitingRadius = 0f;
                if (GoreManager.HasTinyOrbit())
                {
                    GoreManager.draw = false;
                }
            }
                GoreManager.Update(NPC.Center);
            if (_draw.headless)
            {
                _draw.singularityScale = Vector2.Lerp(_draw.singularityScale, Vector2.One, 0.1f);
            }
            else
            {
                _draw.singularityScale = Vector2.Lerp(_draw.singularityScale, Vector2.Zero, 0.1f);

            }
            if (Timer % 5 == 0)
            {
                Vector2 upVelocity = -Vector2.UnitY;
                upVelocity *= 5;
                upVelocity = upVelocity.RotateRandom(0.5f);
                var d = Dust.NewDustPerfect(NPC.Center, DustID.Blood, upVelocity, Scale: Main.rand.NextFloat(1f, 2f));
                d.noGravity = false;
            }
            ModContent.GetInstance<SanguineBloodRenderManager>().DrawBloodyBG = true;
                NPC.spriteDirection = NPC.direction;
            switch (State)
            {
                case AIState.Spawn:
                    AI_Spawn();
                    break;
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Despawn:
                    AI_Despawn();
                    break;
                case AIState.Death:
                    AI_Death();
                    break;


                case AIState.BloodyBurst_Start:
                    AI_BloodyBurst_Start();
                    break;
                case AIState.BloodyBurst:
                    AI_BloodyBurst();
                    break;
                case AIState.BloodyBurst_End:
                    AI_BloodyBurst_End();
                    break;


                case AIState.BloodyCharge_Start:
                    AI_BloodyCharge_Start();
                    break;
                case AIState.BloodyCharge_Rush:
                    AI_BloodyCharge_Rush();
                    break;
                case AIState.BloodyCharge_End:
                    AI_BloodyCharge_End();
                    break;


                case AIState.BloodRain_Start:
                    AI_BloodRain_Start();
                    break;
                case AIState.BloodRain_Rain:
                    AI_BloodRain_Rain();
                    break;
                case AIState.BloodRain_End:
                    AI_BloodRain_End();
                    break;


                case AIState.BloodBoil_Start:
                    AI_BloodBoil_Start();
                    break;
                case AIState.BloodBoil_Walk:
                    AI_BloodBoil_Walk();
                    break;
                case AIState.BloodBoil_End:
                    AI_BloodBoil_End();
                    break;


                case AIState.BloodCrack_Start:
                    AI_BloodCrack_Start();
                    break;
                case AIState.BloodCrack_Geyser:
                    AI_BloodCrack_Geyser();
                    break;
                case AIState.BloodCrack_End:
                    AI_BloodCrack_End();
                    break;


                case AIState.GhastlyBloodDash_Start:
                    AI_GhastlyBloodDash_Start();
                    break;
                case AIState.GhastlyBloodDash_Run:
                    AI_GhastlyBloodDash_Run();
                    break;
                case AIState.GhastlyBloodDash_End:
                    AI_GhastlyBloodDash_End();
                    break;

                case AIState.Phase2Transition:
                    AI_Phase2Transition();
                    break;
            }
        }


        private void AI_Spawn()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
     
            }

            _draw.scale = Vector2.One;
            _draw.alpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 60f));
            _draw.afterImageAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 60f));

            NPC.rotation = 0;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            if (Timer == 1)
            {
                SoundStyle spawnSound = AssetRegistry.Sounds.SanguineSingularity.SanguinePreBurst;
                spawnSound.PitchVariance = 0.1f;
                SoundEngine.PlaySound(spawnSound, NPC.position);
            }


            NPC.velocity.Y = MathHelper.Lerp(-1f, 1f, MathF.Sin(Timer * 0.02f) * 0.5f + 0.5f);
            RetargetCameraModifier.ReTargetPosition = NPC.Center;
            if (Timer == 200)
            {
                _draw.headless = true;
                if (MultiplayerHelper.IsHost)
                {
                    float numProjectiles = 4;
                    for(float f = 0f; f < numProjectiles; f++)
                    {
                        Vector2 velocity = -Vector2.UnitY;
                        velocity = velocity.RotatedByRandom(0.5f);
                        velocity *= Main.rand.NextFloat(5f, 12f);
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity, 
                            ModContent.ProjectileType<BloodyBurst>(), BloodyBurstDamage, 1, Main.myPlayer);
                    }

                }

                CreateGoreBurst(NPC.Center, -Vector2.UnitY * 8);
                float numDust = 24;
                for(float f = 0; f < numDust; f++)
                {
                    Vector2 dustVelocity = Main.rand.NextVector2Circular(20, 20);
                    var d = Dust.NewDustPerfect(NPC.Center, DustID.Blood, dustVelocity, Scale: Main.rand.NextFloat(0.5f, 1.2f));
                    d.noGravity = false;
                }

                ShowNamePlate();
                CreateRedFlash();
                ShakeModSystem.Shake = 12;
                FXUtil.ShakeCamera(NPC.Center, 1024, 8);
                SoundStyle spawnSound = AssetRegistry.Sounds.SanguineSingularity.SanguineCry;
                SoundEngine.PlaySound(spawnSound, NPC.position);
            }

            if (Timer >= 200 && Timer < 300)
            {
                NPC.velocity *= 0.9f;
                _animation = AnimationState.Idle;
                ShakeModSystem.Shake = 8;
                if (Timer % 9 == 0)
                {
                    Particle.NewParticle<ShockParticle>(NPC.Center, Vector2.Zero, Color.White);
                }
            }
            else if (Timer < 200)
            {
                _animation = AnimationState.Walk;
                Vector2 targetVelocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.1f);
                NPC.direction = TargetDirection;
            }

            if (Timer >= 400)
            {
                SwitchState(AIState.Idle);
            }
        }


        private void AI_Despawn()
        {
            Timer++;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.velocity.X *= 0.5f;
            NPC.velocity.Y -= 0.5f;
            TargetOutlineColor = Color.Transparent;
            if (Timer >= 100)
            {
                NPC.active = false;
            }
        }

        private void ChooseAttack()
        {
            SwitchState(PatternManager.NextPattern());
            SwitchState(AIState.BloodyBurst_Start);
        }

        private void AI_Idle()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }


            _contactDamage = false;
            _draw.scale = Vector2.One;
            _draw.alpha = MathHelper.Lerp(_draw.alpha, 1f, 0.1f);
            _draw.afterImageAlpha *= 0.5f;
            AttackNumber = 0;
            NPC.direction = TargetDirection;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            if (Timer > 60)
            {
                _animation = AnimationState.Walk;
                Vector2 directionToTarget = (MyTarget.Center - NPC.Center);
                directionToTarget = directionToTarget.SafeNormalize(Vector2.Zero);

                float distanceToTarget = Vector2.Distance(NPC.Center, MyTarget.Center);
                float speedModifier = MathHelper.Lerp(6f, 12f, EasingFunction.InOutSine(distanceToTarget / 64f));
                directionToTarget *= speedModifier;
                NPC.velocity = Vector2.Lerp(NPC.velocity, directionToTarget, 0.1f);
                NPC.rotation *= 0.5f;
            }
            else
            {
                _animation = AnimationState.Idle;
                NPC.velocity *= 0.9f;
            }

            TargetOutlineColor = Color.Transparent;
            if (Timer >= 120f)
            {
                ChooseAttack();
            }
        }


        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
            if (NPC.life <= 0 && State != AIState.Death)
            {
                NPC.life = 1;
                SwitchState(AIState.Death);
            }


            if (NPC.life <= 0)
            {
                NPC.life = 1;
            }
        }

        private void AI_Death()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            _animation = AnimationState.Walk;
            _draw.alpha = MathHelper.Lerp(_draw.alpha, 1f, 0.1f);
            _draw.afterImageAlpha *= 0.9f;

            //I think for this I want to slow down the movement and make it violently shake before it explodes into bloody geysers
            //Yes this can kill you (lol)
            //ok, so first slow down the velocity I think?
            NPC.velocity.X *= 0.99f;
            NPC.velocity.Y *= 0.99f;


            //Slowly hover upwards I think that'd be cool
            if (NPC.velocity.Y < -1f)
                NPC.velocity.Y -= 0.1f;
            NPC.rotation = NPC.velocity.X * 0.025f;
            RetargetCameraModifier.ReTargetPosition = NPC.Center;
            TargetOutlineColor = Color.Yellow;


            //Shake the goober around quite a bit
            //Maybe emit some dusts?
            if (Timer % 5 == 0)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                Color color = Color.Red;
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), newColor: color,
                    Velocity: velocity,
                    Scale: Main.rand.NextFloat(0.5f, 1f)); ;
            }

            float deathTime = 200;
            float completionRatio = Timer / deathTime;
            float shakeAmount = MathHelper.Lerp(0f, 16, completionRatio);
            Vector2 shake = Main.rand.NextVector2Circular(shakeAmount, shakeAmount);
            _draw.shake = shake;
            if (Timer == deathTime)
            {
                if (MultiplayerHelper.IsHost)
                {
                    for (float n = 0; n < 8; n++)
                    {
                        Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, vel, ModContent.ProjectileType<BloodGeyser>(), BloodGeyserDamage, 1, Main.myPlayer);
                    }
                }

                var p = FXUtil.GlowCircleBoom(NPC.Center, Color.Red, Color.DarkRed, Color.Black);
                var p2 = FXUtil.GlowCircleBoom(NPC.Center, Color.Red, Color.DarkRed, Color.Black);
                p.Scale *= 3;
                p2.Scale *= 2;

                for (float f = 0; f < 16; f++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), vel,
                        newColor: Color.Red,
                        Scale: Main.rand.NextFloat(0.5f, 3f));
                }

                SoundStyle deathSound = AssetRegistry.Sounds.SanguineSingularity.SanguineDeath;
                SoundEngine.PlaySound(deathSound, NPC.position);
                FXUtil.ShakeCamera(NPC.Center, 1024, 8);
                ShakeModSystem.Shake = 9;
                NPC.Kill();
            }
        }

        public override void OnKill()
        {
            base.OnKill();
            DownedBossTracker.ClearFlag(DownedBossFlag.SanguineSingularity);
        }

        #region Bloody Burst
        private void CreateGoreBurst(Vector2 position, Vector2 velocity)
        {
            int[] gores = AutoGoreLoader.FindGores("BloodChunk");
            foreach (int g in gores)
            {
                Gore.NewGore(NPC.GetSource_FromThis(),
                    position,
                    velocity.RotatedByRandom(MathHelper.ToRadians(20)), g, 1f);
            }

            for (float f = 0; f < 16; f++)
            {
                Vector2 vel = velocity.RotatedByRandom(MathHelper.ToRadians(30));
                vel *= Main.rand.NextFloat(0f, 1f);
                var d = Dust.NewDustPerfect(position, DustID.Blood, vel, newColor: Color.White);
                d.noGravity = false;
            }
        }

        private void CreateRedFlash()
        {
            _draw.flashAlpha = 1f;
        }

        private void SlightlyMoveCameraTowardsMe()
        {

            Vector2 cameraOffset = (NPC.Center - Main.LocalPlayer.Center);
            cameraOffset.Y -= 1000;
            cameraOffset *= 0.25f;
            OffsetCameraModifier.FocusTargetOffset = cameraOffset;
        }

        private void SetBloodyBurstVelocity()
        {
            _bloodyBurstTimer += MathHelper.Lerp(0.5f, 1f, AttackNumber / 6f);
            float radians = _bloodyBurstTimer * 0.1f;
            Vector2 ovalOffset = new Vector2();
            ovalOffset.X = MathF.Cos(radians) * 512;
            ovalOffset.Y = MathF.Sin(radians) * 128;
            Vector2 targetCenter = MyTarget.Center + ovalOffset;
            Vector2 velocity = (targetCenter - NPC.Center);
            velocity *= MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(_bloodyBurstTimer / 60f));
            NPC.velocity = velocity;
            NPC.direction = NPC.velocity.X > 0 ? 1 : -1;
        }
        private void AI_BloodyBurst_Start()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();

                if(AttackNumber == 3)
                {
                    SoundStyle telegraphSound = AssetRegistry.Sounds.SanguineSingularity.SanguineCry2;
                    telegraphSound.PitchVariance = 0.2f;
                    SoundEngine.PlaySound(telegraphSound, NPC.position);
                }
             
            }

            //- Opens the fight with several exploding blood magic projectiles
            //- That loosely track the player


            if(Timer == 1)
            {
                Vector2 velocityToPlayer = (MyTarget.Center - NPC.Center);
                velocityToPlayer = velocityToPlayer.SafeNormalize(Vector2.Zero);
                _runDirection = velocityToPlayer;
                NPC.direction = TargetDirection;
            }

            float time = MathHelper.Lerp(40, 20f, AttackNumber / 6f);
            float lerp = Timer / time;
            float ease = EasingFunction.Anticipation(lerp);


            SlightlyMoveCameraTowardsMe();
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            SetBloodyBurstVelocity();
            _contactDamage = false;
         
            _draw.headless = true;
            _draw.scale = Vector2.Lerp(new Vector2(1.2f, 1.2f), Vector2.One, ease);
            if (Timer < time / 2f && AttackNumber == 0)
            {
                _draw.afterImageAlpha = MathHelper.Lerp(_draw.afterImageAlpha, 0f, 0.1f);
                _animation = AnimationState.Walk;
            }
            else
            {
                _draw.afterImageAlpha = MathHelper.Lerp(_draw.afterImageAlpha, 1f, 0.1f);
                _animation = AnimationState.Run;
            }


            if (Timer == (int)(time))
            {
                CreateRedFlash();
                SoundStyle sanguineBurstReady = AssetRegistry.Sounds.SanguineSingularity.SanguineBurstReady;
                sanguineBurstReady.PitchVariance = 0.3f;
                sanguineBurstReady.Volume = 0.1f;
                SoundEngine.PlaySound(sanguineBurstReady, NPC.position);
                FXUtil.ShakeCamera(NPC.Center, 1024, 8);
                ShakeModSystem.Shake = 2;
            }

            TargetOutlineColor = Color.Yellow;
            if (Timer >= time + 10f)
            {
                SwitchState(AIState.BloodyBurst);
            }
        }


        private void AI_BloodyBurst()
        {
            Timer++;
            _animation = AnimationState.Run;
            _contactDamage = true;
            TargetOutlineColor = Color.Red;

            NPC.noTileCollide = true;
            NPC.noGravity = true;
            SetBloodyBurstVelocity();
            SlightlyMoveCameraTowardsMe();
            int time = (int)MathHelper.Lerp(30f, 15f, AttackNumber / 6f);
            if (Timer >= time)
            {
                _draw.scale = new Vector2(1.3f, 1.5f);
                if (MultiplayerHelper.IsHost)
                {
                    float num = 2f;
                    for(float f = 0; f < num; f++)
                    {
                        float completionRatio = f / num;
                        Vector2 upVelocity = -Vector2.UnitX * NPC.direction * 17;
                        upVelocity = upVelocity.RotatedBy(MathHelper.ToRadians(35 * NPC.direction * completionRatio));
                        upVelocity = upVelocity.RotatedBy(MathHelper.ToRadians(65 * NPC.direction));
                        int bloodyBurstProjectile = ModContent.ProjectileType<BloodyBurst>();
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, upVelocity, bloodyBurstProjectile, BloodyBurstDamage, 1, Main.myPlayer);
                    }

                }

                for (float f = 0f; f < 16; f++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(32, 32);
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.Red, Scale: Main.rand.NextFloat(0.5f, 1f));
                }

                SoundStyle burstSound = AssetRegistry.Sounds.SanguineSingularity.SanguineBurst;
                burstSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(burstSound, NPC.position);

                var screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                screenShaderSystem.TintScreen(Color.Red * 0.15f, 0.5f, 30f);
                ShakeModSystem.Shake = 8;
                FXUtil.ShakeCamera(NPC.Center, 1024, 8);
                for (float f = 0f; f < 4f; f++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(3, 3);
                    var boom = FXUtil.GlowCircleBoom(NPC.Center, Color.Red, Color.DarkRed, Color.Black);
                    boom.Scale *= Main.rand.NextFloat(1.5f, 5f);
                    boom.Velocity = velocity;

                    var stretch = FXUtil.GlowStretch(NPC.Center, velocity * 32);
                    stretch.color = Color.Red;
                    stretch.OuterGlowColor = Color.Red;
                }
            }


            if (Timer >= time)
            {
                SwitchState(AIState.BloodyBurst_End);
            }
        }

        private void AI_BloodyBurst_End()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }
            _draw.scale = Vector2.Lerp(_draw.scale, Vector2.One, 0.1f);
            _draw.afterImageAlpha *= 0.9f;
            _animation = AnimationState.Run;
            TargetOutlineColor = Color.Transparent;
            SlightlyMoveCameraTowardsMe();
            SetBloodyBurstVelocity();
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            if (Timer == 30)
            {
                AttackNumber++;
                if (AttackNumber >= 21)
                {
                    SwitchState(AIState.Idle);
                }
                else
                {
                    SwitchState(AIState.BloodyBurst_Start);
                }
            } else if (AttackNumber >= 20)
            {
                NPC.velocity *= 0.9f;

            }
        }

        #endregion


        #region Bloody Charge
        private void AI_BloodyCharge_Start()
        {
            // -Winds up a charge and then runs directly at the player really fast,
            // and explodes into bloody bits before merging itself back together elsewhere

            Timer++;
            if (Timer == 1)
            {
           
                CreateRedFlash();
                NPC.TargetClosest();

                Vector2 directionToPlayer = (MyTarget.Center - NPC.Center);
                directionToPlayer = directionToPlayer.SafeNormalize(Vector2.Zero);
                _runDirection = directionToPlayer;
                SoundStyle preDash = AssetRegistry.Sounds.SanguineSingularity.SanguinePreBurst;
                SoundEngine.PlaySound(preDash, NPC.position);
                NPC.direction = TargetDirection;
            }

            _draw.alpha = MathHelper.Lerp(_draw.alpha, 1f, 0.1f);
            _draw.afterImageAlpha *= 0.5f;
            _animation = AnimationState.Run;


 
            float chargeUpTime = 40f;
            float ease = EasingFunction.Anticipation2(Timer / chargeUpTime);
            Vector2 velocity = Vector2.Lerp(-_runDirection, _runDirection * 35f, ease);

            TargetOutlineColor = Color.Yellow;
            NPC.velocity = velocity;
            NPC.rotation = NPC.velocity.X * 0.025f;
            if (Timer >= chargeUpTime)
            {
                SwitchState(AIState.BloodyCharge_Rush);
            }
        }

        private void AI_BloodyCharge_Rush()
        {
            Timer++;
            if (Timer == 1)
            {
                CreateRedFlash();
                SoundStyle chargeSound = AssetRegistry.Sounds.SanguineSingularity.SanguineCharge;
                SoundEngine.PlaySound(chargeSound, NPC.position);
            }

            _draw.alpha = MathHelper.Lerp(_draw.alpha, 1f, 0.1f);
            _draw.afterImageAlpha = MathHelper.Lerp(0f, 1f, 0.5f);
            _animation = AnimationState.Run;
      
            TargetOutlineColor = Color.Red;
            NPC.rotation = NPC.velocity.X * 0.025f;
            if (Timer == 30)
            {
                _draw.headless = true;
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<BloodyChargeBoom>(), BloodyChargeBoomDamage, 1, Main.myPlayer);
                    float num = 4f;
                    for(float f = 0f;f < num; f++)
                    {
                        float completionRatio = f / num;
                        Vector2 upVelocity = -Vector2.UnitY * NPC.direction * 8;
                        upVelocity = upVelocity.RotatedBy(MathHelper.ToRadians(35 * NPC.direction * completionRatio));
                        upVelocity = upVelocity.RotatedBy(MathHelper.ToRadians(65 * NPC.direction));
                        int bloodyBurstProjectile = ModContent.ProjectileType<BloodyBurst>();
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, upVelocity, bloodyBurstProjectile, BloodyBurstDamage, 1, Main.myPlayer);
                    }
                }
            }

            if (Timer >= 60f)
            {
                SwitchState(AIState.BloodyCharge_End);
            }
        }

        private void AI_BloodyCharge_End()
        {
            _draw.alpha = MathHelper.Lerp(_draw.alpha, 1f, 0.1f);
            _draw.afterImageAlpha *= 0.9f;
            _animation = AnimationState.Walk;

            Timer++;
            if (Timer % 5 == 0)
            {
                Color color = Color.White;
                Vector2 velocity = Main.rand.NextVector2Circular(24, 24);
                Particle.NewBlackParticle<BloodSparkleParticle>(NPC.Center, velocity, color, Scale: Main.rand.NextFloat(0.5f, 3f));
            }

            TargetOutlineColor = Color.Transparent;
            NPC.velocity *= 0.5f;
            NPC.rotation = NPC.velocity.X * 0.025f;
            if (Timer >= 60f)
            {
                SwitchState(AIState.Idle);
            }
        }

        #endregion


        #region Blood Rain
        private void AI_BloodRain_Start()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                var screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                screenShaderSystem.TintScreen(Color.Red * 0.35f, 0.5f, 60f);
            }

            float chargeUpTime = 60f;
            float ease = EasingFunction.Anticipation(Timer / chargeUpTime);
            float siningEase = EasingFunction.InOutSine(Timer / chargeUpTime);

            _animation = AnimationState.Walk;
            _draw.alpha = MathHelper.Lerp(1f, 0f, siningEase);
            _draw.afterImageAlpha = MathHelper.Lerp(0f, 1f, siningEase);

            NPC.noGravity = true;
            NPC.noTileCollide = true;

            float yVelocity = MathHelper.Lerp(2f, -4f, ease);
            NPC.velocity.Y = yVelocity;
            NPC.velocity.X *= 0.9f;
            NPC.rotation = NPC.velocity.X * 0.025f;
            if (Timer >= chargeUpTime)
            {
                SwitchState(AIState.BloodRain_Rain);
            }
        }

        private void AI_BloodRain_Rain()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                SoundStyle laughSound = AssetRegistry.Sounds.SanguineSingularity.SanguineLaugh;
                laughSound.PitchVariance = 0.15f;
                SoundEngine.PlaySound(laughSound, NPC.position);
            }
            OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -300);

            if(AttackNumber >= 7)
            {
                Timer2++;
                if(Timer2 == 1)
                {
                    if (MultiplayerHelper.IsHost)
                    {
                        float direction = AttackNumber % 2 == 0 ? -1 : 1;
                        Vector2 targetCenter = MyTarget.Center;
                        targetCenter.Y -= 300;
                        _teleportPosition = targetCenter - new Vector2(1100 * direction, 0);
                        _runDirection = (targetCenter - _teleportPosition);
                        _runDirection = _runDirection.SafeNormalize(Vector2.Zero);
         
                        NPC.netUpdate = true;
                    }
            
                }
                _animation = AnimationState.Run;
                NPC.direction = _runDirection.X > 0 ? 1 : -1; 
                _draw.alpha = MathHelper.Lerp(_draw.alpha, 1f, 0.2f);
                _draw.afterImageAlpha = MathHelper.Lerp(_draw.afterImageAlpha, 1f, 0.2f);
        
           
                _contactDamage = true;
                TargetOutlineColor = Color.Red;

                float time = 30f;
                float lerp = Timer2 / time;
                float ease = EasingFunction.Anticipation(lerp);
                _draw.scale = Vector2.Lerp(Vector2.One * 0.5f, Vector2.One, ease);
                NPC.velocity = Vector2.Lerp(-_runDirection, _runDirection * 13, ease);
                NPC.rotation = NPC.velocity.X * 0.0125f;
                if(Timer2 == 30)
                {
                    CreateRedFlash();
                    _draw.headless = false;
                    SoundStyle laughSound = AssetRegistry.Sounds.SanguineSingularity.SanguineLaugh;
                    laughSound.PitchVariance = 0.15f;
                    laughSound.Pitch = 0.8f;
                    SoundEngine.PlaySound(laughSound, NPC.position);
                }
                if(Timer2 == 70f)
                {
                    var screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                    screenShaderSystem.TintScreen(Color.Red * 0.3f, 0.5f, 30);
                    CreateRedFlash();
                    SoundStyle chargeSound = AssetRegistry.Sounds.SanguineSingularity.SanguineBurstReady;
                    chargeSound.PitchVariance = 0.5f;
                    chargeSound.Pitch = -0.5f;
                    SoundEngine.PlaySound(chargeSound, NPC.position);
                    _draw.headless = true;
                    var boom = FXUtil.GlowCircleBoom(NPC.Center, Color.Red, Color.DarkBlue, Color.Black);
                    boom.Scale *= 2;
                    ShakeModSystem.Shake = 32;
                    if (MultiplayerHelper.IsHost)
                    {
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero,
                            ModContent.ProjectileType<BloodyChargeBoom>(), BloodyChargeBoomDamage, 1, Main.myPlayer);
                        float num = 3f;
                        for (float f = 0f; f < num; f++)
                        {
                            float completionRatio = f / num;
                            Vector2 upVelocity = Vector2.UnitX * NPC.direction * 15;
                            upVelocity = upVelocity.RotatedBy(MathHelper.ToRadians(135  * completionRatio * -NPC.direction));
                            int bloodyBurstProjectile = ModContent.ProjectileType<BloodyBurst>();
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, upVelocity, bloodyBurstProjectile, BloodyBurstDamage, 1, Main.myPlayer);
                        }
                    }
                    Timer2 = 0;
                }
            }
            else
            {
                _draw.alpha = 0f;
                _draw.afterImageAlpha *= 0.9f;
                _animation = AnimationState.Walk;
                NPC.velocity = Vector2.Zero;
                NPC.rotation = 0;
            }

  
            if (Timer % 20 == 0)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 spawnPoint = MyTarget.Center + new Vector2(0, -1000);
                    spawnPoint.X += Main.rand.NextFloat(-800, 800);
                    Projectile.NewProjectile(SourceFromThis, spawnPoint, Vector2.Zero,
                        ModContent.ProjectileType<BloodRain>(), BloodRainDamage, 1, Main.myPlayer, ai1: Main.rand.NextFloat(25, 50f));
                }
                AttackNumber++;
            }

            if (AttackNumber >= 35)
            {
                SwitchState(AIState.BloodRain_End);
            }
        }

        private void AI_BloodRain_End()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                if (MultiplayerHelper.IsHost)
                {
                    _teleportPosition = MyTarget.Center + new Vector2(-500, 0);
                    NPC.netUpdate = true;
                }
            }
            _animation = AnimationState.Walk;

            float time = 60f;
            _draw.alpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / time));
            NPC.velocity.X = MathHelper.Lerp(3f, 0f, EasingFunction.InOutSine(Timer / time));
            if (Timer >= time)
            {
                SwitchState(AIState.Idle);
            }
        }

        #endregion


        #region Blood Boil
        private void AI_BloodBoil_Start()
        {
            //Walks slowly around the player as bloody boils explode from its body and then home back towards you
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            _animation = AnimationState.Walk;
            TargetOutlineColor = Color.Yellow;
            NPC.velocity.X *= 0.9f;
            NPC.velocity.Y *= 0.95f;
            NPC.rotation = NPC.velocity.X * 0.025f;

            if (Timer >= 30)
            {
                SwitchState(AIState.BloodBoil_Walk);
            }
        }

        private void AI_BloodBoil_Walk()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }
            NPC.noTileCollide = true;
            NPC.noGravity = true;

            _animation = AnimationState.Walk;
            TargetOutlineColor = Color.Red;
        }

        private void AI_BloodBoil_End()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            _animation = AnimationState.Walk;
            if (Timer >= 60)
            {
                SwitchState(AIState.Idle);
            }
        }
        #endregion


        #region Blood Crack
        private void AI_BloodCrack_Start()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            if (Timer >= 60)
            {
                SwitchState(AIState.BloodCrack_Geyser);
            }
        }

        private void CreateGeyser()
        {
            if (MultiplayerHelper.IsHost)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(1800, 1800);
                Projectile.NewProjectile(SourceFromThis, NPC.Center, velocity,
                    ModContent.ProjectileType<BloodGeyser>(), BloodGeyserDamage, 1, Main.myPlayer);
            }
        }

        private void AI_BloodCrack_Geyser()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            if (Timer % 60 == 0)
            {
                NPC.velocity = Vector2.Zero;
                AttackNumber++;
                CreateGeyser();
            }

            if (AttackNumber >= 6)
            {
                SwitchState(AIState.BloodCrack_End);
            }
        }

        private void AI_BloodCrack_End()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }
            SwitchState(AIState.Idle);
        }
        #endregion

        #region Ghastly Blood Dash

        private void AI_GhastlyBloodDash_Start()
        {

        }

        private void AI_GhastlyBloodDash_Run()
        {

        }

        private void AI_GhastlyBloodDash_End()
        {

        }

        #endregion

        private void AI_Phase2Transition()
        {

        }


        #region Drawcode
        private void DrawAfterImage(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
      
            Rectangle frame = NPC.frame;
            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                Vector2 oldDrawPos = oldPos - screenPos;
                Vector2 drawOrigin = new Vector2(128, 64);
                float f = i;
                float interpolant = f / (float)NPC.oldPos.Length;
                Color fadeColor = Color.Lerp(Color.Lerp(Color.Red, Color.Blue, interpolant), Color.Transparent, interpolant);
                fadeColor *= _draw.afterImageAlpha;
                oldDrawPos += NPC.Size / 2f;
                fadeColor *= 0.3f;

                SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                if (spriteEffects == SpriteEffects.FlipHorizontally)
                    drawOrigin.X = (frame.Width - drawOrigin.X);

                spriteBatch.Draw(texture, oldDrawPos, NPC.frame, fadeColor, NPC.oldRot[i], drawOrigin, NPC.scale, spriteEffects, 0f);
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawWalkingTrail(spriteBatch, screenPos, drawColor);
            DrawAfterImage(spriteBatch, screenPos);
            DrawSingularity(spriteBatch, screenPos, drawColor);
            Draw(spriteBatch, screenPos, drawColor);
            DrawRedFlash(spriteBatch, screenPos, drawColor);
            GoreManager.Draw(spriteBatch, screenPos, drawColor);
            return false;
        }

        private Color GetWalkingTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Transparent, completionRatio) * _draw.afterImageAlpha;
        }

        private float GetWalkingTrailWidth(float completionRatio)
        {
            return MathHelper.SmoothStep(0f, 20f, EasingFunction.QuadraticBump(completionRatio));
        }

        private void DrawWalkingTrail(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var shader = BasicLaserShader.Instance;
            shader.InnerColor = Color.Red;
            shader.OuterColor = Color.Transparent;
            TrailDrawer.Draw(spriteBatch, NPC.oldPos, GetWalkingTrailColor, GetWalkingTrailWidth, shader, offset: new Vector2(0, NPC.frame.Height)); ;
        }
        private void DrawRedFlash(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawCenter = NPC.Center - screenPos;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = new Vector2(128, 64);
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (spriteEffects == SpriteEffects.FlipHorizontally)
                drawOrigin.X = (frame.Width - drawOrigin.X);

            Color glowColor = Color.Red;
            glowColor.A = 0;
            glowColor *= _draw.flashAlpha;
            glowColor *= 0.5f;
            spriteBatch.Draw(texture, drawCenter, frame, glowColor, NPC.rotation, drawOrigin, _draw.scale, spriteEffects, 0);

        }
        private void DrawSingularity(SpriteBatch spriteBatch, Vector2 screenPos, Color color)
        {
            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyBigTexture).Value;
            Vector2 drawPosition = NPC.Center - screenPos;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawScale = NPC.scale * Vector2.One;
            drawScale *= _draw.singularityScale;
            drawScale *= ExtraMath.Osc(0.9f, 1f, speed: 18f);

            var shader = SingularityShader.Instance;
            spriteBatch.Restart(effect: shader.Effect);

            Color redSingularity = Color.Red;
            redSingularity *= _draw.alpha;
            spriteBatch.Draw(texture, drawPosition, null, redSingularity, NPC.rotation, drawOrigin, drawScale * 0.5f, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();


            Texture2D diskTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SF").Value;
            Vector2 diskDrawOrigin = diskTexture.Size() / 2f;
            Color diskDrawColor = Color.Lerp(Color.White, Color.Lerp(Color.White, Color.Red, 0.15f), ExtraMath.Osc(0f, 1f, speed: 2));
            diskDrawColor = diskDrawColor.MultiplyRGB(Color.Red);
            diskDrawColor.A = 0;
            diskDrawColor *= _draw.alpha;

            float scaleOsc = ExtraMath.Osc(0.5f, 0.65f, speed: 1);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation, diskDrawOrigin, drawScale * scaleOsc * 0.5f, SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation, diskDrawOrigin, drawScale * scaleOsc * 0.45f, SpriteEffects.None, 0);


            diskTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SF2").Value;
            float rotOffset = MathHelper.ToRadians(-30f + ExtraMath.Osc(5f, 10f, speed: 2));
            for (float f = 0; f < 4; f++)
            {

                spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation + rotOffset, diskDrawOrigin, drawScale * 0.4f * scaleOsc * new Vector2(1.5f, 0.2f) * 0.66f, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation + rotOffset, diskDrawOrigin, drawScale * 0.4f * scaleOsc * new Vector2(3.5f, 0.2f) * 0.66f, SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor * 0.5f, NPC.rotation + rotOffset, diskDrawOrigin, drawScale * 0.4f * scaleOsc * new Vector2(7.5f, 0.2f) * 0.66f, SpriteEffects.None, 0);

            rotOffset = MathHelper.ToRadians(25f + ExtraMath.Osc(-10f, -5f, speed: 2, offset: 2));
            //Inverse rings
            for (float f = 0; f < 4; f++)
            {

                spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation + rotOffset, diskDrawOrigin, drawScale * 0.4f * scaleOsc * new Vector2(1.5f, 0.2f) * 0.36f, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, NPC.rotation + rotOffset, diskDrawOrigin, drawScale * 0.4f * scaleOsc * new Vector2(3.5f, 0.2f) * 0.36f, SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor * 0.5f, NPC.rotation + rotOffset, diskDrawOrigin, drawScale * 0.4f * scaleOsc * new Vector2(7.5f, 0.2f) * 0.36f, SpriteEffects.None, 0);
            DrawIncresionDiskBottom(spriteBatch, screenPos, color);
            DrawIncresionDiskTop(spriteBatch, screenPos, color);
        }
        private void DrawIncresionDiskBottom(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw Incresion Disk
            Rectangle incresionDiskRect = DrawHelper.FrameGrid(_incresionDiskFrameBottom, columns: 5, frameWidth: 400, frameHeight: 200);
            Texture2D supernovaTopTexture = ModContent.Request<Texture2D>(Texture + "_Disk").Value;

            //Incresion Disk Draw Color
            Color incresionDiskDrawColor = Color.White;
            incresionDiskDrawColor *= 0.25f;
            incresionDiskDrawColor.A = 0;

            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = incresionDiskRect.Size() / 2;
            float drawScale = NPC.scale * _draw.singularityScale.X * 1.75f;
            drawScale *= 0.4f;
            float drawRotation = NPC.rotation;
            drawRotation -= MathHelper.ToRadians(30);
            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);

            incresionDiskDrawColor = Color.White;
            incresionDiskDrawColor *= 0.25f;
            incresionDiskDrawColor.A = 0;

            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, drawRotation, drawOrigin, drawScale * 1.5f, SpriteEffects.None, 0);

            incresionDiskDrawColor = Color.Blue;
            incresionDiskDrawColor *= 0.25f;
            incresionDiskDrawColor.A = 0;

            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, drawRotation, drawOrigin, drawScale * 2, SpriteEffects.None, 0);



            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, drawRotation - MathHelper.ToRadians(90), drawOrigin, drawScale * 2, SpriteEffects.None, 0);
        }


        private void DrawIncresionDiskTop(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw Incresion Disk
            Rectangle incresionDiskRect = DrawHelper.FrameGrid(_incresionDiskFrameTop, columns: 4, frameWidth: 480, frameHeight: 200);
            Texture2D supernovaTopTexture = ModContent.Request<Texture2D>(Texture + "_Top").Value;

            //Incresion Disk Draw Color
            Color incresionDiskDrawColor = Color.White;
            incresionDiskDrawColor *= 0.15f;
            incresionDiskDrawColor.A = 0;

            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 drawOrigin = incresionDiskRect.Size() / 2;

            float drawScale = NPC.scale * 3 * _draw.singularityScale.X;
            drawScale *= 0.4f;
            float drawRotation = NPC.rotation;
            drawRotation -= MathHelper.ToRadians(30);
            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);

            drawRotation -= MathHelper.ToRadians(90);
            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

        private int _frame;
        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.2f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }

            switch (_animation)
            {
                case AnimationState.Idle:
                    _frame = 0;
                    break;
                case AnimationState.Walk:
                    if (_frame < 2)
                    {
                        _frame = 2;
                    }
                    else if (_frame >= 10)
                    {
                        _frame = 2;
                    }
                    break;
                case AnimationState.Run:
                    if (_frame < 18)
                    {
                        _frame = 18;
                    }
                    else if (_frame >= 23)
                    {
                        _frame = 18;
                    }
                    break;
            }
            int frame = _frame;
            if (_draw.headless)
            {
                switch (_animation)
                {
                    case AnimationState.Idle:
                        frame += 1;
                        break;
                    case AnimationState.Walk:
                        frame += 8;
                        break;
                    case AnimationState.Run:
                        frame += 5;
                        break;
                }
            }
            NPC.frame.Y = frameHeight * frame;
        }


        private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color color)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawCenter = NPC.Center - screenPos;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = new Vector2(136, 54);
            SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (spriteEffects == SpriteEffects.FlipHorizontally)
                drawOrigin.X = (frame.Width - drawOrigin.X);
            spriteBatch.Draw(texture, drawCenter, frame, color * _draw.alpha, NPC.rotation, drawOrigin, _draw.scale, spriteEffects, 0);
        }

        private void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            DrawSprite(spriteBatch, screenPos, Color.White.MultiplyRGB(lightColor));
        }

        private void DrawOutline(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            DrawSprite(spriteBatch, screenPos, _draw.outlineColor);
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            float outlineOffset = 2;
            Vector2 v = Vector2.UnitX * outlineOffset;
            Vector2 h = Vector2.UnitY * outlineOffset;
            DrawOutline(spriteBatch, screenPos + v);
            DrawOutline(spriteBatch, screenPos - v);
            DrawOutline(spriteBatch, screenPos + h);
            DrawOutline(spriteBatch, screenPos - h);
        }

        #endregion
    }
}
