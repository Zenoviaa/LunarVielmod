using Stellamod.Common;
using Stellamod.Common.Particles;
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
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.NPCsSH;

public class StabingBush : ModNPC,
    IDrawToRenderTarget
{
    bool _isGrounded;
    Vector2 _oldPlayerPos;
    Vector2 _dashDirection;
    Pathfinder _pathfinder;
    Outliner _outliner;
    enum AIState : byte
    {
        Idle,
        Hide,
        Openmouth,
        Chase,
        Bite
    }
    bool _contactDamage;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float AttackCycle => ref NPC.ai[2];
    bool _useAF;
    float _afAlpha;
    public override string Texture => TextureRegistry.EmptyTexture;
    const string ANIM_INVISIBLE = "Invisible";
    const string ANIM_WALKING = "Walking";
    const string ANIM_OPENMOUTH = "Openmouth";
    const string ANIM_CLOSEMOUTH = "Closemouth";
    const string ANIM_CHASING = "Chasing";
    const string ANIM_WALKIN = "Walkin";
    float IdleTime => 24;
    float ChaseSpeed => 5;
    float MaxJumpSpeed => 8;
    float AttackDistance => 128 * 128;
    float OpenMouthTime => 45;
    float BiteTime => 60;
    float HideTime => 30;
    float AggroDistance => 384 * 384;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_dashDirection);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _dashDirection = reader.ReadVector2();
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 0;
        NPCID.Sets.UsesAseprite[Type] = true;
        this.AddToMarsh();
    }
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        _pathfinder = new();
        NPC.damage = 36;
        NPC.width = 58;
        NPC.height = 54;
        NPC.lifeMax = 78;
        NPC.defense = 8;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.65f;
        NPC.noGravity = false;
    }

    public override void AI()
    {
        base.AI();
        _outliner.SetDefaults();
        _contactDamage = false;
        _useAF = false;
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Hide:
                AI_Hide();
                break;
            case AIState.Openmouth:
                AI_Openmouth();
                break;
            case AIState.Chase:
                AI_Chase();
                break;
            case AIState.Bite:
                AI_Bite();
                break;
        }

        _afAlpha = MathHelper.Lerp(_afAlpha, _useAF ? 1f : 0f, 0.1f);
        _isGrounded = MoonUtils.IsGrounded(NPC);
        _outliner.Update();
        NPC.DrawOrigin = new Vector2(32, 38);
        NPC.spriteDirection = NPC.direction;
    }

    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
    }

    void FaceVelocity()
    {
        NPC.direction = (NPC.velocity.X < 0) ? -1 : 1;
    }

    bool IsLookingAtMe()
    {
        if (NPC.life < NPC.lifeMax * 0.5f)
            return false;
        var target = NPC.PlayerImTargeting;
        var targetDir = target.direction;
        var dirToMe = (NPC.Center.X < target.Center.X) ? 1 : -1;
        return dirToMe == -targetDir;
    }
    void AI_Idle()
    {
        NPC.TargetClosest(false);
        Timer++;
        if(Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                NPC.direction = Main.rand.NextBool(2) ? -1 : 1;
                NPC.netUpdate = true;
            }
    
        }
        NPC.velocity.X *= 0.88f;
        NPC.PlayAnimationLooped(ANIM_INVISIBLE);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
        if (Timer >= IdleTime && !IsLookingAtMe() && Vector2.DistanceSquared(NPC.PlayerImTargeting.Center, NPC.Center) < AggroDistance) 
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

        MoonUtils.AIWalk_SusBush
            (_pathfinder, NPC, NPC.PlayerImTargeting.Center, _isGrounded, ChaseSpeed, MaxJumpSpeed);
        FaceVelocity();
        NPC.PlayAnimationLooped(ANIM_WALKIN);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
        var sqrDistPlayer = Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
        if(IsLookingAtMe())
        {
            SwitchState(AIState.Idle);
        }
        if(sqrDistPlayer < AttackDistance)
        {
            SwitchState(AIState.Openmouth);
        }
    }

    void AI_Openmouth()
    {
        Timer++;
        if(Timer == 1)
        {
            _dashDirection = (NPC.PlayerImTargeting.Center - NPC.Center).SafeNormalize(Vector2.Zero);
        }
        _outliner.warning = true;
        NPC.PlayAnimationOneShot(ANIM_OPENMOUTH);
        NPC.velocity *= 0.88f;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
        if (IsLookingAtMe())
        {
            SwitchState(AIState.Idle);
        }
        if (Timer >= OpenMouthTime)
        {
            SwitchState(AIState.Bite);
        }
    }

    void AI_Bite()
    {
        _useAF = true;
     
        Timer++;
        if(Timer == 1)
        {
            var sound = AssetReferences.Assets.Sounds.NaturalHit.Asset with { PitchVariance = 0.6f };
            SoundEngine.PlaySound(sound, NPC.position);
        }
        if (Timer < 15)
        {
            NPC.velocity = Vector2.Lerp(NPC.velocity, -_dashDirection * 2, 0.1f);
     
        }
        else if (Timer < 25)
        {
            NPC.velocity *= 0.96f;
        }
        else if(Timer == 35)
        {
            NPC.velocity = _dashDirection * 15;
            var sound = AssetReferences.Assets.Sounds.NaturalCast.Asset with { PitchVariance = 0.6f };
            SoundEngine.PlaySound(sound, NPC.position);
        }

        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.025f, 0.1f);
        if (Timer > 35)
        {
            if(Timer % 2 == 0)
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
            _contactDamage = true;
            _outliner.attacking = true;
            NPC.velocity *= 0.96f;
        }
        else
        {
            _outliner.warning = true;
        }
        NPC.spriteDirection = _dashDirection.X < 0 ? -1 : 1;
        NPC.PlayAnimationOneShot(ANIM_CHASING);
        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
        if(Timer >= BiteTime)
        {
            SwitchState(AIState.Hide);
        }
    }

    void AI_Hide()
    {
        Timer++;
        NPC.velocity.X *= 0.88f;
        NPC.PlayAnimationOneShot(ANIM_CLOSEMOUTH);
        if(Timer >= HideTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;

        foreach (PositionVelocity posVel in new CircleOrientation(NPC.Center, 32, 1))
        {
            Particles.IvynLeaf.Spawn(new()
            {
                position = posVel.position,
                velocity = posVel.velocity * Main.rand.NextFloat(0.4f, 1f) * 0.3f,
                timeLeft = Main.rand.Next(45, 180),
                scale = Main.rand.NextFloat(0.5f, 1.2f),
                rotation = Main.rand.NextFloat() * 6.28f,
            });
        }

        if (NPC.life > 0)
            return;

        var suond = AssetReferences.Assets.Sounds.NaturalHit.Asset with { PitchVariance = 0.5f };
        SoundEngine.PlaySound(suond, NPC.position);
        foreach(PositionVelocity posVel in new CircleOrientation(NPC.Center, 32, 48))
        {
            Particles.SwirlingFlameDust.Spawn(new()
            {
                position = posVel.position,
                velocity = posVel.velocity * Main.rand.NextFloat(0.4f, 1f) * 0.3f,
                innerColor = Color.LightGreen.ToVector4(),
                outerColor = Color.DarkGreen.ToVector4(),
                scale = Main.rand.NextFloat(0.5f, 1.2f) * Vector2.One,
                timeLeft = Main.rand.Next(45, 120),
            });
        }
        foreach(PositionVelocity posVel in new CircleOrientation(NPC.Center, 32, 48))
        {
            Particles.IvynLeaf.Spawn(new()
            {
                position = posVel.position,
                velocity = posVel.velocity * Main.rand.NextFloat(0.4f, 1f) * 0.3f,
                timeLeft = Main.rand.Next(45, 180),
                scale = Main.rand.NextFloat(0.5f, 1.2f),
                rotation = Main.rand.NextFloat() * 6.28f,
            });
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if(_afAlpha > 0.05f)
        {
            for (var i = 0; i < NPC.oldPos.Length; i++)
            {
                var oldPOs = NPC.oldPos[i] + NPC.Size * 0.5f;
                var info = NPC.GetAnimatorDrawInfo(drawColor, oldPOs);
                info.color = Color.Lerp(Color.DarkGreen, Color.Transparent, (float)i / (float)NPC.oldPos.Length) * 0.2f * _afAlpha;
                spriteBatch.Draw(info);
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
