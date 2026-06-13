using Microsoft.Xna.Framework.Input;
using Stellamod.Common.WeaponUpgrade.UI;
using Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.IO;
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
        Hurricane,

        Tornado,

        TheZoomer,

        ComboAttack,

        SniperShot,

        ThrowSun,

        PhaseTransition,

        Teleport
    }

    private Queue<AIState> _patternBackingField;
    private Queue<AIState> AttackPattern
    {
        get
        {
            if(_patternBackingField == null)
            {
                _patternBackingField = new Queue<AIState>();
            }
            return _patternBackingField;
        }
    }
    private PatternManager<int>? _patternManagerBackingField;
    private PatternManager<int> ComboPattern
    {
        get
        {
            if(_patternManagerBackingField == null)
            {
                _patternManagerBackingField = new();
                _patternManagerBackingField.AddPattern(0, 1);
                _patternManagerBackingField.AddPattern(1, 1);
                _patternManagerBackingField.AddPattern(2, 1);
            }
            return _patternManagerBackingField;
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

    private int _timer;
    private bool _phase2Transition;
    private bool _keyDown;
    private WingsPerspective _wingsPerspective;
    private bool _contactDamage;

    private float _inCircleAlpha;
    private Vector2 _inCircleScale;
    
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

    private Vector2 _teleportPosition;
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
    private float ComboAttack_ZoomTime => 55;
    private float ComboAttack_SecondZoomTime => 21;
    private float ComboAttack_EndingTime => 15f;



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
        NPC.lifeMax = 340000;
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
    private void EnablePlatformArena()
    {
        DomainExpansionManager fallSystem = ModContent.GetInstance<DomainExpansionManager>();
        fallSystem.noWings = true;
        fallSystem.inSpace = true;
        fallSystem.hoveringPlatform = true;
        fallSystem.hoverPlatformY = Ground;
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
        Main.windSpeedTarget = -0.5f;
        Main.windSpeedCurrent = -0.5f;
        if (Main.rand.NextBool(2))
        {
            Vector2 pos = new Vector2();
            pos.X = Main.screenWidth + 500;
            pos.Y = Main.rand.Next(0, Main.screenHeight + 300);
            pos += Main.screenPosition;
            Vector2 vel = -Vector2.UnitX * 48 + -Vector2.UnitY * 0.2f;
            var ufp = UnderworldFlameParticle.Spawn(pos, vel, Scale: Main.rand.NextFloat(0.1f, 0.3f));
            ufp.ySlow = false;
          
//ufp.color *= 0.5f;
            ufp.gothivian = true;
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

            NPC.life = (int)(NPC.lifeMax * 0.48f);
            _keyDown = false;
            SwitchState(AIState.Hurricane);
        }

        if(_teleportPosition != Vector2.Zero)
        {
            NPC.Center = _teleportPosition;
            _teleportPosition = Vector2.Zero;
        }
        _numDirections = 0;
        _wingsPerspective = WingsPerspective.ThreeQ;
        _wingAnimationFrame.maxFrame = 60;
        _wingAnimationFrame.frameSpeed = 2;
        _wingAnimationFrame.UpdateTick();
        _telegraphLineAlpha = MathHelper.Lerp(_telegraphLineAlpha, 0f, 0.1f);
        _renderFigure8Trail = false;
        _renderFinger = false;
        _contactDamage = false;
        _renderAfterImage = false;
        _inCircleAlpha = MathHelper.Lerp(_inCircleAlpha, 0f, 0.1f);
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
            case AIState.ThrowSun:
                AI_ThrowSun();
                break;
            case AIState.SniperShot:
                AI_SniperShot();
                break;
            case AIState.ComboAttack:
                AI_ComboAttack();
                break;
            case AIState.Hurricane:
                AI_Hurricane();
                break;
            case AIState.Tornado:
                AI_Tornado();
                break;
            case AIState.PhaseTransition:
                AI_PhaseTransition();
                break;
            case AIState.Teleport:
                AI_Teleport();
                break;
        }
        if (State != AIState.TheZoomer)
            ResizeTrail(24);

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

    private void AI_Teleport()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        TeleportEffect(NPC.Center);
                        float dir = Main.rand.NextBool(2) ? 1 : -1;
                        Vector2 offset = Vector2.UnitX * dir * 500;
                        Vector2 teleportSpot = MyTarget.Center + offset + Vector2.UnitY * -384;
                        Teleport(teleportSpot);
                        TeleportEffect(teleportSpot);
                    }
                    Animator.PlayAnimation(Anim_ExplodeReverse);
                    NPC.velocity *= 0;
                    if(Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if(Timer >= 15)
                    {
                        ExitOutAttack();
                    }
                }
                break;
        }
    }
    private void AI_PhaseTransition()
    {
        _phase2Transition = true;
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        AttackPattern.Clear();
                    }
                    NPC.velocity *= 0.5f;
                    NPC.rotation *= 0;
                    Animator.PlayAnimation(Anim_Explode);
                    if (Animator.IsFinished())
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

                        Vector2 teleportSpot = MyTarget.Center + Vector2.UnitY * -128;
                        Teleport(teleportSpot);
                        _startCDashOffset = MyTarget.Center.X > teleportSpot.X ? Vector2.UnitX : -Vector2.UnitX;
                        _endCDashOffset = teleportSpot;
                        var fx = FXUtil.GlowCircleBoom(teleportSpot, Color.White, Color.Yellow, Color.Red, duration: 45, baseSize: 0.24f);
                        fx.Scale *= 2f;
                        for (float n = 0; n < 24; n++)
                        {
                            var dp = DustParticle.Spawn(teleportSpot, Main.rand.NextVector2Circular(24, 24));
                            dp.dampening = 0.05f;
                            dp.gravity = 0;
                            dp.noTileCollide = true;
                        }
                        PixelPrimitiveCircleFactory.CreateGenericBoom(teleportSpot, Color.Red, Color.Red, 45, 256);
                        SoundStyle flyAway = AssetReferences.Assets.Sounds.Fire.Gothiviaflyaway.Asset;
                        flyAway.PitchVariance = 0.4f;
                        SoundEngine.PlaySound(flyAway, MyTarget.Center);

                        ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                        shaderSystem.TintScreen(Color.Red, 0.1f, timer: 120);
                        shaderSystem.DistortScreen(TextureRegistry.NormalNoise1, new Vector2(0.001f, 0.001f), blend: 0.025f, timer: 120);

                    }
                    Animator.PlayAnimation(Anim_Aurafarming);
                    NPC.velocity *= 0.5f;
                    NPC.rotation *= 0;

                    if(Timer % 30 == 0)
                    {
                        var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.Yellow, Color.Red, duration: 15, baseSize: 0.24f);
                        fx.Scale *= 2f;
                    }
                    if (Timer % 10 == 0)
                    {
                    //    FXUtil.ShakeCamera(NPC.position, 1024, 24);
                        LegacyParticle.NewParticle<ShockParticle>(NPC.Center, Vector2.Zero, Color.White);
                    }

                    CameraTargetSystem.AddTarget(NPC.Center);
                    ShakeScreenPosition.Shake = 5;
                    var dp2 = DustParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(128, 128), -Vector2.UnitY * 0.3f);
                    dp2.gravity = 0;
                    dp2.noTileCollide = true;
                    dp2.dampening = 0.05f;
                    if(Timer >= 120)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    ExitOutAttack();
                }
                break;
        }
    }
    private void Teleport(Vector2 spot)
    {
        if (!MultiplayerHelper.IsHost)
            return;
        _teleportPosition = spot;
        NPC.netUpdate = true;
    }

    private void ResizeTrail(int length)
    {
        if (NPC.oldPos.Length != length)
        {
            NPC.oldPos = new Vector2[length];
        }
    }
    private bool IsBanned(AIState state)
    {
        if (!InPhase2)
        {
            switch (state)
            {
                case AIState.ComboAttack:
                case AIState.Tornado:
                case AIState.TheZoomer:
                    return true;
            }
        }
        return false;
    }
    private bool NoAttacksLeft()
    {
        return AttackPattern.Count <= 0;
    }
    private void ChoosePattern()
    {
        int pattern = ComboPattern.NextPattern();
        while(!InPhase2 && pattern > 0)
        {
            pattern = ComboPattern.NextPattern();
        }
        while (InPhase2 && pattern == 0)
        {
            pattern = ComboPattern.NextPattern();
        }
        AttackPattern.Clear();
        switch (pattern)
        {
            case 0:
                {
                    AttackPattern.Enqueue(AIState.Dichotamy);
                    AttackPattern.Enqueue(AIState.Kick);
                    AttackPattern.Enqueue(AIState.BoostBounce);
                    AttackPattern.Enqueue(AIState.Suns);
                    AttackPattern.Enqueue(AIState.SniperShot);
                    AttackPattern.Enqueue(AIState.BoostBounce);
                    AttackPattern.Enqueue(AIState.Hurricane);
                }
                break;
            case 1:
                {
                    AttackPattern.Enqueue(AIState.Dichotamy);
                    AttackPattern.Enqueue(AIState.Kick);
                    AttackPattern.Enqueue(AIState.BoostBounce);
                    AttackPattern.Enqueue(AIState.Hurricane);
                    AttackPattern.Enqueue(AIState.BoostBounce);
                    AttackPattern.Enqueue(AIState.Suns);
                    AttackPattern.Enqueue(AIState.SniperShot);
                    AttackPattern.Enqueue(AIState.BoostBounce);
                    AttackPattern.Enqueue(AIState.TheZoomer);
                    AttackPattern.Enqueue(AIState.ComboAttack);
                    AttackPattern.Enqueue(AIState.BoostBounce);
                }
                break;
            case 2:
                {
                    AttackPattern.Enqueue(AIState.ComboAttack);
                    AttackPattern.Enqueue(AIState.Hurricane);
                    AttackPattern.Enqueue(AIState.BoostBounce);
                    AttackPattern.Enqueue(AIState.Suns);
                    AttackPattern.Enqueue(AIState.SniperShot);
                    AttackPattern.Enqueue(AIState.BoostBounce);
                    AttackPattern.Enqueue(AIState.Kick);
                }
                break;
        }
    }
    private void ExitOutAttack() => ChooseAttack();

    private AIState _lastState;
    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            if (NoAttacksLeft())
            {
                ChoosePattern();
            }
  
            if (NPC.life < NPC.lifeMax * 0.5f && !_phase2Transition)
            {
                SwitchState(AIState.PhaseTransition);
                return;
            }
            _timer--;
            if (InPhase2 && _timer <= 0)
            {
                bool tornado = Main.rand.NextBool(2);
                if (tornado)
                {
                    _timer = 3;
                    SwitchState(AIState.Tornado);
                    return;
                }
            }

            AIState pattern = AttackPattern.Dequeue();
            SwitchState(pattern);
        }
    }

    private void AI_Tornado()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.velocity = Vector2.Zero;
                        NPC.TargetClosest();
                        TeleportEffect(NPC.Center);
                        float dir = Main.rand.NextBool(2) ? 1 : -1;
                        Vector2 offset = Vector2.UnitX * dir * 500;
                        Vector2 teleportSpot = MyTarget.Center + offset + Vector2.UnitY * 384;
                        Teleport(teleportSpot);
                        _startCDashOffset = MyTarget.Center.X > teleportSpot.X ? Vector2.UnitX : -Vector2.UnitX;
                        _endCDashOffset = teleportSpot;
                        TeleportEffect(teleportSpot);
                    }

                    if(Timer < 12)
                    {
                        ShakeScreenPosition.Shake = 4;
                    }

                    if (Timer == 19)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 vel = _startCDashOffset + -Vector2.UnitY * 0.75f;
                            vel = vel.SafeNormalize(Vector2.Zero);
                            vel *= 1200;
                            Projectile.NewProjectile(SourceFromThis, _endCDashOffset - vel * 0.5f, vel,
                                ModContent.ProjectileType<FireTornado>(), 1, 1, Main.myPlayer);
                        }
                    }
                    Vector2 vel2 = _startCDashOffset + -Vector2.UnitY * 0.75f;
                    vel2 = vel2.SafeNormalize(Vector2.Zero);
                    NPC.velocity += vel2 * 0.1f;
                    Animator.PlayAnimation(Anim_ExplodeReverse);
                    if (Timer >= 30)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    Animator.PlayAnimation(Anim_Dive);
                    if(Timer == 1)
                    {
                        SoundStyle fireCharge = AssetReferences.Assets.Sounds.Fire.FlaminCharge.Asset with { PitchVariance = 0.5f };
                        SoundEngine.PlaySound(fireCharge, NPC.position);

                        Vector2 vel = _startCDashOffset + -Vector2.UnitY * 0.75f;
                        vel = vel.SafeNormalize(Vector2.Zero);
                        NPC.velocity = vel;
                    }
                    _renderFigure8Trail = true;
                    _outliner.attacking = true;
        
                    float rot = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation() + MathHelper.PiOver2, 0.1f);
                    NPC.rotation = rot;
                    NPC.velocity *= 1.1f;
                    if(Timer >= 60)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    NPC.velocity *= 0.96f;
                    Animator.PlayAnimation(Anim_Explode);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                        Vector2 teleportSpot = MyTarget.Center - new Vector2(0, 384);
                        Teleport(MyTarget.Center - new Vector2(0, 384));
                        TeleportEffect(teleportSpot);
                    }
                    NPC.rotation = 0;
                    NPC.velocity *= 0f;
                    Animator.PlayAnimation(Anim_ExplodeReverse);
                    if (Animator.IsFinished())
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }
    private void AI_ThrowSun()
    {
        void ThrowSun()
        {
            foreach(var proj in Main.ActiveProjectiles)
            {
                if (proj.type != ModContent.ProjectileType<RedSun>())
                    continue;
                if (proj.ai[2] != NPC.whoAmI)
                    continue;


                if(proj.ModProjectile is RedSun sun)
                {
                    Vector2 throwVelocity = MyTarget.Center.X > NPC.Center.X ? Vector2.UnitX : -Vector2.UnitX;
                    sun.throwVelocity = throwVelocity * 2;
                }
                break;
            }
        }

        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                        CircleBlink();
                    }
                    Animator.PlayAnimation(Anim_Aurafarming);
                    _outliner.warning = true;
                    NPC.rotation *= 0;

                    float easeTime = 60;
                    NPC.velocity.Y += MathHelper.Lerp(0f, 0.25f, EasingFunction.InOutExpo(Timer / easeTime));
                    if(Timer >= easeTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    Animator.PlayAnimation(Anim_Aurafarming);
                    _outliner.warning = true;
                    NPC.velocity.Y *= 0.96f;
                    if(Timer >= 50)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    FaceTarget();
                    Animator.PlayAnimation(Anim_Kickstart);
                    _outliner.attacking = true;

                    if(Timer == 25)
                    {
                        ThrowSun();
                    }
                    NPC.velocity.Y *= 0.96f;
                    if(Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    NPC.velocity *= 0.96f;
                    Animator.PlayAnimation(Anim_Explode);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    NPC.velocity *= 0.96f;
                    if(Timer >= 25)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;

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
    private void AI_Hurricane()
    {
        void ZoomMiddle()
        {
            CameraTargetSystem.AddTarget(Vector2.Lerp(MyTarget.Center, _startCDashOffset, 0.7f));
        }


        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        _initialVelocity = NPC.Center;
                    }

                    _startCDashOffset = MyTarget.Center - Vector2.UnitY * 128;
                    _wingsPerspective = WingsPerspective.FourQ;
                    Animator.PlayAnimation(Anim_Floating);

                    float easeInTime = 100;
                    float ratio = Timer / easeInTime;
                    float ease = EasingFunction.InOutExpo(ratio);
              
                    NPC.velocity *= 0.8f;
                    NPC.Center = Vector2.Lerp(_initialVelocity, _startCDashOffset, ease);
                    //NPC.velocity = Vector2.Lerp(_initialVelocity, interpVelocity, EasingFunction.InExpo(ratio));
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
                    if (Timer >= easeInTime)
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
                        _startCDashOffset = MyTarget.Center - Vector2.UnitY* 128;
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

                    if (Timer >= time * 0.2f)
                    {
                       // _renderAfterImage = true;
                        _renderFigure8Trail = true;
                        MakeCircles(Timer);
                    }

                    _endCDashOffset = _endCDashOffset.RotatedBy(MathHelper.ToRadians(2.5f));
                    Vector2 positionToMoveTo = _startCDashOffset + _endCDashOffset;
                    Vector2 vel = positionToMoveTo - NPC.Center;
                    NPC.velocity = Vector2.Lerp(_initialVelocity, vel, EasingFunction.InOutCirc(Timer / 120f));

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
                    _outliner.attacking = true;
                    if (Timer >= FireTornado_TimeBetweenCircleWaves)
                    {
                        Vector2 dirToTarget = (MyTarget.Center - _startCDashOffset).SafeNormalize(Vector2.Zero);
                        float range = 55;
                        if (_phase2Transition)
                            range *= 2.5f;
                        dirToTarget = dirToTarget.RotatedByRandom(MathHelper.ToRadians(range));
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
                    ModContent.ProjectileType<GothinTorch>(), 1, 1, Main.myPlayer, ai2: 3);
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
                    Animator.PlayAnimation(Anim_Aurafarming);
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
                    Animator.PlayAnimation(Anim_Aurafarming);
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
                        SoundStyle chargeSound = AssetReferences.Assets.Sounds.Fire.FlaminChargeFast.Asset;
                        chargeSound = chargeSound with { PitchVariance = 0.5f };
                        SoundEngine.PlaySound(chargeSound, MyTarget.Center);
                    }

                    if (Timer % 4 == 0)
                    {
                        var dp = DustParticle.Spawn(NPC.Center + Main.rand.NextVector2Circular(32, 32), -NPC.velocity.SafeNormalize(Vector2.Zero));
                        dp.noTileCollide = true;
                        dp.gravity = 0;
                    }

                    //CameraTargetSystem.AddTarget(Vector2.Lerp(MyTarget.Center, NPC.Center, 0.13f));
                    Animator.PlayAnimation(Anim_Dive);
                    _renderFigure8Trail = true;
                    _outliner.attacking = true;
                    NPC.velocity *= 1.05f;
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
                    _outliner.attacking = true;
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        SoundStyle chargeSound = AssetReferences.Assets.Sounds.Fire.FlaminCharge.Asset;
                        chargeSound = chargeSound with { PitchVariance = 0.5f };
                        SoundEngine.PlaySound(chargeSound, MyTarget.Center);
                    }

                    MakeCircles(Timer);
                    NPC.velocity.X *= 0.8f;
                    NPC.velocity.Y -= 0.8f;
                    if(NPC.velocity.Y < 0)
                        NPC.velocity.Y *= 1.1f;
                    Animator.PlayAnimation(Anim_Dive);

                    if (Timer >= ComboAttack_ZoomTime * 0.8f)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 6:
                {
                    NPC.velocity *= 0.82f;
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);
                    Animator.PlayAnimation(Anim_Floating);
                    if (Timer >= ComboAttack_EndingTime)
                    {
                        ExitOutAttack();
                    }
                }
                break;
        }
    }

    private void AI_SniperShot()
    {
        Timer++;
        NPC.rotation *= 0.4f;

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
                    _renderFigure8Trail = true;
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
                    _renderFigure8Trail = true;
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
                    _renderFigure8Trail = true;
                    NPC.velocity *= 0.96f;

                    FaceTarget();
                    float x = -NPC.spriteDirection;
                    Vector2 positionToMoveTo = MyTarget.Center + new Vector2(0, -256);
                    NPC.velocity = Vector2.Zero;
                    NPC.Center = Vector2.Lerp(NPC.Center, positionToMoveTo, 0.1f);

                    if (Timer >= 30)
                    {
                        ExitOutAttack();
                    }
                }
                break;
        }
    }

    private void CircleBlinkSound()
    {
        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/BindingBless1") with { PitchVariance = 0.6f }, NPC.Center);
    }
    private void CircleBlink()
    {
        float ai1 = NPC.whoAmI;
        CreateInCircle();
        if (MultiplayerHelper.IsHost)
        {
            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                ModContent.ProjectileType<BlinkingStar>(), 24, 0f, Main.myPlayer, 0f, ai1);
        }
    }
    private void AI_Suns()
    {
        Timer++;
        Animator.PlayAnimation(Anim_Aurafarming);
        if (Timer == 1)
        {
            NPC.TargetClosest();
        
            _initialVelocity = NPC.velocity;
        }
        NPC.rotation *= 0.8f;

        float time = 164;
        if(Timer == (int)(time * 0.5f))
        {
            CircleBlinkSound();
            CircleBlink();
        }
        if (Timer < time)
        {
            _outliner.warning = true;
            FaceTarget();

            float ratio = Timer / time;
            float ease = EasingFunction.InOutExpo(ratio);
            Vector2 targetCenter = MyTarget.Center;
            Vector2 targetHoverCenter = targetCenter + new Vector2(0, -256);
            Vector2 targetVelocity = (targetHoverCenter - NPC.Center);
            Vector2 interpVelocity = Vector2.Lerp(_initialVelocity, targetVelocity, ease);
            NPC.velocity = interpVelocity;
        }

        if (Timer > time)
        {
            NPC.velocity *= 0.96f;
            _outliner.attacking = true;
        }

        //NPC.velocity *= Vector2.Zero;
        if (Timer == (int)time)
        {
            if (MultiplayerHelper.IsHost)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                    ModContent.ProjectileType<RedSun>(), 1, 0, Main.myPlayer, ai2: NPC.whoAmI);
            }
        }

        if (Timer >= 1100)
        {
            SwitchState(AIState.ThrowSun);
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


        if(Timer == 45)
        {
            SoundStyle zoomer = AssetReferences.Assets.Sounds.Fire.FlaminChargeFast.Asset;
            zoomer.PitchVariance = 0.4f;
            SoundEngine.PlaySound(zoomer, NPC.position);
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
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WavingGoth2") with {
                PitchVariance = 0.5f, Volume = 0.3f }, MyTarget.Center);
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
            ExitOutAttack();
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

        _renderFigure8Trail = true;
        _numDirections = 8;
        _wingsPerspective = WingsPerspective.ThreeQ;
        NPC.velocity *= 0.96f;
        float speed = InPhase2 ? 26f : 23f;
        if (Timer < 15 && Timer > 3)
        {
            Animator.PlayAnimation(Anim_Floating);
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
            Animator.PlayAnimation(Anim_Kickstart);

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
                ExitOutAttack();
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
        NPC.rotation *= 0.5f;
        NPC.velocity *= 0.96f;
        Timer++;
        if (AttackCounter == 0)
        {
            if (Timer == 1)
            {
                NPC.TargetClosest();
                _initialVelocity = NPC.velocity;
                SoundStyle fireFast = AssetReferences.Assets.Sounds.Fire.FlaminChargeFast.Asset;
                fireFast.PitchVariance = 0.4f;
                SoundEngine.PlaySound(fireFast, MyTarget.Center);
            }

            float ai1 = NPC.whoAmI;
            float easeInTime = 90;
            float r2 = Timer / easeInTime;
            float ease2 = EasingFunction.InOutExpo(r2);
            _inCircleScale = Vector2.Lerp(Vector2.One * 4, Vector2.Zero, ease2);
            _inCircleAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(r2));
            if (Timer == (int)(easeInTime * 0.5f))
            {
                CreateInCircle();
                SoundStyle fireFast = AssetReferences.Assets.Sounds.Fire.FlaminChargeFast.Asset;
                fireFast.Pitch = -0.4f;
                SoundEngine.PlaySound(fireFast, MyTarget.Center);
            }

            if (Timer < easeInTime)
            {
                Animator.PlayAnimation(Anim_Floating);
                _outliner.warning = true;
                Vector2 targetCenter = MyTarget.Center;
                Vector2 targetHoverCenter = targetCenter + new Vector2(0, -300);
                Vector2 targetVelocity = targetHoverCenter - NPC.Center;

                float ratio = Timer / easeInTime;
                float ease = EasingFunction.InOutExpo(ratio);
                Vector2 interpVelocity = Vector2.Lerp(_initialVelocity, targetVelocity, ease);
                NPC.velocity = interpVelocity;
            }
            else
            {
                Animator.PlayAnimation(Anim_Kickstart);
                _outliner.attacking = true;
            }

            float speed = InPhase2 ? 18f : 16f;
            Vector2 direction = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8.5f;
            if (Timer == (int)(easeInTime))
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

            if (Timer > easeInTime && Timer < easeInTime + 6)
            {
                Vector2 dashDirection = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                Vector2 dashVelocity = dashDirection * speed;
                NPC.velocity = dashDirection;

            }
            if (Timer >= easeInTime + 29)
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
                    ExitOutAttack();
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
            ChooseAttack();
            //SwitchState(AIState.Dichotamy);
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
                ExitOutAttack();
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
