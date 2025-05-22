
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
	public class VeriplantGrass : ModTile
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
			AddMapEntry(new Color(10, 30, 60), name);


			DustType = DustID.PinkFairy;
			DustType = DustID.Grass;
			RegisterItemDrop(ItemID.DirtBlock);
			RegisterItemDrop(ModContent.ItemType<Items.Materials.VeriplantGrassBlock>());
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