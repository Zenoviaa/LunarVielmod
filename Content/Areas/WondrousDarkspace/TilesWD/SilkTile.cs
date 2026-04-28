using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items.Materials;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.TilesWD;

public class SilkGlobalTile : GlobalTile
{
    public override void RandomUpdate(int i, int j, int type)
    {
        base.RandomUpdate(i, j, type);
        if (type == ModContent.TileType<SilkTile>())
        {
            if (Main.rand.NextBool(16))
            {
                WorldGen.PlaceTile(i, j, ModContent.TileType<MiracleSilkTile>(), mute: false, forced: true);
                if(Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendTileSquare(-1, i, j, TileChangeType.None);
                }
            }
        }
    }
}

public class SilkTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;
        RegisterItemDrop(ModContent.ItemType<SilkTileBlock>());
        // DustType = Main.rand.Next(110, 113);

        MineResist = 1f;
        MinPick = 50;

        AddMapEntry(Color.LightGray);

        // TODO: implement
        // SetModTree(new Trees.ExampleTree());
    }
    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
    }
    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
    }
}

[Autoload(Side = ModSide.Client)]
public class MiracleSilkRenderer : ModSystem
{
    public override void Load()
    {
        base.Load();
        On_Main.DrawDust += DrawSilk;
    }

    private void DrawSilk(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        PixelationManager.QueueSpritebatchDrawAction(DrawSilkStrands);


 //       throw new NotImplementedException();
    }

    private void DrawSilkStrands(SpriteBatch sb, Vector2 screenPos)
    {
        (Point topLeft, Point bottomRight) = TileUtilities.CameraTileBounds(256);
        MiracleSilkTile miracleSilkTile = ModContent.GetInstance<MiracleSilkTile>();

        for (int x = topLeft.X; x < bottomRight.X; x++)
        {
            for (int y = topLeft.Y; y < bottomRight.Y; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.TileType != miracleSilkTile.Type)
                    continue;
                miracleSilkTile.DrawString(x, y, Main.spriteBatch);
              //  miracleSilkTile.MakeDust(x, y);
            }
        }
    }
}
public class MiracleSilkTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;
        RegisterItemDrop(ModContent.ItemType<SilkTileBlock>());
        // DustType = Main.rand.Next(110, 113);

        MineResist = 1f;
        MinPick = 50;

        AddMapEntry(Color.LightGray);

        // TODO: implement
        // SetModTree(new Trees.ExampleTree());
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
    }

    public Point GetConnectedTile(int i, int j)
    {
        if (WorldGen.SolidTile(i, j - 1))
        {
            Point fallingPoint = new Point(i, j + 2);
            fallingPoint.X += (int)ExtraMath.Osc(-3f, 3f, 0f, i);
            fallingPoint = TileUtilities.FallToSolidTile(fallingPoint);
            return fallingPoint;
        }
        else
        {
            Point risingPoint = new Point(i, j - 6);
            risingPoint.X += (int)ExtraMath.Osc(-3f, 3f, 0f, i);
            risingPoint = TileUtilities.FallToSolidTile(risingPoint.X, risingPoint.Y, direction: -1);
            return risingPoint;
        }
    }

    private Vector2[] GetTrail(Point tile1, Point tile2)
    {
        Vector2 drawPos1 = tile1.ToWorldCoordinates();
        Vector2 drawPos2 = tile2.ToWorldCoordinates();
        Vector2[] trailPoints = new Vector2[2];
        trailPoints[0] = drawPos1;
        trailPoints[1] = drawPos2;
        MathUtil.LerpTrailPoints(trailPoints, out Vector2[] trailingPoints, smoothFactor: 128);
        return trailingPoints;
    }

    public float GetWidth(float completionRatio)
    {
        float width = 2f;
        float startWidth = width * 64;
        float midWidth = width * 16;
        float ease = EasingFunction.QuadraticBump(completionRatio);
        return MathHelper.Lerp(startWidth, midWidth, ease);
    }

    public float GetBloomWidth(float completionRatio)
    {
        return GetWidth(completionRatio) * 1.3f;
    }
    public Color GetColor(float completionRatio)
    {
        return Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(completionRatio));
    }

    public void MakeDust(int i, int j)
    {
        Point start = new Point(i, j);
        Point end = GetConnectedTile(i, j);

        Vector2 tile1 = start.ToWorldCoordinates();
        Vector2 tile2 = end.ToWorldCoordinates();
        Vector2 dustTile = Vector2.Lerp(tile1, tile2, Main.rand.NextFloat(0f, 1f));
        if (!Main.rand.NextBool(16))
            return;

        var sp = SparkleParticle.Spawn(dustTile, Vector2.Zero, Scale: 0.6f);
        sp.innerColor = Color.White;
        sp.outerColor = Main.DiscoColor;
    }

    public void DrawString(int i, int j, SpriteBatch spriteBatch)
    {

        SimpleTrailShader trailShader = SimpleTrailShader.Instance;
        trailShader.TrailingTexture = TrailRegistry.SilkTrail;
        trailShader.SecondaryTrailingTexture = TrailRegistry.StarTrail;
        trailShader.TertiaryTrailingTexture = TrailRegistry.SilkTrail;
        trailShader.BlendState = BlendState.AlphaBlend;

        Point start = new Point(i, j);
        Point end = GetConnectedTile(i, j);

        Color lightColor = Lighting.GetColor(start.X, end.Y);

        Color rgbColor = Color.Lerp(Color.White, Color.Pink, MathUtil.Osc(0f, 1f, speed: 1));
    //    rgbColor = rgbColor.MultiplyRGB(lightColor);
        trailShader.PrimaryColor = rgbColor;
        trailShader.SecondaryColor = rgbColor * 1f;

        Vector2[] trailingPoints = GetTrail(start, end);
        TrailDrawer.Draw(spriteBatch, trailingPoints, null, GetColor, GetWidth, trailShader);



        BloomTrailShader bloomTrailShader = BloomTrailShader.Instance;
        bloomTrailShader.InnerColor = Color.Lerp(Color.White, Color.Purple, ExtraMath.Osc(0f, 1f, speed: 1f)) * 0.5f;
        bloomTrailShader.OuterColor = Color.Lerp(Color.Pink, Color.Purple, ExtraMath.Osc(0f, 1f, speed: 1)) * 0.5f;
        TrailDrawer.Draw(spriteBatch, trailingPoints, null, GetColor, GetBloomWidth, bloomTrailShader);



        Asset<Texture2D> silkEnd = TrailRegistry.SilkEnd;
        Vector2 startPoint = start.ToWorldCoordinates();
        Vector2 endPoint = end.ToWorldCoordinates();

        float drawRotation = (endPoint - startPoint).ToRotation();
        Vector2 drawPoint = startPoint - Main.screenPosition;
        Color drawColor = Color.White.MultiplyRGB(lightColor) * 0.75f;
        drawColor.A = 0;
        Vector2 origin = silkEnd.Size() / 2f;
        Vector2 drawScale = Vector2.One;


     //   spriteBatch.Draw(silkEnd.Value, drawPoint, null, drawColor, drawRotation, origin, drawScale, SpriteEffects.None, 0);

        Vector2 drawPoint2 = endPoint - Main.screenPosition;
        drawPoint2 += (startPoint - endPoint).SafeNormalize(Vector2.Zero) * 32;
        float drawRotation2 = (startPoint - endPoint).ToRotation();
  //      spriteBatch.Draw(silkEnd.Value, drawPoint2, null, drawColor, drawRotation2, origin, drawScale, SpriteEffects.None, 0);
    }
    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {

        return base.PreDraw(i, j, spriteBatch);
    }
    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
        if (!fail)
        {
            int numThreads = Main.rand.Next(3, 8);
            for (int n = 0; n < numThreads; n++)
            {
                Point start = new Point(i, j);
                Point end = GetConnectedTile(i, j);

                Vector2 tile1 = start.ToWorldCoordinates();
                Vector2 tile2 = end.ToWorldCoordinates();
                Vector2 point = Vector2.Lerp(tile1, tile2, Main.rand.NextFloat(0f, 1f));

                int itemIndex = Item.NewItem(new EntitySource_TileBreak(i, j), point,
                          ModContent.ItemType<MiracleThread>(), 1);
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 1f);

                for (int s = 0; s < 15; s++)
                {
                    Vector2 spawnPoint = point + Main.rand.NextVector2Circular(32, 32);
                    LegacyParticle.NewParticle<SilkParticle>(spawnPoint, Vector2.Zero, Color.Transparent);
                }
            }
        }

    }
}
public class SilkTileBlock : ModItem
{
    public override void SetStaticDefaults()
    {
        // Tooltip.SetDefault("Super silk!");
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<SilkTile>());
    }
}
public class MiracleSilkTileBlock : ModItem
{
    public override void SetStaticDefaults()
    {
        // Tooltip.SetDefault("Super silk!");
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

    }

    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<MiracleSilkTile>());
    }
}
