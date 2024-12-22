using Stellamod.Tiles.Structures.Cathedral;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeable.Cathedral
{
    public class Crystal2I : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crytsal II");
            // Tooltip.SetDefault("A giant crystal");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Crystal2>());
            Item.value = 150;
            Item.maxStack = 20;
            Item.width = 38;
            Item.height = 24;
        }
    }
}