using Stellamod.Assets;
using Terraria;

namespace Stellamod.Core.Rendering;

/// <summary>
/// Shimmer pixel water with cool little wiggles in it
/// </summary>
public class ShimmerPixelWaterStyle : PixelWaterStyle
{
    public override bool IsActive(Player player)
    {
        return (player.ZoneShimmer || player.GetModPlayer<MyPlayer>().ZoneWonder);
    }

    public override void ModifyPixelWater(ref PixelWater pixelWater)
    {
        base.ModifyPixelWater(ref pixelWater);
        pixelWater.StartGradientColor = Color.White;
        pixelWater.EndGradientColor = Color.DarkBlue;
        pixelWater.BackgroundColor = Color.Pink;
        pixelWater.CausticsTexture = AssetRegistry.Textures.Noise.ShimmerWaterCaustics;
        pixelWater.CausticsColor = Color.Purple;
        pixelWater.TilingMultiplier = new Vector2(1f, 2);
        pixelWater.ignoreSkyColor = true;
     
    }
}
