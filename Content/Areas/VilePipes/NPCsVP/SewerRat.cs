using Stellamod.Common;
using Stellamod.Core.Astar;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.VilePipes.NPCsVP;

public class SewerRat : ModNPC,
    IDrawToRenderTarget
{
    enum AIState : byte
    {
        Idle,
        Patrol,
        Chase,
        Backstep,
        JumpReady,
        Jump,
        Land,
        Bite,
    }
    Pathfinder _pathfinder;
    Outliner _outliner;
    Vector2 _jumpDirection;
    bool _contactDamage;
    bool _groundedLastFrame;
    float _squishTimer;
    float _targetAFAlpha;
    float _afAlpha;
    float SqrMaxFriendDistance => 192 * 192;
    float SqrHerdDistance => 384 * 384;
    float SqrSightRange => 192 * 192;
    float SqrAttackRange => 96 * 96;
    float XLevelDistance => 64;

    float IdleTime => 45;
    float PatrolTime => 60;
    float RunSpeed => 4;
    float MaxJumpSpeed => 7;
    float BackStepTime => 35;
    float LandTime => 35;
    float SqrDistanceToPlayer => Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);



    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    public override string Texture => TextureRegistry.EmptyTexture;
    const string ANIM_IDLE = "Idle";
    const string ANIM_RUN = "Run";
    const string ANIM_BACKSTEP = "Backstep";
    const string ANIM_JUMPREADY = "JumpReady";
    const string ANIM_JMPUP = "JumpUp";
    const string ANIM_LAND = "Land";
    const string ANIM_BITEUP = "BiteUp";
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        _pathfinder.NetSend(writer);
        writer.WriteVector2(_jumpDirection);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _pathfinder.NetReceive(reader);
        _jumpDirection = reader.ReadVector2();
    }
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 0;
        NPCID.Sets.UsesAseprite[Type] = true;
        this.AddToVilePipes();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        _pathfinder = new();
        NPC.damage = 10;
        NPC.width = 87;
        NPC.height = 38;
        NPC.lifeMax = 25;
        NPC.defense = 3;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.65f;
        NPC.noGravity = false;
    }
    public override void AI()
    {
        base.AI();
        _outliner.SetDefaults();
        _targetAFAlpha = 0;
        _contactDamage = false;
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
            case AIState.Backstep:
                AI_Backstep();
                break;
            case AIState.Jump:
                AI_Jump();
                break;
            case AIState.Land:
                AI_Land();
                break;
        }
        var grounded = MoonUtils.IsGrounded(NPC);
        if(!_groundedLastFrame && grounded)
        {
            _squishTimer = 25;
        }
        if(_squishTimer > 0)
        {
            _squishTimer--;
        }
       
        _groundedLastFrame = grounded;
        _afAlpha = MathHelper.Lerp(_afAlpha, _targetAFAlpha, 0.1f);
        _outliner.Update();
        NPC.DrawOrigin = new Vector2(48, 39);
        NPC.VectorScale = Vector2.Lerp(Vector2.One, new Vector2(1.1f, 0.9f), EasingFunction.InOutSine(_squishTimer / 25f));
    }

    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
    }

    void AI_Idle()
    {
        Timer++;
        NPC.TargetClosest(false);
        NPC.velocity.X *= 0.92f;
        NPC.PlayAnimationLooped(ANIM_IDLE);
        if(Timer >= IdleTime)
        {
            //I only want the sewer rats to initially aggro you
            //if your ground level is similar to them, so if you're flying they won't chase you
            var player = NPC.PlayerImTargeting;
            var diffX = MathF.Abs(NPC.Center.X - player.Center.X);
            if (diffX < XLevelDistance && SqrDistanceToPlayer < SqrSightRange)
            {
                SwitchState(AIState.Chase);
            }
            else
                SwitchState(AIState.Patrol);
        }
    }


    void AI_Patrol()
    {
        Timer++;
        if(MultiplayerHelper.IsHost && Main.rand.NextBool(128))
        {
            NPC.direction = Main.rand.NextBool(2) ? -1 : 1;
            NPC.netUpdate = true;
        }

        var closestFriend = NPC.ClosestFriend(NPC.type, SqrHerdDistance);
        if(closestFriend != null && Vector2.DistanceSquared(NPC.Center, closestFriend.Center) > SqrMaxFriendDistance)
        {
            var dirToFriend = (closestFriend.Center.X < NPC.Center.X) ? -1 : 1;
            NPC.direction = dirToFriend;
        }

        NPC.TargetClosest(false);
        NPC.QuickStepUp();
        NPC.StraightenRotationSlowly();
        NPC.FaceDirectionsToVelocity();
        NPC.PlayAnimationLooped(ANIM_RUN);
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 2, 0.1f);
        var player = NPC.PlayerImTargeting;
        var diffX = MathF.Abs(NPC.Center.X - player.Center.X);
        if (diffX < XLevelDistance && SqrDistanceToPlayer < SqrSightRange)
        {
            SwitchState(AIState.Chase);
        }
        else if(Timer >= PatrolTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    void AI_Chase()
    {
        Timer++;
        MoonUtils.CreateNewPathIfTooFar(_pathfinder, NPC.Center, NPC.PlayerImTargeting.Center);
        MoonUtils.AIWalk_SusBush(_pathfinder, NPC, NPC.PlayerImTargeting.Center, MoonUtils.IsGrounded(NPC), RunSpeed, MaxJumpSpeed);
        NPC.StraightenRotationSlowly();
        NPC.FaceDirectionsToVelocity();
        NPC.PlayAnimationLooped(ANIM_RUN);

        var targetingPlayer = NPC.PlayerImTargeting;
        var sqrDistanceToPlayer = Vector2.DistanceSquared(NPC.Center, targetingPlayer.Center);
        if (sqrDistanceToPlayer < SqrAttackRange)
        {
            SwitchState(AIState.Backstep);
        }
    }

    void AI_Backstep()
    {
        Timer++;
        if(Timer == 1)
        {
            NPC.velocity.X = -NPC.direction * 8;
        }
        _outliner.warning = true;
        NPC.velocity.X *= 0.9f;
        NPC.StraightenRotationSlowly();
        NPC.SpriteFaceTarget();
        NPC.PlayAnimationOneShot(ANIM_BACKSTEP);
        if(Timer >= BackStepTime)
        {
            SwitchState(AIState.Jump);
        }
    }

    void AI_Jump()
    {
        Timer++;
        if(Timer == 1)
        {
            NPC.velocity = _jumpDirection * 12;
        }
        NPC.velocity.X *= 0.92f;
        NPC.PlayAnimationOneShot(ANIM_JMPUP);

        _targetAFAlpha = 1f;
        _outliner.attacking = true;
        _contactDamage = true;
        if (MoonUtils.IsGrounded(NPC))
        {
            SwitchState(AIState.Land);
        }
    }

    void AI_Land()
    {
        Timer++;
        if(Timer == 1)
        {

        }
        NPC.PlayAnimationOneShot(ANIM_LAND);
        if(Timer >= LandTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;
        if (NPC.life > 0)
            return;

        this.MakeGore("_Gore_Head", NPC.position, NPC.velocity + new Vector2(0, -1));
        this.MakeGore("_Gore_Butt", NPC.position, NPC.velocity + new Vector2(-1, 0));
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        for(var i = 0; i < NPC.oldPos.Length; i++)
        {
            var pos = NPC.oldPos[i] + NPC.Size * 0.5f;
            var color = Color.Lerp(Color.Red, Color.Transparent, i / (float)NPC.oldPos.Length);
            NPC.DrawAnimator(spriteBatch, color, pos);
        }
        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
    }
    public override void OnKill()
    {
        base.OnKill();

    }

    public void DrawToRenderTargets()
    {
        NPC.DrawOutlineByAnimator(_outliner);
    }
}
