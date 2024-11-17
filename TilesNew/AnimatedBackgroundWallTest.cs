using Stellamod.Tiles;
using Terraria.ModLoader;

namespace Stellamod.TilesNew
{
    internal class AnimatedBackgroundWallTestItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<AnimatedBackgroundWallTest>();
        }
    }

    internal class AnimatedBackgroundWallTest : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //Number of frames in the animation
            FrameCount = 15;

            //How fast the animation is
            FrameSpeed = 30;

            //If this is set to true, wall tiles will offset their animation so they're not all synced
            DesyncAnimations = false;
        }
    }
}
