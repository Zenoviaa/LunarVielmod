using Stellamod.Content.Areas.Underground.TilesUG;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.CommonMaterials;

public class DragonShard : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.rare = ItemRarityID.Orange;
        Item.maxStack = Item.CommonMaxStack;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        Recipe recipe = CreateRecipe(1);
        recipe.AddIngredient(ModContent.ItemType<Dragonpiece>(), 50);
        recipe.Register();
    }
}
