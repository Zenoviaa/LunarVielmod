
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
    public class StarbloomWaterStyle : ModWaterStyle
    {
        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("Stellamod/StarbloomWaterfallStyle").Slot;
        public override int GetSplashDust() => DustID.PinkCrystalShard;
        public override int GetDropletGore() => GoreID.WaterDripHallow;
        public override Color BiomeHairColor() => Color.Pink;

        public override void LightColorMultiplier(ref float r, ref float g, ref float b)
        {
            r = 1.4f;
            g = 1f;
            b = 1.4f;
        }
    }
}