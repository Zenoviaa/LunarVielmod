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
namespace Stellamod.Content.Areas.SpringHills.NPCsSH;

public class IvynSpearman : ModNPC,
    IDrawToRenderTarget
{
    bool _contactDamage;
    float _targetAFAlpha;
    float _afAlpha;
    Vector2 _jumpDirection;
    Pathfinder _pathfinder;
    Outliner _outliner;
    enum AIState : byte
    {
        Idle,
        Patrol,
        Chase,
        JumpFall,
        Slam,
        Pullout,
    }

    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float Dir => ref NPC.ai[2];

    float IdleTime => 45;
    float PatrolTime => 80;
    float SqrAttackRange => 100 * 100;
    float SqrChaseDistance => 256 * 256;
    float SqrStopRange => 444 * 444;
    float RunSpeed => 4;
    float MaxJumpSpeed => 6;
    float SlamTime => 75;
    float PulloutTime => 45;
    const string ANIM_IDLE = "Idle";
    const string ANIM_RUN = "Run";
    const string ANIM_CROUCH = "Crouch";
    const string ANIM_JUMP = "Jump";
    const string ANIM_FALL = "fall";
    const string ANIM_SLAM = "Slam";
    const string ANIM_PULLOUT = "Pullout";
    const string ANIM_EDGEFALLFRAME = "edgeFallframe";
    public override string Texture => TextureRegistry.EmptyTexture;
    float SqrDistanceToPlayer => Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
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
        this.AddToSpringHills();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        _pathfinder = new();
        NPC.damage = 32;
        NPC.width = 34;
        NPC.height = 72;
        NPC.lifeMax = 35;
        NPC.defense = 6;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.25f;
        NPC.noGravity = false;
    }

    public override void AI()
    {
        base.AI();
        _outliner.SetDefaults();
        _targetAFAlpha = 0;
        NPC.noGravity = false;
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
            case AIState.JumpFall:
                AI_JumpFall();
                break;
            case AIState.Slam:
                AI_Slam();
                break;
            case AIState.Pullout:
                AI_Pullout();
                break;
        }
        _afAlpha = MathHelper.Lerp(_afAlpha, _targetAFAlpha, 0.1f);
        _outliner.Update();
        NPC.DrawOrigin = new Vector2(74, 97);
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
        NPC.StraightenRotationSlowly();
        NPC.PlayAnimationLooped(ANIM_IDLE);
        NPC.velocity.X *= 0.92f;

        if(Timer >= IdleTime)
        {
           if(SqrDistanceToPlayer < SqrChaseDistance)
            {
                SwitchState(AIState.Chase);
            }
            else
            {
                SwitchState(AIState.Patrol);
            }
        }
    }
    void AI_Patrol()
    {
        Timer++;
        if(Dir == 0)
        {
            Dir = 1;
            NPC.netUpdate = true;
        }
        if (MultiplayerHelper.IsHost && Main.rand.NextBool(5) && Timer == 1)
        {
            Dir = Main.rand.NextBool(2) ? -1 : 1;
            NPC.netUpdate = true;
        }

        NPC.TargetClosest(false);
        NPC.QuickStepUp();
        NPC.StraightenRotationSlowly();
        NPC.FaceDirectionsToVelocity();
        if (!MoonUtils.IsGrounded(NPC))
        {
            NPC.PlayAnimationLooped(ANIM_EDGEFALLFRAME);
        }
        else
        {

            NPC.PlayAnimationLooped(ANIM_RUN);
        }

        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Dir * 2, 0.1f);
        var player = NPC.PlayerImTargeting;
        var diffX = MathF.Abs(NPC.Center.X - player.Center.X);
        if (SqrDistanceToPlayer < SqrChaseDistance)
        {
            SwitchState(AIState.Chase);
        }
        else if (Timer >= PatrolTime)
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
        NPC.QuickStepUp();

        var targetingPlayer = NPC.PlayerImTargeting;
        var sqrDistanceToPlayer = Vector2.DistanceSquared(NPC.Center, targetingPlayer.Center);
        if (sqrDistanceToPlayer < SqrAttackRange)
        {
            SwitchState(AIState.JumpFall);
        }
        if(sqrDistanceToPlayer > SqrStopRange)
        {
            SwitchState(AIState.Idle);
        }
    }

    void AI_JumpFall()
    {
        Timer++;
        if(Timer == 1)
        {
            _jumpDirection.X = NPC.direction;
        }
        if (Timer < 30)

        {
            NPC.velocity.X *= 0.94f;
            NPC.PlayAnimationOneShot(ANIM_CROUCH);
            _outliner.warning = true;
        }

        if (Timer == 30)
        {
            SoundStyle bellHit = AssetRegistry.Sounds.Magic.AutomationHit1;
            bellHit.PitchVariance = 0.2f;
            SoundEngine.PlaySound(bellHit, NPC.position);
            NPC.velocity.Y = -17;
            NPC.velocity += _jumpDirection * 4;
            var p2 = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Bottom, Vector2.UnitY * 4);
            p2.fadeToColor = Color.Black;
            p2.innerColor = Color.White;
            p2.outerColor = Color.DarkGreen;
            p2.Scale *= 0.5f;

        }

        if (Timer >= 45)
        {

            if (Timer % 2 == 0)
            {
                Particles.IvynLeaf.Spawn(new()
                {
                    position = NPC.Center + Main.rand.NextVector2Circular(32, 32),
                    velocity = Main.rand.NextVector2Circular(3, 3),
                    timeLeft = Main.rand.Next(45, 180),
                    scale = Main.rand.NextFloat(0.5f, 1.2f),
                    rotation = Main.rand.NextFloat() * 6.28f,
                });
            }
            _targetAFAlpha = 1f;
            if(Timer >= 55)
            {
                NPC.PlayAnimationOneShot(ANIM_FALL);
            }
            else
            {
                NPC.PlayAnimationOneShot(ANIM_JUMP);
            }
            if (Timer <= 75)
            {
   
                NPC.velocity.Y *= 0.95f;
                NPC.rotation = NPC.velocity.X * 0.05f;
            }
            else
            {
             
                if (Timer == 76)
                {
                    SoundStyle fallSound = AssetReferences.Assets.Sounds.Melee.SlashQuick7.Asset;
                    fallSound.PitchVariance = 0.1f;
                    fallSound.Pitch = 0.5F;
                    SoundEngine.PlaySound(fallSound, NPC.position);
                }
                _outliner.attacking = true;

                _contactDamage = true;
                NPC.rotation = -NPC.velocity.X * 0.03f;
                //   NPC.velocity.X += NPC.direction * 0.1f;
                if (NPC.velocity.Y > 0)
                    NPC.velocity.Y *= 1.06f;
                else
                    NPC.velocity.Y += 0.5f;
                var dirToPlayer = (NPC.PlayerImTargeting.Center.X < NPC.Center.X) ? -1 : 1;
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, dirToPlayer * 7, 0.1f);
                NPC.noGravity = true;
                if (Timer % 4 == 0 && NPC.velocity.Y > 0)
                {
                    Particles.StabTriangle.Spawn(new()
                    {
                        position = NPC.Bottom + NPC.velocity * 3,
                        velocity = -NPC.velocity * 0.5f,
                        rotation = NPC.velocity.ToRotation(),
                        timeLeft = 30,
                        color = Color.LightGreen * 0.22f,
                        scale = 0.5f
                    });
                }
            }
        }
        if (Timer >= 40 && NPC.collideY && NPC.velocity.Y > 0)
        {
            SwitchState(AIState.Slam);
        }
    }

    void AI_Slam()
    {
        Timer++;
        if(Timer == 1)
        {
            SoundStyle fallSound = AssetReferences.Assets.Sounds.Melee.Parendine2.Asset;
            fallSound.PitchVariance = 0.1f;
            fallSound.Pitch = 0.5f;
            SoundEngine.PlaySound(fallSound, NPC.position);
            var impact = ParticleUtils.ParticleFactory.FromSmallBurst(NPC.Center, TriColorPalette.Foresty, new(5, 15));
            ParticleUtils.CreateSwirlingDustCircle(impact);
            for(var i = 0; i < 4; i++)
            {
                var pos = NPC.Bottom + Main.rand.NextVector2Circular(32, 32);
                var vel = (pos - NPC.Bottom).Resize(8) * Main.rand.NextFloat(0.75f, 1.5f) * 2;
                var fx = FXUtil.GlowStretch(pos, vel);
                fx.OuterGlowColor = Color.DarkGreen;
                fx.GlowColor = Color.LightGreen;
            }
        }
        var range = MathHelper.Lerp(0.2f, 0f, EasingFunction.OutSine(Timer / SlamTime));
        var targetRot = ExtraMath.Osc(-range, range, speed: 24);
        NPC.rotation = targetRot;
        NPC.noGravity = false;
        NPC.PlayAnimationOneShot(ANIM_SLAM);
        NPC.velocity.X *= 0;
        if(Timer >= SlamTime )
        {
            SwitchState(AIState.Pullout);
        }
    }
    void AI_Pullout()
    {
        Timer++;
        if(Timer == 1)
        {
            NPC.velocity.Y = -6;
            NPC.velocity.X = -NPC.direction * 2;
        }
        NPC.PlayAnimationOneShot(ANIM_PULLOUT);
        if(Timer >= PulloutTime)
        {
            SwitchState(AIState.Idle);
        }
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if(_afAlpha > 0.04f)
        {
            for (var i = 0; i < NPC.oldPos.Length; i++)
            {
                var pos = NPC.oldPos[i] + NPC.Size * 0.5f;
                var color = Color.Lerp(Color.OrangeRed, Color.Transparent, (float)i / (float)NPC.oldPos.Length) * _afAlpha * 0.2f;
                NPC.DrawAnimator(spriteBatch, color, pos);
            }
        }

        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
        //return base.PreDraw(spriteBatch, screenPos, drawColor);
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;
        if (NPC.life > 0)
            return;
        this.MakeGore("_Gore_Head", NPC.position, NPC.velocity + new Vector2(0, -1));
        this.MakeGore("_Gore_Butt", NPC.position, NPC.velocity + new Vector2(0, 1));
    }

    public void DrawToRenderTargets()
    {
        NPC.DrawOutlineByAnimator(_outliner);
    }
}
