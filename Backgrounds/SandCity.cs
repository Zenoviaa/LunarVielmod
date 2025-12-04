using Microsoft.Xna.Framework;
using Stellamod.Core.Foreground;
using Terraria;

namespace Stellamod.Backgrounds
{
    public class SandCity : ForegroundLayer
    {
        public override bool IsActive()
        {
            return Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneUndergroundDesert;
        }
        public override void SetLayering(ref float zLayer, ref Vector2 parallax)
        {
            base.SetLayering(ref zLayer, ref parallax);
            parallax.X = 0.5f;
            parallax.Y = 0.2f;
        }
    }
}
