using Stellamod.Items.Materials;
using Stellamod.Tiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Stellamod.Items.Accessories.Runes
{
    internal class RuneOfDetonation : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rune of Detonation");
            // Tooltip.SetDefault("Has a chance to explode enemies doing double damage");
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
            recipe.AddIngredient(ModContent.ItemType<LostScrap>(), 15);
            recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 5);
            recipe.AddIngredient(ModContent.ItemType<ArnchaliteBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<BlankRune>(), 1);
            recipe.AddTile(ModContent.TileType<BroochesTable>());
            recipe.Register();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MyPlayer>().DetonationRune = true;

        }
    }
}