using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Jungle.SporedomBoss.Gores;
using Stellamod.Content.Areas.Jungle.SporedomBoss.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.SporedomBoss;

public class Sporedom : ScarletBoss,
    IDrawToRenderTarget
{
    private enum AIState
    {
        Spawn,
        Idle,
        Despawn,
        Death,
        BigSpit,
        BouncingBalls,
        SproutBoom
    }

    private bool _contactDamage;
    private Vector2 _squishScale;
    private Outliner _outliner;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];
    private float IdleTime => 249;
    private float BulbTime => 50;
    private float ShootTime => 30;
    private float BigPollenBulbTime => 90;
    private PatternManager<AIState> PatternManager
    {
        get
        {
            if (field == null)
            {
                field = new();
                field.AddPattern(AIState.BigSpit, 1);
                field.AddPattern(AIState.BouncingBalls, 1);
                field.AddPattern(AIState.SproutBoom, 1);
            }
            return field;
        }
    }
    private const string ANIM_IDLE = "Idle";
    private const string ANIM_BULB = "Bulb";
    private const string ANIM_SHOOT = "Shoot";
    private Color PollenLightColor => Color.Lerp(Color.Gold, Color.Black, 0.8f);
    private Color PollenDarkColor => Color.Lerp(Color.DarkGoldenrod, Color.Black, 0.8f);
    private int DamagePollenSpit => 20;
    private int DamageThornyBounceBall => 19;
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCSets.UseAseprite[Type] = true;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 128;
        NPC.height = 108;
        NPC.damage = 50;
        NPC.defense = 5;
        NPC.lifeMax = 2200;
        NPC.knockBackResist = 0f;

        NPC.boss = true;
        NPC.npcSlots = 30f;
        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/VisciousFoe");
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
    }

    public override BossLevel GetBossLevel()
    {
        return BossLevel.Miniboss;
    }
    public override bool AllowNameplateToBeShown()
    {
        return State != AIState.Despawn && State != AIState.Spawn && State != AIState.Death;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
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
        _outliner.SetDefaults();
        _contactDamage = false;
        _squishScale = Vector2.Lerp(_squishScale, Vector2.One, 0.1f);
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
            case AIState.BigSpit:
                AI_BigSpit();
                break;
            case AIState.BouncingBalls:
                AI_BouncingBalls();
                break;
            case AIState.SproutBoom:
                AI_SproutBoom();
                break;
        }
        if (_contactDamage)
            _outliner.attacking = true;
        _outliner.Update();
        this.SetDrawOrigin(new Vector2(60, 107));
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            AttackCycle = 0;
            AttackCounter = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    private void ChooseAttack()
    {
        var pattern = PatternManager.NextPattern();
        SwitchState(pattern);
    }

    private void AI_Spawn()
    {
        Timer++;
        if (Timer >= 60)
        {
            SwitchState(AIState.Idle);
        }
    }
    private void DeathEffect()
    {
        PixelPrimitiveCircleFactory.CreateGenericBoom(NPC.Center, Color.Gold, Color.DarkOrange, 25, 256);
        FXUtil.GlowCircleBoom(NPC.Center, Color.Gold, Color.Gold, Color.DarkOrange, duration: 0.23f, baseSize: 0.20f);
        for (var f = 0; f < 16; f++)
        {
            Particles.SwirlingFlameDust.Spawn(BitDustFactory.SlowingOverTime with
            {
                position = NPC.Center + Main.rand.NextVector2Circular(24, 24),
                velocity = Main.rand.NextVector2Circular(16, 16) * Main.rand.NextFloat(0.5f, 1f),
                innerColor = PollenLightColor.ToVector4(),
                outerColor = PollenDarkColor.ToVector4(),
                scale = new Vector2(Main.rand.NextFloat(0.5f, 3f)),
                timeLeft = 120
            });
        }
        var goreType = ModContent.GoreType<GreenFallenLeaf>();
        for (var f = 0; f < 12; f++)
        {
            var vel = Main.rand.NextVector2Circular(16, 16) * Main.rand.NextFloat(0.6f, 1f);
            var pos = NPC.Center + Main.rand.NextVector2Circular(32, 32);
            var gore = Gore.NewGore(pos, vel, goreType, Main.rand.NextFloat(0.7f, 1f));
        }
        goreType = ModContent.GoreType<WhiteFallenPetal>();
        for (var f = 0; f < 12; f++)
        {
            var vel = Main.rand.NextVector2Circular(16, 16) * Main.rand.NextFloat(0.6f, 1f);
            var pos = NPC.Center + Main.rand.NextVector2Circular(32, 32);
            var gore = Gore.NewGore(pos, vel, goreType, Main.rand.NextFloat(0.7f, 1f));
        }
        for (var f = 0; f < 32; f++)
        {
            var dp = DustParticle.Spawn(NPC.Center,
                Main.rand.NextVector2Circular(16, 16) * Main.rand.NextFloat(0.5f, 1f),
                DustParticleSpawnParams.Default with
                {
                    innerColor = PollenLightColor,
                    outerColor = PollenDarkColor,
                    gravity = 0.2f,
                    scaleRange = new Vector2(0.3f, 2f)
                });
            dp.dampening = 0.05f;
        }
        var explodeSound = AssetReferences.Assets.Sounds.WetDeath.Asset with { PitchVariance = 0.8f };
        SoundEngine.PlaySound(explodeSound, NPC.position);
    }
    private void AI_Death()
    {
        Timer++;
        CameraTargetSystem.AddTarget(NPC.Center);
        CameraTargetSystem.SetLingerTime(120);

        if (Timer % 4 == 0)
        {
            float range = Main.rand.NextFloat(128, 256);
            Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(range, range);
            Vector2 vel = (NPC.Center - pos);
            vel *= 0.1f;
            FXUtil.GlowStretch(pos, vel);
        }

        if (Timer % 4 == 0)
        {
            float range = Main.rand.NextFloat(252, 400);
            Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(range, range);
            Vector2 vel = (NPC.Center - pos);
            vel *= 0.1f;
            var fx = FXUtil.GlowStretch(pos, vel);
            fx.OuterGlowColor = Color.Lerp(Color.White, Color.Gold, Main.rand.NextFloat(0f, 1f));
            fx.VectorScale *= 0.5f;
        }

        if (Timer >= 120)
        {
            var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.Gold, Color.DarkOrange, duration: 45, baseSize: 0.24f);
            fx.Scale *= 1.5f;

            var fx2 = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.Gold, Color.DarkOrange, duration: 45, baseSize: 0.18f);
            fx2.Scale *= 1.5f;
            for (float f = 0; f < 32; f++)
            {
                var d = DustParticle.Spawn(NPC.Center, Main.rand.NextVector2Circular(24, 24));
                d.outerColor = Color.Black;
                d.innerColor = Color.Gold;
                d.Scale *= 2.1f;
                d.dampening = 0.05f;
                d.noTileCollide = true;
                d.gravity = 0;
            }
            DeathEffect();
            ShakeScreenPosition.Shake = 8;
            FXUtil.ShakeCamera(NPC.Center, 1024, 4);
            if (Main.netMode != NetmodeID.Server)
            {
                int headGore = Mod.Find<ModGore>($"{Name}_Gore_0").Type;
                int legGore = Mod.Find<ModGore>($"{Name}_Gore_1").Type;
                int legGore2 = Mod.Find<ModGore>($"{Name}_Gore_2").Type;
                int legGore3 = Mod.Find<ModGore>($"{Name}_Gore_3").Type;

                // Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore, 1f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(-16, 34), NPC.velocity, legGore);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(16, 34), NPC.velocity, legGore2);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, -8), NPC.velocity, legGore3);
            }
            SoundStyle roarSound = new SoundStyle("Stellamod/Assets/Sounds/SunStalker_Bomb_Explode") with { PitchVariance = 0.3f };
            SoundEngine.PlaySound(roarSound, MyTarget.Center);
            NPC.Kill();
        }

        if (Timer >= 180)
        {
            NPC.Kill();
        }
    }
    private void AI_Idle()
    {
        Timer++;
        if (Timer == 1)
        {
            NPC.TargetClosest();
        }
        this.AseAnimator.PlayAnimation(ANIM_IDLE, AnimationParams.Default);
        if (Timer >= IdleTime)
        {
            ChooseAttack();
        }
    }

    private void AI_Despawn()
    {
        Timer++;
        if (Timer >= 60)
            NPC.active = false;
    }

    private void SpitEffect()
    {
        var hitSound = AssetReferences.Assets.Sounds.Nature.PollenSpit.Asset with { PitchVariance = 1f };
        SoundEngine.PlaySound(hitSound, NPC.position);
        var spitPos = NPC.Center;
        spitPos.Y -= 32;
        for (var i = 0; i < 6; i++)
        {
            var sp = FaintSmokeParticle.Spawn(spitPos + new Vector2(0, -12), Main.rand.NextVector2Circular(10, 10));
            sp.behindLayer = true;
            sp.fadeToColor = Color.Black;
            sp.color = PollenDarkColor;

            sp.Scale *= Main.rand.NextFloat(0.5f, 1f);
            sp.Scale *= 0.3f;
            sp.dampening = 0.05f;
        }
        var fx = FXUtil.GlowCircleBoom(spitPos, Color.Gold, Color.DarkGoldenrod, Color.DarkOrange, duration: 30, baseSize: 0.2f);
        fx.Scale *= 0.66f;
        for(var i = 0; i < 6; i++)
        {
            var up = -Vector2.UnitY * 12;
            up = up.RotatedByRandom(0.6f);
            DustParticle.Spawn(spitPos,
                up * Main.rand.NextFloat(0.5f, 1f),
                DustParticleSpawnParams.Default with
                {
                    innerColor = PollenLightColor,
                    outerColor = PollenDarkColor,
                    gravity = 0.2f,
                    scaleRange = new Vector2(0.3f, 0.7f)
                });
        }
    }

    private void AI_BigSpit()
    {
        OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -64);
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _outliner.warning = true;
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                    }
                    NPC.velocity.X *= 0.96f;
                    _squishScale = Vector2.Lerp(_squishScale, new Vector2(1.1f, 0.9f), EasingFunction.OutSine(Timer / BulbTime));
                    this.AseAnimator.PlayAnimation(ANIM_BULB, AnimationParams.NoLooping);
                    if (Timer >= BulbTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                    if (AttackCounter > 0)
                    {
                        if (Timer >= BulbTime * 0.66f)
                        {
                            Timer = 0;
                            AttackCycle++;
                        }
                    }
                }

                break;

            case 1:
                {

                    if (Timer == 4)
                    {
                        SpitEffect();
                    }
                    _outliner.attacking = true;
                    if (Timer == 4 && MultiplayerHelper.IsHost)
                    {
                        var baseFirer = ProjFirer.From<PollenSpit>(NPC);
                        for (var i = 0; i < 8; i++)
                        {
                            var firer = baseFirer;
                            var dirToTarget = NPC.XDirectionToTarget;

                            firer.velocity = -Vector2.UnitY * 15;
                            firer.velocity = firer.velocity.RotatedByRandom(0.8f);
                            firer.velocity.X += dirToTarget * 5;
                            firer.position += new Vector2(Main.rand.NextFloat(-24, 24), Main.rand.NextFloat(-48, 0));
                            firer.damage = DamagePollenSpit;
                            firer.knockback = 1;
                            firer.New();
                        }
                        for (var i = 0; i < 5; i++)
                        {
                            var firer = baseFirer;
                            var dirToTarget = NPC.XDirectionToTarget;

                            firer.velocity = -Vector2.UnitY * 12;
                            firer.velocity = firer.velocity.RotatedByRandom(0.8f);
                            firer.velocity.X += dirToTarget * 3;
                            firer.position += new Vector2(Main.rand.NextFloat(-24, 24), Main.rand.NextFloat(-48, 0));
                            firer.damage = DamagePollenSpit;
                            firer.knockback = 1;
                            firer.New();
                        }
                    }
                    _squishScale = Vector2.Lerp(new Vector2(1f, 1.1f), Vector2.One, EasingFunction.InSine(Timer / BulbTime));
                    this.AseAnimator.PlayAnimation(ANIM_SHOOT, AnimationParams.NoLooping);
                    if (Timer >= ShootTime)
                    {
                        Timer = 0;
                        AttackCounter++;
                        if (AttackCounter >= 5)
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
                    this.AseAnimator.PlayAnimation(ANIM_IDLE, AnimationParams.NoLooping);
                    if (Timer >= 30)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }
    private void AI_BouncingBalls()
    {
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _outliner.warning = true;
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                    }
                    NPC.velocity.X *= 0.96f;
                    _squishScale = Vector2.Lerp(_squishScale, new Vector2(1.1f, 0.9f), EasingFunction.OutSine(Timer / BulbTime));
                    this.AseAnimator.PlayAnimation(ANIM_BULB, AnimationParams.NoLooping);
                    if (Timer >= BulbTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }

                break;

            case 1:
                {
                    if (Timer == 4 && MultiplayerHelper.IsHost)
                    {
                        var firer = ProjFirer.From<ThornyBounceBall>(NPC);
                        firer.velocity = -Vector2.UnitY * 15;
                        if (AttackCounter == 1)
                        {
                            firer.velocity = -Vector2.UnitY * 10;
                        }
                        firer.damage = DamageThornyBounceBall;
                        firer.knockback = 1;
                        firer.New();
                    }
                    _squishScale = Vector2.Lerp(new Vector2(1f, 1.1f), Vector2.One, EasingFunction.InSine(Timer / BulbTime));
                    this.AseAnimator.PlayAnimation(ANIM_SHOOT, AnimationParams.NoLooping);
                    if (Timer >= ShootTime)
                    {
                        Timer = 0;
                        AttackCounter++;
                        if (AttackCounter >= 2)
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
                    this.AseAnimator.PlayAnimation(ANIM_IDLE, AnimationParams.Default);
                    if (Timer >= 30)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }

    private bool IsThereASprout()
    {
        var type = ModContent.ProjectileType<CorePellet>();
        foreach(var proj in Main.ActiveProjectiles)
        {
            if (proj.type == type && proj.ai[0] == NPC.whoAmI)
                return true;
        }
        return false;
    }
    private void AI_SproutBoom()
    {
        OffsetCameraModifier.FocusTargetOffset = new Vector2(0, -64);
        Timer++;
        switch (AttackCycle)
        {
            case 0:
                {
                    _outliner.warning = true;
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                    }
                    NPC.velocity.X *= 0.96f;
                    _squishScale = Vector2.Lerp(_squishScale, new Vector2(1.1f, 0.9f), EasingFunction.OutSine(Timer / BigPollenBulbTime));
                    this.AseAnimator.PlayAnimation(ANIM_BULB, AnimationParams.NoLooping);
                    if (Timer >= BigPollenBulbTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }

                break;

            case 1:
                {
                    _outliner.attacking = true;
                    if (Timer == 4)
                    {
                        SpitEffect();
                    }
                    if (Timer == 4 && MultiplayerHelper.IsHost)
                    {
                        var firer = ProjFirer.From<CorePellet>(NPC);
                        firer.velocity = -Vector2.UnitY * 12;
                        firer.ai0 = NPC.whoAmI;
                        firer.damage = DamageThornyBounceBall;
                        firer.knockback = 1;
                        firer.New();
                    }
                    _squishScale = Vector2.Lerp(new Vector2(1f, 1.1f), Vector2.One, EasingFunction.InSine(Timer / BulbTime));
                    this.AseAnimator.PlayAnimation(ANIM_SHOOT, AnimationParams.NoLooping);
                    if (Timer >= ShootTime)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }
                break;

            case 2:
                {
                    _outliner.warning = true;
                    if (Timer == 1)
                    {
                        NPC.TargetClosest();
                    }
                    NPC.velocity.X *= 0.96f;
                    _squishScale = Vector2.Lerp(_squishScale, new Vector2(1.1f, 0.9f), EasingFunction.OutSine(Timer / BigPollenBulbTime));
                    this.AseAnimator.PlayAnimation(ANIM_BULB, AnimationParams.NoLooping);
                    if (Timer >= BulbTime * 0.66f)
                    {
                        Timer = 0;
                        AttackCycle++;
                    }
                }

                break;
            case 3:
                {
                    _outliner.attacking = true;
                    if (Timer == 4)
                    {
                        this.AseAnimator.PlayAnimation(ANIM_IDLE, AnimationParams.NoLooping);
                        SpitEffect();
                    }
                    if (Timer == 4 && MultiplayerHelper.IsHost)
                    {
                        var firer = ProjFirer.From<SmallPellet>(NPC);
                        var dirToTarget = NPC.XDirectionToTarget;
                        firer.ai0 = NPC.whoAmI;
                        firer.velocity = -Vector2.UnitY * 12;
                        firer.damage = DamagePollenSpit;
                        firer.knockback = 1;
                        firer.New();
                    }

                    _squishScale = Vector2.Lerp(new Vector2(1f, 1.1f), Vector2.One, EasingFunction.InSine(Timer / BulbTime));
                    this.AseAnimator.PlayAnimation(ANIM_SHOOT, AnimationParams.NoLooping);
                    if (Timer >= ShootTime)
                    {
                        Timer = 0;
                        AttackCounter++;
                        if (!IsThereASprout())
                        {
                            AttackCycle++;
                        }
                        else
                        {
                    
                        }
                    }
                }
                break;
            case 4:
                {
                    this.AseAnimator.PlayAnimation(ANIM_IDLE, AnimationParams.Default);
                    if (Timer >= 30)
                    {
                        SwitchState(AIState.Idle);
                    }
                }
                break;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
    }

    public void DrawToRenderTargets()
    {
        OutlineRenderer.Queue((SpriteBatch sb) => NPC.DrawAnimator(sb, _outliner.outlineColor));
    }

    public override void OnKill()
    {
        base.OnKill();
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (NPC.life <= 0)
        {
            NPC.life = 1;
        }
        if (NPC.life <= 1 && State != AIState.Death)
        {
            SwitchState(AIState.Death);
        }
    }


}

