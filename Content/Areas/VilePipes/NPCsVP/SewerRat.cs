using Stellamod.Common;
using Stellamod.Core.Pixelation;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.VilePipes.NPCsVP;

public class SewerRat : ModNPC,
    IDrawToRenderTarget
{
    enum AIState : byte
    {
        Idle,
        Patrol,
        Chase,
        Backstep,
        JumpReady,
        Jump,
        Land,
        Bite,
    }
    Outliner _outliner;
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    public override string Texture => TextureRegistry.EmptyTexture;
    const string ANIM_IDLE = "Idle";
    const string ANIM_RUN = "Run";
    const string ANIM_BACKSTEP = "Backstep";
    const string ANIM_JUMPREADY = "JumpReady";
    const string ANIM_JMPUP = "JumpUp";
    const string ANIM_LAND = "Land";
    const string ANIM_BITEUP = "BiteUp";
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
        this.AddToVilePipes();
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
        NPC.spriteDirection = NPC.direction;
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);

    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
    }
    public override void OnKill()
    {
        base.OnKill();

    }

    public void DrawToRenderTargets()
    {
        NPC.DrawOutlineByAnimator(_outliner);
    }
}
