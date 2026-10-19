using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Core.Astar;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.NPCsJN;

public class DungeonSkeletonBone : ModProjectile,
    IDrawToRenderTarget
{
    public override string Texture => AssetReferences.Assets.Textures.Projectile.DungeonSkeletonBone.KEY;
    ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(16);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hostile = true;
        Projectile.timeLeft = 120;
        Projectile.light = 0.4f;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer >= 15)
        {
            Projectile.tileCollide = true;
        }
        if (Main.rand.NextBool(6))
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(16, 16);
            var vel = Main.rand.NextVector2Circular(4, 4);
            Dust.NewDustPerfect(pos, DustID.Bone, vel);
        }
        Projectile.velocity.Y += 0.3f;
        Projectile.rotation += MathF.Sign(Projectile.velocity.X) * Projectile.velocity.Length() * 0.05f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.Red, Color.Transparent, 0.2f);
        Main.spriteBatch.Draw(Projectile.Drawer);
        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for (var i = 0; i < 24; i++)
        {
            var pos = Projectile.Center + Main.rand.NextVector2Circular(16, 16);
            var vel = Main.rand.NextVector2Circular(4, 4);
            Dust.NewDustPerfect(pos, DustID.Bone, vel);
        }
    }

    public void DrawToRenderTargets()
    {
        void DrawOutline(SpriteBatch sb)
        {
            var drawer = Projectile.Drawer;
            drawer.color = Color.Red;
            Main.spriteBatch.Draw(drawer);
        }
        OutlineRenderer.Queue(DrawOutline);
    }
}
public class DungeonSkeleton : ModNPC,
    IDrawToRenderTarget
{
    int _boneBarrage;
    float _targetAFAlpha;
    float _afAlpha;
    enum AIState : byte
    {
        Idle,
        Chase,
        ChaseFast,
        Patrol,
        Jump,
        Throw,
        ThrowReady,
        JumpMelee,
        Land,
    }
    bool _groundedLastFrame;
    float _squishTimer;
    Pathfinder _pathfinder;
    Outliner _outliner;
    bool _contactDamage;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float AttackCycle => ref NPC.ai[2];
    float SqrDistToPlayer => Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
    float IdleTime => 35;
    float RunSpeed => 4;
    float ThrowTime => 45;
    float ThrowReadyTime => 25;
    float MaxJumpSpeed => 6;
    float SqrChaseDistance => 256 * 256;
    float SqrThrowDistance => 384 * 384;
    float LandTime => 37;
    float RunFastSpeed => 7;
    float SqrJumpMeleeDistance => 144 * 144;
    int Damage_Bone => 20;
    const string ANIM_IDLE = "Idle";
    const string ANIM_THROWREADY = "ThrowReady";
    const string ANIM_THROW = "Throw";
    const string ANIM_WALKING = "Walkin";
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 0;
        NPCID.Sets.UsesAseprite[Type] = true;
        this.AddToMarsh();
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        _pathfinder.NetSend(writer);
        writer.Write(_boneBarrage);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _pathfinder.NetReceive(reader);
        _boneBarrage = reader.ReadInt32();
    }
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _pathfinder = new();
        NPC.width = 24;
        NPC.height = 47;
        NPC.lifeMax = 240;
        NPC.defense = 7;
        NPC.HitSound = SoundID.NPCHit12;
        NPC.DeathSound = SoundID.NPCDeath2;
        NPC.knockBackResist = 0.25f;
        NPC.noGravity = false;
    }

    public override void AI()
    {
        base.AI();
        _outliner.SetDefaults();
        NPC.noGravity = false;
        _targetAFAlpha = 0;
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Patrol:
                AI_Patrol();
                break;
            case AIState.Chase:
                AI_Chase();
                break;
            case AIState.ChaseFast:
                AI_ChaseFast();
                break;
            case AIState.JumpMelee:
                AI_JumpMelee();
                break;
            case AIState.Throw:
                AI_Throw();
                break;
            case AIState.ThrowReady:
                AI_ThrowReady();
                break;
            case AIState.Land:
                AI_Land();
                break;
        }
        _afAlpha = MathHelper.Lerp(_afAlpha, _targetAFAlpha, 0.1f);
        if (_squishTimer > 0)
        {
            NPC.VectorScale = Vector2.Lerp(Vector2.One, new Vector2(1.2f, 0.8f), EasingFunction.InOutSine(_squishTimer / 25f));
            _squishTimer--;
        }
        var isGrounded = MoonUtils.IsGrounded(NPC);
        if (!_groundedLastFrame && isGrounded)
        {
            _squishTimer = 25;
        }
        _groundedLastFrame = isGrounded;
        _outliner.Update();
        NPC.DrawOrigin = new Vector2(23, 47);
    }

    void AI_Idle()
    {
        Timer++;
        NPC.TargetClosest(false);
        NPC.PlayAnimationLooped(ANIM_IDLE);
        NPC.StraightenRotationSlowly();
        NPC.spriteDirection = NPC.direction;
        NPC.velocity.X *= 0.92f;
        if (Timer >= IdleTime)
        {
            SwitchState(AIState.Patrol);
        }
    }

    void AI_Patrol()
    {
        Timer++;
        if (Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                NPC.direction = Main.rand.NextBool(2) ? -1 : 1;
                NPC.netUpdate = true;
            }
        }
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction, 0.1f);
        NPC.StraightenRotationSlowly();
        NPC.spriteDirection = NPC.direction;
        NPC.PlayAnimationLooped(ANIM_WALKING);
        NPC.QuickStepUp();

        if (SqrDistToPlayer < SqrChaseDistance)
        {
            if (MultiplayerHelper.IsHost)
            {

                if (AttackCycle % 3 != 0)
                {
                    SwitchState(AIState.Chase);
                }
                else
                {
                    SwitchState(AIState.ChaseFast);
                }
                AttackCycle++;
                NPC.netUpdate = true;
            }
          
        }
    }
    void AI_Chase()
    {
        Timer++;
        if (Timer == 1)
        {

        }

        MoonUtils.CreateNewPathIfTooFar(_pathfinder, NPC.Center, NPC.PlayerImTargeting.Center);
        MoonUtils.AIWalk_SusBush(_pathfinder, NPC, NPC.PlayerImTargeting.Center, MoonUtils.IsGrounded(NPC), RunSpeed, MaxJumpSpeed);
        NPC.FaceDirectionsToVelocity();
        NPC.PlayAnimationLooped(ANIM_WALKING);
        NPC.StraightenRotationSlowly();
        if (SqrDistToPlayer < SqrThrowDistance)
        {
            SwitchState(AIState.ThrowReady);
        }
    }

    void AI_ChaseFast()
    {
        _targetAFAlpha = 1;
        Timer++;
        MoonUtils.CreateNewPathIfTooFar(_pathfinder, NPC.Center, NPC.PlayerImTargeting.Center);
        MoonUtils.AIWalk_SusBush(_pathfinder, NPC, NPC.PlayerImTargeting.Center, MoonUtils.IsGrounded(NPC), RunFastSpeed, MaxJumpSpeed);
        NPC.FaceDirectionsToVelocity();
        NPC.PlayAnimationLooped(ANIM_WALKING);
        NPC.GetAnimator().Update();
        NPC.StraightenRotationSlowly();
        if (SqrDistToPlayer < SqrJumpMeleeDistance && Timer > 30)
        {
            SwitchState(AIState.JumpMelee);
        }
    }

    void AI_JumpMelee()
    {
        _targetAFAlpha = 1;
        Timer++;
        if(Timer == 1)
        {
            SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
            bellHit.PitchVariance = 0.2f;
            bellHit.Pitch = -0.5f;
            SoundEngine.PlaySound(bellHit, NPC.position);


            var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Bottom, Vector2.UnitY);
            p2.Scale *= 0.5f;
            p2.fadeToColor = Color.Black;
            p2.innerColor = Color.White;
            p2.outerColor = Color.DarkGray;

            NPC.velocity.Y = -9;
          
            NPC.velocity.X += NPC.direction;
        }
        NPC.noGravity = true;
        NPC.velocity.X *= 0.98f;
        if(Timer < 30)
        {
            NPC.velocity.Y += 0.2f;
        }
        else
        {
            if (NPC.velocity.Y > 0)
                NPC.velocity.Y *= 1.06f;
            else
                NPC.velocity.Y += 0.6f;
        }

        if (Main.rand.NextBool(3))
        {
            var pos = NPC.Center;
            pos += Main.rand.NextVector2Circular(32, 32);
            var d = Dust.NewDustPerfect(pos, DustID.GemDiamond, Main.rand.NextVector2Circular(1, 1), Scale: Main.rand.NextFloat(0.6f, 1.2f));
            d.noGravity = true;
        }
        _boneBarrage = 2;
        NPC.rotation = Utils.AngleLerp(0.25f, -0.1f, EasingFunction.InSine(Timer / 30f));
        if(Timer >= 30 && MoonUtils.IsGrounded(NPC))
        {
            SwitchState(AIState.ThrowReady);
        }
    }

    void AI_Land()
    {
        Timer++;
        if(Timer == 1)
        {
            FXUtil.ShakeCamera(NPC.Center, 1024, 8);
            var impact = ParticleUtils.ParticleFactory.FromSmallBurst(NPC.Center, TriColorPalette.Silver, new(5, 15));
            impact.particleCount *= 2;
            ParticleUtils.CreateSwirlingDustCircle(impact);
            var fx = FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.Silver, Color.Black, 25, 0.16f);
            fx.Scale *= 1.5f;
            for (var i= 0; i < 8; i++)
            {
                var firer = ProjFirer.From<DungeonSkeletonBone>(NPC);
                firer.damage = Damage_Bone;
                firer.position += Main.rand.NextVector2Circular(32, 32);
                firer.velocity = Vector2.UnitX * NPC.direction;
                firer.velocity = firer.velocity.RotatedBy(-NPC.direction * 0.5f);
                firer.velocity *= 12;
                firer.velocity *= Main.rand.NextFloat(0.75f, 1.2f);
                firer.velocity = firer.velocity.RotatedByRandom(1.6f);
                firer.New();
            }
        }
        if(Timer >= LandTime)
        {
            SwitchState(AIState.Idle);
        }
    }
    void AI_ThrowReady()
    {
        Timer++;
        _outliner.warning = true;
        NPC.velocity.X *= 0.92f;
        NPC.PlayAnimationOneShot(ANIM_THROWREADY);
        NPC.SpriteFaceTarget();
        var t = ThrowReadyTime;
        if(_boneBarrage > 0)
        {
            t *= 0.5f;
        }
        if(Timer >= t)
        {
            SwitchState(AIState.Throw);
        }
    }


    void AI_Throw()
    {
        Timer++;
        if (Timer == 1)
        {
            var quickHit = AssetReferences.Assets.Sounds.QuickHit.Asset with { PitchVariance = 0.4f, Pitch = -0.4f };
            SoundEngine.PlaySound(quickHit, NPC.position);
            if (MultiplayerHelper.IsHost)
            {
                var firer = ProjFirer.From<DungeonSkeletonBone>(NPC);
                var xSpeed = MathF.Abs(NPC.PlayerImTargeting.Center.X - NPC.Center.X) / 64F;
                firer.velocity.X = xSpeed * NPC.direction * Main.rand.NextFloat(0.7f, 1f);
                if (_boneBarrage > 0)
                    firer.velocity *= 1.2f;
                firer.velocity.Y -= 10;
                firer.damage = Damage_Bone;
                firer.New();
            }
        }

        NPC.velocity.X *= 0.92f;
        NPC.PlayAnimationOneShot(ANIM_THROW);
        NPC.SpriteFaceTarget();
        var t = ThrowTime;
        if (_boneBarrage > 0)
            t *= 0.5f;
        if (Timer >= t)
        {
            if(_boneBarrage > 0)
            {
                _boneBarrage--;
                SwitchState(AIState.ThrowReady);
            }
            else
            {
                SwitchState(AIState.Idle);
            }
           
        }
    }

    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;
        if (NPC.life > 0)
            return;

        this.MakeGore("_Gore_Head", NPC.position, NPC.velocity + new Vector2(0, -1));
        this.MakeGore("_Gore_Body", NPC.position, NPC.velocity + new Vector2(0, 1));
        var color = Color.Lerp(Color.White, Color.Black, 0.75f);
        foreach (PositionVelocity posVel in new CircleOrientation(NPC.Center, 64, 7))
        {
            Particles.BackCloud.Spawn(new()
            {
                position = posVel.position,
                velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f) * 0.2f,
                color = color,
                timeLeft = Main.rand.Next(60, 120),
                scale = Main.rand.NextFloat(0.5f, 2f)
            });
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (_afAlpha > 0.04f)
        {
            for (var i = 0; i < NPC.oldPos.Length; i++)
            {
                var pos = NPC.oldPos[i] + NPC.Size * 0.5f;
                var color = Color.Lerp(Color.White, Color.Transparent, (float)i / (float)NPC.oldPos.Length) * 0.16f;
                color *= _afAlpha;
                color.A = 0;
                NPC.DrawAnimator(spriteBatch, color, pos);
            }
        }

        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
    }

    public void DrawToRenderTargets()
    {
        NPC.DrawOutlineByAnimator(_outliner);
    }
}
