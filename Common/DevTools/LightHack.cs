using Terraria.ModLoader;

namespace Stellamod.Common.DevTools;

public class LightHack : GlobalWall
{
    public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
    {
        base.ModifyLight(i, j, type, ref r, ref g, ref b);
        if (DevTools.LightIntensity <= 0)
            return;
        r = MathHelper.Clamp(r + DevTools.LightIntensity, 0f, 1f);
        g = MathHelper.Clamp(g + DevTools.LightIntensity, 0f, 1f);
        b = MathHelper.Clamp(b + DevTools.LightIntensity, 0f, 1f);

    }
}
