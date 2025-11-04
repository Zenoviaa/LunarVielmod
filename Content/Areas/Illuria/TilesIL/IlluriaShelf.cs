using Stellamod.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.TilesIL
{

    public class IlluriaShelf1Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<IlluriaShelf1>();
        }
    }
    public class IlluriaShelf1 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();


            StructureColor = BackgroundColor;
        }
    }



    public class IlluriaShelf2Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<IlluriaShelf2>();
        }
    }
    public class IlluriaShelf2 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            StructureColor = BackgroundColor;

        }
    }




    public class IlluriaShelf3Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<IlluriaShelf3>();
        }
    }
    public class IlluriaShelf3 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();


            StructureColor = BackgroundColor;
        }
    }







    public class IlluriaShelf4Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<IlluriaShelf4>();
        }
    }
    public class IlluriaShelf4 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();


            StructureColor = BackgroundColor;
        }
    }
}
