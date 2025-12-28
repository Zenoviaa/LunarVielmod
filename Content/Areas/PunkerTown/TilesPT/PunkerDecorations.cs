using Stellamod.Tiles;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.TilesPT
{
    public class PunkerPillarVaseItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<PunkerPillarVase>();
        }
    }

    public class PunkerPillarVase : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Origin = DrawOrigin.BottomUp;
        }
    }
}
