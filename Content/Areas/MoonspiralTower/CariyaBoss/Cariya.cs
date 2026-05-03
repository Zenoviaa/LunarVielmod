using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Animations;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.EveroseVillage.CelestiaBoss;
using Stellamod.Content.Areas.MoonspiralTower.CariyaBoss.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MoonspiralTower.CariyaBoss;

public class Cariya : ScarletBoss
{
    private enum AIState
    {
        Spawn,
        Despawn,

        Idle,
        Death,

        //Phase 1 alternates comboing these attacks and dashing, mainly grounded, just react and don't panic
        Aura_Monster,
        Overhead_Slash,
        Long_Thrust,
        Uppercut,
        Shes_Right_Behind_Me_Isnt_She,


        //In Phase 2 she gains these two attacks and combos faster
        Sword_Fall,
        Moon_Fall,
    }

    private PatternManager<AIState> _patternManager;
    private PatternManager<AIState> PatternManager
    {
        get
        {
            if(_patternManager == null)
            {
                _patternManager = new PatternManager<AIState>(
                    new Tuple<AIState, float>(AIState.Overhead_Slash, 1.0f),
                    new Tuple<AIState, float>(AIState.Long_Thrust, 1.0f),
                    new Tuple<AIState, float>(AIState.Uppercut, 1.0f),
                    new Tuple<AIState, float>(AIState.Sword_Fall, 1.0f),
                    new Tuple<AIState,float>(AIState.Moon_Fall, 0.1f));
            }
            return _patternManager;
        }
    }
    private bool _warning;
    private bool _attacking;
    private bool _showTrail;
    private bool _showSlideTrail;
    private bool _showMagicCircle;
    private float _startY;
    private float _magicCircleAlpha;
    private float _slideTrailAlpha;
    private float _trailAlpha;
    private bool _show;
    private float _ghostAlpha;
    private bool _allowPhase2Attacks;
    private float _wingAlpha;
    private bool _phase2Effect;
    private bool _showWings;
    private Color _outlineColor;
    private Vector2 _teleportPosition;
    private Asset<Texture2D> _wingTextureAsset;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];

    private bool InPhase2 => NPC.life < NPC.lifeMax * 0.5f;
    private int Overhead_Slash_Damage => 35;
    private int Sword_Fall_Damage => 50;
    private int Magic_Blade_Damage => 25;
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }
    private void CreateNewAfterImage()
    {
        if (Main.netMode == NetmodeID.Server)
            return;

        Vector2 afterImageVelocity = Vector2.Zero;
        string texture = Texture + "_" + Animator.GetAnimation();

        Vector2 drawOrigin = Animator.GetDrawOrigin().Value;
        float rotation = NPC.rotation;
        Rectangle frame = NPC.frame;
        SpriteEffects spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        if (NPC.spriteDirection == -1)
            drawOrigin.X = NPC.frame.Size().X - drawOrigin.X;

        AfterImageRenderer.New(texture, frame, NPC.Bottom, afterImageVelocity, NPC.rotation, Vector2.One, drawOrigin, Color.White * 0.6f, spriteEffects);
    }

    private float DirectionToTarget()
    {
        return (MyTarget.Center.X > NPC.Center.X) ? 1 : -1;
    }

    private int SpriteDirection()
    {
        return (int)DirectionToTarget();
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
        NPC.width = 32;
        NPC.height = 64;
        NPC.damage = 50;
        NPC.defense = 15;
        NPC.lifeMax = 7000;
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
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Cariya");
        }
    }

    public override void AI()
    {
        base.AI();
        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget && State != AIState.Despawn)
                SwitchState(AIState.Despawn);
        }
        if(_teleportPosition != Vector2.Zero)
        {
            NPC.Center = _teleportPosition;
            _teleportPosition = Vector2.Zero;
        }
        _showTrail = false;
        _showSlideTrail = false;
        _warning = false;
        _attacking = false;
        _showWings = false;
        _showMagicCircle = false;
        _show = true;
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
            case AIState.Aura_Monster:
                AI_AuraMonster();
                break;
            case AIState.Shes_Right_Behind_Me_Isnt_She:
                AI_ShesRightBehindMe();
                break;
            case AIState.Overhead_Slash:
                AI_OverheadSlash();
                break;
            case AIState.Long_Thrust:
                AI_LongSlash();
                break;
            case AIState.Uppercut:
                AI_UpSlash();
                break;
            case AIState.Sword_Fall:
                AI_SwordFall();
                break;
            case AIState.Moon_Fall:
                AI_MoonFall();
                break;
        }



        if (_allowPhase2Attacks)
        {
            _showWings = true;
            if(Timer % 12 == 0)
            {
                var p = LegacyParticle.NewParticle<EmberParticle>(NPC.Center + Main.rand.NextVector2Circular(64, 64), -Vector2.UnitY);
                p.outerColor = Color.White;
                p.innerColor = Color.White;
                p.fadeToColor = Color.DarkBlue;
                p.isLong = true;
            }
        }

        float targetWingAlpha = _showWings ? 1f : 0f;
        _wingAlpha = MathHelper.Lerp(_wingAlpha, targetWingAlpha, 0.1f);

        if (Timer % 7 == 0)
        {
            Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(96, 96);
            var d = Dust.NewDustPerfect(pos, DustID.GemSapphire, Scale: 1f);
            d.noGravity = true;
        }

        float targetShowAlpha = _show ? 1f : 0f;
        _ghostAlpha = MathHelper.Lerp(_ghostAlpha, targetShowAlpha, 0.1f);

        float targetMagicAlpha = _showMagicCircle ? 1f : 0f;
        _magicCircleAlpha = MathHelper.Lerp(_magicCircleAlpha, targetMagicAlpha, 0.1f);

        float targetTrailAlpha = _showTrail ? 1f : 0f;
        _trailAlpha = MathHelper.Lerp(_slideTrailAlpha, targetTrailAlpha, 0.1f);


        float targetAlpha = _showSlideTrail ? 1f : 0f;
        _slideTrailAlpha = MathHelper.Lerp(_slideTrailAlpha, targetAlpha, 0.1f);

        Color targetOutlineColor = Color.Transparent;
        if (_attacking)
        {
            targetOutlineColor = Color.Red;
        }
        else if (_warning)
        {
            targetOutlineColor = Color.Yellow;
        }
        _outlineColor = Color.Lerp(_outlineColor, targetOutlineColor, 0.1f);
    }
    private bool IsGrounded()
    {
        Point solidTileBelow = NPC.Bottom.ToTileCoordinates();
        solidTileBelow.Y++;
        bool tileSolid = Main.tileSolid[Main.tile[solidTileBelow].TileType] || Main.tileSolidTop[Main.tile[solidTileBelow].TileType];
        bool isGrounded = Main.tile[solidTileBelow].HasTile && tileSolid;
        return isGrounded;

    }
    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            AttackCounter = 0;
            AttackCycle = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    private void AI_Idle()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
        }

        NPC.velocity.X *= 0.94f;
        Animator.PlayAnimation(ANIM_IDLE);
        if (Timer >= 120)
        {
            SwitchState(AIState.Aura_Monster);
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
    private void AI_MoonFall()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    NPC.noGravity = true;
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        FaceTarget();
                        NPC.velocity.X = NPC.spriteDirection * 15;
                    }
                    _show = false;
                    _warning = true;
                    _showSlideTrail = true;
                    Animator.PlayAnimation(ANIM_DASH);
                    if (Timer > 15)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    _allowPhase2Attacks = true;
                    if(Timer == 1)
                    {
    
                        Vector2 teleportSpot = MyTarget.Center;
                        teleportSpot.Y -= 144;
                        teleportSpot.X += -NPC.spriteDirection * 512;
                        Teleport(teleportSpot);
                    }

                    if (Timer == 4)
                    {
                        FaceTarget();
                        _startY = NPC.position.Y;
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

                    _showTrail = true;
                    _showSlideTrail = true;
                    NPC.velocity.X *= 0.98f;
                    NPC.velocity.Y = 0;
                    if ( Timer % 5 == 0)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 spawnPos = NPC.Center;
                            spawnPos -= new Vector2(64);
                            spawnPos += Main.rand.NextVector2Circular(64, 64);
                            Vector2 velocity = new Vector2(15 * NPC.spriteDirection, 15);
                            Projectile.NewProjectile(SourceFromThis, spawnPos, velocity, ModContent.ProjectileType<CariyaMagicBlade>(), Magic_Blade_Damage, 1, Main.myPlayer);

                        }
                    }
                    NPC.noGravity = true;
                    _warning = true;
                    _showSlideTrail = true;
                    Animator.PlayAnimation(ANIM_DASH);
                    if (Animator.IsTimerFinished(Timer))
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    if (Timer % 5 == 0 && Timer < 30)
                    {
                        if (MultiplayerHelper.IsHost)
                        {
                            Vector2 spawnPos = NPC.Center;
                            spawnPos -= new Vector2(64);
                            spawnPos += Main.rand.NextVector2Circular(64, 64);
                            Vector2 velocity = new Vector2(15 * NPC.spriteDirection, 15);
                            Projectile.NewProjectile(SourceFromThis, spawnPos, velocity, ModContent.ProjectileType<CariyaMagicBlade>(), Magic_Blade_Damage, 1, Main.myPlayer);

                        }
                    }
                    _showMagicCircle = true;
                    _warning = true;
                    _showSlideTrail = true;
                    NPC.noGravity = true;
                    NPC.velocity.X *= 0.94f;
                    Animator.PlayAnimation(ANIM_MOONFALLREADY);

                    if(Timer >= 60)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
              
                    Vector2 velocity = Vector2.Lerp(new Vector2(-4 * NPC.spriteDirection, -4), new Vector2(16 * NPC.spriteDirection, 16), EasingFunction.InOutSine(Timer / 30f));
                    NPC.velocity = velocity;
                    if (Timer % 5 == 0)
                    {
                        var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Bottom, -NPC.velocity);
                        p2.Scale *= 0.5f;
                    }
                    _showSlideTrail = true;
                    _attacking = true;
                    NPC.noGravity = true;
                    if (Timer > 4 && IsGrounded())
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
                        FXUtil.ShakeCamera(NPC.Center, 1024, 32);
                    }
                    if (Timer == 1 && MultiplayerHelper.IsHost)
                    {
                        Projectile.NewProjectile(SourceFromThis, NPC.Bottom, -Vector2.UnitY, ModContent.ProjectileType<CariyaSpear>(), Sword_Fall_Damage, 1, Main.myPlayer);
                        Projectile.NewProjectile(SourceFromThis, NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<CariyaSwordFall>(), Sword_Fall_Damage, 1, Main.myPlayer);
                    }
                    Animator.PlayAnimation(ANIM_SWORDSTUCK);
                    NPC.velocity.X *= 0f;
                    NPC.noGravity = false;
                    if(Timer >= 90)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }

    private void AI_SwordFall()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                        FaceTarget();
                        NPC.velocity.Y -= 13;
                        NPC.velocity.X = NPC.spriteDirection * 6;
                    }
                    _warning = true;
                    _showSlideTrail = true;
                    Animator.PlayAnimation(ANIM_DASH);

                    if (Animator.IsTimerFinished(Timer))
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    _showMagicCircle = true;
                    if(Timer == 1)
                    {
                        PixelPrimitiveCircleFactory.CreateCariyaInMoon(NPC.Center);
                    }

                    if(Timer % 5 == 0)
                    {
                        Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(128, 128);
                        Vector2 vel = (NPC.Center - pos);
                        vel *= 0.1f;
                        var fx = FXUtil.GlowStretch(pos, vel);
                        fx.VectorScale *= 0.5f;
                    }

                    NPC.velocity.X *= 0.94f;
                    NPC.velocity.Y *= 0.5f;
                    Animator.PlayAnimation(ANIM_SWORDFALLREADY);

                    _warning = true;
                    _showSlideTrail = true;
                    if (Timer >= 60)
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
                        NPC.velocity.Y -= 10;
                    }
                    _attacking = true;
                    NPC.noGravity = true;
                    if (NPC.velocity.Y < 15)
                        NPC.velocity.Y += 0.85f;
                    else
                        NPC.velocity.Y *= 1.01f;

                    if (Timer % 5 == 0)
                    {
                        var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Bottom, -NPC.velocity);
                        p2.Scale *= 0.5f;
                    }
                    Animator.PlayAnimation(ANIM_SWORDDROP);
                    if (Timer > 15 && NPC.collideY)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    if(Timer == 1 && MultiplayerHelper.IsHost)
                    {
                        Projectile.NewProjectile(SourceFromThis, NPC.Bottom, -Vector2.UnitY, ModContent.ProjectileType<CariyaSpear>(), Sword_Fall_Damage, 1, Main.myPlayer);
                        Projectile.NewProjectile(SourceFromThis, NPC.Bottom, Vector2.Zero, ModContent.ProjectileType<CariyaSwordFall>(), Sword_Fall_Damage, 1, Main.myPlayer);
                    }

                    NPC.noGravity = false;
                    NPC.velocity.X *= 0.8f;
                    NPC.velocity.Y = 0;
                    Animator.PlayAnimation(ANIM_SWORDSTUCK);
                    if(Timer >= 60)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }
    private void AI_UpSlash()
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
                    _warning = true;
                    FaceTarget();
                    NPC.velocity.X *= 0.7f;
                    Animator.PlayAnimation(ANIM_UPPERCUTREADY);
                    if (Animator.IsTimerFinished(Timer))
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
                        NPC.velocity.X = NPC.spriteDirection * 3;
                        NPC.velocity.Y -= 7;
                    }
                    if (Timer == 8 && MultiplayerHelper.IsHost)
                    {
                        Vector2 fireVelocity = Vector2.UnitX * NPC.spriteDirection;
                        fireVelocity *= 5;
                        fireVelocity.Y -= 10;
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, fireVelocity, ModContent.ProjectileType<CariyaUppercut>(), Overhead_Slash_Damage, 1, Main.myPlayer);
                    }
                    _showTrail = true;
                    _attacking = true;
                    NPC.velocity.X *= 0.7f;
                    Animator.PlayAnimation(ANIM_UPPERCUT);
                    if (Animator.IsTimerFinished(Timer))
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    if (Timer == 8 && MultiplayerHelper.IsHost)
                    {
                        Vector2 fireVelocity = Vector2.UnitX * NPC.spriteDirection;
                        fireVelocity *= 15;
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, fireVelocity, ModContent.ProjectileType<CariyaFlyingSlash>(), Overhead_Slash_Damage, 1, Main.myPlayer);
                    }
                    _attacking = true;
                    NPC.velocity.X *= 0.7f;
                    Animator.PlayAnimation(ANIM_OVERHEAD_SWING);
                    if (Animator.IsTimerFinished(Timer))
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 3:
                {
                    SwitchState(AIState.Aura_Monster);
                }
                break;
        }

    }
    private void AI_LongSlash()
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
                    _warning = true;
                    FaceTarget();
                    NPC.velocity.X *= 0.7f;
                    Animator.PlayAnimation(ANIM_SWORDREADYLONG);
                    if (Animator.IsTimerFinished(Timer))
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if (Timer == 8 && MultiplayerHelper.IsHost)
                    {
                        Vector2 fireVelocity = Vector2.UnitX * NPC.spriteDirection;
                        fireVelocity *= 15;
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, fireVelocity, ModContent.ProjectileType<CariyaThrust>(), Overhead_Slash_Damage, 1, Main.myPlayer);
                    }
                    _attacking = true;
                    NPC.velocity.X *= 0.7f;
                    Animator.PlayAnimation(ANIM_LONGSWING);
                    if (Animator.IsTimerFinished(Timer))
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    SwitchState(AIState.Aura_Monster);
                }
                break;
        }

    }
    private void AI_OverheadSlash()
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
                    _warning = true;
                    if(Timer < 10)
                    {
                        FaceTarget();
                    }
              
                    NPC.velocity.X *= 0.7f;
                    Animator.PlayAnimation(ANIM_SWORD_READY_OVERHEAD);
                    if (Timer >= 50)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 1:
                {
                    if(Timer == 8 && MultiplayerHelper.IsHost)
                    {
                        Vector2 fireVelocity = Vector2.UnitX * NPC.spriteDirection;
                        fireVelocity *= 15;
                        Projectile.NewProjectile(SourceFromThis, NPC.Center, fireVelocity, 
                            ModContent.ProjectileType<CariyaFlyingSlash>(), Overhead_Slash_Damage, 1, Main.myPlayer);
                    }
                    _attacking = true;
                    NPC.velocity.X *= 0.7f;
                    Animator.PlayAnimation(ANIM_OVERHEAD_SWING);
                    if (Animator.IsTimerFinished(Timer))
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;
            case 2:
                {
                    SwitchState(AIState.Long_Thrust);
                }
                break;
        }
    }
    private void AI_ShesRightBehindMe()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                    }

                    Animator.PlayAnimation(ANIM_DASH);

                    if (Timer == 1)
                    {
                        _startY = NPC.position.Y;
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

                    _showTrail = true;
                    _showSlideTrail = true;
                    NPC.position.Y = _startY;
                    NPC.velocity.X *= MathHelper.Lerp(0.98f, 0.95f, EasingFunction.InOutSine(Timer / 30f));
                    NPC.velocity.Y = 0;
                    if (Animator.IsTimerFinished(Timer))
                    {
                        ChooseAttack();
                    }
                }
                break;
        }
    }
    private void FaceTarget()
    {
        NPC.spriteDirection = SpriteDirection();
    }
    private void AI_AuraMonster()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
            if(InPhase2 && MultiplayerHelper.IsHost)
            {
                Vector2 spawnPOs = NPC.Center - Vector2.UnitY * 100;
                Vector2 spawnVelocity = (MyTarget.Center - spawnPOs);
                spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero);
                spawnVelocity *= 15;
                Projectile.NewProjectile(SourceFromThis, spawnPOs, spawnVelocity, 
                    ModContent.ProjectileType<CariyaMagicBlade>(), Magic_Blade_Damage, 1, Main.myPlayer);
            }
        }

        float direction = DirectionToTarget();
        float walkingSpeed = 0.8f;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, walkingSpeed * direction, 0.1f);
        FaceTarget();
        Animator.PlayAnimation(ANIM_WALK);
        if (Timer >= 120)
        {
           SwitchState(AIState.Shes_Right_Behind_Me_Isnt_She);
        }
    }

    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            if(!_allowPhase2Attacks && InPhase2)
            {
                SwitchState(AIState.Moon_Fall);
            }
            else
            {
                AIState pattern = PatternManager.NextPattern();
                while(!InPhase2 && (pattern == AIState.Moon_Fall || pattern == AIState.Sword_Fall))
                {
                    pattern = PatternManager.NextPattern();
                }

                SwitchState(pattern);
            }    
        }
    }

    private void AI_Spawn()
    {
        Timer++;
        if (Timer == 1)
        {
            ShowNamePlate();
        }

        if (Timer >= 60)
        {
            SwitchState(AIState.Idle);
        }
    }
    private void AI_Despawn()
    {
        Timer++;
        if (Timer >= 90)
        {
            NPC.active = false;
        }
    }

    private const string ANIM_IDLE = "Idle";
    private const string ANIM_WALK = "Walk";
    private const string ANIM_SWORD_READY_OVERHEAD = "SwordReadyOverhead";
    private const string ANIM_OVERHEAD_SWING = "OverheadSwing";
    private const string ANIM_SWORDREADYLONG = "SwordReadyLong";
    private const string ANIM_LONGSWING = "LongSwing";
    private const string ANIM_UPPERCUTREADY = "UppercutReady";
    private const string ANIM_UPPERCUT = "Uppercut";
    private const string ANIM_DASH = "Dash";
    private const string ANIM_SWORDFALLREADY = "SwordFallReady";
    private const string ANIM_SWORDDROP = "SwordDrop";
    private const string ANIM_SWORDSTUCK = "SwordStuck";
    private const string ANIM_MOONFALLREADY = "MoonFallReady";
    private const string ANIM_MOONFALL = "MoonFall";

    private Animator _animator;
    private Animator Animator
    {
        get
        {
            if (_animator == null)
            {
                _animator = CreateAnimator();
                _animator.PlayAnimation(ANIM_IDLE);
            }
            return _animator;
        }
    }
    public Animator CreateAnimator()
    {
        Animator animator = new Animator();
        Vector2 drawOrigin = new Vector2(99, 140);

        var idleAnimation = new SpriteAnimation(0, 3, isLooping: true, drawOrigin);
        animator.AddAnimation(ANIM_IDLE, idleAnimation);

        var walkAnimation = new SpriteAnimation(0, 7, isLooping: true, drawOrigin);
        animator.AddAnimation(ANIM_WALK, walkAnimation);

        var swordReadyAnimation = new SpriteAnimation(0, 5, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_SWORD_READY_OVERHEAD, swordReadyAnimation);


        var overheadSwing = new SpriteAnimation(0, 5, isLooping: false, drawOrigin, frameSpeed: 0.25f);
        animator.AddAnimation(ANIM_OVERHEAD_SWING, overheadSwing);


        var swordReadyLunge = new SpriteAnimation(0, 4, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_SWORDREADYLONG, swordReadyLunge);

        var longSwing = new SpriteAnimation(0, 5, isLooping: false, drawOrigin, frameSpeed: 0.25f);
        animator.AddAnimation(ANIM_LONGSWING, longSwing);

        var uppercutReady = new SpriteAnimation(0, 3, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_UPPERCUTREADY, uppercutReady);

        var uppercut = new SpriteAnimation(0, 4, isLooping: false, drawOrigin, frameSpeed: 0.25f);
        animator.AddAnimation(ANIM_UPPERCUT, uppercut);

        var dash = new SpriteAnimation(0, 2, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_DASH, dash);

        var swordfall = new SpriteAnimation(0, 2, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_SWORDFALLREADY, swordfall);

        var swordDrop = new SpriteAnimation(0, 1, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_SWORDDROP, swordDrop);


        var swordStuck = new SpriteAnimation(0, 0, isLooping: true, drawOrigin);
        animator.AddAnimation(ANIM_SWORDSTUCK, swordStuck);

        var moonFallReady = new SpriteAnimation(0, 2, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_MOONFALLREADY, moonFallReady);

        var moonFall = new SpriteAnimation(0, 0, isLooping: false, drawOrigin);
        animator.AddAnimation(ANIM_MOONFALL, moonFall);
        return animator;
    }
    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        Animator.Update();
        NPC.frame.Y = Animator.GetFrameY(frameHeight);
    }

    private void DrawSprite(SpriteBatch spriteBatch, Vector2 drawOffset, Color drawColor)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.texture = ModContent.Request<Texture2D>($"{Texture}_{Animator.GetAnimation()}").Value;
        drawer.sourceRect = NPC.frame;
        drawer.drawOrigin = Animator.GetDrawOrigin().Value;
        if (NPC.spriteDirection == -1)
            drawer.drawOrigin.X = NPC.frame.Size().X - drawer.drawOrigin.X;
        drawer.color = drawColor * _ghostAlpha;
        drawer.worldPosition = NPC.Bottom + drawOffset;
        spriteBatch.Draw(drawer);
    }
    private void DrawWhite(SpriteBatch spriteBatch)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.texture = ModContent.Request<Texture2D>($"{Texture}_{Animator.GetAnimation()}").Value;
        drawer.sourceRect = NPC.frame;
        drawer.drawOrigin = Animator.GetDrawOrigin().Value;
        if (NPC.spriteDirection == -1)
            drawer.drawOrigin.X = NPC.frame.Size().X - drawer.drawOrigin.X;
        drawer.color = _outlineColor * _ghostAlpha;
        drawer.worldPosition = NPC.Bottom;
        spriteBatch.Draw(drawer);

    }
    private float GetTrailWidth(float ratio)
    {
        return MathHelper.SmoothStep(0, 16, EasingFunction.QuadraticBump(ratio)) * _slideTrailAlpha * _ghostAlpha;
    }
    private Color GetTrailColor(float ratio)
    {
        return Color.Lerp(Color.Lerp(Color.White, Color.Blue, ExtraMath.Osc(0f, 1f, speed: 12)), Color.Transparent, ratio) * _slideTrailAlpha * _ghostAlpha;
    }
    private void DrawTrail(GraphicsDevice gDevice)
    {
        RichLaserShader laserShader = RichLaserShader.Instance;
        laserShader.LaserTexture = TrailRegistry.CorkscrewTrail;
        laserShader.InnerColor = Color.White;
        laserShader.OuterColor = Color.White;
        TrailDrawer.Draw(Main.spriteBatch, NPC.oldPos, GetTrailColor, GetTrailWidth, laserShader, new Vector2(NPC.Size.X * 0.5f, NPC.Size.Y));
    }
    private void DrawWing(SpriteBatch spriteBatch)
    {
        SpritebatchDrawer wingDrawer = SpritebatchDrawer.FromTextureAsset(_wingTextureAsset, NPC.Center);
        wingDrawer.RightCenterOrigin();
        wingDrawer.drawOrigin.X += 16;
        wingDrawer.drawOrigin.Y += 32;
        wingDrawer.color = Color.White * ExtraMath.Osc(0.5f, 1f);
        wingDrawer.color.A = 0;
        wingDrawer.spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        if (NPC.spriteDirection == -1)
            wingDrawer.drawOrigin.X = (_wingTextureAsset.Width() - wingDrawer.drawOrigin.X);
        wingDrawer.scale.X *= MathHelper.Lerp(0.8f, 1f, ExtraMath.Osc(0f, 1f, speed: 1));
        wingDrawer.rotation = MathHelper.Lerp(0, MathHelper.ToRadians(18), ExtraMath.Osc(0f, 1f, speed: 1));
        GlowingSwordMaskShader shader = GlowingSwordMaskShader.Instance;
        shader.TrailTexture = TrailRegistry.BulbTrail;
        shader.Distortion = 0.02f;
        shader.DistortionTexture = TrailRegistry.WhispyTrail;
        shader.Time = Main.GlobalTimeWrappedHourly * 16;
        shader.Bloom = 1.4f;
        shader.Tiling = Vector2.One * 0.75f;
        shader.InnerColor = Color.Lerp(Color.LightBlue, Color.Lerp(Color.LightBlue, Color.Blue, 0.4f), ExtraMath.Osc(0f, 1f, 12)) * 0.5f * _ghostAlpha * _wingAlpha;
        shader.OuterColor = Color.DarkBlue * 0.5f * _ghostAlpha * _wingAlpha;
        spriteBatch.Restart(effect: shader.Effect);
        spriteBatch.Draw(wingDrawer);
        wingDrawer.scale *= 1.2f;
        wingDrawer.color *= 0.3f;
        spriteBatch.Draw(wingDrawer);
        spriteBatch.RestartDefaults();
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
        
            drawer.color = Color.Lerp(Color.LightBlue, Color.DarkBlue, i / (float)NPC.oldPos.Length);
            drawer.color *= MathHelper.Lerp(1f, 0f, i / (float)NPC.oldPos.Length);
            drawer.color *= _trailAlpha * _ghostAlpha;
            drawer.color *= 0.75f;
            spriteBatch.Draw(drawer);

        }
        spriteBatch.RestartDefaults();
    }
    private void DrawSpellCircle(SpriteBatch sb)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.MagicCircle2, NPC.Center);
        drawer.color = Color.Lerp(Color.LightBlue, Color.DarkBlue, ExtraMath.Osc(0f, 1f, speed: 4)) * _magicCircleAlpha;
        drawer.color.A = 0;
        drawer.rotation = Main.GlobalTimeWrappedHourly;
        drawer.scale *= 0.8f;
        drawer.scale *= MathHelper.Lerp(1.5f, 1f, _magicCircleAlpha);
        sb.Draw(drawer);
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (!Animator.GetDrawOrigin().HasValue)
            return false;
        _wingTextureAsset ??= ModContent.Request<Texture2D>(Texture + "_Wing");
        PixelationManager.QueuePrimitivesDrawAction(DrawTrail);
        SpritebatchDrawer auraDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.SimpleGlowCircle, NPC.Center);
        auraDrawer.scale *= 0.2f;
        auraDrawer.color = Color.Blue * ExtraMath.Osc(0.5f, 1f) * _ghostAlpha;
        auraDrawer.color.A = 0;
        spriteBatch.Draw(auraDrawer);
        DrawSpellCircle(spriteBatch);
        DrawAfterImage(spriteBatch);
        DrawWing(spriteBatch);
        DrawSprite(spriteBatch, Vector2.Zero, drawColor);
        OutlineRenderer.Queue(DrawWhite);
        return false;
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
