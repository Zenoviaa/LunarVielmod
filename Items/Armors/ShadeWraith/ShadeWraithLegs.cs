using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Armors.ShadeWraith
{
    [AutoloadEquip(EquipType.Legs)]
    public class ShadeWraithLegs : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Shade Wraith Legs");
			// Tooltip.SetDefault("Increases melee critical strike chance by 8% and movement speed by 10%");
		}

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.defense = 4;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.3f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<DarkEssence>(), 5);
            recipe.AddRecipeGroup(nameof(ItemID.DemoniteBar), 6);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
