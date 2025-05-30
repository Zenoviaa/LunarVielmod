﻿
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
			MineResist = 0.4f;
			MinPick = 1;

			RegisterItemDrop(ItemID.StoneBlock);
			AddMapEntry(new Color(83, 83, 83));

			// TODO: implement
			// SetModTree(new Trees.ExampleTree());
		}
		
		public override bool CanExplode(int i, int j) => true;
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}