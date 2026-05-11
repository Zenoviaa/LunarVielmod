using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.WaterSide.BossesWS.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Palettes;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.BossesWS;

public class LeviathanEel : ScarletBoss
{

    [Flags]
    public enum EelVisualEffect
    {
        None = 0,
        Zappy = 1,
        WaterTrail = 2,
        Invisible = 4,
        Mirage = 8
    }

    private Chain[] _eyeTentacles;
    private Chain[] EyeTentacles
    {
        get
        {
            if (_eyeTentacles == null)
            {
                _eyeTentacles = new Chain[3];
                for (int i = 0; i < _eyeTentacles.Length; i++)
                {
                    _eyeTentacles[i] = new Chain(NPC.Center, 2, 64);
                }
            }
            return _eyeTentacles;
        }
    }

    private Chain _hairChain2;
    private Chain HairChain2
    {
        get
        {
            if (_hairChain2 == null)
            {

                _hairChain2 = new Chain(NPC.Center, 2, 128);
            }
            return _hairChain2;
        }
    }
    private Chain _hairChain;
    private Chain HairChain
    {
        get
        {
            if (_hairChain == null)
            {

                _hairChain = new Chain(NPC.Center, 2, 128);
            }
            return _hairChain;
        }
    }

    private Chain _chain;
    private Chain Chain
    {
        get
        {
            if (_chain == null)
            {
                _chain = new Chain(NPC.Center, 100, 64);
            }
            return _chain;
        }
    }
    private struct FloatingEyeball
    {
        public Vector2 position;
        public Vector2 targetPosition;
        public Vector2 scale;
        public float rotation;
        public float speed;
    }

    private bool _inPhase2;
    private FloatingEyeball[] _eyeballs = new FloatingEyeball[3];

    private Vector2 _facingDirection;
    private Vector2 _arenaCenter;
    private Vector2 _teleportPosition;
    private Vector2 _startPosition;
    private Vector2 _initialVelocity;
    private Vector2 _eyeFlashOffset;
    private Vector2 _eatingSquishScale;
    private int _eatenPlayer;
    private float _aliveTimer;
    private float _startRotation;
    private float _bloomLineRot;
    private Color _bloomLineColor;

    private float _blackAlpha;
    private float _eyeFlashAlpha;
    private float _superCharge;
    private float _bulbCharge;
    private float _charge;

    private float _dashTrailAlpha;
    private float _mirageAlpha;
    private float _invisibleAlpha;
    private float _invisibleTimer;

    private bool _contactDamage;
    private bool _showDashTrail;
    private bool _blackedOut;
    private Outliner _outliner;
    private EelVisualEffect _effects;

    private Asset<Texture2D> _bulbGlowTextureAsset;
    private Asset<Texture2D> _eyeballTextureAsset;
    private Asset<Texture2D> _pupilTextureAsset;
    private Asset<Texture2D> _eyebrowTextureAsset;
    private Asset<Texture2D>[] _eyeTextureAssets;
    private Asset<Texture2D>[] _segmentTextureAssets;
    private Asset<Texture2D>[] _segmentGlowTextureAssets;
    private enum AIState
    {
        //First let's break this down, and get all the states that we need
        //Then we can figure out which systems and projectiles we need
        //Solve smaller problems until the whole is complete
        SpawnIntro,
        Despawn,

        Idle,
        Death,

        S_Dash,
        Lightning_Crawl,
        Ball_Bouncer,
        Chomp,
        Lightning_Wiggle,
        Suck,
        Suck_V2,

        Phase_Transition,
        Tesla_Coil,
        Overcharge,
        Eyeline_Dash,
        Swallow

    }
    private int _frame;
    private float _rotationDir;
    private bool _firstAttack;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];

    private AIState _attackToTest;
    private PatternManager<AIState> _patternManagerBackingField;
    private PatternManager<AIState> PatternManager
    {
        get
        {
            if (_patternManagerBackingField == null)
            {
                _patternManagerBackingField = new PatternManager<AIState>();
                _patternManagerBackingField.AddPattern(AIState.Lightning_Crawl, 1.0f);
                _patternManagerBackingField.AddPattern(AIState.S_Dash, 1.0f);
                _patternManagerBackingField.AddPattern(AIState.Ball_Bouncer, 1.0f);
                _patternManagerBackingField.AddPattern(AIState.Chomp, 1.0f);
                _patternManagerBackingField.AddPattern(AIState.Lightning_Wiggle, 1.5f);
                _patternManagerBackingField.AddPattern(AIState.Overcharge, 1.0f);
                _patternManagerBackingField.AddPattern(AIState.Eyeline_Dash, 1.0f);
                _patternManagerBackingField.AddPattern(AIState.Suck, 1.0f);
            }
            return _patternManagerBackingField;
        }
    }

    private bool InPhase2 => NPC.life < NPC.lifeMax * 0.5f;

    #region Damage values
    private int Electric_Rock_Damage => 30;
    private int Super_Zap_Damage => 55;
    private int Bite_Damage => 35;
    private int Lightning_Crawl_Damage => 80;
    private int Bouncing_Ball_Damage => 40;
    private int Sin_Electric_Shock_Damage => 35;
    private int Suck_Damage => 45;

    #endregion

    private float IdleTime => 360;
    private float SDashReadyTime => 120;
    private float SDashChargeTime => 24;
    private float SDashSpeed => 55;
    private float SChompSpeed => 55;

    public Vector2 BulbPosition
    {
        get
        {
            Vector2 chargePos = NPC.Center + new Vector2(90, -64).RotatedBy(NPC.rotation);
            return chargePos;
        }
    }
    public Vector2 EyeFlashPosition
    {
        get
        {
            Vector2 chargePos = NPC.Center + new Vector2(16, -16).RotatedBy(NPC.rotation);
            return chargePos;
        }
    }


    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 64;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.MustAlwaysDraw[Type] = true;
        NPCID.Sets.DoesntDespawnToInactivityAndCountsNPCSlots[Type] = true;

        Main.npcFrameCount[Type] = 5;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 128;
        NPC.height = 128;
        NPC.lifeMax = 7000;
        NPC.defense = 18;
        NPC.damage = 90;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.knockBackResist = 0f;

        NPC.npcSlots = 30f;

        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/LeviathanEel");
        NPC.HitSound = SoundID.NPCHit1 with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        //  NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
    }


    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_teleportPosition);
        writer.Write(_rotationDir);
        writer.WriteVector2(_startPosition);
        writer.WriteVector2(_initialVelocity);
        writer.Write(_startRotation);
        writer.Write(_eatenPlayer);

    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _teleportPosition = reader.ReadVector2();
        _rotationDir = reader.ReadSingle();
        _startPosition = reader.ReadVector2();
        _initialVelocity = reader.ReadVector2();
        _startRotation = reader.ReadSingle();
        _eatenPlayer = reader.ReadInt32();
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    private void MoveAndSinToward(Vector2 directionToTarget, float speed)
    {
        float distance = 6;
        Vector2 initialSpeed = directionToTarget * speed;
        Vector2 offset = initialSpeed.RotatedBy(Math.PI / 2);
        offset.Normalize();
        offset *= (float)(Math.Cos(Timer * 3 * (Math.PI / 180)) * (distance / 3));

        Vector2 targetVelocity = initialSpeed + offset;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.04f);
    }

    private bool IsBanned(AIState state)
    {
        if (!InPhase2)
        {
            switch (state)
            {
                case AIState.Overcharge:
                case AIState.Eyeline_Dash:
                    return true;
            }
        }
        else
        {
            switch (state)
            {
                case AIState.S_Dash:
                    return true;
            }
        }

        return false;
    }

    private void ForceMusicOn()
    {
        ref float musicVolume = ref Main.musicFade[Music];
        musicVolume = 1f;

    }

    public override void AI()
    {
        base.AI();
        if (_arenaCenter == Vector2.Zero)
            _arenaCenter = MyTarget.Center;

        _aliveTimer++;
        if (_aliveTimer >= 120)
            ForceMusicOn();
        _effects = EelVisualEffect.None;
        _contactDamage = false;
        _showDashTrail = false;
        if (_teleportPosition != Vector2.Zero)
        {
            NPC.Center = _teleportPosition;
            for (int i = 0; i < Chain.points.Length; i++)
            {
                Chain.points[i] = _teleportPosition;
            }
            for (int i = 0; i < HairChain.points.Length; i++)
            {
                HairChain.points[i] = _teleportPosition;
            }
            for (int i = 0; i < HairChain2.points.Length; i++)
            {
                HairChain2.points[i] = _teleportPosition;
            }
            for (int i = 0; i < _eyeballs.Length; i++)
            {
                _eyeballs[i].position = _teleportPosition;
            }
            NPC.velocity = Vector2.Zero;
            _teleportPosition = Vector2.Zero;

        }

        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget && State != AIState.Despawn)
            {
                SwitchState(AIState.Despawn);
            }
        }

        _outliner.SetDefaults();
        _charge = MathHelper.Lerp(_charge, 0f, 0.005f);
        _bulbCharge = MathHelper.Lerp(_bulbCharge, 0f, 0.005f);
        _superCharge = MathHelper.Lerp(_superCharge, 0f, 0.005f);
        _eyeFlashAlpha = MathHelper.Lerp(_eyeFlashAlpha, 0f, 0.1f);
        _blackAlpha = MathHelper.Lerp(_blackAlpha, 0f, 0.1f);
        _bloomLineColor = Color.Lerp(_bloomLineColor, Color.Transparent, 0.1f);

        HoldEyesInFrontOfMe();
        _eyeFlashOffset = Vector2.Zero;
        _eatingSquishScale = Vector2.Lerp(_eatingSquishScale, Vector2.One, 0.1f);
        switch (State)
        {
            case AIState.Despawn:
                AI_Despawn();
                break;
            case AIState.SpawnIntro:
                AI_SpawnIntro();
                break;
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.S_Dash:
                AI_SDash();
                break;
            case AIState.Lightning_Crawl:
                AI_LightningCrawl();
                break;
            case AIState.Ball_Bouncer:
                AI_BallBouncer();
                break;
            case AIState.Chomp:
                AI_Chomp();
                break;
            case AIState.Lightning_Wiggle:
                AI_LightningWiggle();
                break;
            case AIState.Suck:
                AI_Suck();
                break;
            case AIState.Eyeline_Dash:
                AI_EyelineDash();
                break;
            case AIState.Tesla_Coil:
                AI_TeslaCoil();
                break;
            case AIState.Overcharge:
                AI_Overcharge();
                break;
            case AIState.Phase_Transition:
                AI_PhaseTransition();
                break;
        }
        _outliner.Update();

        float targetMirageAlpha = _effects.HasFlag(EelVisualEffect.Mirage) ? 1f : 0f;
        _mirageAlpha = MathHelper.Lerp(_mirageAlpha, targetMirageAlpha, 0.1f);


        float targetInvisibleAlpha = _effects.HasFlag(EelVisualEffect.Invisible) ? -1f : 1f;
        _invisibleTimer += targetInvisibleAlpha;
        _invisibleTimer = MathHelper.Clamp(_invisibleTimer, 0f, 40f);
        _invisibleAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.Clamp(_invisibleTimer/30f));

        float targetDashTrailAlpha = _showDashTrail ? 1f : 0f;
        _dashTrailAlpha = MathHelper.Lerp(_dashTrailAlpha, targetDashTrailAlpha, 0.1f);
        Chain.pinned[0] = true;
        Chain.points[0] = NPC.Center;
        Chain.ResolveRootToBack();

        NPC.spriteDirection = 1;
        float facingRotation = _facingDirection.ToRotation();
        NPC.rotation = facingRotation;

        SimulateHair();
        SimulateEyes();
    }

    private void HoldEyesInFrontOfMe()
    {
        for (int i = 0; i < _eyeballs.Length; i++)
        {
            ref FloatingEyeball eyeball = ref _eyeballs[i];
            Vector2 offset = -Vector2.UnitY * 64;
            offset = offset.RotatedBy(i / (float)_eyeballs.Length * MathHelper.TwoPi + Main.GlobalTimeWrappedHourly * 2);
            eyeball.targetPosition = NPC.Center + offset;
        }
    }

    private void SimulateEyes()
    {
        for (int i = 0; i < _eyeballs.Length; i++)
        {
            ref FloatingEyeball eyeball = ref _eyeballs[i];
            eyeball.speed = 64;
            eyeball.position = eyeball.position.MoveTowards(eyeball.targetPosition, eyeball.speed);
            EyeTentacles[i].pinned[0] = true;
            EyeTentacles[i].points[0] = eyeball.position;
            EyeTentacles[i].ResolveRootToBack();
        }
    }

    private void CloseMouth()
    {
        NPC.frameCounter += 0.15f;
        if (NPC.frameCounter >= 1f)
        {
            _frame--;
            NPC.frameCounter = 0;
        }
        if (_frame <= 0)
        {
            _frame = 0;
        }
    }
    private void OpenMouth()
    {
        NPC.frameCounter += 0.15f;
        if (NPC.frameCounter >= 1f)
        {
            _frame++;
            NPC.frameCounter = 0;
        }
        if (_frame >= 4)
        {
            _frame = 4;
        }
    }
    private void AnimateMouthBasedOnDistance()
    {
        float distanceToTarget = Vector2.Distance(NPC.Center, MyTarget.Center);
        float progress = distanceToTarget / 600f;
        progress = EasingFunction.InSine(progress);
        float ratio = 1f - progress;
        _frame = (int)MathHelper.Lerp(0, 5, ratio);
    }
    private void Teleport(Vector2 teleportPosition)
    {
        if (!MultiplayerHelper.IsHost)
            return;

        _teleportPosition = teleportPosition;
        NPC.netUpdate = true;
    }


    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            State = state;
            Timer = 0;
            AttackCycle = 0;
            AttackCounter = 0;
            NPC.netUpdate = true;
        }
    }

    private bool AboveTheSand()
    {
        Point point = NPC.Center.ToTileCoordinates();
        if (!WorldGen.InWorld(point.X, point.Y))
            return false;
        Tile tile = Main.tile[point];
        return !tile.HasTile && NPC.Bottom.Y < MyTarget.Bottom.Y;
    }

    private void CreateInwardElectricRocks()
    {
        if (!MultiplayerHelper.IsHost)
            return;
        if (!Main.rand.NextBool(20))
            return;

        Vector2 pos = MyTarget.Center;
        Vector2 offset = -Vector2.UnitY * 1500;
        offset = offset.RotatedByRandom(MathHelper.TwoPi);
        pos += offset;

        Vector2 vel = MyTarget.Center - pos;
        vel = vel.SafeNormalize(Vector2.Zero);
        vel *= Main.rand.NextFloat(7, 10);
        Projectile.NewProjectile(SourceFromThis, pos, vel, ModContent.ProjectileType<LeviathanElectricRock>(), Electric_Rock_Damage, 1, Main.myPlayer);
    }

    private Vector2 FindArenaCenter() => TileUtilities.GuessArenaCenter(MyTarget.Center);
    private bool BelowTheSand()
    {
        Point point = NPC.Center.ToTileCoordinates();
        if (!WorldGen.InWorld(point.X, point.Y))
            return false;
        Tile tile = Main.tile[point];
        return !tile.HasTile && NPC.Bottom.Y + 300 > FindArenaCenter().Y;
    }

    private void DiveOutToIdle()
    {
        float outTime = 140f;
        float halfTime = outTime / 2f;
        _effects |= EelVisualEffect.Mirage;
        if (Timer >= halfTime)
            _effects |= EelVisualEffect.Invisible;

        float speed = MathHelper.Lerp(0f, 45, EasingFunction.InExpo(Timer / halfTime));
        MoveAndSinToward(Vector2.UnitY, speed);
        FaceVelocity();
        if (Timer >= outTime)
        {
            SwitchState(AIState.Idle);
        }
    }
    private void DiveOutNextState()
    {
        float time = 280;
        float halfTime = time / 2f;
        _effects |= EelVisualEffect.Mirage;
        if (Timer >= halfTime)
            _effects |= EelVisualEffect.Invisible;
        // NPC.velocity.X *= 0.98f;
        // NPC.velocity.Y += 0.5f;
        float speed = MathHelper.Lerp(0f, 45, EasingFunction.InExpo(Timer / 100f));
        MoveAndSinToward(Vector2.UnitY, speed);
        FaceVelocity();
        if (Timer >= 280)
        {
            Timer = 0;
            AttackCycle++;
        }
    }
    private void DiveOutNextStateFast()
    {
        _effects |= EelVisualEffect.Invisible;
        _effects |= EelVisualEffect.Mirage;
        // NPC.velocity.X *= 0.98f;
        // NPC.velocity.Y += 0.5f;
        float speed = MathHelper.Lerp(0f, 45, EasingFunction.InExpo(Timer / 50f));
        MoveAndSinToward(Vector2.UnitY, speed);
        FaceVelocity();
        if (Timer >= 140f)
        {
            Timer = 0;
            AttackCycle++;
        }
    }

    #region Phase 1 Attacks
    private void AI_Suck()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                    }
                    DiveOutNextState();
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        NPC.velocity = Vector2.Zero;
                        _arenaCenter = MyTarget.Center - new Vector2(0, 128);
                        Teleport(MyTarget.Center - new Vector2(1400, 0));
                    }

                    float speed = MathHelper.Lerp(24, 4f, EasingFunction.InOutSine(Timer / 60f));
                    MoveAndSinToward(Vector2.UnitX, speed);
                    FaceVelocity();
                    if (Timer >= 60f)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    //Slowly inch up to target
                    Vector2 positionToMoveTo = MyTarget.Center;
                    float distanceToTarget = Vector2.Distance(positionToMoveTo, NPC.Center);
                    Vector2 targetVector = (positionToMoveTo - NPC.Center).RotatedBy(0.05f);
                    targetVector = targetVector.SafeNormalize(Vector2.Zero);
                    MoveAndSinToward(targetVector, MathHelper.Lerp(8f, 16, EasingFunction.InOutSine(Timer / 120f)));
                    FaceVelocity();
                    _outliner.warning = true;
                    if (distanceToTarget < 512)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    _outliner.warning = true;
                    OpenMouth();
                    NPC.velocity *= 0.97f;
                    if (Timer >= 60f)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    OpenMouth();
                    if (Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero,
                                ModContent.ProjectileType<LeviathanEelSuck>(), Suck_Damage, 1, Main.myPlayer, ai1: NPC.whoAmI);
                        }
                    }

                    float distanceToTarget = Vector2.Distance(NPC.Center, MyTarget.Center);
                    if (distanceToTarget < 32)
                    {
                        Timer = 0;
                        AttackCycle = 7;
                        _eatenPlayer = MyTarget.whoAmI;
                    }

                    Vector2 directionToTarget = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    _facingDirection = Vector2.Lerp(_facingDirection, directionToTarget, 0.2f);
                    _charge = MathHelper.Lerp(0f, 1f, Timer / 480f);
                    _contactDamage = false;
                    _outliner.attacking = true;
                    if (NPC.velocity.Length() > 1)
                        NPC.velocity *= 0.94f;
                    NPC.velocity += (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.1f;
                    if (Timer >= 480)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 5:
                {
                    NPC.velocity *= 0.88f;
                    CloseMouth();
                    if (Timer >= 120)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 6:
                {
                    CloseMouth();
                    DiveOutToIdle();
                }
                break;
            case 7:
                {
                    _charge = 1f;
                    CloseMouth();
                    NPC.velocity *= 0.94f;
                    foreach (var proj in Main.ActiveProjectiles)
                    {
                        if (proj.type != ModContent.ProjectileType<LeviathanEelSuck>())
                            continue;
                        if (proj.timeLeft > 30)
                            proj.timeLeft = 30;
                    }
                    _contactDamage = true;
                    _outliner.attacking = true;
                    _eatingSquishScale = Vector2.Lerp(Vector2.One, new Vector2(1.2f, 0.9f), ExtraMath.Osc(0f, 1f, speed: 12));
                    Player player = Main.player[_eatenPlayer];
                    MovePlayer movePlayer = player.GetModPlayer<MovePlayer>();
                    movePlayer.eatenDelayTimer = 15;
                    movePlayer.overrideVelocity = NPC.Center - player.Center;
                    if (player.dead || Timer >= 280)
                    {
                        Timer = 0;
                        AttackCycle = 6;
                    }
                }
                break;
        }
    }
    private void AI_LightningWiggle()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();

                    }

                    NPC.velocity = NPC.velocity.RotatedBy(-0.05f);
                    FaceVelocity();
                    if (Timer >= 60f)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    //Slowly inch up to target
                    Vector2 positionToMoveTo = MyTarget.Center;
                    float distanceToTarget = Vector2.Distance(positionToMoveTo, NPC.Center);
                    Vector2 targetVector = (positionToMoveTo - NPC.Center).RotatedBy(0.05f);
                    targetVector = targetVector.SafeNormalize(Vector2.Zero);
                    MoveAndSinToward(targetVector, MathHelper.Lerp(8f, 16, EasingFunction.InOutSine(Timer / 120f)));
                    FaceVelocity();
                    if (distanceToTarget < 384)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    NPC.velocity *= 0.88f;

                    _outliner.warning = true;
                    _charge = MathHelper.Lerp(0f, 1f, Timer / 100f);
                    _bulbCharge = _charge;
                    if (Timer >= 100)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    _effects |= EelVisualEffect.Mirage;
                    void Spawn(int index, int offset)
                    {
                        int combinedIndex = index + offset;
                        combinedIndex %= Chain.points.Length;
                        Vector2 pos = Chain.points[combinedIndex];
                        Projectile.NewProjectile(SourceFromThis, pos, Vector2.Zero, ModContent.ProjectileType<SinElectricShock>(), Sin_Electric_Shock_Damage, 1, Main.myPlayer);
                    }

                    bool IsAboutToMakeOne()
                    {
                        int a = (int)AttackCounter + 6;
                        int distanceBtween = 16;
                        for (int i = 0; i < 3; i++)
                        {
                            int combinedIndex = a + i * distanceBtween;
                            combinedIndex %= Chain.points.Length;
                            if (combinedIndex == 0)
                            {
                                return true;
                            }
                            //    Spawn(a, i * distanceBtween);
                        }
                        return false;
                    }

                    if (IsAboutToMakeOne() && Timer % 4 == 0)
                    {
                        PixelPrimitiveCircleFactory.CreateEelSiningSuck(NPC);
                    }
                    if (Timer % 4 == 0 && MultiplayerHelper.IsHost)
                    {
                        int a = (int)AttackCounter;
                        int distanceBtween = 16;
                        for (int i = 0; i < 3; i++)
                        {
                            Spawn(a, i * distanceBtween);
                        }

                        AttackCounter++;
                        AttackCounter %= Chain.points.Length;
                    }

                    _bulbCharge = 1f;
                    _charge = MathHelper.Lerp(0.8f, 1f, ExtraMath.Osc(0f, 1f, speed: 16));
                    if (NPC.velocity.Length() < 15)
                        NPC.velocity *= 1.05f;
                    if (NPC.velocity.Length() < 1)
                        NPC.velocity += (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.2f;
                    Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(NPC.Center, MyTarget.Center, NPC.velocity, degreesToRotate: 2);
                    NPC.velocity = homingVelocity;
                    FaceVelocity();
                    _showDashTrail = true;

                    _outliner.attacking = true;
                    _charge = 1f;
                    if (Timer >= 600f)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    DiveOutToIdle();
                }
                break;

        }
    }
    private void AI_Chomp()
    {
        Timer++;


        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                    }
                    DiveOutNextState();
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        Teleport(MyTarget.Center + new Vector2(0, 1300).RotatedByRandom(MathHelper.TwoPi));
                    }

                    NPC.velocity = Vector2.Zero;
                    Vector2 directionToTarget = (NPC.Center - MyTarget.Center).SafeNormalize(Vector2.Zero);
                    if (Timer % 2 == 0)
                    {
                        Vector2 pos = MyTarget.Center + directionToTarget * 1024;
                        pos += Main.rand.NextVector2Circular(64, 64);
                        var bp = BubbleParticle.Spawn(pos, -directionToTarget * 48);
                        bp.Scale *= Main.rand.NextFloat(0.3f, 0.6f);
                        bp.gravity = 0;

                    }
                    _effects |= EelVisualEffect.Invisible;
                    _effects |= EelVisualEffect.Mirage;
                    _outliner.warning = true;
                    if (Timer >= 40)
                    {

                    }
                    if (Timer >= 60)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    Vector2 directionToMe = (NPC.Center - MyTarget.Center).SafeNormalize(Vector2.Zero);
                    Vector2 newPos = MyTarget.Center + directionToMe * 256;
                    _eyeFlashOffset = newPos - NPC.Center;

                    _eyeFlashAlpha = MathHelper.Lerp(1f, 0f, EasingFunction.InExpo(Timer / SDashChargeTime));
                    AnimateMouthBasedOnDistance();
                    if (Timer == 1)
                    {
                        _initialVelocity = NPC.velocity;
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, NPC.Center + _facingDirection * 128, _facingDirection,
                                ModContent.ProjectileType<EelSpeedTrail>(), Bite_Damage, 1, Main.myPlayer, ai0: NPC.whoAmI);
                            Projectile.NewProjectile(SourceFromThis, NPC.Center + _facingDirection * 128, _facingDirection,
                                ModContent.ProjectileType<LeviathanBite>(), Bite_Damage, 1, Main.myPlayer, ai1: NPC.whoAmI, ai2: 1);
                        }
                    }

                    if (Timer < 5)
                    {
                        _startPosition = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * SChompSpeed;
                    }

                    if (Timer % 5 == 0)
                    {
                        var dp = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity * 0.5f);
                        dp.Scale *= 2f;
                    }
                    if (Timer % 5 == 0)
                    {
                        var dp = LegacyParticle.NewParticle<ZapParticle>(NPC.Center, -NPC.velocity * 0.5f);
                        dp.Scale *= 2f;
                        dp.innerColor = Color.LightBlue;
                        dp.outerColor = Color.Blue;
                        dp.fadeToColor = Color.DarkBlue;
                    }
                    _charge = MathHelper.Lerp(_charge, 1f, 0.02f);
                    _showDashTrail = true;

                    float ratio = Timer / (SDashChargeTime / 3f);
                    float ease2 = EasingFunction.InExpo(ratio);
                    Vector2 easeVelocity = Vector2.Lerp(_initialVelocity, _startPosition, ease2);
                    NPC.velocity = easeVelocity;
                    FaceVelocity();

                    _contactDamage = true;
                    _outliner.attacking = true;
                    if (Timer >= SDashChargeTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    AnimateMouthBasedOnDistance();
                    _effects |= EelVisualEffect.Mirage;
                    _effects |= EelVisualEffect.Invisible;
                    NPC.velocity = NPC.velocity.RotatedBy(0.05f);
                    NPC.velocity *= 0.99f;
                    FaceVelocity();
                    _outliner.attacking = true;
                    if (Timer % 5 == 0)
                    {
                        var dp = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity * 0.5f);
                        dp.Scale *= 2f;
                    }
                    if (Timer >= 80)
                    {
                        Timer = 0;
                        AttackCounter++;
                        if (AttackCounter < 7)
                        {
                            AttackCycle = 1;
                        }
                        else
                        {
                            AttackCycle++;

                        }


                    }
                }
                break;
            case 4:
                {
                    DiveOutToIdle();
                }
                break;
        }
    }
    private void AI_BallBouncer()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    DiveOutNextState();
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        Teleport(FindArenaCenter() + new Vector2(0, -1400));
                    }

                    NPC.velocity.X *= 0.8f;
                    if (NPC.velocity.Y < 12)
                        NPC.velocity.Y += 0.25f;
                    FaceVelocity();
                    if (BelowTheSand())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    OpenMouth();
                    if (NPC.velocity.Length() > 1)
                    {
                        NPC.velocity *= 0.88f;
                    }

                    _outliner.warning = true;
                    if (Timer > 60)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    _outliner.warning = true;
                    if (Timer == 1 && MultiplayerHelper.IsHost)
                    {
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.UnitY, ModContent.ProjectileType<LeviathanEelSuck>(), Suck_Damage, 1, Main.myPlayer, ai1: NPC.whoAmI, ai2: 1);
                    }
                    if (Timer >= 150)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    CloseMouth();

                    NPC.velocity = Vector2.Lerp(_facingDirection * 2, -_facingDirection * 7, EasingFunction.QuadraticBump(Timer / 60));
                    _charge = MathHelper.Lerp(_charge, 1f, 0.04f);
                    _outliner.warning = true;
                    _eyeFlashAlpha = MathHelper.Lerp(1f, 0f, Timer / 30f);
                    if (Timer >= 60)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 5:
                {
                    OpenMouth();
                    if (Timer == 1)
                    {
                        NPC.velocity += _facingDirection * 15;
                    }
                    if (Timer == 1 && MultiplayerHelper.IsHost)
                    {
                        Vector2 spitVelocity = Vector2.UnitY * 20;
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, spitVelocity,
                            ModContent.ProjectileType<BouncingBallCore>(), Bouncing_Ball_Damage, 1, Main.myPlayer, ai1: 1);
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, spitVelocity,
                         ModContent.ProjectileType<BouncingBallCore>(), Bouncing_Ball_Damage, 1, Main.myPlayer, ai1: 2);
                    }

                    _outliner.attacking = true;
                    NPC.velocity *= 0.94f;
                    if (Timer >= 60)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 6:
                {
                    CloseMouth();
                    DiveOutToIdle();
                }
                break;
        }
    }
    private void AI_LightningCrawl()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                    }
                    DiveOutNextState();
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        Teleport(MyTarget.Center + new Vector2(-1200, 666));

                    }

                    NPC.velocity.X += (MyTarget.Center.X > NPC.Center.X) ? 0.2f : -0.2f;
                    if (NPC.velocity.Y > -12)
                        NPC.velocity.Y -= 0.5f;
                    FaceVelocity();
                    if (AboveTheSand())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    if (NPC.velocity.Length() > 16)
                    {
                        NPC.velocity *= 0.97f;
                    }
                    else
                    {
                        Vector2 targetDirectionToSin = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                        MoveAndSinToward(targetDirectionToSin, MathHelper.Lerp(8f, 16, EasingFunction.InOutSine(Timer / 120f)));
                        FaceVelocity();
                    }

                    bool closeEnoughToPlayer = Vector2.Distance(NPC.Center, MyTarget.Center) < 500;
                    if (closeEnoughToPlayer)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    _charge = MathHelper.Lerp(0, 1f, Timer / 180f);
                    if (Timer % 2 == 0)
                    {
                        float range = Main.rand.NextFloat(252, 512);
                        Vector2 pos = BulbPosition + Main.rand.NextVector2CircularEdge(range, range);
                        Vector2 vel = (BulbPosition - pos);
                        vel *= 0.1f;
                        FXUtil.GlowStretch(pos, vel);
                    }

                    if (Timer % 2 == 0)
                    {
                        float range = Main.rand.NextFloat(384, 666);
                        Vector2 pos = BulbPosition + Main.rand.NextVector2CircularEdge(range, range);
                        Vector2 vel = (BulbPosition - pos);
                        vel *= 0.1f;
                        var fx = FXUtil.GlowStretch(pos, vel);
                        fx.OuterGlowColor = Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat(0f, 1f));
                        fx.VectorScale *= 0.5f;
                    }
                    if (Timer % 6 == 0)
                    {
                        var zap = ElectricZapParticle.Spawn(BulbPosition + Main.rand.NextVector2Circular(64, 64), Main.rand.NextVector2Circular(2, 2),
                            Scale: Main.rand.NextFloat(0.3f, 0.6f) * Timer / 240f);
                    }


                    if (Timer % 60 == 0 && Timer <= 236 && Timer > 2)
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            ScreenShaderSystem tintSystem = ModContent.GetInstance<ScreenShaderSystem>();
                            tintSystem.TintScreen(Color.Blue, 0.05f, 15f);
                            PixelPrimitiveCircleFactory.CreateClosingGustCircle(BulbPosition);
                            PixelPrimitiveCircleFactory.CreateEelInSuck(BulbPosition);
                        }
                        string path = $"Stellamod/Assets/Sounds/Dreadmire__LightingRain{AttackCounter + 1}";
                        SoundStyle sound = new SoundStyle(path) with { PitchVariance = 0.3f };
                        SoundEngine.PlaySound(sound, NPC.position);
                        FXUtil.GlowCircleBoom(BulbPosition, Color.White, Color.LightBlue, Color.DarkBlue);
                        AttackCounter++;
                    }
                    _bulbCharge = MathHelper.SmoothStep(0f, 1f, EasingFunction.InOutSine(Timer / 240f));
                    ShakeScreenPosition.Shake = MathHelper.Lerp(0f, 2f, Timer / 240f);
                    CreateInwardElectricRocks();

                    if (NPC.velocity.Length() > 1)
                        NPC.velocity *= 0.94f;
                    FaceVelocity();
                    if (Timer >= 300)
                    {
                        Timer = 0;
                        AttackCycle++;
                        AttackCounter = 0;
                    }
                }
                break;
            case 4:
                {
                    _bulbCharge = 1f;
                    if (Timer % 6 == 0)
                    {
                        var zap = ElectricZapParticle.Spawn(BulbPosition + Main.rand.NextVector2Circular(64, 64), Main.rand.NextVector2Circular(2, 2),
                            Scale: Main.rand.NextFloat(0.3f, 0.6f) * Timer / 240f);
                    }

                    DiveOutNextStateFast();
                }
                break;
            case 5:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        NPC.velocity = Vector2.Zero;
                        _arenaCenter = MyTarget.Center - new Vector2(0, 128);
                        Teleport(FindArenaCenter() - new Vector2(1400, 0).RotatedByRandom(MathHelper.TwoPi));
                    }

                    float speed = MathHelper.Lerp(24, 4f, EasingFunction.InSine(Timer / 100f));
                    Vector2 dir = (FindArenaCenter() - NPC.Center).SafeNormalize(Vector2.Zero);
                    MoveAndSinToward(dir, speed);
                    FaceVelocity();
                    _bulbCharge = 1f;
                    _outliner.warning = true;
                    if (Timer % 6 == 0)
                    {
                        var zap = ElectricZapParticle.Spawn(BulbPosition + Main.rand.NextVector2Circular(64, 64), Main.rand.NextVector2Circular(2, 2),
                            Scale: Main.rand.NextFloat(0.3f, 0.6f) * Timer / 240f);
                    }
                    if (Timer >= 100f)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 6:
                {
                    NPC.velocity *= 0.96f;
                    if (Timer >= 30)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 7:
                {
                    _bulbCharge = 1f;
                    if (Timer == 1)
                    {
                        Vector2 offsetToPlayer = (MyTarget.Center - NPC.Center);
                        float rot = offsetToPlayer.ToRotation();
                        float diff = rot - NPC.rotation;
                        float diff2 = MathHelper.TwoPi - diff;
                        _rotationDir = diff < diff2 ? 1 : -1;
                        _startRotation = NPC.rotation;
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, _facingDirection,
                                ModContent.ProjectileType<LightningCrawl>(), Lightning_Crawl_Damage, 1, Main.myPlayer, ai1: NPC.whoAmI);
                        }
                    }
                    if (Timer % 6 == 0)
                    {
                        var zap = ElectricZapParticle.Spawn(BulbPosition + Main.rand.NextVector2Circular(64, 64), Main.rand.NextVector2Circular(2, 2),
                            Scale: Main.rand.NextFloat(0.3f, 0.6f) * Timer / 240f);
                    }

                    _charge = 1f;
                    _outliner.attacking = true;
                    _bloomLineColor = Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(Timer / 90f));



                    float endRotation = _startRotation + MathHelper.ToRadians(200 * _rotationDir);
                    float ratio = Timer / 180f;
                    float easing = EasingFunction.InOutExpo(ratio);
                    float newRotation = MathHelper.Lerp(_startRotation, endRotation, easing);

                    _bloomLineRot = MathHelper.Lerp(_startRotation, endRotation, EasingFunction.InOutExpo(Timer / 90f));
                    _facingDirection = newRotation.ToRotationVector2();
                    NPC.velocity *= 0.96f;
                    if (Timer >= 180f)
                    {
                        Timer = 0;
                        AttackCycle++;
                        AttackCounter++;
                    }
                }
                break;
            case 8:
                {
                    _bulbCharge = 1f;
                    NPC.velocity *= 0.94f;
                    //  FaceVelocity();
                    if (Timer >= 30)
                    {
                        if (AttackCounter >= 3)
                        {
                            Timer = 0;
                            AttackCycle++;
                        }
                        else
                        {
                            Timer = 0;
                            AttackCycle = 4;
                        }
                    }
                }
                break;
            case 9:
                {
                    DiveOutToIdle();
                }
                break;
        }
    }
    private void AI_SDash()
    {
        Timer++;
        AnimateMouthBasedOnDistance();
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();

                    }

                    NPC.velocity = NPC.velocity.RotatedBy(0.05f);
                    FaceVelocity();
                    if (Timer >= 60f)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        _startPosition = NPC.Center;
                        _initialVelocity = NPC.velocity;
                    }


                    _effects |= EelVisualEffect.Mirage;
                    //Slowly inch up to target
                    Vector2 positionToMoveTo = MyTarget.Center;
                    float distanceToTarget = Vector2.Distance(positionToMoveTo, NPC.Center);
                    Vector2 targetVector = (positionToMoveTo - NPC.Center).RotatedBy(0.05f);
                    targetVector = targetVector.SafeNormalize(Vector2.Zero);
                    MoveAndSinToward(targetVector, MathHelper.Lerp(8f, 16, EasingFunction.InOutSine(Timer / 120f)));
                    FaceVelocity();


                    _charge = MathHelper.Lerp(1f, 0f, distanceToTarget / 384f);
                    //   _outliner.warning = true;
                    if (distanceToTarget < 384)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    if (Timer == 1)
                    {

                    }
                    NPC.velocity *= 0.92f;
                    _effects |= EelVisualEffect.Mirage;

                    // FaceVelocity();

                    _charge = MathHelper.Lerp(_charge, 1f, 0.04f);
                    //  _outliner.warning = true;
                    if (Timer >= 30)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    NPC.velocity = Vector2.Lerp(_facingDirection * 2, -_facingDirection * 7, EasingFunction.QuadraticBump(Timer / 30f));
                    _charge = MathHelper.Lerp(_charge, 1f, 0.04f);
                    _outliner.warning = true;

                    if (Timer == 15)
                    {
                        if (MultiplayerHelper.IsHost)
                        {

                            Projectile.NewProjectile(SourceFromThis, NPC.Center + _facingDirection * 128, _facingDirection,
                                ModContent.ProjectileType<LeviathanBite>(), Bite_Damage, 1, Main.myPlayer, ai1: NPC.whoAmI, ai2: 2);
                        }
                    }

                    _eyeFlashAlpha = MathHelper.Lerp(1f, 0f, Timer / 30f);
                    if (Timer >= 30)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    if (Timer == 1)
                    {
                        _initialVelocity = NPC.velocity;

                    }

                    if (Timer < 5)
                    {
                        _startPosition = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * SDashSpeed;
                    }
                    if (Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero,
                                ModContent.ProjectileType<EelSpeedTrail>(), Bite_Damage, 1, Main.myPlayer, ai0: NPC.whoAmI);
                        }
                    }

                    if (Timer % 5 == 0)
                    {
                        var dp = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity * 0.5f);
                        dp.Scale *= 2f;
                    }
                    if (Timer % 5 == 0)
                    {
                        var dp = LegacyParticle.NewParticle<ZapParticle>(NPC.Center, -NPC.velocity * 0.5f);
                        dp.Scale *= 2f;
                        dp.innerColor = Color.LightBlue;
                        dp.outerColor = Color.Blue;
                        dp.fadeToColor = Color.DarkBlue;
                    }
                    _charge = MathHelper.Lerp(_charge, 1f, 0.02f);
                    _showDashTrail = true;

                    float ratio = Timer / (SDashChargeTime / 3f);
                    float ease2 = EasingFunction.InExpo(ratio);
                    Vector2 easeVelocity = Vector2.Lerp(_initialVelocity, _startPosition, ease2);
                    NPC.velocity = easeVelocity;
                    FaceVelocity();

                    _contactDamage = true;
                    _outliner.attacking = true;
                    if (Timer >= SDashChargeTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 5:
                {
                    _effects |= EelVisualEffect.Mirage;
                    NPC.velocity = NPC.velocity.RotatedBy(0.05f);
                    NPC.velocity *= 0.99f;
                    FaceVelocity();

                    //   _outliner.warning = true;
                    if (Timer >= 15)
                    {
                        Timer = 0;
                        AttackCounter++;
                        if (AttackCounter < 3)
                        {
                            AttackCycle = 1;
                        }
                        else
                        {
                            AttackCycle++;

                        }


                    }
                }
                break;
            case 6:
                {
                    DiveOutToIdle();
                }
                break;
        }
    }
    #endregion

    #region Phase 2 Attacks
    private void AI_PhaseTransition()
    {

        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    NPC.velocity *= 0.8f;
                    _blackAlpha = MathHelper.Lerp(0f, 1f, Timer / 60f);
                    if (Timer >= 60f)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        Teleport(MyTarget.Center + new Vector2(0, -1000));
                    }
                    _blackedOut = true;
                    _inPhase2 = true;
                    _blackAlpha = 1f;
                    SwitchState(AIState.Eyeline_Dash);
                }
                break;
        }
    }
    private void AI_TeslaCoil()
    {
        _inPhase2 = true;
        Timer++;

    }
    private void AI_Overcharge()
    {
        _inPhase2 = true;
        Timer++;

        //So for this attack uhh uhh
        //1. Dive out like usually
        //2. Teleport to the left of the player and come in and slow down
        //3. Start charging up, this one uses a different charge effect that uses the points on the Chain and a lightning trail effect that flickers in and out
        //4. The bulb also charges htough, but make a separate charge variable for this
        //The 3 eyes orbit in a circle and shoot electirc bolts at you while it's charging
        //5. Once full charge, the eyes go away, screen blackens for a second and several lightning bolts come out everywhere in different directions (crazy)
        //6. 
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                    }
                    DiveOutNextState();
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        Teleport(MyTarget.Center + new Vector2(-1500, 0));
                    }

                    float startupTime = 100f;
                    float speed = MathHelper.Lerp(24, 4f, EasingFunction.InOutSine(Timer / startupTime));
                    MoveAndSinToward(Vector2.UnitX, speed);
                    FaceVelocity();
                    if (Timer >= startupTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {


                    if (Timer % 2 == 0)
                    {
                        float range = Main.rand.NextFloat(252, 512);
                        Vector2 pos = BulbPosition + Main.rand.NextVector2CircularEdge(range, range);
                        Vector2 vel = (BulbPosition - pos);
                        vel *= 0.1f;
                        FXUtil.GlowStretch(pos, vel);
                    }

                    if (Timer % 2 == 0)
                    {
                        float range = Main.rand.NextFloat(384, 666);
                        Vector2 pos = BulbPosition + Main.rand.NextVector2CircularEdge(range, range);
                        Vector2 vel = (BulbPosition - pos);
                        vel *= 0.1f;
                        var fx = FXUtil.GlowStretch(pos, vel);
                        fx.OuterGlowColor = Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat(0f, 1f));
                        fx.VectorScale *= 0.5f;
                    }

                    if (Timer % 6 == 0)
                    {
                        var zap = ElectricZapParticle.Spawn(BulbPosition + Main.rand.NextVector2Circular(64, 64), Main.rand.NextVector2Circular(2, 2),
                            Scale: Main.rand.NextFloat(0.3f, 0.6f) * Timer / 240f);
                    }


                    if (Timer % 60 == 0 && Timer <= 236 && Timer > 2)
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            ScreenShaderSystem tintSystem = ModContent.GetInstance<ScreenShaderSystem>();
                            tintSystem.TintScreen(Color.Blue, 0.05f, 15f);
                            PixelPrimitiveCircleFactory.CreateClosingGustCircle(BulbPosition);
                            PixelPrimitiveCircleFactory.CreateEelInSuck(BulbPosition);
                        }
                        string path = $"Stellamod/Assets/Sounds/Dreadmire__LightingRain{AttackCounter + 1}";
                        SoundStyle sound = new SoundStyle(path) with { PitchVariance = 0.3f };
                        SoundEngine.PlaySound(sound, NPC.position);
                        FXUtil.GlowCircleBoom(BulbPosition, Color.White, Color.LightBlue, Color.DarkBlue);
                        AttackCounter++;
                    }
                    _outliner.warning = true;
                    _charge = MathHelper.Lerp(_charge, 1f, Timer / 180f);
                    _superCharge = MathHelper.SmoothStep(0f, 1f, EasingFunction.InOutSine(Timer / 240f));
                    _bulbCharge = MathHelper.SmoothStep(0f, 1f, EasingFunction.InOutSine(Timer / 240f));
                    ShakeScreenPosition.Shake = MathHelper.Lerp(0f, 2f, Timer / 240f);

                    if (NPC.velocity.Length() > 1)
                        NPC.velocity *= 0.94f;
                    FaceVelocity();
                    if (Main.netMode != NetmodeID.Server)
                    {

                        float alpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 300f));
                        _blackAlpha = alpha;
                        //Lighting.GlobalBrightness = MathHelper.Lerp(1.2f, 0f, alpha);
                    }
                    if (Timer >= 300)
                    {
                        Timer = 0;
                        AttackCycle++;
                        AttackCounter = 0;
                    }
                }
                break;
            case 3:
                {
                    SpecialEffectsPlayer effectsPlayer = Main.LocalPlayer.GetModPlayer<SpecialEffectsPlayer>();
                    effectsPlayer.darknessCurve = MathHelper.Lerp(1f, 0.2f, EasingFunction.OutExpo(Timer / 30f));
                    _charge = 1f;
                    _bulbCharge = 1f;
                    _superCharge = 1f;
                    _outliner.attacking = true;
                    if (Timer % 5 == 0 && MultiplayerHelper.IsHost && Timer < 400)
                    {
                        int index = Main.rand.Next(Chain.points.Length);

                        SortedSet<(float, Vector2)> openList =
                            new SortedSet<(float, Vector2)>(Comparer<(float, Vector2)>.Create((a, b) => a.Item1.CompareTo(b.Item1)));
                        for (int i = 0; i < Chain.points.Length; i++)
                        {
                            Vector2 potentialPoint = Chain.points[i];
                            float distToPoint = Vector2.Distance(MyTarget.Center, potentialPoint);
                            openList.Add((distToPoint, potentialPoint));
                        }

                        int rand = Main.rand.Next(12);
                        Vector2 pos = Chain.points[rand];

                        Projectile.NewProjectile(SourceFromThis, pos, Main.rand.NextVector2CircularEdge(1240, 1240),
                            ModContent.ProjectileType<SuperZap>(), Super_Zap_Damage, 1, Main.myPlayer);
                    }
                    if (Timer >= 480)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    DiveOutToIdle();
                }
                break;
        }
    }
    private void AI_EyelineDash()
    {
        _inPhase2 = true;
        Timer++;
        float speedMult = MathHelper.Lerp(1f, 0.75f, EasingFunction.Clamp(AttackCounter / 3f));
        float dashingTime = 70 * speedMult;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (_blackedOut)
                    {
                        _blackAlpha = 1f;
                    }
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                    }
                    DiveOutNextState();
                }
                break;
            case 1:
                {
                    _blackedOut = false;
                    float dashDistance = 900;
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();

                        if (MultiplayerHelper.IsHost)
                        {
                            _startPosition = MyTarget.Center - new Vector2(dashDistance, 0).RotatedByRandom(MathHelper.TwoPi);
                            Teleport(_startPosition);
                            NPC.netUpdate = true;
                        }


                    }
                    _effects |= EelVisualEffect.Invisible;

                    if (Timer < 5)
                        _eyeballs[0].position = _startPosition;

                    float warningTime = dashingTime;
                    NPC.velocity *= 0.8f;
                    Vector2 direction = (MyTarget.Center - _startPosition).SafeNormalize(Vector2.Zero);
                    Vector2 endPosition = _startPosition + direction * dashDistance * 2;

                    float ratio = EasingFunction.InOutSine(Timer / warningTime);
                    Vector2 interpolatedPosition = Vector2.Lerp(_startPosition, endPosition, ratio);
                    Vector2 up = direction.RotatedBy(MathHelper.PiOver2);
                    interpolatedPosition += up * MathF.Sin(ratio * 8) * 128;

                    if (Main.rand.NextBool(2))
                    {
                        var b = BubbleParticle.Spawn(interpolatedPosition, Vector2.Zero, Scale: Main.rand.NextFloat(0.3f, 0.6f));
                        b.gravity = 0;
                    }
                    _initialVelocity = interpolatedPosition;
                    _eyeballs[0].targetPosition = interpolatedPosition;
                    _eyeballs[0].speed = 8;

                    if (Timer > warningTime / 2f)
                    {
                        float halfTime = warningTime / 2f;
                        _eyeFlashAlpha = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine((Timer - halfTime) / halfTime));
                        Vector2 directionToMe = (NPC.Center - MyTarget.Center).SafeNormalize(Vector2.Zero);
                        Vector2 newPos = MyTarget.Center + directionToMe * 256;
                        _eyeFlashOffset = newPos - NPC.Center;

                    }

                    if (Timer >= warningTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {

                    AnimateMouthBasedOnDistance();
                    if (Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, NPC.Center + _facingDirection * 128, _facingDirection,
                                ModContent.ProjectileType<EelSpeedTrail>(), Bite_Damage, 1, Main.myPlayer, ai0: NPC.whoAmI);
                            Projectile.NewProjectile(SourceFromThis, NPC.Center + _facingDirection * 128, _facingDirection,
                                ModContent.ProjectileType<LeviathanBite>(), Bite_Damage, 1, Main.myPlayer, ai1: NPC.whoAmI, ai2: 1);
                        }
                    }


                    float dashTime = dashingTime;

                    float ratio = EasingFunction.InOutSine(Timer / dashTime);
                    Vector2 interpolatedPosition = Vector2.Lerp(_startPosition, _initialVelocity, ratio);
                    Vector2 direction = (_initialVelocity - _startPosition).SafeNormalize(Vector2.Zero);
                    Vector2 up = direction.RotatedBy(MathHelper.PiOver2);
                    interpolatedPosition += up * MathF.Sin(ratio * 8) * 128;
                    Vector2 velocity = (interpolatedPosition - NPC.Center);
                    NPC.velocity = velocity;
                    FaceVelocity();
                    _contactDamage = true;
                    _outliner.attacking = true;
                    if (Timer >= dashTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    void Spawn(int index, int offset)
                    {
                        int combinedIndex = index + offset;
                        combinedIndex %= Chain.points.Length;
                        Vector2 pos = Chain.points[combinedIndex];
                        Projectile.NewProjectile(SourceFromThis, pos, Vector2.Zero, ModContent.ProjectileType<SinElectricShock>(), Sin_Electric_Shock_Damage, 1, Main.myPlayer);
                    }
                    if (Timer % 2 == 0 && MultiplayerHelper.IsHost)
                    {
                        int a = (int)Timer / 2;
                        a %= Chain.points.Length;
                        Spawn(a, 0);


                    }


                    _effects |= EelVisualEffect.Invisible;
                    _effects |= EelVisualEffect.Mirage;
                    NPC.velocity *= 0.98f;
                    if (Timer >= 30f)
                    {
                        Timer = 0;
                        AttackCounter++;
                        if (AttackCounter < 9)
                        {
                            AttackCycle = 1;
                        }
                        else
                        {

                            AttackCycle++;
                        }
                    }
                }
                break;
            case 4:
                {
                    DiveOutToIdle();
                }
                break;
        }
    }
    #endregion

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        NPC.frame.Y = frameHeight * _frame;
    }

    private void AI_Despawn()
    {
        Timer++;
        NPC.velocity.X *= 0.98f;
        NPC.velocity.Y += 0.5f;
        FaceVelocity();
        if (Timer >= 180)
        {
            NPC.active = false;
        }
    }
    private void AI_SpawnIntro()
    {
        ShowNamePlate();
        SwitchState(AIState.Idle);
    }

    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            if (InPhase2 && !_inPhase2)
            {
                SwitchState(AIState.Phase_Transition);
                return;
            }
            if (!_firstAttack)
            {
                SwitchState(AIState.Suck);
                _firstAttack = true;
                return;
            }
            AIState pattern = PatternManager.NextPattern();
            while (IsBanned(pattern))
            {
                pattern = PatternManager.NextPattern();
            }
            SwitchState(pattern);
        }
    }

    private void FaceVelocity()
    {
        _facingDirection = Vector2.Lerp(_facingDirection, NPC.velocity.SafeNormalize(Vector2.Zero), 0.2f);
    }

    private void AI_Idle()
    {
        AttackCycle = 0;
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
            NPC.velocity = Vector2.Zero;
            _arenaCenter = MyTarget.Center - new Vector2(0, 128);
            Teleport(MyTarget.Center - new Vector2(1900, 0));
        }
        _blackedOut = false;
        float idleTime = IdleTime;
        if (InPhase2)
        {
            idleTime *= 0.8f;
        }
        float speed = MathHelper.Lerp(24, 4f, EasingFunction.QuadraticBump(Timer / idleTime));
        MoveAndSinToward(Vector2.UnitX, speed);
        FaceVelocity();
        if (Timer < IdleTime / 2f)
            _effects |= EelVisualEffect.Mirage;


        if (Timer >= idleTime)
        {
            ChooseAttack();
        }
    }
    #region Dash Trail

    private Color DashTrailColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Transparent, EasingFunction.InExpo(completionRatio)) * _dashTrailAlpha * EasingFunction.QuadraticBump(completionRatio);
    }

    private float DashTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(128, 128, completionRatio);
    }
    private void RenderPixelatedDashTrail(GraphicsDevice gDevice)
    {
        BasicLaserShader laserShader = BasicLaserShader.Instance;
        laserShader.LaserTexture = AssetManager.LaserTextures.SplittingTrail;
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.Lerp(Color.White, Color.LightBlue, ExtraMath.Osc(0f, 1f, speed: 16));
        TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, DashTrailColorFunction, DashTrailWidthFunction, laserShader, NPC.Size * 0.5f);
    }
    #endregion
    #region DrawCode
    private void LoadSegmentTextureAssets()
    {
        if (_segmentTextureAssets != null)
            return;
        _bulbGlowTextureAsset = ModContent.Request<Texture2D>($"{Texture}_BulbGlow");
        _eyeballTextureAsset = ModContent.Request<Texture2D>($"{Texture}_Eyeball");
        _pupilTextureAsset = ModContent.Request<Texture2D>($"{Texture}_Pupil");
        _segmentTextureAssets = new Asset<Texture2D>[5];
        _segmentGlowTextureAssets = new Asset<Texture2D>[5];
        for (int i = 0; i < _segmentTextureAssets.Length; i++)
        {
            _segmentTextureAssets[i] = ModContent.Request<Texture2D>($"{Texture}_{i}");
            _segmentGlowTextureAssets[i] = ModContent.Request<Texture2D>($"{Texture}_{i}_Glow");
        }

        _eyeTextureAssets = new Asset<Texture2D>[3];
        for (int i = 0; i < _eyeTextureAssets.Length; i++)
        {
            _eyeTextureAssets[i] = ModContent.Request<Texture2D>($"{Texture}_Eye_{i}");
        }
        _eyebrowTextureAsset = ModContent.Request<Texture2D>($"{Texture}_Eyebrow");
    }

    private void DrawSegment(int index, int segmentIndex, Color? overrideColor = null)
    {
        //Segment 0 is drawn manually
        if (index == 0)
            return;

        Vector2 root = Chain.points[index];
        Vector2 next = Chain.points[index - 1];
        float rotation = (next - root).ToRotation();
        Asset<Texture2D> segmentTextureAsset = _segmentTextureAssets[segmentIndex];
        SpritebatchDrawer segmentDrawer = SpritebatchDrawer.FromTextureAsset(segmentTextureAsset, root);
        segmentDrawer.rotation = rotation;

        float ratio = index / (float)(Chain.points.Length - 2);
        segmentDrawer.scale = Vector2.One * MathHelper.SmoothStep(1f, 0.85f, ratio);

        if (overrideColor.HasValue)
        {
            segmentDrawer.color = overrideColor.Value;
        }
        segmentDrawer.color *= _invisibleAlpha;
        Main.spriteBatch.Draw(segmentDrawer);

        if (overrideColor == null)
        {
            Color lightningColor = new Color(185, 255, 234);
            float chargePerSegment = 1f / Chain.points.Length;
            float myCharge = _charge - (chargePerSegment * index);

            float levelOfCharge = EasingFunction.Clamp(myCharge / chargePerSegment);
            segmentDrawer.texture = _segmentGlowTextureAssets[segmentIndex].Value;
            segmentDrawer.color = Color.Lerp(Color.Black, lightningColor, levelOfCharge) * ExtraMath.Osc(0.5f, 1f, speed: 12, offset: index) * _invisibleAlpha;
            segmentDrawer.color.A = 0;
            Main.spriteBatch.Draw(segmentDrawer);

            SpritebatchDrawer glowingDrawer2 = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, root);
            glowingDrawer2.color = Color.Lerp(Color.Black, lightningColor, levelOfCharge) * ExtraMath.Osc(0.5f, 1f, speed: 12, offset: index) * 0.2f * _invisibleAlpha;
            glowingDrawer2.color.A = 0;
            Main.spriteBatch.Draw(glowingDrawer2);
            Lighting.AddLight(root, lightningColor.ToVector3() * levelOfCharge * 0.3f);
        }
    }

    private void DrawAllSegments(Color? overrideColor = null)
    {
        //Draw Tail 
        int tailIndex = 4;
        int neckIndex = 0;
        DrawSegment(Chain.points.Length - 1, tailIndex, overrideColor);

        int segmentCounter = 0;
        for (int i = Chain.points.Length - 1; i > 1; i--)
        {
            segmentCounter++;
            float ratio = segmentCounter / (float)(Chain.points.Length - 2);
            int segmentTextureIndex = (int)MathHelper.Lerp(3, 1, ratio);
            DrawSegment(i, segmentTextureIndex, overrideColor);
        }

        //Draw Neck
        DrawSegment(1, neckIndex, overrideColor);
    }
    private void DrawWhites(SpriteBatch sb)
    {
        DrawAllSegments(_outliner.outlineColor);
        SpritebatchDrawer segmentDrawer = SpritebatchDrawer.FromNPC(NPC);
        segmentDrawer.color = _outliner.outlineColor;
        sb.Draw(segmentDrawer);
    }

    private void DrawBlacks(SpriteBatch sb, Vector2 screenPos)
    {
        SpritebatchDrawer bloomLineTelegraphDrawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_48"), NPC.Center);
        bloomLineTelegraphDrawer.rotation = _bloomLineRot - MathHelper.PiOver2;
        bloomLineTelegraphDrawer.color = _bloomLineColor;
        bloomLineTelegraphDrawer.color.A = 0;
        bloomLineTelegraphDrawer.scale.Y *= 4f;
        bloomLineTelegraphDrawer.scale.X *= 1.15f;
        sb.Draw(bloomLineTelegraphDrawer);
        if (_blackAlpha < 0.02f)
            return;

        Vector2 targetDirection = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
        SpritebatchDrawer blackDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.BlackTile, Vector2.Zero);
        blackDrawer.dstRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        blackDrawer.color = Color.Black * _blackAlpha;
        blackDrawer.drawOrigin = Vector2.Zero;
        sb.Draw(blackDrawer);

        for (int i = 0; i < _eyeballs.Length; i++)
        {
            ref FloatingEyeball floatingEyeball = ref _eyeballs[i];
            if (!_inPhase2)
                continue;

            SpritebatchDrawer pupilDrawer = SpritebatchDrawer.FromTextureAsset(_pupilTextureAsset, floatingEyeball.position);
            pupilDrawer.drawOrigin -= targetDirection * 10;
            pupilDrawer.color *= _invisibleAlpha * ExtraMath.Osc(0.5f, 1f, speed: 12, offset: i) * _blackAlpha;
            sb.Draw(pupilDrawer);

            pupilDrawer.color = Main.DiscoColor * ExtraMath.Osc(0.5f, 1f, speed: 1, offset: i) * _blackAlpha;
            pupilDrawer.color.A = 0;
            sb.Draw(pupilDrawer);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        LoadSegmentTextureAssets();
        OutlineRenderer.Queue(DrawWhites);
        PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedDashTrail);
        PixelationManager.QueuePrimitivesDrawAction(DrawHair);
        PixelationManager.QueuePrimitivesDrawAction(DrawHairBack, DrawLayer.BehindTiles);
        PixelationManager.QueuePrimitivesDrawAction(DrawEyeTentacles, DrawLayer.BehindTiles);
        PixelationManager.QueueSpritebatchDrawAction(DrawBlacks, DrawLayer.OverPlayers);

        bool drawMirage = _mirageAlpha > 0.03f;
        if (drawMirage)
        {
            MirageShader mirageShader = MirageShader.Instance;
            mirageShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
            mirageShader.Time = Main.GlobalTimeWrappedHourly;
            mirageShader.Alpha = _mirageAlpha;
            spriteBatch.Restart(effect: mirageShader.Effect);
        }

        DrawAllSegments();


        //Finally attach the head

        for (int i = 0; i < NPC.oldPos.Length; i++)
        {
            float ratio = i / (float)NPC.oldPos.Length;
            SpritebatchDrawer afDrawer = SpritebatchDrawer.FromNPC(NPC);
            afDrawer.color *= MathHelper.Lerp(1f, 0f, ratio) * 0.05f;
            afDrawer.color *= _invisibleAlpha;
            afDrawer.color *= _dashTrailAlpha;
            afDrawer.worldPosition = NPC.oldPos[i] + NPC.Size * 0.5f;
            afDrawer.rotation = NPC.oldRot[i];
            spriteBatch.Draw(afDrawer);
        }
        SpritebatchDrawer segmentDrawer = SpritebatchDrawer.FromNPC(NPC);
        segmentDrawer.color *= _invisibleAlpha;
        segmentDrawer.scale *= _eatingSquishScale;
        spriteBatch.Draw(segmentDrawer);

        //draw eyes
        Vector2 targetDirection = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
        for (int i = 0; i < _eyeTextureAssets.Length; i++)
        {
            if (_inPhase2)
                continue;

            Asset<Texture2D> eyeTextureAsset = _eyeTextureAssets[i];
            SpritebatchDrawer eyeDrawer = SpritebatchDrawer.FromTextureAsset(eyeTextureAsset, NPC.Center);
            eyeDrawer.spriteEffects = segmentDrawer.spriteEffects;
            eyeDrawer.rotation = segmentDrawer.rotation;
            eyeDrawer.scale = Vector2.One * NPC.scale;
            eyeDrawer.drawOrigin -= targetDirection * 10;
            eyeDrawer.color *= _invisibleAlpha;
            spriteBatch.Draw(eyeDrawer);

            //Glow in the darkkk
            eyeDrawer.color = Color.White * ExtraMath.Osc(0.5f, 1f, speed: 1, offset: i);
            eyeDrawer.color.A = 0;
            spriteBatch.Draw(eyeDrawer);
        }

        SpritebatchDrawer bulbDrawer = SpritebatchDrawer.FromTextureAsset(_bulbGlowTextureAsset, NPC.Center);
        bulbDrawer.spriteEffects = segmentDrawer.spriteEffects;
        bulbDrawer.rotation = segmentDrawer.rotation;
        bulbDrawer.scale = Vector2.One * NPC.scale;

        bulbDrawer.color = Color.White * ExtraMath.Osc(0.5f, 1f, speed: 1);
        bulbDrawer.color *= _invisibleAlpha;
        bulbDrawer.color.A = 0;
        spriteBatch.Draw(bulbDrawer);

        SpritebatchDrawer eyebrowDrawer = SpritebatchDrawer.FromTextureAsset(_eyebrowTextureAsset, NPC.Center);
        eyebrowDrawer.rotation = segmentDrawer.rotation;
        eyebrowDrawer.spriteEffects = segmentDrawer.spriteEffects;
        eyebrowDrawer.scale = Vector2.One * NPC.scale;
        eyebrowDrawer.drawOrigin = new Vector2(80, 64);
        eyebrowDrawer.color *= _invisibleAlpha;
        spriteBatch.Draw(eyebrowDrawer);

        for (int i = 0; i < _eyeballs.Length; i++)
        {
            ref FloatingEyeball floatingEyeball = ref _eyeballs[i];
            if (!_inPhase2)
                continue;

            SpritebatchDrawer eyeDrawer = SpritebatchDrawer.FromTextureAsset(_eyeballTextureAsset, floatingEyeball.position);
            eyeDrawer.color *= _invisibleAlpha;
            spriteBatch.Draw(eyeDrawer);

            SpritebatchDrawer pupilDrawer = SpritebatchDrawer.FromTextureAsset(_pupilTextureAsset, floatingEyeball.position);
            pupilDrawer.drawOrigin -= targetDirection * 10;
            pupilDrawer.color *= _invisibleAlpha;
            spriteBatch.Draw(pupilDrawer);

            pupilDrawer.color = Color.White * ExtraMath.Osc(0.5f, 1f, speed: 1, offset: i);
            pupilDrawer.color.A = 0;
            spriteBatch.Draw(pupilDrawer);
        }

        if (drawMirage)
        {
            spriteBatch.RestartDefaults();
        }

        if (_superCharge > 0.05f)
        {
            for (int i = 0; i < Chain.points.Length; i++)
            {
                Vector2 pos = Chain.points[i];
                SpritebatchDrawer superChargeDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, pos);
                superChargeDrawer.color = Main.DiscoColor;
                superChargeDrawer.color *= _superCharge * ExtraMath.Osc(0.9f, 1f, speed: 10);
                superChargeDrawer.color.A = 0;
                superChargeDrawer.scale = Vector2.Lerp(Vector2.One * 0.2f, Vector2.One * 0.5f, _superCharge) * ExtraMath.Osc(0.9f, 1f, speed: 10, offset: i) * 2;
                Main.spriteBatch.Draw(superChargeDrawer);

                superChargeDrawer.scale *= 0.4f;
                Main.spriteBatch.Draw(superChargeDrawer);
            }
        }


        SpritebatchDrawer chargeDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, BulbPosition);
        chargeDrawer.color = Color.LightSkyBlue;
        chargeDrawer.color *= _bulbCharge * ExtraMath.Osc(0.9f, 1f, speed: 10);
        chargeDrawer.color.A = 0;
        chargeDrawer.scale = Vector2.Lerp(Vector2.One * 0.2f, Vector2.One * 0.5f, _bulbCharge) * ExtraMath.Osc(0.9f, 1f, speed: 10);
        Main.spriteBatch.Draw(chargeDrawer);

        chargeDrawer.scale *= 0.4f;
        Main.spriteBatch.Draw(chargeDrawer);


        SpritebatchDrawer lampGlow = chargeDrawer;
        lampGlow.color = Color.LightSkyBlue;
        lampGlow.color *= ExtraMath.Osc(0.9f, 1f, speed: 10) * 0.35f * _invisibleAlpha; ;
        lampGlow.color.A = 0;
        lampGlow.scale = Vector2.One * 0.5f * ExtraMath.Osc(0.9f, 1f, speed: 10) * 2f;
        Main.spriteBatch.Draw(lampGlow);


        SpritebatchDrawer eyeFlashDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare2, EyeFlashPosition);
        eyeFlashDrawer.scale = Vector2.Lerp(Vector2.Zero, Vector2.One, EasingFunction.OutSine(_eyeFlashAlpha)) * 1.3f;
        eyeFlashDrawer.color = Color.White;
        eyeFlashDrawer.color.A = 0;
        eyeFlashDrawer.worldPosition += _eyeFlashOffset;
        eyeFlashDrawer.rotation = Main.GlobalTimeWrappedHourly * 4;
        Main.spriteBatch.Draw(eyeFlashDrawer);

        eyeFlashDrawer.scale *= 0.8f;
        Main.spriteBatch.Draw(eyeFlashDrawer);
        return false;
    }
    #region Hair Rendering
    private void SimulateHair()
    {
        HairChain.points[0] = NPC.Center + new Vector2(-80, -64).RotatedBy(NPC.rotation);
        HairChain.points[0].Y -= 4 + ExtraMath.Osc(0f, 16, speed: 2);
        HairChain.pinned[0] = true;

        for (int i = 0; i < 6; i++)
        {
            HairChain.points[i].Y += ExtraMath.Osc(-8f, 8f, speed: 0.5f, offset: i);
        }
        for (int i = 0; i < HairChain.points.Length; i++)
        {
            HairChain.points[i].Y += MathHelper.Lerp(0.2f, 1f, i / (float)HairChain.points.Length);
        }
        HairChain.ResolveBackToRoot();



        HairChain2.points[0] = NPC.Center + new Vector2(-64, -80).RotatedBy(NPC.rotation);
        HairChain2.points[0].Y -= 4 + ExtraMath.Osc(0f, 16, speed: 2);
        HairChain2.pinned[0] = true;

        for (int i = 0; i < 6; i++)
        {
            HairChain2.points[i].Y += ExtraMath.Osc(-8f, 8f, speed: 0.5f, offset: i);
        }
        for (int i = 0; i < HairChain2.points.Length; i++)
        {
            HairChain2.points[i].Y += MathHelper.Lerp(0.2f, 1f, i / (float)HairChain2.points.Length);
        }
        HairChain2.ResolveBackToRoot();
    }
    private float GetHairWidth(float ratio)
    {
        return MathHelper.SmoothStep(24, 0, ratio) * EasingFunction.QuadraticBump(ratio);
    }
    private Color GetHairColor(float ratio)
    {
        return Color.DarkGray * EasingFunction.OutExpo(ratio + 0.5f) * _invisibleAlpha;
    }
    private Color GetHairColor2(float ratio)
    {
        return Color.Lerp(Color.DarkGray, Color.Black, 0.5f) * EasingFunction.OutExpo(ratio + 0.5f) * _invisibleAlpha;
    }
    private float GetEyeWidth(float ratio)
    {
        return MathHelper.SmoothStep(7, 0, ratio) * EasingFunction.QuadraticBump(ratio);
    }
    private Color GetEyeColor(float ratio)
    {
        return Color.DarkOliveGreen * _invisibleAlpha;
    }

    private void DrawHair(GraphicsDevice gDevice)
    {
        HairShader shader = ShaderContent.GetInstance<HairShader>();
        shader.LaserTexture = TrailRegistry.GlowTrailNoBlack;
        shader.Time = Main.GlobalTimeWrappedHourly * 0.2f;
        shader.WaveFrequency = 8;
        shader.XOffset = 12;
        TrailDrawer.Draw(Main.spriteBatch, HairChain.points, GetHairColor, GetHairWidth, shader);
        if (!_inPhase2)
            return;
        for (int i = 0; i < EyeTentacles.Length; i++)
        {
            TrailDrawer.Draw(Main.spriteBatch, EyeTentacles[i].points, GetEyeColor, GetEyeWidth, shader);
        }
    }
    private void DrawEyeTentacles(GraphicsDevice gDevice)
    {
        if (!_inPhase2)
            return;
        HairShader shader = ShaderContent.GetInstance<HairShader>();
        shader.LaserTexture = TrailRegistry.GlowTrailNoBlack;
        shader.Time = Main.GlobalTimeWrappedHourly * 0.2f;
        shader.WaveFrequency = 8;
        shader.XOffset = 12;
        for (int i = 0; i < EyeTentacles.Length; i++)
        {
            TrailDrawer.Draw(Main.spriteBatch, EyeTentacles[i].points, GetEyeColor, GetEyeWidth, shader);
        }
    }
    private void DrawHairBack(GraphicsDevice gDevice)
    {
        HairShader shader = ShaderContent.GetInstance<HairShader>();
        shader.LaserTexture = TrailRegistry.GlowTrailNoBlack;
        shader.Time = Main.GlobalTimeWrappedHourly * 0.2f;
        shader.WaveFrequency = 8;
        shader.XOffset = 12;
        TrailDrawer.Draw(Main.spriteBatch, HairChain2.points, GetHairColor2, GetHairWidth, shader);
    }
    #endregion
    #endregion
}
