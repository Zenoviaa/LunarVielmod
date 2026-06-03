using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Steamworks;
using Stellamod.Assets;
using Stellamod.Content.Areas.MoonspiralTower.VerliaBoss.Projectiles;
using Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.InverseKinematics;
using Stellamod.Core.Palettes;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.Utilities;
using Stellamod.Effects.RoyalMagic;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.RoyalFox;


public partial class RoyalFox : ScarletBoss,
    IDrawToRenderTarget
{
    private Asset<Texture2D> _gravityFieldTextureAsset;
    private Asset<Texture2D> _sigilTextureAsset;
    private enum TailAnimation : byte
    {
        Limp = 0,
        Loose = 1,
        Full_Control = 2
    }

    private float _darkMoonTimer;
    private bool _darkMoon;
    private bool _killYoSelf;
    private float _oldButtRotation;
    private int _precisionAttackCycle;
    private bool _slowDown;
    private float _spinningCRot;
    private float _spiralDashTrailAlpha;
    private float _swingTrailEndRatio;
    private float _swingTrailAlpha;
    private Vector2 _swingVelocity;
    
    //Teleport
    private float _teleportAlpha;
    private Vector2 _teleportTelegraphPosition;

    //Gravity Field effect
    private float _gravityFieldAlpha;

    private Vector2 _wingPos;
    private float _roaringCircleScale;
    private float _roaringCircleAlpha;
    private Color _roaringCircleColor;
    private TailAnimation _tailAnimation;
    private Vector2 _ballPosition;
    private Vector2 _eyeFlashPosition;
    private Vector2 _eyeFlashOffset;
    private float _eyeFlashAlpha;

    private Vector2 _moonPosition;
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
    private bool _canDrawWings;
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

    private VerletChain _verletTail;
    private VerletChain VerletTail
    {
        get
        {
            if(_verletTail == null)
            {
                _verletTail = new VerletChain(128, NPC.Center, -Vector2.UnitX);
                _verletTail.gravity = 0;
            }
            return _verletTail;
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
        Precision_CometTeleportShots,
        Precision_Beyblade,

        Phase2_Transition,
        Zoom_Tired,
        Precision_Tired,
        Death
    }

    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }


    private ref float AttackCycle => ref NPC.ai[2];
   // private ref float AttackCounter => ref NPC.ai[3];
    private bool IsAClone
    {
        get
        {
            return NPC.ai[3] != 0;
        }
        set
        {
            NPC.ai[3] = value ? 1 : 0;
        }
    }
    private float AttackCounter;
    private float _miniAttackCount;

    private TexturedQuad _wingQuad;
    private TexturedQuad WingQuad
    {
        get
        {
            _wingQuad ??= new TexturedQuad();
            return _wingQuad;
        }
    }

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

    private bool _zoomMode;

    private ref float RegularRotation => ref Rig.rootSegment.eulerAngles.W;
    private ref float ZRotation => ref Rig.rootSegment.eulerAngles.X;

    public Vector2 HeadPosition => Rig.headPart.worldPosition;

    private float SlowdownMult => IsADarkMoon() ? 1.25f : 1f;
    //Dash Dance Attack
    private int DashDanceDamage => 80;
    private float NumDashDanceLines => 10;
    private int NumDashDanceBursts => 4;
    private float DashDanceTime => 11 * SlowdownMult;

    //Comet Star Dash
    private int CometStarDamage => 100;
    private float CometStarDashPrepTime => 240 * SlowdownMult;
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
    private float Zoom_Prepare_Time => 280 * SlowdownMult;
    private float CometStarDashMiniPrepTime => 90;
    private float CometStarDashEndingTime => 70;
    private float DelayBetweenDashDanceBursts => 25 * SlowdownMult;

    //Big Fat Laser
    private float BigStarPrepTime => 100 * SlowdownMult;
    private int BigStarCometDamage => 40;
    private int BigFatLaserDamage => 120;
    private float BigFatLaserPrepTime => 100 * SlowdownMult;
    private float BigFatLaserChargeTime => 100;
    private float BigFatLaserFireTime => 72;

    //Sparkle Star Rain
    private int SparkleStarDamage => 70;
    private float SparkleStarRainTime => 490;
    private float TimeBetweenSparkleStars => 60;

    //Tired
    private float TiredTime => 300;

    //Spinning Charge
    private float SpinningChargePrepTime => 120;
    private float SpinningChargeSpeed => MathHelper.Lerp(75, 25, _miniAttackCount / AirbounceChainCount);
    private float AirbounceChainCount => 7;
    private float SpinningChargeBurstCount => 3;

    //Comet Teleport Prep Time
    private float CometTeleportPrepTime => 80;
    private int CometTeleportDamage => 60;
    private float CometBackflipTime => MathHelper.Lerp(100, 50, EasingFunction.OutSine(AttackCounter / CometTeleportCount)) * SlowdownMult;
    private float CometTeleportCount => 21;
    private float CometTeleportEndTime => 45f;

    //Sword Slash
    private float SwordSlashPrepTime => 180;
    private float SwordSlashSlashTime => 100 * (AttackCounter == 0 ? 1.5f : 1) * SlowdownMult;
    private float SwordSlashBetweenTime => 60;
    private float SwordSlashCount => 4;
    private int SwordSlashDamage => 80;

    //Beyblade
    private int FenixSawDamage => 70;
    private float BeybladePrepTime => 120;
    private float BeybladeChargeSpeed => MathHelper.Lerp(75, 25, _miniAttackCount / BeybladeAirbounceChainCount);
    private float BeybladeAirbounceChainCount => 7;
    private float BeybladeChargeBurstCount => 3;
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

        Texture2D headSwordTexture = GetSubTexture("HeadSword");
        var rig = new RoyalFoxRig(backLegTextures, frontLegTextures, bodyTextures, head, headSwordTexture);
        rig.frontLegFrontThighSegment.postDraw = DrawFrontWing;
        rig.frontLegBehindThighSegment.postDraw = DrawBackWing;
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
  
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_startDashPoint);
        writer.WriteVector2(_dashLineVelocity);
        writer.WriteVector2(_teleportPosition);
        writer.WriteVector2(_ballPosition);
        writer.Write(_miniAttackCount);
        writer.Write(_zoomMode);
        writer.Write(_precisionAttackCycle);
        writer.Write(_killYoSelf);
        writer.Write(_darkMoon);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _startDashPoint = reader.ReadVector2();
        _dashLineVelocity = reader.ReadVector2();
        _teleportPosition = reader.ReadVector2();
        _ballPosition = reader.ReadVector2();
        _miniAttackCount = reader.ReadSingle();
        _zoomMode = reader.ReadBoolean();
        _precisionAttackCycle = reader.ReadInt32();
        _killYoSelf = reader.ReadBoolean();
        _darkMoon = reader.ReadBoolean();
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
        _zoomMode = true;
        NPC.width = 90;
        NPC.height = 90;
        NPC.damage = 150;
        NPC.defense = 66;
        NPC.lifeMax = 300000;
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

    private void TryActivateMoon()
    {
        if (!MultiplayerHelper.IsHost)
            return;
        if (!Main.rand.NextBool(2))
            return;
        if (IsAClone)
            return;

        _darkMoon = true;
        NPC.netUpdate = true;
    }

    public bool IsADarkMoon()
    {
        foreach(var npc in Main.ActiveNPCs)
        {
            if (npc.type != Type)
                continue;
            if(npc.ModNPC is RoyalFox fox)
            {
                if (fox._darkMoon)
                    return true;
            }
        }
        return false;
    }

    private bool InPhase2 => NPC.life < NPC.lifeMax * 0.5f;
    private bool _phase2;
    private bool _pressed;
    public bool CanMakeClones()
    {
        return _phase2 && !IsAClone && MultiplayerHelper.IsHost;
    }

    public bool NoClonesAlive()
    {
        int count = 0;
        foreach(var npc in Main.ActiveNPCs)
        {
            if(npc.type == Type)
            {
                count++;
            }
         
        }
        return count == 1;
    }
    private void MakeClone(Vector2 position, AIState state, float attackCycle = 0)
    {
        int x = (int)position.X;
        int y = (int)position.Y;
        NPC.NewNPC(NPC.GetSource_FromThis(), x, y, Type, ai1: (float)state, ai2: attackCycle, ai3: 1);
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


        if (IsADarkMoon())
        {
            _darkMoonTimer++;
            if(_darkMoonTimer == 1)
            {
                _moonPosition = MyTarget.Center;
            }   
        }
        else
        {
            _darkMoonTimer--;
        }

        _darkMoonTimer = MathHelper.Clamp(_darkMoonTimer, 0f, 60);
        Vector2 targetPos = -Vector2.UnitY * 128;
        targetPos = targetPos.RotatedBy(Main.GlobalTimeWrappedHourly);
        Vector2 targetMoonPos = MyTarget.Center + targetPos;
        _moonPosition = Vector2.Lerp(_moonPosition, targetMoonPos, 0.2f);
      

        if (_killYoSelf)
        {
            _goInvisible = true;
            Timer++;
            if(Timer >= 90)
            {
                NPC.active = false;
        
            }
        }

        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
            {
                if (IsAClone)
                {
                    _killYoSelf = true;
                }
                if(State != AIState.Despawn)
                {
                    SwitchState(AIState.Despawn);
                }
            }
        }
        PreUpdateRig();
        _swingTrailAlpha = MathHelper.Lerp(_swingTrailAlpha, 0f, 0.1f);
        _gravityFieldAlpha = MathHelper.Lerp(_gravityFieldAlpha, 0f, 0.1f);
        _spiralDashTrailAlpha = MathHelper.Lerp(_spiralDashTrailAlpha, 0f, 0.1f);
        _spinningCRot = MathHelper.Lerp(_spinningCRot, 0f, 0.1f);
        Rig.useSword = false;
        _tailInFront = true;
      //  _darkMoon = true;
        /*
        if (!_pressed && Keyboard.GetState().IsKeyDown(Keys.L))
        {
            _pressed = true;
        }
        if (_pressed && Keyboard.GetState().IsKeyUp(Keys.L) && !IsAClone)
        {
            _pressed = false;
            _phase2 = true;
            NPC.life = (int)(NPC.lifeMax * 0.49f);
            SwitchState(AIState.Precision_Beyblade);
        }*/

        if (!_killYoSelf)
        {
            switch (State)
            {
                default:

                case AIState.Idle:
                    AI_Idle();
                    break;

                case AIState.Zoom_Tired:
                    AI_ZoomTired();
                    break;

                case AIState.Precision_Tired:
                    AI_PrecisionTired();
                    break;

                case AIState.Despawn:
                    AI_Despawn();
                    break;

                case AIState.Zoom_DashDance:
                    AI_ZoomDashDance();
                    break;

                case AIState.Zoom_CometStarDash:
                    AI_CometStarDash();
                    break;

                case AIState.Zoom_BigFatLaser:

                    AI_BigFatLaser();
                    break;

                case AIState.Zoom_SparkleStarRain:
                    AI_SparkleStarRain();
                    break;

                case AIState.Precision_CometTeleportShots:
                    AI_CometTeleportShots();
                    break;

                case AIState.Precision_SpinningCharge:
                    AI_SpinningCharge();
                    break;

                case AIState.Precision_SwordSlashChase:
                    AI_SwordChase();
                    break;

                case AIState.Precision_Beyblade:
                    AI_Beyblade();
                    break;
            }
        }
 
        //  AddAngularVelocity();
        if(State == AIState.Precision_Beyblade)
            FakeButtTail2();
        else
            FakeButtTail();
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

            NPC.Center = _teleportPosition;
            Rig.rootSegment.eulerAngles.W = 0;
            _oldButtRotation = 0;
            for (int i = 0; i < Tails.Length; i++)
            {
                Vector2 start = _teleportPosition;
                Vector2 end = start - RegularRotation.ToRotationVector2() * 398;
                for (int j = 0; j < Tails[i].segments.Length; j++)
                {
                    Vector2 p =Vector2.Lerp(start, end, (float)j / (float)(Tails[i].segments.Length));
                    ref Vector2 a = ref Tails[i].segments[j].a;
                    ref Vector2 b = ref Tails[i].segments[j].b;

                    a = Vector2.Zero;
                    b = Vector2.Zero;
                }
            }
            _teleportPosition = Vector2.Zero;
        }

        float targetInvisibleAlpha = _goInvisible ? 0f : 1f;
        _invisibleAlpha = MathHelper.Lerp(_invisibleAlpha, targetInvisibleAlpha, 0.1f);

        float targetDashTrailAlpha = _renderDashTrail ? 1f : 0f;
        _dashTrailAlpha = MathHelper.Lerp(_dashTrailAlpha, targetDashTrailAlpha, 0.1f);
        _outliner.Update();
        UpdateRig();
 
        Vector2 diff = Rig.bodyParts[3].worldPosition - _wingPos;
        diff *= 0.8f;
      
        _wingPos += diff; //Vector2.Lerp(_wingPos, Rig.bodyParts[3].worldPosition, 0.4f);

    }

    private void AddAngularVelocity()
    {
        float angleDiff = RegularRotation - _oldButtRotation;
        Vector2 angularVelocity = angleDiff.ToRotationVector2() * 64;
        for (int i = 0; i < Tails.Length; i++)
        {
            for (int j = 0; j < Tails[i].segments.Length; j++)
            {
                ref Vector2 a = ref Tails[i].segments[j].a;
                ref Vector2 b = ref Tails[i].segments[j].b;

                float ratio = (float)j / (float)Tails[i].segments.Length;
               // ratio = 1f - ratio;
                a += angularVelocity;
                b += angularVelocity;
            }
        }
        _oldButtRotation = RegularRotation;
    }

    private float Diff(float a, float b)
    {
        float a1 = MathHelper.ToDegrees(a);
        float a2 = MathHelper.ToDegrees(b);

        float dif = (float)Math.Abs(a1 - a2) % 360;

        if (dif > 180)
            dif = 360 - dif;

        dif = MathHelper.ToRadians(dif);
        return dif;
    }

    private void FakeButtTail2()
    {
        float m = 0.03f;
        float newAngle = Rig.rootSegment.eulerAngles.W;
        float oldAngle = _oldButtRotation;
        float angleDiff = newAngle - oldAngle;
        angleDiff = MathHelper.Clamp(angleDiff, -0.5f, 0.5f);
        for (int i = 0; i < Tails.Length; i++)
        {
            for (int j = 0; j < Tails[i].segments.Length; j++)
            {
                ref Vector2 a = ref Tails[i].segments[j].a;
                ref Vector2 b = ref Tails[i].segments[j].b;

                float ratio = (float)j / (float)Tails[i].segments.Length;
                ratio = 1f - ratio;
                a = a.RotatedBy(angleDiff * ratio, Rig.rootSegment.worldPosition);
                b = b.RotatedBy(angleDiff * ratio, Rig.rootSegment.worldPosition);

            }
        }

        _oldButtRotation += angleDiff;
    }
    private void FakeButtTail()
    {
        float m = 0.03f;
        float newAngle = Rig.rootSegment.eulerAngles.W;
       
        float oldAngle = _oldButtRotation;

        float f = 0.02f;
        float angleDiff = Diff(oldAngle, newAngle) * f;

        //angleDiff = MathHelper.Clamp(angleDiff, -1f, 1f);
        for (int i = 0; i < Tails.Length; i++)
        {
            for (int j = 0; j < Tails[i].segments.Length; j++)
            {
                ref Vector2 a = ref Tails[i].segments[j].a;
                ref Vector2 b = ref Tails[i].segments[j].b;

                float ratio = (float)j / (float)Tails[i].segments.Length;
                ratio = 1f - ratio;
                a = a.RotatedBy(angleDiff * ratio, Rig.rootSegment.worldPosition);
                b = b.RotatedBy(angleDiff * ratio, Rig.rootSegment.worldPosition);

            }
        }

        _oldButtRotation += angleDiff;
    }
    private void SwitchState(AIState state)
    {
        if (IsAClone)
        {
            Timer = 0;
            AttackCycle = 0;
            AttackCounter = 0;
            
            _killYoSelf = true;
          
            return;
        }
        _darkMoon = false;
        if (_phase2)
        {
            TryActivateMoon();
        }
        _miniAttackCount = 0;
        Timer = 0;
        AttackCycle = 0;
        AttackCounter = 0;
        State = state;
        NPC.netUpdate = true;
    }

    private void DebugTeleportLeftOfPlayer()
    {

        Vector2 pos = (MyTarget.Center + new Vector2(-512, 0));
        NPC.velocity = Vector2.Zero;
        NPC.Center = pos;
    }

    public Vector2 CalculateSwingOffset(Vector2 shootVelocity, float interpolant)
    {
        float swingRange = MathHelper.ToRadians(270);
        float ease = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuickOutSlowIn(interpolant));
        float ease2 = MathHelper.Lerp(0f, 1f, EasingFunction.InOutCirc(interpolant));
        float ease3 = MathHelper.Lerp(ease, ease2, EasingFunction.InOutCirc(interpolant));
        Vector2 swingOffset = OvalSwing.CalculateXY(ease3, shootVelocity, swingRange, new Vector2(192, 192), 1);
        return swingOffset;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if(InPhase2 && !_phase2)
        {

            _phase2 = true;
        }
    }

    public override bool? CanBeHitByProjectile(Projectile projectile)
    {
        if (IsAClone)
            return false;
        return base.CanBeHitByProjectile(projectile);
    }

    public override bool? CanBeHitByItem(Player player, Item item)
    {
        if (IsAClone)
            return false;
        return base.CanBeHitByItem(player, item);
    }

    #region Precision Mode

    private void AI_Beyblade()
    {
        void SpinFaster()
        {
            foreach(var proj in Main.ActiveProjectiles)
            {
                if (proj.type != ModContent.ProjectileType<FenixSaw>())
                    continue;
                if (proj.ai[1] != NPC.whoAmI)
                    continue;
                proj.ai[2] = 110;
            }
        }

        void KillSaws()
        {
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type != ModContent.ProjectileType<FenixSaw>())
                    continue;
                if (proj.ai[1] != NPC.whoAmI)
                    continue;
                FenixSaw saw = proj.ModProjectile as FenixSaw;
                saw.shouldDie = true;
            }
        }

        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        NPC.velocity *= 0f;
                        NPC.TargetClosest();
                        Vector2 teleportPoint = MyTarget.Center + new Vector2(550, -100);
                        if (IsAClone)
                        {
                            teleportPoint = NPC.Center;
                            teleportPoint.Y += 32;
                        }

                        TeleportEffect(teleportPoint);
                        Teleport(teleportPoint);
                        if (CanMakeClones())
                        {
                            Vector2 vechHere = (teleportPoint - MyTarget.Center);
                            vechHere.X *= -1;
                            Vector2 clonePos = MyTarget.Center + vechHere;
                            MakeClone(clonePos, State);
                        }
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, teleportPoint, Vector2.Zero, ModContent.ProjectileType<CoolTeleport>(), 1, 1, Main.myPlayer, ai1: 1);
                        }

                    }

                    float time = BeybladePrepTime;
                    _spinningCRot = MathHelper.Lerp(0f, 1f, Timer / time);
                    AnimateC();
                    AnimateTorpedo();

                    _outliner.warning = true;
                    _renderDashTrail = true;
                    Rig.useSword = true;
                    NPC.velocity *= 0.8f;
    
                    RegularRotation -= MathHelper.Lerp(0f, 0.1f, Timer / time);
                    ZRotation = Utils.AngleLerp(ZRotation, MathHelper.ToRadians(90), 0.1f);
                    if (Timer >= time)
                    {
                        if (MultiplayerHelper.IsHost)
                        {

                            Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero,
                                ModContent.ProjectileType<FenixSaw>(), FenixSawDamage, 1, Main.myPlayer, ai1: NPC.whoAmI);
                        }
                        Timer = 0;
                        AttackCycle++;
                    } 
                }
                break;
            case 1:
                {
                    _spinningCRot = 1f;
                    _miniAttackCount = 0;
                    if (Timer == 1)
                    {
                        _dashLineVelocity = Vector2.Zero;
                        NPC.TargetClosest();
                        SoundStyle dashSound = AssetRegistry.Sounds.AlcaricFox.FenixChargin;
                        SoundEngine.PlaySound(dashSound, MyTarget.Center);
                    }

                    _outliner.warning = true;
                    Rig.useSword = true;

                    _startDashPoint = NPC.Center;
             
                    float ratio = Timer / SpinningChargePrepTime;
                    _roaringCircleScale = MathHelper.SmoothStep(5f, 0f, ratio);
                    _roaringCircleAlpha = MathHelper.SmoothStep(0f, 1f, EasingFunction.QuadraticBump(ratio));
                    _roaringCircleColor = Color.Lerp(Color.Pink, Color.Blue, ratio);
               
                    RegularRotation -= MathHelper.Lerp(0f, 0.25f, EasingFunction.InOutExpo(ratio));
                    ZRotation = Utils.AngleLerp(ZRotation, MathHelper.ToRadians(90), 0.1f); 

                    Vector2 lerp1 = Vector2.Lerp(Vector2.Zero, -_dashLineVelocity * 32, EasingFunction.InOutExpo(ratio));
                    Vector2 lerp2 = Vector2.Lerp(Vector2.Zero, _dashLineVelocity * 8, EasingFunction.InExpo(ratio));
                    Vector2 lerp3 = Vector2.Lerp(lerp1, lerp2, ratio);
                    Vector2 lerp4 = Vector2.Lerp(Vector2.Zero, _dashLineVelocity * 8, EasingFunction.InExpo(ratio));
                    Vector2 lerp5 = Vector2.Lerp(lerp3, lerp4, ratio);

                    CameraTargetSystem.AddTarget(Vector2.Lerp(MyTarget.Center, NPC.Center, 0.3f));
                    Vector2 targetVelocity = lerp5;
                    if (Timer < 30)
                    {
                        NPC.velocity *= 0.95f;
                    }
                    else
                    {
                        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, MathHelper.Lerp(0f, 0.1f, EasingFunction.InExpo(ratio)));

                    }

                    AnimateTorpedo();
                
                    if (Timer % 60 == 0)
                    {
                        PixelPrimitiveCircleFactory.CreateGenericInBoom(HeadPosition, Color.Transparent, Color.White, 35, 500);
                    }

                    ChargeParticles(HeadPosition, in Timer);
                    if (Timer >= SpinningChargePrepTime)
                    {
          
                        SoundStyle airdashSound = AssetRegistry.Sounds.AlcaricFox.FenixWindStartup;
                        SoundEngine.PlaySound(airdashSound, HeadPosition);
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 2:
                {
                    _spinningCRot = 1f;
                    if (Timer == 1)
                    {
                        float nextCount = _miniAttackCount + 1;
                        if (nextCount >= AirbounceChainCount)
                        {
                            KillSaws();
                        }
                        _startDashPoint = NPC.Center;
                        Vector2 pointToJumpTo = MyTarget.Center;
                        pointToJumpTo.Y += 200;

                        float directionToJumpFrom = (_startDashPoint.X < MyTarget.Center.X) ? 1 : -1;
                        pointToJumpTo.X += directionToJumpFrom * 600;
                        _dashLineVelocity = pointToJumpTo;
                        SpinFaster();
                        _slowDown = false;
                       
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, HeadPosition, Vector2.Zero, ModContent.ProjectileType<SpiralDashTrail>(), 1, 1, Main.myPlayer, ai1: NPC.whoAmI);
                        }
                    }

                    CameraTargetSystem.AddTarget(Vector2.Lerp(MyTarget.Center, NPC.Center, 0.12f));
                    float time = 62 * SlowdownMult;
                    bool secondVersion = InPhase2 || IsAClone;
                    if(secondVersion)
                        time *= 1.2f;
                    float ease = Timer / time;

                    float direction = _dashLineVelocity.X < _startDashPoint.X ? 1 : -1;
                    Vector2 up = (_dashLineVelocity - _startDashPoint).SafeNormalize(Vector2.Zero);
                    up = up.RotatedBy(MathHelper.PiOver2 * direction);

                    Vector2 midPoint = MyTarget.Center;
                    float arc = 600;
                    if (secondVersion)
                    {
                        arc *= 1.1f;
                    }
                    midPoint += up * arc;
                    Vector2 startPoint = _startDashPoint;
                    Vector2 endPoint = _dashLineVelocity;


                    Vector2 m1 = Vector2.Lerp(_startDashPoint, midPoint, ease);
                    Vector2 m2 = Vector2.Lerp(midPoint, _dashLineVelocity, ease);
                    Vector2 m3 = Vector2.Lerp(m1, m2, ease);
                    Vector2 vel = (m3 - NPC.Center);
                    NPC.velocity = vel;

                    /*
                    float dir = _startDashPoint.X < _dashLineVelocity.X ? 1 : -1;
                    float distance = MathF.Max(1300, Vector2.Distance(_startDashPoint, _dashLineVelocity) + 500);
                    Vector2 offset = Vector2.UnitX * dir * distance;
                    Vector2 endPoint = _startDashPoint + offset;
                    endPoint += Vector2.UnitY * 100;
                    Vector2 m1 = Vector2.Lerp(_startDashPoint, _dashLineVelocity, ease);
                    Vector2 m2 = Vector2.Lerp(_dashLineVelocity, endPoint, ease);
                    Vector2 m3 = Vector2.Lerp(m1, m2, ease);

                    Vector2 arc = Vector2.Lerp(Vector2.Zero, -Vector2.UnitY * _spinningPredictionSpeed, EasingFunction.QuadraticBump(ease));
                    m3 += arc;
                    Vector2 vel = (m3 - NPC.Center);
                    NPC.velocity = vel;
                    */
                       
                    ZRotation = Utils.AngleLerp(ZRotation, MathHelper.ToRadians(90), 0.1f);
                    RegularRotation -= (0.25f + MathHelper.Lerp(0.25f, 0f, EasingFunction.InOutExpo(ease)));
                    Rig.useSword = true;

                    AnimateC();
                    AnimateTorpedo();
                    WalkParticles();
                    RoyalFox.SpawnCometStarParticle(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero), 65);

                    _spiralDashTrailAlpha = EasingFunction.QuadraticBump(Timer / 40f);
                    _contactDamage = true;
                    _outliner.attacking = true;
                    _renderDashTrail = true;
                    if (Timer % 8 == 0)
                    {
                        var donut = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero));
                    }

    

                    if(Timer == 30)
                    {
         
                        SoundStyle soundS = AssetRegistry.Sounds.AlcaricFox.FenixCloseBounce;
                        SoundEngine.PlaySound(soundS, MyTarget.Center);
                    }

                    if (Timer >= time)
                    {
              
                        Timer = 0;
                        _miniAttackCount++;

                        AttackCycle++;
                        if(_miniAttackCount >= AirbounceChainCount)
                        {
                    
                            AttackCycle++;
                        }
                    }

                }
                break;
            case 3:
                {
                    _spinningCRot = 1f;
                    NPC.velocity *= 0.8f;
                    ZRotation += MathHelper.Lerp(0.15f, 0f, EasingFunction.InOutExpo(Timer / 25f));
                    RegularRotation -= 0.1f;

                    AnimateTorpedo();

                    AnimateC();
                    _outliner.warning = true;
                    if (Timer >= 10)
                    {

                        Timer = 0;
                     
                        if (_miniAttackCount >= AirbounceChainCount)
                        {
                            AttackCycle++;
                        }
                        else
                        {
                            NPC.velocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 15;
                            PlayAirbounceSuond2(MyTarget.Center);
                            if (MultiplayerHelper.IsHost)
                            {
                                Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero, ModContent.ProjectileType<CoolTeleport>(), 1, 1, Main.myPlayer, ai1: 1);
                            }

                            FXUtil.CreateRipple(HeadPosition);
                            FXUtil.GlowCircleBoom(HeadPosition, Color.White, Color.Blue, Color.DarkBlue, duration: 40, baseSize: 0.23f);
                            for (float f = 0; f < 3; f++)
                            {
                                Vector2 vel = NPC.velocity.SafeNormalize(Vector2.Zero);
                                vel *= MathHelper.Lerp(3f, 9f, f / 3f);
                                var donut = LegacyParticle.NewParticle<GlowDonutParticle>(HeadPosition + vel * 38, vel.SafeNormalize(Vector2.Zero));
                                donut.Scale *= 3 * MathHelper.Lerp(1f, 1.5f, f / 3f);

                            }

                            for (float f = 0; f < 16; f++)
                            {
                                Vector2 vel = NPC.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5, 45);
                                vel = vel.RotatedByRandom(MathHelper.ToRadians(60));
                                var d = DustParticle.Spawn(HeadPosition, vel);
                                d.outerColor = Color.Blue;
                                d.dampening = 0.1f;
                                d.noTileCollide = true;
                                d.gravity = 0;
                                d.Scale *= 1.2f;
                            }

                            ShakeScreenPosition.Shake = 3;
                            AttackCycle--;
                        }
                        //bounce
                    }
                }
                break;
            case 4:
                {

                    _spinningCRot = 1f;
                    NPC.velocity.Y += 0.4f;
                    ZRotation += MathHelper.Lerp(0.15f, 0f, EasingFunction.InOutExpo(Timer / 25f));
                    RegularRotation -= 0.1f;
                    if (Timer >= 30)
                    {
                        SwitchState(AIState.Precision_Tired);
                    }
                }
                break;
        }
    }
    private void AI_SwordChase()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        Vector2 teleportOffset = -Vector2.UnitY * 384;
                        float radiansOffset = AttackCounter / 4 * MathHelper.TwoPi;
                        if (IsAClone)
                            radiansOffset += MathHelper.Pi;
                        teleportOffset = teleportOffset.RotatedBy(radiansOffset);
                        _dashLineVelocity = teleportOffset;

                        Vector2 teleportPosition = MyTarget.Center + teleportOffset;

                        if (CanMakeClones() && AttackCounter == 0)
                        {
                            Vector2 vecHere = (teleportPosition - MyTarget.Center);
                            vecHere.Y *= -1;
                            Vector2 clonePos = MyTarget.Center + vecHere;
                            MakeClone(clonePos, State);
                        }
                        Teleport(teleportPosition);
                    }

                    if(Timer == 3)
                    {

                        PixelPrimitiveCircleFactory.CreateGenericInBoom(NPC.Center, Color.White, Color.Transparent, 60, 384);
                    }

                    Rig.useSword = true;

                    //OKAY
                    //So we have the point to make hte slash, it's just like any other slash
                    if(Timer < SwordSlashSlashTime * 0.75f)
                        _startDashPoint = MyTarget.Center + _dashLineVelocity;
                    if (Timer >= SwordSlashSlashTime * 0.79f)
                        _goInvisible = true;

                    int halfTime = (int)(SwordSlashSlashTime * 0.5f);
 
                    Vector2 shootVelocity = -_dashLineVelocity;
               
                    float swingRange = MathHelper.ToRadians(245);
                    float interpolant = Timer / SwordSlashSlashTime;
                    float ease = MathHelper.Lerp(0f, 0.3f, EasingFunction.QuickOutSlowIn(interpolant));
                    float ease2 = MathHelper.Lerp(0f, 1f, EasingFunction.InOutCirc(interpolant));
                    float ease3 = MathHelper.Lerp(ease, ease2, EasingFunction.InOutCirc(interpolant));
                    Vector2 swingOffset = CalculateSwingOffset(shootVelocity, interpolant);//OvalSwing.CalculateXY(ease3, shootVelocity, swingRange, new Vector2(128, 192), 1);
                    Vector2 swingPosition = _startDashPoint + swingOffset;

                    Vector2 point = swingPosition;
                    point += _swingVelocity.SafeNormalize(Vector2.Zero) * 196;
                  
                    if (Timer == halfTime)
                    {
                        ShakeScreenPosition.Shake = 4;
                        FXUtil.ShakeCamera(NPC.Center, 1024, 4);
                        if (MultiplayerHelper.IsHost)
                        {
                      
                            Projectile.NewProjectile(SourceFromThis, _startDashPoint + shootVelocity, Vector2.Zero, ModContent.ProjectileType<MagicSwordSlash>(), SwordSlashDamage, 1, Main.myPlayer);
                        }
                        PlaySwordSwingSword(MyTarget.Center);
                    }

                    if(Timer >= SwordSlashSlashTime * 0.5f)
                    {
                        _swingTrailEndRatio = MathHelper.Lerp(0f, 1f, ease3);
                        _swingTrailAlpha = EasingFunction.QuadraticBump(ease3 * EasingFunction.InSine(interpolant));
                    }

                    if (Timer < 30 && Timer > 6)
                    {
                        float t = Timer; 
                        float pr = t / 30f;
                        _eyeFlashAlpha = EasingFunction.QuadraticBump(pr);
                        _eyeFlashPosition = Rig.headPart.worldPosition;
                        _eyeFlashOffset = Vector2.Zero;

                    }
                    if (Timer == SwordSlashSlashTime - 10)
                    {
                        float endPoint = _swingTrailEndRatio;
                        Vector2 p = _startDashPoint + CalculateSwingOffset(_swingVelocity, endPoint);
                        p += _swingVelocity.SafeNormalize(Vector2.Zero) * 200;
                        var fx = FXUtil.GlowCircleBoom(p, Color.White, Color.Blue, Color.DarkBlue, 35, 0.24f);
                        fx.Scale *= 1.9f;
                        for(float f = 0; f < 18; f++)
                        {
                            Vector2 ve = (p - _startDashPoint).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5f, 45f);
                            ve = ve.RotatedByRandom(MathHelper.ToRadians(65));
                            ve = ve.RotatedBy(MathHelper.ToRadians(-90));
                            var d = DustParticle.Spawn(p, ve);
                            d.outerColor = Color.Lerp(Color.Blue, Color.Pink, Main.rand.NextFloat(0.00f, 1.00f));
                            d.noTileCollide = true;
                            d.dampening = 0.1f;
                            d.gravity = 0;
                            d.Scale *= 2;
                        }
                    }
                  
                    _swingVelocity = shootVelocity;
                    for(int i = 0; i < Rig.bodyParts.Length; i++)
                    {
                        var part = Rig.bodyParts[i];
                        float radians = MathHelper.ToRadians(MathHelper.Lerp(45, -45, ease3));
                        part.eulerAngles.W = radians;// MathHelper.Lerp(radians, 0, (float)i / (float)Rig.bodyParts.Length);
                    }

                    if(Timer < SwordSlashSlashTime * 0.05f)
                    {
                        _goInvisible = true;
                    }
                    if(Timer >= SwordSlashSlashTime * 0.4f)
                    {
                        Vector2 so = CalculateSwingOffset(shootVelocity, Main.rand.NextFloat(0f, 1f));//OvalSwing.CalculateXY(ease3, shootVelocity, swingRange, new Vector2(128, 192), 1);
                        Vector2 sp = _startDashPoint + so;
                        sp += _swingVelocity.SafeNormalize(Vector2.Zero) * 196;
                        SpawnCometStarParticle(sp, Vector2.Zero, 70);
                        SpawnSmokeParticle(sp + Main.rand.NextVector2Circular(32, 32), Vector2.Zero, 100);
             
                        if (Main.rand.NextBool(4))
                        {
                            var dp = DustParticle.Spawn(sp, Vector2.Zero);
                            dp.outerColor = Color.DarkGray;
                            dp.gravity = 0;
                            dp.noTileCollide = true;
                            dp.dampening = 0.07f;
                        }
                        if (Main.rand.NextBool(4))
                        {
                            var dp = RoyalMagicStarParticle.Spawn(sp, Vector2.Zero);
                            dp.color = Color.LightBlue;
                            dp.Scale *= 0.3f;
                        }
                    }
                  
                    if(Timer <= SwordSlashSlashTime * 0.1f)
                    {
                        ZRotation =0;
                    }
                    else if(Timer >= SwordSlashSlashTime * 0.5f)
                    {
                 
                       // WalkParticles();
                        ZRotation += 0.12f;
                    }
                    else
                    {
                  //      _renderDashTrail = true;
                        ZRotation = Utils.AngleLerp(ZRotation, MathHelper.ToRadians(90), 0.1f);
                    }
           
                    AnimateTorpedo();

                    Vector2 moveVelocity = swingPosition - NPC.Center;
                    NPC.velocity = moveVelocity;

                    float targetRotation = (swingPosition - _startDashPoint).ToRotation();
                    RegularRotation = targetRotation;
              
                    _outliner.attacking = true;
                    if(Timer >= SwordSlashSlashTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    _goInvisible = true;
                    NPC.velocity *= 0.98f;
                    if(Timer >= SwordSlashBetweenTime)
                    {
                        Timer = 0;
                        AttackCounter++;
                        if(AttackCounter >= SwordSlashCount)
                        {
                            AttackCycle++;
                        }
                        else
                        {
                            AttackCycle = 0;
                        }
                    }
                }
                break;
            case 2:
                {
                    SwitchState(AIState.Precision_Beyblade);
                }
                break;
        }
    }
    private void AI_CometTeleportShots()
    {
        void MoveTeleportIndicator(float time)
        {

            float ratio = Timer / time;
            float dist = Vector2.Distance(_teleportTelegraphPosition, MyTarget.Center);
            float strength = dist / 384f;
            strength = EasingFunction.InOutSine(strength);
            float maxAllowToMove = MathHelper.Lerp(16, 1, strength);
            _teleportAlpha = EasingFunction.QuadraticBump(ratio);
            Vector2 targetPos = MyTarget.Center;
            Vector2 offset = -Vector2.UnitY * 128;
            offset = offset.RotatedBy(ratio * MathHelper.TwoPi + (MathHelper.TwoPi * 3 * (AttackCounter / CometTeleportCount)));
            targetPos += offset;
            _teleportTelegraphPosition = Vector2.Lerp(_teleportTelegraphPosition, targetPos, MathHelper.Lerp(0.18f, 0.0f, EasingFunction.InOutExpo(ratio)));
          //  _teleportTelegraphPosition = _teleportTelegraphPosition.MoveTowards(MyTarget.Center, maxAllowToMove);
        }
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        _teleportTelegraphPosition = MyTarget.Center;
                     
                    }
                    MoveTeleportIndicator(CometTeleportPrepTime);
                    FaceTargetWhileFlying();
                    AnimateFlying();
                    _outliner.warning = true;
                    if(Timer >= CometTeleportPrepTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if(Timer == 1)
                    {
                        _slowDown = false;
                       // TeleportEffect(_teleportTelegraphPosition);
                        Teleport(_teleportTelegraphPosition);
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, _teleportTelegraphPosition, Vector2.Zero, 
                                ModContent.ProjectileType<CoolTeleport>(), 1, 1, Main.myPlayer);
                            Vector2 midVelocity = (MyTarget.Center - _teleportTelegraphPosition);
                            midVelocity = midVelocity.SafeNormalize(Vector2.Zero);
                            midVelocity *= 15;

                            float spread = MathHelper.ToRadians(MathHelper.Lerp(60, 180, AttackCounter / CometTeleportCount));
                            float num = MathHelper.Lerp(3, 6, AttackCounter / CometTeleportCount);
                            for(int i = 0; i < num; i++)
                            {
                                float ratio = (float)i / num;

                                Vector2 vel = midVelocity.RotatedBy(MathHelper.Lerp(-spread * 0.5f, spread * 0.5f, (float)i / num));
                                vel = vel.RotatedBy(spread * 0.25f);
                                Projectile.NewProjectile(SourceFromThis, _teleportTelegraphPosition, vel,
                                    ModContent.ProjectileType<SpiralComet>(), CometTeleportDamage, 1, Main.myPlayer);
                            }
                        }

                        _startDashPoint = _teleportTelegraphPosition;
                        Vector2 backflipOffset = (_teleportTelegraphPosition - MyTarget.Center).SafeNormalize(Vector2.Zero);
                        backflipOffset *= 1200 * -1;
                        _dashLineVelocity = _startDashPoint + backflipOffset;
                    }

                    if (!_slowDown)
                    {
                        float ratio2 = Timer / CometBackflipTime;
                        Vector2 endPoint = _dashLineVelocity.RotatedBy(MathHelper.ToRadians(245), _startDashPoint);
                        Vector2 v1 = Vector2.Lerp(_startDashPoint, _dashLineVelocity, ratio2);
                        Vector2 v2 = Vector2.Lerp(_dashLineVelocity, endPoint, ratio2);
                        Vector2 v3 = Vector2.Lerp(v1, v2, EasingFunction.InExpo(ratio2));
                        Vector2 v4 = (v3 - NPC.Center);
                        NPC.velocity = v4;

                        if(AttackCounter == CometTeleportCount - 1)
                        {
                            float dp = Vector2.Dot(NPC.velocity.SafeNormalize(Vector2.Zero), (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero));
                            if (dp < 0)
                            {
                                _slowDown = true;
                            }
                        }
                      
                    }

                    if (_slowDown)
                    {
                        NPC.velocity *= 0.97f;
                    }
            

                    _renderDashTrail = true;
                    AnimateTorpedo();
                    if (Timer < CometBackflipTime * 0.5f)
                        WalkParticles();
                    else
                        _goInvisible = true;
                    MoveTeleportIndicator(CometBackflipTime);
                    RegularRotation = NPC.velocity.ToRotation();
                    ZRotation += 0.12f;
                    _outliner.attacking = true;
                    if(Timer >= CometBackflipTime)
                    {
                        Timer = 0;
                        AttackCounter++;
                        if(AttackCounter >= CometTeleportCount)
                        {
                            Timer = 0;
                            AttackCycle++;
                        }
                    }
                }
                break;
            case 2:
                {
                    NPC.velocity *= 0.96f;
                    RegularRotation = NPC.velocity.ToRotation();
                    ZRotation += 0.12f;
                    if (Timer >= CometTeleportEndTime)
                    {
                        SwitchState(AIState.Precision_SpinningCharge);
                    }
                }
                break;
        }
    }

    private void AI_SpinningCharge()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _miniAttackCount = 0;
                    if (Timer == 1)
                    {
                        _dashLineVelocity = Vector2.Zero;
                        NPC.TargetClosest();
                        SoundStyle dashSound = AssetRegistry.Sounds.AlcaricFox.FenixChargin;
                        SoundEngine.PlaySound(dashSound, MyTarget.Center);
                        if (CanMakeClones() && AttackCounter == 0 && _miniAttackCount == 0)
                        {
                            Vector2 vecHere = (NPC.Center - MyTarget.Center);
                            vecHere *= -1;
                            Vector2 clonePos = MyTarget.Center + vecHere;
                            MakeClone(clonePos, State);
                        }
                    }

                    Vector2 directionToTarget = (MyTarget.Center - NPC.Center);
                    directionToTarget = directionToTarget.SafeNormalize(Vector2.Zero);
                    _dashLineVelocity = _dashLineVelocity.MoveTowards(directionToTarget, 0.5f);
                    _startDashPoint = NPC.Center;

                    RegularRotation = Utils.AngleLerp(RegularRotation, _dashLineVelocity.ToRotation(), 0.1f);

                    float ratio = Timer / SpinningChargePrepTime;
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
                    if (Timer % 60 == 0)
                    {
                        PixelPrimitiveCircleFactory.CreateGenericInBoom(HeadPosition, Color.Transparent, Color.White, 35, 500);
                    }

                    ChargeParticles(HeadPosition, in Timer);
                    if (Timer >= SpinningChargePrepTime)
                    {
                        SoundStyle airdashSound = AssetRegistry.Sounds.AlcaricFox.FenixWindStartup;
                        SoundEngine.PlaySound(airdashSound, HeadPosition);
                        Timer = 0;
                        AttackCycle++;
                    }
                    //For the spinning charge, the startup is pretty similar to the big dash but it's a bit faster
                    //ALright, first step is to wind up like the other dash attack, then she's gonna jump and spiral towards you
                }
                break;

            case 1:
                {
                    if(Timer == 1)
                    {
                        //So basically
                        //Take the current point, then get a point behind the player
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, HeadPosition, Vector2.Zero, ModContent.ProjectileType<SpiralDashTrail>(), 1, 1, Main.myPlayer, ai1: NPC.whoAmI);
                        }
                    }

                    Vector2 directionToTarget = (MyTarget.Center - NPC.Center);
                    directionToTarget = directionToTarget.SafeNormalize(Vector2.Zero);
                    Vector2 targetVelocity = directionToTarget * SpinningChargeSpeed;
                    ZRotation += MathHelper.Lerp(0.45f, 0.12f, EasingFunction.InOutExpo(Timer /40f));
                    RegularRotation = NPC.velocity.ToRotation();
                    AnimateTorpedo();
                    WalkParticles();
                    RoyalFox.SpawnCometStarParticle(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero), 65);
                    _spiralDashTrailAlpha = EasingFunction.QuadraticBump(Timer / 40f);
                    _contactDamage = true;
                    _outliner.attacking = true;
                    _renderDashTrail = true;
                    if(Timer % 8 == 0)
                    {
                        var donut = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero));
                    }

                    float dp = Vector2.Dot(directionToTarget, NPC.velocity.SafeNormalize(Vector2.Zero));
                    if(dp > 0)
                    {
                        NPC.velocity = NPC.velocity.MoveTowards(targetVelocity, 0.6f);
                        NPC.velocity *= MathHelper.Lerp(1.02f, 1.01f, _miniAttackCount / SpinningChargeBurstCount);
                    }
                    
                    if(Timer >= 40)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                 
                }
                break;
            case 2:
                {
                    NPC.velocity *= 0.975f;
                    ZRotation += MathHelper.Lerp(0.15f, 0f, EasingFunction.InOutExpo(Timer / 25f));
                    RegularRotation = NPC.velocity.ToRotation();
                    AnimateTorpedo();
                    _outliner.warning = true;
                    if (Timer >= 10)
                    {
                 
                        Timer = 0;
                        _miniAttackCount++;
                        if(_miniAttackCount >= AirbounceChainCount)
                        {
                            AttackCounter++;
                            if(AttackCounter >= SpinningChargeBurstCount)
                            {
                                AttackCycle++;
                            }
                            else
                            {
                                AttackCycle = 0;
                            }
                        }
                        else
                        {
                            NPC.velocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 15;
                            PlayAirbounceSuond(MyTarget.Center);
                            FXUtil.CreateRipple(HeadPosition);
                            FXUtil.GlowCircleBoom(HeadPosition, Color.White, Color.Blue, Color.DarkBlue, duration: 40, baseSize: 0.23f);
                            for(float f = 0; f < 3; f++)
                            {
                                Vector2 vel = NPC.velocity.SafeNormalize(Vector2.Zero);
                                vel *= MathHelper.Lerp(3f, 9f, f / 3f);
                                var donut = LegacyParticle.NewParticle<GlowDonutParticle>(HeadPosition + vel * 38, vel.SafeNormalize(Vector2.Zero));
                                donut.Scale *= 3 * MathHelper.Lerp(1f, 1.5f, f / 3f);

                            }

                            for(float f = 0; f < 16; f++)
                            {
                                Vector2 vel = NPC.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(5, 45);
                                vel = vel.RotatedByRandom(MathHelper.ToRadians(60));
                                var d = DustParticle.Spawn(HeadPosition, vel);
                                d.outerColor = Color.Blue;
                                d.dampening = 0.1f;
                                d.noTileCollide = true;
                                d.gravity = 0;
                                d.Scale *= 1.2f;
                            }

                            ShakeScreenPosition.Shake = 3;
                            AttackCycle--;
                        }
                        //bounce
                    }
                }
                break;
            case 3:
                {
                    _goInvisible = true;
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();

                    }
                    if(NPC.velocity.Length() < 30)
                        NPC.velocity *= 1.1f;
                    ZRotation += MathHelper.Lerp(0.15f, 0f, EasingFunction.InOutExpo(Timer / 25f));
                    RegularRotation = NPC.velocity.ToRotation();
                    AnimateTorpedo();
                    if (Timer >= 60)
                    {
                        SwitchState(AIState.Precision_SwordSlashChase);
                    }
                }
                break;
        }
    }

    private void AI_ZoomTired()
    {
        AI_Tired();
        if(AttackCycle == 3)
        {
            SwitchState(AIState.Precision_CometTeleportShots);
        }
    }

    private void AI_PrecisionTired()
    {
        AI_Tired2();
        if(AttackCycle == 2)
        {
            SwitchState(AIState.Zoom_CometStarDash);
        }
    }
    private void AI_Tired()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        Teleport(MyTarget.Center + new Vector2(0, -1500));
                    }

                    NPC.velocity *= Vector2.Zero;

                    FaceTargetWhileFlying();
                    AnimateFlying();
                    if (Timer >= 90)
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
                        SoundStyle tired = AssetRegistry.Sounds.AlcaricFox.FenixAppeartired;
                        SoundEngine.PlaySound(tired, MyTarget.position);
                        NPC.TargetClosest();
                        float dir = Main.rand.NextBool(2) ? 1 : -1;
                        Vector2 pointToTeleportTo = MyTarget.Center + new Vector2(300 * dir, -64);
                        TeleportEffect(pointToTeleportTo);
                        Teleport(pointToTeleportTo);
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, pointToTeleportTo, Vector2.Zero, ModContent.ProjectileType<CoolTeleport>(), 1, 1, Main.myPlayer, ai1: 2);
                        }
                    }

                    FaceTargetWhileFlying();
                    AnimateFlying();
                    if (Main.rand.NextBool(8))
                    {
                        Vector2 velocity = Rig.headPart.FinalAngle.ToRotationVector2();
                        Vector2 pos = HeadPosition;
                        pos += velocity * 40;

                        Vector2 down = velocity.RotatedBy(MathHelper.PiOver2 * FacingDirectionToTarget);
                        pos += down * 18;
                        var sp = SmokeParticle.SpawnInAlphaLayer(pos, velocity * Main.rand.NextFloat(1f, 2f));
                        sp.initialColor = Color.White;
                        sp.fadeToColor = Color.DarkGray;
                        sp.behindLayer = true;
                        sp.Scale *= 0.6f;
                    }

                    if (Main.rand.NextBool(4))
                    {
                        var dp = SparkleParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(128, 128), Main.rand.NextVector2Circular(4, 4));
                        dp.noTileCollide = true;
                        dp.gravity = 0;
                        dp.dampening = 0.1f;
                        dp.outerColor = Color.Pink;
                        dp.Scale *= 0.6f;
                    }

                    _tailAnimation = TailAnimation.Loose;
                    NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, MathF.Sin(Timer * 0.5f) * 1f, 0.1f);
                    _gravityFieldAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(Timer / 60f));
                    if (Timer >= TiredTime)
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
                        NPC.TargetClosest();
                        Vector2 teleportPoint = MyTarget.Center + new Vector2(0, -2000);
                        TeleportEffect(teleportPoint);
                        PixelPrimitiveCircleFactory.CreateGenericInBoom(HeadPosition, Color.Transparent, Color.White, 60, 512);
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, teleportPoint, Vector2.Zero, ModContent.ProjectileType<CoolTeleport>(), 1, 1, Main.myPlayer, ai1: 2);
                        }
                    }
                    NPC.velocity *= 0.98f;
                    AnimateFlying();
                    FaceTargetWhileFlying();
                    if (Timer >= 180)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
        }
    }
    private void AI_Tired2()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        Teleport(MyTarget.Center + new Vector2(0, -1500));
                    }

                    NPC.velocity *= Vector2.Zero;

                    FaceTargetWhileFlying();
                    AnimateFlying();
                    if (Timer >= 90)
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
                        SoundStyle tired = AssetRegistry.Sounds.AlcaricFox.FenixAppeartired;
                        SoundEngine.PlaySound(tired, MyTarget.position);
                        NPC.TargetClosest();
                        float dir = Main.rand.NextBool(2) ? 1 : -1;
                        Vector2 pointToTeleportTo = MyTarget.Center + new Vector2(300 * dir, -64);
                        TeleportEffect(pointToTeleportTo);
                        Teleport(pointToTeleportTo);
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, pointToTeleportTo, Vector2.Zero, ModContent.ProjectileType<CoolTeleport>(), 1, 1, Main.myPlayer, ai1: 2);
                        }
                    }

                    FaceTargetWhileFlying();
                    AnimateFlying();
                    if (Main.rand.NextBool(8))
                    {
                        Vector2 velocity = Rig.headPart.FinalAngle.ToRotationVector2();
                        Vector2 pos = HeadPosition;
                        pos += velocity * 40;

                        Vector2 down = velocity.RotatedBy(MathHelper.PiOver2 * FacingDirectionToTarget);
                        pos += down * 18;
                        var sp = SmokeParticle.SpawnInAlphaLayer(pos, velocity * Main.rand.NextFloat(1f, 2f));
                        sp.initialColor = Color.White;
                        sp.fadeToColor = Color.DarkGray;
                        sp.behindLayer = true;
                        sp.Scale *= 0.6f;
                    }

                    if (Main.rand.NextBool(4))
                    {
                        var dp = SparkleParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(128, 128), Main.rand.NextVector2Circular(4, 4));
                        dp.noTileCollide = true;
                        dp.gravity = 0;
                        dp.dampening = 0.1f;
                        dp.outerColor = Color.Pink;
                        dp.Scale *= 0.6f;
                    }

                    _tailAnimation = TailAnimation.Loose;
                    NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, MathF.Sin(Timer * 0.5f) * 1f, 0.1f);
                    _gravityFieldAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(Timer / 60f));
                    if (Timer >= TiredTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
        }
    }

    #endregion
    private void AI_Despawn()
    {
        Timer++;
        if(Timer >= 90)
        {
            NPC.active = false;
        }
        NPC.velocity.X *= 0.98f;
        NPC.velocity.Y -= 0.05f;
        AnimateTorpedo();
        ZRotation += 0.12f;
        RegularRotation = NPC.velocity.ToRotation();
        _renderDashTrail = true;
        WalkParticles();
    }

    private void AI_Idle()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();

        }
        for (int i = 0; i < Rig.bodyParts.Length; i++)
        {
            var part = Rig.bodyParts[i];
            part.eulerAngles.W = 0;// MathHelper.Lerp(radians, 0, (float)i / (float)Rig.bodyParts.Length);
        }

        NPC.velocity *= 0.8f;
        FaceTargetWhileFlying();
        AnimateFlying();
        AnimateStanding();
        SwitchState(AIState.Zoom_SparkleStarRain);
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

    #region Zoom Mode 

    private void FaceTargetWhileFlying()
    {
        Vector2 facingDirection = Vector2.UnitX ;
        Vector2 up = facingDirection.RotatedBy(-MathHelper.ToRadians(85));
        RegularRotation = up.ToRotation();

        float targetRotation = 0;
        if (FacingDirectionToTarget == -1)
            targetRotation = -MathHelper.Pi;
        ZRotation = Utils.AngleLerp(ZRotation, targetRotation, 0.05f);
    }

    private void GoRainWithoutTeleport()
    {
        Timer = 0;
        AttackCycle = 1;
        State = AIState.Zoom_SparkleStarRain;
        AttackCounter = 0;
        NPC.netUpdate=true;
    }

    private void AI_SparkleStarRain()
    {

        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                        float dir = Main.rand.NextBool(2) ? 1 : -1;
                        Vector2 pointToTeleportTo = MyTarget.Center + new Vector2(300 * dir, -64);

                        TeleportEffect(pointToTeleportTo);
                        Teleport(pointToTeleportTo);

                    }
                    if(Timer == 1)
                    {
                        NPC.velocity.Y = -10;
                    }

                    _tailAnimation = TailAnimation.Loose;
                    FaceTargetWhileFlying();
                    AnimateFlying();
            
                    NPC.velocity *= 0.94f;
                    _outliner.warning = true;
                    if(Timer >= 30)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if(Timer % TimeBetweenSparkleStars == 0)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 centerPos = MyTarget.Center;
                            centerPos.X += MathHelper.Lerp(-2500 * TargetDirection, -1200 * TargetDirection, Main.rand.NextFloat(0f, 1f));
                            centerPos.Y -= 1800;
                            Vector2 velocity = Vector2.UnitY;
                            velocity = velocity.RotatedBy(MathHelper.ToRadians(-45) * TargetDirection);
                            velocity *= 16;
                            Projectile.NewProjectile(SourceFromThis, centerPos, velocity, ModContent.ProjectileType<MagicFallingStar>(), SparkleStarDamage, 1, Main.myPlayer);
                        }
                    }


                    OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -100);
                    //We want to face the entire body slightly up and then we're going to angle each part manually with forward kinematics
                    _tailAnimation = TailAnimation.Loose;
                    FaceTargetWhileFlying();


                    NPC.velocity.X *= 0.98f;
                    NPC.velocity.Y = MathF.Sin(Timer * 0.05f) * 0.3f;
                    AnimateFlying();
                    FloatParticles();
                    _outliner.attacking = true;
                    if(Timer >= SparkleStarRainTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            default:
                SwitchState(AIState.Zoom_CometStarDash);
                break;
        }
    }
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
                        if (_phase2)
                        {
                            dir = 1;
                        }

                        Vector2 pointToTeleportTo = MyTarget.Center + new Vector2(600 * dir, -192);

                        if (IsAClone)
                        {
                            pointToTeleportTo = NPC.Center;
                        }
                        if (CanMakeClones())
                        {
                            Vector2 dirToHere = (pointToTeleportTo - MyTarget.Center);
                            dirToHere.X *= -1;
                            Vector2 clonePoint = MyTarget.Center + dirToHere;
                            MakeClone(clonePoint, State);
                        }

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
                    float d = 1;
                    if (IsAClone)
                    {
                        d *= -1;
                    }
                    offset = offset.RotatedBy(MathHelper.Pi * d * (Timer / BigFatLaserPrepTime));
                    if (IsAClone)
                    {

                    }
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


;
                    float rotAmount = MathHelper.ToRadians(45 ) / BigFatLaserFireTime;
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
                        if (IsAClone)
                        {
                            tteleportPos = MyTarget.Center - new Vector2(-1500, -500);

                        }
                        Teleport(tteleportPos);
                        PoofParticles(tteleportPos);
                        _startDashPoint = tteleportPos;


                        _dashLineVelocity = MyTarget.Center;
                        _dashLineVelocity.Y -= 1000;
                    
                    }

                    WalkParticles();
                    AnimateStretched();
                    _renderDashTrail = true;

                    float cloneDirection = IsAClone ? -1 : 1;
                    _startDashPoint.X += 4 * cloneDirection;
                    _dashLineVelocity.X += 4 * cloneDirection;
             
                    float ratio = Timer / time;
                    float inOut = EasingFunction.OutExpo(ratio);
                    _dashLineVelocity.Y += MathHelper.Lerp(1, 4, EasingFunction.InOutExpo(ratio));

                    float slowInRatio = Timer - time * 0.78f;
                    float slowEase = EasingFunction.InExpo(slowInRatio / (time * 0.22f));
                    float slowIn = MathHelper.Lerp(0f, 1f, slowEase);

                    Vector2 endPoint = _dashLineVelocity + Vector2.UnitX * 384 * cloneDirection;
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
                
                SwitchState(AIState.Zoom_Tired);
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
                        if (CanMakeClones())
                        {
                            Vector2 toPlayer = (NPC.Center - MyTarget.Center);
                            toPlayer.X *= -1;
                            Vector2 clonePosition = MyTarget.Center + toPlayer;
                            MakeClone(clonePosition, State);
                        }
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
                        float dir = IsAClone ? -1 : 1;
                        _startDashPoint = MyTarget.Center + new Vector2(-1000 * dir, 0);
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
                        float dir = IsAClone ? -1 : 1;
                        _startDashPoint = MyTarget.Center - new Vector2(1000 * dir, 0);
                        _dashLineVelocity = (MyTarget.Center - _startDashPoint);
                        PoofParticles();
                        Teleport(MyTarget.Center - new Vector2(1000 * dir, 0));
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

    private void PreUpdateRig()
    {
        Rig.ResetOverrides();
    }
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
