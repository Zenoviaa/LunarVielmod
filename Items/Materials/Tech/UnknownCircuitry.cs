using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
namespace Stellamod.Items.Materials.Tech
{
    public class UnknownCircuitry : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Unknown Circuitry");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddIngredient(ItemType<VirulentPlating>(), 4);
            recipe.AddIngredient(ItemType<LostScrap>(), 2);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

    }

}
