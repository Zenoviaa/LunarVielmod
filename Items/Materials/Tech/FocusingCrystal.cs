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
