using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Core.Utilities;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.TheFalling.PerfectSingularityBoss;

public class PerfectEyesShader : CrystalShader<PerfectEyesShader>
{
    public Texture2D Texture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[0] = value;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        }
    }
    public Matrix TransformMatrix
    {
        set
        {
            Effect.Parameters["transformMatrix"].SetValue(value);
        }
    }
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }
}
public class PerfectRingShader : CrystalShader<PerfectRingShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }
}

public class PerfectSingularityShader : CrystalShader<PerfectSingularityShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public Texture2D NoiseTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
        }
    }
}

public partial class PerfectSingularity : ScarletBoss
{
    private enum PerfectSingularityPhase
    {
        Spawn,
        Despawn,
        Idle,
        Death,

        Chain_Whip,
        Chain_Jail,
        Chain_Arrow,
        Astral_Chains,
        Astral_Blast,

        Astral_Dash,
        Meteor_Rain,
        Solare_Flare,
        Alien_Laser_Beam
    }

    private bool _contactDamage;
    private Outliner _outliner;
    private ref float Timer => ref NPC.ai[0];
    private PerfectSingularityPhase Phase
    {
        get => (PerfectSingularityPhase)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.TrailCacheLength[NPC.type] = 32;
        NPCID.Sets.TrailingMode[Type] = 3;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCID.Sets.MustAlwaysDraw[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 90;
        NPC.height = 90;
        NPC.damage = 150;
        NPC.defense = 66;
        NPC.lifeMax = 300000;
        NPC.scale = 1f;
        NPC.aiStyle = -1;

        NPC.value = Item.buyPrice(gold: 5);
        NPC.knockBackResist = 0f;
        NPC.boss = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.npcSlots = 30f;

        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/ItRains");
        NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
    }
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }
    public override bool? CanBeHitByItem(Player player, Item item) => false;
    public override bool CanBeHitByNPC(NPC attacker) => false;
    public override bool? CanBeHitByProjectile(Projectile projectile) => false;
    public override void AI()
    {
        base.AI();
        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
            {
                if (Phase != PerfectSingularityPhase.Despawn)
                {
                    SwitchState(PerfectSingularityPhase.Despawn);
                }
            }
        }

        _contactDamage = false;
        _outliner.SetDefaults();
        switch (Phase)
        {
            case PerfectSingularityPhase.Spawn:
                AI_Spawn();
                break;
            case PerfectSingularityPhase.Despawn:
                AI_Despawn();
                break;
            case PerfectSingularityPhase.Idle:
                AI_Idle();
                break;
            case PerfectSingularityPhase.Death:
                AI_Death();
                break;
            case PerfectSingularityPhase.Chain_Whip:
                AI_ChainWhip();
                break;
            case PerfectSingularityPhase.Chain_Jail:
                AI_ChainJail();
                break;
            case PerfectSingularityPhase.Chain_Arrow:
                AI_ChainArrow();
                break;
            case PerfectSingularityPhase.Astral_Chains:
                AI_AstralChains();
                break;
            case PerfectSingularityPhase.Astral_Blast:
                AI_AstralBlast();
                break;
            case PerfectSingularityPhase.Astral_Dash:
                AI_AstralDash();
                break;
            case PerfectSingularityPhase.Meteor_Rain:
                AI_MeteorRain();
                break;
            case PerfectSingularityPhase.Solare_Flare:
                AI_SolarFlare();
                break;
            case PerfectSingularityPhase.Alien_Laser_Beam:
                AI_AlienLaser();
                break;
        }
        _outliner.Update();
        NPC.rotation = NPC.velocity.X * 0.05f;
    }

    private void ChooseAttack()
    {

    }

    private void AI_AlienLaser()
    {

    }
    private void AI_SolarFlare()
    {

    }
    private void AI_MeteorRain()
    {

    }
    private void AI_AstralDash()
    {

    }
    private void AI_AstralBlast()
    {

    }
    private void AI_AstralChains()
    {

    }
    private void AI_ChainArrow()
    {

    }
    private void AI_ChainJail()
    {

    }
    private void AI_ChainWhip()
    {

    }
    private void AI_Spawn()
    {
        SwitchState(PerfectSingularityPhase.Idle);
    }

    private void AI_Idle()
    {

    }
    private void AI_Despawn()
    {

    }
    private void AI_Death()
    {

    }
    private void SwitchState(PerfectSingularityPhase phase)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            Phase = phase;
            AttackCounter = 0;
            AttackCycle = 0;
            NPC.netUpdate = true;
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }
    public override void OnKill()
    {
        base.OnKill();
    }
}
