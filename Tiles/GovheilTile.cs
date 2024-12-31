
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
    public class GovheilTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			
			Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
			Main.tileOreFinderPriority[Type] = 410; // Metal Detector value, see https://terraria.gamepedia.com/Metal_Detector
 // Modifies the draw color slightly.
 // How often tiny dust appear off this tile. Larger is less frequently
			Main.tileMergeDirt[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			MineResist = 2f;
			MinPick = 225;

			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Huntiac Silk");
			AddMapEntry(new Color(36, 31, 27), name);

			
		
			RegisterItemDrop(ModContent.ItemType<Items.Materials.HuntiacBlock>());
			HitSound = SoundID.Tink;
			
		}
		public override bool CanExplode(int i, int j) => false;
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
		
	}
}