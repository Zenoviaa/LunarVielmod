using Stellamod.Items.Materials;
using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Runes
{
    internal class RuneOfShade : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rune of Reality");
            // Tooltip.SetDefault("When you hit an enemy, you will release homing magic bolts that summon mana stars when they hit ");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 2500;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.CrimtaneBar, 15);
            recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 20);
            recipe.AddIngredient(ModContent.ItemType<BlankRune>(), 1);
            recipe.AddTile(ModContent.TileType<RunicTableT>());
            recipe.Register();

            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.DemonBanner, 15);
            recipe2.AddIngredient(ModContent.ItemType<DarkEssence>(), 20);
            recipe2.AddIngredient(ModContent.ItemType<BlankRune>(), 1);
            recipe2.AddTile(ModContent.TileType<RunicTableT>());
            recipe2.Register();
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statManaMax2 += 20;
            Item.defense = 2;
            player.GetModPlayer<MyPlayer>().ShadeRune = true;

        }
    }
}