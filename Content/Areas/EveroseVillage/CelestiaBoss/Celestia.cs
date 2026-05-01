using Stellamod.Assets;
using Stellamod.Common.Animations;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.EveroseVillage.CelestiaBoss.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.EveroseVillage.CelestiaBoss;

internal class Celestia : ScarletBoss,
    IDrawOutlines
{
    private enum AIState
    {
        Spawn,
        Despawn,
        Idle,
        Death,

        Horse_Ride_Backflip_Shot,
        Horse_Ride_Big_Bow_Shot,

        Bow_Spin,
        Grounded_Small_Shot,
        Backflip_Bow_Rain,
        Projection_Dash,

        Punish_Snipe,
        Dizzy,
    }
    private int _bowIndex;
    private bool _contactDamage;
    private bool _warning;
    private bool _attacking;
    private bool _showTrail;
    private bool _show;
    private bool _showSlideTrail;
    private bool _projectionFlicker;
    private float _projectionAlpha;
    private float _ghostAlpha;
    private float _alphaTimer;
    private bool _firstAttack;
    private float _trailAlpha;
    private float _slideTrailAlpha;
    private Color _outlineColor;
    private Vector2 _teleportPosition;
    private Vector2 _squishScale;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }


    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];
    private const string ANIM_CONFUSED = "Confused";
    private const string ANIM_IDLE = "Idle";
    private const string ANIM_DISAPPEAR = "Disappear";
    private const string ANIM_THROWBOW = "ThrowBow";
    private const string ANIM_HOLDINGBOW = "HoldingBow";
    private const string ANIM_BOWOUT = "BowOut";
    private const string ANIM_BACKFLIPREADY = "BackflipReady";
    private const string ANIM_BACKFLIP = "Backflip";
    private const string ANIM_AIRTIME = "Airtime";
    private const string ANIM_LANDBACKFLIP = "LandBackflip";

    private PatternManager<AIState> _horseAttacksBackingField;
    private PatternManager<AIState> HorseAttacks
    {
        get
        {
            if(_horseAttacksBackingField == null)
            {
                _horseAttacksBackingField = new PatternManager<AIState>(
                    new Tuple<AIState, float>(AIState.Horse_Ride_Backflip_Shot, 1.0f),
                    new Tuple<AIState, float>(AIState.Horse_Ride_Big_Bow_Shot, 1.0f));
            }
            return _horseAttacksBackingField;
        }
    }

    private PatternManager<AIState> _groundAttacksBackingField;
    private PatternManager<AIState> GroundAttacks
    {
        get
        {
            if (_groundAttacksBackingField == null)
            {
                _groundAttacksBackingField = new PatternManager<AIState>(
                    new Tuple<AIState, float>(AIState.Bow_Spin, 2.0f),
                    new Tuple<AIState, float>(AIState.Backflip_Bow_Rain, 1.0f),
                    new Tuple<AIState, float>(AIState.Grounded_Small_Shot, 2.0f));
            }

            return _groundAttacksBackingField;
        }
    }
    private Animator _animatorBackingField;
    private Animator Animator
    {
        get
        {
            if (_animatorBackingField == null)
            {
                _animatorBackingField = CreateAnimator();
                _animatorBackingField.PlayAnimation(ANIM_IDLE);
            }
            return _animatorBackingField;
        }
    }

    private int Backflip_Bow_Damage => 35;
    private int Big_Celestial_Bow_Damage => 40;
    private int Bow_Spin_Damage => 35;
    private int Bow_Rain_Damage => 25;
    public Animator CreateAnimator()
    {
        Animator animator = new Animator();
        Vector2 drawOrigin = new Vector2(55, 123);
        var confused = new SpriteAnimation(0, 5, isLooping: true, drawOrigin);
        animator.AddAnimation(ANIM_CONFUSED, confused);

        var idle = new SpriteAnimation(0, 5, isLooping: true, drawOrigin);
        animator.AddAnimation(ANIM_IDLE, idle);

        var disappear = new SpriteAnimation(0, 8, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_DISAPPEAR, disappear);

        var throwBow = new SpriteAnimation(0, 6, isLooping: false, drawOrigin, frameSpeed: 0.25f);
        animator.AddAnimation(ANIM_THROWBOW, throwBow);

        var holdingBow = new SpriteAnimation(0, 0, isLooping: true, drawOrigin);
        animator.AddAnimation(ANIM_HOLDINGBOW, holdingBow);

        var bowOut = new SpriteAnimation(0, 5, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_BOWOUT, bowOut);

        var backflipReady = new SpriteAnimation(0, 5, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_BACKFLIPREADY, backflipReady);

        var backflip = new SpriteAnimation(0, 10, isLooping: false, drawOrigin, frameSpeed: 0.25f);
        animator.AddAnimation(ANIM_BACKFLIP, backflip);

        var airtime = new SpriteAnimation(0, 0, isLooping: true, drawOrigin);
        animator.AddAnimation(ANIM_AIRTIME, airtime);

        var landBackflip = new SpriteAnimation(0, 5, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_LANDBACKFLIP, landBackflip);
        return animator;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
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
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.TrailCacheLength[NPC.type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        _squishScale = Vector2.One;
        NPC.width = 32;
        NPC.height = 64;
        NPC.damage = 50;
        NPC.defense = 15;
        NPC.lifeMax = 12000;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0f;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        NPC.value = Item.buyPrice(gold: 12);
        NPC.npcSlots = 10f;
        NPC.scale = 1f;
        NPC.aiStyle = -1;
        
        // The following code assigns a music track to the boss in a simple way.
        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Moonwalker");
        }
    }
    private void FaceTarget()
    {
        NPC.spriteDirection = MyTarget.Center.X > NPC.Center.X ? -1 : 1;
        NPC.direction = MyTarget.Center.X > NPC.Center.X ? 1 : -1;
    }
    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            AttackCycle = 0;
            AttackCounter = 0;
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }
    public override void AI()
    {
        base.AI();
        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget && State != AIState.Despawn)
            {
                SwitchState(AIState.Despawn);
            }
        }

        if(_teleportPosition != Vector2.Zero)
        {
            NPC.Center = _teleportPosition;
            _teleportPosition = Vector2.Zero;
        }

        Lighting.AddLight(NPC.position, Color.LightGreen.ToVector3() * 0.78f);
        _contactDamage = false;
        _warning = false;
        _attacking = false;
        _showTrail = false;
        _show = true;
        _projectionFlicker = false;
        _showSlideTrail = false;
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
            case AIState.Bow_Spin:
                AI_BowSpin();
                break;
            case AIState.Backflip_Bow_Rain:
                AI_BowRain();
                break;
            case AIState.Grounded_Small_Shot:
                AI_GroundedSmallShot();
                break;
            case AIState.Horse_Ride_Backflip_Shot:
                AI_HorseRideBackflipShot();
                break;
            case AIState.Horse_Ride_Big_Bow_Shot:
                AI_HorseRideBackBigShot();
                break;
            case AIState.Dizzy:
                AI_Dizzy();
                break;
            case AIState.Projection_Dash:
                AI_ProjectionDash();
                break;
            case AIState.Punish_Snipe:
                AI_PunishSnipe();
                break;
        }

        float a = _projectionFlicker ? 1 : 0;
        _projectionAlpha = MathHelper.Lerp(_projectionAlpha, a, 0.1f);
        if (_show)
        {
            _alphaTimer++;
        }
        else
        {
            _alphaTimer--;
        }

        float fadeTime = 75;
        _alphaTimer = MathHelper.Clamp(_alphaTimer, 0f, fadeTime);

        _ghostAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(_alphaTimer / fadeTime));
        float targetTrailAlpha = _showTrail ? 1f : 0f;
        _trailAlpha = MathHelper.Lerp(_trailAlpha, targetTrailAlpha, 0.1f);


        float slideTrailAlpha = _showSlideTrail ? 1f : 0f;
        _slideTrailAlpha = MathHelper.Lerp(_slideTrailAlpha, slideTrailAlpha, 0.1f);



        Color targetOutlineColor = Color.Transparent;
        if (_attacking)
        {
            targetOutlineColor = Color.Red;
        } else if (_warning)
        {
            targetOutlineColor = Color.Yellow;
        }
        _outlineColor = Color.Lerp(_outlineColor, targetOutlineColor, 0.1f);
    }

    private void Warn()
    {
        SoundStyle warningSound = AssetRegistry.Sounds.Celestia.CelestiaAbouttoAttack;
        warningSound.PitchVariance = 0.3f;
        SoundEngine.PlaySound(warningSound, NPC.position)
;    }
    private void ProjectOut()
    {
        Timer++;
        _show = false;
        NPC.velocity.X += 0.1f * -NPC.direction;
        Animator.PlayAnimation(ANIM_DISAPPEAR);
        if (Animator.IsFinished())
        {
            Timer = 0;
            AttackCycle++;
        }

    }

    private void Teleport(Vector2 teleportPosition)
    {
        if (MultiplayerHelper.IsHost)
        {
            _teleportPosition = teleportPosition;
            NPC.netUpdate = true;
        }
    }

    private Vector2 Fall(Vector2 startPosition)
    {
        Point tile = startPosition.ToTileCoordinates();
        tile.Y -= 1;
        tile = TileUtilities.FallToSolidTile(tile);
        return tile.ToWorldCoordinates();
    }

    private float DirectionToTarget()
    {
        return MyTarget.Center.X > NPC.Center.X ? 1 : -1;
    }

    private void TeleportHorseBackStart()
    {

        //Get the starting position and teleport there
        if (MultiplayerHelper.IsHost)
        {
            Vector2 startFrom = MyTarget.Center;
            float dir = Main.rand.NextBool(2) ? 1 : -1;
         
            startFrom = Fall(startFrom);
            startFrom.X += 1024 * dir;
            startFrom.Y -= 100;
            Teleport(startFrom);
        }

    }

    private void AI_Dizzy()
    {

    }

    private void AI_ProjectionDash()
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

                    _projectionFlicker = true;
                    FaceTarget();
                
                    Animator.PlayAnimation(ANIM_THROWBOW);
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
                        Vector2 vel = Vector2.UnitX * DirectionToTarget();
                        float speed = MathF.Abs(MyTarget.Center.X - NPC.Center.X) / 16f;
                        NPC.velocity = vel * speed;
                    }


                    if (Timer % 4 == 0)
                    {
                        Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(48, 48);
                        var d = Dust.NewDustPerfect(pos, DustID.GemEmerald, Scale: 1f);
                        d.noGravity = true;
                    }

                    if (NPC.velocity.Length() > 15 && Timer % 2 == 0)
                    {
                        Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(48, 48);
                        Vector2 vel = NPC.velocity * 0.5f;
                        var fx = FXUtil.GlowStretch(pos, vel);
                        fx.OuterGlowColor = Color.Turquoise;
                        fx.Scale *= 0.5f;
                        CreateNewAfterImage();
                    }


                    NPC.velocity.X *= MathHelper.Lerp(0.96f, 0.92f, EasingFunction.InOutSine(Timer / 30f));
                    _showTrail = true;
                    Animator.PlayAnimation(ANIM_BOWOUT);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    ChooseAttack();
                }
                break;
        }
    }

    private void AI_PunishSnipe()
    {

    }

    private void CreateNewAfterImage()
    {
        if (Main.netMode == NetmodeID.Server)
            return;

        Vector2 afterImageVelocity = Vector2.Zero;
        string texture = Texture + "_"+Animator.GetAnimation();

        Vector2 drawOrigin = Animator.GetDrawOrigin().Value;
        float rotation = NPC.rotation;
        Rectangle frame = NPC.frame;
        SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        if (NPC.spriteDirection == -1)
            drawOrigin.X = NPC.frame.Size().X - drawOrigin.X;

        AfterImageRenderer.New(texture, frame, NPC.Bottom, afterImageVelocity, NPC.rotation, Vector2.One, drawOrigin, Color.White * 0.6f, spriteEffects);
    }

    private void AI_BowSpin()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 vel = Vector2.UnitX * DirectionToTarget();
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel,
                                ModContent.ProjectileType<CelestialBowSpin>(), Bow_Spin_Damage, 1, Main.myPlayer, ai1: NPC.whoAmI, ai2: 1);
                        }
                        NPC.TargetClosest();
                        Warn();
                    }

                    _warning = true;
                    FaceTarget();
                    Animator.PlayAnimation(ANIM_IDLE);
                    _squishScale = Vector2.Lerp(Vector2.One, new Vector2(1.2f, 0.9f), EasingFunction.InOutSine(Timer / 60f));
                    if(Timer >= 60)
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

                    _squishScale = Vector2.Lerp(new Vector2(1.2f, 0.9f), Vector2.One, EasingFunction.InSine(Timer / 30f));
                    _attacking = true;
                    _showSlideTrail = true;
                    NPC.velocity.X *= 0.98f;
                    if(Timer == 25)
                    {
                        NPC.velocity.X = DirectionToTarget() * 6;
                    }

                    if (Timer == 25 && MultiplayerHelper.IsHost)
                    {
                        Vector2 vel = Vector2.UnitX * DirectionToTarget();
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel,
                            ModContent.ProjectileType<CelestialBowSpin>(), Bow_Spin_Damage, 1, Main.myPlayer, ai1: NPC.whoAmI);
                    }
                   
                    FaceTarget();
                    Animator.PlayAnimation(ANIM_THROWBOW);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    _attacking = true;
                    _showSlideTrail = true;
                    NPC.velocity.X *= 0.98f;
                    if (Timer % 4 == 0)
                    {
                        Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(48, 48);
                        var d = Dust.NewDustPerfect(pos, DustID.GemEmerald, Scale: 1f);
                        d.noGravity = true;
                    }

                    if(NPC.velocity.Length() > 15 && Timer % 2 == 0)
                    {
                        Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(48, 48);
                        Vector2 vel = NPC.velocity * 0.5f;
                        var fx = FXUtil.GlowStretch(pos, vel);
                        fx.OuterGlowColor = Color.Turquoise;
                        fx.Scale *= 0.5f;
                        CreateNewAfterImage();
                    }


                    NPC.velocity.X *= MathHelper.Lerp(0.96f, 0.92f, EasingFunction.InOutSine(Timer/30f));
         
                    Animator.PlayAnimation(ANIM_HOLDINGBOW);
                    if (Timer >= 78)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    NPC.velocity.X *= 0.92f;
                    Animator.PlayAnimation(ANIM_BOWOUT);
                    if(Animator.IsFinished())
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }

    private void AI_BowRain()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    //Prepare the attack, basic telegraph
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                        Warn();
                    }

                    _warning = true;
                    Animator.PlayAnimation(ANIM_BACKFLIPREADY);
                    if (Timer >= 60)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    //Backflip jump
                    if (Timer == 1)
                    {
                        SoundStyle backflipSound = AssetRegistry.Sounds.Celestia.CelestiaBackflip with { PitchVariance = 0.3f };
                        SoundEngine.PlaySound(backflipSound, NPC.position);
                        NPC.velocity.X = -(DirectionToTarget() * 4);
                        NPC.velocity.Y = -15;
                    }
                    _squishScale = Vector2.Lerp(new Vector2(0.9f, 1.2f), Vector2.One, EasingFunction.InOutSine(Timer / 30f));
                    _showTrail = true;
                    _attacking = true;
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;

                    if (Timer == 1)
                    {
                     
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 vel = new Vector2();
                            vel.X = DirectionToTarget() * 5;
                            vel.Y -= 15;
                            var proj = Projectile.NewProjectileDirect(SourceFromThis, NPC.Center - Vector2.UnitY * 4, vel,
                                ModContent.ProjectileType<ArrowRainBow>(), Bow_Rain_Damage, 1, Main.myPlayer, ai1: MyTarget.whoAmI);
                            ArrowRainBow bow = proj.ModProjectile as ArrowRainBow;
                            bow.parentIndex = NPC.whoAmI;
                            bow.Projectile.netUpdate = true;
                        }
                    }

                    OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -100);
                    if (NPC.velocity.Y < 15)
                        NPC.velocity.Y += 0.25f;
                    NPC.velocity.X *= 0.99f;
                    Animator.PlayAnimation(ANIM_BACKFLIP);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    _squishScale = Vector2.Lerp(Vector2.One, new Vector2(0.9f, 1.2f), EasingFunction.InOutSine(Timer / 30f));
                    _showTrail = true;
                    NPC.noGravity = true;
                    NPC.noTileCollide = false;

                    if (NPC.velocity.Y < 15)
                        NPC.velocity.Y += 0.5f;
                    NPC.velocity.X *= 0.99f;

                    Animator.PlayAnimation(ANIM_AIRTIME);


                    if (IsGrounded())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    if (Timer == 1)
                    {
                        NPC.velocity.X += DirectionToTarget() * 7;
                    }
                    if (Timer % 6 == 0)
                    {
                        var sp = SparkleParticle.Spawn(NPC.Bottom + Main.rand.NextVector2Circular(12, 12), Vector2.Zero);
                        sp.outerColor = Color.Turquoise;
                        sp.Scale *= 0.5f;
                        sp.gravity = 0;
                        sp.fast = true;
                    }
                    _showSlideTrail = true;
                    _showTrail = true;
                    _squishScale = Vector2.Lerp(new Vector2(1.3f, 0.9f), Vector2.One, EasingFunction.InOutSine(Timer / 30f));
                    NPC.noGravity = false;
                    NPC.noTileCollide = false;
                    Animator.PlayAnimation(ANIM_LANDBACKFLIP);
                    NPC.velocity.X *= 0.98f;
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {

                    _showSlideTrail = true;
                    _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.2f);
                    FaceTarget();
                    NPC.noGravity = true;
                    NPC.noTileCollide = false;
                    if (Timer % 6 == 0)
                    {
                        var sp = SparkleParticle.Spawn(NPC.Bottom + Main.rand.NextVector2Circular(12, 12), Vector2.Zero);
                        sp.outerColor = Color.Turquoise;
                        sp.Scale *= 0.5f;
                        sp.gravity = 0;
                        sp.fast = true;
                    }
                    NPC.velocity.X *= 0.98f;
                    Animator.PlayAnimation(ANIM_IDLE);
                    if (Timer >= 45)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }

    private void AI_GroundedSmallShot()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                        Warn();
                    }
                    if(Timer == 3 && MultiplayerHelper.IsHost)
                    {
                        Vector2 vel = NPC.velocity;
                        vel.X += Main.rand.NextFloat(-4f, 4f);
                        var p = Projectile.NewProjectileDirect(SourceFromThis, NPC.Center - Vector2.UnitY * 4, vel,
                            ModContent.ProjectileType<CelestialBow>(), Backflip_Bow_Damage, 1, Main.myPlayer, ai1: MyTarget.whoAmI);
                        if(p.ModProjectile is  CelestialBow bow)
                        {
                            bow.style = 1;
                            bow.Projectile.netUpdate = true;
                        }
                    }
                    _warning = true;
                    Animator.PlayAnimation(ANIM_THROWBOW);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    _attacking = true;
                    Animator.PlayAnimation(ANIM_BOWOUT);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    SwitchState(AIState.Idle);
                }
                break;
        }
    }

    private void AI_HorseRideBackBigShot()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    ProjectOut();
                    if(Timer == 1)
                    {
                        Warn();
                    }
                }
                break;
            case 1:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        TeleportHorseBackStart();
                        if (MultiplayerHelper.IsHost)
                        {
                            _bowIndex = Projectile.NewProjectile(SourceFromThis, NPC.Center, Vector2.Zero, ModContent.ProjectileType<BigCelestialBow>(), 
                                Big_Celestial_Bow_Damage, 1, Main.myPlayer, ai1: NPC.whoAmI);
                        }
                    }


                    //Ride in from whatever sides
                    _warning = true;
                    FaceTarget();
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    NPC.velocity.Y = MathF.Sin(Timer * 0.5f) * 0.5f;

                    float dist = MathF.Abs(MyTarget.Center.X - NPC.Center.X);
                    float direction = DirectionToTarget();
                    float gallopSpeed = MathHelper.Lerp(5, 10, EasingFunction.Clamp(dist / 384f));
                    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, gallopSpeed * direction, 0.1f);
                    Animator.PlayAnimation(ANIM_HOLDINGBOW);

                    _showTrail = true;
                    if (dist <= 666 && Timer > 30)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    NPC.velocity.X *= 0.98f;
                    _attacking = true;
                    FaceTarget();
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    NPC.velocity.Y = MathF.Sin(Timer * 0.5f) * 0.5f;

                    if (MultiplayerHelper.IsHost)
                    {
                        Projectile p = Main.projectile[_bowIndex];
                        BigCelestialBow bigCelestialBow = p.ModProjectile as BigCelestialBow;
                        if (bigCelestialBow.ready)
                        {
                            Timer = 0;
                            AttackCycle++;
                            NPC.netUpdate = true;
                        }
            
                    }
  
                }
                break;
            case 3:
                {
                    _attacking = true;
                    FaceTarget();
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    NPC.velocity.Y = MathF.Sin(Timer * 0.5f) * 0.5f;

                    if (Timer == 12 && MultiplayerHelper.IsHost)
                    {
                        Main.projectile[_bowIndex].ai[2] = 1;
                        Main.projectile[_bowIndex].netUpdate = true;
                    }


                    Animator.PlayAnimation(ANIM_BOWOUT);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    NPC.velocity.Y = MathF.Sin(Timer * 0.5f) * 0.5f;
                    Animator.PlayAnimation(ANIM_BACKFLIPREADY);
                    if(Timer >= 60)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 5:
                {
                    //Gotta jump off
                    if (Timer == 1)
                    {
                        SoundStyle backflipSound = AssetRegistry.Sounds.Celestia.CelestiaBackflip with { PitchVariance = 0.3f };
                        SoundEngine.PlaySound(backflipSound, NPC.position);
                        NPC.velocity.X = -(MathF.Sign(NPC.velocity.X) * 8);
                        NPC.velocity.Y = -15;
                    }
                    _squishScale = Vector2.Lerp(new Vector2(0.9f, 1.2f), Vector2.One, EasingFunction.InOutSine(Timer / 30f));
                    _showTrail = true;
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    if (NPC.velocity.Y < 15)
                        NPC.velocity.Y += 0.5f;
                    NPC.velocity.X *= 0.94f;
                    Animator.PlayAnimation(ANIM_BACKFLIP);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 6:
                {
                    _squishScale = Vector2.Lerp(Vector2.One, new Vector2(0.9f, 1.2f), EasingFunction.InOutSine(Timer / 30f));
                    _showTrail = true;
                    NPC.noGravity = true;
                    NPC.noTileCollide = false;

                    if (NPC.velocity.Y < 15)
                        NPC.velocity.Y += 0.5f;
                    NPC.velocity.X *= 0.94f;

                    Animator.PlayAnimation(ANIM_AIRTIME);
                    if (IsGrounded())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 7:
                {
                    if (Timer == 1)
                    {
                        NPC.velocity.X += DirectionToTarget() * 7;
                    }
                    if (Timer % 6 == 0)
                    {
                        var sp = SparkleParticle.Spawn(NPC.Bottom + Main.rand.NextVector2Circular(12, 12), Vector2.Zero);
                        sp.outerColor = Color.Turquoise;
                        sp.Scale *= 0.5f;
                        sp.gravity = 0;
                        sp.fast = true;
                    }
                    _showSlideTrail = true;
                    _showTrail = true;
                    _squishScale = Vector2.Lerp(new Vector2(1.3f, 0.9f), Vector2.One, EasingFunction.InOutSine(Timer / 30f));
                    NPC.noGravity = false;
                    NPC.noTileCollide = false;
                    Animator.PlayAnimation(ANIM_LANDBACKFLIP);
                    NPC.velocity.X *= 0.98f;
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 8:
                {
                    _showSlideTrail = true;
                    _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.2f);
                    FaceTarget();
                    NPC.noGravity = true;
                    NPC.noTileCollide = false;
                    if (Timer % 6 == 0)
                    {
                        var sp = SparkleParticle.Spawn(NPC.Bottom + Main.rand.NextVector2Circular(12, 12), Vector2.Zero);
                        sp.outerColor = Color.Turquoise;
                        sp.Scale *= 0.5f;
                        sp.gravity = 0;
                        sp.fast = true;
                    }
                    NPC.velocity.X *= 0.98f;
                    Animator.PlayAnimation(ANIM_IDLE);
                    if (Timer >= 45)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }
    private bool IsGrounded()
    {
        Point solidTileBelow = NPC.Bottom.ToTileCoordinates();
        solidTileBelow.Y++;
        bool isGrounded = Main.tile[solidTileBelow].HasTile && Main.tileSolid[Main.tile[solidTileBelow].TileType];
        return isGrounded;
         
    }
    private void AI_HorseRideBackflipShot()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    ProjectOut();
                    if(Timer == 1)
                    {
                        Warn();
                    }
                }
                break;
            case 1:
                {
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                        TeleportHorseBackStart();
                    }

                    //Ride in from whatever sides
                    _warning = true;
                    FaceTarget();
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    NPC.velocity.Y = MathF.Sin(Timer * 0.5f) * 0.5f;

                    float dist = MathF.Abs(MyTarget.Center.X - NPC.Center.X);
                    float direction = DirectionToTarget();
                    float gallopSpeed = MathHelper.Lerp(5, 10, EasingFunction.Clamp(dist / 384f));
                    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, gallopSpeed * direction, 0.1f);
                    Animator.PlayAnimation(ANIM_BACKFLIPREADY);

                    _showTrail = true;
                    if (dist <= 384 && Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    if(Timer == 1)
                    {
                        SoundStyle backflipSound = AssetRegistry.Sounds.Celestia.CelestiaBackflip with { PitchVariance = 0.3f };
                        SoundEngine.PlaySound(backflipSound, NPC.position);
                        NPC.velocity.X = -(MathF.Sign(NPC.velocity.X) * 8);
                        NPC.velocity.Y = -15;
                    }
                    _squishScale = Vector2.Lerp(new Vector2(0.9f, 1.2f), Vector2.One, EasingFunction.InOutSine(Timer / 30f));
                    _showTrail = true;
                    _attacking = true;
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;

                    if(Timer == 25 || Timer == 35 || Timer == 42)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 vel = NPC.velocity;
                            vel.X += Main.rand.NextFloat(-2f, 2f);
                            Projectile.NewProjectile(SourceFromThis, NPC.Center, vel,
                                ModContent.ProjectileType<CelestialBow>(), Backflip_Bow_Damage, 1, Main.myPlayer, ai1: MyTarget.whoAmI);
                        }
                    }
                    
                    if(NPC.velocity.Y < 15)
                        NPC.velocity.Y += 0.5f;
                    NPC.velocity.X *= 0.96f;
                    Animator.PlayAnimation(ANIM_BACKFLIP);
                    if (Animator.IsFinished())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    _squishScale = Vector2.Lerp(Vector2.One, new Vector2(0.9f, 1.2f), EasingFunction.InOutSine(Timer / 30f));
                    _showTrail = true;
                    NPC.noGravity = true;
                    NPC.noTileCollide = false;

                    if (NPC.velocity.Y < 15)
                        NPC.velocity.Y += 0.5f;
                    NPC.velocity.X *= 0.94f;

                    Animator.PlayAnimation(ANIM_AIRTIME);
      
                    
                    if (IsGrounded())
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 4:
                {
                    if(Timer == 1)
                    {
                        NPC.velocity.X += DirectionToTarget() * 7;
                    }
                    if(Timer % 6 == 0)
                    {
                        var sp = SparkleParticle.Spawn(NPC.Bottom + Main.rand.NextVector2Circular(12, 12), Vector2.Zero);
                        sp.outerColor = Color.Turquoise;
                        sp.Scale *= 0.5f;
                        sp.gravity = 0;
                        sp.fast = true;
                    }
                    _showSlideTrail = true;
                    _showTrail = true;
                    _squishScale = Vector2.Lerp( new Vector2(1.3f, 0.9f), Vector2.One, EasingFunction.InOutSine(Timer / 30f));
                    NPC.noGravity = false;
                    NPC.noTileCollide = false;
                    Animator.PlayAnimation(ANIM_LANDBACKFLIP);
                    NPC.velocity.X *= 0.98f;
                    if (Animator.IsFinished() )
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 5:
                {
                    _showSlideTrail = true;
                    _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.2f);
                    FaceTarget();
                    NPC.noGravity = true;
                    NPC.noTileCollide = false;
                    if (Timer % 6 == 0)
                    {
                        var sp = SparkleParticle.Spawn(NPC.Bottom + Main.rand.NextVector2Circular(12, 12), Vector2.Zero);
                        sp.outerColor = Color.Turquoise;
                        sp.Scale *= 0.5f;
                        sp.gravity = 0;
                        sp.fast = true;
                    }
                    NPC.velocity.X *= 0.98f;
                    Animator.PlayAnimation(ANIM_IDLE);
                    if(Timer >= 45)
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
        ProjectOut();
        if (Timer >= 90)
        {
            NPC.active = false;
        }
    }

    private void AI_Spawn()
    {

        Timer++;
        if (Timer == 1)
        {
            ShowNamePlate();
        }

        if (Timer >= 120)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            AIState state;
            if(!_firstAttack || GroundAttacks.HasNothingLeft())
            {
                state = HorseAttacks.NextPattern();
                GroundAttacks.ResetToDefaultWeights();
                _firstAttack = true;
            }
            else
            {
                state = GroundAttacks.NextPattern();
            }

            SwitchState(state);
        }
        //SwitchState(AIState.Horse_Ride_Big_Bow_Shot);
    }
    private void AI_Idle()
    {
        _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.2f);
        NPC.velocity.X *= 0.98f;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        Timer++;
        FaceTarget();
        Animator.PlayAnimation(ANIM_IDLE);
        if(Timer >= 90)
        {
            ChooseAttack();
        }
    }

    private void AI_Death()
    {

    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        Animator.Update();
        NPC.frame.Y = Animator.GetFrameY(frameHeight);
    }
    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(0, 8, EasingFunction.QuadraticBump(ratio)) * _slideTrailAlpha;
    }
    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.Lerp(Color.White, Color.Turquoise, ExtraMath.Osc(0f, 1f, speed: 12)), Color.Transparent, ratio) * _slideTrailAlpha;
    }
    private void DrawTrail(GraphicsDevice gDevice)
    {
        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserTexture = TrailRegistry.CorkscrewTrail;
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.White;
        TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, GetTrailColor, GetTrailWidth, laserShader, new Vector2(NPC.Size.X * 0.5f, NPC.Size.Y));
    }
    private void DrawAfterImage(SpriteBatch spriteBatch)
    {
        SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
        spriteBatch.Restart(effect: whiteShader.Effect);

        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.texture = ModContent.Request<Texture2D>($"{Texture}_{Animator.GetAnimation()}").Value;
        drawer.sourceRect = NPC.frame;
        drawer.drawOrigin = Animator.GetDrawOrigin().Value;
        if (NPC.spriteDirection == -1)
            drawer.drawOrigin.X = NPC.frame.Size().X - drawer.drawOrigin.X;
        for (int i = 0; i < NPC.oldPos.Length; i++)
        {
            Vector2 pos = NPC.oldPos[i];
            Vector2 oldCenter = pos + new Vector2(NPC.Size.X * 0.5f, NPC.Size.Y);
            drawer.worldPosition = oldCenter;
            drawer.worldPosition.Y -= 2;
            drawer.color = Color.Lerp(Color.Lerp(Color.White, Color.Turquoise, 0.85f), Color.DarkGreen, i / (float)NPC.oldPos.Length);
            drawer.color *= MathHelper.Lerp(1f, 0f, i / (float)NPC.oldPos.Length);
            drawer.color *= _trailAlpha;
            drawer.color *= 0.35f;
            spriteBatch.Draw(drawer);

        }
        spriteBatch.RestartDefaults();
    }
    private void DrawSprite(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.texture = ModContent.Request<Texture2D>($"{Texture}_{Animator.GetAnimation()}").Value;
        drawer.sourceRect = NPC.frame;
        drawer.drawOrigin = Animator.GetDrawOrigin().Value;
        if (NPC.spriteDirection == -1)
            drawer.drawOrigin.X = NPC.frame.Size().X - drawer.drawOrigin.X;
        drawer.color = drawColor * _ghostAlpha;
        drawer.worldPosition =  NPC.Bottom + screenPos;
        drawer.worldPosition.Y -= 2;
        drawer.scale *= _squishScale;
        spriteBatch.Draw(drawer);

    }

    private void DrawPixelatedSprites(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare2, NPC.Bottom);
        sbDrawer.scale *= 0.12f * ExtraMath.Osc(0.9f, 1f, speed: 24);
        sbDrawer.color = Color.Turquoise * _slideTrailAlpha;
        sbDrawer.color.A = 0;
        Main.spriteBatch.Draw(sbDrawer);

        //     sbDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, NPC.Bottom);
        sbDrawer.scale *= 0.9f;
        sbDrawer.color = Color.White * _slideTrailAlpha;
        sbDrawer.color.A = 0;
        Main.spriteBatch.Draw(sbDrawer);
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        PixelationManager.QueuePrimitivesDrawAction(DrawTrail);
        DrawAfterImage(spriteBatch);
        DrawSprite(spriteBatch, Vector2.Zero, drawColor);
        PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedSprites);
        if (_projectionFlicker)
        {
            SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
            spriteBatch.Restart(effect: whiteShader.Effect);
            DrawSprite(spriteBatch, Vector2.Zero,
                Color.Lerp(Color.Turquoise, Color.DarkTurquoise, ExtraMath.Osc(0f, 1f, speed: 24)) * 0.5f * _projectionAlpha * ExtraMath.Osc(0f, 1f, speed: 24));
            spriteBatch.RestartDefaults();
        }
        return false;
    }

    public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
    {
        Vector2 v = Vector2.UnitX * 2;
        Vector2 h = Vector2.UnitY * 2;

        DrawSprite(spriteBatch, v, _outlineColor);
        DrawSprite(spriteBatch, -v, _outlineColor);
        DrawSprite(spriteBatch, h, _outlineColor);
        DrawSprite(spriteBatch, -h, _outlineColor);
    }
}
