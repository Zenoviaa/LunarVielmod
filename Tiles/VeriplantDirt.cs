
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
	public class VeriplantDirt : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
			Main.tileOreFinderPriority[Type] = 200; // Metal Detector value, see https://terraria.gamepedia.com/Metal_Detector
													// How often tiny dust appear off this tile. Larger is less frequently
			Main.tileMergeDirt[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Huntiac Silk");
			AddMapEntry(new Color(161, 170, 119), name);
			RegisterItemDrop(ModContent.ItemType<Items.Materials.VeriplantDirtBlock>());


			DustType = DustID.PinkFairy;
			DustType = DustID.Grass;
			RegisterItemDrop(ItemID.DirtBlock);
			HitSound = SoundID.Grass;
			MineResist = 1f;
		}

        public override bool CanExplode(int i, int j) => true;
        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

	}
}