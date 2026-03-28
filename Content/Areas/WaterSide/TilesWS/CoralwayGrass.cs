using Stellamod.Core.Grass;
using Stellamod.Helpers;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.WaterSide.TilesWS;

public class CoralwayGrass : GrassProfile
{
    private UnifiedRandom _random;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();


        frameCount = 6;
        maxHeight = 90;
        maxWidth = 4.4f;
        maxExtraBladesPerPatch = 1;
        minBladesPerPatch = 1;
        grassColor = new Color(80, 107, 26);

        //RegisterReed<CatTail>();
        // RegisterReed<WildFlower>();
    }

    public override GrassProfile GetVariantProfile(int i, int j)
    {
        minBladesPerPatch = 1;
        maxExtraBladesPerPatch = 3;
        dontRenderPrimGrasses = true;
        _random ??= new UnifiedRandom();

        float x = i;
        _random.SetSeed(i * 8);
        int c = (int)ExtraMath.Osc(0f, 2f, 0, i);
        return base.GetVariantProfile(i, j);
    }
}