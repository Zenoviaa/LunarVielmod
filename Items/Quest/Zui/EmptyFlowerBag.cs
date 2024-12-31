
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Quest.Zui
{
    internal class EmptyFlowerBag : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20; // The item texture's width
            Item.height = 20; // The item texture's height

            Item.maxStack = 99; // The item's max stack value
            Item.value = Item.buyPrice(copper: 50); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
            Item.rare = ItemRarityID.Quest;
            Item.questItem = true;
        }
    }
}
