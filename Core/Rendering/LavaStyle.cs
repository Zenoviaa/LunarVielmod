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
        pixelWater.CausticsColor = Color.White;
        pixelWater.BackgroundColor = Color.White;

        Color lavaTop = new Color(255, 153, 31);
        Color lava = new Color(222, 30, 6);
        pixelWater.StartGradientColor = lavaTop;
  
        pixelWater.EndGradientColor = lava;
        pixelWater.affectsLava = true;
        pixelWater.noReflection = true;
        pixelWater.ignoreSkyColor = true;
        pixelWater.TilingMultiplier = new Vector2(1.5f, 3.5F);
        pixelWater.lightRadius = 300;
     //   pixelWater.noLighting = true;
    }
}
