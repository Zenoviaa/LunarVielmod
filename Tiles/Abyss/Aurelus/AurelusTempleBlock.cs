using Microsoft.Xna.Framework;
using Stellamod.Items.Placeable.Cathedral;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Tiles.Abyss.Aurelus
{
    public class AurelusTempleBlock : ModTile
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
            RegisterItemDrop(ItemType<AurelusTempleTile>());
            AddMapEntry(new Color(6, 5, 7));
            MineResist = 8f;
            MinPick = 200;

        }


        public override bool CanExplode(int i, int j) => false;
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
        }
    }
}