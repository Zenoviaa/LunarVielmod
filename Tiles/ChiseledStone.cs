
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
    public class ChiseledStone : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
			Main.tileOreFinderPriority[Type] = 110; // Metal Detector value, see https://terraria.gamepedia.com/Metal_Detector
											 // How often tiny dust appear off this tile. Larger is less frequently
			Main.tileMergeDirt[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Huntiac Silk");
			AddMapEntry(new Color(10, 30, 60), name);

	
			DustType = DustID.Stone;
			DustType = DustID.Web;
			RegisterItemDrop(ItemID.StoneBlock);
			HitSound = SoundID.Grass;
		 MineResist = 1f;
		 MinPick = 220;
		}
		public override bool CanExplode(int i, int j) => false;
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
		
	}
}