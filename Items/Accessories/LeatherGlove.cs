
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class LeatherGlove : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Leather Glove");
			// Tooltip.SetDefault("Increases throwing damage and throwing velocity.");
		}

		public override void SetDefaults()
		{
			Item.value = Item.sellPrice(gold: 1);
            Item.width = 26;
            Item.height = 34;
            Item.rare = ItemRarityID.Blue;
            Item.value = 1200;
            Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Throwing) += 0.10f;
            player.ThrownVelocity += 2;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Leather, 1);
            recipe.AddRecipeGroup(nameof(ItemID.IronBar), 2);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
	}
}
