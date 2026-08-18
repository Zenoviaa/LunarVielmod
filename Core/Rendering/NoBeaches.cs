using Stellamod.Content.Biomes;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Rendering;

[Autoload(Side = ModSide.Client)]
public class NoBeaches : ModSystem
{
    public override void Load()
    {
        base.Load();
        On_Player.CanSeeShimmerEffects += RemoveBeachWater;
        On_WorldGen.oceanDepths += RemoveBeachWater;
    }

    private bool RemoveBeachWater(On_Player.orig_CanSeeShimmerEffects orig, Player self)
    {
        if(self.GetModPlayer<BiomePlayer>().ZoneHarmonicCoralways)
        {
            self.ZoneBeach = false;
        }
        return orig(self);
    }

    private bool RemoveBeachWater(On_WorldGen.orig_oceanDepths orig, int x, int y)
    {
        if (Main.gameMenu)
            return orig(x, y);
        if (Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneHarmonicCoralways)
            return false;
        return orig(x, y);
    }
}
