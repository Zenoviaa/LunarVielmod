using Stellamod.Helpers;
using Stellamod.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpecialTiles.EffectTiles
{
    public class TheUrdveilDoorItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<TheUrdveilDoor>();
        }
    }

    public class TheUrdveilDoor : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //Number of frames in the animation
            FrameCount = 1;

            Origin = DrawOrigin.BottomUp;
        }
    }
}
