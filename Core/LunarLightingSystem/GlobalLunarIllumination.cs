using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{
    public class GlobalLumSystem : ModSystem
    {

        public static float GlobalLightStrength;
        public static float GlobalLum = 0.3f;
        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            if (Main.LocalPlayer.ZoneUnderworldHeight)
            {
                GlobalLum = 1f;
            }
            GlobalLightStrength = MathHelper.Lerp(GlobalLightStrength, GlobalLum, 0.1f);
            GlobalLum = 0.3f;
        }
    }

    public class GlobalLunarIllumination : GlobalWall
    {

        public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
        {
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.BeamingLights)
                return;
          
            if (!Main.tile[i, j].HasTile)
            {
                float lightStrength = GlobalLumSystem.GlobalLightStrength;
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
