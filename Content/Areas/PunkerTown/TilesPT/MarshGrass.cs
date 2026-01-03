using Microsoft.Xna.Framework;
using Stellamod.Core.Grass;
using Stellamod.Helpers;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.TilesPT
{
    public class MarshGrass : GrassProfile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            maxHeight = 90;
            maxWidth = 4.4f;
            maxExtraBladesPerPatch = 3;
            minBladesPerPatch = 2;
            grassColor = new Color(80, 107, 26);
        }

        public override void Grow(int i, int j)
        {
            base.Grow(i, j);
            int c = (int)ExtraMath.Osc(0f, 2f, 0, i);
            if (c == 0)
                ModContent.GetInstance<GrassRenderer>().AddReed<CatTail>();
        }
    }
}
