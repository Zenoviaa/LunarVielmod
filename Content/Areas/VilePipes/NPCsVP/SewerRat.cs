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
    float _fakeOutCount;
    bool _fakedOut;
    Pathfinder _pathfinder;
    Outliner _outliner;
    Vector2 _jumpDirection;
    Vector2 _attackScale;
    float _idleTime;
    bool _contactDamage;
    bool _groundedLastFrame;
    float _squishTimer;
    float _targetAFAlpha;
    float _afAlpha;
    float SqrMaxFriendDistance => 192 * 192;
    float SqrHerdDistance => 768 * 768;
    float SqrSightRange => 444 * 444;
    float SqrAttackRange => 96 * 96;
    float XLevelDistance => 64;

    float IdleTime => 45;
    float PatrolTime => 25;
    float RunSpeed => 6;
    float FastRunSpeed => 9;
    float MaxJumpSpeed => 7;
    float BackStepTime => 35;
    float LandTime => 26;
    float SqrDistanceToPlayer => Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);



    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float Dir => ref NPC.ai[2];
    ref float Size => ref NPC.ai[3];
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
        writer.Write(_idleTime);
        writer.Write(_fakedOut);
        writer.Write(_fakeOutCount);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _pathfinder.NetReceive(reader);
        _jumpDirection = reader.ReadVector2();
        _idleTime = reader.ReadSingle();
        _fakedOut = reader.ReadBoolean();
        _fakeOutCount = reader.ReadSingle();
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
        NPC.damage = 43;
        NPC.width = 87;
        NPC.height = 38;
        NPC.lifeMax = 160;
        NPC.defense = 3;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.65f;
        NPC.noGravity = false;
    }
    public override void AI()
    {
        base.AI();
        if(MultiplayerHelper.IsHost && Size == 0)
        {
            Size = Main.rand.NextFloat(0.75f, 1.2f);
            NPC.netUpdate = true;
        }
        _outliner.SetDefaults();
        _targetAFAlpha = 0;
        _contactDamage = false;
        _attackScale = Vector2.Lerp(_attackScale, Vector2.One, 0.1f);
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
        if (Main.rand.NextBool(252))
        {
            var ratIdle = AssetReferences.Assets.Sounds.RatIdle1.Asset with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(ratIdle, NPC.position);
        }
        if (Main.rand.NextBool(47))
        {
            var pos = NPC.Center + Main.rand.NextVector2Circular(32, 32);
            var vel = (pos - NPC.Center).Resize(Main.rand.NextFloat(1, 3));
            Particles.GlowDust.Spawn(new()
            {
                position = pos,
                velocity = vel,
                color = Color.Lerp(Color.Lerp(Color.DarkGreen, Color.Green, 0.5f), Color.LightGreen, Main.rand.NextFloat()) with { A = 0 },
                scale = Main.rand.NextFloat(0.2f, 0.35f) * 0.45f,
                timeLeft = Main.rand.Next(45, 130)
            });
        }
        if (Main.rand.NextBool(24))
        {
            var color = Color.Lerp(Color.Green, Color.Black, 0.75f);
            var pos = NPC.Center + Main.rand.NextVector2Circular(32, 32);
            var vel = (pos - NPC.Center).Resize(Main.rand.NextFloat(1, 3));
            Particles.BackCloud.Spawn(new()
            {
                position = pos,
                velocity = vel,
                color = color,
                timeLeft = Main.rand.Next(60, 120),
                scale = Main.rand.NextFloat(0.5f, 1f) * 0.3f
            });
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
        NPC.VectorScale = Vector2.Lerp(Vector2.One, new Vector2(1.1f, 0.9f), EasingFunction.InOutSine(_squishTimer / 25f)) * Size;
        NPC.VectorScale *= _attackScale;
    }

    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
    }

    void AI_Idle()
    {
        Timer++;
        if(Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                _idleTime = Main.rand.NextFloat(30, 140f);
                NPC.netUpdate = true;
            }
       
        }
        NPC.TargetClosest(false);
        NPC.velocity.X *= 0.92f;
        NPC.PlayAnimationLooped(ANIM_IDLE);
        if(Timer >= _idleTime)
        {
            //I only want the sewer rats to initially aggro you
            //if your ground level is similar to them, so if you're flying they won't chase you
            var player = NPC.PlayerImTargeting;
            var diffX = MathF.Abs(NPC.Center.Y - player.Center.Y);
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
        if(MultiplayerHelper.IsHost && Timer == 1)
        {
            Dir = Main.rand.NextBool(2) ? -1 : 1;
            NPC.netUpdate = true;
        }

        var closestFriend = NPC.ClosestFriend(NPC.type, SqrHerdDistance);
        if(closestFriend != null && Vector2.DistanceSquared(NPC.Center, closestFriend.Center) > SqrMaxFriendDistance)
        {
            var dirToFriend = (closestFriend.Center.X < NPC.Center.X) ? -1 : 1;
            Dir = dirToFriend;
        }

        NPC.TargetClosest(false);
        NPC.QuickStepUp();
        NPC.StraightenRotationSlowly();
        NPC.FaceDirectionsToVelocity();
        NPC.PlayAnimationLooped(ANIM_RUN);
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Dir * 6, 0.1f);
        var player = NPC.PlayerImTargeting;
        var diffY= MathF.Abs(NPC.Center.Y - player.Center.Y);
        if (diffY < XLevelDistance && SqrDistanceToPlayer < SqrSightRange)
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
        _fakedOut = false;
        Timer++;
        MoonUtils.CreateNewPathIfTooFar(_pathfinder, NPC.Center, NPC.PlayerImTargeting.Center);
        MoonUtils.AIWalk_SusBush(_pathfinder, NPC, NPC.PlayerImTargeting.Center, MoonUtils.IsGrounded(NPC), RunSpeed, MaxJumpSpeed);
        NPC.StraightenRotationSlowly();
        NPC.FaceDirectionsToVelocity();
        NPC.PlayAnimationLooped(ANIM_RUN);
        var diffY = MathF.Abs(NPC.Center.Y - NPC.PlayerImTargeting.Center.Y);
        if (diffY > XLevelDistance)
        {
            SwitchState(AIState.Idle);
        }
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
            if (MultiplayerHelper.IsHost)
            {
                _fakedOut = Main.rand.NextBool(2);
                NPC.netUpdate = true;
            }

            NPC.PlayAnimationOneShot(ANIM_RUN);
            var s = AssetReferences.Assets.Sounds.RatSqueak.Asset with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(s, NPC.position);
            NPC.velocity.X = -NPC.direction * 8;
            NPC.velocity.Y = -2;
        }
        _jumpDirection = (NPC.PlayerImTargeting.Center - NPC.Center).SafeNormalize(Vector2.Zero);

        _outliner.warning = true;
        NPC.velocity.X *= 0.9f;
        NPC.StraightenRotationSlowly();
        NPC.SpriteFaceTarget();
        NPC.PlayAnimationOneShot(ANIM_BACKSTEP);
        NPC.QuickStepUp();
        var t = BackStepTime;
        if (!_fakedOut || _fakeOutCount >= 2)
        {
            t *= 1.5f;
            _attackScale = Vector2.Lerp(Vector2.One, new Vector2(1.2f, 0.8f), EasingFunction.InOutSine(Timer / BackStepTime));
        }
        if(Timer >= t)
        {
            if (MultiplayerHelper.IsHost)
            {
                if (_fakedOut && _fakeOutCount < 2)
                {
                    _fakeOutCount++;
                    SwitchState(AIState.Backstep);

                }
                else
                {
                    _fakeOutCount = 0;
                    SwitchState(AIState.Jump);
                }
                NPC.netUpdate = true;
            }

    
        }
    }

    void AI_Jump()
    {
        Timer++;
        if(Timer == 1)
        {
            var s = AssetReferences.Assets.Sounds.RatAttack.Asset with { PitchVariance = 0.5f };
            SoundEngine.PlaySound(s, NPC.position);
            NPC.velocity = _jumpDirection * 12;
            NPC.velocity.Y = -4;
        }
        NPC.velocity.X *= 0.96f;
        NPC.PlayAnimationOneShot(ANIM_BITEUP);
        NPC.QuickStepUp();

        _targetAFAlpha = 1f;
        _outliner.attacking = true;
        _contactDamage = true;
        if (MoonUtils.IsGrounded(NPC) && Timer >= 20)
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
        NPC.velocity.X *= 0.92f;
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
        if(_afAlpha > 0.02f)
        {
            for (var i = 0; i < NPC.oldPos.Length; i++)
            {
                var pos = NPC.oldPos[i] + NPC.Size * 0.5f;
                var color = Color.Lerp(Color.Red, Color.Transparent, i / (float)NPC.oldPos.Length);
                color *= 0.15f;
                color *= _afAlpha;
                color.A = 0;
                NPC.DrawAnimator(spriteBatch, color, pos);
            }
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
