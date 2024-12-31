
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
    public class SeavathanBrick : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;

			DustType = Main.rand.Next(110, 113);
			RegisterItemDrop(ModContent.ItemType<Items.Materials.SeavathanBlock>());
			MinPick = 225;

			AddMapEntry(new Color(38, 86, 75));

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