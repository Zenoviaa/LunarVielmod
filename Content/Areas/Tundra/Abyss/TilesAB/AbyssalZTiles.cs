using ReLogic.Content;
using Stellamod.Core.ZTileSystem;
using Stellamod.WorldG;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.Tundra.Abyss.TilesAB;

file static class AbyssalZTileUtilties
{
    public static void SetAbyssFlowerWindDefaults(ZTile zTile)
    {
        //idk
        zTile.windSwayOffset = 0f;

        //The max it can sway
        zTile.windSwayMagnitude = 0.05f;

        //How fast it sways
        zTile.windSwaySpeed = 0.02f;
    }
    public static void AddAbyssFlowerLighting(int i, int j)
    {
        Color color = Color.SkyBlue;
        Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), color.ToVector3());
    }
    public static void DrawAbyssFlowerGlow(ZTile zTile, SpriteBatch spriteBatch, in ZTileDrawData drawData, in ZTileDrawParams drawParams)
    {
        var glowAsset = ModContent.Request<Texture2D>(zTile.Texture + "_Glow");//AssetReferences.Content.Areas.Tundra.Abyss.TilesAB.AbyssalFlower_Glow.Asset;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromZTileDraw(glowAsset, drawData);
        drawer.color = DrawUtilities.InterpolateColorArray(ExtraMath.Osc(0f, 1f, speed: 3, offset: drawParams.tilePosition.x * drawParams.tilePosition.y), Color.White, Color.SkyBlue, Color.Pink);
        drawer.color *= 0.35f;
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);
    }
}

public class AbyssalKelp : ModTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        LocalizedText name = CreateMapEntryName();
        TileID.Sets.IsATreeTrunk[Type] = true;
        Main.tileAxe[Type] = true;
        AddMapEntry(new Color(169, 200, 93), name);
       // RegisterItemDrop(ItemID.Wood);
    }

    private float GetLeafSway(float offset, float magnitude, float speed)
    {
        return (float)Math.Sin(Main.GameUpdateCount * speed + offset) * magnitude;
    }


    private void DrawKelp(int i, int j, SpriteBatch spriteBatch)
    {
        Tile tile = Main.tile[i, j];
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.Tile[Type],
            TileUtilities.ToWorldCoordinatesFromTileRendering(i, j));
        drawer.color = Color.White.MultiplyRGB(Lighting.GetColor(i, j));
        drawer.rotation = GetLeafSway(i + j, 0.07f, 0.05f);
        drawer.VerticalFrame(tile.TileFrameY, 6);
        drawer.BottomCenterOrigin();
        drawer.worldPosition.Y += drawer.sourceRect.Value.Height;
        spriteBatch.Draw(drawer);
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        DrawKelp(i, j, spriteBatch);
        return false;
    }

    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        if (fail || effectOnly)
            return;

        Framing.GetTileSafely(i, j).HasTile = false;

        bool up = Framing.GetTileSafely(i, j - 1).TileType == Type || Framing.GetTileSafely(i, j - 1).TileType == Type;
        bool down = Framing.GetTileSafely(i, j + 1).TileType == Type;

        if (up)
            WorldGen.KillTile(i, j - 1);
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
    {
        short x = 0;
        short y = 0;

        bool up = Framing.GetTileSafely(i, j - 1).TileType == Type|| Framing.GetTileSafely(i, j - 1).TileType == Type;
        bool down = Framing.GetTileSafely(i, j + 1).TileType == Type;

        if (up && down)
        {
            y = (short)Main.rand.Next(1, 3);
        }
        else if (up)
        {
            y = 0;
        }
        else if (down)
        {
            y = (short)Main.rand.Next(4, 6);
        }

        Tile tile = Framing.GetTileSafely(i, j);
        tile.TileFrameX = x;
        tile.TileFrameY = y;
        return false;
    }
}

public class AbyssalFlower : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 8;
        drawOrigin = TileDrawOrigin.BottomUp;
        AbyssalZTileUtilties.SetAbyssFlowerWindDefaults(this);
    }

    public override void Update(int i, int j)
    {
        base.Update(i, j);
        AbyssalZTileUtilties.AddAbyssFlowerLighting(i, j);
    }

    public override void PostDraw(SpriteBatch spriteBatch, in ZTileDrawData drawData, in ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawData, drawParams);
        AbyssalZTileUtilties.DrawAbyssFlowerGlow(this, spriteBatch, drawData, drawParams);
    }
}

public class AbyssalReed : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.BottomUp;
        AbyssalZTileUtilties.SetAbyssFlowerWindDefaults(this);
    }

    public override void Update(int i, int j)
    {
        base.Update(i, j);
        AbyssalZTileUtilties.AddAbyssFlowerLighting(i, j - 4);
    }

    public override void PostDraw(SpriteBatch spriteBatch, in ZTileDrawData drawData, in ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawData, drawParams);
        AbyssalZTileUtilties.DrawAbyssFlowerGlow(this, spriteBatch, drawData, drawParams);
    }
}
public class AbyssalOrbFlower : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 2;
        drawOrigin = TileDrawOrigin.BottomUp;
        AbyssalZTileUtilties.SetAbyssFlowerWindDefaults(this);
    }

    public override void Update(int i, int j)
    {
        base.Update(i, j);
        AbyssalZTileUtilties.AddAbyssFlowerLighting(i, j);
    }

    public override void PostDraw(SpriteBatch spriteBatch, in ZTileDrawData drawData, in ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawData, drawParams);
        AbyssalZTileUtilties.DrawAbyssFlowerGlow(this, spriteBatch, drawData, drawParams);
    }
}

public class AbyssalWhiteFlower : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 6;
        drawOrigin = TileDrawOrigin.BottomUp;
        AbyssalZTileUtilties.SetAbyssFlowerWindDefaults(this);
    }

    public override void Update(int i, int j)
    {
        base.Update(i, j);
        AbyssalZTileUtilties.AddAbyssFlowerLighting(i, j);
    }

    public override void PostDraw(SpriteBatch spriteBatch, in ZTileDrawData drawData, in ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawData, drawParams);
        //  AbyssalZTileUtilties.DrawAbyssFlowerGlow(this, spriteBatch, drawData, drawParams);
    }
}
