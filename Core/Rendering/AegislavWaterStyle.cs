using Stellamod.Content.Biomes;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Rendering;

public class AegislavWaterStyle : PixelWaterStyle
{
    //private bool _inMarsh;
    public override bool IsActive(Player player)
    {
        return player.GetModPlayer<BiomePlayer>().ZoneAegislavSurface;
    }

    public override void ModifyPixelWater(ref PixelWater pixelWater)
    {
        base.ModifyPixelWater(ref pixelWater);
            pixelWater.noLighting = false;
            pixelWater.vibrant = true;
        pixelWater.StartGradientColor = Color.RosyBrown;
        pixelWater.EndGradientColor = Color.Red;
        pixelWater.BackgroundColor = Color.Lerp(Color.Pink, Color.Black, ExtraMath.Osc(0f,0.5f, speed: 3));
        pixelWater.CausticsColor = Color.Lerp(Color.DarkRed, Color.Red, ExtraMath.Osc(0f, 1f, speed: 2, offset: 1));
        pixelWater.CausticsTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/LavaDepths");
        pixelWater.TilingMultiplier = Vector2.One * 2;
    }
}
