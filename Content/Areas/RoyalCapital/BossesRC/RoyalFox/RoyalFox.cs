using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Areas.MoonspiralTower.VerliaBoss.Projectiles;
using Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.InverseKinematics;
using Stellamod.Core.Palettes;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox;


public partial class RoyalFox : ScarletBoss,
    IDrawToRenderTarget
{
    private Asset<Texture2D> _sigilTextureAsset;
    private enum TailAnimation : byte
    {
        Limp = 0,
        Loose = 1,
        Full_Control = 2
    }

    private float _roaringCircleScale;
    private float _roaringCircleAlpha;
    private Color _roaringCircleColor;
    private TailAnimation _tailAnimation;
    private Vector2 _startPosition;
    private Vector2 _ballPosition;
    private Vector2 _eyeFlashPosition;
    private Vector2 _eyeFlashOffset;
    private float _eyeFlashAlpha;

    private Vector2 _initialStartDashPosition;
    private Vector2 _initialStartDashVelocity;

    private Vector2 _teleportPosition;
    private Vector2 _startDashPoint;
    private Vector2 _dashLineVelocity;
    private float _dashTrailAlpha;
    private bool _renderDashTrail;
    private bool _renderMotionBlur;
    private float _laserTelegraphAlpha;


    private float _telegraphLineAlpha;
    private bool _showTelegraphLine;

    private float _invisibleAlpha;
    private bool _goInvisible;
    private bool _dontRender;
    private bool _tailInFront;

    private float _direction;
    private Outliner _outliner;
    private bool _contactDamage;
    private RoyalFoxRig _rigBackingField;
    private RoyalFoxRig Rig
    {
        get
        {
            _rigBackingField ??= CreateRig();
            return _rigBackingField;
        }
    }

    private ref float Timer => ref NPC.ai[0];
    private enum AIState
    {
        Spawn,
        Despawn,
        Idle,

        Zoom_SparkleStarRain,
        Zoom_DashDance,
        Zoom_CometStarDash,
        Zoom_BigFatLaser,

        Precision_OutOfBreathTransition,
        Precision_SwordSlashChase,
        Precision_SpinningCharge,
        Precision_CometTeleportShots
    }

    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];

    private float _miniAttackCount;

    private Vector2[] _tailEndIK;
    private Vector2[] TailEndIK
    {
        get
        {
            if (_tailEndIK == null)
            {
                _tailEndIK = new Vector2[3];
            }
            return _tailEndIK;
        }
    }

    private Armature[] _tails;
    private Armature[] Tails
    {
        get
        {
            if (_tails == null)
            {
                _tails = new Armature[3];
                for (int i = 0; i < _tails.Length; i++)
                {
                    _tails[i] = new Armature(144, 4);
                    for (int k = 0; k < _tails[i].segments.Length; k++)
                    {
                        var segment = _tails[i].segments[k];
                        segment.rangeOfMotion = 10f;
                        segment.rootDirection = -Vector2.UnitX;
                    }

                    _tails[i].SetDefaults();
                }
            }

            return _tails;
        }
    }

    private ref float RegularRotation => ref Rig.rootSegment.eulerAngles.W;
    private ref float ZRotation => ref Rig.rootSegment.eulerAngles.X;

    private Vector2 HeadPosition => Rig.headPart.worldPosition;

    //Dash Dance Attack
    private int DashDanceDamage => 80;
    private float NumDashDanceLines => 10;
    private int NumDashDanceBursts => 4;
    private float DashDanceTime => 11;

    //Comet Star Dash
    private int CometStarDamage => 100;
    private float CometStarDashPrepTime => 240;
    private float CometStarDashTime
    {
        get
        {
            float time = 15;
            if (AttackCounter != 0)
                time = 27;


            return time;
        }
    }
    private float CometStarDashMiniPrepTime => 90;
    private float CometStarDashEndingTime => 70;
    private float DelayBetweenDashDanceBursts => 25;

    //Big Fat Laser
    private float BigStarPrepTime => 100;
    private int BigStarCometDamage => 40;
    private int BigFatLaserDamage => 120;
    private float BigFatLaserPrepTime => 100;
    private float BigFatLaserChargeTime => 100;
    private float BigFatLaserFireTime => 72;
    public Texture2D GetSubTexture(string fileName)
    {
        string path = Texture + $"_{fileName}";
        return ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad).Value;
    }

    public RoyalFoxRig CreateRig()
    {
        Texture2D[] backLegTextures = new Texture2D[3];
        backLegTextures[0] = GetSubTexture("BackThigh");
        backLegTextures[1] = GetSubTexture("BackLeg");
        backLegTextures[2] = GetSubTexture("Foot");

        Texture2D[] frontLegTextures = new Texture2D[3];
        frontLegTextures[0] = GetSubTexture("FrontThigh");
        frontLegTextures[1] = GetSubTexture("FrontLeg");
        frontLegTextures[2] = GetSubTexture("Foot");

        Texture2D head = GetSubTexture("Head");

        Texture2D[] bodyTextures = new Texture2D[4];
        bodyTextures[0] = GetSubTexture("Body3");
        bodyTextures[1] = GetSubTexture("Body2");
        bodyTextures[2] = GetSubTexture("Body1");
        bodyTextures[3] = GetSubTexture("Neck");

        var rig = new RoyalFoxRig(backLegTextures, frontLegTextures, bodyTextures, head);
        return rig;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    public override BossLevel GetBossLevel()
    {
        return BossLevel.Superboss;
    }

    private float Ground => 16000;
    private void EnablePlatformArena()
    {
        DomainExpansionManager fallSystem = ModContent.GetInstance<DomainExpansionManager>();
        fallSystem.noWings = true;
        fallSystem.inSpace = true;
        fallSystem.hoveringPlatform = true;
        fallSystem.hoverPlatformY = Ground;
   //     fallSystem.noProjTileCollide = true;
        if (Main.netMode == NetmodeID.Server)
            return;
        ModContent.GetInstance<FenixDomain>().drawFenix = true;
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_startDashPoint);
        writer.WriteVector2(_dashLineVelocity);
        writer.WriteVector2(_teleportPosition);
        writer.WriteVector2(_ballPosition);
        writer.Write(_miniAttackCount);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _startDashPoint = reader.ReadVector2();
        _dashLineVelocity = reader.ReadVector2();
        _teleportPosition = reader.ReadVector2();
        _ballPosition = reader.ReadVector2();
        _miniAttackCount = reader.ReadSingle();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.TrailCacheLength[NPC.type] = 32;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCID.Sets.MustAlwaysDraw[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 90;
        NPC.height = 90;
        NPC.damage = 150;
        NPC.defense = 20;
        NPC.lifeMax = 200000;
        NPC.scale = 1f;
        NPC.aiStyle = -1;

        NPC.value = Item.buyPrice(gold: 5);
        NPC.knockBackResist = 0f;
        NPC.boss = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.npcSlots = 30f;

        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/AlcaricFox");
        NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
    }


    private void Teleport(Vector2 position)
    {
        if (!MultiplayerHelper.IsHost)
            return;
        _teleportPosition = position;
        NPC.netUpdate = true;
    }


    public override void AI()
    {
        base.AI();
        _contactDamage = false;
        _renderMotionBlur = false;
        _renderDashTrail = false;
        _goInvisible = false;
        _dontRender = false;
        _showTelegraphLine = false;
        _tailInFront = false;
        _roaringCircleAlpha = 0;
        _laserTelegraphAlpha = 0;
        _tailAnimation = TailAnimation.Limp;
        _eyeFlashAlpha = MathHelper.Lerp(_eyeFlashAlpha, 0f, 0.1f);
        _outliner.SetDefaults();
        EnablePlatformArena();
        switch (State)
        {
            default:
            case AIState.Idle:
                AI_Idle();
                break;

            case AIState.Zoom_DashDance:
                AI_ZoomDashDance();
                break;

            case AIState.Zoom_CometStarDash:
                AI_CometStarDash();
                break;

            case AIState.Zoom_BigFatLaser:
                _tailInFront = true;
                AI_BigFatLaser();
                break;
        }

        switch (_tailAnimation)
        {
            case TailAnimation.Limp:
                DragTailsLiimp();
                break;
            case TailAnimation.Loose:
                DragTailsLoose();
                break;
            case TailAnimation.Full_Control:
                SimulateHairIK();
                break;
        }

        float targetTelegraphLineAlpha = _showTelegraphLine ? 1f : 0f;
        _telegraphLineAlpha = MathHelper.Lerp(_telegraphLineAlpha, targetTelegraphLineAlpha, 0.1f);

        if (_teleportPosition != Vector2.Zero)
        {
            if(_tailAnimation == TailAnimation.Loose)
            {
                for(int i = 0; i < Tails.Length; i++)
                {
                  for(int j = 0; j < Tails[i].segments.Length; j++)
                    {
                        Tails[i].segments[j].a = NPC.Center;
                    }
                }
            }
            NPC.Center = _teleportPosition;
            _teleportPosition = Vector2.Zero;
        }

        float targetInvisibleAlpha = _goInvisible ? 0f : 1f;
        _invisibleAlpha = MathHelper.Lerp(_invisibleAlpha, targetInvisibleAlpha, 0.1f);

        float targetDashTrailAlpha = _renderDashTrail ? 1f : 0f;
        _dashTrailAlpha = MathHelper.Lerp(_dashTrailAlpha, targetDashTrailAlpha, 0.1f);
        _outliner.Update();
        UpdateRig();
    }

    private void SwitchState(AIState state)
    {
        _miniAttackCount = 0;
        Timer = 0;
        AttackCycle = 0;
        AttackCounter = 0;
        State = state;
        NPC.netUpdate = true;
        Main.NewText(state);
    }

    private void DebugTeleportLeftOfPlayer()
    {

        Vector2 pos = (MyTarget.Center + new Vector2(-512, 0));
        NPC.velocity = Vector2.Zero;
        NPC.Center = pos;
    }

    private void AI_Idle()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
        }
        DebugTeleportLeftOfPlayer();
        NPC.velocity *= 0.8f;
        if (Timer >= 100)
        {
            ChooseAttack();
        }
        AnimateStanding();
    }

    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            SwitchState(AIState.Zoom_CometStarDash);
        }
    }

    private float Zoom_Prepare_Time => 280;
    private void AnimateCrouching()
    {

    }

    public Vector2 CalculateLaserSpawnPoint(float ratio = 1f)
    {
        Vector2 startDashPoint = _initialStartDashPosition.RotatedBy(MathHelper.PiOver4, _ballPosition);
        Vector2 dashLineVelocity = _initialStartDashVelocity.RotatedBy(MathHelper.PiOver4, _ballPosition);

        float inOut = EasingFunction.OutExpo(ratio);
        float slowIn = EasingFunction.InExpo(ratio);
        Vector2 movementPos = Vector2.Lerp(startDashPoint, dashLineVelocity, inOut);
        Vector2 backPos = Vector2.Lerp(dashLineVelocity, _ballPosition, slowIn);
        Vector2 pos = Vector2.Lerp(movementPos, backPos, ratio * 0.99f);
        return pos;
    }

    public Vector2 CalculateLaserSpawnVelocity()
    {
        Vector2 endPoint = CalculateLaserSpawnPoint(1f);
        Vector2 pointBefore = CalculateLaserSpawnPoint(0.99f);
        return (endPoint - pointBefore).SafeNormalize(Vector2.Zero);
    }

    private void AnimateStanding()
    {
        float start = MathHelper.ToRadians(-2);
        float end = MathHelper.ToRadians(2);

        float runningSpeed = 4;
        float frontFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed);
        //     float easeing = EasingFunction.InOutSine(legPair1);
        Rig.frontFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontFrontLeg);
        Rig.frontFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontFrontLeg);

        float frontBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 1);
        Rig.frontBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontBackLeg);
        Rig.frontBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontBackLeg);


        //Back Legs
        float backFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3);
        Rig.backFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backFrontLeg);
        Rig.backFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backFrontLeg);


        float backBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3 + 1);
        Rig.backBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backBackLeg);
        Rig.backBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backBackLeg);


        float headRotOffset = MathHelper.Lerp(start, end, ExtraMath.Osc(0f, 1f, speed: runningSpeed));
        Rig.bodyParts[3].eulerAngles.Z = MathHelper.ToRadians(19) + headRotOffset;
    }

    private void AnimateRunning()
    {
        float start = MathHelper.ToRadians(-25);
        float end = MathHelper.ToRadians(25);

        float runningSpeed = 9;
        float frontFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed);
        Rig.frontFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontFrontLeg);
        Rig.frontFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontFrontLeg);

        float frontBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 1);
        Rig.frontBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontBackLeg);
        Rig.frontBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontBackLeg);

        //Back Legs
        float backFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3);
        Rig.backFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backFrontLeg);
        Rig.backFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backFrontLeg);


        float backBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3 + 1);
        Rig.backBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backBackLeg);
        Rig.backBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backBackLeg);


        Rig.bodyParts[3].eulerAngles.Z = MathHelper.ToRadians(15);
    }
    private void AnimateTorpedo()
    {
        float start = MathHelper.ToRadians(65);
        float end = start + MathHelper.ToRadians(2);

        float runningSpeed = 9;
        float frontFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed);
        float targetAngle = MathHelper.Lerp(start, end, frontFrontLeg);
        Rig.frontFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(Rig.frontFrontLeg[0].eulerAngles.Z, targetAngle, 0.1f);
        Rig.frontFrontLeg[1].eulerAngles.Z = 0;

        float frontBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 1);
        targetAngle = MathHelper.Lerp(start, end, frontBackLeg);

        Rig.frontBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(Rig.frontBehindLeg[0].eulerAngles.Z, targetAngle, 0.1f);
        Rig.frontBehindLeg[1].eulerAngles.Z = 0;

        //Back Legs
        float backFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3);
        start = MathHelper.ToRadians(65);
        end = start + MathHelper.ToRadians(2);
        targetAngle = MathHelper.Lerp(start, end, backFrontLeg);
        Rig.backFrontLeg[0].fakeAngle = MathHelper.Lerp(Rig.backFrontLeg[0].fakeAngle, MathHelper.ToRadians(35), 0.1f);
        Rig.backFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(Rig.backFrontLeg[0].eulerAngles.Z, targetAngle, 0.1f);
        Rig.backFrontLeg[1].eulerAngles.Z = 0;
        Rig.backFrontLeg[2].fakeAngle = MathHelper.Lerp(Rig.backFrontLeg[2].fakeAngle, MathHelper.ToRadians(45), 0.1f);

        float backBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3 + 1);
        targetAngle = MathHelper.Lerp(start, end, backBackLeg);
        Rig.backBehindLeg[0].fakeAngle = MathHelper.Lerp(Rig.backBehindLeg[0].fakeAngle, MathHelper.ToRadians(35), 0.1f);
        Rig.backBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(Rig.backBehindLeg[0].eulerAngles.Z, targetAngle, 0.1f);
        Rig.backBehindLeg[1].eulerAngles.Z = 0;
        Rig.backBehindLeg[2].fakeAngle = MathHelper.Lerp(Rig.backBehindLeg[2].fakeAngle, MathHelper.ToRadians(-45), 0.1f);

        Rig.bodyParts[3].eulerAngles.Z = MathHelper.ToRadians(15);
    }
    private void AnimateStretched()
    {
        float start = MathHelper.ToRadians(-45);
        float end = MathHelper.ToRadians(-15);

        float runningSpeed = 9;
        float frontFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed);
        Rig.frontFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontFrontLeg);
        Rig.frontFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontFrontLeg);

        float frontBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 1);
        Rig.frontBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, frontBackLeg);
        Rig.frontBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, frontBackLeg);

        start = MathHelper.ToRadians(45);
        end = MathHelper.ToRadians(15);

        //Back Legs
        float backFrontLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3);
        Rig.backFrontLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backFrontLeg);
        Rig.backFrontLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backFrontLeg);


        float backBackLeg = ExtraMath.Osc(0f, 1f, speed: runningSpeed, offset: 3 + 1);
        Rig.backBehindLeg[0].eulerAngles.Z = MathHelper.Lerp(start, end, backBackLeg);
        Rig.backBehindLeg[1].eulerAngles.Z = MathHelper.Lerp(start * 2, end * 2, backBackLeg);


        Rig.bodyParts[3].eulerAngles.Z = MathHelper.ToRadians(15);
    }


    #region Zoom Mode 

    private void CreateDashLines()
    {
        if (MultiplayerHelper.IsHost)
        {
            for (int i = 0; i < NumDashDanceLines; i++)
            {
                Vector2 posToPutLine = Vector2.Zero;
                posToPutLine.X = MathHelper.Lerp(-750, 750, i / NumDashDanceLines);
                posToPutLine.Y = Main.rand.NextFloat(-300, 300);
                posToPutLine += MyTarget.Center;

                Vector2 velocity = (posToPutLine - MyTarget.Center).RotatedByRandom(MathHelper.ToDegrees(45)).SafeNormalize(Vector2.Zero);
                if (i == 0)
                {
                    velocity = (posToPutLine - MyTarget.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);
                }

                Projectile.NewProjectile(SourceFromThis, posToPutLine, velocity, ModContent.ProjectileType<DashLine>(), DashDanceDamage, 1,
                    Main.myPlayer, ai0: i * -2);
            }
        }
    }

    private void CommandStars()
    {
        foreach (var proj in Main.ActiveProjectiles)
        {
            if (proj.type != ModContent.ProjectileType<RoyalMagicMiniStar>())
                continue;
            proj.ai[1] = 1;
        }
    }



    private void AI_BigFatLaser()
    {
        bool FindBall(out Projectile ball)
        {
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type != ModContent.ProjectileType<RoyalStarBomb>())
                    continue;
                if (proj.ai[1] != NPC.whoAmI)
                    continue;
                ball = proj;
                return true;
            }
            ball = null;
            return false;
        }

        void BounceBall(float rot)
        {
            if (FindBall(out Projectile ball))
            {
                ball.ai[2] = rot;
            }
        }

        void GrabBall()
        {
            if (FindBall(out Projectile ball))
            {
                ball.ai[2] = 10;
            }
        }

        void ExplodeBall()
        {
            if (FindBall(out Projectile ball))
            {
                ball.ai[2] = 11;
            }
        }


        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        PoofParticles(NPC.Center);
                        float dir = Main.rand.NextBool(2) ? 1 : -1;
                        Vector2 pointToTeleportTo = MyTarget.Center + new Vector2(600 * dir, -192);
               
                        Teleport(pointToTeleportTo);
                        PoofParticles(pointToTeleportTo);
                        _ballPosition = pointToTeleportTo;

                        ShakeScreenPosition.Shake = 8;
                        var fx = FXUtil.GlowCircleBoom(pointToTeleportTo, Color.White, Color.SkyBlue, Color.DarkBlue, duration: 30, baseSize: 0.2f); ;
                        fx.Scale *= 2f;
                        for (int i = 0; i < 32; i++)
                        {
                            var dp = DustParticle.Spawn(pointToTeleportTo, Main.rand.NextVector2Circular(24, 24));
                            dp.outerColor = Color.DarkBlue;
                            dp.dampening = 0.1f;
                            dp.noTileCollide = true;
                            dp.gravity = 0;
                            dp.Scale *= 1.5f;
                        }

                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, _ballPosition, Vector2.Zero, ModContent.ProjectileType<RoyalStarBomb>(), BigFatLaserDamage, 1, Main.myPlayer, ai1: NPC.whoAmI);
                        }

                        PixelPrimitiveCircleFactory.CreateGenericInBoom(pointToTeleportTo, Color.White, Color.Transparent, 60, 512);
                        PixelPrimitiveCircleFactory.CreateGenericBoom(pointToTeleportTo, Color.White, Color.Transparent, 60, 512);
                    }

                    WalkParticles();
                    AnimateTorpedo();

                    Vector2 offset = Vector2.UnitY * MathHelper.Lerp(0, 252, EasingFunction.OutExpo(Timer / (BigFatLaserPrepTime / 1.5f)));
                    offset = offset.RotatedBy(MathHelper.Pi * (Timer / BigFatLaserPrepTime));
                    Vector2 newPoint = _ballPosition + offset;
                    Vector2 vel = newPoint - NPC.Center;
                    NPC.velocity = vel;

                    ZRotation += MathHelper.Lerp(0.25f, 0f, Timer / BigFatLaserPrepTime);
                    RegularRotation = NPC.velocity.ToRotation();
                    _outliner.warning = true;
                    _renderDashTrail = true;
                    if (Timer >= BigFatLaserPrepTime)
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
                        Vector2 up = (NPC.Center - _ballPosition).SafeNormalize(Vector2.Zero);
                        Vector2 forward = up.RotatedBy(MathHelper.ToRadians(90));
                        _startDashPoint = NPC.Center;
                        _dashLineVelocity = NPC.Center + forward * 512;
                        SoundStyle chargeSound = AssetRegistry.Sounds.AlcaricFox.FenixChargin;
                        SoundEngine.PlaySound(chargeSound, MyTarget.Center);
                    }


                    float ratio = Timer / BigStarPrepTime;
                    _roaringCircleScale = MathHelper.SmoothStep(5f, 0f, ratio);
                    _roaringCircleAlpha = MathHelper.SmoothStep(0f, 1f, EasingFunction.QuadraticBump(ratio));
                    _roaringCircleColor = Color.Lerp(Color.Pink, Color.Blue, ratio);

                    NPC.velocity *= 0.94f;
                    RegularRotation = MathHelper.Lerp(RegularRotation, _dashLineVelocity.ToRotation(), 0.1f);
                    ZRotation += MathHelper.Lerp(0f, 0.2f, Timer / BigStarPrepTime);

                    _outliner.warning = true;

                    _tailAnimation = TailAnimation.Limp;
                    AnimateTorpedo();

                    if (Timer % 30 == 0)
                    {
                        PixelPrimitiveCircleFactory.CreateGenericInBoom(NPC.Center, Color.Transparent, Color.White * 0.5f, 35, 500);
                    }


                    ChargeParticles(HeadPosition, in Timer);

                    if (Timer >= BigStarPrepTime - 60)
                    {
                        float t = Timer - (BigStarPrepTime - 60);
                        float pr = t / 30f;
                        _eyeFlashAlpha = EasingFunction.QuadraticBump(pr);
                        _eyeFlashPosition = Rig.headPart.worldPosition;
                        _eyeFlashOffset = Vector2.Zero;

                        float pr2 = t / 60f;
                        Vector2 vel = _dashLineVelocity.SafeNormalize(Vector2.Zero);
                        NPC.velocity = Vector2.Lerp(vel, -vel, EasingFunction.QuickOutSlowIn(pr2)) * MathHelper.Lerp(1f, 8f, EasingFunction.InOutSine(pr2));
                    }

                    if (Timer >= BigStarPrepTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 2:
                {
                    //So we need to pick an offset direction and make ovals basically and slam back into the ball
                    //Lets just do perp to our position
                    if (Timer == 1)
                    {
                        Vector2 up = (NPC.Center - _ballPosition).SafeNormalize(Vector2.Zero);
                        Vector2 forward = up.RotatedBy(MathHelper.ToRadians(90));
                        _startDashPoint = NPC.Center;
                        _dashLineVelocity = NPC.Center + forward * 666;
                        _initialStartDashPosition = _startDashPoint;
                        _initialStartDashVelocity = _dashLineVelocity;
                        if(AttackCounter == 0)
                        {
                            SoundStyle chargeSound = AssetRegistry.Sounds.AlcaricFox.FenixWindStartup;
                            SoundEngine.PlaySound(chargeSound, MyTarget.Center);
                        }
                    }

                 

                    float rotAmount = MathHelper.ToRadians(45) / BigFatLaserFireTime;

                    _startDashPoint = _startDashPoint.RotatedBy(rotAmount, _ballPosition);
                    _dashLineVelocity = _dashLineVelocity.RotatedBy(rotAmount, _ballPosition);


                    float ratio = Timer / BigFatLaserFireTime;
                    float inOut = EasingFunction.OutExpo(ratio);
                    float slowIn = EasingFunction.InExpo(ratio);
                    Vector2 movementPos = Vector2.Lerp(_startDashPoint, _dashLineVelocity, inOut);
                    Vector2 backPos = Vector2.Lerp(_dashLineVelocity, _ballPosition, slowIn);
                    Vector2 pos = Vector2.Lerp(movementPos, backPos, ratio * 0.99f);
                    _laserTelegraphAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(ratio));
                    _renderDashTrail = true;


                    if (Main.rand.NextBool(2))
                    {
                        Vector2 endPoint = CalculateLaserSpawnPoint();
                        Vector2 endVelocity = CalculateLaserSpawnVelocity();
                        endPoint += endVelocity * 92;
                        endPoint += Main.rand.NextVector2Circular(64, 64);
                        var dp = DustParticle.Spawn(endPoint + endVelocity * 32, endVelocity * 8);
                        dp.outerColor = Color.Blue;
                        dp.dampening = 0.1f;
                        dp.noTileCollide = true;
                        dp.gravity = 0;
                        dp.Scale *= 0.66f;
                    }

                    RegularRotation = (pos - NPC.Center).ToRotation();
                    ZRotation += MathHelper.Lerp(0f, 0.25f, EasingFunction.InOutExpo(Timer / BigFatLaserFireTime));

                    Vector2 targetPos = Vector2.Lerp(MyTarget.Center, NPC.Center, 0.35f);

                    CameraTargetSystem.AddTarget(targetPos);
                    AnimateStretched();
                    if (Timer > BigFatLaserFireTime * 0.5f)
                        WalkParticles();

                    //Again with such quick movement just set it directly
                    _outliner.attacking = true;
                    NPC.velocity = Vector2.Zero;
                    NPC.Center = pos;
                    if (Timer >= BigFatLaserFireTime)
                    {
                        PlayImpactSound(MyTarget.Center);
                        if (MultiplayerHelper.IsHost)
                        {

                            Vector2 vel = (_ballPosition - NPC.Center).SafeNormalize(Vector2.Zero);
                            BounceBall(vel.ToRotation());
                            //   vel *= -1;
                            vel *= 3500;
                            Projectile.NewProjectile(SourceFromThis, _ballPosition, vel,
                                ModContent.ProjectileType<RoyalMagicBeam>(), BigFatLaserDamage, 1, Main.myPlayer);
                        }
                        Timer = 0;

                        AttackCounter++;
                        if (AttackCounter >= 7)
                        {
                            AttackCycle++;
                        }
                        else
                        {

                        }
                        // AttackCycle++;
                    }
                }
                break;

            case 3:
                {
                    float time = BigFatLaserFireTime * 3;
                    //Lets just do perp to our position
                    if (Timer == 1)
                    {
                        SoundStyle chargeSound = AssetRegistry.Sounds.AlcaricFox.FenixWindStartup;
                        SoundEngine.PlaySound(chargeSound, MyTarget.Center);
                        Vector2 up = (NPC.Center - _ballPosition).SafeNormalize(Vector2.Zero);
                        Vector2 forward = up.RotatedBy(MathHelper.ToRadians(90));
                        _startDashPoint = NPC.Center;
                        _dashLineVelocity = NPC.Center + forward * 700;
                    }

                    float rotAmount = MathHelper.ToRadians(180) / time;

                    _startDashPoint = _startDashPoint.RotatedBy(rotAmount, _ballPosition);
                    _dashLineVelocity = _dashLineVelocity.RotatedBy(rotAmount, _ballPosition);
                    float ratio = Timer / time;
                    float inOut = EasingFunction.OutExpo(ratio);


                    float slowInRatio = Timer - time * 0.78f;
                    float slowEase = EasingFunction.InExpo(slowInRatio / (time * 0.22f));
                    float slowIn = MathHelper.Lerp(0f, 1f, slowEase);
                    Vector2 movementPos = Vector2.Lerp(_startDashPoint, _dashLineVelocity, inOut);
                    Vector2 backPos = Vector2.Lerp(_dashLineVelocity, _ballPosition, slowIn);
                    Vector2 pos = Vector2.Lerp(movementPos, backPos, ratio * 0.99f);

                    _renderDashTrail = true;


                    RegularRotation = (pos - NPC.Center).ToRotation();
                    ZRotation += MathHelper.Lerp(0f, 0.25f, EasingFunction.InOutExpo(Timer / time));

                    Vector2 targetPos = Vector2.Lerp(MyTarget.Center, NPC.Center, 0.5f);
                    CameraTargetSystem.AddTarget(targetPos);
                    AnimateStretched();
                    if (Timer > time * 0.5f)
                        WalkParticles();

                    //Again with such quick movement just set it directly
                    _outliner.attacking = true;
                    NPC.velocity = Vector2.Zero;
                    NPC.Center = pos;
                    if (Timer >= time)
                    {
                        PlayImpactSound(MyTarget.Center);
                        Vector2 vel = (_ballPosition - NPC.Center).SafeNormalize(Vector2.Zero);
                        _dashLineVelocity = vel;
                        GrabBall();
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 4:
                {
                    NPC.velocity = _dashLineVelocity * 80;
                    RegularRotation = _dashLineVelocity.ToRotation();
                    ZRotation += 0.25f;

                    AnimateStretched();
                    WalkParticles();
                    _renderDashTrail = true;
                    if (Timer >= 60)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 5:
                {
                    _goInvisible = true;
                    NPC.velocity *= 0.98f;
                    if (Timer >= 60)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 6:
                {
                    float time = BigFatLaserFireTime * 3;
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        Vector2 tteleportPos = MyTarget.Center - new Vector2(1500, -500);
                        Teleport(tteleportPos);
                        PoofParticles(tteleportPos);
                        _startDashPoint = tteleportPos;


                        _dashLineVelocity = MyTarget.Center;
                        _dashLineVelocity.Y -= 1000;
                    
                    }

                    WalkParticles();
                    AnimateStretched();
                    _renderDashTrail = true;

                    _startDashPoint.X += 4;
                    _dashLineVelocity.X += 4;
             
                    float ratio = Timer / time;
                    float inOut = EasingFunction.OutExpo(ratio);
                    _dashLineVelocity.Y += MathHelper.Lerp(1, 4, EasingFunction.InOutExpo(ratio));

                    float slowInRatio = Timer - time * 0.78f;
                    float slowEase = EasingFunction.InExpo(slowInRatio / (time * 0.22f));
                    float slowIn = MathHelper.Lerp(0f, 1f, slowEase);

                    Vector2 endPoint = _dashLineVelocity + Vector2.UnitX * 384;
                    endPoint.Y = Ground;
                    Vector2 movementPos = Vector2.Lerp(_startDashPoint, _dashLineVelocity, inOut);
                    Vector2 backPos = Vector2.Lerp(_dashLineVelocity, endPoint, slowIn);
                    backPos = Vector2.Lerp(backPos, MyTarget.Center, slowIn);
                    Vector2 pos = Vector2.Lerp(movementPos, backPos, ratio * 0.99f);

                    if (Timer % 4 == 0)
                    {
                        var donute = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -_dashLineVelocity * 3);
                    }


                    Vector2 targetPos = Vector2.Lerp(MyTarget.Center, NPC.Center, 0.5f);
                    CameraTargetSystem.AddTarget(targetPos);
                    RegularRotation = (pos - NPC.Center).ToRotation();
                    ZRotation += MathHelper.Lerp(0f, 0.25f, EasingFunction.InOutExpo(Timer / time));

                    _outliner.attacking = true;
                    NPC.velocity = Vector2.Zero;
                    NPC.Center = pos;

                    if (Timer >= time)
                    {
                 
                        ExplodeBall();
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            default:
                SwitchState(AIState.Idle);
                break;
        }
    }


    private void AI_CometStarDash()
    {
        /*
         * 
         * Squishes in her body parts as a starting charge (first time this happens is at the drop, 
         * then it's random), then she starts her zoom mode,
         * before she takes off she has an arrow deciding which direction she’ll go, 
         * and it tries to follow the player loosely. And she does an extremely fast dash, 
         * with a super star trail and comets flying down from her starting point in a lobbing motion, 
         * she zooms, straight and goes off screen, and then another line will come 
         * and you have to dodge before she zooms really fast with the same attack, 
         * this is a one shot btw, having the cool uh circles and whatnot idk look at bayle

         */
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        _dashLineVelocity = Vector2.Zero;
                        NPC.TargetClosest();
                    }

                    Vector2 directionToTarget = (MyTarget.Center - NPC.Center);
                    directionToTarget = directionToTarget.SafeNormalize(Vector2.Zero);
                    _dashLineVelocity = _dashLineVelocity.MoveTowards(directionToTarget, 0.5f);
                    _startDashPoint = NPC.Center;

                    RegularRotation = MathHelper.Lerp(RegularRotation, _dashLineVelocity.ToRotation(), 0.1f);

                    float ratio = Timer / CometStarDashPrepTime;
                    _roaringCircleScale = MathHelper.SmoothStep(5f, 0f, ratio);
                    _roaringCircleAlpha = MathHelper.SmoothStep(0f, 1f, EasingFunction.QuadraticBump(ratio));
                    _roaringCircleColor = Color.Lerp(Color.Pink, Color.Blue, ratio);
                    _outliner.warning = true;



                    ZRotation += MathHelper.Lerp(0.05f, 0.15f, EasingFunction.InOutExpo(ratio));

                    Vector2 lerp1 = Vector2.Lerp(Vector2.Zero, -_dashLineVelocity * 32, EasingFunction.InOutExpo(ratio));
                    Vector2 lerp2 = Vector2.Lerp(Vector2.Zero, _dashLineVelocity * 8, EasingFunction.InExpo(ratio));
                    Vector2 lerp3 = Vector2.Lerp(lerp1, lerp2, ratio);
                    Vector2 lerp4 = Vector2.Lerp(Vector2.Zero, _dashLineVelocity * 8, EasingFunction.InExpo(ratio));
                    Vector2 lerp5 = Vector2.Lerp(lerp3, lerp4, ratio);

                    NPC.velocity = lerp5;

                    AnimateTorpedo();
                    _showTelegraphLine = true;
                    if (Timer % 30 == 0)
                    {
                        PixelPrimitiveCircleFactory.CreateGenericInBoom(HeadPosition, Color.Transparent, Color.White, 35, 500);
                    }

                    ChargeParticlesBig(HeadPosition, in Timer);

                    if (Timer == CometStarDashPrepTime - 170)
                    {
                        SoundStyle dashSound = AssetRegistry.Sounds.AlcaricFox.FenixChargin;
                        SoundEngine.PlaySound(dashSound, MyTarget.Center);
                    }

                    if (Timer == CometStarDashPrepTime - 110)
                    {
                        SoundStyle dashSound = AssetRegistry.Sounds.AlcaricFox.FenixSonicSpeedBoost;
                        SoundEngine.PlaySound(dashSound, MyTarget.Center);
                    }

                    if (Timer >= CometStarDashPrepTime - 30)
                    {

                        float t = Timer - (CometStarDashPrepTime - 30);
                        float pr = t / 30f;
                        _eyeFlashAlpha = EasingFunction.QuadraticBump(pr);
                        _eyeFlashPosition = Rig.headPart.worldPosition;
                        _eyeFlashOffset = Vector2.Zero;

                    }
                    if (Timer >= CometStarDashPrepTime)
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
                    }
                    if (Timer == 3)
                    {
                        if (AttackCounter != 0)
                        {
                            SoundStyle boomSound = AssetRegistry.Sounds.AlcaricFox.FenixBooma;
                            SoundEngine.PlaySound(boomSound, MyTarget.Center);
                        }
                        if (AttackCounter == 0)
                        {
                            ShockwavePlayer shockwavePlayer = Main.LocalPlayer.GetModPlayer<ShockwavePlayer>();
                            shockwavePlayer.Bee = 120;
                            shockwavePlayer.shockwavePosition = NPC.Center;
                            shockwavePlayer.rippleSize = 5;

                        }

                        if (MultiplayerHelper.IsHost)
                        {
                            int damage = BigFatLaserDamage;
                            if (AttackCounter == 0)
                                damage += 20;
                            Projectile.NewProjectile(SourceFromThis, _startDashPoint, _dashLineVelocity * 2500, ModContent.ProjectileType<RoyalMagicStarryDashTrail>(), damage, 1, Main.myPlayer, ai1: NPC.whoAmI);
        
                            for (int i = 0; i < 15; i++)
                            {
                                Vector2 launchVelocity = _dashLineVelocity.RotatedBy(MathHelper.ToRadians(-45)) * 45;
                                float dir = 1;
                                if (Main.rand.NextBool(2))
                                    dir *= -1;
                                launchVelocity.X *= dir;
                                if (launchVelocity.Y > 0)
                                    launchVelocity.Y *= -1;

                                Vector2 startPos = Vector2.Lerp(_startDashPoint, _startDashPoint + _dashLineVelocity * 2500, Main.rand.NextFloat(0f, 1f));
                                Projectile.NewProjectile(SourceFromThis, startPos, launchVelocity, ModContent.ProjectileType<RoyalMagicComet>(), BigStarCometDamage, 1, Main.myPlayer);
                            }
                        }
                        FXUtil.CreateRipple(_startDashPoint);
                        FXUtil.ShakeCamera(_startDashPoint, 1024, 2);


                        for (int i = 1; i < 5; i++)
                        {
                            var tp = ThrustParticle.Spawn(_startDashPoint, _dashLineVelocity * 14 * i, Scale: 2);
                            tp.bloomColor = Color.White;
                        }
                        for (int i = 1; i < 16; i++)
                        {
                            var tp = DustParticle.Spawn(_startDashPoint + Main.rand.NextVector2Circular(40, 40), _dashLineVelocity * 14 * i);
                            tp.Scale *= 2;
                            tp.outerColor = Color.Violet;
                            tp.gravity = 0;
                            tp.dampening = 0.1f;
                            tp.noTileCollide = true;
                        }
                    }

                    var sp = RoyalMagicStarParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(64, 64), _dashLineVelocity, Scale: Main.rand.NextFloat(0.15f, 0.25f));
                    sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));
                    if (Timer % 2 == 0)
                    {
                        var donute = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -_dashLineVelocity * 3);
                    }


                    float ratio = Timer / CometStarDashTime;

                    var d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(32, 32), DustID.GemDiamond);
                    d.noGravity = true;

                    ShakeScreenPosition.Shake = MathHelper.Lerp(16, 2, EasingFunction.InOutExpo(ratio));
                    RegularRotation = _dashLineVelocity.ToRotation();
                    ZRotation = 0;

                    //Velocity is unreliable for how fast this movement is
                    //So we just set the position directly.
                    Vector2 endDashPoint = _startDashPoint + _dashLineVelocity * 3000;
                    Vector2 pointToMoveTo = Vector2.Lerp(_startDashPoint, endDashPoint, ratio);
                    NPC.velocity = Vector2.Zero;
                    NPC.Center = pointToMoveTo;

                    if (Timer % 5 == 0)
                    {
                        FXUtil.CreateRipple(NPC.Center);
                    }



                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 spawnPos = NPC.Center;
                        spawnPos += Main.rand.NextVector2Circular(64, 64);
                        SirestiasSparkleParticle sireSparkle = SirestiasSparkleParticle.Spawn(spawnPos, Vector2.Zero);
                        sireSparkle.gravity = 0;
                        sireSparkle.noTileCollide = true;
                        sireSparkle.Scale *= 0.1f;
                        sireSparkle.fast = true;
                        sireSparkle.outerColor = Color.Yellow;
                    }


                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 spawnPos = NPC.Center;
                        spawnPos += Main.rand.NextVector2Circular(64, 64);
                        SparkleParticle sireSparkle = SparkleParticle.Spawn(spawnPos, Vector2.Zero);
                        sireSparkle.gravity = 0;
                        sireSparkle.noTileCollide = true;
                        sireSparkle.Scale *= 0.1f;
                        sireSparkle.fast = true;
                        sireSparkle.outerColor = Color.Yellow;
                    }


                    WalkParticles2();
                    AnimateTorpedo();

                    _outliner.attacking = true;
                    if (Timer >= CometStarDashTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                        if (AttackCounter < 3)
                        {
                            AttackCounter++;
                        }
                        else
                        {
                            AttackCycle++;
                        }

                    }
                }
                break;
            case 2:
                {
                    if (Timer == 1)
                    {
                        _startDashPoint = MyTarget.Center + new Vector2(-1000, 0);
                        PoofParticles();
                        NPC.TargetClosest();
                    }

                    _goInvisible = true;

                    Vector2 targetDashLineVElocity = (MyTarget.Center - _startDashPoint).SafeNormalize(Vector2.Zero);
                    _dashLineVelocity = Vector2.Lerp(_dashLineVelocity, targetDashLineVElocity, 0.05f);
                    _outliner.warning = true;
                    _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(Timer / CometStarDashMiniPrepTime));
                    _showTelegraphLine = true;

                    AnimateTorpedo();
                    if (Timer >= CometStarDashMiniPrepTime)
                    {
                        Timer = 0;
                        AttackCycle--;
                    }
                }
                break;
            case 3:
                {
                    if (Timer == 1)
                    {
                        _startDashPoint = MyTarget.Center - new Vector2(1000, 0);
                        _dashLineVelocity = (MyTarget.Center - _startDashPoint);
                        PoofParticles();
                        Teleport(MyTarget.Center - new Vector2(1000, 0));
                        NPC.TargetClosest();
                    }

                    float ratio = Timer / CometStarDashEndingTime;
                    Vector2 endDashPoint = _startDashPoint + _dashLineVelocity;
                    Vector2 pointToMoveTo = Vector2.Lerp(_startDashPoint, endDashPoint, EasingFunction.OutExpo(ratio));
                    RegularRotation = _dashLineVelocity.ToRotation();
                    ZRotation = MathHelper.Lerp(MathHelper.ToRadians(90), MathHelper.ToRadians(360), EasingFunction.OutExpo(ratio));
                    AnimateTorpedo();
                    NPC.velocity = Vector2.Zero;
                    NPC.Center = pointToMoveTo;
                    //Timer++;
                    if (Timer >= CometStarDashEndingTime - 55)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            default:
                SwitchState(AIState.Zoom_DashDance);
                break;
        }
    }
    private void AI_ZoomDashDance()
    {
        (Vector2, Vector2) NextDashLine()
        {

            (Vector2 position, Vector2 velocity) dashLine = new(Vector2.Zero, Vector2.Zero);
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type != ModContent.ProjectileType<DashLine>())
                    continue;
                if (proj.ai[1] > 0)
                    continue;
                proj.ai[1] = 1;
                dashLine.position = proj.Center;
                dashLine.velocity = proj.velocity;
                break;
            }
            return dashLine;
        }

        bool HasNextDashLine()
        {
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type != ModContent.ProjectileType<DashLine>())
                    continue;
                if (proj.ai[1] > 0)
                    continue;
                return true;
            }
            return false;
        }
        //Fenix flies up and does the like cogwork dancers thing where a bunch of lines appear and she dashes through them really fast, this is a two shot btw
        //For this attack, we'll create a new blurring shader for the motion blur
        //And also have cool effects for the trailing

        //PART ONE:
        //Let's break it down
        //First, fenix, with a bit of anticipation, slowly flies up and teleports/fades out, cool starry/smoke visuals on this
        //For the starry/smoke part, we'll create a new smoke effect

        //PART TWO:
        //We generate a bunch of positions that Fenix will dash through, this can just be a projectile, they fade in around her target
        //She then dashes through each of the lines one by one with a really fast and cool blurring shader

        //PART THREE
        //She probably does the attack 3 times before ending the attack and going back to her cycle
        //The first dash has quite a bit of anticipation btw.

        Timer++;
        switch (AttackCycle)
        {
            case 0:
                if (Timer == 1)
                {
                    NPC.TargetClosest();
                    _direction = FacingDirectionToTarget;
                }

                _outliner.warning = true;
                _renderDashTrail = true;

                {
                    float progress = Timer / Zoom_Prepare_Time;

                    //  float xBackUp = MathHelper.Lerp(0, _direction * -8, EasingFunction.QuadraticBump(Timer / Zoom_Prepare_Time));
                    float xSpeedUp1 = MathHelper.Lerp(MathHelper.Lerp(_direction * 15, _direction * 0, EasingFunction.InExpo(Timer / (Zoom_Prepare_Time / 2))), _direction * 180, EasingFunction.InOutExpo(Timer / Zoom_Prepare_Time));
                    float xSpeedUp2 = MathHelper.Lerp(MathHelper.Lerp(0.4f, 0f, EasingFunction.InExpo(Timer / (Zoom_Prepare_Time * 0.5f))), 1f, EasingFunction.InExpo(Timer / (Zoom_Prepare_Time)));
                    float xSpeedUp = MathHelper.SmoothStep(xSpeedUp2, xSpeedUp1, progress);
                    NPC.velocity.X = xSpeedUp;
                    NPC.velocity.X += MathHelper.Lerp(_direction * 5, 0, EasingFunction.InOutExpo(Timer / (Zoom_Prepare_Time * 0.5f)));
                    RegularRotation = NPC.velocity.ToRotation();
                    //     ZRotation = MathHelper.Lerp(0f, MathHelper.ToRadians(90 + 360), EasingFunction.InOutExpo(progress));
                    //    AnimateRunning();
                    WalkParticles();
                    CreateFootsteps();
                }

                ZRotation += MathHelper.Lerp(0.08f, 0.45f, EasingFunction.InOutExpo(Timer / Zoom_Prepare_Time));
                AnimateTorpedo();


                if (Timer > Zoom_Prepare_Time / 2f)
                {
                    if (Timer % 2 == 0)
                    {
                        var donute = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero) * 3);
                        donute.Scale *= 2f;
                    }




                    int zT = (int)(Zoom_Prepare_Time / 2f);
                    zT += 4;
                    if (Timer == zT)
                    {
                        SoundStyle fastDash = AssetRegistry.Sounds.AlcaricFox.FenixWindStartup;
                        SoundEngine.PlaySound(fastDash, NPC.position);
                    }
                }

                if (Timer == Zoom_Prepare_Time - 75)
                {
                    CreateDashLines();
                }
                if (Timer >= Zoom_Prepare_Time - 60)
                {
                    PoofParticles();
                    Timer = 0;
                    AttackCycle++;
                }
                break;

            //how htis is gonna work is dash line sare gonna appear
            //and as logn as a dash line projectile exists she'll dash through them all

            case 1:
                if (Timer == 1)
                {
                    NPC.TargetClosest();

                }

                _goInvisible = true;
                if (Timer >= DelayBetweenDashDanceBursts)
                {
                    Timer = 0;
                    AttackCycle++;
                }

                NPC.velocity *= 0.98f;
                break;
            case 2:

                if (Timer == 1)
                {

                    //  PoofParticles();
                    (Vector2 position, Vector2 velocity) = NextDashLine();
                    if (position != default(Vector2))
                    {

                        _dashLineVelocity = velocity;
                        position -= velocity * 384;
                        _startDashPoint = position;
                        Teleport(position);
                        NPC.netUpdate = true;
                    }
                }

                if (Timer < 3)
                {
                    _goInvisible = true;
                }

                if (Timer == 3)
                {
                    PlayDashSound(MyTarget.Center);
                    FXUtil.CreateRipple(_startDashPoint);
                    FXUtil.ShakeCamera(_startDashPoint, 1024, 2);
                    ShakeScreenPosition.Shake = 8;

                    for (int i = 1; i < 5; i++)
                    {
                        var tp = ThrustParticle.Spawn(_startDashPoint, _dashLineVelocity * 14 * i, Scale: 2);
                        tp.bloomColor = Color.White;
                    }
                    for (int i = 1; i < 16; i++)
                    {
                        var tp = DustParticle.Spawn(_startDashPoint + Main.rand.NextVector2Circular(40, 40), _dashLineVelocity * 14 * i);
                        tp.Scale *= 2;
                        tp.outerColor = Color.Violet;
                        tp.gravity = 0;
                        tp.dampening = 0.1f;
                        tp.noTileCollide = true;

                    }
                }

                if (Timer < 3)
                {
                    _dontRender = true;
                }
                if (Timer == 3)
                {

                }
                float ratio = Timer / DashDanceTime;
                float easing = EasingFunction.InOutExpo(ratio);
                if (_miniAttackCount > 0)
                {
                    easing = 1f;
                }

                if (Timer % 10 == 0)
                {
                    if (MultiplayerHelper.IsHost)
                    {
                        Projectile.NewProjectile(SourceFromThis, Vector2.Lerp(_startDashPoint, _startDashPoint + _dashLineVelocity * 1200, 0.8f), _dashLineVelocity * 9,
                            ModContent.ProjectileType<RoyalMagicMiniStar>(), DashDanceDamage, 1, Main.myPlayer);
                    }
                }

                if (Timer > 3)
                {
                    Vector2 startDashPoint = _startDashPoint;
                    if (!HasNextDashLine())
                    {
                        startDashPoint -= _dashLineVelocity * 1200;
                        // targetPosition = startDashPoint + _dashLineVelocity * 350;
                    }
                    Vector2 targetPosition = startDashPoint + _dashLineVelocity * 700;

                    Vector2 pointToMoveTo = Vector2.Lerp(_startDashPoint, targetPosition, ratio);
                    Vector2 vel = pointToMoveTo - NPC.Center;
                    NPC.velocity = Vector2.Zero;
                    NPC.Center = pointToMoveTo;

                }
                var sp = RoyalMagicStarParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(64, 64), _dashLineVelocity, Scale: Main.rand.NextFloat(0.15f, 0.25f));
                sp.color = Color.Lerp(new Color(117, 100, 210), Color.White, Main.rand.NextFloat(0f, 1f));

                if (Timer % 4 == 0)
                {
                    var donute = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -_dashLineVelocity * 3);
                }

                Rig.rootSegment.eulerAngles.W = _dashLineVelocity.ToRotation();
                Rig.rootSegment.eulerAngles.X = MathHelper.Lerp(0f, MathHelper.ToRadians(360), EasingFunction.InOutSine(_miniAttackCount / NumDashDanceLines));

                _contactDamage = true;
                _outliner.attacking = true;
                _renderMotionBlur = true;
                AnimateTorpedo();


                _tailAnimation = TailAnimation.Loose;
                WalkParticles2();

                var fx = FXUtil.GlowStretch(NPC.Center + Main.rand.NextVector2Circular(32, 32), _dashLineVelocity);
                fx.VectorScale *= 0.5f;
                if (Timer >= DashDanceTime)
                {
                    if (!HasNextDashLine())
                    {
                        AttackCycle++;
                    }
                    Timer = 0;

                }
                break;
            case 3:
                float time = Zoom_Prepare_Time / 2f;
                if (Timer == 1)
                {
                    PixelPrimitiveCircleFactory.CreateGenericInBoom(MyTarget.Center, Color.Transparent, Color.White * 0.35f, 45, 512);
                    CommandStars();
                    if (_direction == 0)
                        _direction = 1;
                    else
                        _direction *= -1;
                }

                Vector2 easeIn = _dashLineVelocity * MathHelper.Lerp(50, 0, EasingFunction.OutExpo(Timer / time));
                Vector2 easeOut = _dashLineVelocity * MathHelper.Lerp(0f, 150, EasingFunction.InOutExpo(Timer / time));
                NPC.velocity = Vector2.Lerp(easeIn, easeOut, EasingFunction.InOutSine(Timer / time));

                if (Timer < 10)
                {
                    _goInvisible = true;
                }

                ZRotation += 0.12f;
                if (Timer > time / 2f)
                {
                    if (Timer % 2 == 0)
                    {
                        var donute = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero) * 3);
                        donute.Scale *= 2f;
                    }


                    AnimateTorpedo();


                    int zT = (int)(time / 2f);
                    zT += 4;
                    if (Timer == zT)
                    {
                        SoundStyle fastDash = AssetRegistry.Sounds.AlcaricFox.FenixWindStartup;
                        SoundEngine.PlaySound(fastDash, NPC.position);
                    }
                }
                else
                {

                    AnimateTorpedo();
                }


                WalkParticles();

                if (Timer == time - 25 && (AttackCounter + 1) < NumDashDanceBursts)
                {
                    CreateDashLines();
                }

                if (AttackCounter + 1 < NumDashDanceBursts)
                {
                    _outliner.warning = true;
                }

                AnimateTorpedo();

                if (Timer >= time)
                {
                    AttackCounter++;
                    Timer = 0;
                    if (AttackCounter >= NumDashDanceBursts)
                    {
                        AttackCycle++;
                    }
                    else
                    {
                        AttackCycle -= 2;
                    }

                }
                break;
            case 4:

                _goInvisible = true;
                if (Timer >= DelayBetweenDashDanceBursts)
                {
                    Timer = 0;
                    AttackCycle++;
                }
                break;
            default:
                SwitchState(AIState.Zoom_BigFatLaser);
                break;
        }
    }
    #endregion
    private void UpdateRig()
    {
        //Calling update twice sine it has to calculate the new x axis position
        //Yeah this is technically inefficient but it's too inexpensive to matter, quick and dirty solution :p
        Rig.rootSegment.worldPosition = NPC.Center;
        Rig.Update();
        Rig.Update();
    }

    public override void OnKill()
    {
        base.OnKill();
    }
}
