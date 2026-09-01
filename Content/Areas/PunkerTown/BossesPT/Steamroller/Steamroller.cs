using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Animations;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.PunkerTown.BossesPT.Steamroller.Projectiles;
using Stellamod.Content.Dusts;
using Stellamod.Content.Gores;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Steamroller;

public class Steamroller : ScarletBoss,
    IDrawOutlines
{
    public class SteamrollerSegment
    {
        public const string Anim_SpinSlow = "spinslow";
        public const string Anim_SpinFast = "spinfast";
        public const string Anim_CannonComeOut = "cannoncomeout";
        public const string Anim_CannonShoot = "cannonshoot";
        public const string Anim_CannonIdle = "cannonidle";
        private int _index;
        public SteamrollerSegment(int index)
        {
            _index = index;
            Animator.extraUpdates = _index * 4;
            glowColor = Color.Black;
        }

        public Animator _animator;
        public Animator Animator
        {
            get
            {
                if (_animator == null)
                {
                    _animator = new Animator();
                    var idle = new SpriteAnimation(0, 3, isLooping: true);
                    _animator.AddAnimation(Anim_SpinSlow, idle);

                    var running = new SpriteAnimation(4, 12, isLooping: true, frameSpeed: 0.35f);
                    _animator.AddAnimation(Anim_SpinFast, running);

                    var cannotComeOut = new SpriteAnimation(12, 18, isLooping: false);
                    _animator.AddAnimation(Anim_CannonComeOut, cannotComeOut);

                    var cannotShoot = new SpriteAnimation(18, 28, isLooping: false);
                    _animator.AddAnimation(Anim_CannonShoot, cannotShoot);

                    var cannonIde = new SpriteAnimation(28, 28, isLooping: true);
                    _animator.AddAnimation(Anim_CannonIdle, cannonIde);
                }

                return _animator;
            }
        }
        public enum SteamrollerAnimationState
        {
            Spin_Slow,
            Spin_Fast,
            Cannon_ComeOut,
            Cannon_Shoot,
            Cannon_Idle
        }

        public SteamrollerAnimationState animationState;
        public Asset<Texture2D> steamrollerSegmentTextureAsset;
        public Asset<Texture2D> steamrollerGlowSegmentTextureAsset;
        public Color glowColor;
        public bool paused;
        public bool needsFiring;
        public float firingTimer;
        public bool isDying;
        public float dyingTimer;
        public float dyingRot;
        public void Update()
        {
            if (isDying)
            {
                dyingTimer++;
                if(dyingTimer % 4 == 0)
                {
                    float radians = MathHelper.ToRadians(15);
                    dyingRot = Main.rand.NextFloat(-radians, radians);
                }
            }
            switch (animationState)
            {
                case SteamrollerAnimationState.Spin_Slow:
                    AI_SpinSlow();
                    break;
                case SteamrollerAnimationState.Spin_Fast:
                    AI_SpinFast();
                    break;
                case SteamrollerAnimationState.Cannon_ComeOut:
                    AI_CannonComeOut();
                    break;
                case SteamrollerAnimationState.Cannon_Shoot:
                    AI_CannonShoot();
                    break;
                case SteamrollerAnimationState.Cannon_Idle:
                    AI_CannonIdle();
                    break;
            }
            if (paused)
                return;
            Animator.Update();
        }

        private void AI_SpinSlow()
        {
            firingTimer = 0;
            Animator.PlayAnimation(Anim_SpinSlow);
        }
        private void AI_SpinFast()
        {
            firingTimer = 0;
            Animator.PlayAnimation(Anim_SpinFast);
        }
        private void AI_CannonComeOut()
        {
            firingTimer = 0;
            Animator.PlayAnimation(Anim_CannonComeOut);
            if (Animator.IsFinished())
            {
                animationState = SteamrollerAnimationState.Cannon_Idle;
            }
        }
        private void AI_CannonShoot()
        {

            firingTimer++;
            if (firingTimer == 1)
            {
                needsFiring = false;
            }
            Animator.PlayAnimation(Anim_CannonShoot);
            if (firingTimer == 45)
            {
                needsFiring = true;
            }
            if (Animator.IsFinished())
            {

                animationState = SteamrollerAnimationState.Cannon_Idle;
            }
        }
        private void AI_CannonIdle()
        {
            firingTimer = 0;
            Animator.PlayAnimation(Anim_CannonIdle);
        }

        public void Draw(SpriteBatch sb, Vector2 segmentPosition, Vector2 nextSegmentPosition, Color drawColor)
        {
            steamrollerGlowSegmentTextureAsset ??= ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "SteamrollerBody_Glow");
            steamrollerSegmentTextureAsset ??= ModContent.Request<Texture2D>(this.GetTypeDirectoryWithSlash() + "SteamrollerBody");
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(steamrollerSegmentTextureAsset, segmentPosition);
            float rotation = (segmentPosition - nextSegmentPosition).ToRotation();
            rotation += MathHelper.PiOver2;

            int frameHeight = 148;
            drawer.sourceRect = new Rectangle(0, Animator.GetFrameY(frameHeight), steamrollerSegmentTextureAsset.Width(), frameHeight);
            drawer.drawOrigin = new Vector2(drawer.sourceRect.Value.Width, drawer.sourceRect.Value.Height) * 0.5f;
            drawer.rotation = rotation + dyingRot;
            drawer.color = drawColor;
            sb.Draw(drawer);

            if (glowColor == Color.Black)
                return;

            drawer.color = glowColor;
            drawer.color.A = 0;
            drawer.texture = steamrollerGlowSegmentTextureAsset.Value;
            sb.Draw(drawer);
        }
    }
    private const string Anim_SpinSlow = "spinslow";
    private const string Anim_SpinFast = "spinfast";
    private enum AIState
    {
        SpawnDrill,
        IdleDrill,
        Driller,
        Despawn,

        Death_Start,
        Death_Rise,
        Death_Dying,

        X_Drill_Start,
        X_Drill_Rise,
        X_Drill_Fall,

        Snagret_PopStart,
        Snagret_PopRise,
        Snagret_PopFallNStuckk,

        DuneJump_Start,
        DuneJump_Rise,
        DuneJump_Fall,

        DungDefenderRock_Start,
        DungDefenderRock_Blast,
        DungDefenderRock_End,

        Phase_Transition,
        Phase_TransitionRise,
        Phase_TransitionFall,

        Cannon_Start,
        Cannon_Fire,
        Cannon_Barrage,
        Cannon_End,

        HeadPop_Start,
        HeadPop_Drill,
        HeadPop_Spin,
        HeadPop_Fall,
        HeadPop_End,

        MeteorJump_Start,
        MeteorJump_Fall,
        MeteorJump_Repair
    }

    private enum AttackVariant
    {
        None,
        Snagret,
        Dung,
        Fall
    }

    private bool _pauseAnimation;
    private bool _isDying;
    private bool _fromMeteorRain;
    private bool _phase2;
    private bool _quickDrill;
    private bool _driller2;
    private bool _freezeBodyMovement;
    private bool _renderDashTrail;
    private bool _crashed;
    private bool _contactDamage;
    private bool _isMainWorm;
    private float _delayTimer;
    private AttackVariant _variant;

    private float _dashTrailTimer;
    private float _dashTrailAlpha;
    private Color _targetOutlineColor;
    private Color _outlineColor;

    private float _jumpSpeed;
    private float _currentSpeed;

    private Vector2 _squishScale;
    private Vector2 _targetPosition;
    private Vector2 _startVelocity;
    private Vector2 _positionToWarpTo;
    private Animator _animator;
    private Animator Animator
    {
        get
        {
            if (_animator == null)
            {
                _animator = new Animator();
                var idle = new SpriteAnimation(0, 3, isLooping: true);
                _animator.AddAnimation(Anim_SpinSlow, idle);

                var running = new SpriteAnimation(4, 11, isLooping: true, frameSpeed: 0.35f);
                _animator.AddAnimation(Anim_SpinFast, running);
            }

            return _animator;
        }
    }

    private SteamrollerSegment[] _steamrollerSegments;
    private SteamrollerSegment[] SteamRollerSegments
    {
        get
        {
            if (_steamrollerSegments == null)
            {
                _steamrollerSegments = new SteamrollerSegment[16];
                for (int i = 0; i < _steamrollerSegments.Length; i++)
                {
                    _steamrollerSegments[i] = new SteamrollerSegment(i);

                }

            }


            return _steamrollerSegments;
        }
    }
    public Chain _chain;
    public Chain Chain
    {
        get
        {
            if (_chain == null)
            {

                _chain = new Chain(NPC.Center, 80, 16);
            }
            return _chain;
        }
    }


    private PatternManager<AIState> _patternManager;
    private PatternManager<AIState> PatternManager
    {
        get
        {
            if (_patternManager == null)
            {
                _patternManager = new PatternManager<AIState>(
                    new Tuple<AIState, float>(AIState.X_Drill_Start, 1.0f),
                    new Tuple<AIState, float>(AIState.Snagret_PopStart, 1.0f),
                    new Tuple<AIState, float>(AIState.DuneJump_Start, 1.0f),
                    new Tuple<AIState, float>(AIState.DungDefenderRock_Start, 1.0f),
                    new Tuple<AIState, float>(AIState.Cannon_Start, 1.0f),
                    new Tuple<AIState, float>(AIState.HeadPop_Start, 1.0f),
                    new Tuple<AIState, float>(AIState.MeteorJump_Start, 1.0f));
            }
            return _patternManager;
        }
    }

    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private AIState _nextAttackToDo;

    private ref float AttackCycle => ref NPC.ai[2];
    private bool IsSmall => NPC.ai[3] == 1;
    //Damage Values
    private int FallingSteamrollerDamage => 27;
    private int BedrockDamage => 43;
    private int SteamrollerBombDamage => 40;
    private int ShockwaveDamage => 48;
    private float IdleTime => 60;
    private float XDrillWarningTime => 60;
    private float DrillTime = 160;
    private float DungDefenderWarningTime => 90;

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(_isDying);
        writer.Write(_fromMeteorRain);
        writer.Write(_driller2);
        writer.Write(_quickDrill);
        writer.Write(_crashed);
        writer.Write(_jumpSpeed);
        writer.Write(_currentSpeed);
        writer.WriteVector2(_targetPosition);
        writer.WriteVector2(_startVelocity);
        writer.WriteVector2(_positionToWarpTo);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _isDying = reader.ReadBoolean();
        _fromMeteorRain = reader.ReadBoolean();
        _driller2 = reader.ReadBoolean();
        _quickDrill = reader.ReadBoolean();
        _crashed = reader.ReadBoolean();
        _jumpSpeed = reader.ReadSingle();
        _currentSpeed = reader.ReadSingle();
        _targetPosition = reader.ReadVector2();
        _startVelocity = reader.ReadVector2();
        _positionToWarpTo = reader.ReadVector2();
    }

    public Vector2 GetSegmentPosition(int verletIndex)
    {
        if (verletIndex < 0)
            return NPC.Center;

        return Chain.points[verletIndex];
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 12;
        NPCID.Sets.TrailCacheLength[Type] = 32;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        if (!_pauseAnimation)
            Animator.Update();
        NPC.frame.Y = Animator.GetFrameY(frameHeight);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();

        _squishScale = Vector2.One;

        NPC.width = 128;
        NPC.height = 128;
        NPC.damage = 180;
        NPC.defense = 28;
        NPC.lifeMax = 13800;

        NPC.value = Item.buyPrice(gold: 5);
        NPC.knockBackResist = 0f;
        NPC.boss = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.npcSlots = 30f;
        NPC.behindTiles = true;

        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/SitriAndTheMechs");
        NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
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

    private void Teleport(Vector2 warpPosition)
    {
        if (MultiplayerHelper.IsHost)
        {
            _positionToWarpTo = warpPosition;
            NPC.netUpdate = true;
        }

    }

    public override void AI()
    {
        base.AI();
        if (_positionToWarpTo != Vector2.Zero)
        {
            NPC.Center = _positionToWarpTo;
            _positionToWarpTo = Vector2.Zero;
        }
        for (int i = 0; i < SteamRollerSegments.Length; i++)
        {
            SteamRollerSegments[i].paused = false;
        }

        if (IsSmall)
        {
            if (NPC.life > NPC.lifeMax * 0.5f)
            {
                NPC.life = (int)(NPC.lifeMax * 0.5f) - 1;
            }
            _phase2 = true;
        }

        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget && State != AIState.Despawn)
            {
                SwitchState(AIState.Despawn);
            }
        }

        if (NPC.life <= 1 && !_isDying)
        {
            NPC.life = 1;
            SwitchState(AIState.Death_Start);
        }

        //Sync Health between copies
        foreach (var npc in Main.ActiveNPCs)
        {
            if (npc.type != Type)
                continue;
            if (npc.life > NPC.life)
                npc.life = NPC.life;
            if (_isMainWorm)
            {
                if(npc.ModNPC is Steamroller babySteamroller)
                {
                    babySteamroller._nextAttackToDo = _nextAttackToDo;
                }
            }
        }

        _pauseAnimation = false;
        _freezeBodyMovement = false;
        _variant = AttackVariant.None;
        _renderDashTrail = false;
        _contactDamage = false;
        _targetOutlineColor = Color.Transparent;
  
        switch (State)
        {
            case AIState.SpawnDrill:
                AI_SpawnDrill();
                break;
            case AIState.IdleDrill:
                AI_IdleDrill();
                break;
            case AIState.Driller:
                AI_Driller();
                break;
            case AIState.Despawn:
                AI_Despawn();
                break;

            case AIState.Death_Start:
                _isDying = true;
                AI_CannonStart();
                break;
            case AIState.Death_Rise:
                _isDying = true;
                AI_CannonFire();
                break;
            case AIState.Death_Dying:
                _isDying = true;
                AI_CannonBarrage();
                break;

            case AIState.X_Drill_Start:
                AI_XDrillStart();
                break;
            case AIState.X_Drill_Rise:
                AI_XDrillRise();
                break;
            case AIState.X_Drill_Fall:
                AI_XDrillFall();
                break;

            case AIState.DuneJump_Start:
                _variant = AttackVariant.Fall;
                AI_XDrillStart();
                break;
            case AIState.DuneJump_Rise:
                _variant = AttackVariant.Fall;
                AI_XDrillRise();
                break;
            case AIState.DuneJump_Fall:
                _variant = AttackVariant.Fall;
                AI_XDrillFall();
                break;

            case AIState.DungDefenderRock_Start:
                AI_DungDefenderRockStart();
                break;
            case AIState.DungDefenderRock_Blast:
                AI_DungDefenderRockBlast();
                break;
            case AIState.DungDefenderRock_End:
                _variant = AttackVariant.Dung;
                AI_DungDefenderRockEnd();
                break;

            case AIState.Snagret_PopStart:
                _variant = AttackVariant.Snagret;
                AI_XDrillStart();
                break;
            case AIState.Snagret_PopRise:
                _variant = AttackVariant.Snagret;
                AI_XDrillRise();
                break;
            case AIState.Snagret_PopFallNStuckk:
                _variant = AttackVariant.Snagret;
                AI_XDrillFall();
                break;

            case AIState.Cannon_Start:
                AI_CannonStart();
                break;
            case AIState.Cannon_Fire:
                AI_CannonFire();
                break;
            case AIState.Cannon_Barrage:
                AI_CannonBarrage();
                break;
            case AIState.Cannon_End:
                AI_CannonEnd();
                break;

            case AIState.HeadPop_Start:
                AI_HeadPopStart();
                break;
            case AIState.HeadPop_Fall:
                AI_HeadPopFall();
                break;
            case AIState.HeadPop_Spin:
                AI_HeadPopSpin();
                break;
            case AIState.HeadPop_Drill:
                AI_HeadPopDrill();
                break;
            case AIState.HeadPop_End:
                AI_HeadPopEnd();
                break;

            case AIState.MeteorJump_Start:
                AI_MeteorJumpStart();
                break;
            case AIState.MeteorJump_Fall:
                AI_MeteorJumpFall();
                break;
            case AIState.MeteorJump_Repair:
                AI_MeteorJumpEnd();
                break;

            case AIState.Phase_Transition:
                AI_PhaseTransition();
                break;
            case AIState.Phase_TransitionRise:
                AI_PhaseTransitionRise();
                break;
            case AIState.Phase_TransitionFall:
                AI_PhaseTransitionFall();
                break;
        }

        for (int i = 0; i < SteamRollerSegments.Length; i++)
        {
            SteamrollerSegment segment = SteamRollerSegments[i];
            segment.Update();
        }
        _dashTrailTimer += _renderDashTrail ? 1 : -1;
        _dashTrailTimer = MathHelper.Clamp(_dashTrailTimer, 0, 60);
        _dashTrailAlpha = EasingFunction.InOutSine(_dashTrailTimer / 60f);
        _outlineColor = Color.Lerp(_outlineColor, _targetOutlineColor, 0.3f);
    }

    public override void PostAI()
    {
        base.PostAI();
        if (_freezeBodyMovement)
            return;

        Chain.points[0] = NPC.Center;
        Chain.pinned[0] = true;
        for (int i = 0; i < 32; i++)
        {
            Chain.ResolveBackToRoot();
        }
    }

    private void AI_Despawn()
    {
        Timer++;
        if (NPC.velocity.Y < 0)
            NPC.velocity.Y *= 0.94f;
        else
        {
            NPC.velocity.Y += 0.2f;
            NPC.velocity.Y *= 1.025f;
        }
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        if (Timer >= 120)
            NPC.active = false;
    }

    #region Phase Shift

    private void AI_PhaseTransition()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
        }
        if (Timer == 1)
        {
            _startVelocity = NPC.velocity;
            NPC.TargetClosest();
            _targetPosition = MyTarget.Bottom;
            if (_variant == AttackVariant.Snagret)
            {
                _targetPosition.X += MyTarget.direction * 128;
            }
        }

        //X Appears on the ground
        if (Timer == 1 && MultiplayerHelper.IsHost)
        {
            Projectile.NewProjectile(NPC.GetSource_FromAI(), MyTarget.Bottom, Vector2.Zero,
                ModContent.ProjectileType<RedX>(), 1, 1, Main.myPlayer);
        }

        //Ease in to the start position for the attack
        float ratio = Timer / XDrillWarningTime;
        float ease = EasingFunction.InOutSine(ratio);
        Vector2 startPosition = _targetPosition + new Vector2(0, 1000);
        Vector2 targetVelocity = (startPosition - NPC.Center);
        Vector2 interpVelocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
        NPC.velocity = interpVelocity;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

        _targetOutlineColor = Color.Yellow;
        if (Timer >= XDrillWarningTime)
        {
            SwitchState(AIState.Phase_TransitionRise);
        }
    }

    private void AI_PhaseTransitionRise()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
        }

        Animator.PlayAnimation(Anim_SpinFast);

        _renderDashTrail = true;
        _contactDamage = true;
        _targetOutlineColor = Color.Red;
        Vector2 shootVelocity = -Vector2.UnitY * 45;
        NPC.velocity = shootVelocity;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        ShakeScreenPosition.Shake = 3;
        if (Timer > 200 || NPC.Center.Y < MyTarget.Bottom.Y)
        {
            SwitchState(AIState.Phase_TransitionFall);
        }
    }

    private void AI_PhaseTransitionFall()
    {
        Timer++;
        if (Timer == 1)
        {
            float dir = NPC.Center.X < MyTarget.Center.X ? 1 : -1;
            _jumpSpeed = dir;
            _jumpSpeed *= 21;
            if (MultiplayerHelper.IsHost)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 spawnPosition = NPC.Center;
                    spawnPosition.X += Main.rand.NextFloat(-64, 64);
                    spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                    Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-12, -17);
                    spawnVelocity.X = dir * Main.rand.NextFloat(2f, 15f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition, spawnVelocity,
                        ModContent.ProjectileType<Bedrock>(), BedrockDamage, 1, Main.myPlayer);
                }

            }

            _currentSpeed = NPC.velocity.X;
            _crashed = false;

            GroundImpact();

            SoundStyle smash = AssetRegistry.Sounds.Melee.HammerSmash3;
            smash.PitchVariance = 0.3f;
            SoundEngine.PlaySound(smash, NPC.position);

            SoundStyle steaming = AssetRegistry.Sounds.SteamPunking.MechSteaming;
            steaming.PitchVariance = 0.3f;
            steaming.Volume = 0.5f;
            SoundEngine.PlaySound(steaming, NPC.position);

            SoundStyle mechMove = AssetRegistry.Sounds.SteamPunking.MechMove;
            mechMove.PitchVariance = 0.3f;
            //    SoundEngine.PlaySound(mechMove, NPC.position);


        }

        _targetOutlineColor = Color.Red;
        _contactDamage = true;
        _renderDashTrail = true;
        if(_isMainWorm)
        {
            Vector2 targetPos = Vector2.Lerp(MyTarget.Center, NPC.Center, 0.35f);
            if (Timer < 70)
                CameraTargetSystem.AddTarget(targetPos);
        }

        if (Timer == 60)
        {
            if (MultiplayerHelper.IsHost && _isMainWorm)
            {
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPC.type, 
                    ai0: 0, ai1: (int)AIState.Phase_TransitionFall, ai3: 1);
            }
            NPC.ai[3] = 1;
        }

        _phase2 = true;

        for (int i = 0; i < _steamrollerSegments.Length; i++)
        {
            var segment = _steamrollerSegments[i];
            segment.glowColor = Color.Lerp(Color.Transparent, Color.Red, EasingFunction.InOutSine(Timer / 60f)) * ExtraMath.Osc(0f, 1f, speed: 10, offset: i);
        }




        if (NPC.velocity.Y < 0)
            NPC.velocity.Y *= 0.97f;
        if (NPC.velocity.Y < 25)
            NPC.velocity.Y += 0.5f;

        if (NPC.velocity.Y > 12)
        {
            Animator.PlayAnimation(Anim_SpinFast);
            NPC.velocity.Y *= 1.1f;

        }
        else
        {
            Animator.PlayAnimation(Anim_SpinSlow);
        }

        if (NPC.velocity.Y > 50)
            NPC.velocity.Y = 50;
        float xDirectionToTarget = NPC.Center.X < MyTarget.Center.X ? 1 : -1;
        float dist = MathF.Abs(MyTarget.Center.X - NPC.Center.X);
        float xSpeed = xDirectionToTarget * dist * 0.25f;

        NPC.velocity.X = MathHelper.Lerp(_currentSpeed, _jumpSpeed, EasingFunction.InOutSine(Timer / 25f));
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

        Vector2 bottom = NPC.Bottom + Vector2.UnitY * 64;
        Point tilePoint = bottom.ToTileCoordinates();
        if (WorldGen.InWorld(tilePoint.X, tilePoint.Y) && Timer > 20)
        {
            Tile tile = Main.tile[tilePoint];
            if (WorldGen.SolidTile(tile) && !_crashed)
            {
                _crashed = true;
                SoundStyle smash2 = AssetRegistry.Sounds.Melee.HammerSmash2;
                smash2.PitchVariance = 0.3f;
                SoundEngine.PlaySound(smash2, NPC.position);

                FXUtil.ShakeCamera(NPC.Center, 1024, 24);
                SwitchState(AIState.Driller);

            }
        }

        MakeSteamParticlesRandomlyAtSegments();
    }

    #endregion

    #region Meteor Jump

    private void AI_MeteorJumpStart()
    {
        Timer++;
        if (Timer == 1)
        {
            _startVelocity = NPC.velocity;
            NPC.TargetClosest();
            _targetPosition = MyTarget.Bottom;
        }

        //X Appears on the ground
        if (Timer == 1 && MultiplayerHelper.IsHost)
        {
            Projectile.NewProjectile(NPC.GetSource_FromAI(), MyTarget.Bottom, Vector2.Zero,
                ModContent.ProjectileType<RedX>(), 1, 1, Main.myPlayer);
        }

        //Ease in to the start position for the attack
        float ratio = Timer / XDrillWarningTime;
        float ease = EasingFunction.InOutSine(ratio);
        Vector2 startPosition = _targetPosition + new Vector2(0, 1000);
        Vector2 targetVelocity = (startPosition - NPC.Center);
        Vector2 interpVelocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
        NPC.velocity = interpVelocity;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        _targetOutlineColor = Color.Yellow;
        if (Timer >= XDrillWarningTime)
        {
            SwitchState(AIState.MeteorJump_Fall);
        }
    }

    private void AI_MeteorJumpFall()
    {
        Animator.PlayAnimation(Anim_SpinSlow);
        Timer++;
        if (Timer == 1)
        {
            _crashed = false;
            for (int i = 0; i < Chain.points.Length; i++)
            {
                Chain.points[i] = NPC.Center;
            }
        }


        _renderDashTrail = true;
        _contactDamage = true;
        _targetOutlineColor = Color.Red;
        Vector2 shootVelocity = -Vector2.UnitY * 35;
        NPC.velocity = shootVelocity;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        ShakeScreenPosition.Shake = 3;
        if (Timer > 200 || NPC.Center.Y < MyTarget.Bottom.Y)
        {
            SwitchState(AIState.MeteorJump_Repair);
        }
    }

    private void AI_MeteorJumpEnd()
    {
        Timer++;
        if (Timer == 1)
        {
            GroundImpact();
            SoundStyle smash = AssetRegistry.Sounds.Melee.HammerSmash3;
            smash.PitchVariance = 0.3f;
            SoundEngine.PlaySound(smash, NPC.position);

            SoundStyle steaming = AssetRegistry.Sounds.SteamPunking.MechSteaming;
            steaming.PitchVariance = 0.3f;
            steaming.Volume = 0.5f;
            SoundEngine.PlaySound(steaming, NPC.position);

            SoundStyle mechMove = AssetRegistry.Sounds.SteamPunking.MechMove;
            mechMove.PitchVariance = 0.3f;
        }

        _freezeBodyMovement = true;
        for (int i = 0; i < SteamRollerSegments.Length; i++)
        {
            ref Vector2 point = ref Chain.points[i];
            point += NPC.velocity;
            Vector2 flyingVelocity = Vector2.Lerp(-Vector2.UnitX, Vector2.UnitX, ExtraMath.Osc(0f, 1f, offset: i * 4));
            point += flyingVelocity * 8;
        }

        Vector2 targetPos = Vector2.Lerp(MyTarget.Center, MyTarget.Center + new Vector2(0, -500), 0.35f);
        CameraTargetSystem.AddTarget(targetPos);
        if (Timer > 60)
        {
            NPC.velocity *= 0.9f;
        }

        int count = 16;
        if (IsSmall)
            count /= 4;
        if (Timer > 60 && AttackCycle < count)
        {
            if (Timer % 10 == 0)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 fallPosition = Vector2.Lerp(-Vector2.UnitX, Vector2.UnitX, Main.rand.NextFloat(0f, 1f)) * 666 + MyTarget.Center;
                    fallPosition.Y -= 1000;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), fallPosition, Vector2.UnitY,
                        ModContent.ProjectileType<FallingSteamrollerPart>(), FallingSteamrollerDamage, 1, Main.myPlayer);
                }
                AttackCycle++;
            }
        }
        else if (AttackCycle >= count)
        {
            _fromMeteorRain = true;
            Teleport(MyTarget.Center + new Vector2(MyTarget.direction * 1500, -750));
            SwitchState(AIState.HeadPop_Spin);
        }
    }

    #endregion

    #region Head Pop Drill Spin Attack
    private void AI_HeadPopStart()
    {
        Timer++;
        if (Timer == 1)
        {
            _startVelocity = NPC.velocity;
            NPC.TargetClosest();
            _targetPosition = MyTarget.Bottom;
        }

        //X Appears on the ground
        if (Timer == 1 && MultiplayerHelper.IsHost)
        {
            Projectile.NewProjectile(NPC.GetSource_FromAI(), MyTarget.Bottom, Vector2.Zero,
                ModContent.ProjectileType<RedX>(), 1, 1, Main.myPlayer);
        }

        //Ease in to the start position for the attack
        float ratio = Timer / XDrillWarningTime;
        float ease = EasingFunction.InOutSine(ratio);
        Vector2 startPosition = _targetPosition + new Vector2(0, 1000);
        Vector2 targetVelocity = (startPosition - NPC.Center);
        Vector2 interpVelocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
        NPC.velocity = interpVelocity;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

        _targetOutlineColor = Color.Yellow;
        if (Timer >= XDrillWarningTime)
        {
            SwitchState(AIState.HeadPop_Drill);
        }
    }
    private void AI_HeadPopDrill()
    {
        Animator.PlayAnimation(Anim_SpinFast);
        Timer++;
        if (Timer == 1)
        {
            _crashed = false;
            WarpSegments();
        }

        _renderDashTrail = true;
        _contactDamage = true;
        _targetOutlineColor = Color.Red;
        Vector2 shootVelocity = -Vector2.UnitY * 45;
        NPC.velocity = shootVelocity;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        ShakeScreenPosition.Shake = 3;
        if (Timer > 200 || NPC.Center.Y < MyTarget.Bottom.Y)
        {
            SwitchState(AIState.HeadPop_Spin);
        }
    }
    private void AI_HeadPopSpin()
    {
        Timer++;
        if (Timer == 1)
        {
            _currentSpeed = NPC.velocity.Length();
            GroundImpact();

            SoundStyle smash = AssetRegistry.Sounds.Melee.HammerSmash3;
            smash.PitchVariance = 0.3f;
            SoundEngine.PlaySound(smash, NPC.position);

            SoundStyle steaming = AssetRegistry.Sounds.SteamPunking.MechSteaming;
            steaming.PitchVariance = 0.3f;
            steaming.Volume = 0.5f;
            SoundEngine.PlaySound(steaming, NPC.position);

            SoundStyle mechMove = AssetRegistry.Sounds.SteamPunking.MechMove;
            mechMove.PitchVariance = 0.3f;
        }

        _targetOutlineColor = Color.Red;
        _contactDamage = true;
        _renderDashTrail = true;
        _freezeBodyMovement = true;
        Vector2 bodyVel = Vector2.Lerp(-Vector2.UnitY * 15, Vector2.Zero, EasingFunction.InExpo(Timer / 60f));
        for (int i = 0; i < Chain.points.Length; i++)
        {
            ref Vector2 point = ref Chain.points[i];
            point += bodyVel;
        }

        if (!_fromMeteorRain)
        {
            Vector2 targetPos = Vector2.Lerp(MyTarget.Center, NPC.Center, 0.35f);
            if (Timer < 70)
                CameraTargetSystem.AddTarget(targetPos);
        }

        for (int i = 0; i < _steamrollerSegments.Length; i++)
        {
            var segment = _steamrollerSegments[i];
            segment.glowColor = Color.Lerp(Color.Transparent, Color.Red, EasingFunction.InOutSine(Timer / 60f)) * ExtraMath.Osc(0f, 1f, speed: 10, offset: i);
        }

        if (NPC.velocity.Y < 0)
        {
            NPC.velocity.Y *= MathHelper.Lerp(0.94f, 0.92f, EasingFunction.InOutSine(Timer / 45f));
            NPC.velocity.Y -= MathHelper.Lerp(0.5f, 0f, EasingFunction.InOutSine(Timer / 45f));
        }
        if (NPC.velocity.Y < 25)
            NPC.velocity.Y += 0.35f;

        if (NPC.velocity.Y > 12)
        {
            Animator.PlayAnimation(Anim_SpinFast);
            NPC.velocity.Y *= 1.075f;
        }
        else
        {
            Animator.PlayAnimation(Anim_SpinSlow);
        }

        if (NPC.velocity.Y > 50)
            NPC.velocity.Y = 50;
        float xDirectionToTarget = NPC.Center.X < MyTarget.Center.X ? 1 : -1;
        float dist = MathF.Abs(MyTarget.Center.X - NPC.Center.X);
        float xSpeed = xDirectionToTarget * dist * 0.25f;

        NPC.velocity.X = MathHelper.Lerp(_currentSpeed, xSpeed, EasingFunction.InOutSine(Timer / 60f));
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        Vector2 bottom = NPC.Bottom + Vector2.UnitY * 64;
        Point tilePoint = bottom.ToTileCoordinates();
        if (WorldGen.InWorld(tilePoint.X, tilePoint.Y) && Timer > 20)
        {
            Tile tile = Main.tile[tilePoint];
            if (WorldGen.SolidTile(tile) && !_crashed)
            {
                _crashed = true;
                SoundStyle smash2 = AssetRegistry.Sounds.Melee.HammerSmash2;
                smash2.PitchVariance = 0.3f;
                SoundEngine.PlaySound(smash2, NPC.position);

                FXUtil.ShakeCamera(NPC.Center, 1024, 24);
                SwitchState(AIState.HeadPop_Fall);
            }
        }
    }

    private void AI_HeadPopFall()
    {
        Animator.PlayAnimation(Anim_SpinFast);
        Timer++;

        if (Timer == 1)
        {
            _targetPosition = Vector2.UnitX * ((MyTarget.Center.X > NPC.Center.X) ? 1 : -1);
            NPC.TargetClosest();
        }
        Vector2 bottom = NPC.Top - Vector2.UnitY * 64;
        Point point = bottom.ToTileCoordinates();
        while (!WorldGen.SolidTile(point))
            point.Y++;
        bottom = point.ToWorldCoordinates();

        _targetOutlineColor = Color.Red;
        _contactDamage = true;
        _renderDashTrail = true;

        ShakeScreenPosition.Shake = 3;
        Vector2 velToTarget = _targetPosition;
        float ratio = Timer / 60f;
        float ease = Easing.InExpo(ratio);
        float speed = MathHelper.Lerp(1, 24, ease);
        velToTarget *= speed;
        NPC.velocity = velToTarget;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, Vector2.UnitY.ToRotation() + MathHelper.PiOver2, 0.1f);
        if (Main.rand.NextBool(3))
        {
            Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
            int d = WorldGen.KillTile_MakeTileDust(point.X, point.Y, Framing.GetTileSafely(point));
            Dust dust = Main.dust[d];
            dust.velocity = spawnVelocity;
            dust.noLightEmittence = true;
        }
        if (Main.rand.NextBool(4))
        {
            Vector2 vel = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(80));
            vel *= Main.rand.NextFloat(8f, 25);
            var spawnParams = DustParticleSpawnParams.Default;

            spawnParams.outerColor = Color.Red;
            var dp = DustParticle.Spawn(bottom, vel, spawnParams);
            dp.fast = true;

            vel = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(80));
            vel *= Main.rand.NextFloat(1, 2);

            var sp = SmokeParticle.Spawn(bottom, vel);
            sp.initialColor = Color.Brown * 0.5f;
            sp.fadeToColor = Color.Transparent;
        }
        if (Main.rand.NextBool(6) && Main.netMode != NetmodeID.Server)
        {
            Vector2 spawnPosition = bottom;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
            ModContent.GetInstance<FlyingSoilSystem>().NewSoil(spawnPosition, spawnVelocity);
        }
        Vector2 bodyVel = Vector2.Lerp(Vector2.Zero, Vector2.UnitY * 18, EasingFunction.InExpo(Timer / 60f));
        for (int i = 0; i < Chain.points.Length; i++)
        {
            ref Vector2 p = ref Chain.points[i];
            p += bodyVel;
        }
        if (Timer >= 100)
        {
            SwitchState(AIState.HeadPop_End);
        }
        _freezeBodyMovement = true;
    }

    private void AI_HeadPopEnd()
    {
        Timer++;

        NPC.velocity.Y += 0.5f;
        NPC.velocity.Y *= 1.1f;
        if (NPC.velocity.Y > 50)
            NPC.velocity.Y = 50;

        if (Timer >= 90)
        {
            SwitchState(AIState.IdleDrill);
        }
        _freezeBodyMovement = true;
    }
    #endregion

    #region Cannon
    private void AI_CannonStart()
    {
        Timer++;
        if (Timer == 1)
        {
            _startVelocity = NPC.velocity;
            NPC.TargetClosest();
            _targetPosition = MyTarget.Bottom;
        }

        //X Appears on the ground
        if (Timer == 1 && MultiplayerHelper.IsHost)
        {
            Projectile.NewProjectile(NPC.GetSource_FromAI(), MyTarget.Bottom, Vector2.Zero,
                ModContent.ProjectileType<RedX>(), 1, 1, Main.myPlayer);
        }

        //Ease in to the start position for the attack
        float ratio = Timer / XDrillWarningTime;
        float ease = EasingFunction.InOutSine(ratio);
        Vector2 startPosition = _targetPosition + new Vector2(0, 1000);
        Vector2 targetVelocity = (startPosition - NPC.Center);
        Vector2 interpVelocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
        NPC.velocity = interpVelocity;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

        _targetOutlineColor = Color.Yellow;
        if (Timer >= XDrillWarningTime)
        {
            SwitchState(AIState.Cannon_Fire);
        }
    }

    private void AI_CannonFire()
    {
        Animator.PlayAnimation(Anim_SpinSlow);
        Timer++;
        if (Timer == 1)
        {
            _crashed = false;
            WarpSegments();
        }

        for (int i = 0; i < SteamRollerSegments.Length; i++)
        {
            SteamRollerSegments[i].animationState = SteamrollerSegment.SteamrollerAnimationState.Cannon_ComeOut;
        }

        _renderDashTrail = true;
        _contactDamage = true;
        _targetOutlineColor = Color.Red;
        Vector2 shootVelocity = -Vector2.UnitY * 45;
        NPC.velocity = shootVelocity;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        ShakeScreenPosition.Shake = 3;
        if (Timer > 200 || NPC.Center.Y < MyTarget.Bottom.Y)
        {
            SwitchState(AIState.Cannon_Barrage);
        }
    }

    private void AI_CannonBarrage()
    {
        _pauseAnimation = true;

        Animator.PlayAnimation(Anim_SpinSlow);
        Timer++;
        if (Timer == 1)
        {
            _currentSpeed = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
            GroundImpact();
            if (_isDying)
            {
                SoundStyle steamingSound = AssetRegistry.Sounds.SteamPunking.SteamingDeathStart;
                steamingSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(steamingSound, NPC.position);
            }

            SoundStyle smash = AssetRegistry.Sounds.Melee.HammerSmash3;
            smash.PitchVariance = 0.3f;
            SoundEngine.PlaySound(smash, NPC.position);

            SoundStyle steaming = AssetRegistry.Sounds.SteamPunking.MechSteaming;
            steaming.PitchVariance = 0.3f;
            steaming.Volume = 0.5f;
            SoundEngine.PlaySound(steaming, NPC.position);

            SoundStyle mechMove = AssetRegistry.Sounds.SteamPunking.MechMove;
            mechMove.PitchVariance = 0.3f;
        }

        NPC.velocity = NPC.velocity.RotatedBy(_currentSpeed * MathHelper.Lerp(0.1f, 0f, EasingFunction.InOutSine(Timer / 30f)));
        if (NPC.velocity.Length() > 1)
        {
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

        }
        NPC.velocity *= 0.95f;
        float deathTime = 250;
        //This state gets reused for the death animation
        if (_isDying)
        {
            for (int i = 0; i < SteamRollerSegments.Length; i++)
            {
                var steamRollerSegment = SteamRollerSegments[i];
                steamRollerSegment.glowColor = Color.Lerp(Color.Transparent, Color.Red, ExtraMath.Osc(0f, 1f, speed: 32));
            }

            float ease = EasingFunction.InOutSine(Timer / deathTime);
            float segmentLength = MathHelper.Lerp(80, 35, ease);
            Chain.segmentLength = segmentLength;

            if (Timer >= deathTime)
            {
                FXUtil.ShakeCamera(NPC.Center, 1024, 8);
                ShakeScreenPosition.Shake = 5;
                for (int i = 0; i < SteamRollerSegments.Length / 2; i++)
                {
                    var steamRollerSegment = SteamRollerSegments[i];
                    int headGore = Mod.Find<ModGore>($"{Name}_Gore_Body_0").Type;
                    int legGore = Mod.Find<ModGore>($"{Name}_Gore_Body_1").Type;

                    // Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
                    Vector2 position = Chain.points[i];
                    var fx = FXUtil.GlowCircleBoom(position, Color.Yellow, Color.Red, Color.DarkRed);
                    fx.Scale *= 2;
                    for (float f = 0; f < 3; f++)
                    {
                        Vector2 vel = Main.rand.NextVector2Circular(8, 8);
                        var spawnParams = DustParticleSpawnParams.Default;
                        spawnParams.innerColor = Color.Yellow;
                        DustParticle.Spawn(position, vel, spawnParams);
                    }

                    Gore.NewGore(NPC.GetSource_Death(), position + new Vector2(-32, 0), NPC.velocity + new Vector2(Main.rand.NextFloat(-5f, 5f), -15), headGore, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), position + new Vector2(32, 0), NPC.velocity + new Vector2(Main.rand.NextFloat(-5f, 5f), -15), legGore);
                }

                SoundStyle kaboom = new SoundStyle("Stellamod/Assets/Sounds/RekShockwave");
                SoundEngine.PlaySound(kaboom, NPC.position);
                if (Main.netMode != NetmodeID.Server)
                    ModContent.GetInstance<ScreenShaderSystem>().TintScreen(Color.Red, 0.2f, 15);
                NPC.Kill();
            }
            return;
        }

        _targetOutlineColor = Color.Red;
        for (int i = 0; i < SteamRollerSegments.Length; i++)
        {
            var segment = SteamRollerSegments[i];
            if (segment.needsFiring)
            {
                if (MultiplayerHelper.IsHost)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (j == 0)
                            continue;

                        Vector2 posToFireFrom = GetSegmentPosition(i);
                        Vector2 forwardVector2 = GetSegmentPosition(i - 1) - posToFireFrom;
                        forwardVector2 = forwardVector2.SafeNormalize(Vector2.Zero);
                        forwardVector2 = forwardVector2.RotatedBy(MathHelper.PiOver2 * j);

                        Vector2 fireVelocity = forwardVector2 * 15;
                        posToFireFrom += forwardVector2 * 50;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), posToFireFrom, fireVelocity, 
                            ModContent.ProjectileType<SteamrollerBomb>(), SteamrollerBombDamage, 1, Main.myPlayer);
                    }

                }
                segment.needsFiring = false;
            }
        }

        if (Timer % 15 == 0)
        {
            int index = (int)AttackCycle;
            index %= 6;
            SteamrollerSegment nextSegment = SteamRollerSegments[index + 1];
            nextSegment.animationState = SteamrollerSegment.SteamrollerAnimationState.Cannon_Shoot;
            if (AttackCycle < SteamRollerSegments.Length)
            {
                AttackCycle++;
            }
            else
            {
                SwitchState(AIState.Cannon_End);
            }
        }
    }

    private void AI_CannonEnd()
    {
        Animator.PlayAnimation(Anim_SpinSlow);
        Timer++;

        NPC.velocity.Y += 0.125f;
        NPC.velocity.Y *= 1.025f;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        if (NPC.velocity.Y > 25)
        {
            NPC.velocity.Y = 25;
        }

        WaitForAndDrill();
    }
    #endregion

    #region Dung Defender
    private void DungDefenderRocks()
    {
        Vector2 bottom = _targetPosition - Vector2.UnitY * 64;
        Point point = bottom.ToTileCoordinates();
        for(int i = 0; i < 1000; i++)
        {
            if (WorldGen.SolidTile(point))
                break;
            else
                point.Y++;
        }
  
        bottom = point.ToWorldCoordinates();
        if (Main.rand.NextBool(4))
        {
            Vector2 vel = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(80));
            vel *= Main.rand.NextFloat(8f, 25);

            vel = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(80));
            vel *= Main.rand.NextFloat(1, 2);

            var sp = SmokeParticle.Spawn(bottom, vel);
            sp.initialColor = Color.Brown * 0.5f;
            sp.fadeToColor = Color.Transparent;

            if (Main.rand.NextBool(3))
            {
                Vector2 spawnPosition = bottom;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Vector2.UnitX * (Main.rand.NextBool(2) ? -5 : 5);

                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
            }
            if (Main.rand.NextBool(3))
            {
                Vector2 spawnPosition = bottom;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -50);

                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
            }
        }

        if (Main.rand.NextBool(3))
        {
            Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
            Dust.NewDustPerfect(bottom + Main.rand.NextVector2Circular(64, 64), DustID.Dirt, spawnVelocity, Scale: 2);
        }

        if (Main.rand.NextBool(6) && Main.netMode != NetmodeID.Server)
        {
            Vector2 spawnPosition = bottom;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
            ModContent.GetInstance<FlyingSoilSystem>().NewSoil(spawnPosition, spawnVelocity);
        }
    }
    private void AI_DungDefenderRockStart()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
            //Teleport(MyTarget.Bottom + new Vector2(0, 1000));
        }

        if (Timer < DungDefenderWarningTime - 30)
        {
            _startVelocity = NPC.velocity;
            _targetPosition = MyTarget.Bottom;
        }


        if (NPC.velocity.Y < 0)
        {
            NPC.velocity.Y += 0.125f;
            NPC.velocity.Y *= 0.65f;
        }
        else
        {
            NPC.velocity.Y += 0.5f;
            if (NPC.velocity.Y > 25)
            {
                NPC.velocity.Y = 25;
            }
        }
        for (int i = 0; i < Chain.points.Length; i++)
        {
            ref Vector2 p = ref Chain.points[i];
            p += NPC.velocity;
        }
        /*

        //Ease in to the start position for the attack
        float ratio = Timer / DungDefenderWarningTime;
        float ease = EasingFunction.InOutSine(ratio);
        Vector2 startPosition = _targetPosition + new Vector2(0, 400);
        Vector2 targetVelocity = (startPosition - NPC.Center);
        Vector2 interpVelocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
        NPC.velocity = interpVelocity;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

    */
        DungDefenderRocks();
        _targetOutlineColor = Color.Yellow;

        if (Timer >= DungDefenderWarningTime)
        {
            if (AttackCycle >= 4)
            {
                SwitchState(AIState.IdleDrill);
            }
            else
            {
                SwitchState(AIState.DungDefenderRock_Blast);
            }
        }
    }
    private void AI_DungDefenderRockBlast()
    {
        Animator.PlayAnimation(Anim_SpinFast);
        Timer++;
        if (Timer == 1)
        {
            _crashed = false;
            Teleport(_targetPosition + new Vector2(0, 400));
            WarpSegments();
        }

        _renderDashTrail = true;
        _contactDamage = true;
        _targetOutlineColor = Color.Red;
        Vector2 shootVelocity = -Vector2.UnitY * 45;
        NPC.velocity = shootVelocity;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        ShakeScreenPosition.Shake = 3;
        if (Timer > 200 || NPC.Center.Y < MyTarget.Bottom.Y)
        {
            SwitchState(AIState.DungDefenderRock_End);
        }
    }

    private void AI_DungDefenderRockEnd()
    {
        Animator.PlayAnimation(Anim_SpinSlow);
        Timer++;
        if (Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 spawnPosition = NPC.Center;
                    spawnPosition.X += Main.rand.NextFloat(-64, 64);
                    spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                    float dir = Main.rand.NextBool(2) ? -1 : 1;
                    Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-12, -17);
                    spawnVelocity.X = dir * Main.rand.NextFloat(2f, 3f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition, spawnVelocity,
                        ModContent.ProjectileType<Bedrock>(), BedrockDamage, 1, Main.myPlayer);
                }
            }

            GroundImpact();
            SoundStyle smash = AssetRegistry.Sounds.Melee.HammerSmash3;
            smash.PitchVariance = 0.3f;
            SoundEngine.PlaySound(smash, NPC.position);
        }

        if (NPC.velocity.Y < 0)
        {
            NPC.velocity.Y += 0.125f;
            NPC.velocity.Y *= 0.65f;
        }
        else
        {
            NPC.velocity.Y += 0.5f;
            if (NPC.velocity.Y > 25)
            {
                NPC.velocity.Y = 25;
            }
        }

        for (int i = 0; i < Chain.points.Length; i++)
        {
            ref Vector2 p = ref Chain.points[i];
            p += NPC.velocity;
        }

        if (Timer >= 45)
        {
            AttackCycle++;
            if (AttackCycle < 4)
            {
                SwitchState(AIState.DungDefenderRock_Start);
            }
            else
            {
                SwitchState(AIState.IdleDrill);
            }
        }
    }
    #endregion

    private void WarpSegments()
    {
        for (int i = 0; i < Chain.points.Length; i++)
        {
            Chain.points[i] = NPC.Center + Vector2.UnitY * i * 5;
        }
    }

    private void MakeSteamParticlesRandomlyAtSegments()
    {
        for (int i = 0; i < Chain.points.Length; i++)
        {
            Vector2 point = Chain.points[i];
            if (Main.rand.NextBool(150))
            {
                var zap = LegacyParticle.NewParticle<ZapParticle>(point + Main.rand.NextVector2Circular(32, 32), Main.rand.NextVector2Circular(1, 1), Color.White, 1f);
                zap.innerColor = Color.Goldenrod;
                zap.outerColor = Color.Lerp(zap.innerColor, Color.Black, 0.5f);
                zap.fadeToColor = Color.Lerp(zap.outerColor, Color.Black, 0.5f);
            }

            if (Main.rand.NextBool(150))
            {
                Vector2 spawnPosition = point;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Main.rand.NextVector2Circular(2, 2);

                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
            }
        }
    }

    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            if (!_isMainWorm)
            {
                SwitchState(_nextAttackToDo);
                return;
            }

            _nextAttackToDo = PatternManager.NextPattern();
       //     _nextAttackToDo = AIState.DungDefenderRock_Start;
            SwitchState(_nextAttackToDo);
            if (!_phase2 && NPC.life < NPC.lifeMax * 0.5f)
            {
                SwitchState(AIState.Phase_Transition);
            }
        }
    }

    #region Idle States
    private void AI_SpawnDrill()
    {
        _isMainWorm = true;
        ShowNamePlate();
        SwitchState(AIState.IdleDrill);
    }

    private void AI_IdleDrill()
    {
        AttackCycle = 0;
        _fromMeteorRain = false;
        _quickDrill = false;
        _driller2 = false;
        if (!_isMainWorm && _delayTimer < 60)
        {
            NPC.velocity *= 0;
            _delayTimer++;
            return;
        }

        for (int i = 0; i < SteamRollerSegments.Length; i++)
        {
            var segment = SteamRollerSegments[i];
            segment.glowColor = Color.Black;
            segment.animationState = SteamrollerSegment.SteamrollerAnimationState.Spin_Slow; 
        }
        Animator.PlayAnimation(Anim_SpinSlow);
        Timer++;
        if (Timer == 1)
        {
            _currentSpeed = NPC.velocity.Length();
            NPC.TargetClosest();
        }

        if (NPC.velocity.Y < 0)
        {
            NPC.velocity.Y += 0.125f;
            NPC.velocity.Y *= 0.65f;
        }
        else
        {
            NPC.velocity.Y += 0.5f;
            if (NPC.velocity.Y > 25)
            {
                NPC.velocity.Y = 25;
            }
        }
        for (int i = 0; i < Chain.points.Length; i++)
        {
            ref Vector2 p = ref Chain.points[i];
            p += NPC.velocity;
        }
        /*
        Vector2 undergroundPosition = MyTarget.Center + new Vector2(0, 1500);
        Vector2 vel = (undergroundPosition - NPC.Center).SafeNormalize(Vector2.Zero);

        float ratio = Timer / 90f;
        float ease = EasingFunction.InOutSine(ratio);
        float speed = MathHelper.Lerp(_currentSpeed, 30, ease);

        float distToTarget = Vector2.Distance(undergroundPosition, NPC.Center);
        if (speed < distToTarget)
            speed = distToTarget;
        NPC.velocity = vel * speed;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;*/
        if (Timer >= IdleTime)
        {
            ChooseAttack();
        }
    }
    #endregion

    private void AI_Driller()
    {
        Timer++;
        if (Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), 
                    NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<SteamrollerImpactShockwave>(), ShockwaveDamage, 1, Main.myPlayer);
            }
            SoundStyle sound = AssetRegistry.Sounds.SteamPunking.SteamrollerDig;
            sound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(sound, NPC.position);
        }

        ShakeScreenPosition.Shake = 4;
        float ratio = Timer / 30f;
        float ease = EasingFunction.QuadraticBump(ratio);
        _squishScale = Vector2.Lerp(Vector2.One, new Vector2(1.2f, 0.9f), ease);

        Vector2 bottom = NPC.Top - Vector2.UnitY * 64;
        Point point = bottom.ToTileCoordinates();
        while (!WorldGen.SolidTile(point))
            point.Y++;
        bottom = point.ToWorldCoordinates();
        if (Main.rand.NextBool(4))
        {
            Vector2 vel = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(80));
            vel *= Main.rand.NextFloat(8f, 25);
            var spawnParams = DustParticleSpawnParams.Default;

            spawnParams.outerColor = Color.Red;
            var dp = DustParticle.Spawn(bottom, vel, spawnParams);
            dp.fast = true;

            vel = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(80));
            vel *= Main.rand.NextFloat(1, 2);

            var sp = SmokeParticle.Spawn(bottom, vel);
            sp.initialColor = Color.Brown * 0.5f;
            sp.fadeToColor = Color.Transparent;

            if (Main.rand.NextBool(3))
            {
                Vector2 spawnPosition = bottom;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Vector2.UnitX * (Main.rand.NextBool(2) ? -5 : 5);
                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
            }

            if (Main.rand.NextBool(3))
            {
                Vector2 spawnPosition = bottom;
                spawnPosition.X += Main.rand.NextFloat(-64, 64);
                spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -50);
                float spawnScale = Main.rand.NextFloat(0.75f, 1f);
                Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
            }
        }

        if (Main.rand.NextBool(3))
        {
            Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
            int d = WorldGen.KillTile_MakeTileDust(point.X, point.Y, Framing.GetTileSafely(point));
            Dust dust = Main.dust[d];
            dust.velocity = spawnVelocity;
            dust.noLightEmittence = true;
        }

        if (Main.rand.NextBool(6) && Main.netMode != NetmodeID.Server)
        {
            Vector2 spawnPosition = bottom;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);
            spawnPosition.Y += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-5, -15);
            ModContent.GetInstance<FlyingSoilSystem>().NewSoil(spawnPosition, spawnVelocity);
        }

        for (int i = 0; i < SteamRollerSegments.Length; i++)
        {
            SteamRollerSegments[i].paused = true;
        }

        MakeSteamParticlesRandomlyAtSegments();
        float dt = DrillTime;
        if (_driller2)
        {
            if (MultiplayerHelper.IsHost && Timer < dt)
            {
                if (Timer % 10 == 0)
                {
                    Vector2 spawnPosition = bottom;
                    spawnPosition.X += Main.rand.NextFloat(-64, 64);

                    Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-15, -25);
                    spawnVelocity.X = Main.rand.NextFloat(-15, 15);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition, spawnVelocity,
                        ModContent.ProjectileType<Bedrock>(), BedrockDamage, 1, Main.myPlayer);
                }
            }
        }


    
        if (_quickDrill)
            dt *= 0.2f;
        if (Timer < dt)
        {
            NPC.velocity.Y *= 0.01f;
            NPC.velocity.X *= 0.01f;
            NPC.velocity.Y += 0.15f;
            if (Timer < 60)
            {
                Animator.PlayAnimation(Anim_SpinSlow);
            }
            else
            {
                Animator.PlayAnimation(Anim_SpinFast);
            }

        }
        else
        {
            if (Timer == DrillTime)
            {
                SoundStyle kaboom = AssetRegistry.Sounds.SteamPunking.Steamrollerheadingdown;
                SoundEngine.PlaySound(kaboom, NPC.position);
            }

            NPC.velocity.Y += 0.45f;
            NPC.velocity.Y *= 1.025f;
            if (_quickDrill)
            {
                AttackCycle++;
                SwitchState(AIState.DungDefenderRock_Start);
            }
            else if (Timer > dt + 60)
            {
                SwitchState(AIState.IdleDrill);

            }
        }
    }

    #region X Drill
    private void AI_XDrillStart()
    {
        Timer++;
        if (Timer == 1)
        {
            _startVelocity = NPC.velocity;
            NPC.TargetClosest();
            _targetPosition = MyTarget.Bottom;
        }

        //X Appears on the ground
        if (Timer == 1 && MultiplayerHelper.IsHost)
        {
            Projectile.NewProjectile(NPC.GetSource_FromAI(), MyTarget.Bottom, Vector2.Zero,
                ModContent.ProjectileType<RedX>(), 1, 1, Main.myPlayer);
        }

        //Ease in to the start position for the attack
        float ratio = Timer / XDrillWarningTime;
        float ease = EasingFunction.InOutSine(ratio);
        Vector2 startPosition = _targetPosition + new Vector2(0, 1000);
        Vector2 targetVelocity = (startPosition - NPC.Center);
        Vector2 interpVelocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
        NPC.velocity = interpVelocity;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

        _targetOutlineColor = Color.Yellow;
        if (Timer >= XDrillWarningTime)
        {
            if (_variant == AttackVariant.Snagret)
            {
                SwitchState(AIState.Snagret_PopRise);
            }
            else if (_variant == AttackVariant.Fall)
            {
                SwitchState(AIState.DuneJump_Rise);
            }
            else
            {
                SwitchState(AIState.X_Drill_Rise);
            }

        }
    }

    private void GroundImpact()
    {
        int[] gores = AutoGoreLoader.FindGores("GrayRock");
        foreach (int g in gores)
        {
            Gore.NewGore(NPC.GetSource_FromThis(),
                NPC.Center,
                -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
        }

        var p = Particle<ThickSmokeParticle>.Spawn(NPC.Bottom, Vector2.Zero, Color.DarkGray);

        var sear = LegacyParticle.NewParticle<SearParticle>(NPC.Center, Vector2.Zero);
        sear.innerColor = Color.Gray;
        sear.outerColor = Color.Blue;
        sear.fadeToColor = Color.Black;
        FXUtil.ShakeCamera(NPC.Center, 1024, 8);
        ShakeScreenPosition.Shake = 2;


        for (float f = 0; f < 4f; f++)
        {
            Vector2 pos = NPC.Center;
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
            Gore.NewGore(NPC.GetSource_FromThis(),
                NPC.Center,
                -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
        }
        FXUtil.ShakeCamera(NPC.Center, 1024, 64);

        var p3 = FXUtil.GlowCircleBoom(NPC.Center,
           innerColor: Color.Gray,
           glowColor: Color.Red,
           outerGlowColor: Color.DarkRed, duration: 15, baseSize: .09f);
        p3.Scale *= 4;

        smashSound.PitchVariance = 0.2f;
        SoundEngine.PlaySound(smashSound, NPC.position);


        var part = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, Vector2.Zero, Color.White);
        part.fadeToColor = Color.Black;
        part.outerColor = Color.Gray;
        part.noStretch = true;
        part.shrink = true;

        var part2 = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, Vector2.Zero, Color.White);
        part2.fadeToColor = Color.Black;
        part2.outerColor = Color.Gray;
        part2.noStretch = true;
        part2.color *= 0.5f;
        for (float f = 0; f < 5; f++)
        {
            Vector2 vel = Main.rand.NextVector2Circular(16, 16);
            vel.Y -= 10;
            var d = Dust.NewDustPerfect(NPC.Center,
                ModContent.DustType<GlowSparkleDust>(), newColor: Color.Gray, Scale: Main.rand.NextFloat(0f, 2f), Velocity: vel);

        }

        for (float f = 0; f < 16; f++)
        {
            Vector2 vel = -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(80));
            vel *= Main.rand.NextFloat(8f, 50);
            var spawnParams = DustParticleSpawnParams.Default;
            spawnParams.scaleRange *= 2f;
            spawnParams.outerColor = Color.Red;
            DustParticle.Spawn(NPC.Center, vel, spawnParams);
        }

    }

    private void AI_XDrillRise()
    {
        Animator.PlayAnimation(Anim_SpinSlow);
        Timer++;
        if (Timer == 1)
        {
            _crashed = false;
            WarpSegments();
        }

        _renderDashTrail = true;
        _contactDamage = true;
        _targetOutlineColor = Color.Red;
        Vector2 shootVelocity = -Vector2.UnitY * 45;
        NPC.velocity = shootVelocity;
        NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        ShakeScreenPosition.Shake = 3;
        if (Timer > 200 || NPC.Center.Y < MyTarget.Bottom.Y)
        {
            switch (_variant)
            {
                default:
                    SwitchState(AIState.X_Drill_Fall);
                    break;
                case AttackVariant.Dung:
                    SwitchState(AIState.X_Drill_Fall);
                    break;
                case AttackVariant.Snagret:
                    SwitchState(AIState.Snagret_PopFallNStuckk);
                    break;
                case AttackVariant.Fall:
                    SwitchState(AIState.DuneJump_Fall);
                    break;
            }
        }
    }

    private void AI_XDrillFall()
    {
        Timer++;
        if (Timer == 1)
        {
            if (_variant == AttackVariant.Fall)
            {
                float dir = NPC.Center.X < MyTarget.Center.X ? 1 : -1;
                _jumpSpeed = dir;
                _jumpSpeed *= 21;
                if (MultiplayerHelper.IsHost)
                {
                    for (int i = 0; i < 18; i++)
                    {
                        Vector2 spawnPosition = NPC.Center;
                        spawnPosition.X += Main.rand.NextFloat(-64, 64);
                        spawnPosition.Y += Main.rand.NextFloat(-64, 64);

                        Vector2 spawnVelocity = Vector2.UnitY * Main.rand.NextFloat(-12, -17);
                        spawnVelocity.X = dir * Main.rand.NextFloat(2f, 15f);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition, spawnVelocity,
                            ModContent.ProjectileType<Bedrock>(), BedrockDamage, 1, Main.myPlayer);
                    }

                }
            }
            _currentSpeed = NPC.velocity.X;
            _crashed = false;
            GroundImpact();

            SoundStyle smash = AssetRegistry.Sounds.Melee.HammerSmash3;
            smash.PitchVariance = 0.3f;
            SoundEngine.PlaySound(smash, NPC.position);

            SoundStyle steaming = AssetRegistry.Sounds.SteamPunking.MechSteaming;
            steaming.PitchVariance = 0.3f;
            steaming.Volume = 0.5f;
            SoundEngine.PlaySound(steaming, NPC.position);
        }

        _targetOutlineColor = Color.Red;
        _contactDamage = true;
        _renderDashTrail = true;
        Vector2 targetPos = Vector2.Lerp(MyTarget.Center, NPC.Center, 0.35f);
        if (Timer < 70)
            CameraTargetSystem.AddTarget(targetPos);

        for (int i = 0; i < _steamrollerSegments.Length; i++)
        {
            var segment = _steamrollerSegments[i];
            segment.glowColor = Color.Lerp(Color.Transparent, Color.Red, EasingFunction.InOutSine(Timer / 60f)) * ExtraMath.Osc(0f, 1f, speed: 10, offset: i);
        }

        if (_variant == AttackVariant.Dung)
        {
            if (NPC.velocity.Y < 0)
                NPC.velocity.Y *= 0.9f;
            if (NPC.velocity.Y < 25)
                NPC.velocity.Y += 0.5f;

            if (NPC.velocity.Y > 12)
            {
                Animator.PlayAnimation(Anim_SpinFast);
                NPC.velocity.Y *= 1.1f;
            }
            else
            {
                Animator.PlayAnimation(Anim_SpinSlow);
            }

            if (NPC.velocity.Y > 50)
                NPC.velocity.Y = 50;

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
        }
        else if (_variant == AttackVariant.Snagret)
        {
            if (NPC.velocity.Y < -0.5f)
            {
                NPC.velocity.Y *= 0.9f;
                // NPC.velocity.X = MathF.Sin(Timer * 0.04f) * 8;
            }
            else
            {
                Vector2 targetVel = (MyTarget.Center - NPC.Center);
                float rot = targetVel.ToRotation();
                rot += MathHelper.PiOver2;
                NPC.velocity += targetVel.SafeNormalize(Vector2.Zero) * MathHelper.Lerp(0f, 4f, EasingFunction.OutExpo((Timer - 90) / 90f));
                NPC.rotation = Utils.AngleLerp(NPC.rotation, rot, 0.1f);
            }
        }
        else
        {
            if (NPC.velocity.Y < 0)
                NPC.velocity.Y *= 0.97f;
            if (NPC.velocity.Y < 25)
                NPC.velocity.Y += 0.5f;

            if (NPC.velocity.Y > 12)
            {
                Animator.PlayAnimation(Anim_SpinFast);
                NPC.velocity.Y *= 1.03f;

            }
            else
            {
                Animator.PlayAnimation(Anim_SpinSlow);
            }

            if (NPC.velocity.Y > 30)
                NPC.velocity.Y = 30;
            float xDirectionToTarget = NPC.Center.X < MyTarget.Center.X ? 1 : -1;
            float dist = MathF.Abs(MyTarget.Center.X - NPC.Center.X);
            float xSpeed = xDirectionToTarget * dist * 0.25f;

            if (_variant == AttackVariant.Dung)
            {
                NPC.velocity.X *= 0.9f;
            }
            else if (_variant == AttackVariant.Fall)
                NPC.velocity.X = MathHelper.Lerp(_currentSpeed, _jumpSpeed, EasingFunction.InOutSine(Timer / 25f));
            else if (Timer < 45)
                NPC.velocity.X = MathHelper.Lerp(_currentSpeed, xSpeed, EasingFunction.InOutSine(Timer / 60f));

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

        }

        WaitForAndDrill();
        MakeSteamParticlesRandomlyAtSegments();
    }

    private void WaitForAndDrill()
    {

        Vector2 bottom = NPC.Bottom + Vector2.UnitY * 64;
        Point tilePoint = bottom.ToTileCoordinates();
        if (WorldGen.InWorld(tilePoint.X, tilePoint.Y) && Timer > 20)
        {
            Tile tile = Main.tile[tilePoint];
            if (WorldGen.SolidTile(tile) && !_crashed)
            {
                _crashed = true;
                SoundStyle smash2 = AssetRegistry.Sounds.Melee.HammerSmash2;
                smash2.PitchVariance = 0.3f;
                SoundEngine.PlaySound(smash2, NPC.position);

                FXUtil.ShakeCamera(NPC.Center, 1024, 24);
                if (_variant == AttackVariant.Snagret)
                {
                    _driller2 = true;
                }
                if (_variant == AttackVariant.Dung)
                {
                    _quickDrill = true;
                }

                SwitchState(AIState.Driller);

            }
        }

    }

    #endregion

    #region Trail Visuals
    private Vector2 GetDrawOrigin()
    {
        if (_animator == null)
            return NPC.frame.Size() / 2f;
        Vector2? drawOrigin = _animator.GetDrawOrigin();
        if (drawOrigin.HasValue)
            return drawOrigin.Value;
        return NPC.frame.Size() / 2f;
    }

    private Color DashTrailColorFunction(float completionRatio)
    {
        return Color.Lerp(Color.White, Color.Transparent, completionRatio) * _dashTrailAlpha;
    }

    private float DashTrailWidthFunction(float completionRatio)
    {
        return MathHelper.SmoothStep(64, 64, completionRatio);
    }
    private void RenderPixelatedDashTrail(GraphicsDevice gDevice)
    {
        BasicLaserShader laserShader = BasicLaserShader.Instance;
        laserShader.LaserTexture = AssetManager.LaserTextures.SplittingTrail;
        laserShader.InnerColor = Color.Yellow;
        laserShader.OuterColor = Color.Lerp(Color.Yellow, Color.Red, ExtraMath.Osc(0f, 1f, speed: 16));
        TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, DashTrailColorFunction, DashTrailWidthFunction, laserShader, NPC.Size * 0.5f);
    }
    #endregion

    #region Draw Code
    public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        string texturePath = Texture;
        Texture2D texture = TextureAssets.Npc[Type].Value;
        Vector2 drawPos = NPC.Center - screenPos;

        Vector2 drawOrigin = GetDrawOrigin();
        float drawRotation = NPC.rotation;
        Vector2 drawScale = _squishScale * NPC.scale;
        SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        if (NPC.spriteDirection == -1)
            drawOrigin.X = NPC.frame.Size().X - drawOrigin.X;

        spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, drawRotation, drawOrigin, drawScale, spriteEffects, 0f);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (_steamrollerSegments == null)
            return false;

        PixelationManager.QueuePrimitivesDrawAction(RenderPixelatedDashTrail);


        if (_targetOutlineColor != Color.Transparent)
        {
            SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
            spriteBatch.Restart(effect: whiteShader.Effect);
            float outlineScale = 2;
            Vector2 left = Vector2.UnitX * -outlineScale;
            Vector2 right = Vector2.UnitX * outlineScale;
            Vector2 up = Vector2.UnitY * -outlineScale;
            Vector2 down = Vector2.UnitY * outlineScale;
            Draw(spriteBatch, screenPos + left, _outlineColor);
            Draw(spriteBatch, screenPos + right, _outlineColor);
            Draw(spriteBatch, screenPos + up, _outlineColor);
            Draw(spriteBatch, screenPos + down, _outlineColor);
            spriteBatch.RestartDefaults();
        }


        Draw(spriteBatch, screenPos, drawColor);
        int segmentsToDraw = _steamrollerSegments.Length - 1;
        if (IsSmall)
            segmentsToDraw /= 2;
        for (int i = 1; i < segmentsToDraw; i++)
        {
            SteamrollerSegment segment = _steamrollerSegments[i];
            segment.isDying = _isDying;
            Vector2 pos = Chain.points[i];
            Vector2 nextPos = Chain.points[i + 1];
            Color lightingColor = Lighting.GetColor(pos.ToTileCoordinates());
            segment.Draw(spriteBatch, pos, nextPos, drawColor);
        }

        return false;
    }

    #endregion

    public override void OnKill()
    {
        base.OnKill();
        DownedBossTracker.ClearFlag(DownedBossFlag.Steamroller);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);

        if (NPC.life <= 0)
        {
            NPC.life = 1;
        }
    }

    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        //   throw new NotImplementedException();


    }
}
