using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Content.Tiles.IceTiles
{
    //Wall Version
    public class SmallIceyStoneBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SmallIceyStone>();
        }
    }

    internal class SmallIceyStone : DecorativeWall
    {
        public override void SetStaticDefaults()
        {

            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }
}
