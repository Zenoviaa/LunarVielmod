using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Light;
using Terraria.Map;
using Terraria.ModLoader;

namespace Stellamod.Core
{
    public class LightingChanges : ModSystem
    {
        public override void OnModLoad()
        {
            base.OnModLoad();
            On_WorldMap.UpdateLighting += NoUpdate;
            On_Main.DoLightTiles += _Main_DoLightTiles;
            On_LightingEngine.ProcessBlur += _LightingEngine_ProcessBlur;
            On_LightMap.Blur += _LightMap_Blur;
            On_TileLightScanner.ApplySurfaceLight += _TileLightScanner_ApplySurfaceLight;
            On_TileLightScanner.ApplyHellLight += _TileLightScanner_ApplyHellLight;
            On_TileLightScanner.ApplyLiquidLight += _TileLightScanner_ApplyLiquidLight;
        }

        private void _TileLightScanner_ApplyLiquidLight(On_TileLightScanner.orig_ApplyLiquidLight orig, TileLightScanner self, Tile tile, ref Vector3 lightColor)
        {
           
        }

        private void _TileLightScanner_ApplyHellLight(On_TileLightScanner.orig_ApplyHellLight orig, TileLightScanner self, Tile tile, int x, int y, ref Vector3 lightColor)
        {
           
        }

        private void _TileLightScanner_ApplySurfaceLight(On_TileLightScanner.orig_ApplySurfaceLight orig, TileLightScanner self, Tile tile, int x, int y, ref Vector3 lightColor)
        {
       
        }

        private void _Main_DoLightTiles(On_Main.orig_DoLightTiles orig, Main self)
        {

        }

        private void _LightMap_Blur(On_LightMap.orig_Blur orig, LightMap self)
        {
         
        }

        private void _LightingEngine_ProcessBlur(On_LightingEngine.orig_ProcessBlur orig, LightingEngine self)
        {
           
        }

        private bool NoUpdate(On_WorldMap.orig_UpdateLighting orig, WorldMap self, int x, int y, byte light)
        {
            return false;
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
            On_WorldMap.UpdateLighting -= NoUpdate;
            On_Main.DoLightTiles -= _Main_DoLightTiles;
            On_LightingEngine.ProcessBlur -= _LightingEngine_ProcessBlur;
            On_LightMap.Blur -= _LightMap_Blur;
            On_TileLightScanner.ApplySurfaceLight -= _TileLightScanner_ApplySurfaceLight;
            On_TileLightScanner.ApplyHellLight -= _TileLightScanner_ApplyHellLight;
            On_TileLightScanner.ApplyLiquidLight -= _TileLightScanner_ApplyLiquidLight;
        }
    }
}
