using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
    //Wall Version
    public class ExampleDecorativeWallItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<ExampleDecorativeWall>();
        }
    }

    internal class ExampleDecorativeWall : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            StructureColor = Color.Gray;
            Origin = DrawOrigin.BottomUp;
            base.SetStaticDefaults();
            //If you need other static defaults it go here
        }
    }

}
