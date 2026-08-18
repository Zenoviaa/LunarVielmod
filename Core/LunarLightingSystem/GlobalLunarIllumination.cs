using Microsoft.Xna.Framework;
using Stellamod.Content.Biomes;
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
            GlobalLum = 0.15f;
            BiomePlayer biomePlayer = Main.LocalPlayer.GetModPlayer<BiomePlayer>();
            if (biomePlayer.ZoneHarmonicCoralways)
            {
                GlobalLum = 0.3f;
            }

            if (Main.LocalPlayer.ZoneUnderworldHeight)
            {
                GlobalLum = 0.1f;
            }
            if (biomePlayer.ZoneMoonspiralTower)
            {
                GlobalLum = 0.5f;
            }
        

            if(Main.LocalPlayer.GetModPlayer<MyPlayer>().ZoneDrakonic || Main.LocalPlayer.GetModPlayer<MyPlayer>().ZoneCinder)
            {
                GlobalLum = 0.5f;
            }
            GlobalLightStrength = MathHelper.Lerp(GlobalLightStrength, GlobalLum, 0.1f);
  
        }
    }

    public class GlobalLunarIllumination : GlobalWall
    {

        public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
        {
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.BeamingLights)
                return;

          
            Tile tile = Main.tile[i, j];
            if (tile.HasTile)
                return;
            if (tile.WallType > 0)
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
