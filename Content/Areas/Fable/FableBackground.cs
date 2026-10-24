using Stellamod.Content.Biomes;
using Stellamod.Core.Backgrounds;
using Stellamod.Core.Effects;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable;

public class FableBackground : CustomBG
{
    public CustomBGLayer farLayer;
    public CustomBGLayer midLayer;
    public CustomBGLayer closeLayer;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        DrawScale = 1.5f;
        DrawOffset = new Vector2(0, -100);

        float startParallax = 0.15f;

        farLayer = new CustomBGLayer();
        farLayer.Texture = AssetReferences.Assets.Textures.Backgrounds.FableFar.Asset;
        farLayer.Parallax = startParallax;
        farLayer.DrawOffset = Vector2.Zero;
        AddLayer(farLayer);


        midLayer = new CustomBGLayer();
        midLayer.Texture = AssetReferences.Assets.Textures.Backgrounds.FableMid.Asset;
        midLayer.Parallax = startParallax + 0.02f;
        midLayer.DrawOffset = Vector2.Zero;
        AddLayer(midLayer);

        closeLayer = new CustomBGLayer();
        closeLayer.Texture = AssetReferences.Assets.Textures.Backgrounds.FableClose.Asset;
        closeLayer.Parallax = startParallax + 0.05f;
        closeLayer.DrawOffset = Vector2.Zero;
        AddLayer(closeLayer);


    }
    public override void SetDrawDefaults()
    {
        base.SetDrawDefaults();
        NoSurfaceOffset = true;
        DrawScale = 1f;
        LocalParallaxSpeed = 1 / 1.5f;
        farLayer.Texture = AssetReferences.Assets.Textures.Backgrounds.FableFar.Asset;
        farLayer.Parallax = 0.06f;
        midLayer.Parallax = 0.09f;
        closeLayer.Parallax = 0.12f;
        var p = (float)Main.worldSurface;
        p *= 16;
        SnapParallaxY = p - 2430;
        ParallaxReferenceY = SnapParallaxY - 252;

        closeLayer.SnapParallaxYFactor = 0.1f;
        midLayer.SnapParallaxYFactor = 0.165f;
        farLayer.SnapParallaxYFactor = 0.25f;
        DrawOffset = new Vector2(0, 0);
        ParallaxYOffset = 0;
    }

    public override bool IsActive()
    {
        return Main.LocalPlayer.ZoneWitchesFable;
    }
}
