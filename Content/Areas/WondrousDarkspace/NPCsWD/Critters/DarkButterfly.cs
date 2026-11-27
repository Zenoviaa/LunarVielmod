
using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Core.NPCHelpers;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.NPCsWD.Critters;
public class DarkButterflyItem : ModItem
{

    public override string Texture => AssetReferences.Assets.Textures.Darkspace.Items.DarkButterflyItem.KEY;
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
        Item.makeNPC = ModContent.NPCType<DarkButterfly>();
        Item.value += Item.sellPrice(silver: 5); // Make this critter sell for 5 silver more than the normal frog
        Item.rare = ItemRarityID.Blue;
        Item.bait = 20;
    }
}


public class DarkButterfly : ModNPC
{
    const string ANIM_IDLE = "Idle";
    float _alpha;
    Vector2 _wanderPos;
    float Glow => ExtraMath.Osc(0.1f, 0.7f, offset: NPC.whoAmI);
    ref float Timer => ref NPC.ai[0];
    ref float WanderTimer => ref NPC.ai[1];
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
        writer.WriteVector2(_wanderPos);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _wanderPos = reader.ReadVector2();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.width = NPC.height = 16;
        NPC.lifeMax = 32;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.catchItem = ModContent.ItemType<DarkButterflyItem>();
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

    private void NewWanderPos()
    {
        _wanderPos = NPC.Center;
        _wanderPos.X += Main.rand.Next(-128, 128);
        _wanderPos.Y += Main.rand.Next(-8, 8);
    }

    public override void AI()
    {
        base.AI();
        _alpha = 1f;
        Timer++;
        Vector2 targetPos = _wanderPos;
        targetPos.X += MathF.Sin(Timer * 0.005f) * 9;
        targetPos.Y += MathF.Sin(Timer * 0.01f) * 9;
        Vector2 targetVelocity = targetPos - NPC.Center;
        targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
        float speed = 1f;
        targetVelocity *= speed;
        NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.03f);
        FaceMovement();
        WanderTimer--;
        if ((WanderTimer <= 0 || Vector2.DistanceSquared(NPC.Center, targetPos) < 64) && MultiplayerHelper.IsHost)
        {
            NewWanderPos();
            WanderTimer = 120;
            NPC.netUpdate = true;
        }

        //this.AseAnimator.PlayAnimation("Idle", AnimationParams.Default);
        NPC.PlayAnimationLooped(ANIM_IDLE);
        Lighting.AddLight(NPC.Center, Vector3.One * 0.4f);
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
        drawer.color = Color.SkyBlue * Glow * 0.2f * _alpha;
        drawer.color.A = 0;
        drawer.scale *= 0.25f;
        spriteBatch.Draw(drawer);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        DarkspaceEnemyCommon.HitAndDeathEffectsMini(NPC);
    }
}
