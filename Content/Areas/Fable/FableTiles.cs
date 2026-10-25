using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.ZTileSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable;

public abstract class AHangingLantern : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        drawOrigin = TileDrawOrigin.TopDown;
        windSwayMagnitude = 0.2f;
        windSwaySpeed = 0.1f;
        windSwayOffset = 0.1f;
    }
    public override void Update(int i, int j)
    {
        base.Update(i, j);
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        return base.PreDraw(spriteBatch, drawPosition, screenPos, drawParams);
    }
}


#region Hanging Lanterns
file static class FableHangingLanternHelper
{

}

public class FableHangingLanternTiny : AHangingLantern
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableLanternHangingTiny.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 2;
    }
    public override void Update(int i, int j)
    {
        base.Update(i, j);
    }
}
public class FableHangingLanternSmall : AHangingLantern
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableLanternHangingSmall.KEY;

    public override void Update(int i, int j)
    {
        base.Update(i, j);
    }
}
public class FableHangingLanternBig : AHangingLantern
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableLanternHangingBig.KEY;
    public override void Update(int i, int j)
    {
        base.Update(i, j);
    }
}
#endregion

#region Wavey Banners
public abstract class AWaveyBanner : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        drawOrigin = TileDrawOrigin.TopDown;
        frameCount = 1;

    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        //TODO: Don't spam ModContent.Request
        Asset<Texture2D> texture = ModContent.Request<Texture2D>(Texture);

        Vector2 flagPosition = drawPosition;
        flagPosition.X += ExtraMath.Osc(0f, 4, speed: 3);
        //  flagPosition.Y -= texture.Height() * 0.5f;
        Vector2 drawOrigin = new Vector2(texture.Width() / 2f, 0f);
        BannerWavingShader wavingShader = BannerWavingShader.Instance;
        wavingShader.OscStrength = 0.1f;
        wavingShader.XOffset = 4;
        wavingShader.Time = Main.GlobalTimeWrappedHourly * 2 + drawParams.tilePosition.x;

        spriteBatch.Restart(effect: wavingShader.Effect);
        spriteBatch.Draw(texture.Value, flagPosition, null, drawParams.lightColor, 0, drawOrigin, 1, SpriteEffects.None, 0);
        spriteBatch.RestartDefaults();
        return false;
    }
}

public class FableWaveyBanner : AWaveyBanner
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableWaveyBanner.KEY;
}

public class FableWaveyBannerSmall : AWaveyBanner
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableWaveyBannerSmall.KEY;
}

#endregion

public abstract class ALitPumpkinTile : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 2;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
    public override void Update(int i, int j)
    {
        base.Update(i, j);
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 drawPosition, Vector2 screenPos, ZTileDrawParams drawParams)
    {
        return base.PreDraw(spriteBatch, drawPosition, screenPos, drawParams);
    }
    public override void ModifyDraw(ref ZTileDrawData drawData)
    {
        base.ModifyDraw(ref drawData);
    }
}
public class PotionRack : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.PotionRack.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class FableCandle : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableCandle.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        drawOrigin = TileDrawOrigin.BottomUp;
        frameCount = 3;
    }

    public override void Update(int i, int j)
    {
        base.Update(i, j);
    }
}
public class FunnyPumpkin : ALitPumpkinTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FunnyPumpkin.KEY;
}

public class WidePumpkin : ALitPumpkinTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.WidePumpkin.KEY;
}

public class RoundPumpkin : ALitPumpkinTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.RoundPumpkin.KEY;
}

public class TallPumpkin : ALitPumpkinTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.TallPumpkin.KEY;
}
public class FableSpikes : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableSpikes.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class FableMushroom : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableMushroom.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
    public override void PostDraw(SpriteBatch spriteBatch, in ZTileDrawData drawData, in ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawData, drawParams);
    }
}
public class FableMushroomSpotted : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableMushroomSpotted.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class FableRuins : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableRuins.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 2;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class FableLog : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableLog.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class FableMiniBooks : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableMiniBooks.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 6;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
#region Potion Decorations


public class FablePotion : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FablePotion.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 15;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class PotionBundle : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.PotionBundle.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 9;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
#endregion
public class FableBookcase : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableBookcase.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 8;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class BigFableDoor : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.BigFableDoor.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class FableBackBrick : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableBackBrick.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 2;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class FableTower : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableTower.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class FableBackBrickBig : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableBackBrickBig.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class FableBox : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableBox.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class FableWall : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableWall.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class FableBrickWall : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableBrickWall.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 3;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class FableWindowWithCurtain : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableWindowWithCurtain.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class FableSkinnyWall : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableSkinnyWall.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class SkullSpike : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.SkullSpike.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 2;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

#region Lanterns
file static class FableLanternHelper
{

    public static void Glow(SpriteBatch spriteBatch, Asset<Texture2D> textureAsset,
        ZTileDrawData drawData,
        ZTileDrawParams drawParams,
        float strength = 1f)
    {
        var drawer = SpritebatchDrawer.FromZTileDraw(textureAsset, drawData);
        var inc = MathHelper.Lerp(6, 0, strength);
        drawer.color = Color.White * ExtraMath.Osc(0.6f, 1f, 6 + inc) * 0.3f;
        drawer.color.A = 0;
        spriteBatch.Draw(drawer);

        void DrawGlow(SpriteBatch sb, Vector2 sp)
        {
            var glowDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Assets.GlowMasks.SimpleGlowCircle.Asset, drawData.drawPosition + Main.screenPosition);
            glowDrawer.color = Color.Gold * 0.14f * ExtraMath.Osc(0.5f, 1f, speed: 6 + inc) * MathHelper.Lerp(0.5f, 1f, strength);
            glowDrawer.color.A = 0;
            glowDrawer.scale *= strength * ExtraMath.Osc(0.8f, 1f);
            spriteBatch.Draw(glowDrawer);
        }
        PixelationManager.QueueSpritebatchDrawAction(DrawGlow, DrawLayer.OverPlayers);

    }
}
public class FableLanternTiny : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableLanternTiny.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
    public override void Update(int i, int j)
    {
        base.Update(i, j);
        Lighting.AddLight(i, j, TorchID.Torch, 0.5f);
    }
    public override void PostDraw(SpriteBatch spriteBatch, in ZTileDrawData drawData, in ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawData, drawParams);
        FableLanternHelper.Glow(spriteBatch, _tileTextureAsset, drawData, drawParams, 0.2f);
    }
}

public class FableLanternSmall : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableLanternSmall.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
    public override void Update(int i, int j)
    {
        base.Update(i, j);
        Lighting.AddLight(i, j, TorchID.Torch, 0.5f);
    }
    public override void PostDraw(SpriteBatch spriteBatch, in ZTileDrawData drawData, in ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawData, drawParams);
        FableLanternHelper.Glow(spriteBatch, _tileTextureAsset, drawData, drawParams, 0.4f);
    }
}
public class FableLanternBig : ZTile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Tiles.FableLanternBig.KEY;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
    public override void Update(int i, int j)
    {
        base.Update(i, j);
        Lighting.AddLight(i, j, TorchID.Torch, 0.65f);
    }
    public override void PostDraw(SpriteBatch spriteBatch, in ZTileDrawData drawData, in ZTileDrawParams drawParams)
    {
        base.PostDraw(spriteBatch, drawData, drawParams);
        FableLanternHelper.Glow(spriteBatch, _tileTextureAsset, drawData, drawParams, 0.6f);
    }
}

#endregion