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
    enum AIState : byte
    {
        Idle,
        Chase,
        Patrol,
        Jump,
        Throw
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
    float SqrDistToPlayer => Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
    float IdleTime => 35;
    float RunSpeed => 6;
    float ThrowTime => 45;
    float MaxJumpSpeed => 6;
    float SqrChaseDistance => 256 * 256;
    float SqrThrowDistance => 144 * 144;
    int Damage_Bone => 20;
    const string ANIM_IDLE = "Idle";
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
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _pathfinder.NetReceive(reader);
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
        NPC.lifeMax = 80;
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
            case AIState.Throw:
                AI_Throw();
                break;
        }

        if (_squishTimer > 0)
        {
            NPC.VectorScale = Vector2.Lerp(Vector2.One, new Vector2(1.1f, 0.9f), EasingFunction.InOutSine(_squishTimer / 25f));
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

        if (SqrDistToPlayer < SqrChaseDistance)
        {
            SwitchState(AIState.Chase);
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
        if (SqrDistToPlayer < SqrThrowDistance)
        {
            SwitchState(AIState.Throw);
        }
    }

    void AI_Throw()
    {
        Timer++;

        var throwFrame = 26;
        if (Timer < throwFrame)
        {
            _outliner.warning = true;
        }
        if (Timer == throwFrame)
        {
            if (MultiplayerHelper.IsHost)
            {
                var firer = ProjFirer.From<DungeonSkeletonBone>(NPC);
                firer.velocity = (NPC.PlayerImTargeting.Center - NPC.Center).Resize(12) * Main.rand.NextFloat(0.6f, 1f);
                firer.damage = Damage_Bone;
                firer.New();
            }
        }

        NPC.velocity.X *= 0.92f;
        NPC.PlayAnimationOneShot(ANIM_THROW);
        NPC.SpriteFaceTarget();
        if (Timer >= ThrowTime)
        {
            SwitchState(AIState.Idle);
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
        for (var i = 0; i < NPC.oldPos.Length; i++)
        {
            var pos = NPC.oldPos[i] + NPC.Size * 0.5f;
            var color = Color.Lerp(Color.Red, Color.Transparent, i / (float)NPC.oldPos.Length);
            NPC.DrawAnimator(spriteBatch, color, pos);
        }
        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
    }

    public void DrawToRenderTargets()
    {
        NPC.DrawOutlineByAnimator(_outliner);
    }
}
