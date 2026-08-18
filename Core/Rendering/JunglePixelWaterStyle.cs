using Stellamod.Assets;
using Stellamod.Content.Biomes;
using Terraria;

namespace Stellamod.Core.Rendering;

/// <summary>
/// Pixel water style for the jungle, with greens, yellows, and leaves in the water!
/// </summary>
public class JunglePixelWaterStyle : PixelWaterStyle
{
    private bool _inMarsh;
    public override bool IsActive(Player player)
    {
        _inMarsh = player.GetModPlayer<BiomePlayer>().ZoneMarsh;
        return player.ZoneJungle || _inMarsh;
    }

    public override void ModifyPixelWater(ref PixelWater pixelWater)
    {
        base.ModifyPixelWater(ref pixelWater);
        pixelWater.StartGradientColor = Color.LightGoldenrodYellow;
        pixelWater.EndGradientColor = Color.Green;
        pixelWater.BackgroundColor = Color.DarkGreen;
        pixelWater.CausticsColor = Color.Yellow * 0.75f;
        pixelWater.CausticsTexture = AssetRegistry.Textures.Noise.Clouds3;
        pixelWater.TilingMultiplier = Vector2.One;

        if (_inMarsh)
        {
            float lerp = 0.8f;
            pixelWater.StartGradientColor = Color.Lerp(pixelWater.StartGradientColor, Color.LightSkyBlue, lerp);
            pixelWater.EndGradientColor = Color.Lerp(pixelWater.EndGradientColor, Color.LightSkyBlue, lerp);
            pixelWater.BackgroundColor = Color.Lerp(pixelWater.BackgroundColor, Color.LightSkyBlue, lerp);
            pixelWater.CausticsColor = Color.Lerp(pixelWater.CausticsColor, Color.LightSkyBlue, lerp);
        }
    }
}
