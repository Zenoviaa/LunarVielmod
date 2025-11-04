using Stellamod.Tiles;
using System.Drawing;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.TilesIL
{
    public class IlluriaBannerItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<IlluriaBanner>();
        }
    }
    public class IlluriaBanner : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Origin = DrawOrigin.TopDown;

            StructureColor = BackgroundColor;
        }
    }


    public class IlluriaWallsItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<IlluriaWalls>();
        }
    }
    public class IlluriaWalls : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
   
            StructureColor = BackgroundColor;
        }
    }
}
