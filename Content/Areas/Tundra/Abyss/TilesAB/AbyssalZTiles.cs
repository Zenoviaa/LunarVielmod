using Stellamod.Core;
using Stellamod.Core.ZTileSystem;
using Terraria;

namespace Stellamod.Content.Areas.Tundra.Abyss.TilesAB;

public class AbyssalFlower : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 8;
        drawOrigin = TileDrawOrigin.BottomUp;

        //idk
        windSwayOffset = 0f;

        //The max it can sway
        windSwayMagnitude = 0.1f;

        //How fast it sways
        windSwaySpeed = 0.02f;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawPosition, screenPos, drawParams);
     
    }

    public override void PostDraw(SpriteBatch spriteBatch, in ZTileDrawData drawData, in ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawData, drawParams);
        var glowAsset = AssetReferences.Content.Areas.Tundra.Abyss.TilesAB.AbyssalFlower_Glow.Asset;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromZTileDraw(glowAsset, drawData);
        drawer.color = DrawUtilities.InterpolateColorArray(ExtraMath.Osc(0f, 1f, speed: 3, offset: drawParams.tilePosition.x * drawParams.tilePosition.y), Color.White, Color.SkyBlue, Color.Pink);
        drawer.color *= 0.35f;
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);
    }
}