using Stellamod.Content.Biomes;
using Terraria;

namespace Stellamod.Core.Rendering;

/// <summary>
/// Default pixel water that looks like the ocean
/// </summary>
public class BeachPixelWaterStyle : PixelWaterStyle
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override bool IsActive(Player player)
    {
        return player.ZoneBeach && !(player.GetModPlayer<BiomePlayer>().ZoneHarmonicCoralways || player.GetModPlayer<BiomePlayer>().ZoneDeepBelowCoralways);
    }
    public override void ModifyPixelWater(ref PixelWater pixelWater)
    {
        base.ModifyPixelWater(ref pixelWater);
        pixelWater.noLighting = true;
    }
}
