using Stellamod.Tiles;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.TilesUG
{
    public class OpenChestItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<OpenChest>();
        }
    }

    public class OpenChest : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Origin = DrawOrigin.BottomUp;
        }
    }
    public class ShaftLadderItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<ShaftLadder>();
        }
    }

    public class ShaftLadder : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Origin = DrawOrigin.BottomUp;
        }
    }
}
