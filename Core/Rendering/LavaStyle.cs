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

        BiomePlayer biomePlayer = player.GetModPlayer<BiomePlayer>();
        if (biomePlayer.ZoneHeatedDepths && !player.GetModPlayer<MyPlayer>().ZoneWonder)
            return true;


        return 
            player.ZoneUnderworldHeight || 
            player.GetModPlayer<MyPlayer>().ZoneCinder || 
            player.GetModPlayer<MyPlayer>().ZoneDrakonic;
    }
    public override void ModifyPixelWater(ref PixelWater pixelWater)
    {
        base.ModifyPixelWater(ref pixelWater);
        priority = 3;
        pixelWater.NoiseTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/NormalNoise1");
        pixelWater.CausticsTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Clouds4");
        pixelWater.CausticsColor = Color.Lerp(Color.Orange, Color.Red, 0.5f);
        pixelWater.BackgroundColor = Color.White;
        pixelWater.StartGradientColor = Color.Gold;
        pixelWater.EndGradientColor = Color.Black;
        pixelWater.affectsLava = true;
        pixelWater.noReflection = true;
        pixelWater.ignoreSkyColor = true;
        pixelWater.TilingMultiplier = new Vector2(1.5f, 2f);
   //     pixelWater.TilingMultiplier = new Vector2(
     //  pixelWater.noLighting = true;
    }
}
