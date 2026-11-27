using ReLogic.Content;
using Stellamod.Common.Particles;
using Stellamod.Content.Ambient;
using Stellamod.Content.Dusts;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Content.Areas.CrystalLake.TilesCL;

public class RosyQuartzCrystal : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.CrystalLake.Items.RosyQuartzCrystal.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
}

public abstract class ARosyQuartz : ModTile
{
    public abstract Asset<Texture2D> GlowTextureAsset { get; }
    public abstract TileObjectData BaseTileObjectData { get; }
    public override void SetStaticDefaults()
    {
        // Properties
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;


        AdjTiles = [TileID.Dressers];
        DustType = ModContent.DustType<Sparkle>();

        // Names
        AddMapEntry(new Color(231, 179, 255), CreateMapEntryName());

        // Placement
        TileObjectData.newTile.CopyFrom(BaseTileObjectData);
        TileObjectData.newTile.AnchorInvalidTiles = [
            TileID.MagicalIceBlock,
                TileID.Boulder,
                TileID.BouncyBoulder,
                TileID.LifeCrystalBoulder,
                TileID.RollingCactus
        ];
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.addTile(Type);
        RegisterItemDrop(ModContent.ItemType<RosyQuartzCrystal>());
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        base.PostDraw(i, j, spriteBatch);
        var tex = GlowTextureAsset;
        Vector2 pos = (new Vector2(i, j)) * 16;
        pos += new Vector2(Main.offScreenRange);
        Color color = Lighting.GetColor(i, j);

        Tile tile = Framing.GetTileSafely(i, j);

        Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
        Color glowColor = Color.Lerp(Color.White, Color.Gray, ExtraMath.Osc(0f, 1f));
        glowColor.A = 0;
        spriteBatch.Draw(tex.Value, pos - Main.screenPosition, frame, glowColor, 0, Vector2.Zero, 1, 0, 1);
    }
}

public class RosyQuartzBig : ABreakableNPC
{
    public override string Texture => AssetReferences.Assets.Textures.CrystalLake.Tiles.RosyQuartzBig.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        NPC.lifeMax = 9;
        NPC.HitSound = SoundID.DD2_CrystalCartImpact;
        NPC.DeathSound = SoundID.DD2_WitherBeastCrystalImpact;
    }

    public override void PostAI()
    {
        base.PostAI();
        if (MoonUtils.Rand.NextBool(8))
        {
            var pos = NPC.Center;
            pos += Main.rand.NextVector2Circular(64, 64);
            Dust.NewDustPerfect(pos, DustID.GemAmethyst, Main.rand.NextVector2Circular(0.5f, 0.5f), Scale: 0.4f);
        }

        Lighting.AddLight(NPC.Center, new Vector3(0.5F));
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        base.ModifyNPCLoot(npcLoot);
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RosyQuartzCrystal>(), 1, 4, 5));
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        var drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.VerticalFrame(0, Main.npcFrameCount[Type]);
        drawer.BottomCenterOrigin();
        drawer.worldPosition = NPC.Bottom;
        spriteBatch.Draw(drawer);
        return false;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        base.PostDraw(spriteBatch, screenPos, drawColor);
        var drawer = SpritebatchDrawer.FromNPC(NPC);
        drawer.texture = AssetReferences.Assets.Textures.CrystalLake.Tiles.RosyQuartzBig_Glow.Asset.Value;
        drawer.VerticalFrame(0, Main.npcFrameCount[Type]);
        drawer.BottomCenterOrigin();
        drawer.worldPosition = NPC.Bottom;
        spriteBatch.Draw(drawer);
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        base.HitEffect(hit);
        if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
        {
            var fac = ParticleUtils.ParticleFactory.FromSmallBurst(NPC.Center, TriColorPalette.Rosy, new Vector2(7, 15));
            ParticleUtils.CreateSwirlingDustCircle(fac);

            var pos = NPC.Center;
            var vel = -Vector2.UnitY;
            PS.SmokeBoom.Spawn(new()
            {
                position = pos,
                velocity = vel,
                rot = MoonUtils.Rand.NextFloat(0, 6),
                color = Color.White
            });
        }
    }
}

public class RosyQuartzSmall1 : ARosyQuartz
{
    public override string Texture => AssetReferences.Assets.Textures.CrystalLake.Tiles.RosyQuartzSmall1.KEY;
    public override Asset<Texture2D> GlowTextureAsset => AssetReferences.Assets.Textures.CrystalLake.Tiles.RosyQuartzSmall1_Glow.Asset;
    public override TileObjectData BaseTileObjectData => TileObjectData.Style3x2;
}

public class RosyQuartzSmall2 : ARosyQuartz
{
    public override string Texture => AssetReferences.Assets.Textures.CrystalLake.Tiles.RosyQuartzSmall2.KEY;
    public override Asset<Texture2D> GlowTextureAsset => AssetReferences.Assets.Textures.CrystalLake.Tiles.RosyQuartzSmall2_Glow.Asset;
    public override TileObjectData BaseTileObjectData => TileObjectData.Style3x3;
}

public class RosyQuartzSmall3 : ARosyQuartz
{
    public override string Texture => AssetReferences.Assets.Textures.CrystalLake.Tiles.RosyQuartzSmall3.KEY;
    public override Asset<Texture2D> GlowTextureAsset => AssetReferences.Assets.Textures.CrystalLake.Tiles.RosyQuartzSmall3_Glow.Asset;
    public override TileObjectData BaseTileObjectData => TileObjectData.Style3x2;
}
