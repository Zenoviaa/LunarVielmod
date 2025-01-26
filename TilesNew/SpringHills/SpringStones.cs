using Microsoft.Xna.Framework;
using Stellamod.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.TilesNew.SpringHills
{
    //Wall Version
    public class StoneUnderBlock : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<StoneUnder>();
        }
    }

    internal class StoneUnder : DecorativeWall
    {
        public override void SetStaticDefaults()
        {

            base.SetStaticDefaults();
            Origin = DrawOrigin.TopDown;
            StructureColor = Color.Gray;
            //If you need other static defaults it go here
        }
    }
}
