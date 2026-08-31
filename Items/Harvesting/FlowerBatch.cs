using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Harvesting
{
    public class FlowerBatch : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(silver: 50);
        }
    }
}
