using Stellamod.Common;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.NPCsUG;

public class BlastingManiac : ModNPC,
    IDrawToRenderTarget
{
    enum AIState : byte
    {
        Idle,
        Walk,
        ThrowReady,
        Throw
    }
    Outliner _outliner;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float AttackCycle => ref NPC.ai[2];
    float AttackDist => 200 * 200;
    float IdleTime => 80;
    float WalkTime => 80;
    float ThrowReadyTime => 60;
    float ThrowTime => 40;
    const string ANIM_IDLE = "Idle";
    const string ANIM_WALK = "Walk";
    const string ANIM_THROWSTARTUP = "ThrowStartup";
    const string ANIM_THROW = "Throw";
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.UsesAseprite[Type] = true;
        this.AddToUnderground();
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.damage = 10;
        NPC.width = 80;
        NPC.height = 40;
        NPC.lifeMax = 50;
        NPC.defense = 7;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.65f;
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
            case AIState.Walk:
                AI_Walk();
                break;
            case AIState.ThrowReady:
                AI_ThrowReady();
                break;
            case AIState.Throw:
                AI_Throw();
                break;
        }
        _outliner.Update();
        NPC.DrawOrigin = new Vector2(44, 42);
        NPC.spriteDirection = NPC.direction;
    }

    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
        AttackCycle = 0;
    }

    void AI_Idle()
    {
        Timer++;
        NPC.TargetClosest(faceTarget: false);
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
        NPC.velocity.X *= 0.92f;
        NPC.PlayAnimationLooped(ANIM_IDLE);
        if(Timer >= IdleTime)
        {
            SwitchState(AIState.Walk);
        }
        if(Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center) <= AttackDist && Timer >= IdleTime)
        {
            SwitchState(AIState.ThrowReady);
        }
    }

    void AI_Walk()
    {
        Timer++;
        if(Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                NPC.direction = Main.rand.NextBool(2) ? -1 : 1;
                NPC.netUpdate = true;
            }
        }

        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction, 0.03f);
        NPC.PlayAnimationLooped(ANIM_WALK);
        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
        var inFrntOfMe = NPC.Top + new Vector2(NPC.direction * 32, 0);
        var t = Main.tile[inFrntOfMe.ToTileCoordinates()];
        if (WorldGen.SolidTile(t))
        {
            NPC.direction *= -1;
        }

        if(Timer >= WalkTime)
        {
            SwitchState(AIState.Idle);
        }

        if (Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center) <= AttackDist)
        {
            SwitchState(AIState.ThrowReady);
        }
    }

    void AI_ThrowReady()
    {
        Timer++;
        _outliner.warning = true;
        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
        NPC.velocity.X *= 0.9f;
        NPC.direction = (NPC.PlayerImTargeting.Center.X < NPC.Center.X) ? -1 : 1;
        NPC.PlayAnimationOneShot(ANIM_THROWSTARTUP);
        if(Timer >= ThrowReadyTime)
        {
            SwitchState(AIState.Throw);
        }
    }

    void AI_Throw()
    {
        Timer++;
        if(Timer == 1)
        {
            NPC.velocity.X = NPC.direction * 4;
            NPC.velocity.Y = -5;
 
        }

        if(Timer == 15)
        {
            if (MultiplayerHelper.IsHost)
            {
                var x = (int)NPC.Center.X;
                var y = (int)NPC.Center.Y;
                var type = ModContent.NPCType<BlastingBomb>();
                var throwDir = (NPC.PlayerImTargeting.Center - NPC.Center).ToRotation();
                NPC.NewNPC(NPC.GetSource_FromThis(), x, y, type, ai3: 10f + throwDir);
            }
        }
        _outliner.attacking = true;
        NPC.velocity.X *= 0.9f;
        NPC.PlayAnimationOneShot(ANIM_THROW);
        if(Timer >= ThrowTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;
        if(NPC.life <= 0)
        {
            this.MakeGore("_Gore_Head", NPC.position, NPC.velocity);
            this.MakeGore("_Gore_Arm", NPC.position + new Vector2(-5, 0), NPC.velocity + new Vector2(-2));
            this.MakeGore("_Gore_Butt", NPC.position + new Vector2(-8, -2), NPC.velocity + new Vector2(-1, 0));
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
    }

    public void DrawToRenderTargets()
    {
        if (!_outliner.VisibleEnough)
            return;
        void DrawOutline(SpriteBatch sb)
        {
            NPC.DrawAnimator(sb, _outliner.outlineColor);
        }
        OutlineRenderer.Queue(DrawOutline);
    }
}
