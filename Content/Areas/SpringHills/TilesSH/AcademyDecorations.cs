using Stellamod.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.TilesSH
{
    public class WitchAcademyPosterItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<WitchAcademyPoster>();
        }
    }

    public class WitchAcademyPoster : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Origin = DrawOrigin.Center;
            StructureColor = BackgroundColor;
        }
    }

    public class WitchAcademyBannerItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<WitchAcademyPoster>();
        }
    }

    public class WitchAcademyBanner : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Origin = DrawOrigin.TopDown;
            StructureColor = BackgroundColor;
        }
    }

    public class WitchAcademyBookshelfItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<WitchAcademyBookshelf>();
        }
    }

    public class WitchAcademyBookshelf : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Origin = DrawOrigin.BottomUp;
            StructureColor = BackgroundColor;
        }
    }
}
