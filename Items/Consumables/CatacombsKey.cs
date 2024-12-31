

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables
{
    public class CatacombsKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Electronic Death Remote (EDR)");
            // Tooltip.SetDefault("'that big red button probably will do something you’ll regret... \n Your conscience advises you to press it and see what happens!'");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 28;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Orange;
        }
    }
}