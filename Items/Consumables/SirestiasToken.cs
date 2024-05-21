

using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee.Greatswords;
using Stellamod.Items.Weapons.Melee.Greatswords.INY;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Items.Weapons.Summon.Orbs;
using Stellamod.Items.Weapons.Thrown.Jugglers;
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

            Recipe recipe4 = CreateRecipe();
            recipe4.AddIngredient(ModContent.ItemType<RavestBlast>(), 1);
            recipe4.Register();

            Recipe recipe5 = CreateRecipe();
            recipe5.AddIngredient(ModContent.ItemType<IshNYire>(), 1);
            recipe5.Register();

            Recipe recipe6 = CreateRecipe();
            recipe6.AddIngredient(ModContent.ItemType<StickyCards>(), 1);
            recipe6.Register();

            Recipe recipe7 = CreateRecipe();
            recipe7.AddIngredient(ModContent.ItemType<Mordred>(), 1);
            recipe7.Register();

            Recipe recipe8 = CreateRecipe();
            recipe8.AddIngredient(ModContent.ItemType<SirestiasMask>(), 1);
            recipe8.Register();
        }
    }
}