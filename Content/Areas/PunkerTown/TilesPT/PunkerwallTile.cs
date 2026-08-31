
using Microsoft.Xna.Framework;
using Stellamod.Content.Dusts;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.TilesPT
{
    public class PunkerwallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;

            DustType = ModContent.DustType<Solution>();
            RegisterItemDrop(ModContent.ItemType<PunkWallBlock>());

            AddMapEntry(new Color(200, 200, 200));
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}