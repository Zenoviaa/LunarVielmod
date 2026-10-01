using Stellamod.Core;
using Stellamod.Core.NPCHelpers;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.ZigguratraBoss;

public partial class Zigguratra : ScarletBoss
{
    private enum AIState : byte
    {
        Spawn,
        Despawn,
        Idle,
        Death,

        Axe_Crash,
        Rude_Buster,
        Release_The_Bees,
        Call_The_Bees,
        Splitting_Lightning,
        Splitting_Lightning_Crash,
        Splitting_Lightning_WallJump,
    }

    Vector2 _teleportPos;
    private bool _version2;
    private Outliner _outliner;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    PatternManager<AIState> AttackPattern
    {
        get
        {
            if (field == null)
            {
                field = new();
                field.AddPattern(AIState.Axe_Crash, 1f);
            }

            return field;
        }
    }

    Vector2 _vector21;
    Vector2 _vector22;
    float IdleTime => 100;

    bool _axeWalk;

    float _targetAfterIamgeAlpha;
    float _afterImageAlpha;
    float _invisibleAlpha;
    float _axeLightningAlpha;
    bool _axeLightningDashed;
    const string ANIM_IDLE = "Idle";
    const string ANIM_RELEASE_THE_BEES = "ReleaseTheBees";
    const string ANIM_RELEASE_THE_BEES_HOLD = "ReleaseTheBeesHold";
    const string ANIM_RELEASE_THE_BEES_OUT = "ReleaseTheBeesOut";
    const string ANIM_AXE_SUMMON = "AxeSummon";
    const string ANIM_AXE_CHARGE_1 = "AxeCharge1";
    const string ANIM_AXE_CHARGE_2 = "AxeCharge2";
    const string ANIM_AXE_CHARGE_3 = "AxeCharge3";
    const string ANIM_AXE_CHARGE_DASH_OUT = "AxeChargeDashOut";
    const string ANIM_AXE_READY = "AxeReady";
    const string ANIM_AXE_WALK = "AxeWalk";
    const string ANIM_AXE_SLAM = "AxeSlam";
    const string ANIM_AXE_SLAM_DASH_AWAY = "AxeSlamDashAway";
    const string ANIM_AXE_SLAM_DASH_OUT = "AxeSlamDashOut";
    const string ANIM_WALL_CLING = "WallCling";
    const string ANIM_WALL_DASH = "WallDash";
    const string ANIM_WALL_AXE_CRASH = "WallAxeCrash";
    const string ANIM_SUPER_AXE_CRASH = "SuperAxeCrash";
    const string ANIM_FALLING_AXE_CRASH = "FallingAxeCrash";
    const string ANIM_SPIN = "Spin";

    const string ANIM_JUMPSTART = "JumpStart";
    const string ANIM_SPINBUSTER = "SpinBuster";
    const string ANIM_JUMP = "Jump";
    ref float AttackCycle => ref NPC.ai[2];
    ref float AttackCounter => ref NPC.ai[3];


    int Damage_RudeLightning => 37;
    int Damage_SplittingLightningCrash => 50;
    int Damage_ReleaseTheBees => 26;
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _teleportPos = reader.ReadVector2();
        _axeLightningDashed = reader.ReadBoolean();
        _version2 = reader.ReadBoolean();
        _vector21 = reader.ReadVector2();
        _vector22 = reader.ReadVector2();
        _rudelySwung = reader.ReadBoolean();
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_teleportPos);
        writer.Write(_axeLightningDashed);
        writer.Write(_version2);
        writer.WriteVector2(_vector21);
        writer.WriteVector2(_vector22);
        writer.Write(_rudelySwung);
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCSets.UseAseprite[Type] = true;
        NPCID.Sets.TrailCacheLength[Type] = 12;
        NPCID.Sets.TrailingMode[Type] = 3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 128;
        NPC.height = 158;
        NPC.damage = 50;
        NPC.defense = 5;
        NPC.lifeMax = 2200;
        NPC.knockBackResist = 0f;

        NPC.boss = true;
        NPC.npcSlots = 30;

        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/JazziestBugs");
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
    }

    public override void AI()
    {
        base.AI();
        if (!NPC.HasValidTarget)
        {
            NPC.TargetClosest();
            if (!NPC.HasValidTarget && State != AIState.Despawn)
                SwitchState(AIState.Despawn);
        }

        if (_teleportPos != Vector2.Zero)
        {
            NPC.Center = _teleportPos;
            _teleportPos = Vector2.Zero;
        }
        _outliner.SetDefaults();
        _version2 = false;
        _axeLightningAlpha = MathHelper.Lerp(_axeLightningAlpha, 0f, 0.1f);
        _invisibleAlpha = 1f;
        _targetAfterIamgeAlpha = 0f;
        switch (State)
        {
            case AIState.Spawn:
                AI_Spawn();
                break;
            case AIState.Despawn:
                AI_Despawn();
                break;
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Death:
                AI_Death();
                break;
            case AIState.Axe_Crash:
                AI_AxeCrash();
                break;
            case AIState.Rude_Buster:
                AI_RudeBuster();
                break;
            case AIState.Release_The_Bees:
                AI_ReleaseTheBees();
                break;
            case AIState.Call_The_Bees:
                AI_CallTheBees();
                break;
            case AIState.Splitting_Lightning:
                AI_SplittingLightning();
                break;
            case AIState.Splitting_Lightning_Crash:
                AI_SplittingLightningCrash();
                break;

        }
        _afterImageAlpha = MathHelper.Lerp(_afterImageAlpha, _targetAfterIamgeAlpha, 0.1f);
        _outliner.Update();
        this.SetDrawOrigin(new Vector2(170, 299));
        Lighting.AddLight(NPC.Center, TorchID.Torch);
    }
    void ChooseAttack()
    {
        var state = AttackPattern.NextPattern();
        SwitchState(state);
        SwitchState(AIState.Axe_Crash);
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            AttackCycle = 0;
            AttackCounter = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }
    private void AI_Spawn()
    {
        Timer++;
        if (Timer >= 90)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void AI_Despawn()
    {
        Timer++;
        if (Timer >= 90)
        {
            NPC.active = false;
        }
    }

    private void AI_Idle()
    {
        this.AseAnimator.PlayAnimation(ANIM_IDLE);
        NPC.SpriteFaceTarget();
        NPC.StayGroundedAndRooted();
        Timer++;
        if (Timer >= IdleTime)
        {
            ChooseAttack();
        }
    }

    private void AI_Death()
    {

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
