
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
    public class FrileOreTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.Ore[Type] = true;
			Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
			Main.tileOreFinderPriority[Type] = 390; // Metal Detector value, see https://terraria.gamepedia.com/Metal_Detector
			Main.tileShine2[Type] = true; // Modifies the draw color slightly.
			Main.tileShine[Type] = 300; // How often tiny dust appear off this tile. Larger is less frequently
			Main.tileMergeDirt[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Frile Ore");
			AddMapEntry(new Color(0, 19, 187), name);

			DustType = 84;
			DustType = DustID.Firework_Blue;
			DustType = DustID.BlueCrystalShard;
			RegisterItemDrop(ModContent.ItemType<Items.Ores.FrileOre>());
			HitSound = SoundID.DD2_CrystalCartImpact;
		 MineResist = 2f;
		 MinPick = 20;
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
		
	}
}