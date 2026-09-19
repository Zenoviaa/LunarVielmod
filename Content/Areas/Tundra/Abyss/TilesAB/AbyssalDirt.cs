using Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;
using Stellamod.WorldG;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.TilesAB;

public class AbyssalCoarseDirtItem : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<AbyssalCoarseDirt>());
    }
}

file static class AbyssalDirtUtilities
{
    public static void DrawOutline(int i, int j, int type, SpriteBatch spriteBatch)
    {
  
        Vector2 pos = (new Vector2(i, j) + VeilGen.TileAdj) * 16;
        //   pos += new Vector2(Main.offScreenRange);

        Tile tile = Main.tile[i, j];
        Rectangle frame = new Rectangle(tile.TileFrameX + 234, tile.TileFrameY, 16, 16);

        spriteBatch.Draw(TextureAssets.Tile[type].Value, pos - Main.screenPosition, frame, AbyssEffectsRenderer.TileGlowColor, 0, Vector2.Zero, 1, 0, 1);
    }
}
public class AbyssalCoarseDirt : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMerge[Type][Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileLargeFrames[Type] = 2;
        Main.tileLighted[Type] = true;
        Main.tileMerge[TileID.IceBlock][Type] = true;
        Main.tileMerge[TileID.SnowBlock][Type] = true;
        Main.tileMerge[ModContent.TileType<AbyssalIce>()][Type] = true;
        Main.tileBlendAll[Type] = true;
        TileSets.BlockMineshafts[Type] = true;
        RegisterItemDrop(ModContent.ItemType<AbyssalCoarseDirtItem>());
        AddMapEntry(new Color(57, 55, 172));
    }
    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        base.ModifyLight(i, j, ref r, ref g, ref b);
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        return base.PreDraw(i, j, spriteBatch);
    }
    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {

        base.DrawEffects(i, j, spriteBatch, ref drawData);
        if (BellFlowerSystem.WhisperingAlpha > 0)
        {
            //            Main.instance.TilesRenderer.AddSpecialLegacyPoint(new Point(i, j));
        }

    }
    public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
    {
        base.SpecialDraw(i, j, spriteBatch);

    }
    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        base.PostDraw(i, j, spriteBatch);
    }
    public override void RandomUpdate(int i, int j)
    {

        Tile tile = Framing.GetTileSafely(i, j);
        Tile tileBelow = Framing.GetTileSafely(i, j + 1);
        int[] pool = new int[]
        {
            ModContent.TileType<BlueFlower>(),
            ModContent.TileType<BlueFlower2>(),
            ModContent.TileType<TealBulb>(),
            ModContent.TileType<TealBulb2>(),
            ModContent.TileType<TealBulb3>()
        };

        if (!Main.rand.NextBool(32))
            return;
        if (!MultiplayerHelper.IsHost)
            return;

        //Tile tileAbove = Framing.GetTileSafely(i, j - 1);
        if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0)//grass
        {
            WorldGen.PlaceTile(i, j - 1, pool[Main.rand.Next(0, pool.Length)], true);
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendTileSquare(-1, i, j - 1, TileChangeType.None);
            }
        }

        //Try place vine
        if (WorldGen.genRand.NextBool(3) && !tileBelow.HasTile && !(tileBelow.LiquidType == LiquidID.Lava))
        {
            if (!tile.BottomSlope)
            {
                tileBelow.TileType = (ushort)ModContent.TileType<AbyssalVines>();
                tileBelow.HasTile = true;
                WorldGen.SquareTileFrame(i, j + 1, true);
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                }
            }
        }
        if (WorldGen.genRand.NextBool(3) && !tileBelow.HasTile && !(tileBelow.LiquidType == LiquidID.Lava))
        {
            if (!tile.BottomSlope)
            {
                tileBelow.TileType = (ushort)ModContent.TileType<AbyssalVines2>();
                tileBelow.HasTile = true;
                WorldGen.SquareTileFrame(i, j + 1, true);
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                }
            }
        }
    }
}
public class AbyssalDirtItem : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<AbyssalDirt>());
    }
}


public class AbyssalDirt : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMerge[Type][Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileLargeFrames[Type] = 2;
        Main.tileLighted[Type] = true;
        Main.tileMerge[TileID.IceBlock][Type] = true;
        Main.tileMerge[TileID.SnowBlock][Type] = true;
        Main.tileMerge[ModContent.TileType<AbyssalIce>()][Type] = true;
        Main.tileBlendAll[Type] = true;
        TileSets.BlockMineshafts[Type] = true;
        RegisterItemDrop(ModContent.ItemType<AbyssalDirtItem>());
        AddMapEntry(new Color(57, 55, 172));
    }
    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        base.ModifyLight(i, j, ref r, ref g, ref b);
    }

    public override void RandomUpdate(int i, int j)
    {

        Tile tile = Framing.GetTileSafely(i, j);
        Tile tileBelow = Framing.GetTileSafely(i, j + 1);
        int[] pool = new int[]
        {
            ModContent.TileType<BlueFlower>(),
            ModContent.TileType<BlueFlower2>(),
            ModContent.TileType<TealBulb>(),
            ModContent.TileType<TealBulb2>(),
            ModContent.TileType<TealBulb3>()
        };

        if (!Main.rand.NextBool(32))
            return;


        //Tile tileAbove = Framing.GetTileSafely(i, j - 1);
        if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == 0)//grass
        {
            WorldGen.PlaceTile(i, j - 1, pool[Main.rand.Next(0, pool.Length)], true);
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendTileSquare(-1, i, j - 1, TileChangeType.None);
            }
        }

        //Try place vine
        if (WorldGen.genRand.NextBool(3) && !tileBelow.HasTile && !(tileBelow.LiquidType == LiquidID.Lava))
        {
            if (!tile.BottomSlope)
            {
                tileBelow.TileType = (ushort)ModContent.TileType<AbyssalVines>();
                tileBelow.HasTile = true;
                WorldGen.SquareTileFrame(i, j + 1, true);
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                }
            }
        }
        if (WorldGen.genRand.NextBool(3) && !tileBelow.HasTile && !(tileBelow.LiquidType == LiquidID.Lava))
        {
            if (!tile.BottomSlope)
            {
                tileBelow.TileType = (ushort)ModContent.TileType<AbyssalVines2>();
                tileBelow.HasTile = true;
                WorldGen.SquareTileFrame(i, j + 1, true);
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                }
            }
        }
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        base.DrawEffects(i, j, spriteBatch, ref drawData);

    }
    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        base.PostDraw(i, j, spriteBatch);

    }
    public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
    {
        base.SpecialDraw(i, j, spriteBatch);
    }
}