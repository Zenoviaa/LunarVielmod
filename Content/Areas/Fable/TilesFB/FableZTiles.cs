using ReLogic.Content;
using Stellamod.Core.Pixelation;
using Stellamod.Core.ZTileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace Stellamod.Content.Areas.Fable.TilesFB;

public class FableBackBrick : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 2;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class FableBackBrickBig : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

public class FableBox : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class FableWall : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class FableSkinnyWall : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 1;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}
public class SkullSpike : ZTile
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        frameCount = 2;
        drawOrigin = TileDrawOrigin.BottomUp;
    }
}

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
