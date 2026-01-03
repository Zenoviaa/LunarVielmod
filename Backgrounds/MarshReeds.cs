using Microsoft.Xna.Framework;
using Stellamod.Content.Biomes;
using Stellamod.Core.Foreground;
using Stellamod.WorldG;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Backgrounds
{
    public class MarshReeds : ForegroundLayer
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            drawOffset = new Vector2(0, -150);
        }
        public override bool IsActive()
        {
            return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneMarsh;
        }
        public override void SetLayering(ref float zLayer, ref Vector2 parallax)
        {
            base.SetLayering(ref zLayer, ref parallax);
            parallax.X = 1.2f;
            parallax.Y = 1;
            drawOffset = new Vector2(0, -1700);
        }
        public override float GetFloorY()
        {
            Point marshFloor = ModContent.GetInstance<StellaWorld>().MarshLocation;
            return marshFloor.ToWorldCoordinates().Y;
        }
    }
}
