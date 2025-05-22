using Stellamod.Items.Harvesting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Leather
{
    [AutoloadEquip(EquipType.Legs)]
    public class LeatherLegs : ModItem
    {
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
            player.GetCritChance(DamageClass.Ranged) += 4f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Leather, 1);
            recipe.AddIngredient(ModContent.ItemType<Mushroom>(), 4);
            recipe.AddRecipeGroup(nameof(ItemID.IronBar), 2);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
