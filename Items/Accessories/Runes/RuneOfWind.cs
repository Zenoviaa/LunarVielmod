using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Tiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Stellamod.Items.Accessories.Runes
{
    internal class RuneOfWind : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rune of Corsage");
            // Tooltip.SetDefault("Has a chance to gift super healing, if you were hit while under the effects of super healing everything on screen will be poisoned.");
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
            recipe.AddIngredient(ModContent.ItemType<AlcadizScrap>(), 15);
            recipe.AddIngredient(ModContent.ItemType<GintzlMetal>(), 10);
            recipe.AddIngredient(ModContent.ItemType<BlankRune>(), 1);
            recipe.AddTile(ModContent.TileType<BroochesTable>());
            recipe.Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            
            player.GetModPlayer<MyPlayer>().WindRune = true;

        }
    }
}