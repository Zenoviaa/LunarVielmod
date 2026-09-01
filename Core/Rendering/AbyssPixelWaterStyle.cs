using Stellamod.Content.Biomes;
using Stellamod.Core.Palettes;
using Terraria;

namespace Stellamod.Core.Rendering;

public class AbyssPixelWaterStyle : PixelWaterStyle
{
    public override bool IsActive(Player player)
    {

        return player.ZoneAbyss;
    }
    public override void ModifyPixelWater(ref PixelWater pixelWater)
    {
        base.ModifyPixelWater(ref pixelWater);
        pixelWater.StartGradientColor = Color.White;
        pixelWater.EndGradientColor = Color.White;
        pixelWater.BackgroundColor = Color.Cyan;
        pixelWater.CausticsColor = Color.White;
        pixelWater.NoiseTexture = AssetReferences.Assets.NoiseTextures.WaterCaustics.Asset;
        pixelWater.CausticsTexture = AssetReferences.Assets.NoiseTextures.WaterCaustics.Asset;
        pixelWater.TilingMultiplier = Vector2.One;
        pixelWater.Palette = PaletteAssets.FromPaletteFile(PaletteAssets.ABYSSWATER).Value;
        pixelWater.vibrant = true;
        pixelWater.ignoreSkyColor = true;
        pixelWater.noLighting = true;

    
    }
}