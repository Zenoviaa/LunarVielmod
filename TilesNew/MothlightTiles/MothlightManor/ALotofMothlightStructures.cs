using Microsoft.Xna.Framework;
using Stellamod.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.TilesNew.MothlightTiles.MothlightManor
{
    //Wall Version
    #region MothlightBanner
    public class MothlightBannerBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MothlightBanner>();
        }
    }

    public class MothlightBanner : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            Origin = DrawOrigin.TopDown;
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }

    #endregion

    #region MothlightBannerHanger
    public class MothlightBannerHangerBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MothlightBannerHanger>();
        }
    }

    public class MothlightBannerHanger : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            Origin = DrawOrigin.TopDown;
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }

    #endregion

    #region MothlightBannerSmall
    public class MothlightBannerSmallBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MothlightBannerSmall>();
        }
    }

    public class MothlightBannerSmall : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            Origin = DrawOrigin.TopDown;
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }

    #endregion

    #region MothlightBookshelf
    public class MothlightBookshelfBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MothlightBookshelf>();
        }
    }

    public class MothlightBookshelf : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }

    #endregion

    #region MothlightBowl
    public class MothlightBowlBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MothlightBowl>();
        }
    }

    public class MothlightBowl : DecorativeWall
    {
        public override void SetStaticDefaults()
        {

            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }

    #endregion

    #region MothlightChairLeft
    public class MothlightChairLeftBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MothlightChairLeft>();
        }
    }

    public class MothlightChairLeft : DecorativeWall
    {
        public override void SetStaticDefaults()
        {

            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }

    #endregion

    #region MothlightChairRight
    public class MothlightChairRightBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MothlightChairRight>();
        }
    }

    public class MothlightChairRight : DecorativeWall
    {
        public override void SetStaticDefaults()
        {

            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }

    #endregion

    #region MothlightClock
    public class MothlightClockBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MothlightClock>();
        }
    }

    public class MothlightClock : DecorativeWall
    {
        public override void SetStaticDefaults()
        {

            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }

    #endregion

    #region MothlightDoor
    public class MothlightDoorBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MothlightDoor>();
        }
    }

    public class MothlightDoor : DecorativeWall
    {
        public override void SetStaticDefaults()
        {

            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }

    #endregion

    #region MothlightFloor
    public class MothlightFloorBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MothlightFloor>();
        }
    }

    public class MothlightFloor : DecorativeWall
    {
        public override void SetStaticDefaults()
        {

            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }

    #endregion

    #region MothlightFlowerbad
    public class MothlightFlowerbedBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MothlightFlowerbed>();
        }
    }

    public class MothlightFlowerbed : DecorativeWall
    {
        public override void SetStaticDefaults()
        {

            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }

    #endregion

    #region MothlightHanger
    public class MothlightHangerBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MothlightHanger>();
        }
    }

    public class MothlightHanger : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            Origin = DrawOrigin.TopDown;
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }

    #endregion

    #region MothlightLeftStairs
    public class MothlightLeftStairsBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MothlightLeftStairs>();
        }
    }

    public class MothlightLeftStairs : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
           // Origin = DrawOrigin.TopDown;
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }

    #endregion

    #region MothlightRightStairs
    public class MothlightRightStairsBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MothlightRightStairs>();
        }
    }

    public class MothlightRightStairs : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            // Origin = DrawOrigin.TopDown;
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }

    #endregion

    #region MothlightTable
    public class MothlightTableBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MothlightTable>();
        }
    }

    public class MothlightTable : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            // Origin = DrawOrigin.TopDown;
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }

    #endregion

    #region MothlightWindow
    public class MothlightWindowBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MothlightWindow>();
        }
    }

    public class MothlightWindow : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            // Origin = DrawOrigin.TopDown;
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }

    #endregion

   

    #region MothlightBiggerTable
    public class MothlightHighTableBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MothlightHighTable>();
        }
    }

    public class MothlightHighTable : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            // Origin = DrawOrigin.TopDown;
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }

    #endregion

    #region MothlightPalendine
    public class MothlightPalendineBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MothlightPalendine>();
        }
    }

    public class MothlightPalendine : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            // Origin = DrawOrigin.TopDown;
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }

    #endregion
}
