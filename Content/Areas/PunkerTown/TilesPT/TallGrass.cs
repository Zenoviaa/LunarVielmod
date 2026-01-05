using Microsoft.Xna.Framework;
using Stellamod.Core.Grass;
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
            maxExtraBladesPerPatch = 1;
            minBladesPerPatch = 1;
            grassColor = new Color(80, 107, 26);

            RegisterReed<CatTail>();
            RegisterReed<WildFlower>();
        }

        public override GrassProfile GetVariantProfile(int i, int j)
        {
            minBladesPerPatch = 2;
            maxExtraBladesPerPatch = 6;
            _random ??= new UnifiedRandom();

            float x = i;
            _random.SetSeed(i * 8);
            int c = (int)ExtraMath.Osc(0f, 2f, 0, i);

            if (_random.NextBool(2))
                return ModContent.GetInstance<TallerGrass>();
            if(_random.NextBool(16))
                return ModContent.GetInstance<WildBush>();

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
            maxExtraBladesPerPatch = 2;
            minBladesPerPatch = 1;
            grassColor = new Color(80, 107, 26);

            RegisterReed<CatTail>();
        }


        public override void Grow(int i, int j)
        {
            base.Grow(i, j);
            minBladesPerPatch = 1;
            maxExtraBladesPerPatch = 3;
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
