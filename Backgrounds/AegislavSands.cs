using Stellamod.Content.Biomes;
using Stellamod.Core.Foreground;
using Terraria;

namespace Stellamod.Backgrounds
{
    public class AegislavSands : ForegroundLayer
    {
        public override bool IsActive()
        {
            return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneAegislavSurface;
        }
        public override void SetLayering(ref float zLayer, ref Vector2 parallax)
        {
            base.SetLayering(ref zLayer, ref parallax);
            parallax.X = 1.2f;
            parallax.Y = 2;
            drawAlpha = 1f;
        }
    }
}
