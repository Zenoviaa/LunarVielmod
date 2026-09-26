using Microsoft.Xna.Framework;
using Stellamod.Content.CommonMaterials;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
    public class CindersparkDirt : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[TileID.IceBlock][Type] = true;
            Main.tileMerge[TileID.SnowBlock][Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(100, 25, 40));

            MineResist = 3f;
            MinPick = 65;
            // name.SetDefault("Arnchar");
            RegisterItemDrop(ModContent.ItemType<Cinderscrap>());
        }

        public override void RandomUpdate(int i, int j)
        {

        }
    }
}