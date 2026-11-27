
using Stellamod.Assets;
using Stellamod.Common;
using Stellamod.Core.NPCHelpers;
using Stellamod.WorldG;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.NPCsWD.Critters;

public class DarkGlowflyItem : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Darkspace.Items.DarkGlowflyItem.KEY;
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
        Item.makeNPC = ModContent.NPCType<DarkGlowFly>();
        Item.value += Item.sellPrice(silver: 5); // Make this critter sell for 5 silver more than the normal frog
        Item.rare = ItemRarityID.Blue;
        Item.bait = 20;
    }
}
public class DarkGlowFly : ModNPC
{
    const string ANIM_IDLE = "Idle";
    float _alpha;
    Vector2 _lurkDirection;
    float Glow => ExtraMath.Osc(0.1f, 0.7f, offset: NPC.whoAmI);
    ref float Timer => ref NPC.ai[0];
    ref float WanderTimer => ref NPC.ai[1];
    ref float MovementCycle => ref NPC.ai[2];

    public float TopPatrolRange => SavedGenerationParameters.GetDarkspaceTop(NPC.Center.X).ToWorldCoordinates().Y;
    public float BottomPatrolRange => SavedGenerationParameters.GetDarkspaceBottom(NPC.Center.X).ToWorldCoordinates().Y;
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        NPCSets.UseAseprite[Type] = true;
        NPC.StaticDefaultToCritter();
        this.AddToDarkspaceCritter();
        this.PreferLand();
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.WriteVector2(_lurkDirection);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        _lurkDirection = reader.ReadVector2();
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
        NPC.catchItem = ModContent.ItemType<DarkGlowflyItem>();
        NPC.lavaImmune = true;
        NPC.noTileCollide = true;
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


    float To(float start, float end)
    {
        if (end < start)
            return -1;
        else
            return 1;
    }
    float Dist(float start, float end)
    {
        return MathF.Abs(end - start);
    }

    public override void AI()
    {
        base.AI();
        _alpha = 1f;
        Timer++;
        if(_lurkDirection == Vector2.Zero && MultiplayerHelper.IsHost)
        {
            _lurkDirection.X = Main.rand.NextBool(2) ? -1 : 1;
            NPC.netUpdate = true;
        }
        Vector2 targetVelocity = Vector2.Zero;
        FaceMovement();

        switch (MovementCycle)
        {
            case 0:
                {
                    targetVelocity = _lurkDirection * 1.4F;
                    targetVelocity.Y = To(NPC.Center.Y, TopPatrolRange) * 3;
                    if (Dist(NPC.Center.Y, TopPatrolRange) <= 50)
                    {
                        MovementCycle = 1;
                    }

                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.02f);
                }
                break;
            case 1:
                {
                    targetVelocity = _lurkDirection * 1.4F;
                    targetVelocity.Y = To(NPC.Center.Y, BottomPatrolRange) * 3;
                    if (Dist(NPC.Center.Y, BottomPatrolRange) <= 50)
                    {
                        MovementCycle = 0;
                    }

                    NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity, 0.02f);
                }
                break;
        }


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
