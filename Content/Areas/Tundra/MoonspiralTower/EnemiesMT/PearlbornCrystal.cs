using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.EnemiesMT;

public class PearlbornCrystal : ModNPC
{
    enum AIState : byte
    {
        Idle,
        PickedUp,
        Throw
    }
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    Player PickedUpTarget
    {
        get => Main.player[(int)NPC.ai[2]];
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = NPC.height = 32;
        NPC.damage = 1;
        NPC.defense = 9999;
        NPC.lifeMax = 15;

    }

    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.PickedUp:
                AI_PickedUp();
                break;
        }

        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
    }
    void SwitchState(AIState state)
    {
        if (MultiplayerHelper.IsHost)
        {
            Timer = 0;
            State = state;
            NPC.netUpdate = true;
        }
    }

    void AI_Idle()
    {
        Timer++;
        NPC.velocity.X *= 0.94f;
    }

    void AI_PickedUp()
    {
        Timer++;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var drawer = SpritebatchDrawer.FromNPC(NPC);
        spriteBatch.Draw(drawer);
        return false;
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

