using Stellamod.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.TilesSH
{
    public class SpringFlowerItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringFlower>();
        }
    }
    public class SpringFlower : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            //idk
            WindSwayOffset = 0f;

            //The max it can sway
            WindSwayMagnitude = 0.2f;

            //How fast it sways
            WindSwaySpeed = 0.02f;
        }
    }

    public class SpringFlowerBlueBushItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringFlowerBlueBush>();
        }
    }
    public class SpringFlowerBlueBush : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            //idk
            WindSwayOffset = 0f;

            //The max it can sway
            WindSwayMagnitude = 0.2f;

            //How fast it sways
            WindSwaySpeed = 0.02f;
        }
    }

    public class SpringFlowerDarkPurpleItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringFlowerDarkPurple>();
        }
    }
    public class SpringFlowerDarkPurple : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            //idk
            WindSwayOffset = 0f;

            //The max it can sway
            WindSwayMagnitude = 0.2f;

            //How fast it sways
            WindSwaySpeed = 0.02f;
        }
    }


    public class SpringFlowerGrassItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringFlowerGrass>();
        }
    }
    public class SpringFlowerGrass : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            //idk
            WindSwayOffset = 0f;

            //The max it can sway
            WindSwayMagnitude = 0.2f;

            //How fast it sways
            WindSwaySpeed = 0.02f;
        }
    }

    public class SpringFlowerGrassSmallItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringFlowerGrassSmall>();
        }
    }
    public class SpringFlowerGrassSmall : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            //idk
            WindSwayOffset = 0f;

            //The max it can sway
            WindSwayMagnitude = 0.2f;

            //How fast it sways
            WindSwaySpeed = 0.02f;
        }
    }


    public class SpringFlowerPurpleBushItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringFlowerPurpleBush>();
        }
    }
    public class SpringFlowerPurpleBush : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            //idk
            WindSwayOffset = 0f;

            //The max it can sway
            WindSwayMagnitude = 0.2f;

            //How fast it sways
            WindSwaySpeed = 0.02f;
        }
    }

    public class SpringFlowerPurpleLeafItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringFlowerPurpleLeaf>();
        }
    }
    public class SpringFlowerPurpleLeaf : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            //idk
            WindSwayOffset = 0f;

            //The max it can sway
            WindSwayMagnitude = 0.2f;

            //How fast it sways
            WindSwaySpeed = 0.02f;
        }
    }
    public class SpringFlowerRedBushItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringFlowerRedBush>();
        }
    }
    public class SpringFlowerRedBush : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            //idk
            WindSwayOffset = 0f;

            //The max it can sway
            WindSwayMagnitude = 0.2f;

            //How fast it sways
            WindSwaySpeed = 0.02f;
        }
    }


    public class SpringFlowerVineItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringFlowerVine>();
        }
    }
    public class SpringFlowerVine : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            //idk
            WindSwayOffset = 0f;

            //The max it can sway
            WindSwayMagnitude = 0.2f;

            //How fast it sways
            WindSwaySpeed = 0.02f;
        }
    }

    public class SpringFlowerWhiteItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringFlowerWhite>();
        }
    }
    public class SpringFlowerWhite : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            //idk
            WindSwayOffset = 0f;

            //The max it can sway
            WindSwayMagnitude = 0.2f;

            //How fast it sways
            WindSwaySpeed = 0.02f;
        }
    }

    public class SpringFlowerWhiteBudItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringFlowerWhiteBud>();
        }
    }
    public class SpringFlowerWhiteBud : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            //idk
            WindSwayOffset = 0f;

            //The max it can sway
            WindSwayMagnitude = 0.2f;

            //How fast it sways
            WindSwaySpeed = 0.02f;
        }
    }

    public class SpringFlowerWhiteBudSmallItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringFlowerWhiteBudSmall>();
        }
    }
    public class SpringFlowerWhiteBudSmall : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            //idk
            WindSwayOffset = 0f;

            //The max it can sway
            WindSwayMagnitude = 0.2f;

            //How fast it sways
            WindSwaySpeed = 0.02f;
        }
    }
}
