using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
    public class VirulentPlating : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Virulent Plating");
            // Tooltip.SetDefault("Radiating within the acid of entities of the tocins");
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Orange;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
        }
    }
}