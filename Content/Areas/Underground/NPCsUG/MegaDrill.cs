using Stellamod.Common;
using Stellamod.Common.Particles;
using Stellamod.Core.NPCHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.NPCsUG;

public class MegaDrill : ModNPC
{
    enum AIState : byte
    {
        Teleport,
        Drill,
        Slam,
        Stuck,
    }

    bool _contactDamage;
    bool _solidLastFrame;
    Vector2 _teleportPosition;
    Outliner _outliner;
    const string ANIM_IDLE = "Idle";
    const string ANIM_DRILL = "Drill";
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float AttackCycle => ref NPC.ai[2];
    float SlamTime => 60;
    float SlamCount => 3;
    float StopDistance => 300 * 300;
    float TooFar => 1024 * 1024f;
    float StuckTime => 120;
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_teleportPosition);
        writer.Write(_solidLastFrame);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _teleportPosition = reader.ReadVector2();
        _solidLastFrame = reader.ReadBoolean();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 8;
        NPCID.Sets.UsesAseprite[Type] = true;
        this.AddToUnderground();
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.damage = 17;
        NPC.width = NPC.height = 58;
        NPC.lifeMax = 50;
        NPC.defense = 7;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.65f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
    }


    public void Teleport(Vector2 posToTeleportTo)
    {
        if (MultiplayerHelper.IsHost)
        {
            _teleportPosition = posToTeleportTo;
            NPC.netUpdate = true;
        }
    }
    public override void AI()
    {
        base.AI();
        if(_teleportPosition != Vector2.Zero)
        {
            NPC.velocity = Vector2.Zero;
            NPC.Center = _teleportPosition;
            _teleportPosition = Vector2.Zero;
        }
        NPC.TargetClosest();
        var distToPlayer = Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center);
        if(distToPlayer > TooFar)
        {
            SwitchState(AIState.Teleport);
        }
        _contactDamage = false;
        _outliner.SetDefaults();
        switch (State)
        {
            case AIState.Drill:
                AI_Drill();
                break;
            case AIState.Stuck:
                AI_Stuck();
                break;
            case AIState.Slam:
                AI_Slam();
                break;
            case AIState.Teleport:
                AI_Teleport();
                break;
        }
        _outliner.Update();
    }
    
    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
    }
    void AI_Teleport()
    {
        Timer++;
        if(Timer == 1)
        {
            Teleport(NPC.PlayerImTargeting.Center + new Vector2(0, -384));
        }
        _outliner.warning = true;
        NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.UnitY * 15, 0.2f);
        if(NPC.velocity.LengthSquared() > 12 * 12)
        {
            SwitchState(AIState.Drill);
        }

    }

    void AI_Drill()
    {
        _outliner.attacking = true;
        Timer++;
        NPC.TargetClosest();
        NPC.PlayAnimationLooped(ANIM_DRILL);
        var tilePos = NPC.Center.ToTileCoordinates();
        var tile = Main.tile[tilePos];
        if(!WorldGen.SolidOrSlopedTile(tile) && _solidLastFrame)
        {
            SwitchState(AIState.Slam);
        }
        _solidLastFrame = WorldGen.SolidOrSlopedTile(tile);
    }


    void AI_Stuck()
    {
        Timer++;
        NPC.PlayAnimationLooped(ANIM_IDLE);
        if (Timer >= StuckTime)
        {
            if(NPC.velocity.Length() < 15)
                NPC.velocity *= 1.01f;
        }
        else if (NPC.velocity.Length() > 0.1f)
        {
            NPC.velocity *= 0.1f;
        }
    }

    void ImpactFX()
    {
        var fac = ParticleUtils.ParticleFactory.FromSmallBurst(NPC.Center, TriColorPalette.Fiery, new Vector2(5, 15f));
        ParticleUtils.CreateSwirlingDustCircle(fac);

        foreach (PositionVelocity posVel in new CircleOrientation(NPC.Center, 64, 32))
        {
            Particles.GlowDust.Spawn(new()
            {
                position = posVel.position,
                velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f),
                color = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat()),
                timeLeft = Main.rand.Next(45, 120),
                scale = Main.rand.NextFloat(0.15f, 0.25f)
            });
        }
    }

    void AI_Slam()
    {
        _contactDamage = true;
        _outliner.attacking = true;
        Timer++;
        if(Timer == 1)
        {
            ImpactFX();
        }
        NPC.PlayAnimationLooped(ANIM_DRILL);
        NPC.velocity *= 0.2f;
        if(Timer >= SlamTime)
        {
            AttackCycle++;
            if(AttackCycle >= SlamCount && Vector2.DistanceSquared(NPC.Center, NPC.PlayerImTargeting.Center) < StopDistance)
            {
                SwitchState(AIState.Stuck);
            }
            else
            {
                SwitchState(AIState.Drill);
            }
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (Main.netMode == NetmodeID.Server)
            return;
        if (NPC.life > 0)
            return;

        var color = Color.Lerp(Color.OrangeRed, Color.Black, 0.75f);
        foreach (PositionVelocity posVel in new CircleOrientation(NPC.Center, 64, 32))
        {
            Particles.BackCloud.Spawn(new()
            {
                position = posVel.position,
                velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f),
                color = color,
                timeLeft = Main.rand.Next(60, 120),
                scale = Main.rand.NextFloat(0.5f, 1f)
            });
        }

        foreach (PositionVelocity posVel in new CircleOrientation(NPC.Center, 64, 16))
        {
            Particles.GlowDust.Spawn(new()
            {
                position = posVel.position,
                velocity = posVel.velocity * Main.rand.NextFloat(0.5f, 1f),
                color = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat()),
                timeLeft = Main.rand.Next(45, 120),
                scale = Main.rand.NextFloat(0.15f, 0.25f)
            });
        }

        this.MakeGore("_Gore_Left", NPC.position, NPC.velocity + new Vector2(-5, 0));
        this.MakeGore("_Gore_Right", NPC.position, NPC.velocity + new Vector2(5, 0));
        this.MakeGore("_Gore_Head", NPC.position, NPC.velocity + new Vector2(0, -5));
        this.MakeGore("_Gore_Bottom", NPC.position, NPC.velocity + new Vector2(0, 5));
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
