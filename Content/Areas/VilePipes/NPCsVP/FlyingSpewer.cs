using Stellamod.Common;
using Stellamod.Core.Astar;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.VilePipes.NPCsVP;

public class FlyingSpewerBullet : ModProjectile,
    IDrawToRenderTarget
{
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => AssetReferences.Assets.Textures.Projectile.FlyingSpewerShot.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.SetTrailCacheLength(12);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.timeLeft = 180;
        Projectile.light = 0.6f;
        Projectile.hostile = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawUtilities.DrawSpriteAfterImage(Main.spriteBatch, Projectile, Color.Green, Color.Transparent, 0.17f);
        Main.spriteBatch.Draw(Projectile.Drawer);

        return false;
    }

    public void DrawToRenderTargets()
    {
 
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}

public class FlyingSpewer : ModNPC,
    IDrawToRenderTarget
{
    Vector2 _oldPlayerPos;
    Pathfinder _pathfinder;
    enum AIState : byte
    {
        Idle,
        Move,
        Chase,
        BlitzReady,
        Blitz
    }
    Vector2 _home;
    Vector2 _posToMoveTo;
    Vector2 _fireVelocity;
    Outliner _outliner;

    const string ANIM_IDLE = "Idle";
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float AttackCycle => ref NPC.ai[2];
    float IdleTime => 90;
    float ChaseRange => 512 * 512;
    float AttackDistance => 256 * 256;
    float HomeRange => 256;
    float ChaseSpeed => 8;
    float MoveTime => 100;
    float BlitzReadyTime => 60;
    float BlitzTime => 60;
    int Damage_SpewBullet => 35;
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_home);
        writer.WriteVector2(_posToMoveTo);
        writer.WriteVector2(_fireVelocity);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _home = reader.ReadVector2();
        _posToMoveTo = reader.ReadVector2();
        _fireVelocity = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.UsesAseprite[Type] = true;
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 0;
        this.AddToVilePipes();
    }
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
    {
        npcHitbox = npcHitbox.CenterPad(56);
        return base.ModifyCollisionData(victimHitbox, ref immunityCooldownSlot, ref damageMultiplier, ref npcHitbox);
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        _pathfinder = new();
        NPC.width = NPC.height = 8;
        NPC.lifeMax = 150;
        NPC.defense = 3;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.noGravity = true;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(_home == default(Vector2))
        {
            _home = NPC.Center;
            _posToMoveTo = _home;
            NPC.netUpdate = true;
        }
        _outliner.SetDefaults();
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Move:
                AI_Move();
                break;
            case AIState.Chase:
                AI_Chase();
                break;
            case AIState.BlitzReady:
                AI_BlitzReady();
                break;
            case AIState.Blitz:
                AI_Blitz();
                break;
        }
        _outliner.Update();
        NPC.PlayAnimationLooped(ANIM_IDLE);
        NPC.DrawOrigin = new Vector2(42, 47);
    }

    void AI_Idle()
    {
        NPC.TargetClosest();
        Timer++;
        NPC.velocity.X *= 0.88f;
        NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, MathF.Sin(Timer * 0.3f) * 2f, 0.1f);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0f, 0.1f);
        if(Timer >= IdleTime)
        {
            SwitchState(AIState.Move);
        }

        var sqrDistToPlayer = Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
        if(sqrDistToPlayer < ChaseRange)
        {
            SwitchState(AIState.Chase);
        }
    }

    void AI_Move()
    {
        Timer++;
        if(Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                _posToMoveTo = _home + Main.rand.NextVector2Circular(HomeRange, HomeRange);
                NPC.netUpdate = true;
            }
        }

        var targetVelocity = (_posToMoveTo - NPC.Center).Resize(2);
        NPC.velocity = NPC.velocity.MoveTowards(targetVelocity, 0.25f);
        NPC.velocity.Y += MathF.Sin(Timer * 0.03f) * 0.03f;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.2f);
        if (Timer >= MoveTime)
        {
            SwitchState(AIState.Idle);
        }

        var sqrDistToPlayer = Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
        if (sqrDistToPlayer < ChaseRange)
        {
            SwitchState(AIState.Chase);
        }
    }


    void AI_Chase()
    {
        Timer++;
        MoonUtils.CreateNewPathIfTooFar(_pathfinder,
            currentPosition: NPC.Center,
            destination: NPC.PlayerImTargeting.Center, ref _oldPlayerPos);

        MoonUtils.AIWalk_FlyChase(_pathfinder, NPC, NPC.PlayerImTargeting.Center, ChaseSpeed);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);

        var sqrDistToTarget = Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
        if (sqrDistToTarget < AttackDistance)
        {
            SwitchState(AIState.BlitzReady);
        }
    }

    void AI_BlitzReady()
    {
        _outliner.warning = true;
        Timer++;
        NPC.velocity *= 0.88f;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.01f);
        if (Timer >= BlitzReadyTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    void AI_Blitz()
    {
        _outliner.attacking = true;
        Timer++;
        if(Timer % 4 == 0)
        {
            if (MultiplayerHelper.IsHost)
            {
                var firer = ProjFirer.From<FlyingSpewerBullet>(NPC);
                firer.damage = Damage_SpewBullet;
                firer.velocity = _fireVelocity;
                firer.New();
            }
            NPC.velocity = -_fireVelocity.SafeNormalize(Vector2.Zero) * 1.5f;
        }
        NPC.velocity *= 0.88f;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.01f);
        if(Timer >= BlitzTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
        AttackCycle = 0;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
    }

    public void DrawToRenderTargets()
    {
        NPC.DrawOutlineByAnimator(_outliner);
    }
}
