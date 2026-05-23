using Stellamod.Assets;
using Stellamod.Common.Animations;
using Stellamod.Common.Shaders;
using Stellamod.Content.Gores;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime;

public struct PunkerPrimeDraw
{
    public Color outlineColor;
    public Vector2 scale;
    public Vector2 shakeOffset;
    public float afterImageStrength;

    public void SetDefaults()
    {
        scale = Vector2.One;
        outlineColor = Color.Transparent;
        afterImageStrength = 0f;
    }
}
public class PunkerBoom : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 256;
        Projectile.height = 256;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 12;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            if (this.OwnedByLocalClient())
            {
                for (float f = 0; f < 7; f++)
                {
                    float ratio = (f + 1) / 7;
                    Vector2 vel = Vector2.Lerp(-Vector2.UnitX, Vector2.UnitX, ratio);
                    vel *= 7;
                    vel.Y -= 1;
                    Vector2 offset = Projectile.Center + vel;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), offset, vel, ModContent.ProjectileType<AssaultBullet>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, ai1: 1);
                }
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -Vector2.UnitX * 6, ModContent.ProjectileType<AssaultBullet>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, ai1: 1);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitX * 6, ModContent.ProjectileType<AssaultBullet>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, ai1: 1);

            }
            PixelPrimitiveCircleFactory.CreatePunkerBoom(Projectile.Center);
            SoundStyle smash = AssetRegistry.Sounds.Melee.HammerSmash3;
            smash.PitchVariance = 0.3f;
            SoundEngine.PlaySound(smash, Projectile.position);

            SoundStyle steaming = AssetRegistry.Sounds.SteamPunking.MechSteaming;
            steaming.PitchVariance = 0.3f;
            steaming.Volume = 0.5f;
            SoundEngine.PlaySound(steaming, Projectile.position);

            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Red, Color.DarkRed, 35, 0.24f);
            fx.Scale *= 2;
            int[] gores = AutoGoreLoader.FindGores("GrayRock");
            foreach (int g in gores)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
            }

            var p = Particle<ThickSmokeParticle>.Spawn(Projectile.Bottom, Vector2.Zero, Color.DarkGray);

            var sear = LegacyParticle.NewParticle<SearParticle>(Projectile.Center, Vector2.Zero);
            sear.innerColor = Color.Gray;
            sear.outerColor = Color.Blue;
            sear.fadeToColor = Color.Black;
            FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
    
            for (float f = 0; f < 4f; f++)
            {
                Vector2 pos = Projectile.Center;
                pos += Main.rand.NextVector2Circular(80, 80);
                var zap = LegacyParticle.NewParticle<ZapParticle>(pos, Vector2.UnitY.RotatedByRandom(10) * Main.rand.NextFloat(2, 15));
                zap.innerColor = Color.Gray;
                zap.outerColor = Color.Red;
                zap.fadeToColor = Color.Black;
                zap.Scale *= Main.rand.NextFloat(0f, 0.5f);
                zap.Rotation = Main.rand.NextFloat(0f, 3f);
            }

            SoundStyle smashSound;
            smashSound = AssetRegistry.Sounds.Bishinine.Comet2;
            foreach (int g in gores)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
            }
            FXUtil.ShakeCamera(Projectile.Center, 1024, 64);

            var p3 = FXUtil.GlowCircleBoom(Projectile.Center,
               innerColor: Color.Gray,
               glowColor: Color.Red,
               outerGlowColor: Color.DarkRed, duration: 15, baseSize: .09f);
            p3.Scale *= 7;

            smashSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(smashSound, Projectile.position);


            var part = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
            part.fadeToColor = Color.Black;
            part.outerColor = Color.Gray;
            part.noStretch = true;
            part.shrink = true;

            var part2 = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
            part2.fadeToColor = Color.Black;
            part2.outerColor = Color.Gray;
            part2.noStretch = true;
            part2.color *= 0.5f;
            for (float f = 0; f < 5; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(16, 16);
                vel.Y -= 10;
                var d = Dust.NewDustPerfect(Projectile.Center,
                    ModContent.DustType<GlowSparkleDust>(), newColor: Color.Gray, Scale: Main.rand.NextFloat(0f, 2f), Velocity: vel);
            }

            for (float f = 0; f < 24; f++)
            {
                Vector2 vel = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(80));
                vel *= Main.rand.NextFloat(8f, 50);
                var spawnParams = DustParticleSpawnParams.Default;
                spawnParams.scaleRange *= 2f;
                spawnParams.outerColor = Color.Red;
                var dp = DustParticle.Spawn(Projectile.Center, vel, spawnParams);
                dp.noTileCollide = true;
                dp.dampening = 0.05f;
                dp.gravity = 0;
            }
        }
        float alpha = (float)Projectile.timeLeft / 12f;
        ShakeScreenPosition.Shake = MathHelper.Lerp(8, 2, alpha);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }
}
public class PunkerPrime : ScarletBoss,
    IDrawOutlines
{
    private Vector2 _teleportPosition;
    private enum AIState
    {
        Spawn,
        Despawn,
        Idle,
        Flurry,
        Death,
        Warning_Prepare_Attacks,
        SummonArms,

        Special_Start,
        Special_Loop,
        Special_End,

        Crash_Start,
        Crash_Smash,
        Crash_Rise,
        Emote_Start,
        Emote_Laugh
    }
    private const string Anim_Bouncing_Fast = "bouncefast";
    private const string Anim_Bouncing_Slow = "bounceslow";
    private const string Anim_Idle = "idle";
    private Animator _animator;
    private Animator Animator
    {
        get
        {
            if (_animator == null)
            {
                _animator = new Animator();
                var bounceFast = new SpriteAnimation(0, 9, isLooping: true, frameSpeed: 0.4f);
                _animator.AddAnimation(Anim_Bouncing_Fast, bounceFast);

                var running = new SpriteAnimation(11, 15, isLooping: true, frameSpeed: 0.1f);
                _animator.AddAnimation(Anim_Bouncing_Slow, running);

                var idle = new SpriteAnimation(10, 10, isLooping: true, frameSpeed: 0.35f);
                _animator.AddAnimation(Anim_Idle, idle);
            }

            return _animator;
        }
    }
    private PunkerPrimeDraw _draw;
    private Vector2 _startCenter;
    private Vector2 _hoverCenter;
    private Color TargetOutlineColor;
    private bool[] _disabledArms;
    private bool[] DisabledArms
    {
        get
        {
            if( _disabledArms == null)
            {
                _disabledArms = new bool[8];
            }
            return _disabledArms;
        }
    }
    private bool _showNamePlate;
    private bool _phaseTransition;
    private Color _spotlightColor;
    private Color _targetSpotlightColor;
    private float _glowAlpha;
    private float _targetGlowAlpha;
    private float _upDown;
    private float _rotOffset;
    private Vector2 _upDownOffset;
    private Vector2 _originalVelocity;
    private string _animationToPlay = string.Empty;
    private Queue<int> _armQueueBacking;
    private Queue<int> ArmQueue
    {
        get
        {
            if (_armQueueBacking == null)
            {
                _armQueueBacking = new Queue<int>();
            }
            if (_armQueueBacking.Count <= 0)
            {
                while (_armQueueBacking.Count < 5)
                {
                    int armToSummon = Main.rand.Next(0, 5);
                    if (InPhase2)
                    {
                        armToSummon = Main.rand.Next(0, 8);
                    }
                    if (_armQueueBacking.Contains(armToSummon))
                        continue;
                    _armQueueBacking.Enqueue(armToSummon);
                }
            }
            return _armQueueBacking;
        }
    }
    private Metronome _metronome;
    private Metronome Metronome
    {
        get
        {
            _metronome ??= new Metronome(150);
            return _metronome;
        }
    }
    private float _attackCycle;
    private ref float Timer => ref NPC.ai[0];

    private PunkerPrimeArm[] _arms;
    private NPC _boomBoxNPC;
    private ref PunkerPrimeArm Chainsaw1 => ref _arms[0];
    private ref PunkerPrimeArm Chainsaw2 => ref _arms[1];
    private ref PunkerPrimeArm Drill => ref _arms[2];
    private ref PunkerPrimeArm Pincher => ref _arms[3];
    private ref PunkerPrimeArm SawbladeLauncher => ref _arms[4];

    public bool InPhase2 => NPC.life < NPC.lifeMax / 2;
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    private ref float SuperchargeTimer => ref NPC.ai[2];
    private ref float SpecialTimer => ref NPC.ai[3];
    private int PrimeSawbladeDamage => 45;
    private int PunkerBoomDamage => 70;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_teleportPosition);
        writer.WriteVector2(_hoverCenter);
        writer.WriteVector2(_startCenter);
        writer.WriteVector2(_originalVelocity);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _teleportPosition = reader.ReadVector2();
        _hoverCenter = reader.ReadVector2();
        _startCenter = reader.ReadVector2();
        _originalVelocity = reader.ReadVector2();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 16;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCID.Sets.TrailCacheLength[NPC.type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _draw.SetDefaults();
        NPC.width = 128;
        NPC.height = 128;
        NPC.damage = 100;
        NPC.defense = 28;
        NPC.lifeMax = 24000;

        NPC.value = Item.buyPrice(gold: 5);
        NPC.knockBackResist = 0f;
        NPC.boss = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.npcSlots = 30f;

        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/PunkerPrime");
        NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        Animator.Update();
        NPC.frame.Y = Animator.GetFrameY(frameHeight);
    }
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && false;
    }

    private void ManageMetronome()
    {
        Metronome.Update();
        if (_upDown == 0)
            _upDown = 1;
        if (Metronome.beatHit)
        {
            if (!string.IsNullOrEmpty(_animationToPlay))
                Animator.PlayAnimation(_animationToPlay);
            _upDown *= -1;
        }
        _rotOffset = MathHelper.Lerp(_rotOffset, 0.1f * _upDown, 0.2f);
        _upDownOffset = Vector2.Lerp(_upDownOffset, Vector2.UnitY * _upDown * 8, 0.2f);
    }
    public override void AI()
    {
        base.AI();

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
            NPC.position = _teleportPosition;
            _teleportPosition = Vector2.Zero;
        }

        if (!_phaseTransition && InPhase2)
        {
            float numDust = 16;
            for (float d = 0; d < numDust; d++)
            {
                Vector2 spawnPosition = NPC.Top;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Vector2.Zero;
                spawnVelocity.Y = Main.rand.NextFloat(-10, -1f);
                spawnVelocity += Main.rand.NextVector2Circular(8, 8);


                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                var steamParticle = Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
            }

            var boom = FXUtil.GlowCircleBoom(NPC.Center, Color.Yellow, Color.Red, Color.DarkRed);
            boom.Scale *= 2f;

            float numGlowDust = 16f;
            for (float d = 0; d < numGlowDust; d++)
            {
                Vector2 dustVelocity = Main.rand.NextVector2Circular(16, 16);
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), newColor: Color.Red, Scale: Main.rand.NextFloat(0.5f, 1f));
            }

            SoundStyle mechSteaming = AssetRegistry.Sounds.SteamPunking.MechSteaming;
            SoundEngine.PlaySound(mechSteaming, NPC.position);
            _phaseTransition = true;
        }


        _targetGlowAlpha = 0;
        _targetSpotlightColor = Color.Transparent;
        ManageMetronome();
        MoveSlightlyTowardMe();
        Lighting.AddLight(NPC.Center, TorchID.Red);
        switch (State)
        {
            case AIState.Spawn:
                AI_Spawn();
                break;
            case AIState.Despawn:
                AI_Despawn();
                break;
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Flurry:
                AI_Flurry();
                break;
            case AIState.Death:
                AI_Death();
                break;
            case AIState.SummonArms:
                AI_SummonArms();
                break;
            case AIState.Warning_Prepare_Attacks:
                AI_WarningPrepareAttacks();
                break;
            case AIState.Special_Start:
                AI_Special();
                break;
            case AIState.Special_Loop:
                AI_SpecialLoop();
                break;
            case AIState.Special_End:
                AI_SpecialEnd();
                break;

            case AIState.Crash_Start:
                AI_CrashStart();
                break;
            case AIState.Crash_Smash:
                AI_CrashSmash();
                break;
            case AIState.Crash_Rise:
                AI_CrashRise();
                break;
            case AIState.Emote_Start:
                AI_EmoteStart();
                break;
            case AIState.Emote_Laugh:
                AI_EmoteLaugh();
                break;
        }

        _glowAlpha = MathHelper.Lerp(_glowAlpha, _targetGlowAlpha, 0.1f);
        _spotlightColor = Color.Lerp(_spotlightColor, _targetSpotlightColor, 0.1f);
    }

    private bool CanUseArm(int armIndex)
    {
        return !DisabledArms[armIndex];
    }

    private void MoveSlightlyTowardMe()
    {
        Player player = Main.LocalPlayer;
        Vector2 vectorHere = (NPC.Center - player.Center);
        vectorHere *= 0.2f;
        OffsetCameraModifier.FocusTargetOffset = vectorHere;
    }
    private void AI_SummonArms()
    {
        int armToSummon = ArmQueue.Dequeue();

        //Just recall this function until you get to an arm that you can summon
        if (!CanUseArm(armToSummon))
        {
            AI_SummonArms();
            return;
        }
        if (MultiplayerHelper.IsHost)
        {
            PunkerPrimeArm arm = _arms[armToSummon];

            if (InPhase2 && SuperchargeTimer > 600)
            {
                SuperchargeTimer = 0f;
                arm.SuperchargeAttack();
            }
            else
            {
                arm.Attack();
            }
        }

        SwitchState(AIState.Flurry);
    }

    private void SummonArm()
    {
        int armToSummon = ArmQueue.Dequeue();

        //Just recall this function until you get to an arm that you can summon
        if (!CanUseArm(armToSummon))
        {
            AI_SummonArms();
            return;
        }
        if (MultiplayerHelper.IsHost)
        {
            PunkerPrimeArm arm = _arms[armToSummon];

            if (InPhase2 && SuperchargeTimer > 600)
            {
                SuperchargeTimer = 0f;
                arm.SuperchargeAttack();
            }
            else
            {
                arm.Attack();
            }
        }
    }

    private void AI_WarningPrepareAttacks()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
        }
    }


    private void AI_CrashStart()
    {
        _animationToPlay = Anim_Idle;
        Timer++;
        if(Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                _boomBoxNPC.ai[3] = 1;
                _boomBoxNPC.netUpdate = true;
            }
            NPC.TargetClosest();
            _originalVelocity = NPC.velocity;
        }

        float revTime = 100;
        float ratio = Timer / revTime;
        float ease = EasingFunction.InOutSine(ratio);
        Vector2 targetPosition = MyTarget.Center + new Vector2(0, -500);
        Vector2 targetVelocity = targetPosition - NPC.Center;
        Vector2 easedVelocity = Vector2.Lerp(_originalVelocity, targetVelocity, ease);

        NPC.velocity = easedVelocity;
        NPC.rotation = NPC.velocity.X * 0.025f;
        NPC.rotation += MathHelper.SmoothStep(0, MathHelper.TwoPi, EasingFunction.InOutExpo(ratio));
        TargetOutlineColor = Color.Yellow;
        if(Timer >= revTime)
        {
            SwitchState(AIState.Crash_Smash);
        }
    }

    private void AI_CrashSmash()
    {
        _animationToPlay = Anim_Bouncing_Fast;
        Timer++;
        if(Timer == 1)
        {
            NPC.TargetClosest();
        }

        if (Timer % 5 == 0)
        {
            var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Bottom, -NPC.velocity);
            p2.Scale *= 0.5f;
        }

        if (Timer % 10 == 0)
        {
            Vector2 velocity = -Vector2.UnitY;
            velocity *= Main.rand.NextFloat(5f, 10f);
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(60));
            
            DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
            spawnParams.outerColor = Color.Red;
            var d = DustParticle.Spawn(NPC.Bottom, velocity, spawnParams);
            d.noTileCollide = true;
        }

        NPC.noTileCollide = false;
        NPC.velocity.X *= 0.8f;
        NPC.velocity.Y += 0.1f;
        if(NPC.velocity.Y > 0)
            NPC.velocity.Y *= 1.05f;
        if (NPC.velocity.Y > 35)
            NPC.velocity.Y = 35;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.01f);
        if (NPC.collideY && NPC.Bottom.Y > MyTarget.Top.Y)
        {
            SwitchState(AIState.Crash_Rise);
        }
    }

    private void AI_CrashRise()
    {
        _animationToPlay = Anim_Bouncing_Slow;
        Timer++;
        if(Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero, ModContent.ProjectileType<PunkerBoom>(), PunkerBoomDamage, 1, Main.myPlayer);
            }
            NPC.TargetClosest();         
        }

        if(NPC.velocity.Y > 0)
            NPC.velocity.Y *= 0.8f;
        else
        {
            NPC.velocity.Y -= 0.05f;
            NPC.velocity.Y *= 1.05f;
            if(NPC.velocity.Y < -10)
            {
                SwitchState(AIState.Emote_Start);
            }
        }
    }

    private void AI_EmoteStart()
    {
        _animationToPlay = Anim_Bouncing_Fast;
        Timer++;
        if(Timer == 1)
        {
            SoundStyle cheering = AssetRegistry.Sounds.Collosseum.GintzeCheer;

            SoundEngine.PlaySound(cheering);
            if (MultiplayerHelper.IsHost)
            {
                for (int i = 0; i < _arms.Length; i++)
                {
                    if(i < 4)
                    {
                        _arms[i].dabinState = PunkerPrimeArm.DabState.DabLeft_Bent;
                    }
                    else
                    {
                        _arms[i].dabinState = PunkerPrimeArm.DabState.DabLeft_Straight;
                    }
                    _arms[i].NPC.netUpdate = true;
                }
            }
        }

        if(Timer % 30 == 0)
        {
            int gore1 = GoreHelper.TypeFallingLeafWhite;
            int gore2 = GoreHelper.TypeFallingLeafRed;
            for (int i = 0; i < 7; i++)
            {
                Vector2 pos = NPC.Center;
                pos += Main.rand.NextVector2Circular(256, 256);
                Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                Gore.NewGore(NPC.GetSource_FromThis(), pos, velocity, gore1);

                velocity = velocity.RotatedByRandom(MathHelper.TwoPi);
                Gore.NewGore(NPC.GetSource_FromThis(), pos, velocity, gore2);
            }
        }

        _targetSpotlightColor = Color.White * 0.5f;
        NPC.velocity.X = ExtraMath.Osc(-4f, 4f, speed: 3);
        NPC.velocity.Y *= 0.95f;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.01f);
        CameraTargetSystem.AddTarget(NPC.Center);
        if(Timer >= 160)
        {
            SwitchState(AIState.Emote_Laugh);
        }
    }

    private void AI_EmoteLaugh()
    {
        _animationToPlay = Anim_Bouncing_Fast;
        Timer++;
        if (Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                for (int i = 0; i < _arms.Length; i++)
                {
                    _arms[i].dabTimer = 0;
                    _arms[i].dabinState = PunkerPrimeArm.DabState.DabEnd;
                    _arms[i].NPC.netUpdate = true;
                }
            }
        }

        NPC.velocity.X = ExtraMath.Osc(-4f, 4f, speed: 3);
        NPC.velocity.Y *= 0.95f;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.01f);
        if(Timer >= 80)
        {
            SwitchState(AIState.Idle);
        }
        
    }

    private void AI_Special()
    {
        //This is the saw attack that this goober has
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
            _startCenter = NPC.Center;
            _hoverCenter = MyTarget.Center + new Vector2(0, -300);
            SoundStyle prepSound = AssetRegistry.Sounds.SteamPunking.MechSaw;
            prepSound.PitchVariance = 0.3f;
            prepSound.Pitch = -0.5f;
            SoundEngine.PlaySound(prepSound, NPC.position);
        }

        TargetOutlineColor = Color.Yellow;
        float revTime = 60f;
        float completionRatio = Timer / revTime;
        float ease = EasingFunction.Anticipation2(completionRatio);
        Vector2 targetCenter = Vector2.Lerp(_startCenter, _hoverCenter, ease);
        Vector2 velocity = (targetCenter - NPC.Center);
        NPC.velocity = velocity;
        NPC.rotation = NPC.velocity.X * 0.02f;

        _draw.afterImageStrength = MathHelper.Lerp(_draw.afterImageStrength, 1f, 0.1f);
        if (Timer >= revTime)
        {
            SwitchState(AIState.Special_Loop);
        }
    }

    private void AI_SpecialLoop()
    {
        Timer++;
        if (Timer == 1)
        {
            _startCenter = _hoverCenter;
            _hoverCenter = MyTarget.Center + new Vector2(0, -64);

            if (MultiplayerHelper.IsHost)
            {
                int shouldDrop = InPhase2 ? 2 : 0;
                Projectile.NewProjectile(SourceFromThis, NPC.Bottom, Vector2.Zero,
                    ModContent.ProjectileType<PrimeMegaSaw>(), PrimeSawbladeDamage, 1, Main.myPlayer, ai1: NPC.whoAmI, ai2: shouldDrop);
            }
        }

        TargetOutlineColor = Color.Red;
        float revTime = 60f;
        float completionRatio = Timer / revTime;
        float ease = EasingFunction.Anticipation2(completionRatio);
        Vector2 targetCenter = Vector2.Lerp(_startCenter, _hoverCenter, ease);
        Vector2 velocity = (targetCenter - NPC.Center);
        NPC.velocity = velocity;
        _draw.shakeOffset = Main.rand.NextVector2Circular(2, 2);
        NPC.rotation = _draw.shakeOffset.ToRotation() * 0.02f;

        _draw.afterImageStrength = MathHelper.Lerp(_draw.afterImageStrength, 1f, 0.1f);
        if (Timer >= revTime)
        {
            SwitchState(AIState.Special_End);
        }
    }

    private void AI_SpecialEnd()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
        }

        if (Timer % 5 == 0)
        {
            Dust.NewDust(NPC.BottomLeft, NPC.width, 2, DustID.FireworkFountain_Red);
        }

        if (Timer % 6 == 0)
        {
            LegacyParticle.NewParticle<SparkParticle>(NPC.Bottom + Main.rand.NextVector2Circular(16, 16),
                Main.rand.NextVector2Circular(4, 4), Color.Red);
        }
        if (Timer % 3 == 0)
        {
            SpawnSteamParticle();
        }


        float endTime = 240;

        Vector2 targetPos = MyTarget.Center;
        if (Timer > endTime / 120 && InPhase2)
        {
            targetPos.Y -= 128;
        }

        Vector2 velToTarget = (targetPos - NPC.Center).SafeNormalize(Vector2.Zero);
        velToTarget *= 8f;
        NPC.velocity = Vector2.Lerp(NPC.velocity, velToTarget, 0.1f);
        _draw.shakeOffset = Main.rand.NextVector2Circular(2, 2);
        NPC.rotation = _draw.shakeOffset.ToRotation() * 0.02f;
        if (Timer >= endTime)
        {
            SwitchState(AIState.Idle);
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

    private void TeleportTo(Vector2 teleportCenter)
    {
        if (MultiplayerHelper.IsHost)
        {
            NPC.Center = teleportCenter;
            _teleportPosition = NPC.position;
            NPC.netUpdate = true;
        }
    }

    private void AI_Spawn()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();

            //Start from above the player and come down
            Vector2 teleportCenter = MyTarget.Center + new Vector2(0, -500);
            TeleportTo(teleportCenter);

            SoundStyle mechTurnSound = AssetRegistry.Sounds.SteamPunking.MechTurn;
            mechTurnSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(mechTurnSound, NPC.position);

            SummonArms();
        }

        TargetOutlineColor = Color.Transparent;
        MoveSlightlyTowardMe();

        float time = 120f;
        float completionRatio = Timer / time;
        float ease = EasingFunction.InOutSine(completionRatio);
        float yVelocity = MathHelper.Lerp(3f, 0f, ease);
        NPC.velocity.Y = yVelocity;
        NPC.velocity.X = 0;
        NPC.rotation = 0;
        if (Timer >= time)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void AI_Despawn()
    {
        TargetOutlineColor = Color.Transparent;
        //Just fly up and despawn, very simepl
        Timer++;
        float despawnTime = 90;
        NPC.velocity.Y -= 0.5f;
        NPC.velocity.X *= 0.5f;
        NPC.rotation = 0;
        if (Timer >= despawnTime)
        {
            NPC.active = false;
        }
    }

    private bool CanAttack()
    {
        //This is going to check all of the arms and check how many of them are moving
        int attackingArmCount = 0;
        for (int i = 0; i < _arms.Length; i++)
        {
            PunkerPrimeArm arm = _arms[i];

            //wait
            if (arm.isAttacking)
                attackingArmCount++;
        }
        return attackingArmCount < 2;
    }

    private void InitializeArmsIfDead()
    {
        if(_arms == null)
        {
            _arms = new PunkerPrimeArm[8];
            int index = 0;
            foreach(var npc in Main.ActiveNPCs)
            {
                if (npc.ModNPC == null)
                    continue;
                if (npc.ModNPC is not PunkerPrimeArm arm)
                    continue;
                if (npc.ai[1] != NPC.whoAmI)
                    continue;
                _arms[index++] = arm;
            }
        }
    }
    private T SummonArm<T>() where T : PunkerPrimeArm
    {
        T t = ModContent.GetInstance<T>();
        int type = t.Type;
        int x = (int)NPC.Center.X;
        int y = (int)NPC.Center.Y;
        int npcIndex = NPC.NewNPC(SourceFromThis, x, y, type, ai1: NPC.whoAmI);
        T arm = Main.npc[npcIndex].ModNPC as T;
        return arm;
    }

    private void SummonArms()
    {

        if (!MultiplayerHelper.IsHost)
            return;

        _arms = new PunkerPrimeArm[8];
        _arms[0] = SummonArm<Chainsaw>();
        _arms[1] = SummonArm<Chainsaw2>();
        _arms[2] = SummonArm<Drill>();
        _arms[3] = SummonArm<Pincher>();
        _arms[4] = SummonArm<SawbladeLauncher>();
        _arms[5] = SummonArm<AssaultRifle>();
        _arms[6] = SummonArm<LaserRifle>();
        _arms[7] = SummonArm<ElectroFieldLauncher>();

        int x = (int)NPC.Center.X;
        int y = (int)NPC.Center.Y;
        _boomBoxNPC = NPC.NewNPCDirect(SourceFromThis, x, y, ModContent.NPCType<Boombox>(), ai2: NPC.whoAmI);
    }

    private void AI_Idle()
    {
        _draw.afterImageStrength *= 0.5f;
        _animationToPlay = Anim_Idle;
        SpecialTimer++;

        //Steampunker prime is just going to hover around and above you most of the time for the most part
        //If you get far from him he'll track you, but otherwise he's mostly stationary and doesn't move too much
        //Which should be easy to deal with?
        //The extension from melee should make it easy to hit the cores
        //Hopefully;
        Timer++;
        if (Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                for(int i = 0; i < _arms.Length; i++)
                {
                    _arms[i].dabinState = PunkerPrimeArm.DabState.None;
                    _arms[i].NPC.netUpdate = true;
                }
            }
            NPC.TargetClosest();
        }
        if (InPhase2)
        {
            SuperchargeTimer++;
        }


        //Starts slow and gets faster over time
        float idleTime = 240;
        if (Timer % 15 == 0)
        {
            SpawnSteamParticle();
            if (Main.rand.NextBool(3))
            {
                var d = Dust.NewDustPerfect(NPC.Top, ModContent.DustType<TSmokeDust>(), Scale: Main.rand.NextFloat(0.5f, 1.2f));
            }
        }

        _draw.afterImageStrength = MathHelper.Lerp(_draw.afterImageStrength, 0f, 0.1f);
        if (!_showNamePlate)
        {
            ShowNamePlate();
            _showNamePlate = true;
        }
        Chase(speedMult: 0.25f);

        if (Timer > idleTime / 2)
        {
            _animationToPlay = Anim_Bouncing_Slow;
        }

        TargetOutlineColor = Color.Transparent;
        NPC.rotation = NPC.velocity.X * 0.02f;
        if (Timer >= idleTime)
        {
            if (SpecialTimer >= 1000)
            {
                SpecialTimer = 0f;
                _attackCycle++;
                if(_attackCycle % 2 == 0)
                {
                    SwitchState(AIState.Special_Start);
                }
                else
                {
                    SwitchState(AIState.Crash_Start);
                }
         
            }
            else
            {
                SwitchState(AIState.Flurry);
            }
            //SwitchState(AIState.Crash_Start);
        }
    }

    private void AI_Flurry()
    {
        Timer++;
        _animationToPlay = Anim_Bouncing_Fast;
        if (Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                _boomBoxNPC.ai[3] = 1;
                _boomBoxNPC.netUpdate = true;
            }

            NPC.TargetClosest();
        }

        if (Timer % 15 == 0)
        {
            SpawnSteamParticle();
            if (Main.rand.NextBool(3))
            {
                var d = Dust.NewDustPerfect(NPC.Top, ModContent.DustType<TSmokeDust>(), Scale: Main.rand.NextFloat(0.5f, 1.2f));
            }
        }
        _targetGlowAlpha = 1;
        SpecialTimer++;
        _draw.afterImageStrength = MathHelper.Lerp(_draw.afterImageStrength, 1f, 0.1f);
        Chase();
        NPC.velocity *= 0.94f;
        TargetOutlineColor = Color.Transparent;
        NPC.rotation = NPC.velocity.X * 0.02f;
        if (Timer > 100 && Timer % 90 == 0 && Timer < 400)
        {
            SummonArm();
        }

        if (Timer >= 500)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void Chase(float speedMult = 1f)
    {
        //Crazy movement code
        Vector2 targetPosition = MyTarget.Center;
        targetPosition.Y -= 128;
        Vector2 velocityToPlayer = (targetPosition - NPC.Center);
        velocityToPlayer = velocityToPlayer.SafeNormalize(Vector2.Zero);
        float dist = Vector2.Distance(NPC.Center, targetPosition);
        if (dist <= 0)
            dist = 1;

        float interp = dist / 384;
        interp = EasingFunction.InOutSine(interp);
        float speed = MathHelper.Lerp(6, 20, interp);
        speed *= speedMult;

        float xDist = MathF.Abs(targetPosition.X - NPC.Center.X);
        if (xDist < 256)
            velocityToPlayer.Y -= 0.5f;

        if (dist < speed)
            speed = dist;
        velocityToPlayer *= speed;
        velocityToPlayer *= ExtraMath.Osc(0.5f, 1f, speed: 2);
        velocityToPlayer.Y += ExtraMath.Osc(-5, 5, speed: 2);
        NPC.velocity = Vector2.Lerp(NPC.velocity, velocityToPlayer, 0.04f);
    }

    private void ChooseAttack()
    {
        if (!CanAttack())
            return;
        if (SpecialTimer >= 1000)
        {
            SpecialTimer = 0f;
            SwitchState(AIState.Special_Start);
        }
        else
        {
            SwitchState(AIState.SummonArms);
        }

    }

    private void AI_Death()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
        }

        float deathTime = 300f;
        if (Timer % 5 == 0)
        {
            SpawnSteamParticle();
        }

        if (Timer % 2 == 0)
        {
            _draw.shakeOffset = Main.rand.NextVector2Circular(16, 16);
            NPC.rotation = _draw.shakeOffset.X * 0.05f;
        }

        if (Timer % 12 == 0)
        {
            Vector2 spawnPoint = NPC.Top;
            spawnPoint.X += Main.rand.NextFloat(-64f, 64f);
            var fireDust = Dust.NewDustPerfect(spawnPoint, DustID.FireworkFountain_Red, Scale: Main.rand.NextFloat(0.5f, 1f));
            fireDust.noGravity = false;
        }

        NPC.velocity = Vector2.Zero;
        _draw.afterImageStrength = MathHelper.Lerp(_draw.afterImageStrength, 0f, 0.1f);
        _draw.outlineColor = Color.Lerp(Color.Transparent, Color.Yellow, ExtraMath.Osc(0f, 1f, speed: 12f));
        if (Timer >= deathTime)
        {
            FXUtil.ShakeCamera(NPC.position, 1024, 8);
            ShakeScreenPosition.Shake = 16;

            var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.Yellow, Color.Red, Color.DarkRed);
            fx.Scale *= 4;
            FXUtil.ShakeCamera(NPC.Center, 1024, 8);
            ShakeScreenPosition.Shake = 5;
            if (Main.netMode != NetmodeID.Server)
            {
                int[] gores = new int[]
                {
                     Mod.Find<ModGore>($"{Name}_Gore_0").Type,
                     Mod.Find<ModGore>($"{Name}_Gore_1").Type,
                     Mod.Find<ModGore>($"{Name}_Gore_2").Type,
                     Mod.Find<ModGore>($"{Name}_Gore_3").Type,
                };
                for (int i = 0; i < gores.Length; i++)
                {
                    var gore = gores[i];

                    // Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
                    Vector2 position = NPC.Center;

                    for (float f = 0; f < 7; f++)
                    {
                        Vector2 vel = Main.rand.NextVector2Circular(8, 8);
                        var spawnParams = DustParticleSpawnParams.Default;
                        spawnParams.innerColor = Color.Yellow;
                        Vector2 pos2 = position + Main.rand.NextVector2Circular(32, 32);
                        DustParticle.Spawn(pos2, vel, spawnParams);
                    }

                    Vector2 vel2 = Main.rand.NextVector2Circular(8, 8);
                    vel2 += NPC.velocity;
                    vel2.Y -= 8;
                    Gore.NewGore(NPC.GetSource_Death(), position, vel2, gore, 1f);
                }
            }

            SoundStyle kaboom = new SoundStyle("Stellamod/Assets/Sounds/RekShockwave");
            SoundEngine.PlaySound(kaboom, NPC.position);
            if (Main.netMode != NetmodeID.Server)
                ModContent.GetInstance<ScreenShaderSystem>().TintScreen(Color.Red, 0.2f, 15);

            for (int i = 0; i < 16; i++)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<TSmokeDust>(),
                    (Vector2.One * Main.rand.Next(5, 15)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
            }

            for (float f = 0; f < 12; f++)
            {
                Vector2 v = Main.rand.NextVector2Circular(128, 128);
                var fx2 = FXUtil.GlowStretch(NPC.Center, v);
                fx2.OuterGlowColor = Color.Red;
            }

            float numDust = 32;
            for (float n = 0; n < numDust; n++)
            {
                Vector2 dustVelocity = Main.rand.NextVector2Circular(32, 32);
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), dustVelocity,
                    newColor: Color.Red,
                    Scale: Main.rand.NextFloat(0.5f, 1.5f));
            }

            NPC.Kill();
        }
    }

    public override void OnKill()
    {
        base.OnKill();
        DownedBossTracker.ClearFlag(DownedBossFlag.PunkerPrime);
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
    private void SpawnSteamParticle()
    {
        Vector2 spawnPosition = NPC.Top;
        spawnPosition.X += Main.rand.NextFloat(-64, 64);

        Vector2 spawnVelocity = Vector2.Zero;
        spawnVelocity.Y = Main.rand.NextFloat(-10, -1f);

        float spawnScale = Main.rand.NextFloat(0.75f, 1f);
        var steamParticle = Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (_arms == null)
        {
            InitializeArmsIfDead();
            return false;
        }
        for (int i = 0; i < _arms.Length; i++)
        {
            var arm = _arms[i];
            arm.ForwardIK();
        }

        for (int i = 0; i < _arms.Length; i++)
        {
            var arm = _arms[i];
            arm.DrawPowerCord(spriteBatch, screenPos, drawColor);
        }

        for (int i = 0; i < _arms.Length; i++)
        {
            var arm = _arms[i];
            arm.DrawGunArm(spriteBatch, screenPos, drawColor);
        }

        for (int i = 0; i < _arms.Length; i++)
        {
            var arm = _arms[i];
            arm.DrawGunEffects(spriteBatch, screenPos, drawColor);
        }

        for (int i = 0; i < _arms.Length; i++)
        {
            var arm = _arms[i];
            arm.DrawGun(spriteBatch, screenPos, drawColor);
        }
        for (int i = 0; i < _arms.Length; i++)
        {
            var arm = _arms[i];
            arm.DrawGlowBall(spriteBatch, screenPos, drawColor);
        }
        DrawBodyAfterImage(spriteBatch, screenPos);
        DrawBodySprite(spriteBatch, screenPos, drawColor);

        //oh yeah spot light
        if(_spotlightColor != Color.Transparent)
        {
            float height = -250;
            Vector2 startPos = NPC.Center + new Vector2(-250, height);
            Vector2 endPos = NPC.Center;
            float rotat = (endPos - startPos).ToRotation();
            SpritebatchDrawer spotlightDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.Spotlight, startPos);
            spotlightDrawer.LeftCenterOrigin();
            spotlightDrawer.rotation = rotat;
            spotlightDrawer.color = _spotlightColor;
            spotlightDrawer.color.A = 0;
            spotlightDrawer.scale *= 2;
            spriteBatch.Draw(spotlightDrawer);

            startPos = NPC.Center + new Vector2(250, height);
            rotat = (endPos - startPos).ToRotation();
            spotlightDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.Spotlight, startPos);
            spotlightDrawer.LeftCenterOrigin();
            spotlightDrawer.rotation = rotat;
            spotlightDrawer.color = _spotlightColor;
            spotlightDrawer.color.A = 0;
            spotlightDrawer.scale *= 2;
            spriteBatch.Draw(spotlightDrawer);
        }
        return false;
    }

    private void DrawBodyAfterImage(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Rectangle frame = NPC.frame;
        Vector2 drawOrigin = frame.Size() / 2f;
        float length = NPCID.Sets.TrailCacheLength[Type];
        for (int i = 0; i < length; i++)
        {
            float f = i;
            float completionRatio = f / length;
            Vector2 oldPosition = NPC.oldPos[i];
            Vector2 oldCenter = oldPosition + NPC.Size / 2f - screenPos;
            Color color = Color.Red;
            color *= 0.1f;
            color *= _draw.afterImageStrength;
            color *= MathHelper.SmoothStep(1f, 0f, completionRatio);
            spriteBatch.Draw(texture, oldCenter, frame, color, NPC.rotation, drawOrigin, _draw.scale, SpriteEffects.None, 0);
        }
    }

    private void DrawBodySprite(SpriteBatch spriteBatch, Vector2 screenPos, Color color)
    {
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Rectangle frame = NPC.frame;
        Vector2 drawCenter = NPC.Center - screenPos;
        Vector2 drawOrigin = frame.Size() / 2f;
        drawCenter += _draw.shakeOffset;
        spriteBatch.Draw(texture, drawCenter + _upDownOffset, frame, color, NPC.rotation + _rotOffset, drawOrigin, _draw.scale, SpriteEffects.None, 0);

        Texture2D glowMask = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        Color glowColor = Color.Lerp(Color.Black, Color.Red, ExtraMath.Osc(0f, 1f, speed: 2)) * _glowAlpha;
        spriteBatch.Draw(glowMask, drawCenter + _upDownOffset, frame, glowColor, NPC.rotation + _rotOffset, drawOrigin, _draw.scale, SpriteEffects.None, 0);

    }


    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        float outlineOffset = 2f;
        if (_draw.outlineColor == Color.Transparent)
            return;
        Vector2 h = Vector2.UnitX * outlineOffset;
        Vector2 v = Vector2.UnitY * outlineOffset;
        DrawBodySprite(spriteBatch, screenPos + h, _draw.outlineColor);
        DrawBodySprite(spriteBatch, screenPos - h, _draw.outlineColor);
        DrawBodySprite(spriteBatch, screenPos + v, _draw.outlineColor);
        DrawBodySprite(spriteBatch, screenPos - v, _draw.outlineColor);
    }
}
