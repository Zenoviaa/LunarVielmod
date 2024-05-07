using Stellamod.Items.Materials;
using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Runes
{
    internal class RuneOfCorsage : ModItem
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
            recipe.AddIngredient(ItemID.Wood, 30);
            recipe.AddIngredient(ModContent.ItemType<Ivythorn>(), 10);
            recipe.AddIngredient(ItemID.Emerald, 5);
            recipe.AddIngredient(ModContent.ItemType<BlankRune>(), 1);
            recipe.AddTile(ModContent.TileType<BroochesTable>());
            recipe.Register();


        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
           
            player.GetModPlayer<MyPlayer>().CorsageRune = true;

        }
    }
}