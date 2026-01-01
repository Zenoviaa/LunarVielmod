using Microsoft.Xna.Framework;
using Stellamod.Content.Biomes;
using Stellamod.Core.Foreground;
using Terraria;

namespace Stellamod.Backgrounds
{
    public class MarshReeds : ForegroundLayer
    {
        public override bool IsActive()
        {
            return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneMarsh;
        }
        public override void SetLayering(ref float zLayer, ref Vector2 parallax)
        {
            base.SetLayering(ref zLayer, ref parallax);
            parallax.X = 1.2f;
            parallax.Y = 1;
        }
    }
}
