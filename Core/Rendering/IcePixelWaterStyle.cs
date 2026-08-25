using Stellamod.Assets;
using Terraria;

namespace Stellamod.Core.Rendering;

/// <summary>
/// Ice-y pixel water with little crystals in it
/// </summary>
public class IcePixelWaterStyle : PixelWaterStyle
{
    public override bool IsActive(Player player)
    {
        return player.ZoneSnow;
    }

    public override void ModifyPixelWater(ref PixelWater pixelWater)
    {
        base.ModifyPixelWater(ref pixelWater);
        pixelWater.StartGradientColor = Color.White;
        pixelWater.EndGradientColor = Color.Cyan;
        pixelWater.BackgroundColor = Color.Blue;
        pixelWater.CausticsColor = Color.Cyan * 0.75f;
        pixelWater.NoiseTexture = AssetRegistry.NoiseTextures.IceWaterCaustics;
        pixelWater.CausticsTexture = AssetRegistry.NoiseTextures.IceWaterCaustics;
        pixelWater.TilingMultiplier = Vector2.One;
    }
}
