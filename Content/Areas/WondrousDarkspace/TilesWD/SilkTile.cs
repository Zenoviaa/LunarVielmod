using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Effects.Darkspace;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Xml;
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

        if(i <= 2 || i > Main.maxTilesX - 2 || j <= 2 || j >= Main.maxTilesY - 2)
        {
            return;
        }

        if (type == ModContent.TileType<SilkTile>() && WorldGen.TileIsExposedToAir(i, j))
        {
            if (Main.rand.NextBool(128))
            {
                WorldGen.PlaceTile(i, j, ModContent.TileType<MiracleSilkTile>(), mute: false, forced: true);
                if(Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendTileSquare(-1, i, j, TileChangeType.None);
                }
            }
        }
        if (type == TileID.Granite)
        {
            if (Main.rand.NextBool(2048))
            {
                WorldGen.PlaceTile(i, j, ModContent.TileType<SilkTile>(), mute: false, forced: true);
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendTileSquare(-1, i, j, TileChangeType.None);
                }
            }

            bool checkAgain = false;
            for(int x = -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    Tile tile = Main.tile[i + x, j + y];
                    if(tile.TileType == ModContent.TileType<SilkTile>())
                    {
                        checkAgain = true;
                        break;
                    }
                }
            }
            if (Main.rand.NextBool(128) && checkAgain)
            {
                WorldGen.PlaceTile(i, j, ModContent.TileType<SilkTile>(), mute: false, forced: true);
                if (Main.netMode != NetmodeID.SinglePlayer)
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
    public static float renderSilk;
    public override void Load()
    {
        base.Load();
        On_Main.DrawDust += DrawSilk;
    }

    private void DrawSilk(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        renderSilk--;
        if (renderSilk > 0 || Main.LocalPlayer.GetModPlayer<MyPlayer>().ZoneWonder) 
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawSilkStrands, DrawLayer.OverPlayers);
        }
    }

    private void DrawSilkStrands(SpriteBatch sb, Vector2 screenPos)
    {
        (Point topLeft, Point bottomRight) = TileUtilities.CameraTileBounds(2048);
        MiracleSilkTile miracleSilkTile = ModContent.GetInstance<MiracleSilkTile>();
        Color rgbColor = Color.Lerp(Color.DarkViolet, Color.Pink, MathUtil.Osc(0.5f, 1f, speed: 1));
        SilkStrandShader strandShader = SilkStrandShader.Instance;
        strandShader.Time = Main.GlobalTimeWrappedHourly * 1.3f;
        strandShader.BloomColor = rgbColor * 1f;
        strandShader.SilkTexture = TrailRegistry.BeamTrail.Value;
        var start = SpritebatchParams.InWorldAndZoomed();
        start.effect = strandShader.Effect;
        start.samplerState = SamplerState.AnisotropicClamp;
        sb.End();
        sb.Begin(start);
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
        for (int x = topLeft.X; x < bottomRight.X; x++)
        {
            for (int y = topLeft.Y; y < bottomRight.Y; y++)
            {
                Tile tile = Main.tile[x, y];
                if (tile.TileType != miracleSilkTile.Type)
                    continue;
                miracleSilkTile.DrawStringBig(x, y, Main.spriteBatch);
                //  miracleSilkTile.MakeDust(x, y);
            }
        }
        sb.End();
        sb.Begin(SpritebatchParams.InWorldAndZoomed());
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

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        MiracleSilkRenderer.renderSilk = 30;
        return base.PreDraw(i, j, spriteBatch);
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
        Point start = new Point(i, j);
        Point end = GetConnectedTile(i, j);
        Color lightColor = Lighting.GetColor(start.X, end.Y);


        Vector2 startWorld = start.ToWorldCoordinates();
        Vector2 endWorld = end.ToWorldCoordinates();
        Vector2 center = startWorld + endWorld;
        center *= 0.5f;
        float rot = (endWorld - startWorld).ToRotation();


        var trail = AssetManager.LaserTextures.SilkStrand;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(trail, center);
        drawer.rotation = rot;
        drawer.scale.X = Vector2.Distance(endWorld, startWorld) / (float)trail.Width();
        drawer.scale.Y *= 0.25f;
        drawer.color = Color.White * 0.5f;
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);

        drawer.scale.Y *= 2f;
        drawer.color = Color.DarkBlue * 0.5f;
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);
    }
    public void DrawStringBig(int i, int j, SpriteBatch spriteBatch)
    {
        Point start = new Point(i, j);
        Point end = GetConnectedTile(i, j);
        Color lightColor = Lighting.GetColor(start.X, end.Y);


        Vector2 startWorld = start.ToWorldCoordinates();
        Vector2 endWorld = end.ToWorldCoordinates();
        Vector2 center = startWorld + endWorld;
        center *= 0.5f;
        float rot = (endWorld - startWorld).ToRotation();


        var trail = TrailRegistry.BeamTrail;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(trail, center);
        drawer.rotation = rot;
        drawer.scale.X = Vector2.Distance(endWorld, startWorld) / (float)trail.Width();
        drawer.scale.Y *= 0.1f;
        drawer.color = Color.White * 0.5f;
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);

        drawer.scale.Y *= 2f;
        drawer.color = Color.DarkBlue * 0.5f;
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);
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
