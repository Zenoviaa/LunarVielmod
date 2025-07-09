using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Content.Tiles.IceTiles
{
    //Wall Version
    public class IceyAlterBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<IceyAlter>();
        }
    }

    internal class IceyAlter : DecorativeWall
    {
        public override void SetStaticDefaults()
        {

            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }
}
