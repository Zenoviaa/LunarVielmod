using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.ForestCore
{
    [AutoloadEquip(EquipType.Legs)]
    public class ForestCoreLegs : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Forest Core Boots");
			// Tooltip.SetDefault("Increases movement speed by 10%");
		}

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 3);
            recipe.AddIngredient(ModContent.ItemType<Ivythorn>(), 2);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
