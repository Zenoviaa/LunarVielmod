using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Astrasilk
{
    [AutoloadEquip(EquipType.Legs)]
    public class AstrasilkLegs : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Astrasilk Boots");
			// Tooltip.SetDefault("Increases movement speed by 20%");
		}

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.2f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<StarSilk>(), 6);
            recipe.AddIngredient(ModContent.ItemType<AuroreanStarI>(), 2);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
