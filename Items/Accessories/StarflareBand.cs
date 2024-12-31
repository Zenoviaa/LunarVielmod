using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    internal class StarflareBand : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 2500;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.FallenStar, 5);
            recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 15);
            recipe.AddIngredient(ModContent.ItemType<WanderingFlame>(), 25);
            recipe.AddIngredient(ModContent.ItemType<PearlescentScrap>(), 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) *= 1.1f;
            player.manaCost -= 0.1f;
            player.manaRegen += 1;

        }
    }
}