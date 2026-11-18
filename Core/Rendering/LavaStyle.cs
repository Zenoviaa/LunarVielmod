using Stellamod.Content.Biomes;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Rendering;

public class LavaStyle : PixelWaterStyle
{
   
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override bool IsActive(Player player)
    {

        if (player.ZoneHeatedDepths && !player.ZoneWonder)
            return true;


        return 
            player.ZoneUnderworldHeight || 
            player.ZoneCinder || 
            player.ZoneDrakonic;
    }
    public override void ModifyPixelWater(ref PixelWater pixelWater)
    {
        base.ModifyPixelWater(ref pixelWater);
        priority = 3;
        pixelWater.NoiseTexture = AssetReferences.Assets.NoiseTextures.NormalNoise1.Asset;//ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/NormalNoise1");
        pixelWater.CausticsTexture = AssetReferences.Assets.NoiseTextures.Clouds4.Asset;// ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Clouds4");
        pixelWater.CausticsColor = Color.Lerp(Color.Orange, Color.Red, 0.5f);
        pixelWater.BackgroundColor = Color.White;
        pixelWater.StartGradientColor = Color.Gold;
        pixelWater.EndGradientColor = Color.Lerp(Color.DarkRed, Color.Black, 0.5f);
        pixelWater.affectsLava = true;
        pixelWater.noReflection = true;
        pixelWater.ignoreSkyColor = true;
        pixelWater.TilingMultiplier = new Vector2(1.5f, 2f);
        pixelWater.noLighting = true;
    }
}
