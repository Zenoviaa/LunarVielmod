
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
	public class CatagrassBlock : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;

			DustType = Main.rand.Next(110, 113);
			DustType = DustID.Firework_Yellow;
			RegisterItemDrop(ModContent.ItemType<Items.Materials.Catagrass>());

			AddMapEntry(new Color(205, 242, 245));

			// TODO: implement
			// SetModTree(new Trees.ExampleTree());
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
		// TODO: implement
		// public override void ChangeWaterfallStyle(ref int style) {
		// 	style = mod.GetWaterfallStyleSlot("ExampleWaterfallStyle");
		// }
		//
		// public override int SaplingGrowthType(ref int style) {
		// 	style = 0;
		// 	return TileType<ExampleSapling>();
		// }
	}
}