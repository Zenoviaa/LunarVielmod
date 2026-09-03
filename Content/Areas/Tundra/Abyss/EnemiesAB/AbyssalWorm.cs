
using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Core.NPCHelpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;

public class AbyssalWorm : ModNPC
{
    private float Glow => ExtraMath.Osc(0.1f, 0.4f, offset: NPC.whoAmI);
    private ref float Timer => ref NPC.ai[0];
    private ref float WanderTime => ref NPC.ai[1];
    private ref float WanderDirection => ref NPC.ai[2];
    private Player MyTarget => Main.player[NPC.target];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCSets.UseAseprite[Type] = true;
        this.AddToAbyss();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 40;
        NPC.height = 24;
        NPC.damage = 34;
        NPC.defense = 8;
        NPC.lifeMax = 50;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath41;
        NPC.aiStyle = -1;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    private void FaceMovement()
    {
        if (NPC.velocity.X < 0)
            NPC.spriteDirection = -1;
        else
            NPC.spriteDirection = 1;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (MultiplayerHelper.IsHost && Timer >= WanderTime)
        {
            Timer = 0;
            WanderTime = Main.rand.NextFloat(60, 180);
            WanderDirection = Main.rand.NextBool(2) ? -1 : 1;
            NPC.netUpdate = true;
        }

        if(Timer > 60)
        {
            float x = 0;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, x, 0.2f);
            this.AseAnimator.PlayAnimation("Idle", AnimationParams.Default);
        }
        else
        {
            float x = WanderDirection;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, x, 0.2f);
            this.AseAnimator.PlayAnimation("Walk", AnimationParams.Default);
            FaceMovement();
        }

        if (NPC.wet)
        {
            NPC.noGravity = true;
            NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, -0.5f, 0.1f);
        }
        else
        {
            NPC.noGravity = false;
        }
            
        this.AseAnimator.drawEffects.DrawOrigin = new Vector2(30, 25);

        float stepSpeed = 1;
        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref stepSpeed, ref NPC.gfxOffY);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);

        Color glowColor = Color.White * Glow;
        glowColor.A = 0;
        NPC.DrawAnimator(spriteBatch, glowColor);
        return false;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(spriteBatch, screenPos, drawColor);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        AbyssEnemyCommon.HitAndDeathEffects(NPC);
    }
}
