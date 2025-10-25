using Stellamod.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria;

namespace Stellamod.Content.Areas.Cinderspark.TilesCS
{
    public class ManorPillarItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<ManorPillar>();
        }
    }
    public class ManorPillar : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
    }

}
