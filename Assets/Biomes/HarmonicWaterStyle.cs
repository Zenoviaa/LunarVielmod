
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Assets.Biomes;

public class HarmonicWaterStyle : ModWaterStyle
{
    public override int ChooseWaterfallStyle() => ModContent.GetInstance<HarmonicWaterfallStyle>().Slot;
    public override int GetSplashDust() => DustID.Water;
    public override int GetDropletGore() => GoreID.WaterDrip;
    public override Color BiomeHairColor() => Color.Blue;

    public override void LightColorMultiplier(ref float r, ref float g, ref float b)
    {
        r = 0.8f;
        g = 0.8f;
        b = 1f;
    }
}
public class HarmonicWaterfallStyle : ModWaterfallStyle
{
    // Makes the waterfall provide light
    // Learn how to make a waterfall: https://terraria.wiki.gg/wiki/Waterfall
    public override void AddLight(int i, int j) =>
        Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), Color.White.ToVector3() * 0.5f);
}