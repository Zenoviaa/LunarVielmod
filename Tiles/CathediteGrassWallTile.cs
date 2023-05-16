
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
			ItemDrop/* tModPorter Note: Removed. Tiles and walls will drop the item which places them automatically. Use RegisterItemDrop to alter the automatic drop if necessary. */ = ModContent.ItemType<CathediteGrassWall>();

			AddMapEntry(new Color(200, 200, 200));
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}