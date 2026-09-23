using Microsoft.Xna.Framework;
using Stellamod.Core.Grass;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.PunkerTown.TilesPT
{
    public class TallGrass : GrassProfile
    {
        private UnifiedRandom _random;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();


            frameCount = 4;
            maxHeight = 90;
            maxWidth = 4.4f;
            minBladesPerPatch = 2;
            maxExtraBladesPerPatch = 6;
            grassColor = new Color(80, 107, 26);

            RegisterReed<CatTail>();
            RegisterReed<WildFlower>();
        }

        public override GrassProfile GetVariantProfile(int i, int j)
        {
            var h = i;
            if (h % 16 == 0)
                return ModContent.GetInstance<WildBush>();

            if (h % 2 == 0)
                return ModContent.GetInstance<TallerGrass>();
       
            return base.GetVariantProfile(i, j);
        }
    }
    public class TallerGrass : GrassProfile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            frameCount = 3;
            maxHeight = 90;
            maxWidth = 4.4f;
            minBladesPerPatch = 1;
            maxExtraBladesPerPatch = 3;
            grassColor = new Color(80, 107, 26);

            RegisterReed<CatTail>();
        }
    }
    public class WildBush : GrassProfile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            frameCount = 4;
            maxHeight = 90;
            maxWidth = 4.4f;
            maxExtraBladesPerPatch = 0;
            minBladesPerPatch = 1;
            grassColor = new Color(80, 107, 26);
        }
    }
}
