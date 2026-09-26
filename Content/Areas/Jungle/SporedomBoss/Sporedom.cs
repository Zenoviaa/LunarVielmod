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

namespace Stellamod.Content.Areas.Jungle.SporedomBoss;

public class Sporedom : ScarletBoss
{
    private enum AIState
    {
        Spawn,
        Idle,
        Despawn,
        Death,
        BigSpit,
        BouncingBalls,
        SproutBoom
    }

    private bool _contactDamage;
    private Outliner _outliner;
    private ref float Timer => ref NPC.ai[0];
    private AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    private ref float AttackCycle => ref NPC.ai[2];

    private PatternManager<AIState> PatternManager
    {
        get
        {
            if(field == null)
            {
                field = new();
                field.AddPattern(AIState.BigSpit, 1);
                field.AddPattern(AIState.BouncingBalls, 1);
                field.AddPattern(AIState.SproutBoom, 1);
            }
            return field;
        }
    }
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCSets.UseAseprite[Type] = true;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 128;
        NPC.height = 128;
        NPC.damage = 90;
        NPC.defense = 24;
        NPC.lifeMax = 8000;

        NPC.knockBackResist = 0f;
        NPC.boss = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.npcSlots = 30f;
        NPC.behindTiles = true;

        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/ViciousFoe");
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
    }

    public override void AI()
    {
        base.AI();
        _outliner.SetDefaults();
        _contactDamage = false;
        switch (State)
        {
            case AIState.Spawn:
                AI_Spawn();
                break;
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Despawn:
                AI_Despawn();
                break;
            case AIState.BigSpit:
                AI_BigSpit();
                break;
            case AIState.BouncingBalls:
                AI_BouncingBalls();
                break;
            case AIState.SproutBoom:
                AI_SproutBoom();
                break;
        }
        if (_contactDamage)
            _outliner.attacking = true;
        _outliner.Update();

    }

    private void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            AttackCycle = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    private void ChooseAttack()
    {
        var pattern = PatternManager.NextPattern();
        SwitchState(pattern);
    }

    private void AI_Spawn()
    {

    }
    private void AI_Idle()
    {

    }
    private void AI_Despawn()
    {

    }
    private void AI_BigSpit()
    {

    }
    private void AI_BouncingBalls()
    {

    }
    private void AI_SproutBoom()
    {

    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (NPC.life <= 0)
        {
            NPC.life = 1;
        }
        if (NPC.life <= 1 && State != AIState.Death)
        {
            SwitchState(AIState.Death);
        }
    }
}

