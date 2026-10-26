using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Ambient;

public abstract class ABreakableNPC : ModNPC
{
    float WiggleTime => 60;
    float WiggleRange => 0.15f;
    float _wiggle;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.ImmuneToAllBuffs[Type] = true;
        NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.lifeMax = 5;
        NPC.defense = 99999;
        NPC.width = 64;
        NPC.height = 32;
        NPC.damage = 1;
        NPC.knockBackResist = 0;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        if(_wiggle > 0)
            _wiggle--;
        NPC.rotation = MathHelper.Lerp(-WiggleRange, WiggleRange, MathF.Sin(_wiggle * 0.3f) * 0.5f + 0.5f) * _wiggle / WiggleTime;
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        _wiggle = WiggleTime;
    }

    public override void OnKill()
    {
        base.OnKill();
    }
}
