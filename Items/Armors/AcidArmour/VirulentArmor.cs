using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Armors.AcidArmour
{
    [AutoloadEquip(EquipType.Body)]
    public class VirulentArmor : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("AcidBody");
			// Tooltip.SetDefault("Increases ranged damage by 13% and ranged speed by 10%");
		}

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 80000;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.lifeRegen += 3;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<VirulentPlating>(), 10);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
