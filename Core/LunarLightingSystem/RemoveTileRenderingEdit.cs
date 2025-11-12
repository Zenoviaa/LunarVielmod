using Terraria.ModLoader;
using Terraria;

namespace Stellamod.Core.LunarLightingSystem
{
    [Autoload(Side = ModSide.Client)]
    public class RemoveTileRenderingEdit : ModSystem
    {
        public override void OnModLoad()
        {
            base.OnModLoad();

            On_Main.DoDraw_WallsAndBlacks += NoWallsAndBlacks;
            On_Main.DrawTiles += NoTilesDraw;
            On_Main.RenderTiles += NoTileRender;
            On_Main.RenderTiles2 += NoTileRender2;
            On_Main.DoDraw_Tiles_Solid += NoDrawTiles;
            On_Main.DoDraw_Tiles_NonSolid += NoDrawTiles;   
        }

     
        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.DoDraw_WallsAndBlacks -= NoWallsAndBlacks;
            On_Main.DrawTiles -= NoTilesDraw;
            On_Main.RenderTiles -= NoTileRender;
            On_Main.RenderTiles2 -= NoTileRender2;
            On_Main.DoDraw_Tiles_Solid -= NoDrawTiles;
            On_Main.DoDraw_Tiles_NonSolid -= NoDrawTiles;
        }
        private bool ShouldRemoveTileRendering()
        {
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (config.DisableTileRendering)
                return true;
            return false;

        }
        private void NoDrawTiles(On_Main.orig_DoDraw_Tiles_NonSolid orig, Main self)
        {
            if (ShouldRemoveTileRendering())
                return;
            orig(self);
        }

        private void NoWallsAndBlacks(On_Main.orig_DoDraw_WallsAndBlacks orig, Main self)
        {
            if (ShouldRemoveTileRendering())
                return;
            orig(self);
        }

        private void NoTilesDraw(On_Main.orig_DrawTiles orig, Main self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets, int waterStyleOverride)
        {
            if (ShouldRemoveTileRendering())
                return;
            orig(self, solidLayer, forRenderTargets, intoRenderTargets, waterStyleOverride);
        }
        private void NoDrawTiles(On_Main.orig_DoDraw_Tiles_Solid orig, Main self)
        {

            if (ShouldRemoveTileRendering())
                return;
            orig(self);
        }

        private void NoTileRender2(On_Main.orig_RenderTiles2 orig, Main self)
        {
            if (ShouldRemoveTileRendering())
                return;
            orig(self);
        }

        private void NoTileRender(On_Main.orig_RenderTiles orig, Main self)
        {
            if (ShouldRemoveTileRendering())
                return;
            orig(self);
        }
    }
}
