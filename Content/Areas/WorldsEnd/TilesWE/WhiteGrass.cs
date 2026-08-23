using ReLogic.Content;
using Stellamod.Core.ZTileSystem;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.WorldsEnd.TilesWE;

public class BigWhiteFlower : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.Center;

        //idk
        windSwayOffset = 0f;

        //The max it can sway
        windSwayMagnitude = 0.2f;

        //How fast it sways
        windSwaySpeed = 0.02f;
    }
}

public class WhiteFlower : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 4;
        drawOrigin = TileDrawOrigin.Center;

        //idk
        windSwayOffset = 0f;

        //The max it can sway
        windSwayMagnitude = 0.2f;

        //How fast it sways
        windSwaySpeed = 0.02f;
    }
}

public class WhiteGrassBlock : ModItem
{
    public override void SetStaticDefaults()
    {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

    }

    public override void SetDefaults()
    {
        Item.width = 12;
        Item.height = 12;
        Item.maxStack = Item.CommonMaxStack;
        Item.useTurn = true;
        Item.autoReuse = true;
        Item.useAnimation = 10;
        Item.useTime = 10;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<WhiteGrass>();
    }
}

[Autoload(Side = ModSide.Client)]
public class WhiteGrassSystem : ModSystem
{
    private UnifiedRandom _flowerRandom;
    private Asset<Texture2D> _flowerTextureAsset;
    private Asset<Texture2D> _flowerTextureAsset2;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        string texture = ModContent.GetInstance<WhiteGrass>().Texture;
        _flowerTextureAsset = ModContent.Request<Texture2D>(texture + "_Flowers");
        _flowerTextureAsset2 = ModContent.Request<Texture2D>(texture + "_Flowers2");
    }
    public override void Unload()
    {
        base.Unload();
        _flowerTextureAsset = null;
        _flowerTextureAsset2 = null;
    }

    public override void PostDrawTiles()
    {
        base.PostDrawTiles();
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Begin(SpritebatchParams.InWorldAndZoomed());
        (Point topLeft, Point bottomRight) = TileUtilities.CameraTileBounds(64);
        int type = ModContent.TileType<WhiteGrass>();
        for (int x = topLeft.X; x < bottomRight.X; x++)
        {
            for (int y = topLeft.Y; y < bottomRight.Y; y++)
            {
                Tile t = Main.tile[x, y];
                if (!t.HasTile)
                    continue;
                if (t.TileType != type)
                    continue;

                DrawGrass(x, y, Main.spriteBatch);
            }
        }
        spriteBatch.End();
    }

    private void DrawGrass(int i, int j, SpriteBatch spriteBatch)
    {
        Tile tileAbove = Framing.GetTileSafely(i, j - 1);
        if (tileAbove.HasTile)
            return;
        _flowerRandom ??= new UnifiedRandom();
        _flowerRandom.SetSeed(i + j);
        float numFlowers = _flowerRandom.Next(1, 3);
        VelocityMap velocityMap = ModContent.GetInstance<VelocityMap>();
        Vector2 worldPos = new Point(i, j).ToWorldCoordinates();

        bool altFlowers = _flowerRandom.NextBool(2);
        Vector2 offset = new Vector2(0);
        Vector2 origin = GetFrame(0).Size() * 0.5f;
        if (altFlowers)
        {
            offset.Y = 12;
            origin = GetFrame2(1).Size() * new Vector2(0.5f, 1f);
        }

        Color lightColor = Lighting.GetColor(i, j);
        float range = MathHelper.ToRadians(15);
        var textureAsset = altFlowers ? _flowerTextureAsset2 : _flowerTextureAsset;


        for (int k = 0; k < numFlowers; k++)
        {

            Vector2 flowerDrawPos = worldPos - Main.screenPosition + offset;
            flowerDrawPos.X += _flowerRandom.NextFloat(-4f, 4f);
            flowerDrawPos.Y += _flowerRandom.NextFloat(-4f, 4f);
            flowerDrawPos.Y -= 12;

            Rectangle frame;
            if (altFlowers)
            {
                int index = _flowerRandom.Next(0, 8);
                frame = GetFrame2(index);
            }
            else
            {
                int index = _flowerRandom.Next(0, 4);
                frame = GetFrame(index);
            }



            float rotation = ExtraMath.Osc(-range, range, speed: 2, i + k);

            float scale = _flowerRandom.NextFloat(0.5f, 1f);
            spriteBatch.Draw(textureAsset.Value, flowerDrawPos, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0);
        }

    }
    private Rectangle GetFrame(int index)
    {
        int frameCount = 4;
        int frameWidth = _flowerTextureAsset.Width();
        int frameHeight = _flowerTextureAsset.Height() / frameCount;
        Rectangle sourceRect = new Rectangle(0, frameHeight * index, frameWidth, frameHeight);
        return sourceRect;
    }
    private Rectangle GetFrame2(int index)
    {
        int frameCount = 8;
        int frameWidth = _flowerTextureAsset2.Width();
        int frameHeight = _flowerTextureAsset2.Height() / frameCount;
        Rectangle sourceRect = new Rectangle(0, frameHeight * index, frameWidth, frameHeight);
        return sourceRect;
    }
}
public class WhiteGrass : ModTile
{

    public override void SetStaticDefaults()
    {

        Main.tileSolid[Type] = true;
        Main.tileMerge[Type][Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileMerge[TileID.Dirt][Type] = true;
        Main.tileMerge[TileID.Grass][Type] = true;
        Main.tileBlendAll[Type] = true;
        Main.tileBlockLight[Type] = true;
        AddMapEntry(Color.LightGray);
    }
}
