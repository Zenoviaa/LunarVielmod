using Stellamod.Core;
using Stellamod.Core.ZTileSystem;
using Terraria;
using Terraria.ModLoader;

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
