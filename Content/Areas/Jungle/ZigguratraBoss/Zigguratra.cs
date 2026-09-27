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
    private void AI_Spawn()
    {

    }
    
    private void AI_Despawn()
    {

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

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        return base.PreDraw(spriteBatch, screenPos, drawColor);
    }

    public override void OnKill()
    {
        base.OnKill();
    }
}
