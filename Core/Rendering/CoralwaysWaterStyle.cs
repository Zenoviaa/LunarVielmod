using Stellamod.Content.Biomes;
using Terraria;

namespace Stellamod.Core.Rendering;

public class CoralwaysWaterStyle : PixelWaterStyle
{

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        priority = 2;
    }

    public override bool IsActive(Player player)
    {
        if (player.GetModPlayer<MyPlayer>().ZoneCinder)
            return false;
        return player.GetModPlayer<BiomePlayer>().ZoneHarmonicCoralways || player.GetModPlayer<BiomePlayer>().ZoneDeepBelowCoralways;
    }
    public override void ModifyPixelWater(ref PixelWater pixelWater)
    {
        base.ModifyPixelWater(ref pixelWater);
        pixelWater.noLighting = true;
        pixelWater.vibrant = true;
        pixelWater.EndGradientColor = Color.Lerp(Color.Aqua, Color.Black, 0.05f);
        pixelWater.ignoreSkyColor = true;

    }
}
