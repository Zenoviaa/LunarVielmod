using Stellamod.Tiles;
using Terraria;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.SpecialTiles.EffectTiles
{
    public class SpectralEntranceItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpectralEntrance>();
        }
    }

    public class SpectralEntrance : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //Number of frames in the animation
            FrameCount = 40;
            HorizontalFrameCount = 5;
            VerticalFrameCount = 8;
            Origin = DrawOrigin.BottomUp;
            Alpha = 0.5f;
            //How fast the animation is
            FrameSpeed = 30;

            //Draw Scale
            DrawScale = 2;

            //If this is set to true, wall tiles will offset their animation so they're not all synced
            DesyncAnimations = false;
            BlackIsTransparency = true;
        }

    }
}
