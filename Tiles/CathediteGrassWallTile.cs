
using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Items.Placeable.Cathedral;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
	public class CathediteGrassWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;

			DustType = ModContent.DustType<Sparkle>();
		

			AddMapEntry(new Color(200, 200, 200));
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}