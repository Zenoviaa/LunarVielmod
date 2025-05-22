using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;



namespace Stellamod.Items.Armors.Leather
{
    [AutoloadEquip(EquipType.Body)]
    public class LeatherBody : ModItem
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Leather Vest");
            // Tooltip.SetDefault("Increases throwing damage by 25%");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 4;
        }


        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Ranged) += 4;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Leather, 2);
            recipe.AddIngredient(ModContent.ItemType<Mushroom>(), 6);
            recipe.AddRecipeGroup(nameof(ItemID.IronBar), 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
