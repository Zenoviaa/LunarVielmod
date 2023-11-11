using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Assets.Biomes
{
    public class CathedralWaterfallStyle : ModWaterfallStyle
	{
		public override void AddLight(int i, int j) =>
			Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), Color.LightSkyBlue.ToVector3() * 0.5f);
	}
}
