using Stellamod.Core;
using Stellamod.Core.NPCHelpers;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public partial class WarriorSTARR : ScarletBoss
{
    private enum AIState
    {
        Spawn,
        Despawn,
        Idle,
        BoulderKick,
        JumpRockSlam,
        WindUpPunch,
        FakeOutPunch,
        DiscThrow,
        Laugh,
        CapeOut,
        AnkleBreakerMaybe,
        CommandGrab,
        RockSpikeRun,
        Phase2Transition,
        Death
    }

    private bool _contactDamage;
    public override string Texture => TextureRegistry.EmptyTexture;
    private Outliner _outliner;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];

    public bool Phase2Active { get; private set; }
    private bool ShouldBeInPhase2
    {
        get
        {
            return NPC.life < NPC.lifeMax / 2f;
        }
    }
    private float IdleTime
    {
        get
        {
            if (Phase2Active)
            {
                return 120;
            }
            return 180;
        }
    }
    private bool _fakeOut;
    private bool _eyeFlash;
    private bool _jumpingTrail;
    private bool _grabbing;
    private int _grabbedPlayer;
    private Vector2 _vector1;
    private Vector2 _vector2;
    private const string ANIM_IDLE = "Idle";
    private const string ANIM_KICK_UP = "KickUp";
    private const string ANIM_KICK_DOWN = "KickDown";
    private const string ANIM_PUNCH_READY = "PunchReady";
    private const string ANIM_PUNCH = "Punch";
    private const string ANIM_CROUCH = "Crouch";
    private const string ANIM_PUNCH_DOWN_READY = "PunchDownReady";
    private const string ANIM_PUNCH_DOWN = "PunchDown";
    private const string ANIM_PUNCH_DOWN_OUT = "PunchDownOut";
    private const string ANIM_CAPE_OUT = "CapeOut";
    private const string ANIM_CAPE_IN = "CapeIn";
    private const string ANIM_DISC_THROW = "DiscThrow";
    private const string ANIM_RUN = "Run";
    private const string ANIM_SPIN_KICK = "SpinKick";
    private const string ANIM_SPIN_KICK_READY = "SpinKickReady";
    private const string ANIM_LOW_KICK = "LowKick";
    private const string ANIM_LOW_KICK_READY = "LowKickReady";
    private const string ANIM_GRAB_READY = "GrabReady";
    private const string ANIM_GRAB_START_SLOW = "GrabStartSlow";
    private const string ANIM_GRAB_TRY = "GrabTry";
    private const string ANIM_GRAB_THROW = "GrabThrow";
    private const string ANIM_GRAB_NO = "GrabNo";
    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write(Phase2Active);
        writer.WriteVector2(_vector1);
        writer.WriteVector2(_vector2);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        Phase2Active = reader.ReadBoolean();
        _vector1 = reader.ReadVector2();
        _vector2 = reader.ReadVector2();
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[NPC.type] = 1;
        NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCSets.UseAseprite[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 80;
        NPC.height = 80;
        NPC.damage = 100;
        NPC.defense = 23;
        NPC.lifeMax = 18000;

        NPC.scale = 1f;
        NPC.value = Item.buyPrice(gold: 5);
        NPC.knockBackResist = 0.04f;
        NPC.boss = true;
        NPC.npcSlots = 30f;

        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/ROYALSTARR");
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && (_contactDamage || _grabbing);
    }

    public override bool AllowNameplateToBeShown()
    {
        return State != AIState.Spawn && State != AIState.Despawn;
    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            AttackCycle = 0;
            AttackCounter = 0;
        }
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
        _fakeOut =false;
        _contactDamage = false;
        _jumpingTrail = false;
        _grabbing = false;
        _outliner.SetDefaults();
        switch (State)
        {
            case AIState.Despawn:
                AI_Despawn();
                break;
            case AIState.Spawn:
                AI_Spawn();
                break;
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.BoulderKick:
                AI_BoulderKick();
                break;
            case AIState.JumpRockSlam:
                AI_JumpRockSlam();
                break;
            case AIState.CapeOut:
                AI_CapeOut();
                break;
            case AIState.WindUpPunch:
                AI_WindUpPunch();
                break;
            case AIState.FakeOutPunch:
                _fakeOut = true;
                AI_WindUpPunch();
                break;
            case AIState.DiscThrow:
                AI_DiscThrow();
                break;
            case AIState.AnkleBreakerMaybe:
                AI_AnkleBreakerMaybe();
                break;
            case AIState.CommandGrab:
                AI_CommandGrab();
                break;
            case AIState.RockSpikeRun:
                AI_RockSpikeRun();
                break;
            case AIState.Phase2Transition:
                AI_Phase2Transition();
                break;
            case AIState.Death:
                AI_Death();
                break;
        }
        _outliner.Update();
    }

    private void AI_Phase2Transition()
    {
        Timer++;
        Phase2Active = true;
        NPC.velocity.X *= 0.96f;
        NPC.noGravity = false;
        if (Timer >= 120)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void AI_Death()
    {
        Timer++;
    }

    private void AI_Spawn()
    {
        StayGrounded();
        FaceTarget();
        this.AseAnimator.PlayAnimation(ANIM_IDLE);
        if(Timer >= 60)
        {
            SwitchState(AIState.Idle);
        }
    }

    private void AI_Despawn()
    {
        NPC.active = false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
    {
        base.OnHitPlayer(target, hurtInfo);
        if (_grabbing)
        {
            _grabbedPlayer = target.whoAmI;
            AttackCounter = 1;
            NPC.netUpdate = true;
        }
    }

    private void ChooseAttack()
    {
        if (MultiplayerHelper.IsHost)
        {
            SwitchState(AIState.BoulderKick);
        }
    }


    private void AI_Idle()
    {
        Timer++;
        NPC.velocity.X *= 0.96f;
        NPC.noGravity = false;
        this.AseAnimator.PlayAnimation(ANIM_IDLE, AnimationParams.Default);
        FaceTarget();
        if (Timer >= IdleTime)
        {
            ChooseAttack();
        }
        else if (ShouldBeInPhase2 && IsGrounded())
        {
            SwitchState(AIState.Phase2Transition);
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (NPC.life <= 0)
        {
            NPC.life = 1;
            if (State != AIState.Death)
                SwitchState(AIState.Death);
        }
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
        OutlineRenderer.Queue(DrawOutlineWhite);
        return false;
    }

    private void DrawOutlineWhite(SpriteBatch spriteBatch)
    {
        NPC.DrawAnimator(spriteBatch, _outliner.outlineColor);
    }
}
