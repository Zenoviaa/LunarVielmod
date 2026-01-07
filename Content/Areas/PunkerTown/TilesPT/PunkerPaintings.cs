using Stellamod.Content.Areas.Dungeon.TilesDG;
using Stellamod.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.TilesPT
{

    public class PaintingSunsetItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<PaintingSunset>();
        }
    }
    public class PaintingComputerItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<PaintingComputer>();
        }
    }
    public class PaintingBeachItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<PaintingBeach>();
        }
    }
    public class PaintingSnowForestItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<PaintingSnowForest>();
        }
    }
    public class PaintingCityWallItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<PaintingCity>();
        }
    }

    public abstract class AbstractPaintingWall : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Origin = DrawOrigin.Center;
        }
    }

    public class PaintingSunset : AbstractPaintingWall { }
    public class PaintingComputer : AbstractPaintingWall { }
    public class PaintingSnowForest : AbstractPaintingWall { }
    public class PaintingBeach : AbstractPaintingWall { }
    public class PaintingCity : AbstractPaintingWall { }
}
