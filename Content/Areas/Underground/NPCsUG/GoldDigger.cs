using Stellamod.Assets.ContentReader.Aseprite;
using Stellamod.Core.NPCHelpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.NPCsUG;

public class GoldDigger : ModNPC
{
    private ref float Timer => ref NPC.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCSets.UseAseprite[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 34;
        NPC.height = 40;
        NPC.aiStyle = NPCAIStyleID.Fighter;
        NPC.damage = 34;
        NPC.defense = 8;
        NPC.lifeMax = 140;
        NPC.HitSound = SoundID.NPCHit48;
        NPC.DeathSound = SoundID.DD2_SkeletonDeath;
        NPC.value = 563f;
        NPC.knockBackResist = .45f;
        NPC.aiStyle = NPCAIStyleID.Fighter;
        AIType = NPCID.SnowFlinx;

    }

    public override void AI()
    {
        base.AI();
        this.GetAnimator().PlayAnimation("Run");
        this.SetDrawOrigin(new Vector2(34, 32));
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        OutlineRenderer.Queue(DrawWhite);
        return base.PreDraw(spriteBatch, screenPos, drawColor);
    }

    private void DrawWhite(SpriteBatch spriteBatch)
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
