
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Placeables
{
    internal class RunicTablel : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Runic Table");
        }


        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 28;
            Item.value = 500;

            Item.maxStack = 99;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;

            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;

            Item.createTile = Mod.Find<ModTile>("RunicTableT").Type;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.WorkBench, 1);
            recipe.AddIngredient(ModContent.ItemType<LostScrap>(), 10);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

    }
}