
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Tiles.RoyalCapital
{
    public class CarianWallNatural : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;

			DustType = DustID.SilverCoin;
			RegisterItemDrop(ModContent.ItemType<Items.Materials.CarianWoodWall>());

			AddMapEntry(new Color(200, 200, 200));
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
		public override bool CanExplode(int i, int j) => false;
	}
}