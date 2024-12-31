
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
    public class LostScrapT : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;
            HitSound = SoundID.Tink;
            AddMapEntry(new Color(20, 20, 50));
            RegisterItemDrop(ModContent.ItemType<LostScrap>());
            Main.tileMerge[TileID.Dirt][Type] = true;
            Main.tileMerge[TileID.Stone][Type] = true;
            Main.tileMerge[TileID.ClayBlock][Type] = true;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Lost Scrap");
        }



        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
        }
    }
}