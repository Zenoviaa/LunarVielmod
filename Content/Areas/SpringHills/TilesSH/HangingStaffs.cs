using Stellamod.Tiles;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.TilesSH
{
    public abstract class HangingStaff : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Origin = DrawOrigin.TopDown;
            //idk
            WindSwayOffset = 0f;

            //The max it can sway
            WindSwayMagnitude = 0.2f;

            //How fast it sways
            WindSwaySpeed = 0.02f;
        }
    }
    public class HangingStaff1Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<HangingStaff1>();
        }
    }
    public class HangingStaff1 : HangingStaff
    {

    }
    public class HangingStaff2Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<HangingStaff2>();
        }
    }
    public class HangingStaff2 : HangingStaff
    {

    }
    public class HangingStaff3Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<HangingStaff3>();
        }
    }
    public class HangingStaff3 : HangingStaff
    {

    }
    public class HangingStaff4Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<HangingStaff4>();
        }
    }
    public class HangingStaff4 : HangingStaff
    {

    }
    public class HangingStaff5Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<HangingStaff5>();
        }
    }
    public class HangingStaff5 : HangingStaff
    {

    }
    public class HangingStaff6Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<HangingStaff6>();
        }
    }
    public class HangingStaff6 : HangingStaff
    {

    }
    public class HangingStaff7Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<HangingStaff7>();
        }
    }
    public class HangingStaff7 : HangingStaff
    {

    }
}
