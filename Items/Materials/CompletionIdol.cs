
using Stellamod.Helpers;
using Stellamod.Items.Armors.Vanity.Gothivia;
using Stellamod.Items.Ores;
using Stellamod.Items.Special;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Materials
{
    internal class CompletionIdol : ModItem
    {

        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("This doesn’t look like it will do anything by itself"); // The (English) text shown below your item's name
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
        }

        public override void SetDefaults()
        {
            Item.width = 20; // The item texture's width
            Item.height = 20; // The item texture's height

            Item.maxStack = 1; // The item's max stack value
            Item.value = Item.sellPrice(gold: 50); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
            Item.rare = ModContent.RarityType<SirestiasSpecialRarity>();
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddIngredient(ModContent.ItemType<VeiledScriptureGothivia>(), 1);
            recipe.AddIngredient(ModContent.ItemType<Twirlers>(), 1);
            recipe.Register();

            Recipe recipe2 = CreateRecipe(1);
            recipe2.AddIngredient(ModContent.ItemType<VeiledScriptureCozmire>(), 1);
            recipe2.Register();

            Recipe recipe3 = CreateRecipe(1);
            recipe3.AddIngredient(ModContent.ItemType<VeiledScriptureAzurerin>(), 1);
            recipe3.AddIngredient(ModContent.ItemType<Superfragment>(), 10);
            recipe3.Register();
        }

    }
}
