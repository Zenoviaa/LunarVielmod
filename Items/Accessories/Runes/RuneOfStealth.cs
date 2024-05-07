using Stellamod.Items.Materials;
using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Runes
{
    internal class RuneOfStealth : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rune of Stealth");
            // Tooltip.SetDefault("You will slowly increase damage until you are hit \n You will glow a bright red when you are at your most powerful form ");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(silver: 75);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Leather, 10);
            recipe.AddRecipeGroup(nameof(ItemID.IronBar), 5);
            recipe.AddIngredient(ModContent.ItemType<BlankRune>(), 1);
            recipe.AddTile(ModContent.TileType<BroochesTable>());
            recipe.Register();
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
           
            player.GetModPlayer<MyPlayer>().StealthRune = true;

        }
    }
}