using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Armors.AcidArmour
{
    [AutoloadEquip(EquipType.Legs)]
    public class VirulentLegs : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Acid Legs");
			// Tooltip.SetDefault("Increases Acceleration by 5% and movement speed by 4%");
		}
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 4;
        }

        public override void UpdateEquip(Player player)
        {
            player.maxRunSpeed += 0.04f;
            player.runAcceleration += 0.12f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<VirulentPlating>(), 4);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
