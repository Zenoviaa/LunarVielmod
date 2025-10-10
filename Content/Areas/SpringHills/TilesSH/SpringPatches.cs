using Stellamod.Tiles;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.TilesSH
{
    public class SpringPatch1Item : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringPatch1>();
        }
    }
    public class SpringPatch1 : DecorativeWall
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

    public class SpringPatch2Item : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringPatch2>();
        }
    }
    public class SpringPatch2 : DecorativeWall
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

    public class SpringPatch3Item : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringPatch3>();
        }
    }
    public class SpringPatch3 : DecorativeWall
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

