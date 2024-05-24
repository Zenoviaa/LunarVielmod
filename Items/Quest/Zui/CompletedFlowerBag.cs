
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Quest.Zui
{
    internal class CompletedFlowerBag : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20; // The item texture's width
            Item.height = 20; // The item texture's height

            Item.maxStack = 1; // The item's max stack value
            Item.value = Item.buyPrice(gold: 3); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
            Item.rare = ItemRarityID.Quest;
            Item.questItem = true;
        }


        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe.AddIngredient(ItemID.Blinkroot, 20);
            recipe.AddIngredient(ItemID.Fireblossom, 3);
            recipe.AddIngredient(ModContent.ItemType<EmptyFlowerBag>(), 1);
            recipe.Register();



            Recipe recipe3 = CreateRecipe();
            recipe3.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe3.AddIngredient(ModContent.ItemType<Morrowshroom>(), 20);
            recipe3.AddIngredient(ModContent.ItemType<FlowerBatch>(), 1);
            recipe3.AddIngredient(ModContent.ItemType<EmptyFlowerBag>(), 1);
            recipe3.Register();

            Recipe recipe4 = CreateRecipe();
            recipe4.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe4.AddIngredient(ModContent.ItemType<Cinderscrap>(), 30);
            recipe4.AddIngredient(ModContent.ItemType<Stick>(), 5);
            recipe4.AddIngredient(ModContent.ItemType<CondensedDirt>(), 999);
            recipe4.AddIngredient(ModContent.ItemType<EmptyFlowerBag>(), 1);
            recipe4.Register();

            Recipe recipe5 = CreateRecipe();
            recipe5.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe5.AddIngredient(ModContent.ItemType<MorrowVine>(), 500);
            recipe5.AddIngredient(ModContent.ItemType<Mushroom>(), 30);
            recipe5.AddIngredient(ModContent.ItemType<Morrowshroom>(), 30);
            recipe5.AddIngredient(ModContent.ItemType<EmptyFlowerBag>(), 1);
            recipe5.Register();

            Recipe recipe2 = CreateRecipe();
            recipe2.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe2.AddIngredient(ModContent.ItemType<FlowerBatch>(), 3);
            recipe2.AddIngredient(ModContent.ItemType<EmptyFlowerBag>(), 1);
            recipe2.Register();


            Recipe recipe6 = CreateRecipe();
            recipe6.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe6.AddIngredient(ModContent.ItemType<Bagitem>(), 10);
            recipe6.AddIngredient(ModContent.ItemType<FlowerBatch>(), 2);
            recipe6.AddIngredient(ModContent.ItemType<EmptyFlowerBag>(), 1);
            recipe6.AddIngredient(ItemID.Deathweed, 3);
            recipe6.Register();


            Recipe recipe7 = CreateRecipe();
            recipe7.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe7.AddIngredient(ModContent.ItemType<FlowerBatch>(), 1);
            recipe7.AddIngredient(ModContent.ItemType<EmptyFlowerBag>(), 1);
            recipe7.AddIngredient(ItemID.OrangeBloodroot, 1);
            recipe7.AddIngredient(ItemID.BlueBerries, 1);
            recipe7.Register();

            Recipe recipe8 = CreateRecipe();
            recipe8.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe8.AddIngredient(ModContent.ItemType<FlowerBatch>(), 1);
            recipe8.AddIngredient(ModContent.ItemType<EmptyFlowerBag>(), 1);
            recipe8.AddIngredient(ItemID.Seashell, 3);
            recipe8.AddIngredient(ItemID.Feather, 3);
            recipe8.Register();

            Recipe recipe9 = CreateRecipe();
            recipe9.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe9.AddIngredient(ModContent.ItemType<FlowerBatch>(), 1);
            recipe9.AddIngredient(ModContent.ItemType<EmptyFlowerBag>(), 1);
            recipe9.AddIngredient(ItemID.JungleSpores, 30);
            recipe9.Register();




           
        }



    }
}
