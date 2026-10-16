using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Core.Astar;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.VilePipes.NPCsVP;

public class ExplodingFlyBoom : ModProjectile
{
    ref float Timer => ref Projectile.ai[0];
    float ExplodeTime => 60;
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.light = 0.78f;
        Projectile.WidthAndHeight = 164;
        Projectile.timeLeft = (int)ExplodeTime;
        Projectile.hostile = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            FXUtil.ShakeCamera(Projectile.Center, 1024, 16);

            var sound = AssetReferences.Assets.Sounds.ExplosionGaseous.Asset with { PitchVariance = 0.6f };
            SoundEngine.PlaySound(sound, Projectile.position);

            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(Projectile.Center, TriColorPalette.Fiery, new Vector2(5, 15f));
            ParticleUtils.CreateSwirlingDustCircle(fac);

            var fx = FXUtil.GlowCircleBoom(Projectile.Center, Color.LightGreen, Color.DarkGreen, Color.Black, duration: 25, baseSize: 0.18f);
            fx.Scale *= 2.5f;
            foreach (PositionVelocity posVel in new CircleOrientation(Projectile.Center, 64, 48))
            {
                Particles.GlowDust.Spawn(new()
                {
                    position = posVel.position,
                    velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f) * 0.3f,
                    color = Color.Lerp(Color.Green, Color.DarkGreen, Main.rand.NextFloat()),
                    timeLeft = Main.rand.Next(45, 120),
                    scale = Main.rand.NextFloat(0.15f, 0.25f) * 2
                });
            }

            var color = Color.Lerp(Color.Green, Color.Black, 0.75f);
            foreach (PositionVelocity posVel in new CircleOrientation(Projectile.Center, 64, 32))
            {
                Particles.BackCloud.Spawn(new()
                {
                    position = posVel.position,
                    velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f) * 0.3f,
                    color = color,
                    timeLeft = Main.rand.Next(60, 120),
                    scale = Main.rand.NextFloat(0.5f, 1f)
                });
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        base.OnHitPlayer(target, info);
    }
}
public class ExplodingFly : ModNPC,
    IDrawToRenderTarget
{
    Vector2 _oldPlayerPos;
    Vector2 _home;
    Vector2 _posToMoveTo;
    Outliner _outliner;
    Pathfinder _pathfinder;
    enum AIState : byte
    {
        Idle,
        ChaseTarget,
        Move,
        Explode
    }
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float AttackCycle => ref NPC.ai[2];
    public override string Texture => TextureRegistry.EmptyTexture;
    const string ANIM_FLY = "Fly";
    const string ANIM_EXPLODING = "Exploding";
    float ExplodeTime => 36;
    float IdleTime => 90;
    float HomeRadius => 128;
    int Damage_Explode => 30;
    float MoveTime => 90;
    float ChaseDistance => 128 * 128;
    float ChaseSpeed => 5;
    float AttackDistance => 48 * 48;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.UsesAseprite[Type] = true;
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 0;
        this.AddToVilePipes();
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_posToMoveTo);
        writer.WriteVector2(_home);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _posToMoveTo = reader.ReadVector2();
        _home = reader.ReadVector2();
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _pathfinder = new();
        NPC.damage = 10;
        NPC.width = 80;
        NPC.height = 40;
        NPC.lifeMax = 50;
        NPC.defense = 7;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.65f;
        NPC.noGravity = true;
    }

    public override void AI()
    {
        base.AI();
        if (_home == default(Vector2))
        {
            _home = NPC.Center + new Vector2(0, -64);
            NPC.netUpdate = true;
        }

        _outliner.SetDefaults();
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.ChaseTarget:
                AI_ChaseTarget();
                break;
            case AIState.Move:
                AI_Move();
                break;
            case AIState.Explode:
                AI_Explode();
                break;
        }
        _outliner.Update();
        NPC.DrawOrigin = new Vector2(45, 38);
        NPC.VectorScale = Vector2.Lerp(new Vector2(1.05f, 0.95f), new Vector2(0.95f, 1.05f), ExtraMath.Osc(0f, 1f, speed: 2, NPC.whoAmI));

    }

    void FaceVelocity() => NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
        AttackCycle = 0;
    }

    void AI_Explode()
    {
        _outliner.warning = true;
        NPC.velocity *= 0.88f;
        Timer++;
        NPC.PlayAnimationOneShot(ANIM_EXPLODING);
        if (Timer % 2 == 0)
        {
            var pos = NPC.Center + Main.rand.NextVector2Circular(32, 32);
            var vel = (pos - NPC.Center).Resize(Main.rand.NextFloat(10f, 15f));
            Particles.GlowDust.Spawn(new()
            {
                position = pos,
                velocity = vel,
                color = Color.Lerp(Color.Lerp(Color.DarkGreen, Color.Green, 0.5f), Color.LightGreen, Main.rand.NextFloat()),
                scale = Main.rand.NextFloat(0.2f, 0.35f) * 0.85f,
                timeLeft = Main.rand.Next(45, 130)
            });
        }

        if (Timer >= ExplodeTime)
        {
            Explode();
        }
    }

    void Explode()
    {
        DeathGores();
        if (MultiplayerHelper.IsHost)
        {
            var firer = ProjFirer.From<ExplodingFlyBoom>(NPC);
            firer.damage = Damage_Explode;
            firer.New();
        }
        NPC.Kill();
    }

    void AI_Idle()
    {
        NPC.TargetClosest(false);
        Timer++;
        NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, MathF.Sin(Timer * 0.03f) * 1), 0.1f);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, ExtraMath.Osc(-0.05f, 0.05f, 2f, NPC.whoAmI), 0.05f);

        NPC.PlayAnimationLooped(ANIM_FLY);
        if (Timer >= IdleTime)
        {
            SwitchState(AIState.Move);
        }
        var sqrDistToTarget = Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
        if (sqrDistToTarget < ChaseDistance)
        {
            SwitchState(AIState.ChaseTarget);
        }
    }

    void AI_Move()
    {
        NPC.TargetClosest(false);
        Timer++;
        if (Timer == 1 && MultiplayerHelper.IsHost)
        {
            _posToMoveTo = _home + Main.rand.NextVector2Circular(HomeRadius, HomeRadius);
            NPC.netUpdate = true;
        }

        var targetVelocity = (_posToMoveTo - NPC.Center).Resize(2);
        NPC.velocity = NPC.velocity.MoveTowards(targetVelocity, 0.1f);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, ExtraMath.Osc(-0.05f, 0.05f, 2f, NPC.whoAmI), 0.05f);
        NPC.PlayAnimationLooped(ANIM_FLY);
        FaceVelocity();
        NPC.spriteDirection = NPC.direction;
        if (Timer >= MoveTime)
        {
            SwitchState(AIState.Idle);
        }

        var sqrDistToTarget = Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
        if (sqrDistToTarget < ChaseDistance)
        {
            SwitchState(AIState.ChaseTarget);
        }
    }


    void AI_ChaseTarget()
    {
        Timer++;
        MoonUtils.CreateNewPathIfTooFar(_pathfinder,
            currentPosition: NPC.Center,
            destination: NPC.PlayerImTargeting.Center, ref _oldPlayerPos);

        MoonUtils.AIWalk_FlyChase(_pathfinder, NPC, NPC.PlayerImTargeting.Center, ChaseSpeed);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
        NPC.PlayAnimationLooped(ANIM_FLY);
        FaceVelocity();
        NPC.spriteDirection = NPC.direction;
        var sqrDistToTarget = Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
        if (sqrDistToTarget < AttackDistance)
        {
            SwitchState(AIState.Explode);
        }
    }


    void DeathGores()
    {
        foreach (PositionVelocity posVel in new CircleOrientation(NPC.Center, 32, 16))
        {
            Particles.BackCloud.Spawn(new()
            {
                position = posVel.position,
                velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f),
                color = Color.DarkGreen,
                timeLeft = Main.rand.Next(45, 130)
            });
        }

        this.MakeGore("_Gore_Left", NPC.position, NPC.velocity + new Vector2(-1, 0));
        this.MakeGore("_Gore_Right", NPC.position, NPC.velocity + new Vector2(1, 0));
        this.MakeGore("_Gore_Bottom", NPC.position, NPC.velocity + new Vector2(0, 1));
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;

        if(NPC.life <= 0)
        {
            DeathGores();
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
    }

    public void DrawToRenderTargets()
    {
        NPC.DrawOutlineByAnimator(_outliner);
    }
}
