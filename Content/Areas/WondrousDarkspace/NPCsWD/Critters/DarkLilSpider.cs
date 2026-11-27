using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Core.NPCHelpers;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.NPCsWD.Critters;

public class DarkLilSpiderItem : ModItem
{

    public override string Texture => AssetReferences.Assets.Textures.Darkspace.Items.DarkLilSpiderItem.KEY;
    public override void SetStaticDefaults()
    {
        ItemID.Sets.IsLavaBait[Type] = true; // While this item is not bait, this will require a lava bug net to catch.
        //1.4.5 i think
        //   ItemID.Sets.PlaceTileOnAltUse[Type] = true; // Places a critter anchor with right click
    }

    public override void SetDefaults()
    {
        // useStyle = 1;
        // autoReuse = true;
        // useTurn = true;
        // useAnimation = 15;
        // useTime = 10;
        // consumable = true;
        // makeNPC = 361;
        // noUseGraphic = true;
        // createTile = TileID.CritterAnchor;
        // value = Item.sellPrice(0, 0, 10); // Sell for 10 silver 

        // Cloning ItemID.Frog sets the preceding values
        Item.CloneDefaults(ItemID.Frog);
        Item.makeNPC = ModContent.NPCType<DarkLilSpider>();
        Item.value += Item.sellPrice(silver: 5); // Make this critter sell for 5 silver more than the normal frog
        Item.rare = ItemRarityID.Blue;
        Item.bait = 20;
    }
}

public class DarkLilSpider : ModNPC
{
    enum AIState : byte
    {
        Idle,
        Crawl
    }
    const string ANIM_IDLE = "Idle";
    const string ANIM_CRAWL = "Crawl";
    float _alpha;
    Vector2 _wanderVelocity;
    float Glow => ExtraMath.Osc(0.1f, 0.7f, offset: NPC.whoAmI);
    ref float Timer => ref NPC.ai[0];
    AIState State
    {
        get => (AIState)NPC.ai[1];
        set => NPC.ai[1] = (float)value;
    }
    float IdleTime => 90;
    float CrawlTime => ExtraMath.Osc(90, 120, 0, NPC.whoAmI);
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCSets.UseAseprite[Type] = true;
        NPC.StaticDefaultToCritter();
        this.AddToAbyssCritter();
        this.PreferLand();
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_wanderVelocity);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _wanderVelocity = reader.ReadVector2();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = NPC.height = 16;
        NPC.lifeMax = 32;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.aiStyle = -1;
        NPC.noGravity = false;
        NPC.catchItem = ModContent.ItemType<DarkLilSpiderItem>();
        NPC.lavaImmune = true;
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
        _alpha = 1f;

        switch (State)
        {
            case AIState.Idle:
                AI_Idle();
                break;
            case AIState.Crawl:
                AI_Crawl();
                break;
        }


        //this.AseAnimator.PlayAnimation("Idle", AnimationParams.Default);

        NPC.DrawOrigin = new Vector2(15, 18);
        Lighting.AddLight(NPC.Center, Vector3.One * 0.4f);
    }

    void SwitchState(AIState state)
    {
        Timer = 0;
        State = state;
        NPC.netUpdate = true;
    }
    void AI_Idle()
    {
        Timer++;
        NPC.velocity.X *= 0.96F;
        NPC.PlayAnimationLooped(ANIM_IDLE);
        if (Timer >= IdleTime)
        {
            SwitchState(AIState.Crawl);
        }
    }

    void AI_Crawl()
    {
        Timer++;
        if (Timer == 1)
        {
            if (MultiplayerHelper.IsHost)
            {
                _wanderVelocity.X = Main.rand.NextBool(2) ? -1 : 1;
                NPC.netUpdate = true;
            }
        }
        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, _wanderVelocity.X, 0.1f);
        NPC.PlayAnimationLooped(ANIM_CRAWL);
        NPC.QuickStepUp();
        FaceMovement();
        if (Timer >= CrawlTime)
        {
            SwitchState(AIState.Idle);
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        NPC.DrawAnimator(spriteBatch, drawColor * _alpha);

        Color glowColor = Color.White * Glow;
        glowColor.A = 0;
        NPC.DrawAnimator(spriteBatch, glowColor * _alpha);
        return false;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(spriteBatch, screenPos, drawColor);
        Texture2D glowCircle = AssetManager.GlowMask.SimpleGlowCircle.Value;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureWhite(glowCircle);
        drawer.worldPosition = NPC.Center;
        drawer.color = Color.White * Glow * 0.2f * _alpha;
        drawer.color.A = 0;
        drawer.scale *= 0.5f;
        spriteBatch.Draw(drawer);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        DarkspaceEnemyCommon.HitAndDeathEffectsMini(NPC);
    }
}
