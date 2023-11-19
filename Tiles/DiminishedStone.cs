
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
    public class DiminishedStone: ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;

			DustType = DustID.SilverCoin;
		
			MineResist = 2f;
			MinPick = 50;
			RegisterItemDrop(ItemID.StoneBlock);
			AddMapEntry(new Color(46, 26, 2));

			// TODO: implement
			// SetModTree(new Trees.ExampleTree());
		}
		public override bool CanExplode(int i, int j) => false;
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