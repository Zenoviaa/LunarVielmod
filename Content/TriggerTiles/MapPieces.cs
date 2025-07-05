using Stellamod.Content.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Content.TriggerTiles
{

    internal abstract class BaseMapPiece : DecorativeWall
    {
        public override string Texture => (typeof(BaseMapPiece).FullName + "_S").Replace(".", "/");
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
    }
    public class SpringHillsInnerPieceItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringHillsInnerPiece>();
        }
    }

    internal class SpringHillsInnerPiece : BaseMapPiece
    {

    }
    public class SpringHillsOuterPieceItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<SpringHillsOuterPiece>();
        }
    }
    internal class SpringHillsOuterPiece : BaseMapPiece
    {

    }
    public class WarriorsDoorPieceItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<WarriorsDoorPiece>();
        }
    }
    internal class WarriorsDoorPiece : BaseMapPiece
    {

    }
    public class WitchTownPieceItem : DecorativeWallItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<WitchTownPiece>();
        }
    }
    internal class WitchTownPiece : BaseMapPiece
    {

    }
}
