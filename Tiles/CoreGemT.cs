using Stellamod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Tiles
{
	public class CoreGemT : ModTile
	{
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[this.Type] = true;

            HitSound = SoundID.DD2_CrystalCartImpact;
            DustType = DustID.Copper;

            Main.tileMerge[TileID.Dirt][Type] = true;
            Main.tileMerge[TileID.Stone][Type] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileMerge[TileID.ClayBlock][Type] = true;
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(0, 100, 120), name);
            // name.SetDefault("Arnchar");
            RegisterItemDrop(ModContent.ItemType<CoreGem>());
        }
        public override bool CanExplode(int i, int j)
		{
			return false;
		}
	}
}