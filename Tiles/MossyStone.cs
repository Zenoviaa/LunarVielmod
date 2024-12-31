
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
    public class MossyStone : ModTile
	{
		public override void SetStaticDefaults()
		{
			// The tile will be affected by spelunker highlighting
			Main.tileOreFinderPriority[Type] = 10; // Metal Detector value, see https://terraria.gamepedia.com/Metal_Detector
		// How often tiny dust appear off this tile. Larger is less frequently
			Main.tileMergeDirt[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Huntiac Silk");
			AddMapEntry(new Color(90, 130, 50), name);

	
			DustType = DustID.Stone;
			DustType = DustID.Web;
			RegisterItemDrop(ModContent.ItemType<Items.Materials.MossyStones>());
			HitSound = SoundID.Grass;
			MineResist = 2f;
		}
        public override bool CanExplode(int i, int j) => true;
        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
		
	}
}