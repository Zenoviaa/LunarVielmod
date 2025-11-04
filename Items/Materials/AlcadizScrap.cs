using Stellamod.Content;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
    public class AlcadizScrap : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ModContent.RarityType<FableScrapRarity>();
        }
    }
}
