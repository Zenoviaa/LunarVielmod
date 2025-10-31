using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Materials
{
    public class Mushroom : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ModContent.RarityType<SpringMushroomRarity>();
        }
    }
}
