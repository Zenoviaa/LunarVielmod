using Stellamod.Common;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.NPCsSH;

public class IvynSpearman : ModNPC,
    IDrawToRenderTarget
{
    Outliner _outliner;
    enum AIState : byte
    {
        Idle,
        Patrol,
        Case,
        Crouch,
        Jump,
        Fall,
        Slam,
        Pullout,
        EdgeFall,
    }
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    ref float AttackCycle => ref NPC.ai[2];

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
        return base.CanHitPlayer(target, ref cooldownSlot);
    }
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 0;
        this.AddToSpringHills();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.damage = 10;
        NPC.width = 58;
        NPC.height = 66;
        NPC.lifeMax = 25;
        NPC.defense = 3;
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

        }
        _outliner.Update();
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
        //return base.PreDraw(spriteBatch, screenPos, drawColor);
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
    }

    public void DrawToRenderTargets()
    {
        NPC.DrawOutlineByAnimator(_outliner);
    }
}
