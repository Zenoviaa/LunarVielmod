using Stellamod.Content.Biomes;
using Stellamod.Core.Backgrounds;
using Terraria;

namespace Stellamod.Content.Areas.Fable;

public class FableBackground : CustomBG
{
    public override bool UseCustomDrawing()
    {
        return true;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        BackgroundHelper.DrawSurfaceWrappingBackground(spriteBatch, new()
        {
            Alpha = Alpha,
            yParallaxStartHeight = GetParallaxYStartHeight() - 4000,
            ParallaxSpeed = new Vector2(LocalParallaxSpeed),
            Close = AssetReferences.Assets.Textures.Backgrounds.FableClose.Asset,
            Mid = AssetReferences.Assets.Textures.Backgrounds.FableMid.Asset,
            Far = AssetReferences.Assets.Textures.Backgrounds.FableFar.Asset,
            Underground = AssetReferences.Assets.Textures.Backgrounds.AegislavSurface_UndergroundLoop.Asset
        });
    }

    public override bool IsActive()
    {
        return Main.LocalPlayer.ZoneWitchesFable;
    }
}
