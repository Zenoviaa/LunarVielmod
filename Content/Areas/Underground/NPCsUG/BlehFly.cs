using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Core.Astar;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.NPCsUG;

public class BlehFlySpit : ModProjectile,
    IDrawToRenderTarget
{
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => AssetReferences.Assets.Textures.Projectile.BlehFlySpit.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(12);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.hostile = true;
        Projectile.width = Projectile.height = 32;
        Projectile.light = 0.6f;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 200;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        Projectile.velocity.Y += 0.3F;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.DarkGreen, Color.Transparent, 0.15f);
        Main.spriteBatch.Draw(Projectile.Drawer);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public void DrawToRenderTargets()
    {
        void DrawOutline(SpriteBatch sb)
        {
            sb.Draw(Projectile.Drawer with { color = Color.Red });
        }
        OutlineRenderer.Queue(DrawOutline);
    }
}

public class BlehFly : ModNPC,
    IDrawToRenderTarget
{
    Pathfinder _pathfinder;
    enum AIState : byte
    {
        Idle,
        Move,
        ChaseTarget,
        SpitReady,
        Spit
    }
    Outliner _outliner;
    Vector2 _oldPlayerPos;
    Vector2 _home;
    Vector2 _posToMoveTo;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float AttackCycle => ref NPC.ai[2];
    public override string Texture => TextureRegistry.EmptyTexture;
    const string ANIM_FLY = "Fly";
    const string ANIM_FASTFLY = "FastFly";
    const string ANIM_SPIT = "Spit";
    const string ANIM_SIT = "Sit";
    float IdleTime => 129;
    float HomeRadius => 196 * 196;
    float ChaseDistance => 384 * 384;
    float AttackDistance => 192 * 192;
    float MoveTime => 90;
    float ChaseSpeed => 9;
    float SpitTime => 60;
    float SpitReadyTime => 60;
    int Damage_Spit => 25;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_home);
        writer.WriteVector2(_posToMoveTo);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _home = reader.ReadVector2();
        _posToMoveTo = reader.ReadVector2();
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 8;
        NPCID.Sets.UsesAseprite[Type] = true;
        this.AddToUnderground();
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
            case AIState.Spit:
                AI_Spit();
                break;
            case AIState.SpitReady:
                AI_SpitReady();
                break;
        }
        _outliner.Update();
        NPC.DrawOrigin = new Vector2(45, 38);
        NPC.spriteDirection = NPC.direction;
    }

    void FaceVelocity() => NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
        AttackCycle = 0;
    }

    void AI_Idle()
    {
        NPC.TargetClosest();
        Timer++;
        NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, MathF.Sin(Timer * 0.03f) * 1), 0.1f);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, ExtraMath.Osc(-0.05f, 0.05f, 2f, NPC.whoAmI), 0.05f);

        NPC.PlayAnimationLooped(ANIM_FLY);
        if (Timer >= IdleTime)
        {
            SwitchState(AIState.Move);
        }
    }

    void AI_Move()
    {
        NPC.TargetClosest();
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
        var sqrDistToTarget = Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
        if (sqrDistToTarget < AttackDistance)
        {
            SwitchState(AIState.SpitReady);
        }
    }

    void AI_Spit()
    {
        _outliner.attacking = true;
        Timer++;
        NPC.velocity *= 0.92f;
        NPC.PlayAnimationOneShot(ANIM_SPIT);
        if (Timer == 45 && MultiplayerHelper.IsHost)
        {
            var firer = ProjFirer.From<BlehFlySpit>(NPC);
            firer.damage = Damage_Spit;
            firer.velocity = (NPC.PlayerImTargeting.Center - NPC.Center).Resize(15);
            firer.velocity.Y -= 4;
            firer.New();
        }

        if (Timer >= SpitTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    void AI_SpitReady()
    {
        _outliner.warning = true;
        Timer++;
        NPC.velocity *= 0.92f;
        NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, MathF.Sin(Timer * 0.03f) * 1), 0.1f);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, ExtraMath.Osc(-0.05f, 0.05f, 2f, NPC.whoAmI), 0.05f);
        NPC.PlayAnimationLooped(ANIM_FASTFLY);
        if (Timer >= SpitReadyTime)
        {
            SwitchState(AIState.Spit);
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;

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

        this.MakeGore("_Gore_Butt", NPC.position, NPC.velocity + new Vector2(-5, 0));
        this.MakeGore("_Gore_Head", NPC.position, NPC.velocity + new Vector2(5, 0));
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
