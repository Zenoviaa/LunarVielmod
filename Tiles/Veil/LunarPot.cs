using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Tiles.Veil
{
	public class LunarPot : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Height = 10;
			TileObjectData.newTile.Width = 6;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				16,
				16,
				16,
				16,
				16,
				16,
				16
			};
			TileObjectData.newTile.AnchorBottom = default;
			TileObjectData.newTile.AnchorTop = default;
			TileObjectData.newTile.AnchorWall = true;
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();

			// name.SetDefault("Alcaology Station");
			AddMapEntry(new Color(126, 77, 59), name);
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{

		}
	}
}