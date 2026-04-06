using Stellamod.Common.Animations;
using Stellamod.Common.SummonerSystem;
using Stellamod.Content.Areas.Fable.WeaponsFB;
using Stellamod.Core.Bases;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.WeaponsUG;

public class MinersStorm : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToBellMinion(ModContent.ProjectileType<MinersStormSummon>(), isGuardian: true);
        Item.damage = 15;
    }
}
public class MinerBoom : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private bool IsSmall => Projectile.ai[1] == 1;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 6;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.friendly = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 6 * 5;
    }

    public override void AI()
    {
        base.AI();
      //  Main.projFrames[Type] = 6;
        Timer++;
        if(Timer == 1)
        {
            Projectile.scale = Main.rand.NextFloat(0.2f, 0.5f);
            if (IsSmall)
                Projectile.scale *= 0.5f;
            float numDust = 16;
            if (IsSmall)
            {
               var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.Goldenrod, Color.DarkGoldenrod);
                fx.Scale *= 0.5f;
                for(float f = 0; f < 4; f++)
                {
                    Vector2 velocity = Vector2.UnitX.RotateRandom(MathHelper.TwoPi);
                    velocity *= Main.rand.NextFloat(4, 7);
                    DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                    spawnParams.outerColor = Color.Goldenrod;
                    spawnParams.scaleRange *= 0.5f;
                    var d = DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
                    d.dampening = 0.1f;
                    d.gravity = 0;
                }
            }
            else
            {

                for (float f = 0; f < numDust; f++)
                {
                    Vector2 vel = -Vector2.UnitY;
                    vel = vel.RotatedBy((f / numDust) * MathHelper.TwoPi);
                    vel *= Main.rand.NextFloat(13, 15);

                    var sp = SparkleParticle.Spawn(Projectile.Center, vel, Scale: 0.25f);
                    sp.outerColor = Color.Yellow;
                    sp.noTileCollide = true;
                    sp.gravity = 0;
                    sp.dampening = 0.1f;

                    Vector2 vel2 = new Vector2();
                    vel2.X = MathF.Sin(f / numDust * MathHelper.TwoPi) * 64;
                    vel2.Y = MathF.Cos(f / numDust * MathHelper.TwoPi) * 32;
                    vel2 *= 0.1f;

                    var sp2 = SparkleParticle.Spawn(Projectile.Center, vel2, Scale: 0.25f);
                    sp2.outerColor = Color.Yellow;
                    sp2.noTileCollide = true;
                    sp2.gravity = 0;
                    sp2.dampening = 0.1f;
                }
            }

         
        }

        DrawHelper.AnimateTopToBottom(Projectile, 5);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        drawer.color.A = 0;
        Main.spriteBatch.Draw(drawer);
        Main.spriteBatch.Draw(drawer);
        return false;
        //return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class MinersStormRing : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    private bool Slow => Projectile.ai[1] == 1;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        
        Projectile.tileCollide = true;
        Projectile.timeLeft = 180;
        Projectile.extraUpdates = 1;
    }

    public override void AI()
    {
        base.AI();
        if (Slow)
        {
            Projectile.extraUpdates = 0;
        }

        Timer++;
        if(Timer % 6 == 0)
        {
            var sp2 = SparkleParticle.Spawn(Projectile.Center, Vector2.Zero, Scale: 0.25f);
            sp2.outerColor = Color.Yellow;
            sp2.noTileCollide = true;
            sp2.gravity = 0;
            sp2.dampening = 0.1f;
            sp2.fast = true;
        }

        Projectile.velocity.Y += 0.25f;
        Projectile.rotation = Projectile.velocity.ToRotation() + Timer * 0.05f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        for(int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 pos = Projectile.oldPos[i];
            Vector2 center = pos + Projectile.Size * 0.5f;
            float ratio = (float)i / (float)Projectile.oldPos.Length;
            Color afterImageColor = Color.Lerp(Color.White, Color.Transparent, ratio);
            afterImageColor *= 0.2f;
            SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
            drawer.color = afterImageColor;
            drawer.worldPosition = center;
            Main.spriteBatch.Draw(drawer);
        }
        SpritebatchDrawer drawer2 = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(drawer2);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 
                ModContent.ProjectileType<MinerBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: Projectile.ai[1]);
        }
    }
}

public class MinersStormSummon : AbstractBellSummon
{
    private ref float Timer => ref Projectile.ai[0];
    private enum AIState
    {
        //Movement States
        Idle,
        RingSnipe,
        RingToss
    }

    private AIState State
    {
        get => (AIState)Projectile.ai[1];
        set => Projectile.ai[1] = (float)value;
    }

    private ref float AttackCycle => ref Projectile.ai[2];

    private const string Anim_Idle = "idle";
    private const string Anim_Throw = "throw";
    private const string Anim_Hurt = "hurt";
    private Animator _animator;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 13;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        _animator = new Animator();
        var idle = new SpriteAnimation(0, 4, isLooping: true);
        _animator.AddAnimation(Anim_Idle, idle);

        var running = new SpriteAnimation(4, 11, isLooping: false);
        _animator.AddAnimation(Anim_Throw, running);

        var cannotComeOut = new SpriteAnimation(12, 12, isLooping: false);
        _animator.AddAnimation(Anim_Hurt, cannotComeOut);

        Projectile.width = 32;
        Projectile.height = 96;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 3600;
    }
    private void SwitchState(AIState state)
    {
        if (this.OwnedByLocalClient())
        {
            State = state;
            Timer = 0;
            Projectile.netUpdate = true;
        }
    }
    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.RingSnipe:
                AI_RingSnipe();
                break;
            case AIState.RingToss:
                AI_RingToss();
                break;

        }
        Projectile.rotation = Projectile.velocity.X * 0.05f;
        _animator.Update();
        Projectile.frame = _animator.GetFrame();
    }

    private void AI_Idle()
    {
        Timer++;
        _animator.PlayAnimation(Anim_Idle);
        Projectile.spriteDirection = Owner.direction;

        Vector2 targetPosition = Owner.Top + new Vector2(32 * -Owner.direction, -32);
        targetPosition += Vector2.UnitY * ExtraMath.Osc(0f, -16, speed: 2);
        Vector2 targetVelocity = (targetPosition - Projectile.Center) * 0.08f;

        float easing = EasingFunction.InOutSine(Timer / 120f);
        Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, easing);

        NPC target = NPCHelper.FindClosestNPC(Projectile.position, 1024);
        if (target != null && Timer > 60)
        {
            SwitchState(AIState.RingSnipe);
            //   Projectile.spriteDirection = target.Center.X < Projectile.Center.X ? 1 : -1;
        }
    }

    private void FollowTarget(NPC target)
    {
        if (target != null)
        {
            int direction = Projectile.Center.X < target.Center.X ? 1 : -1;
            float distance = 252;
            if(AttackCycle >= 3)
            {
                distance *= 1.5f;
            }
            Vector2 targetPosition = target.Top + new Vector2(distance * -direction, -32);
            targetPosition += Vector2.UnitY * ExtraMath.Osc(0f, -16, speed: 2);
            Vector2 targetVelocity = (targetPosition - Projectile.Center) * 0.08f;

            float easing = EasingFunction.InOutSine(Timer / 60f);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, easing);
            Projectile.spriteDirection = direction;
        }
    }
    private void AI_RingSnipe()
    {
        Timer++;
        _animator.PlayAnimation(Anim_Idle);


        NPC target = NPCHelper.FindClosestNPC(Projectile.position, 1024);
        if (target != null)
        {
            FollowTarget(target);
            float dist = Vector2.Distance(Projectile.Center, target.Center);
            if(dist < 384)
            {
                SwitchState(AIState.RingToss);
            }
        }
        else
        {
            SwitchState(AIState.Idle);
        }
        float distanceToOwner = Vector2.Distance(Projectile.Center, Owner.Center);
        if (distanceToOwner > 500)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void AI_RingToss()
    {
        Timer++;
        _animator.PlayAnimation(Anim_Throw);
        NPC target = NPCHelper.FindClosestNPC(Projectile.position, 1024);
        FollowTarget(target);
        if(Timer == 30)
        {
            AttackCycle++;
            if (this.OwnedByLocalClient())
            {
                if(AttackCycle >= 4)
                {
                    for(int i = 0; i < 7; i++)
                    {
                        Vector2 throwingVelocity = (target.Center - Projectile.Center);
                        throwingVelocity = throwingVelocity.SafeNormalize(Vector2.Zero);
                        throwingVelocity *= Main.rand.NextFloat(8, 12);
                        throwingVelocity = throwingVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                        throwingVelocity.Y -= 5;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, throwingVelocity,
                            ModContent.ProjectileType<MinersStormRing>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, ai1: 1);
                    }
                    AttackCycle = 0;
                }
                else
                {
                    Vector2 throwingVelocity = (target.Center - Projectile.Center);
                    throwingVelocity = throwingVelocity.SafeNormalize(Vector2.Zero);
                    throwingVelocity *= 15;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, throwingVelocity,
                        ModContent.ProjectileType<MinersStormRing>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }

            }
        }
        if (_animator.IsFinished() || target == null || Timer > 60)
        {
            SwitchState(AIState.RingSnipe);
        }
    }
}
