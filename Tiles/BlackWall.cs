
using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
    public class BlackWall: ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;

			DustType = ModContent.DustType<Sparkle>();
			RegisterItemDrop(ModContent.ItemType<Items.Materials.BlackWallBlock>());

			AddMapEntry(new Color(1, 1, 1));
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
		public override bool CanExplode(int i, int j) => false;
	}
}