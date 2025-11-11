using Stellamod.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.TilesCL
{

    public class GintzeCage1Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<GintzeCage1>();
        }
    }

    public class GintzeCage1 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            StructureColor = BackgroundColor;

        }
    }

    public class GintzeCage2Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<GintzeCage2>();
        }
    }


    public class GintzeCage2 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            StructureColor = BackgroundColor;

        }
    }


    public class GintzeCage3Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<GintzeCage3>();
        }
    }


    public class GintzeCage3 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            StructureColor = BackgroundColor;

        }
    }


    public class GintzeCage4Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<GintzeCage4>();
        }
    }


    public class GintzeCage4 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            StructureColor = BackgroundColor;

        }
    }

    public class GintzeCage5Item : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<GintzeCage5>();
        }
    }

    public class GintzeCage5 : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            StructureColor = BackgroundColor;

        }
    }

}
