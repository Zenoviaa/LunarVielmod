
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
	public class IlluriaGrass : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;

			DustType = Main.rand.Next(110, 113);
			DustType = DustID.Firework_Blue;
			RegisterItemDrop(ModContent.ItemType<Items.Materials.IlluriaGrassBlock>());

			AddMapEntry(new Color(25, 142, 142));

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