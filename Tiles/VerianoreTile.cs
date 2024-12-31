
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
    public class VerianoreTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.Ore[Type] = true;
			Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
			Main.tileOreFinderPriority[Type] = 400; // Metal Detector value, see https://terraria.gamepedia.com/Metal_Detector
			Main.tileShine2[Type] = true; // Modifies the draw color slightly.
			Main.tileShine[Type] = 975; // How often tiny dust appear off this tile. Larger is less frequently
			Main.tileMergeDirt[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("VerianOre");
			AddMapEntry(new Color(175, 0, 0), name);

			DustType = 84;
			DustType = DustID.Firework_Yellow;
			DustType = DustID.FlameBurst;
			RegisterItemDrop(ModContent.ItemType<Items.Ores.VerianOre>());
			HitSound = SoundID.Tink;
		 MineResist = 2f;
		 MinPick = 65;
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

		public override bool CanExplode(int i, int j) => false;
	}
}