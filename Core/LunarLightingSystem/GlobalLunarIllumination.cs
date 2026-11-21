using Stellamod.Content.Biomes;
using Terraria;
using Terraria.ID;
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
            GlobalLum = 0.15f;
            if (Main.LocalPlayer.ZoneHarmonicCoralways)
            {
                GlobalLum = 0.3f;
            }

            if (Main.LocalPlayer.ZoneUnderworldHeight)
            {
                GlobalLum = 0.1f;
            }
            if (Main.LocalPlayer.ZoneMoonspiralTower)
            {
                GlobalLum = 0.5f;
            }
            if (Main.LocalPlayer.ZoneDrakonic || Main.LocalPlayer.ZoneCinder)
            {
                GlobalLum = 0.65f;
            }
            GlobalLightStrength = MathHelper.Lerp(GlobalLightStrength, GlobalLum, 0.1f);
        }
    }

    public class GlobalLunarIllumination : GlobalWall
    {
        public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
        {
            //We should not be calling the ModContent.Get methods as much as possible, cache them.
            //var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!LunarVeilClientConfig.Instance.BeamingLights)
                return;
            Tile tile = Main.tile[i, j];
            if (tile.HasTile)
                return;
            if (tile.WallType > WallID.None)
                return;


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
