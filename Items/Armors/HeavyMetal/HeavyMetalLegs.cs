using Stellamod.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Armors.HeavyMetal
{
    [AutoloadEquip(EquipType.Legs)]
    public class HeavyMetalLegs : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("HeavyMetal Legs");
			// Tooltip.SetDefault("Increases movement speed by 10%");
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
            player.moveSpeed *= 1.05f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<GintzlMetal>(), 17);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }


    }
}
