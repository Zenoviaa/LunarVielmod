using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials.Tech
{
    internal class FocusingCrystal : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 5, 0, 0);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.CrystalShard, 15);
            recipe.AddIngredient(ModContent.ItemType<MetallicOmniSource>(), 2);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
