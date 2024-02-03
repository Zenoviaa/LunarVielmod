
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Quest.Zui
{
    internal class CompletedCollectorsBag : ModItem
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
           




            Recipe recipe10 = CreateRecipe();
            recipe10.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe10.AddIngredient(ModContent.ItemType<FlowerBatch>(), 1);
            recipe10.AddIngredient(ModContent.ItemType<GraftedSoul>(), 10);
            recipe10.AddIngredient(ModContent.ItemType<EmptyCollectorsBag>(), 1);
            recipe10.AddIngredient(ItemID.JungleSpores, 20);
            recipe10.Register();

            Recipe recipe11 = CreateRecipe();
            recipe11.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe11.AddIngredient(ModContent.ItemType<FlowerBatch>(), 1);
            recipe11.AddIngredient(ModContent.ItemType<EldritchSoul>(), 10);
            recipe11.AddIngredient(ModContent.ItemType<EmptyCollectorsBag>(), 1);
            recipe11.AddIngredient(ItemID.JungleSpores, 20);
            recipe11.Register();

            Recipe recipe12 = CreateRecipe();
            recipe12.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe12.AddIngredient(ModContent.ItemType<FlowerBatch>(), 1);
            recipe12.AddIngredient(ModContent.ItemType<EmptyCollectorsBag>(), 1);
            recipe12.AddIngredient(ItemID.SoulofFright, 15);
            recipe12.Register();

            Recipe recipe13 = CreateRecipe();
            recipe13.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe13.AddIngredient(ModContent.ItemType<FlowerBatch>(), 3);
            recipe13.AddIngredient(ModContent.ItemType<EmptyCollectorsBag>(), 1);
            recipe13.AddIngredient(ItemID.SoulofLight, 10);
            recipe13.Register();

            Recipe recipe14 = CreateRecipe();
            recipe14.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe14.AddIngredient(ModContent.ItemType<FlowerBatch>(), 3);
            recipe14.AddIngredient(ModContent.ItemType<AlcaricMush>(), 10);
            recipe14.AddIngredient(ModContent.ItemType<EmptyCollectorsBag>(), 1);
            recipe14.AddIngredient(ItemID.Moonglow, 10);
            recipe14.AddIngredient(ModContent.ItemType<STARCORE>(), 1);
            recipe14.Register();

            Recipe recipe15 = CreateRecipe();
            recipe15.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe15.AddIngredient(ModContent.ItemType<FlowerBatch>(), 2);
            recipe15.AddIngredient(ModContent.ItemType<EmptyCollectorsBag>(), 1);
            recipe15.AddIngredient(ItemID.Deathweed, 5);
            recipe15.AddIngredient(ItemID.Waterleaf, 5);
            recipe15.AddIngredient(ItemID.Blinkroot, 15);
            recipe15.AddIngredient(ItemID.SoulofMight, 5);
            recipe15.AddIngredient(ModContent.ItemType<Cinderscrap>(), 50);
            recipe15.Register();

            Recipe recipe16 = CreateRecipe();
            recipe16.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe16.AddIngredient(ModContent.ItemType<FlowerBatch>(), 1);
            recipe16.AddIngredient(ModContent.ItemType<Cinderscrap>(), 30);
            recipe16.AddIngredient(ModContent.ItemType<EmptyCollectorsBag>(), 1);
            recipe16.AddIngredient(ItemID.Hellstone, 5);
            recipe16.AddIngredient(ItemID.Waterleaf, 5);
            recipe16.AddIngredient(ItemID.Fireblossom, 15);
            recipe16.AddIngredient(ItemID.SoulofMight, 5);
            recipe16.Register();

            Recipe recipe17 = CreateRecipe();
            recipe17.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe17.AddIngredient(ModContent.ItemType<FlowerBatch>(), 1);
            recipe17.AddIngredient(ModContent.ItemType<Morrowshroom>(), 15);
            recipe17.AddIngredient(ModContent.ItemType<EmptyCollectorsBag>(), 1);
            recipe17.AddIngredient(ItemID.Bone, 5);
            recipe17.AddIngredient(ItemID.SoulofNight, 5);
            recipe17.AddIngredient(ItemID.SharkFin, 5);
            recipe17.Register();

            Recipe recipe18 = CreateRecipe();
            recipe18.AddTile(ModContent.TileType<AlcaologyTable>());
            recipe18.AddIngredient(ModContent.ItemType<FlowerBatch>(), 3);
            recipe18.AddIngredient(ModContent.ItemType<CondensedDirt>(), 500);
            recipe18.AddIngredient(ModContent.ItemType<EmptyCollectorsBag>(), 1);
            recipe18.AddIngredient(ItemID.WaterBucket, 5);
            recipe18.AddIngredient(ItemID.SoulofSight, 5);
            recipe18.AddIngredient(ItemID.LavaBucket, 5);
            recipe18.Register();
        }



    }
}
