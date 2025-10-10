using Stellamod.Tiles;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.TilesSH
{
    public class SpringPotFlowerItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringPotFlower>();
        }
    }
    public class SpringPotFlower : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

        }
    }

    public class SpringPotFlowerBlueItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringPotFlowerBlue>();
        }
    }
    public class SpringPotFlowerBlue : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
    }

    public class SpringPotFlowerRedItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringPotFlowerRed>();
        }
    }

    public class SpringPotFlowerRed : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
    }

    public class SpringPotFlowerPinkItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringPotFlowerPink>();
        }
    }

    public class SpringPotFlowerPink : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

        }
    }
    public class HangingSpringPotFlowerItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<HangingSpringPotFlower>();
        }
    }
    public class HangingSpringPotFlower : DecorativeWall
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
    public class HangingSpringPotFlowerBlueItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<HangingSpringPotFlowerBlue>();
        }
    }
    public class HangingSpringPotFlowerBlue : DecorativeWall
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
    public class HangingSpringPotFlowerPinkItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<HangingSpringPotFlowerPink>();
        }
    }
    public class HangingSpringPotFlowerPink : DecorativeWall
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
    public class HangingSpringPotFlowerRedItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<HangingSpringPotFlowerRed>();
        }
    }
    public class HangingSpringPotFlowerRed : DecorativeWall
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
}
