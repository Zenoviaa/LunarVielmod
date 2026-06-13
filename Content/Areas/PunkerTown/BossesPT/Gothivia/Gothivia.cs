using Microsoft.Xna.Framework.Input;
using Stellamod.Common.WeaponUpgrade.UI;
using Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia;

public partial class Gothivia : ScarletBoss
{
    private enum WingsPerspective : byte
    {
        ThreeQ,
        FourQ
    }
    private enum AIState
    {
        Spawn,
        Death,
        Despawn,

        Idle,

        //This is where she summons the discs
        Dichotamy,

        //This is where she does the blowtorches
        Archery,

        //Bounce Kick
        Kick,

        //This is the one 
        BoostBounce,

        Suns,

        //The infinity sign
        SunCharge,

        //Fire Tornado
        FireTornado,

        TheZoomer,

        ComboAttack,

        SniperShot
    }

    private PatternManager<AIState>? _patternManageBackingField;
    private PatternManager<AIState> AttackPattern
    {
        get
        {
            if (_patternManageBackingField == null)
            {
                _patternManageBackingField = new PatternManager<AIState>();
                _patternManageBackingField.AddPattern(AIState.Kick, 1f);
                _patternManageBackingField.AddPattern(AIState.BoostBounce, 1f);
            }
            return _patternManageBackingField;
        }
    }

    private List<float>? _shootRotations;
    private List<float> ShootRotations
    {
        get
        {
            _shootRotations ??= new List<float>(capacity: 8);
            return _shootRotations;
        }
    }

    private bool _keyDown;
    private WingsPerspective _wingsPerspective;
    private bool _contactDamage;
    
    private float _telegraphLineOffTimer;
    private float _telegraphLineAlpha;
    private float _bowDissipateAlpha;
    private float _afterImageAlpha;
    
    private bool _renderAfterImage;
    private bool _renderFigure8Trail;
    private bool _renderFinger;
    
    private float _fingerAlpha;
    private float _figure8TrailAlpha;
    private float _numDirections;

    private int _bowFrame;
    private float _dashDirection;

    private Vector2 _startCDashOffset;
    private Vector2 _endCDashOffset;
    private Vector2 _initialVelocity;
    private Vector2 _aimingVelocity;
    private Vector2 _figureEightStartCenter;

    private Outliner _outliner;
    private AnimationFramer _wingAnimationFrame;
    private AnimationFramer _bowAnimationFrame;
    private ref float Timer => ref NPC.ai[0];

    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];

    private bool InPhase2 => NPC.life < NPC.lifeMax * 0.5f;

    private float SniperShot_PrepTime => 100;
    private float SniperShot_TelegraphTime => 360;
    private float SniperShot_ShootTime => 65;

    private float ComboAttack_PrepTime => 100f;
    private float ComboAttack_BlowtorchTelegraphTime => 35;
    private float ComboAttack_ShootTime => 25f;
    private float ComboAttack_BlastUpTime => 55f;
    private float ComboAttack_RotateTime => 55f;
    private float ComboAttack_ZoomTime => 30f;
    private float ComboAttack_SecondZoomTime => 45;
    private float ComboAttack_EndingTime => 30f;



    private float FireTornado_CircleSpeedUpTime => 180f;
    private float FireTornado_TimeBetweenCircleWaves => 80;
    private float FireTornado_EndingTime => 60;

    private float FireTornado_CircleCount => 8;
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.VortexPillar,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Empress of the Green sun and nature. Everything empowering and living falls under her reign.")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Gothivia, One of the Green Sun", "2"))
            });
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 1;

        NPCID.Sets.TrailCacheLength[Type] = 128;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MustAlwaysDraw[Type] = true;
    }

    public override void SetDefaults()
    {
        NPC.width = 60;
        NPC.height = 60;
        NPC.damage = 1;
        NPC.defense = 150;
        NPC.lifeMax = 300000;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.value = Item.buyPrice(gold: 99);
        NPC.boss = true;
        NPC.npcSlots = 10f;
        NPC.scale = 1f;

        NPC.aiStyle = -1;
        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Gothivia");
        }
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            AttackCycle = 0;
            AttackCounter = 0;
            NPC.netUpdate = true;
        }
    }
    private float Ground => 16000;
    private void EnablePlatformArena()
    {
        DomainExpansionManager fallSystem = ModContent.GetInstance<DomainExpansionManager>();
        fallSystem.noWings = true;
        fallSystem.inSpace = true;
        fallSystem.hoveringPlatform = true;
        fallSystem.hoverPlatformY = Ground;
        if (Main.netMode == NetmodeID.Server)
            return;

        FlameWinds s = ScreenShader.GetInstance<FlameWinds>();
        s.alpha = 1;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
        target.GetModPlayer<GothiviaPlayer>().AddSunStack();
    }

    private void CreateFlameNSmokeParticles()
    {
        if (Main.netMode == NetmodeID.Server)
            return;
        Main.windSpeedTarget = 0.5f;
        if (Main.rand.NextBool(8))
        {
            Vector2 pos = new Vector2();
            pos.X = Main.rand.Next(0, Main.screenWidth * 2);
            pos.Y = Main.rand.Next(Main.screenHeight, Main.screenHeight + 300);
            pos += Main.screenPosition - Main.screenWidth * Vector2.UnitX;
            var ufp = UnderworldFlameParticle.Spawn(pos, -Vector2.UnitY * 10 + Vector2.UnitX * 5, Scale: Main.rand.NextFloat(0.1f, 0.3f));
            ufp.ySlow = false;
        }
        if (Main.rand.NextBool(3))
        {
            Vector2 pos = new Vector2();
            pos.X = Main.rand.Next(0, Main.screenWidth * 2);
            pos.Y = Main.rand.Next(0, Main.screenHeight);
            pos += Main.screenPosition - Main.screenWidth * Vector2.UnitX;
            UnderworldSmokeParticle.Spawn(pos, -Vector2.UnitY * 2 + -Vector2.UnitX, Scale: Main.rand.NextFloat(0.5f, 0.8f));
        }
    }

    public override BossLevel GetBossLevel()
    {
        return BossLevel.Superboss;
    }

    public override void AI()
    {
        base.AI();
        EnablePlatformArena();
        CreateFlameNSmokeParticles();
        _outliner.SetDefaults();

        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
            {
                if (State != AIState.Despawn)
                {
                    SwitchState(AIState.Despawn);
                }
            }
        }
        //Animate the wings
        //The perspective only decides which wing texture to use
        //We'll set that in the ai states, check the original code

        if (Keyboard.GetState().IsKeyDown(Keys.L))
        {
            _keyDown = true;

        }
        if (_keyDown && !Keyboard.GetState().IsKeyDown(Keys.L))
        {

            _keyDown = false;
            SwitchState(AIState.FireTornado);
        }

        _numDirections = 0;
        _wingsPerspective = WingsPerspective.ThreeQ;
        _wingAnimationFrame.maxFrame = 60;
        _wingAnimationFrame.frameSpeed = 2;
        _wingAnimationFrame.UpdateTick();
        _telegraphLineAlpha = MathHelper.Lerp(_telegraphLineAlpha, 0f, 0.1f);
        _renderFigure8Trail = false;
        _renderFinger = false;
        _renderAfterImage = false;
        ShootRotations.Clear();
        switch (State)
        {
            case AIState.Despawn:
                AI_Despawn();
                break;
            case AIState.Spawn:
                SwitchState(AIState.Idle);
                break;
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Dichotamy:
                AI_Dichotamy();
                break;
            case AIState.Archery:
                AI_Archery();
                break;
            case AIState.BoostBounce:
                AI_BoostBounce();
                break;
            case AIState.Kick:
                AI_Kick();
                break;
            case AIState.TheZoomer:
                AI_TheZoomer();
                break;
            case AIState.Suns:
                AI_Suns();
                break;
            case AIState.SniperShot:
                AI_SniperShot();
                break;
            case AIState.ComboAttack:
                AI_ComboAttack();
                break;
            case AIState.FireTornado:
                AI_FireTornado();
                break;
        }

        float targetFingerAlpha = _renderFinger ? 1f : 0f;
        _fingerAlpha = MathHelper.Lerp(_fingerAlpha, targetFingerAlpha, 0.1f);

        float targetAfterImageAlpha = _renderAfterImage ? 1f : 0f;
        _afterImageAlpha = MathHelper.Lerp(_afterImageAlpha, targetAfterImageAlpha, 0.1f);

        float targetAlpha = _renderFigure8Trail ? 1f : 0f;
        _figure8TrailAlpha = MathHelper.Lerp(_figure8TrailAlpha, targetAlpha, 0.1f);
        if (_telegraphLineOffTimer > 0)
        {
            _telegraphLineOffTimer--;
            _telegraphLineAlpha *= 0.4f;
        }
        _outliner.Update();
    }

    private void ResizeTrail(int length)
    {
        if (NPC.oldPos.Length != length)
        {
            NPC.oldPos = new Vector2[length];
        }
    }
    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            SwitchState(AttackPattern.NextPattern());
        }
    }

    private void AI_Despawn()
    {
        Timer++;
        if (Timer >= 90)
        {
            NPC.active = false;
        }
        NPC.velocity.X *= 0.97f;
        NPC.velocity.Y -= 0.05f;
    }
    private void AI_FireTornado()
    {
        void ZoomMiddle()
        {
            CameraTargetSystem.AddTarget(Vector2.Lerp(MyTarget.Center, _startCDashOffset, 0.3f));
        }

        ResizeTrail(24);
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                    }

                    _wingsPerspective = WingsPerspective.FourQ;
                    Animator.PlayAnimation(Anim_Floating);
                    Vector2 pointToMoveTo = MyTarget.Center + new Vector2(0, -256);
                    NPC.velocity *= 0.8f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
                    NPC.Center = Vector2.Lerp(NPC.Center, pointToMoveTo, 0.1f);
                    if (Timer >= 60)
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

                        _startCDashOffset = MyTarget.Center;
                        _endCDashOffset = -Vector2.UnitY * 512;
                        _initialVelocity = NPC.velocity;
                    }

                    _wingsPerspective = WingsPerspective.ThreeQ;

                    _outliner.warning = true;
                    float ratio = Timer / FireTornado_CircleSpeedUpTime;
                    float ease = EasingFunction.InExpo(ratio);
                    float radiansToRotateBy = MathHelper.Lerp(0, MathHelper.ToRadians(15), ease);
                    _endCDashOffset = _endCDashOffset.RotatedBy(radiansToRotateBy);

                    int time = (int)(FireTornado_CircleSpeedUpTime * 0.85f);
                    if (Timer == time)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), _startCDashOffset, Vector2.Zero,
                                ModContent.ProjectileType<FlameHurricane>(), 1, 1, Owner: Main.myPlayer);
                        }
                    }
                    ZoomMiddle();

                    if (Timer >= time)
                    {
                        _renderAfterImage = true;
                        _renderFigure8Trail = true;
                        MakeCircles(Timer);
                    }

                    Vector2 positionToMoveTo = _startCDashOffset + _endCDashOffset;
                    Vector2 vel = positionToMoveTo - NPC.Center;
                    NPC.velocity = Vector2.Lerp(_initialVelocity, vel, EasingFunction.InOutSine(Timer / 30f));

                    float targetRotation = vel.ToRotation() + MathHelper.PiOver2;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, targetRotation, 0.3f);
                    Animator.PlayAnimation(Anim_Dive);
                    if (Timer >= FireTornado_CircleSpeedUpTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    ZoomMiddle();
                    _endCDashOffset = _endCDashOffset.RotatedBy(MathHelper.ToRadians(15));

                    Vector2 positionToMoveTo = _startCDashOffset + _endCDashOffset;
                    Vector2 vel = positionToMoveTo - NPC.Center;
                    NPC.velocity = vel;

                    float targetRotation = vel.ToRotation() + MathHelper.PiOver2;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, targetRotation, 0.4f);
                    Animator.PlayAnimation(Anim_Dive);

                    _renderFigure8Trail = true;
                    _renderAfterImage = true;
                    _outliner.attacking = true;
                    if (Timer >= FireTornado_TimeBetweenCircleWaves)
                    {
                        Vector2 dirToTarget = (MyTarget.Center - _startCDashOffset).SafeNormalize(Vector2.Zero);
                        dirToTarget = dirToTarget.RotatedByRandom(MathHelper.ToRadians(24));
                        float midAngle = dirToTarget.ToRotation();
                        float angleRadius = 0.55f;
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(SourceFromThis, _startCDashOffset, Vector2.Zero,
                                ModContent.ProjectileType<FlameSwirl>(), 1, 1, Main.myPlayer, ai1: midAngle, ai2: angleRadius);
                        }
                        Timer = 0;
                        AttackCounter++;
                        if (AttackCounter >= FireTornado_CircleCount)
                        {
                            AttackCycle++;
                        }
                    }
                }
                break;
            case 3:
                {
                    NPC.velocity *= 0.8f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
                    if (Timer >= 60)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }

    private void AI_ComboAttack()
    {
        void Blast()
        {
            if (!MultiplayerHelper.IsHost)
                return;


            for (int i = 0; i < ShootRotations.Count; i++)
            {
                float angle = ShootRotations[i];
                Vector2 offset = angle.ToRotationVector2();
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, offset * 2400,
                    ModContent.ProjectileType<GothinTorch>(), 1, 1, Main.myPlayer);
            }
        }

        void SetBlowtorchShootRotations()
        {
            switch (AttackCounter)
            {
                case 0:
                    {
                        for (float angle = 0; angle < MathHelper.TwoPi; angle += MathHelper.PiOver2)
                        {
                            ShootRotations.Add(angle);
                        }
                    }
                    break;
                case 1:
                    {
                        for (float angle = MathHelper.PiOver4; angle < MathHelper.TwoPi; angle += MathHelper.PiOver2)
                        {
                            ShootRotations.Add(angle);
                        }
                    }
                    break;
                case 2:
                    {
                        for (float angle = 0; angle < MathHelper.TwoPi; angle += MathHelper.PiOver4)
                        {
                            ShootRotations.Add(angle);
                        }
                    }
                    break;
                case 3:
                    {
                        for (float angle = 0; angle < MathHelper.TwoPi; angle += MathHelper.PiOver4)
                        {
                            float a = angle + MathHelper.ToRadians(22.5f);
                            ShootRotations.Add(a);
                        }
                    }
                    break;
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
                    }

                    _outliner.warning = true;
                    Animator.PlayAnimation(Anim_Floating);
                    FaceTarget();
                    float x = -NPC.spriteDirection;
                    Vector2 positionToMoveTo = MyTarget.Center + new Vector2(0, -256);
                    NPC.velocity = Vector2.Zero;
                    NPC.Center = Vector2.Lerp(NPC.Center, positionToMoveTo, 0.05f);
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
                    if (Timer >= ComboAttack_PrepTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 1:
                {
                    _outliner.warning = true;
                    _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / ComboAttack_BlowtorchTelegraphTime));
                    Animator.PlayAnimation(Anim_Arrowhold);
                    FaceTarget();
                    SetBlowtorchShootRotations();
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
                    if (Timer >= ComboAttack_BlowtorchTelegraphTime)
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
                        SetBlowtorchShootRotations();
                        Blast();
                    }
                    Animator.PlayAnimation(Anim_Arrowshot);
                    _outliner.attacking = true;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
                    if (Timer >= ComboAttack_ShootTime)
                    {
                        Timer = 0;
                        AttackCounter++;
                        if (AttackCounter >= 4)
                        {
                            AttackCycle++;
                        }
                        else
                        {
                            AttackCycle--;
                        }
                    }
                }
                break;

            case 3:
                {
                    Animator.PlayAnimation(Anim_Floating);
                    _outliner.attacking = true;
                    float ratio = Timer / ComboAttack_BlastUpTime;
                    ChargeParticlesBig(NPC.Center, Timer);
                    //I should really stop writing nested interpolations like this
                    //But it's funny
                    Vector2 velocity = -Vector2.UnitY * 18;
                    Vector2 interpolatedVelocity = Vector2.Lerp(Vector2.Lerp(Vector2.Zero, -velocity * 0.5f, EasingFunction.OutCirc(ratio)), velocity, EasingFunction.InOutExpo(ratio));
                    NPC.velocity = interpolatedVelocity * MathHelper.Lerp(1f, 2.5f, (Timer - ComboAttack_BlastUpTime) / 30f);
                    NPC.rotation = NPC.velocity.X * 0.05f;
                    if (Timer >= ComboAttack_BlastUpTime && NPC.Center.Y < MyTarget.Center.Y)
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
                        if (MultiplayerHelper.IsHost)
                        {

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY * 2400,
                                ModContent.ProjectileType<GothinTorch>(), 1, 1, Main.myPlayer, ai2: 1);
                        }
                    }

                    if (Timer % 4 == 0)
                    {
                        var dp = DustParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(32, 32), -NPC.velocity.SafeNormalize(Vector2.Zero));
                        dp.noTileCollide = true;
                        dp.gravity = 0;
                    }

                    CameraTargetSystem.AddTarget(Vector2.Lerp(MyTarget.Center, NPC.Center, 0.3f));
                    Animator.PlayAnimation(Anim_Dive);
                    _renderFigure8Trail = true;
                    _outliner.attacking = true;
                    NPC.velocity *= 1.05f;
                    NPC.velocity = NPC.velocity.RotatedBy(0.05f);
                    if (Timer >= 8)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 5:
                {
                    _contactDamage = true;
                    _renderFigure8Trail = true;
                    _renderAfterImage = true;
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        _startCDashOffset = NPC.Center;
                        _endCDashOffset = NPC.velocity;
                        _dashDirection = NPC.Center.X < MyTarget.Center.X ? -1 : 1;
                    }

                    Vector2 endPoint = MyTarget.Center;

                    float ratio = Timer / ComboAttack_ZoomTime;

                    Vector2 inBetweenPoint = Vector2.Lerp(_startCDashOffset, endPoint, ratio);
                    Vector2 offset = Vector2.Lerp(Vector2.Zero, Vector2.UnitX * 445 * _dashDirection, EasingFunction.QuickOutSlowIn(ratio));
                    Vector2 offset2 = Vector2.Lerp(-Vector2.UnitY * 128, Vector2.Zero, EasingFunction.InOutSine(ratio));
                    Vector2 pointToMoveTo = inBetweenPoint + offset + offset2;
                    Vector2 vel = pointToMoveTo - NPC.Center;
                    NPC.velocity = Vector2.Lerp(_endCDashOffset, vel, EasingFunction.InOutSine(Timer / 20f));
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation() + MathHelper.PiOver2, 0.1f);

                    CameraTargetSystem.AddTarget(Vector2.Lerp(MyTarget.Center, NPC.Center, 0.3f));
                    Animator.PlayAnimation(Anim_Dive);

                    if (Timer >= ComboAttack_ZoomTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 6:
                {
                    _contactDamage = true;
                    _renderFigure8Trail = true;
                    _renderAfterImage = true;
                    _outliner.attacking = true;
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();

                        _startCDashOffset = (NPC.Center - MyTarget.Center);
                        _endCDashOffset = MyTarget.Center;
                        _endCDashOffset.Y -= 512;
                        _initialVelocity = NPC.velocity;
                        ;
                        // _dashDirection = NPC.Center.X < MyTarget.Center.X ? -1 : 1;
                        //    _dashDirection *= -1;
                    }

                    float time = ComboAttack_SecondZoomTime;
                    float maxRadians = MathHelper.ToRadians(233);
                    float radians = maxRadians / time;
                    NPC.velocity = NPC.velocity.RotatedBy(radians);
                    if (Timer >= time)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }

                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation() + MathHelper.PiOver2, 0.1f);
                    CameraTargetSystem.AddTarget(Vector2.Lerp(MyTarget.Center, NPC.Center, 0.3f));
                    Animator.PlayAnimation(Anim_Dive);


                }
                break;
            case 7:
                {
                    float reelInTime = 90;
                    _contactDamage = true;
                    _renderFigure8Trail = true;
                    _renderAfterImage = true;
                    _outliner.attacking = true;
                    Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(NPC.Center, MyTarget.Center, NPC.velocity, 8);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, homingVelocity, 0.35f);

                    if (Timer < reelInTime * 0.5f)
                        NPC.velocity *= 0.96f;
                    else if (NPC.velocity.Length() < 60)
                    {
                        MakeCircles(Timer);
                        NPC.velocity *= 1.4f;
                    }

                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation() + MathHelper.PiOver2, 0.1f);
                    CameraTargetSystem.AddTarget(Vector2.Lerp(MyTarget.Center, NPC.Center, 0.3f));
                    Animator.PlayAnimation(Anim_Dive);
                    if (Timer >= reelInTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 8:
                {
                    NPC.velocity *= 0.92f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    Animator.PlayAnimation(Anim_Floating);
                    if (Timer >= ComboAttack_EndingTime)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }

    private void AI_SniperShot()
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

                    if (Timer == 10)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/BindingBless1") with { PitchVariance = 0.6f }, NPC.Center);
                        CreateInCircle();
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                                ModContent.ProjectileType<BlinkingStar>(), 24, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                        }
                    }

                    _wingsPerspective = WingsPerspective.ThreeQ;
                    Animator.PlayAnimation(Anim_Dichotamy);
                    _outliner.warning = true;
                    FaceTarget();
                    float x = -NPC.spriteDirection;
                    Vector2 positionToMoveTo = MyTarget.Center + new Vector2(x * 256, 0);
                    NPC.velocity = Vector2.Zero;
                    NPC.Center = Vector2.Lerp(NPC.Center, positionToMoveTo, 0.05f);
                    if (Timer >= SniperShot_PrepTime)
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
                        FXUtil.ApplyVignette(1f, timer: SniperShot_TelegraphTime);
                    }
                    _wingsPerspective = WingsPerspective.ThreeQ;
                    Animator.PlayAnimation(Anim_Arrowhold);
                    _outliner.warning = true;
                    _renderFinger = true;
                    _bowDissipateAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 30f));
                    _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(Timer / 60f));

                    Vector2 targetVelocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);


                    float radians = MathHelper.Lerp(MathHelper.ToRadians(360), 0, EasingFunction.InOutExpo(Timer / SniperShot_TelegraphTime));

                    _aimingVelocity = targetVelocity.RotatedBy(radians);

                    Vector2 offset = -Vector2.UnitY;
                    offset *= 384;
                    offset = offset.RotatedBy(radians);
                    Vector2 pointToMoveTo = MyTarget.Center + offset;
                    NPC.Center = Vector2.Lerp(NPC.Center, pointToMoveTo, MathHelper.Lerp(0f, 0.1f, EasingFunction.InOutExpo(Timer / 60f)));
                    NPC.velocity = Vector2.Zero;

                    if (Timer < SniperShot_TelegraphTime - 30)
                    {
                        _renderAfterImage = true;


                    }

                    if (Timer == SniperShot_TelegraphTime - 30)
                    {
                        CreateInCircle();
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                                ModContent.ProjectileType<BlinkingStar>(), 24, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                        }
                    }
                    if (Timer >= SniperShot_TelegraphTime - 30)
                    {
                        float t = Timer - (SniperShot_TelegraphTime - 30);
                        _bowDissipateAlpha = MathHelper.Lerp(1f, 0f, t / 30f);

                    }
                    if (Timer >= SniperShot_TelegraphTime)
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
                        if (MultiplayerHelper.IsHost)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, _aimingVelocity.SafeNormalize(Vector2.Zero) * 2400,
                                ModContent.ProjectileType<GothinTorch>(), 1, 1, Main.myPlayer, ai2: 1);
                        }
                        NPC.velocity = -Vector2.UnitY * 24;
                    }
                    Animator.PlayAnimation(Anim_Arrowshot);
                    NPC.velocity *= 0.96f;
                    _outliner.attacking = true;
                    if (Timer >= SniperShot_ShootTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    NPC.velocity *= 0.96f;

                    FaceTarget();
                    float x = -NPC.spriteDirection;
                    Vector2 positionToMoveTo = MyTarget.Center + new Vector2(0, -256);
                    NPC.velocity = Vector2.Zero;
                    NPC.Center = Vector2.Lerp(NPC.Center, positionToMoveTo, 0.1f);

                    if (Timer >= 30)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }
    private void AI_Suns()
    {
        float ai1 = NPC.whoAmI;

        Timer++;
        Animator.PlayAnimation(Anim_Aurafarming);
        if (Timer == 1)
        {
            NPC.TargetClosest();
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/BindingBless1") with { PitchVariance = 0.6f }, NPC.Center);
            CreateInCircle();
            if (MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                    ModContent.ProjectileType<BlinkingStar>(), 24, 0f, Main.myPlayer, 0f, ai1);
            }
        }
        if (Timer < 80)
        {
            _outliner.warning = true;
            FaceTarget();
            Vector2 targetCenter = MyTarget.Center;
            Vector2 targetHoverCenter = targetCenter + new Vector2(0, -256);
            NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);
            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Zero, 0.1f);
        }

        if (Timer > 81)
        {
            _outliner.attacking = true;
        }

        //NPC.velocity *= Vector2.Zero;
        if (Timer == 81)
        {
            if (MultiplayerHelper.IsHost)
            {

                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                    ModContent.ProjectileType<RedSun>(), 1, 0, Main.myPlayer, ai2: NPC.whoAmI);
            }
        }

        if (Timer >= 900)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void ResetTrail()
    {
        for (int i = 0; i < NPC.oldPos.Length; i++)
        {
            NPC.oldPos[i] = Vector2.Zero;
        }
    }

    private void AI_TheZoomer()
    {
        ResizeTrail(128);
        FaceTarget();

        Timer++;
        Player target = MyTarget;
        float ai1 = NPC.whoAmI;

        _figureEightStartCenter = Vector2.Lerp(_figureEightStartCenter, target.Center, 0.07f);
        if (Timer == 1)
        {
            ResetTrail();
            NPC.TargetClosest();
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/BindingBless1") with { PitchVariance = 0.6f }, NPC.Center);
            CreateInCircle();
            if (MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                    ModContent.ProjectileType<BlinkingStar>(), 1, 0f, Main.myPlayer, 0f, ai1);
            }
        }

        float te = 90;
        if (Timer < te)
        {
            NPC.rotation *= 0.4f;
            Animator.PlayAnimation(Anim_Floating);
            _outliner.warning = true;
        }


        if (Timer < te)
        {
            //I should really stop writing nested interpolations like this
            //But it's funny
            Vector2 goDown = Vector2.Lerp(Vector2.Zero, Vector2.UnitY * 14, EasingFunction.InOutSine(Timer / te));
            Vector2 goUp = Vector2.Lerp(Vector2.Zero, -Vector2.UnitY * 40, EasingFunction.OutExpo(Timer / te));
            Vector2 combine = Vector2.Lerp(goDown, goUp, EasingFunction.InExpo(Timer / te));
            NPC.velocity = combine;


            float fixTime = 45;
            if (Timer < fixTime)
            {
                Vector2 targetCenter = target.Center;
                Vector2 targetHoverCenter = targetCenter + new Vector2(0, MathHelper.Lerp(256, 444, EasingFunction.InOutSine(Timer / fixTime)));
                //     targetHoverCenter.Y += 256;
                NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, EasingFunction.OutExpo(Timer / fixTime));

            }
            else
            {
                _renderAfterImage = true;
                _renderFigure8Trail = true;
            }


            NPC.rotation = NPC.velocity.X * 0.05f;
        }

        if (Timer == te)
        {
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WavingGoth2") with { PitchVariance = 0.5f }, MyTarget.Center);
        }

        float up = te + 380;

        if (Timer > te && Timer < up)
        {
            Animator.PlayAnimation(Anim_Dive);
            _renderAfterImage = true;
            _renderFigure8Trail = true;
            _outliner.attacking = true;
            _contactDamage = true;

            float movementSpeed = 40;
            float size = 812;
            float figureEightSpeed = 0.06f;

            float t = Timer * figureEightSpeed;
            float scale = 2 / (3 - MathF.Cos(2 * t));

            scale *= size;
            float x = scale * MathF.Cos(t);
            float y = scale * MathF.Sin(2 * t) / 2;

            Vector2 targetCenter = _figureEightStartCenter + new Vector2(x, y);
            Vector2 targetVelocity = NPC.Center.DirectionTo(targetCenter) * movementSpeed;
            float distance = Vector2.Distance(NPC.Center, targetCenter);
            if (distance < movementSpeed)
            {
                targetVelocity = NPC.Center.DirectionTo(targetCenter) * distance;
            }

            if (Timer % 3 == 0)
            {
                var dp = DustParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(64, 64), -NPC.velocity.SafeNormalize(Vector2.Zero) * 5);
                dp.innerColor = Color.Yellow;
                dp.outerColor = Color.Red;
                dp.Scale *= 1.2f;
                dp.gravity = 0.05f;
                dp.dampening = 0.05f;
                dp.fast = true;
                dp.noTileCollide = true;
            }

            float ratio = (Timer - te) / 120f;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, EasingFunction.InOutSine(ratio));
        }

        if (Timer >= up + 40)
        {
            NPC.velocity *= 0.2f;
            SwitchState(AIState.Suns);
            NPC.rotation = 0;
        }
    }

    private void AI_Kick()
    {
        FaceTarget();
        Timer++;
        float ai1 = NPC.whoAmI;
        if (Timer == 2)
        {
            if (MultiplayerHelper.IsHost)
            {
                AttackCycle = Main.rand.Next(1, 5);
                NPC.netUpdate = true;
            }
        }
        _numDirections = 8;
        _wingsPerspective = WingsPerspective.ThreeQ;
        NPC.velocity *= 0.96f;
        float speed = InPhase2 ? 26f : 23f;
        if (Timer < 15 && Timer > 3)
        {
            _outliner.warning = true;
            if (Timer == 10)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/BindingBless1") with { PitchVariance = 0.7f }, NPC.Center);
            }

            if (AttackCycle == 1)
            {
                Vector2 targetCenter = MyTarget.Center;
                Vector2 targetHoverCenter = targetCenter + new Vector2(0, -300);
                NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);

                float hoverSpeed = 5;
                float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
            }
            else if (AttackCycle == 2)
            {
                Vector2 targetCenter = MyTarget.Center;
                Vector2 targetHoverCenter = targetCenter + new Vector2(0, 300);
                NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);

                float hoverSpeed = 5;
                float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
            }
            else if (AttackCycle == 3)
            {
                Vector2 targetCenter = MyTarget.Center;
                Vector2 targetHoverCenter = targetCenter + new Vector2(300, 0);
                NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);

                float hoverSpeed = 5;
                float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
            }
            else if (AttackCycle == 4)
            {
                Vector2 targetCenter = MyTarget.Center;
                Vector2 targetHoverCenter = targetCenter + new Vector2(-300, 0);
                NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);

                float hoverSpeed = 5;
                float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
            }
        }

        if (Timer > 15 && Timer < 70)
        {

            _outliner.warning = true;
            if (Timer == 25)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/BlindingBless2") with { PitchVariance = 0.6f }, NPC.Center);
            }

            if (AttackCycle == 1)
            {
                Vector2 targetCenter = MyTarget.Center;
                Vector2 targetHoverCenter = targetCenter + new Vector2(-300, 0);
                NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);

                float hoverSpeed = 5;
                float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
            }
            else if (AttackCycle == 2)
            {
                Vector2 targetCenter = MyTarget.Center;
                Vector2 targetHoverCenter = targetCenter + new Vector2(300, 0);
                NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);

                float hoverSpeed = 5;
                float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
            }
            else if (AttackCycle == 3)
            {
                Vector2 targetCenter = MyTarget.Center;
                Vector2 targetHoverCenter = targetCenter + new Vector2(0, -450);
                NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);

                float hoverSpeed = 5;
                float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
            }
            else if (AttackCycle == 4)
            {
                Vector2 targetCenter = MyTarget.Center;
                Vector2 targetHoverCenter = targetCenter + new Vector2(0, 450);
                NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);

                float hoverSpeed = 5;
                float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
            }
        }


        Vector2 direction = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8.5f;
        if (Timer == 24)
        {
            if (MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction,
                    ModContent.ProjectileType<BlinkingStar>(), NPC.damage, 0f, Main.myPlayer, 0f, ai1);

            }
        }
        if (Timer >= 70)
        {
            _outliner.attacking = true;
        }

        if (Timer > 70 && Timer < 82)
        {


            if (Timer == 71)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RazorWing") with { PitchVariance = 0.6f }, NPC.Center);

                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction,
                        ModContent.ProjectileType<RazorWingDash>(), 1, 0f, Main.myPlayer, 0f, ai1);
                }
            }

            Vector2 dashDirection = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * speed;
            NPC.velocity = dashDirection;
            ShakeScreenPosition.Shake = 4;
        }

        if (Timer > 100 && Timer < 135)
        {
            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo((Timer - 100) / 30f));
        }
        if (Timer > 50 && Timer < 56)
        {
            Vector2 dashDirection = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * speed;
            NPC.velocity = dashDirection;
            ShakeScreenPosition.Shake = 3;
        }

        if (Timer >= 150)
        {
            float numTimes = InPhase2 ? 8 : 4;
            AttackCounter++;
            if (AttackCounter >= numTimes)
            {
                SwitchState(AIState.BoostBounce);
            }
            else
            {
                Timer = 0;
            }

            NPC.velocity *= 0.3f;
        }
    }

    private void AI_BoostBounce()
    {
        FaceTarget();
        _renderAfterImage = true;
        NPC.velocity *= 0.96f;
        Timer++;
        if (AttackCounter == 0)
        {
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            float ai1 = NPC.whoAmI;
            if (Timer == 2)
            {
                PixelPrimitiveCircleFactory.CreateGenericInBoom(NPC.Center, Color.White, Color.White, 45, 444);
            }

            if (Timer < 50)
            {
                Animator.PlayAnimation(Anim_Floating);
                _outliner.warning = true;
                Vector2 targetCenter = MyTarget.Center;
                Vector2 targetHoverCenter = targetCenter + new Vector2(0, -300);
                NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);

                float hoverSpeed = 5;
                float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
            }
            else
            {
                Animator.PlayAnimation(Anim_Kickstart);
                _outliner.attacking = true;
            }

            float speed = InPhase2 ? 18f : 16f;
            Vector2 direction = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8.5f;
            if (Timer == 51)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothKickSlap") with { PitchVariance = 0.7f }, NPC.Center);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RazorClash") with { PitchVariance = 0.7f }, NPC.Center);
                if (MultiplayerHelper.IsHost)
                {
                    float var = AttackCounter % 2 == 0 ? 0 : 1;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction,
                        ModContent.ProjectileType<Kickboom>(), 1, 0f, Main.myPlayer, 0f, ai1, ai2: var);
                }
            }

            if (Timer > 50 && Timer < 56)
            {
                Vector2 dashDirection = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                Vector2 dashVelocity = dashDirection * speed;
                NPC.velocity = dashDirection;

            }
            if (Timer >= 85)
            {
                Timer = 0;
                AttackCounter++;
                NPC.velocity *= 0.3f;
                if (AttackCounter >= 3)
                {
                    SwitchState(AIState.Idle);
                }
            }
        }
        else
        {
            Player target = Main.player[NPC.target];
            float ai1 = NPC.whoAmI;

            float speed = InPhase2 ? 26f : 20f;
            if (NPC.life < NPC.lifeMax / 2)
            {
                speed = 26f;
            }
            if (NPC.life > NPC.lifeMax / 2)
            {
                speed = 20f;
            }

            Vector2 direction = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8.5f;
            if (Timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothKickSlap") with { PitchVariance = 0.7f }, NPC.Center);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RazorClash") with { PitchVariance = 0.7f }, NPC.Center);
                if (MultiplayerHelper.IsHost)
                {
                    float var = AttackCounter % 2 == 0 ? 0 : 1;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction,
                        ModContent.ProjectileType<Kickboom>(), 1, 0f, Main.myPlayer, 0f, ai1, ai2: var);
                }
            }

            float e = 5;
            if (AttackCounter == 2)
            {
                e = 10;
                speed += 4;
            }

            if (Timer < e)
            {
                Vector2 dashDirection = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * speed;
                NPC.velocity = dashDirection;
            }

            if (Timer >= 45)
            {
                Timer = 0;
                AttackCounter++;
                NPC.velocity *= 0.3f;
                if (AttackCounter >= 3)
                {
                    SwitchState(AIState.Idle);
                }
            }
        }



    }

    private void AI_Idle()
    {

        _wingsPerspective = WingsPerspective.FourQ;
        NPC.velocity *= 0.96f;
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
        }

        Animator.PlayAnimation(Anim_Floating);
        FaceTarget();
        Vector2 targetCenter = MyTarget.Center;
        Vector2 targetHoverCenter = targetCenter + new Vector2(0, -196);
        NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.05f);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
        if (Timer >= 60)
        {
            SwitchState(AIState.Dichotamy);
        }
    }

    private void AI_Dichotamy()
    {
        NPC.velocity *= 0.96f;
        Animator.PlayAnimation(Anim_Dichotamy);
        Timer++;
        Player player = Main.player[NPC.target];
        float ai1 = NPC.whoAmI;
        if (Timer == 1)
        {
            FXUtil.ApplyVignette(2f, timer: 100);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothSummon") { PitchVariance = 0.3f }, NPC.Center);
            PixelPrimitiveCircleFactory.CreateGenericInBoom(NPC.Center, Color.White, Color.White, 80, 460);
            if (MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                    ModContent.ProjectileType<BlinkingStar>(), NPC.damage, 0f, Main.myPlayer, 0f, ai1);
            }
        }
        CameraTargetSystem.AddTarget(Vector2.Lerp(MyTarget.Center, NPC.Center, 0.35f));

        if (Timer == 80)
        {
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DUAL2") { PitchVariance = 0.5f }, NPC.Center);
            ShakeScreenPosition.Shake = 5;
            if (MultiplayerHelper.IsHost)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 offset = Vector2.UnitY * 512;
                    offset = offset.RotatedBy(i / 2f * MathHelper.TwoPi);
                    Vector2 spawnPoint = NPC.Center + offset;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPoint, -offset, ModContent.ProjectileType<BouncingRazorSuns>(), 1, 1, Main.myPlayer, ai2: i);
                }
            }
        }


        if (Timer >= 150)
        {
            SwitchState(AIState.Archery);
        }
    }

    private float _circleDegrees;
    private float _circleDistance;
    private float _circleSpeed;
    private float _movementSpeed;
    private float _accelTimer;
    private void FaceTarget()
    {
        NPC.direction = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
        NPC.spriteDirection = NPC.direction;
    }

    private void AI_Archery()
    {
        void BowShot()
        {
            //Setting the attack cycle to 1 in this case does the bow shot
            AttackCycle = 2;
            PixelPrimitiveCircleFactory.CreateGenericInBoom(NPC.Center, Color.White, Color.White, 80, 460);
            _telegraphLineAlpha = 0;
            _telegraphLineOffTimer = 45;
            if (!MultiplayerHelper.IsHost)
                return;


            // Vector2 direction = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 2400;
            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, _aimingVelocity.SafeNormalize(Vector2.Zero) * 2400,
                ModContent.ProjectileType<GothinTorch>(), 1, 1, Main.myPlayer);
        }

        _outliner.attacking = true;

        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
        }


        FaceTarget();
        Vector2 velocity = NPC.Center.DirectionTo(MyTarget.Center) * 10;
        float ai1 = NPC.whoAmI;
        if (Timer == 3)
        {
            _circleDistance = 270;
        }

        if (Timer == 80)
        {
            _movementSpeed = 12;
            _circleSpeed = 3;
        }

        if (Timer == 170)
        {
            _movementSpeed = 25;

        }

        if (Timer == 210)
        {
            _movementSpeed = 16;
        }


        if (Timer == 240)
        {
            _movementSpeed = 12;
            _circleSpeed = 2;
        }


        void Circle()
        {
            float movementSpeed = 17;
            Vector2 offset = -Vector2.UnitY * 200;
            offset = offset.RotatedBy(MathHelper.ToRadians(_circleDegrees));
            Vector2 targetPos = MyTarget.Center + offset;
            Vector2 targetVelocity = (targetPos - NPC.Center);
            NPC.velocity = VectorHelper.VelocitySlowdownTo(NPC.Center, targetPos, movementSpeed);

        }


        switch (AttackCycle)
        {
            case 0:
                _accelTimer++;
                _circleDegrees += _circleSpeed;
                Circle();

                {
                    Vector2 targetAimingVelocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    _aimingVelocity = Vector2.Lerp(_aimingVelocity, targetAimingVelocity, 1f);
                    _telegraphLineAlpha = MathHelper.Lerp(_telegraphLineAlpha, 1f, 0.3f);
                }


                if (Timer % 8 == 0 && _bowFrame < 3)
                {
                    _bowFrame++;
                }
                if (_bowDissipateAlpha < 1)
                    _bowDissipateAlpha += 0.045f;
                if (_bowFrame > 3)
                    _bowFrame = 0;
                Animator.PlayAnimation(Anim_Arrowhold);
                break;
            case 1:
                //                Circle();
                {
                    Vector2 targetAimingVelocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    _aimingVelocity = Vector2.Lerp(_aimingVelocity, targetAimingVelocity, 0.02f);

                }

                _accelTimer = 0;
                if (_bowDissipateAlpha < 1)
                    _bowDissipateAlpha += 0.045f;
                _telegraphLineAlpha = MathHelper.Lerp(_telegraphLineAlpha, 1f, 0.3f);
                _bowFrame = 3;
                NPC.velocity *= 0.98f;
                break;
            case 2:
                if (Timer % 8 == 0 && _bowFrame < 6)
                {
                    _bowFrame++;
                }
                _bowDissipateAlpha -= 0.05f;
                NPC.velocity *= 0.4f;
                Animator.PlayAnimation(Anim_Arrowshot);
                if (Animator.IsFinished())
                    AttackCycle = 0;
                break;
        }
        NPC.velocity *= 0.96f;

        void PrepareBowShot(int time)
        {
            if (Timer == time - 48)
            {
                AttackCycle = 1;
            }
            if (Timer == time)
            {
                BowShot();
            }
        }
        PrepareBowShot(60);
        PrepareBowShot(154);
        PrepareBowShot(248);

        if (Timer >= 282)
        {
            Timer = 0;
            AttackCounter++;
            if (AttackCounter >= 3)
            {
                ChooseAttack();
                //For now, we gotta make the discs first
                //SwitchState(AIState.Idle);
            }
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }

    public override void OnKill()
    {
        base.OnKill();
    }
}
