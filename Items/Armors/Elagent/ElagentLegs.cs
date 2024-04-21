using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Armors.Elagent
{
    [AutoloadEquip(EquipType.Legs)]
    public class ElagentLegs : ModItem
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
            Item.rare = ItemRarityID.Orange;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.5f;
            player.maxMinions += 2;
            player.GetDamage(DamageClass.Summon) *= 1.1f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<StarSilk>(), 9);
            recipe.AddIngredient(ItemType<PearlescentScrap>(), 5);
            recipe.AddIngredient(ItemID.Feather, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
