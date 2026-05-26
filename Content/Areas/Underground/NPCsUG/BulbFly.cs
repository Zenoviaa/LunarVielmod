using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Content.CommonMaterials;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.NPCsUG;

public class BulbFly : ModNPC
{
    private int _frame;
    private ref float Timer => ref NPC.ai[0];
    private ref float Dir => ref NPC.ai[1];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 3;
        NPCID.Sets.TrailCacheLength[Type] = 16;
        NPCID.Sets.TrailingMode[Type] = 3;
        this.AddToHeatedDepths();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = 16;
        NPC.height = 16;
        NPC.lifeMax = 20;
        NPC.noGravity = true;
        NPC.noTileCollide = false;
        NPC.defense = 4;
        NPC.HitSound = SoundID.NPCHit15;
        NPC.DeathSound = SoundID.NPCDeath11;
    }

    public override void FindFrame(int frameHeight)
    {
        base.FindFrame(frameHeight);
        NPC.frameCounter += 0.2f;
        if(NPC.frameCounter >= 1f)
        {
            NPC.frameCounter = 0;
            _frame++;
            _frame %= Main.npcFrameCount[Type];
        }
        NPC.frame.Y = frameHeight * _frame;
    }

    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    {
        return false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer >= 200)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                Dir = Main.rand.NextFloat(-2f, 2f);
                NPC.netUpdate = true;
            }
        }
        NPC.rotation = NPC.velocity.X * 0.04f;
        Lighting.AddLight(NPC.position, TorchID.Torch);
        if (Main.rand.NextBool(32))
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch);
        }

        NPC.velocity = Dir.ToRotationVector2() * 0.3f;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        SpritebatchDrawer glowFlyDrawer = SpritebatchDrawer.FromNPC(NPC);
        spriteBatch.Draw(glowFlyDrawer);

        Asset<Texture2D> glowCircle = AssetManager.GlowMask.SimpleGlowCircle;
        SpritebatchDrawer glowCircleDrawer = SpritebatchDrawer.FromTextureAsset(glowCircle, NPC.Center);
        glowCircleDrawer.color = Color.Red * 0.4f;
        glowCircleDrawer.color.A = 0;
        glowCircleDrawer.scale *= 0.33f * ExtraMath.Osc(0.9f, 1f, speed: 2, offset: NPC.whoAmI);
        spriteBatch.Draw(glowCircleDrawer);
        return false;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AlcadizScrap>(), 1, minimumDropped: 5, maximumDropped: 6));
    }

    public override void OnKill()
    {
        base.OnKill();

        FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.Yellow, Color.Red, 25, baseSize: 0.13f);
        FXUtil.GlowCircleBoom(NPC.Center, Color.White, Color.Yellow, Color.Red, 45, baseSize: 0.2f);
        float numDust = 16;
        for(float n = 0; n < numDust; n++)
        {
            var dp = DustParticle.Spawn(NPC.Center, Main.rand.NextVector2Circular(9, 9));
            dp.outerColor = Color.Red;
            dp.Scale *= 0.5f;
        }
    }
}
