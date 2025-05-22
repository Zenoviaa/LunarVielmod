using Stellamod.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Armors.HeavyMetal
{
    [AutoloadEquip(EquipType.Body)]
    public class HeavyMetalBody : ModItem
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heavy Metal Vest");
            // Tooltip.SetDefault("Increases throwing damage by 25%");
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<GintzlMetal>(), 22);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
