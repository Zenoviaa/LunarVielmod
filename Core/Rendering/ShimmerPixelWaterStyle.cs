using Stellamod.Assets;
using Stellamod.Content.Biomes;
using Stellamod.Core.Palettes;
using Terraria;

namespace Stellamod.Core.Rendering;

/// <summary>
/// Shimmer pixel water with cool little wiggles in it
/// </summary>
public class ShimmerPixelWaterStyle : PixelWaterStyle
{
    public override bool IsActive(Player player)
    {
        return (player.ZoneShimmer || player.ZoneWonder);
    }

    public override void ModifyPixelWater(ref PixelWater pixelWater)
    {
        base.ModifyPixelWater(ref pixelWater);
        pixelWater.StartGradientColor = Color.Blue.Towards(Color.Purple, 1f);
        pixelWater.EndGradientColor = Color.DarkBlue;
        pixelWater.BackgroundColor = Color.DarkViolet.Darken(0.75F);
        pixelWater.CausticsTexture = AssetReferences.Assets.NoiseTextures.CloudNoise2.Asset;//AssetRegistry.NoiseTextures.ShimmerWaterCaustics;
        pixelWater.CausticsColor = Color.White;
        pixelWater.TilingMultiplier = new Vector2(1.5F, 2);
        pixelWater.ignoreSkyColor = true;
        pixelWater.starryShimmer = true;
        pixelWater.lightRadius = 180;
    
    //   pixelWater.Palette = PaletteAssets.FromPaletteFile(PaletteAssets.ROYALCAPITAL).Value;
    }
}
