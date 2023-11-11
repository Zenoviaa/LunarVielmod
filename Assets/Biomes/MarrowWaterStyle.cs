using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Assets.Biomes
{
    public class MarrowWaterStyle : ModWaterStyle
	{
		public override int ChooseWaterfallStyle() => Find<ModWaterfallStyle>("Stellamod/MarrowWaterfallStyle").Slot;
		public override int GetSplashDust() => DustType<Solution>();
		public override int GetDropletGore() => Find<ModGore>("Stellamod/EggGore").Type;
		public override void LightColorMultiplier(ref float r, ref float g, ref float b)
		{
			r = 0f;
			g = 1f;
			b = 0f;
		}
		public override Color BiomeHairColor()
			=> Color.LemonChiffon;
	}
}