using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{
    //TODO: Rewrite this and try implementing Radiance Cascades instead, might be really cool
    //I'll make a prototype elsewhere first though
    public class LightingPreDrawEdit : GlobalTile
    {
        public static bool DontRenderPreDraw;
        public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            if (type == TileID.FogMachine && DontRenderPreDraw)
                return false;
            if (type == TileID.FogMachine && NPC.AnyDanger())
                return false;
            return base.PreDraw(i, j, type, spriteBatch);
        }
    }
}
