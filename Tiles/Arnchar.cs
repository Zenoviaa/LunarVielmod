
using Stellamod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Tiles
{
    public class Arnchar : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[this.Type] = true;

            HitSound = SoundID.DD2_CrystalCartImpact;
            DustType = DustID.Copper;
            AddMapEntry(new Color(247, 118, 34));
            Main.tileMerge[TileID.Dirt][Type] = true;
            Main.tileMerge[TileID.Stone][Type] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileMerge[TileID.ClayBlock][Type] = true;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Arnchar");
            RegisterItemDrop(ModContent.ItemType<ArncharChunk>());
        }




        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
 
        }
    }
}