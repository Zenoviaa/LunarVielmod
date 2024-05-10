using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Materials.Tech
{
    public class MetallicOmniSource : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Metallic Omni Source");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(2);
            recipe.AddIngredient(ItemType<ArncharChunk>(), 1);
            recipe.AddIngredient(ItemType<BrokenTech>(), 2);
            recipe.AddIngredient(ItemType<UnknownCircuitry>(), 2);
            recipe.AddIngredient(ItemType<DriveConstruct>(), 3);
            recipe.AddIngredient(ItemID.CrystalShard, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

    }

}
