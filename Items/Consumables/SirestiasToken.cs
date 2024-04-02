

using Stellamod.Helpers;
using Stellamod.Items.Weapons.Melee.Greatswords;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Items.Weapons.Summon.Orbs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables
{
    public class SirestiasToken : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Electronic Death Remote (EDR)");
            // Tooltip.SetDefault("'that big red button probably will do something you’ll regret... \n Your conscience advises you to press it and see what happens!'");
        }

        public override void SetDefaults()
        {
            Item.maxStack = 1;
            Item.width = 18;
            Item.height = 28;
            Item.rare = ModContent.RarityType<SirestiasSpecialRarity>();
            Item.value = Item.sellPrice(1, 0, 0, 0);
     
        }


        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<ReflectionSeeker>(), 1);
            recipe.Register();


            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ModContent.ItemType<Maelstrom>(), 1);
            recipe2.Register();

            Recipe recipe3 = CreateRecipe();
            recipe3.AddIngredient(ModContent.ItemType<SineSire>(), 1);
            recipe3.Register();
        }
    }
}