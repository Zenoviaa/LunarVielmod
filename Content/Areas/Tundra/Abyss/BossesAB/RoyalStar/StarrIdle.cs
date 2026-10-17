using Stellamod.Common.Particles;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.Pixelation;
using Terraria;
using Terraria.Graphics.Light;
using Terraria.ID;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.RoyalStar;

public class StarrIdle : VeilTownNPC,
    IDrawToRenderTarget
{
    enum AIState : byte
    {
        Sleeping,
        Idle
    }
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }

    public override string Texture => TextureRegistry.EmptyTexture;
    const string ANIM_SLEEPING = "Sleeping";
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCID.Sets.UsesAseprite[Type] = true;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.friendly = true; // NPC Will not attack player
        NPC.width = 80;
        NPC.height = 36;
        NPC.aiStyle = -1;
        NPC.damage = 90;
        NPC.defense = 42;
        NPC.lifeMax = 2000;
        NPC.npcSlots = 0;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0f;
        NPC.dontTakeDamageFromHostiles = true;
        NPC.BossBar = Main.BigBossProgressBar.NeverValid;
    }
    public override void AI()
    {
        base.AI();
        switch (State)
        {
            case AIState.Sleeping:
                AI_Sleeping();
                break;
        }
        NPC.spriteDirection = -1;
        NPC.VectorScale = Vector2.Lerp(   Vector2.One, new Vector2(1.1f, 0.9f), ExtraMath.Osc(0f, 1f));
        NPC.DrawOrigin = new Vector2(121, 148);
        Lighting.AddLight(NPC.Center, TorchID.Torch);
    }
    public override bool CheckActive()
    {
        return false;
    }
    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
        NPC.netUpdate = true;
    }
    void AI_Sleeping()
    {
        Timer++;
        NPC.PlayAnimationLooped(ANIM_SLEEPING);
        if(Timer % 100 == 0)
        {
            Particles.FlyingZee.Spawn(new()
            {
                position = NPC.TopRight,
                velocity = Vector2.Zero,
                timeLeft = 240
            });
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
       // return base.PreDraw(spriteBatch, screenPos, drawColor);
    }

    public void DrawToRenderTargets()
    {
      
    }
}
