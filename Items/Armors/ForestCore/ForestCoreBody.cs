using Terraria;

using Terraria.ModLoader;
using Terraria.ID;

using Stellamod.Items.Materials;

namespace Stellamod.Items.Armors.ForestCore
{
    [AutoloadEquip(EquipType.Body)]
    public class ForestCoreBody : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Forest Core Body");
            // Tooltip.SetDefault("Increases ranged crit chance by 2%");
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2;
            Item.vanity = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Ranged) += 0.25f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Wood, 20);
            recipe.AddIngredient(ModContent.ItemType<Ivythorn>(), 11);
            recipe.AddIngredient(ItemID.WoodBreastplate, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }

    }
}
