using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{
    public class GlobalLunarIllumination : GlobalWall
    {
        public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
        {
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.BeamingLights)
                return;
            if (!Main.tile[i, j].HasTile)
            {
                float lightStrength =1;
                if (lightStrength > 0)
                {
                    r = MathHelper.Clamp(r + lightStrength, 0, 1);
                    g = MathHelper.Clamp(g + lightStrength, 0, 1);
                    b = MathHelper.Clamp(b + lightStrength, 0, 1);
                }
            }

        }
    }
}
