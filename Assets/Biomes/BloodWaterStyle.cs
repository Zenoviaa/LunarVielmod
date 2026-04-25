using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Assets.Biomes
{
    public class BloodWaterStyle : ModWaterStyle
    {
        public override int ChooseWaterfallStyle() => ModContent.GetInstance<BloodWaterfallStyle>().Slot;
        public override int GetSplashDust() => DustID.BloodWater;
        public override int GetDropletGore() => GoreID.WaterDripBlood;
        public override Color BiomeHairColor() => Color.Red;

        public override void LightColorMultiplier(ref float r, ref float g, ref float b)
        {
            r = 1f;
            g = 1f;
            b = 1f;
        }
    }
}