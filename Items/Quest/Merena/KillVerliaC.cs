using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Quest.Merena
{
    internal class KillVerliaC : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20; // The item texture's width
            Item.height = 20; // The item texture's height

            Item.maxStack = 1; // The item's max stack value
            Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
            Item.rare = ItemRarityID.Quest;
            Item.questItem = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<StolenMagicTome>(), 1);
            recipe.AddIngredient(ModContent.ItemType<AlcaricMush>(), 15);
            recipe.AddIngredient(ModContent.ItemType<KillVerlia>(), 1);
            recipe.Register();
        }
    }
}
