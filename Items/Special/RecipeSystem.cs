using Microsoft.Xna.Framework;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Special
{
    // This file shows a very simple example of a GlobalItem class. GlobalItem hooks are called on all items in the game and are suitable for sweeping changes like
    // adding additional data to all items in the game. Here we simply adjust the damage of the Copper Shortsword item, as it is simple to understand.
    // See other GlobalItem classes in ExampleMod to see other ways that GlobalItem can be used.
    internal class RecipeSystem : ModSystem
    {
        public override void PostAddRecipes()
        {
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];
                if (recipe.TryGetResult(ItemID.MythrilAnvil, out Item result))
                {
                    recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 15);
                }

                if (recipe.TryGetResult(ItemID.OrichalcumAnvil, out Item result2))
                {
                    recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 15);
                }

                if (recipe.TryGetResult(ItemID.Zenith, out Item result3))
                {
                    recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 25);
                    recipe.AddIngredient(ModContent.ItemType<ManifestedLove>(), 1);
                }
            }
        }
      

    }
}