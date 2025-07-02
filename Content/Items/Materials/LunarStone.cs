using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Materials
{
    internal class LunarStone : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 20; // The item texture's width
            Item.height = 20; // The item texture's height
            Item.rare = ItemRarityID.Green;
            Item.maxStack = Item.CommonMaxStack; // The item's max stack value
            Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
            Item.rare = ItemRarityID.LightRed;
        }
    }

    internal class RareLunarStone : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 20; // The item texture's width
            Item.height = 20; // The item texture's height
            Item.rare = ItemRarityID.Green;
            Item.maxStack = Item.CommonMaxStack; // The item's max stack value
            Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
            Item.rare = ItemRarityID.LightRed;
        }
    }

    internal class AncientLunarStone : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 20; // The item texture's width
            Item.height = 20; // The item texture's height
            Item.rare = ItemRarityID.Green;
            Item.maxStack = Item.CommonMaxStack; // The item's max stack value
            Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
            Item.rare = ItemRarityID.LightRed;
        }
    }
}
