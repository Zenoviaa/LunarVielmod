
using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Core.NPCHelpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;

public class AbyssUrchin : ModNPC
{
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
        NPC.height = 37;
        NPC.damage = 34;
        NPC.defense = 8;
        NPC.lifeMax = 140;
        NPC.HitSound = SoundID.NPCHit38;
        NPC.DeathSound = SoundID.NPCDeath41;
        NPC.aiStyle = -1;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
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

        float x = WanderDirection * 0.14f;
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, x, 0.2f);
        this.AseAnimator.PlayAnimation("Idle", AnimationParams.Default);
        this.AseAnimator.drawEffects.DrawOrigin = new Vector2(29, 40);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor);
        return false;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(spriteBatch, screenPos, drawColor);
        Texture2D glowCircle = AssetManager.GlowMask.SimpleGlowCircle.Value;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(glowCircle, NPC.Center);
        drawer.color = Color.White * ExtraMath.Osc(0.5f, 1f, speed: 3) * 0.2f;
        drawer.color.A = 0;
        drawer.scale *= 0.5f;
        spriteBatch.Draw(drawer);
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (!spawnInfo.Water)
            return 0;
        return base.SpawnChance(spawnInfo);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        AbyssEnemyCommon.HitAndDeathEffects(NPC);
    }
}
