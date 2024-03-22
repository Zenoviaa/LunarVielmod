using Microsoft.Xna.Framework;
using Terraria;

namespace Stellamod.Helpers
{
    public class ParallaxHelper : ForegroundItem
    {
        public float parallax = 0;

        public ParallaxHelper(Vector2 pos, Vector2 vel, float sc, string path) : base(pos, vel, sc, path)
        {
        }

        public virtual Vector2 ParallaxPosition()
        {
            Vector2 playerPos = Main.screenPosition + new Vector2(0, Main.LocalPlayer.gfxOffY);
            Vector2 offset = (playerPos - Center) * -parallax;
            return offset;
        }
    }
}