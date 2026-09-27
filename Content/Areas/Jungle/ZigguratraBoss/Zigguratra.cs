using Stellamod.Core;
using Stellamod.Core.NPCHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }

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
            if(field == null)
            {
                field = new();
                field.AddPattern(AIState.Axe_Crash, 1f);
            }

            return field;
        }
    }

    private const string ANIM_AXE_WALK = "AxeWalk";
    private const string ANIM_AXE_READY = "AxeReady";
    private const string ANIM_AXE_SLAM = "AxeSlam";

    private ref float AttackCycle => ref NPC.ai[2];
    private ref float AttackCounter => ref NPC.ai[3];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCSets.UseAseprite[Type] = true;
        NPCID.Sets.TrailCacheLength[Type] = 32;
        NPCID.Sets.TrailingMode[Type] = 3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 128;
        NPC.height = 108;
        NPC.damage = 50;
        NPC.defense = 5;
        NPC.lifeMax = 2200;
        NPC.knockBackResist = 0f;

        NPC.boss = true;
        NPC.npcSlots = 30;

        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/VisciousFoe");
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

        _outliner.SetDefaults();
        _version2 = false;
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
           
        }
        _outliner.Update();
    }
    void ChooseAttack()
    {

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
