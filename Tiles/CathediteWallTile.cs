
using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
	public class CathediteWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;

			DustType = ModContent.DustType<Sparkle>();
			ItemDrop = ModContent.ItemType<Items.Placeable.Cathedral.CathediteWall>();

			AddMapEntry(new Color(200, 200, 200));
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}