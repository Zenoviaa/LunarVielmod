using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{
    public class TileMergeEdit : ModSystem
    {
        public override void PostAddRecipes()
        {
            base.PostAddRecipes();
            for (int i = 0; i < Main.tileMerge.Length; i++)
            {
                for (int j = 0; j < Main.tileMerge.Length; j++)
                {
                    Main.tileMerge[i][j] = true;
                    Main.tileMerge[j][i] = true;
                }
            }
        }
        public override void OnModLoad()
        {
            base.OnModLoad();
            /*
            foreach(var tile in ModContent.GetContent<ModTile>())
            {
                Main.tileBlendAll[tile.Type] = true;
                TileID.Sets.BlockMergesWithMergeAllBlockOverride[tile.Type] = true;
            }*/
        }
    }
}
